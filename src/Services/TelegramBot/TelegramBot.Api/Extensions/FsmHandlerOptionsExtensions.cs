using TelegramBot.Api.UpdateSpecifications.Abstractions;
using TelegramBot.Api.Handlers.Abstractions;
using TelegramBot.Api.Options;

namespace TelegramBot.Api.Extensions;

public static class FsmHandlerOptionsExtensions
{
    public static TransitionBuilder From(this FsmOptions options, string fromState) => new(fromState, options);

    public readonly record struct TransitionBuilder(string FromState, FsmOptions Options)
    {
        public TransitionWithSpecificationBuilder<TSpecification> With<TSpecification>()
            where TSpecification : IFsmSpecification
                => new(FromState, Options);

        public readonly record struct TransitionWithSpecificationBuilder<TSpecification>(string FromState, FsmOptions Options)
            where TSpecification : IFsmSpecification
        {
            public void To<THandler>()
                where THandler : IFsmHandler
            {
                Options.Add<TSpecification, THandler>(FromState);
            }
        }
    }
}