//using FunctionZero.ExpressionParserZero.BackingStore;
//using FunctionZero.ExpressionParserZero.Evaluator;
//using Maui.zBind.z;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FunctionZero.Maui.zBind.z
//{
//    public class Latch : Behavior<Element>
//    {
//        BindableObject _host;
//        private IBackingStore _backingStore;

//        protected override void OnAttachedTo(BindableObject thing)
//        {
//            _host = thing;

//            thing.BindingContextChanged += _host_BindingContextChanged;
//            base.OnAttachedTo(thing);
//        }

//        private void _host_BindingContextChanged(object sender, EventArgs e)
//        {
//            // this.BindingContext is needed for the BindingContext of the Condition.
//            this.BindingContext = _host.BindingContext;
//            // If the binding source has not been explicitly set, track the host's binding context.
//            if (Action.Source == null)
//                if (BindingContext != null)
//                {
//                    _backingStore = new PocoBackingStore(_host.BindingContext);
//                }
//            else
//                {
//                    _backingStore = null;
//                }
//        }

//        protected override void OnDetachingFrom(Element thing)
//        {
//            _host = null;
//            _backingStore = null;
//            thing.BindingContextChanged -= _host_BindingContextChanged;

//            base.OnDetachingFrom(thing);
//        }

//        public static readonly BindableProperty ConditionProperty = BindableProperty.Create(nameof(Condition), typeof(object), typeof(Latch), 2.0, BindingMode.TwoWay, null, ConditionChanged);

//        public object Condition
//        {
//            get { return (object)GetValue(ConditionProperty); }
//            set { SetValue(ConditionProperty, value); }
//        }

//        private static void ConditionChanged(BindableObject bindable, object oldvalue, object newvalue)
//        {
//            var self = (Latch)bindable;

//            if (self._backingStore != null)
//                try
//                {
//                    self.Action?.Tree?.Evaluate(self._backingStore);
//                }
//                catch (Exception ex)
//                {
//                    Debug.WriteLine(ex);
//                }
//        }

//        public static readonly BindableProperty ActionProperty = BindableProperty.Create(nameof(Action), typeof(EdgeTriggerAction), typeof(EdgeTrigger), null, BindingMode.TwoWay, null, ActionChanged);

//        public EdgeTriggerAction Action
//        {
//            get { return (EdgeTriggerAction)GetValue(ActionProperty); }
//            set { SetValue(ActionProperty, value); }
//        }

//        private static void ActionChanged(BindableObject bindable, object oldvalue, object newvalue)
//        {
//            try
//            {
//                var self = (EdgeTrigger)bindable;

//                // If the binding source is explicitly set, that trumps all and is set here.
//                if (self.Action.Source != null)
//                    self._backingStore = new PocoBackingStore(self.Action.Source);
//            }
//            catch (Exception ex)
//            { }
//        }
//    }
//}
