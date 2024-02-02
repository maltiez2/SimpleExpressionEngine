using System;
using System.Collections.Generic;

namespace SimpleExpressionEngine;

public interface IContext
{
    double ResolveVariable(string name);
    double CallFunction(string name, IEnumerable<double> arguments);
}

public interface INode
{
    double Evaluate(IContext context);
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