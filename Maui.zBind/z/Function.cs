using FunctionZero.ExpressionParserZero.BackingStore;
using System.Xml;

[assembly: Microsoft.Maui.Controls.XmlnsPrefix("FunctionZero.Maui.zBind.z", "zero")]

namespace FunctionZero.Maui.zBind.z
{
    [ContentProperty("Expression")]
    public class Function : IMarkupExtension<TreeAndSource>
    {
        private string _expression;

        public string Expression 
        { 
            get => _expression;
            set => _expression = value; 
        }

        public object Source { get; set; }

        public Function()
        {
        }

        public TreeAndSource ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Expression))
            {
                IXmlLineInfo lineInfo = serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider lineInfoProvider ? lineInfoProvider.XmlLineInfo : new XmlLineInfo();
                throw new XamlParseException("Expression requires 'Expression' property to be set", lineInfo);
            }

            var ep = ExpressionParserZero.Binding.ExpressionParserFactory.GetExpressionParser();
            var compiledExpression = ep.Parse(Expression);

            if (Source == null)
                return new TreeAndSource(compiledExpression, (obj) => obj?.BindingContext == null ? null : new PocoBackingStore(obj.BindingContext));
            else
                return new TreeAndSource(compiledExpression, (obj) => new PocoBackingStore(Source));
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<TreeAndSource>).ProvideValue(serviceProvider);
        }
    }
}
