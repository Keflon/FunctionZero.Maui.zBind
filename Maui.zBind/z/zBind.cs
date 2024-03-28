using FunctionZero.ExpressionParserZero.Exceptions;
using FunctionZero.ExpressionParserZero.Operands;
using FunctionZero.ExpressionParserZero.Tokens;
using System.Diagnostics;
using System.Xml;

[assembly: Microsoft.Maui.Controls.XmlnsPrefix("FunctionZero.Maui.zBind", "zero")]

namespace FunctionZero.Maui.zBind.z
{
    [ContentProperty("Expression")]
    public class Bind : IMarkupExtension<BindingBase>
    {
        public string Expression { set; get; }
        public BindingMode Mode { get; set; }

        private IList<string> _bindingLookup;

        public object Source { get; set; }

        /// <summary>
        /// For internal use.
        /// </summary>
        public object ConstantResult { get; set; }

        public Bind()
        {
        }

        private MultiBinding _multiBind;

        internal BindableObject BindableTarget { get; private set; }

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            _bindingLookup = new List<string>();

            if (string.IsNullOrEmpty(Expression))
            {
                IXmlLineInfo lineInfo = serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
                throw new XamlParseException("ZeroBind requires 'Expression' property to be set", lineInfo);
            }

            object bindingSourceObject = Source;

            var ep = ExpressionParserZero.Binding.ExpressionParserFactory.GetExpressionParser();

            try
            {
                _multiBind = new MultiBinding();
                
                var compiledExpression = ep.Parse(Expression);

                foreach (IToken item in compiledExpression.RpnTokens)
                {
                    if (item is Operand op)
                    {
                        if (op.Type == OperandType.Variable)
                        {
                            if (_bindingLookup.Contains(op.ToString()) == false)
                            {
                                var binding = new Binding(op.ToString(), BindingMode.OneWay, null, null, null, bindingSourceObject);
                                _bindingLookup.Add(op.ToString());
                                _multiBind.Bindings.Add(binding);
                            }
                        }
                    }
                }
                _multiBind.Converter = new EvaluatorMultiConverter(_bindingLookup, compiledExpression, this);

                if (_bindingLookup.Count == 0)
                {
                    // The expression is a constant, so there is nothing to bind to. Evaluate it and return a suitable dummy Binding.
                    var stack = compiledExpression.Evaluate(null);
                    if (this is MultiBind == false)
                    {
                        var operand = stack.Pop();
                        ConstantResult = operand.GetValue();
                    }
                    else
                    {
                        var result = new List<object>();
                        while (stack.Count > 0)
                            result.Add(stack.Pop().GetValue());
                        ConstantResult = result;
                    }

                    return new Binding(nameof(ConstantResult), BindingMode.OneTime, null, null, null, this);
                }

                return _multiBind;
            }
            catch (ExpressionParserException ex)
            {
                IXmlLineInfo lineInfo = serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
                string problem =
                    $"z:Bind exception at line {lineInfo.LineNumber}, Column {lineInfo.LinePosition + ex.Offset}: " + Environment.NewLine +
                    $"Expression '{Expression}' error at offset {ex.Offset} - " +
                    ex.Message + Environment.NewLine +
                    "If your expression contains commas remember to enclose the expression within quotes, or the xaml parser will truncate it";

                Debug.WriteLine(problem);
                ConstantResult = problem;
                return new Binding("ConstantResult", BindingMode.OneTime, null, null, null, this);
            }
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<BindingBase>).ProvideValue(serviceProvider);
        }
    }
}
