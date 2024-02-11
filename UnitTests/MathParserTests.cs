using Newtonsoft.Json.Linq;
using System;

namespace SimpleExpressionEngine.UnitTests;

[TestClass]
public class MathParserTests
{
    private static readonly Dictionary<string, float> mConstants = new()
    {
        { "11.2", 11.2f },
        { "10 + 12.2", 22.2f },
        { "10 * 12.2", 122f },
        { "10 - 12.2", -2.2f },
        { "10 * -12.2", -122f },
        { "12.2 / 10", 1.22f },
        { "12.2 / -10", -1.22f }
    };
    [TestMethod]
    public void MathConstants()
    {
        MathContext context = new();

        foreach ((string formula, float result) in mConstants)
        {
            INode<float, float, float> node = MathParser.Parse(formula);
            float evaluated = node.Evaluate(context);
            Assert.AreEqual(result, evaluated, 0.001, $"\nConstant '{formula}' has evaluated to '{evaluated}', but expected value is '{result}'");
        }
    }

    private static readonly Dictionary<Func<float, string>, Func<float, float>> mFunctions = new()
    {
        { _ => "pi", _ => MathF.PI },
        { _ => "e", _ => MathF.E },
        { value => $"sin({value})", MathF.Sin },
        { value => $"cos({value})", MathF.Cos },
        { value => $"abs({value})", MathF.Abs },
        { value => $"sqrt({value})", MathF.Sqrt },
        { value => $"ceiling({value})", MathF.Ceiling },
        { value => $"floor({value})", MathF.Floor },
        { value => $"exp({value})", MathF.Exp },
        { value => $"log({value})", MathF.Log },
        { value => $"round({value})", MathF.Round },
        { value => $"sign({value})", value => MathF.Sign(value) },

        { value => $"clamp({value}, 0, 1)", value => Math.Clamp(value, 0, 1) },
        { value => $"max({value, 0}, 0)", value =>  MathF.Max(value, 0) },
        { value => $"min({value, 0}, 0)", value =>  MathF.Min(value, 0) },

        { value => $"greater({value}, 0, 0, 1)", value => value > 0 ? 0 : 1 },
        { value => $"lesser({value}, 0, 0, 1)", value => value < 0 ? 0 : 1 },
        { value => $"equal({value}, 0, 0, 1)", value => MathF.Abs(value - 0) < MathF.Max(1E-15f, 1E-15f * MathF.Min(value, 0)) ? 0 : 1 },
        { value => $"notequal({value}, 0, 0, 1)", value => MathF.Abs(value - 0) > MathF.Max(1E-15f, 1E-15f * MathF.Min(value, 0)) ? 0 : 1 }
    };
    private static readonly List<float> mValues = new()
    {
        0,
        1,
        -1,
        MathF.PI,
        MathF.E,
        1E-14f,
        1E14f,
        -1E-14f,
        -1E14f,
        0.5f,
        -0.5f,
        1.23456f,
        -1.23456f
    };
    [TestMethod]
    public void MathFunctions()
    {
        MathContext context = new();

        foreach ((Func<float, string> formulaGetter, Func<float, float> resultGetter) in mFunctions)
        {
            foreach (float value in mValues)
            {
                float result = resultGetter(value);
                string formula = formulaGetter(value);

                try
                {
                    INode<float, float, float> node = MathParser.Parse(formula);
                    float evaluated = node.Evaluate(context);
                    if (float.IsNaN(evaluated) && float.IsNaN(result)) continue;
                    Assert.AreEqual(result, evaluated, 0.001, $"\nFunction '{formula}' has evaluated to '{evaluated}', but expected value is '{result}'");
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Exception on evaluating/parsing '{formula}'.\n{ex.Message}");
                }
            }
        }
    }

    private class ReflectionTest
    {
        private float mValue;
        
        public float publicField;
        protected float protectedField;
        private float privateField;

        public float publicProperty { get; set; }
        protected float protectedProperty { get; set; }
        private float privateProperty { get; set; }

        public float publicMethod() => mValue;
        protected float protectedMethod() => mValue;
        private float privateMethod() => mValue;

        public float publicMethodArgs(float value) => mValue;
        protected float protectedMethodArgs(float value) => mValue;
        private float privateMethodArgs(float value) => mValue;

        public ReflectionTest(float value)
        {
            mValue = value;
            publicField = value;
            protectedField = value;
            privateField = value;
            publicProperty = value;
            protectedProperty = value;
            privateProperty = value;
        }
    }
    private static readonly List<string> mReflectionFormulas = new()
    {
        "publicField",
        "protectedField",
        "privateField",

        "publicProperty",
        "protectedProperty",
        "privateProperty",

        "publicMethod",
        "protectedMethod",
        "privateMethod",

        "publicMethodArgs(0)",
        "protectedMethodArgs(0)",
        "privateMethodArgs(0)",
    };
    [TestMethod]
    public void ReflectionContext()
    {
        foreach (float value in mValues)
        {
            ReflectionContext<float, float> context = new(new ReflectionTest(value));

            foreach (string formula in mReflectionFormulas)
            {
                INode<float, float, float> node = MathParser.Parse(formula);
                float evaluated = node.Evaluate(context);
                Assert.AreEqual(value, evaluated, 0.001, $"\nReflection formula '{formula}' has evaluated to '{evaluated}', but expected value is '{value}'");
            }
        }
    }
}