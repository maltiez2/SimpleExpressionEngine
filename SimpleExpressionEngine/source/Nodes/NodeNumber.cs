namespace SimpleExpressionEngine;

internal readonly struct NodeNumber : INode
{
    private readonly double mValue;

    public NodeNumber(double number)
    {
        mValue = number;
    }

    public double Evaluate(IContext context) => mValue;
}
