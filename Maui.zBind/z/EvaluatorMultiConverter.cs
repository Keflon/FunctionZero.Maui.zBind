using FunctionZero.ExpressionParserZero.BackingStore;
using FunctionZero.ExpressionParserZero.Evaluator;
using FunctionZero.ExpressionParserZero.Operands;
using FunctionZero.ExpressionParserZero.Tokens;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;


namespace FunctionZero.Maui.zBind.z
{
    internal class EvaluatorMultiConverter : IMultiValueConverter
    {
        private readonly VariableEvaluator _evaluator;
        private readonly ExpressionTree _compiledExpressionTree;
        private readonly List<ExpressionTreeNode> _unExpressionTreeParentList;
        private readonly ExpressionTree _unExpressionTree;
        private readonly string _variableName;

        public event EventHandler Converted;

        public EvaluatorMultiConverter(ICollection<string> keys, ExpressionTree compiledExpression, Bind bindingExtension)
        {
            _evaluator = new VariableEvaluator(new List<string>(keys), bindingExtension);
            _compiledExpressionTree = compiledExpression;

            // If the ExpressionTree has a variable at the leaf, where all ancestors are a unary operator,
            // we can build an expression that does the opposite. Bail out if that's not possible.

            // This ExpressionTree will lie about its RpnTokens.
            var unExpressionTree = new ExpressionTree(new List<IToken>());
            List<ExpressionTreeNode> unExpressionTreeParentList = unExpressionTree.RootNodeList;
            var node = _compiledExpressionTree.RootNodeList[0];

            // For each unary operator in the _compiledExpressionTree, copy it to the unExpressionTree.
            while (node.Children.Count == 1)
            {
                unExpressionTreeParentList.Add(new ExpressionTreeNode(node.Token, 1));
                unExpressionTreeParentList = unExpressionTreeParentList[0].Children;
                node = node.Children[0];
            }
            if (node.Token is Operand operand)
            {
                if (operand.Type == OperandType.Variable)
                {
                    // Leaf node of original expression.
                    // We'll want to substitute this with an operand wrapping the new value.
                    _unExpressionTreeParentList = unExpressionTreeParentList;
                    // Placeholder
                    _unExpressionTreeParentList.Add(new ExpressionTreeNode(null, 0));
                    // node is the variable we want to assign.
                    _variableName = (string)operand.GetValue();
                    _unExpressionTree = unExpressionTree;
                }
            }
            else
            {
                // Non-unary operator. We cannot build the un-expression. Do nothing. 
            }
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            _evaluator.SetValues(values);

            try
            {
                var stack = _compiledExpressionTree.Evaluate(_evaluator);
                var thing = ExpressionParserZero.OperatorActions.PopAndResolve(stack, _evaluator);
                return thing.GetValue();
            }
            catch (Exception ex)
            {
                if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
                    return Activator.CreateInstance(targetType);
                else
                    return null;
            }
            finally
            {
                Converted?.Invoke(this, EventArgs.Empty);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (_unExpressionTree != null)
            {
                var propInfo = _evaluator.GetPropertyInfo(_variableName);

                if (propInfo != null && propInfo.CanWrite)
                {
                    if (BackingStoreHelpers.OperandTypeLookup.TryGetValue(propInfo.PropertyType, out var theOperandType))
                    {
                        var valueContainer = new Operand(theOperandType, value);
                        _unExpressionTreeParentList[0] = new ExpressionTreeNode(valueContainer, 0);
                        var result = _unExpressionTree.Evaluate(_evaluator);

                        _evaluator.SetValue(propInfo, result.Pop().GetValue());
                    }
                }
            }
            else
            {
                Debug.WriteLine("z:Bind attempt to write a value back to source. This is only possible if the expression contains one and only one token and the token is a variable.");
            }
            return null;
        }
    }
}