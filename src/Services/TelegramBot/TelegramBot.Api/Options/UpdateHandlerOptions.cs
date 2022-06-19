using TelegramBot.Api.UpdateSpecifications.Abstractions;
using TelegramBot.Api.Handlers.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System;

namespace TelegramBot.Api.Options;

public class KeyComparer : IEqualityComparer<(string s, Type t)>
{
    private readonly EqualityComparer<(string, Type)> _defaultEqualityComparer = EqualityComparer<(string, Type)>.Default;
    public bool Equals((string s, Type t) x, (string s, Type t) y)
    {
        return string.Equals(x.s, y.s, StringComparison.OrdinalIgnoreCase) && x.t == y.t;
    }

    public int GetHashCode([DisallowNull] (string s, Type t) obj)
    {
        return _defaultEqualityComparer.GetHashCode((obj.s.ToUpper(), obj.t));
    }
}

public class UpdateHandlerOptions
{
    // currentState + specification = newState (set by the handler)
    private readonly Dictionary<(string, Type), Type> _transitions = new(new KeyComparer());

    public Type? Get<TSpecification>(string fromState, TSpecification specification)
        where TSpecification : IUpdateSpecification
            => _transitions.GetValueOrDefault((fromState, specification.GetType()));

    public void Add<TSpecification, THandler>(string fromState)
        where TSpecification : IUpdateSpecification
        where THandler : IUpdateHandler
            => _transitions.Add((fromState, typeof(TSpecification)), typeof(THandler));
}
