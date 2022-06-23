using FunctionZero.ExpressionParserZero.BackingStore;
using FunctionZero.ExpressionParserZero;
using FunctionZero.ExpressionParserZero.Binding;
using FunctionZero.ExpressionParserZero.Operands;

namespace Maui.zBindSample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var parser = ExpressionParserFactory.GetExpressionParser();
            parser.RegisterFunction("GetColor", DoGetColor, 3, 3);

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            return builder.Build();
        }

        private static void DoGetColor(Stack<IOperand> operandStack, IBackingStore backingStore, long paramCount)
        {
            // Pop the correct number of parameters from the operands stack, ** in reverse order **
            // If an operand is a variable, it is resolved from the backing store provided
            IOperand fourth = OperatorActions.PopAndResolve(operandStack, backingStore);
            IOperand third = OperatorActions.PopAndResolve(operandStack, backingStore);
            IOperand second = OperatorActions.PopAndResolve(operandStack, backingStore);
            IOperand first = OperatorActions.PopAndResolve(operandStack, backingStore);

            var r = Convert.ToSingle(first.GetValue());
            var g = Convert.ToSingle(second.GetValue());
            var b = Convert.ToSingle(third.GetValue());
            var a = Convert.ToSingle(fourth.GetValue());

            // The result is of type Color
            object result = new Color(r, g, b, 1);

            // Push the result back onto the operand stack
            operandStack.Push(new Operand(-1, OperandType.Object, result));
        }

    }
}