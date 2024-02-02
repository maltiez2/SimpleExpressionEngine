using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleExpressionEngine;

public class FloatParser
{
    private readonly FloatTokenizer mTokenizer;

    public FloatParser(FloatTokenizer tokenizer)
    {
        mTokenizer = tokenizer;
    }

    public INode<float, float, float> ParseExpression()
    {
        INode<float, float, float> expression = ParseAddSubtract();

        if (mTokenizer.Token != Token.EOF)
        {
            throw new SyntaxException("Unexpected characters at end of expression");
        }

        return expression;
    }

    INode<float, float, float> ParseAddSubtract()
    {
        INode<float, float, float> leftOperand = ParseMultiplyDivide();

        while (true)
        {
            Func<float, float, float>? operation = null;
            if (mTokenizer.Token == Token.Add)
            {
                operation = (a, b) => a + b;
            }
            else if (mTokenizer.Token == Token.Subtract)
            {
                operation = (a, b) => a - b;
            }

            if (operation == null) return leftOperand;

            mTokenizer.NextToken();
            
            INode<float, float, float> rightOperand = ParseMultiplyDivide();

            leftOperand = new Nodes.Binary<float, float, float>(leftOperand, rightOperand, operation);
        }
    }

    // Parse an sequence of add/subtract operators
    INode<float, float, float> ParseMultiplyDivide()
    {
        // Parse the left hand side
        INode<float, float, float> lhs = ParseUnary();

        while (true)
        {
            // Work out the operator
            Func<float, float, float> op = null;
            if (mTokenizer.Token == Token.Multiply)
            {
                op = (a, b) => a * b;
            }
            else if (mTokenizer.Token == Token.Divide)
            {
                op = (a, b) => a / b;
            }

            // Binary operator found?
            if (op == null)
                return lhs;             // no

            // Skip the operator
            mTokenizer.NextToken();

            // Parse the right hand side of the expression
            INode<float, float, float> rhs = ParseUnary();

            // Create a binary node and use it as the left-hand side from now on
            lhs = new Nodes.Binary<float, float, float>(lhs, rhs, op);
        }
    }


    // Parse a unary operator (eg: negative/positive)
    INode<float, float, float> ParseUnary()
    {
        while (true)
        {
            // Positive operator is a no-op so just skip it
            if (mTokenizer.Token == Token.Add)
            {
                // Skip
                mTokenizer.NextToken();
                continue;
            }

            // Negative operator
            if (mTokenizer.Token == Token.Subtract)
            {
                // Skip
                mTokenizer.NextToken();

                // Parse RHS 
                // Note this recurses to self to support negative of a negative
                INode<float, float, float> rhs = ParseUnary();

                // Create unary node
                return new Nodes.Unary<float, float, float>(rhs, (a) => -a);
            }

            // No positive/negative operator so parse a leaf node
            return ParseLeaf();
        }
    }

    // Parse a leaf node
    // (For the moment this is just a number)
    INode<float, float, float> ParseLeaf()
    {
        // Is it a number?
        if (mTokenizer.Token == Token.Number)
        {
            var node = new Nodes.Value<float, float, float>(mTokenizer.Number);
            mTokenizer.NextToken();
            return node;
        }

        // Parenthesis?
        if (mTokenizer.Token == Token.OpenParenthesis)
        {
            // Skip '('
            mTokenizer.NextToken();

            // Parse a top-level expression
            INode<float, float, float> node = ParseAddSubtract();

            // Check and skip ')'
            if (mTokenizer.Token != Token.CloseParenthesis)
                throw new SyntaxException("Missing close parenthesis");
            mTokenizer.NextToken();

            // Return
            return node;
        }

        // Variable
        if (mTokenizer.Token == Token.Identifier)
        {
            // Capture the name and skip it
            string name = mTokenizer.Identifier;
            mTokenizer.NextToken();

            // Parens indicate a function call, otherwise just a variable
            if (mTokenizer.Token != Token.OpenParenthesis)
            {
                // Variable
                return new Nodes.Variable<float, float>(name);
            }
            else
            {
                // Function call

                // Skip parens
                mTokenizer.NextToken();

                // Parse arguments
                List<SimpleExpressionEngine.INode<float, float, float>> arguments = new();
                while (true)
                {
                    // Parse argument and add to list
                    arguments.Add(ParseAddSubtract());

                    // Is there another argument?
                    if (mTokenizer.Token == Token.Comma)
                    {
                        mTokenizer.NextToken();
                        continue;
                    }

                    // Get out
                    break;
                }

                // Check and skip ')'
                if (mTokenizer.Token != Token.CloseParenthesis)
                    throw new SyntaxException("Missing close parenthesis");
                mTokenizer.NextToken();

                // Create the function call node
                return new Nodes.FunctionCall<float, float>(name, arguments.ToArray());
            }
        }

        // Don't Understand
        throw new SyntaxException($"Unexpect token: {mTokenizer.Token}");
    }


    #region Convenience Helpers

    // Static helper to parse a string
    public static INode<float, float, float> Parse(string str)
    {
        return Parse(new FloatTokenizer(new StringReader(str)));
    }

    // Static helper to parse from a tokenizer
    public static INode<float, float, float> Parse(FloatTokenizer tokenizer)
    {
        FloatParser parser = new(tokenizer);
        return parser.ParseExpression();
    }

    #endregion
}
