using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.FSM.Abstractions;
using TelegramBot.Api.Domain.Entities;
using TelegramBot.Api.FSM.Attributes;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.Api.FSM.Models;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Domain;
using IdentityModel.Client;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Telegram.Bot;
using System.Text;

namespace TelegramBot.Api.FSM.Handlers;

public class DocumentHandler : BaseFsmHandler<BotFsmContext, Update>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserTokenRepository _userTokenRepository;

    public DocumentHandler(IHttpClientFactory httpClientFactory, IUserTokenRepository userTokenRepository,
    IContextAccessor<BotFsmContext, Update> updateContextAccessor) : base(updateContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _userTokenRepository = userTokenRepository;
    }

    [WithDocument(GlobalStates.Any)]
    public async Task<string> RecieveDocument()
    {
        long? userId = Context.Input.Message?.From?.Id;
        long chatId = Context.SafeChatId!.Value;

        UserToken? accessToken = await _userTokenRepository.GetByIdAsync((userId!.Value, "idsrv", IdentityModel.OidcConstants.TokenTypes.AccessToken));
        if (accessToken is null)
        {
            await Context.BotClient.SendTextMessageAsync(userId!, "You aren't authenticated, please do it", ParseMode.Html);
            return GlobalStates.Any;
        }

        var diskClient = _httpClientFactory.CreateClient();
        diskClient.BaseAddress = new Uri("https://disk");
        diskClient.SetBearerToken(accessToken.Value);

        Document document = Context.Input.Message!.Document!;

        var botFile = await Context.BotClient.GetFileAsync(document.FileId);
        string fileName = Path.GetFileName(document.FileName!);

        // todo: does not work with MemoryStream
        await using (var sCreate = System.IO.File.Create(botFile.FileUniqueId))
        {
            await Context.BotClient.DownloadFileAsync(botFile.FilePath!, sCreate);
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
            await Context.BotClient.SendTextMessageAsync(chatId, "File has been sent", ParseMode.Html);
        }
        else
        {
            await Context.BotClient.SendTextMessageAsync(chatId,
                $"{response.StatusCode} {await response.Content.ReadAsStringAsync()}", ParseMode.Html);
        }
        return GlobalStates.Any;
    }

    [SlashCommand(GlobalStates.Any, "files", "Show files")]
    public async Task<string> GetFiles()
    {
        long? userId = Context.Input.Message?.From?.Id;
        long chatId = Context.SafeChatId!.Value;

        UserToken? accessToken = await _userTokenRepository.GetByIdAsync((userId!.Value, "idsrv", IdentityModel.OidcConstants.TokenTypes.AccessToken));
        if (accessToken is null)
        {
            await Context.BotClient.SendTextMessageAsync(userId!, "You aren't authenticated, please do it", ParseMode.Html);
            return GlobalStates.Any;
        }

        var diskClient = _httpClientFactory.CreateClient();
        diskClient.BaseAddress = new Uri("https://disk");
        diskClient.SetBearerToken(accessToken.Value);

        string[] query = Context.Input.Message!.Text!.TrimStart().Split(' ');
        if (query.Length == 1)
        {
            var response = await diskClient.GetAsync("/files");

            if (response.IsSuccessStatusCode)
            {
                var fileNames = JsonConvert.DeserializeObject<IEnumerable<string>>(await response.Content!.ReadAsStringAsync());
                StringBuilder responseMessage = new();
                int i = 1;
                responseMessage.AppendLine("ID - File name");
                foreach (var fileName in fileNames)
                {
                    responseMessage.AppendLine($"{i} - {fileName}");
                    i++;
                }
                // responseMessage.AppendLine(@"Print <i>\/files \{ID\}</i> or <i>\/files \{File name\}<i>");
                await Context.BotClient.SendTextMessageAsync(chatId, responseMessage.ToString(), ParseMode.Html);
            }
            else
            {
                await Context.BotClient.SendTextMessageAsync(chatId,
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
                    await Context.BotClient.SendDocumentAsync(chatId, inputOnlineFile);
                }
                else
                {
                    await Context.BotClient.SendTextMessageAsync(chatId,
                        $"{response.StatusCode} {await response.Content.ReadAsStringAsync()}", ParseMode.Html);
                }
            }
        }

        return GlobalStates.Any;
    }
}