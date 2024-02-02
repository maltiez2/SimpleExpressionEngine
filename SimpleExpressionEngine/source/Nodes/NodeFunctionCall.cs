using System.Collections.Generic;
using System.Linq;

namespace SimpleExpressionEngine;

internal sealed class NodeFunctionCall : INode
{
    private readonly INode[] mArguments;
    private readonly string mFunctionName;

    public NodeFunctionCall(string functionName, INode[] arguments)
    {
        mFunctionName = functionName;
        mArguments = arguments;
    }

    public double Evaluate(IContext context)
    {
        IEnumerable<double> argumentsValues = mArguments.Select(argument => argument.Evaluate(context));

        return context.CallFunction(mFunctionName, argumentsValues);
    }
}
