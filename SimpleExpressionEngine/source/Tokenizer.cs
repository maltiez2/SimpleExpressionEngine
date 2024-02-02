using System.Globalization;
using System.IO;
using System.Text;

namespace SimpleExpressionEngine;

public class FloatTokenizer
{
    private readonly TextReader mReader;
    private char mCurrentCharacter;
    private Token mCurrentToken;
    private float mNumber;
    private string mIdentifier = "";

    public FloatTokenizer(TextReader reader)
    {
        mReader = reader;
        NextChar();
        NextToken();
    }

    public Token Token => mCurrentToken;
    public float Number => mNumber;
    public string Identifier => mIdentifier;

    public void NextToken()
    {
        while (char.IsWhiteSpace(mCurrentCharacter))
        {
            NextChar();
        }

        if (ProcessSpecialSymbol()) return;
        if (ProcessDigit()) return;
        if (ProcessIdentifier()) return;
    }

    private void NextChar()
    {
        int character = mReader.Read();
        mCurrentCharacter = character < 0 ? '\0' : (char)character;
    }

    private bool ProcessSpecialSymbol()
    {
        switch (mCurrentCharacter)
        {
            case '\0':
                mCurrentToken = Token.EOF;
                return true;

            case '+':
                NextChar();
                mCurrentToken = Token.Add;
                return true;

            case '-':
                NextChar();
                mCurrentToken = Token.Subtract;
                return true;

            case '*':
                NextChar();
                mCurrentToken = Token.Multiply;
                return true;

            case '/':
                NextChar();
                mCurrentToken = Token.Divide;
                return true;

            case '(':
                NextChar();
                mCurrentToken = Token.OpenParenthesis;
                return true;

            case ')':
                NextChar();
                mCurrentToken = Token.CloseParenthesis;
                return true;

            case ',':
                NextChar();
                mCurrentToken = Token.Comma;
                return true;
            default:
                return false;
        }
    }

    private bool ProcessDigit()
    {
        if (!char.IsDigit(mCurrentCharacter) && mCurrentCharacter != '.') return false;

        StringBuilder numberString = new();
        bool haveDecimalPoint = false;
        while (char.IsDigit(mCurrentCharacter) || (!haveDecimalPoint && mCurrentCharacter == '.'))
        {
            numberString.Append(mCurrentCharacter);
            haveDecimalPoint = mCurrentCharacter == '.';
            NextChar();
        }

        mNumber = float.Parse(numberString.ToString(), CultureInfo.InvariantCulture);
        mCurrentToken = Token.Number;
        return true;
    }

    private bool ProcessIdentifier()
    {
        if (!char.IsLetter(mCurrentCharacter) && mCurrentCharacter != '_') return false;

        StringBuilder identifier = new();

        while (char.IsLetterOrDigit(mCurrentCharacter) || mCurrentCharacter == '_')
        {
            identifier.Append(mCurrentCharacter);
            NextChar();
        }

        mIdentifier = identifier.ToString();
        mCurrentToken = Token.Identifier;
        return true;
    }
}
