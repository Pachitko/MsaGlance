using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Domain.Entities;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.Api.Attribute;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Filters;
using TelegramBot.Api.Domain;
using IdentityModel.Client;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Telegram.Bot;
using System.Text;

namespace TelegramBot.Api.Controllers;

[ApiController]
[TypeFilter(typeof(BotStateActionFilterAttribute))]
public class FilesController : BotControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserTokenRepository _userTokenRepository;

    public FilesController(
        IHttpClientFactory httpClientFactory,
        IUserTokenRepository userTokenRepository)
    {
        _httpClientFactory = httpClientFactory;
        _userTokenRepository = userTokenRepository;
    }

    [BotDocumentPost(BotDefaults.AnyState)]
    public async Task<string> PostDocument([FromBody] Document document)
    {
        long userId = BotContext.UserId;
        long chatId = BotContext.SafeChatId!.Value;

        UserToken? accessToken = await _userTokenRepository.GetByIdAsync((userId, "idsrv", IdentityModel.OidcConstants.TokenTypes.AccessToken));
        if (accessToken is null)
        {
            await BotContext.BotClient.SendTextMessageAsync(userId!, "You aren't authenticated, please do it", ParseMode.Html);
            return BotDefaults.AnyState;
        }

        var diskClient = _httpClientFactory.CreateClient();
        diskClient.BaseAddress = new Uri("https://disk");
        diskClient.SetBearerToken(accessToken.Value);

        var botFile = await BotContext.BotClient.GetFileAsync(document.FileId);
        string fileName = Path.GetFileName(document.FileName!);

        // todo: does not work with MemoryStream
        await using (var sCreate = System.IO.File.Create(botFile.FileUniqueId))
        {
            await BotContext.BotClient.DownloadFileAsync(botFile.FilePath!, sCreate);
        }

        await using var sRead = System.IO.File.OpenRead(botFile.FileUniqueId);

        using StreamContent streamContent = new(sRead);
        using MultipartFormDataContent content = new()
                    {
                        { streamContent, "file", fileName }
                    };

        using HttpRequestMessage request = new(HttpMethod.Post, "/files")
        {
            Content = content
        };

        var response = await diskClient.SendAsync(request);
        System.IO.File.Delete(botFile.FileUniqueId);

        if (response.IsSuccessStatusCode)
        {
            await BotContext.BotClient.SendTextMessageAsync(chatId, "File has been sent", ParseMode.Html);
        }
        else
        {
            await BotContext.BotClient.SendTextMessageAsync(chatId,
                $"{response.StatusCode} {await response.Content.ReadAsStringAsync()}", ParseMode.Html);
        }
        return BotDefaults.AnyState;
    }

    [BotCommandGet(BotDefaults.AnyState, "files")]
    public async Task<string> GetFiles()
    {
        long? userId = BotContext.Input!.Message?.From?.Id;
        long chatId = BotContext.SafeChatId!.Value;

        UserToken? accessToken = await _userTokenRepository.GetByIdAsync((userId!.Value, "idsrv", IdentityModel.OidcConstants.TokenTypes.AccessToken));
        if (accessToken is null)
        {
            await BotContext.BotClient.SendTextMessageAsync(userId!, "You aren't authenticated, please do it", ParseMode.Html);
            return BotDefaults.AnyState;
        }

        var diskClient = _httpClientFactory.CreateClient();
        diskClient.BaseAddress = new Uri("https://disk");
        diskClient.SetBearerToken(accessToken.Value);

        string[] query = BotContext.Input!.Message!.Text!.TrimStart().Split(' ');
        if (query.Length == 1)
        {
            var response = await diskClient.GetAsync("/files");

            if (response.IsSuccessStatusCode)
            {
                var fileNames = JsonConvert.DeserializeObject<IEnumerable<string>>(await response.Content!.ReadAsStringAsync());
                StringBuilder responseMessage = new();
                int i = 1;
                responseMessage.AppendLine("ID - File name");
                foreach (var fileName in fileNames!)
                {
                    responseMessage.AppendLine($"{i} - {fileName}");
                    i++;
                }
                // responseMessage.AppendLine(@"Print <i>\/files \{ID\}</i> or <i>\/files \{File name\}<i>");
                await BotContext.BotClient.SendTextMessageAsync(chatId, responseMessage.ToString(), ParseMode.Html);
            }
            else
            {
                await BotContext.BotClient.SendTextMessageAsync(chatId,
                    $"{response.StatusCode} {await response.Content.ReadAsStringAsync()}", ParseMode.Html);
            }
        }
        else if (query.Length > 1)
        {
            for (int i = 1; i < query.Length; i++)
            {
                var response = await diskClient.GetAsync($"/files/{query[i]}");
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    InputOnlineFile inputOnlineFile = new(stream, "qwe");
                    await BotContext.BotClient.SendDocumentAsync(chatId, inputOnlineFile);
                }
                else
                {
                    await BotContext.BotClient.SendTextMessageAsync(chatId,
                        $"{response.StatusCode} {await response.Content.ReadAsStringAsync()}", ParseMode.Html);
                }
            }
        }

        return BotDefaults.AnyState;
    }
}