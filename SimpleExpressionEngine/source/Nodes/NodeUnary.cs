using System;

namespace SimpleExpressionEngine;

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
