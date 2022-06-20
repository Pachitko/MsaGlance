using TelegramBot.Api.Handlers.Abstractions;
using TelegramBot.Api.UpdateSpecifications;
using TelegramBot.Api.Data.Repositories;
using TelegramBot.Api.Domain.Entities;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.Api.Extensions;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Options;
using TelegramBot.Api.Models;
using System.Threading.Tasks;
using TelegramBot.Api.Domain;
using IdentityModel.Client;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using System.Net.Http;
using Telegram.Bot;
using System.Text;
using System.IO;
using System;

namespace TelegramBot.Api.Handlers;

public class DocumentHandler : IFsmHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserTokenRepository _userTokenRepository;

    public DocumentHandler(IHttpClientFactory httpClientFactory, IUserTokenRepository userTokenRepository)
    {
        _httpClientFactory = httpClientFactory;
        _userTokenRepository = userTokenRepository;
    }

    public void AddTransitionsToFsmOptions(FsmOptions config)
    {
        config.From(GlobalStates.Any).With<DocumentUpdateSpecification>().To<DocumentHandler>();
        config.From(GlobalStates.Any).With<FilesUpdateSpecification>().To<DocumentHandler>();
    }

    public async Task<string> HandleAsync(UpdateContext context)
    {
        long? userId = context.Update.Message?.From?.Id;
        long chatId = context.SafeChatId!.Value;

        UserToken? accessToken = await _userTokenRepository.GetByIdAsync((userId!.Value, "idsrv", IdentityModel.OidcConstants.TokenTypes.AccessToken));
        if (accessToken is null)
        {
            await context.BotClient.SendTextMessageAsync(userId!, "You aren't authenticated, please do it", ParseMode.Html);
            return GlobalStates.Any;
        }

        var diskClient = _httpClientFactory.CreateClient();
        diskClient.BaseAddress = new Uri("https://disk");
        diskClient.SetBearerToken(accessToken.Value);

        switch (context.Key)
        {
            case (GlobalStates.Any, DocumentUpdateSpecification):
                {
                    Document document = context.Update.Message!.Document!;

                    var botFile = await context.BotClient.GetFileAsync(document.FileId);
                    string fileName = Path.GetFileName(document.FileName!);

                    // todo: does not work with MemoryStream
                    await using (var sCreate = System.IO.File.Create(botFile.FileUniqueId))
                    {
                        await context.BotClient.DownloadFileAsync(botFile.FilePath!, sCreate);
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
                        await context.BotClient.SendTextMessageAsync(chatId, "File has been sent", ParseMode.Html);
                    }
                    else
                    {
                        await context.BotClient.SendTextMessageAsync(chatId,
                            $"{response.StatusCode} {await response.Content.ReadAsStringAsync()}", ParseMode.Html);
                    }
                }
                break;
            case (GlobalStates.Any, FilesUpdateSpecification):
                {
                    string[] query = context.Update.Message!.Text!.TrimStart().Split(' ');
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
                            await context.BotClient.SendTextMessageAsync(chatId, responseMessage.ToString(), ParseMode.Html);
                        }
                        else
                        {
                            await context.BotClient.SendTextMessageAsync(chatId,
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
                                await context.BotClient.SendDocumentAsync(chatId, inputOnlineFile);
                            }
                            else
                            {
                                await context.BotClient.SendTextMessageAsync(chatId,
                                    $"{response.StatusCode} {await response.Content.ReadAsStringAsync()}", ParseMode.Html);
                            }
                        }
                    }
                }
                break;
        }

        return GlobalStates.Any;
    }
}