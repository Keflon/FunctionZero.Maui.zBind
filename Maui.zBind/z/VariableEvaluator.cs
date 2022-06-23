using FunctionZero.ExpressionParserZero.BackingStore;
using FunctionZero.ExpressionParserZero.Operands;
using System.Collections.Generic;
using System.Reflection;

namespace FunctionZero.Maui.zBind.z
{
    internal class VariableEvaluator : IBackingStore
    {
        private object[] _values;
        private readonly IList<string> _keys;
        private readonly Bind _bindingExtension;

        public VariableEvaluator(IList<string> keys, Bind bindingExtension)
        {
            _keys = keys;
            _bindingExtension = bindingExtension;
        }

        internal void SetValues(object[] values)
        {
            _values = values;
        }

        public (OperandType type, object value) GetValue(string qualifiedName)
        {

            int index = _keys.IndexOf(qualifiedName);
            object value = _values[index];

            if (value == null)
                return (OperandType.Null, null);

            if (BackingStoreHelpers.OperandTypeLookup.TryGetValue(value.GetType(), out var theOperandType))
                return (theOperandType, value);

            return (OperandType.Object, value);
        }

        private static char[] _dot = new[] { '.' };

        public void SetValue(string qualifiedName, object value)
        {
            var host = _bindingExtension.Source ?? _bindingExtension.BindableTarget.BindingContext;
            if (host != null)
            {
                var bits = qualifiedName.Split(_dot);

                for (int c = 0; c < bits.Length - 1; c++)
                {
                    PropertyInfo prop = host.GetType().GetProperty(bits[c], BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanRead)
                    {
                        host = prop.GetValue(host);
                    }
                    else
                        return;
                }
                var variableName = bits[bits.Length - 1];

                PropertyInfo prop2 = host.GetType().GetProperty(variableName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop2 && prop2.CanWrite)
                {
                    prop2.SetValue(host, value, null);
                }
            }
        }

        public void SetValue(PropertyInfo propInfo, object value)
        {
            var host = _bindingExtension.Source ?? _bindingExtension.BindableTarget.BindingContext;
            if (host != null)
            {
                propInfo.SetValue(host, value, null);
            }
        }

        public PropertyInfo GetPropertyInfo(string qualifiedName)
        {
            var host = _bindingExtension.Source ?? _bindingExtension.BindableTarget.BindingContext;
            if (host != null)
            {
                var bits = qualifiedName.Split(_dot);

                for (int c = 0; c < bits.Length - 1; c++)
                {
                    PropertyInfo prop = host.GetType().GetProperty(bits[c], BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanRead)
                    {
                        host = prop.GetValue(host);
                    }
                    else
                        return null;
                }
                var variableName = bits[bits.Length - 1];

                return host.GetType().GetProperty(variableName, BindingFlags.Public | BindingFlags.Instance);
            }
            return null;
        }
    }
}