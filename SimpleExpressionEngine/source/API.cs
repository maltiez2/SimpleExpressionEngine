﻿using System;

namespace SimpleExpressionEngine;

public interface IContext<out TResult, in TArguments>
{
    TResult Resolve(string name, params TArguments[] arguments);
}

public interface INode<TOutput, TIntermediate, TInput>
{
    TOutput Evaluate(IContext<TIntermediate, TInput> context);
}

public enum Token
{
    EOF,
    Add,
    Subtract,
    Multiply,
    Divide,
    OpenParenthesis,
    CloseParenthesis,
    Comma,
    Identifier,
    Number,
}

public class SyntaxException : Exception
{
    public SyntaxException(string message)
        : base(message)
    {
    }
}