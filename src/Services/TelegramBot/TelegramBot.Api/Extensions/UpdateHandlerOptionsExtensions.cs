using TelegramBot.Api.UpdateSpecifications.Abstractions;
using TelegramBot.Api.Handlers.Abstractions;
using TelegramBot.Api.Options;

namespace TelegramBot.Api.Extensions;

public static class UpdateHandlerOptionsExtensions
{
    public static TransitionBuilder From(this UpdateHandlerOptions options, string fromState) => new(fromState, options);

    public readonly record struct TransitionBuilder(string FromState, UpdateHandlerOptions Options)
    {
        public TransitionWithSpecificationBuilder<TSpecification> With<TSpecification>() 
            where TSpecification : IUpdateSpecification
                => new(FromState, Options);

        public readonly record struct TransitionWithSpecificationBuilder<TSpecification>(string FromState, UpdateHandlerOptions Options)
            where TSpecification : IUpdateSpecification
        {
            public void To<THandler>()
                where THandler : IUpdateHandler
            {
                Options.Add<TSpecification, THandler>(FromState);
            }
        }
    }
}