using System;

namespace SimpleExpressionEngine;

internal sealed class NodeBinary : INode
{
    private readonly INode mLeftSide;
    private readonly INode mRightSide;
    private readonly Func<double, double, double> mOperation;

    public NodeBinary(INode leftSide, INode rightSide, Func<double, double, double> operation)
    {
        mLeftSide = leftSide;
        mRightSide = rightSide;
        mOperation = operation;
    }

    public double Evaluate(IContext contex)
    {
        double leftSideValue = mLeftSide.Evaluate(contex);
        double rightSideValue = mRightSide.Evaluate(contex);
        double result = mOperation(leftSideValue, rightSideValue);
        return result;
    }
}
