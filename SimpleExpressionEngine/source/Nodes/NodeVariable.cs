namespace SimpleExpressionEngine;

internal sealed class NodeVariable : INode
{
    private readonly string mVariableName;

    public NodeVariable(string variableName)
    {
        mVariableName = variableName;
    }

    public double Evaluate(IContext context) => context.ResolveVariable(mVariableName);
}
