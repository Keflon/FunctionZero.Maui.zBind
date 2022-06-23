using FunctionZero.ExpressionParserZero.BackingStore;
using FunctionZero.ExpressionParserZero.Evaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.zBind.z
{
    public class TreeAndSource
    {
        public TreeAndSource(ExpressionTree tree, Func<BindableObject, IBackingStore> sourceGetter)
        {
            Tree = tree;
            SourceGetter = sourceGetter;
        }

        public ExpressionTree Tree { get; }
        public Func<BindableObject, IBackingStore> SourceGetter { get; }
    }
}
