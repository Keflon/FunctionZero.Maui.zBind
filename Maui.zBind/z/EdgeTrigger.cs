using FunctionZero.ExpressionParserZero.BackingStore;
using FunctionZero.ExpressionParserZero.Evaluator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.zBind.z
{
    public class EdgeTrigger : Behavior<Element>
    {
        BindableObject _host;
        private IBackingStore _risingBackingStore;
        private IBackingStore _fallingBackingStore;

        protected override void OnAttachedTo(BindableObject host)
        {
            _host = host;

            var binding = new Binding("BindingContext", BindingMode.OneWay, null, null, null, _host);

            this.SetBinding(BindingContextProperty, binding);

            //host.BindingContextChanged += _host_BindingContextChanged;
            base.OnAttachedTo(host);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // Get the backing store from the Rising and Falling instances ...
            _risingBackingStore = Rising?.SourceGetter(_host);
            _fallingBackingStore = Falling?.SourceGetter(_host);

            DoAction(Condition);
        }

        protected override void OnDetachingFrom(Element host)
        {
            _host = null;
            _risingBackingStore = null;
            _fallingBackingStore = null;
            this.RemoveBinding(BindingContextProperty);
            base.OnDetachingFrom(host);
        }

        public static readonly BindableProperty ConditionProperty = BindableProperty.Create(nameof(Condition), typeof(bool), typeof(EdgeTrigger), false, BindingMode.OneWay, null, ConditionChanged);

        public bool Condition
        {
            get { return (bool)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }

        private static void ConditionChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var self = (EdgeTrigger)bindable;

            self.DoAction(self.Condition);
        }

        private void DoAction(bool condition)
        {
            try
            {
                if (condition == true)
                {
                    if (_risingBackingStore != null)
                        Rising?.Tree?.Evaluate(_risingBackingStore);
                }
                else
                {
                    if (_fallingBackingStore != null)
                        Falling?.Tree?.Evaluate(_fallingBackingStore);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static readonly BindableProperty RisingProperty = BindableProperty.Create(nameof(Rising), typeof(TreeAndSource), typeof(EdgeTrigger), null, BindingMode.OneWay, null, RisingChanged);

        public TreeAndSource Rising
        {
            get { return (TreeAndSource)GetValue(RisingProperty); }
            set { SetValue(RisingProperty, value); }
        }

        private static void RisingChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            try
            {
                var self = (EdgeTrigger)bindable;

                self._risingBackingStore = self.Rising.SourceGetter(self._host);

                if (self.Condition == true)
                {
                    self.DoAction(self.Condition);
                }
            }
            catch (Exception ex)
            { }
        }

        public static readonly BindableProperty FallingProperty = BindableProperty.Create(nameof(Falling), typeof(TreeAndSource), typeof(EdgeTrigger), null, BindingMode.OneWay, null, FallingChanged);

        public TreeAndSource Falling
        {
            get { return (TreeAndSource)GetValue(FallingProperty); }
            set { SetValue(FallingProperty, value); }
        }

        private static void FallingChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            try
            {
                var self = (EdgeTrigger)bindable;

                self._fallingBackingStore = self.Falling.SourceGetter(self._host);

                if (self.Condition == false)
                {
                    self.DoAction(self.Condition);
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
