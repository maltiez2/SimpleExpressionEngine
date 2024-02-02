using System;
using System.Linq;

namespace SimpleExpressionEngine;

internal sealed class NodeBinary : INode
{
    private readonly INode mLeftOperand;
    private readonly INode mRightOperand;
    private readonly Func<double, double, double> mOperation;

    public NodeBinary(INode leftOperand, INode rightOperand, Func<double, double, double> operation)
    {
        mLeftOperand = leftOperand;
        mRightOperand = rightOperand;
        mOperation = operation;
    }

    public double Evaluate(IContext context) => mOperation(mLeftOperand.Evaluate(context), mRightOperand.Evaluate(context));
}

internal sealed class NodeFunctionCall : INode
{
    private readonly INode[] mArguments;
    private readonly string mFunctionName;

    public NodeFunctionCall(string functionName, INode[] arguments)
    {
        mFunctionName = functionName;
        mArguments = arguments;
    }

    public double Evaluate(IContext context) => context.CallFunction(mFunctionName, mArguments.Select(argument => argument.Evaluate(context)));
}

internal readonly struct NodeNumber : INode
{
    private readonly double mValue;

    public NodeNumber(double number)
    {
        mValue = number;
    }

    public double Evaluate(IContext context) => mValue;
}

internal sealed class NodeUnary : INode
{
    private readonly INode mOperand;
    private readonly Func<double, double> mOperation;

    public NodeUnary(INode operand, Func<double, double> operation)
    {
        mOperand = operand;
        mOperation = operation;
    }

    public double Evaluate(IContext context) => mOperation.Invoke(mOperand.Evaluate(context));
}

internal sealed class NodeVariable : INode
{
    private readonly string mVariableName;

    public NodeVariable(string variableName)
    {
        mVariableName = variableName;
    }

    public double Evaluate(IContext context) => context.ResolveVariable(mVariableName);
}
