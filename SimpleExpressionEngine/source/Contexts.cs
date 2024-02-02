using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Vintagestory.API.Common;

namespace SimpleExpressionEngine;

public class ChainedContext<TResult, TArguments> : IContext<TResult, TArguments>
{
    public TResult Resolve(string name, params TArguments[] arguments) => throw new NotImplementedException();
}

public sealed class MathContext : IContext<float, float>
{
    private const double cEpsilon = 1E-15;

    public MathContext()
    {
    }

    public float Resolve(string name, params float[] arguments)
    {
        return name switch
        {
            "pi" => MathF.PI,
            "e" => MathF.E,
            "sin" => MathF.Sin(arguments[0]),
            "cos" => MathF.Cos(arguments[0]),
            "abs" => MathF.Abs(arguments[0]),
            "sqrt" => MathF.Sqrt(arguments[0]),
            "ceiling" => MathF.Ceiling(arguments[0]),
            "floor" => MathF.Floor(arguments[0]),
            "exp" => MathF.Exp(arguments[0]),
            "log" => MathF.Log(arguments[0]),
            "round" => MathF.Round(arguments[0]),
            "sign" => MathF.Sign(arguments[0]),
            "clamp" => Math.Clamp(arguments[0], arguments[1], arguments[2]),
            "max" => MathF.Max(arguments[0], arguments[1]),
            "min" => MathF.Min(arguments[0], arguments[1]),
            "greater" => arguments[0] > arguments[1] ? arguments[2] : arguments[3],
            "lesser" => arguments[0] < arguments[1] ? arguments[2] : arguments[3],
            "equal" => MathF.Abs(arguments[0] - arguments[1]) < cEpsilon * MathF.Min(arguments[0], arguments[1]) ? arguments[2] : arguments[3],
            _ => throw new InvalidDataException($"Unknown function: '{name}'")
        };
    }
}

public sealed class ReflectionContext<TResult, TArguments> : IContext<TResult, TArguments>
{
    private readonly object mSource;

    public ReflectionContext(object source)
    {
        mSource = source;
    }

    public TResult Resolve(string name, params TArguments[] arguments)
    {
        if (arguments.Length != 0) return CallFunction(name, arguments) ?? throw new InvalidDataException($"Unknown function: '{name}'");

        return ResolveProperty(name) ?? ResolveField(name) ?? CallFunction(name, arguments) ?? throw new InvalidDataException($"Unknown function, property or field: '{name}'");
    }

    private TResult? ResolveProperty(string name)
    {
        PropertyInfo? property = mSource.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        return (TResult?)property?.GetValue(mSource);
    }

    private TResult? ResolveField(string name)
    {
        FieldInfo? field = mSource.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        return (TResult?)field?.GetValue(mSource);
    }

    public TResult? CallFunction(string name, params TArguments[] arguments)
    {
        MethodInfo? methodInfo = mSource.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        return (TResult?)methodInfo?.Invoke(mSource, arguments.Select(value => (object?)value).ToArray());
    }
}

public sealed class StatsContext<TArguments> : IContext<float, TArguments>
{
    private readonly IPlayer mPlayer;

    public StatsContext(IPlayer player)
    {
        mPlayer = player;
    }

    public float Resolve(string name, params TArguments[] arguments) => mPlayer.Entity.Stats.GetBlended(name);
}