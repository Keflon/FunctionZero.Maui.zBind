using FunctionZero.ExpressionParserZero.BackingStore;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionZero.Maui.zBind.z
{
    public class TapTrigger : Behavior<Element>
    {
        BindableObject _host;
        private IBackingStore _risingBackingStore;
        public TapTrigger()
        {
            // TODO: 
        }
        protected override void OnAttachedTo(BindableObject host)
        {
            // TODO: Hook the appropriate event on host, depending on type, ...
            // TODO: Button -> Click event
            // TODO: TapGestureRecognizer -> Tapped event
            // TODO: And call DoAction(sender, e)
            _host = host;

            if (host is Button b)
            {
                host.BindingContextChanged += _host_BindingContextChanged;
                b.Clicked += DoAction;
            }
            else if (host is View v)
            {
                host.BindingContextChanged += _host_BindingContextChanged;
                var tgr = new TapGestureRecognizer();
                tgr.Tapped += DoAction;
                v.GestureRecognizers.Add(tgr);
            }
            else
                throw new NotImplementedException("TapTrigger - host is not a View");

            base.OnAttachedTo(host);
        }

        protected override void OnDetachingFrom(Element host)
        {
            // TODO: Hook the appropriate event on host, depending on type, ...
            // TODO: Button -> Click event
            // TODO: TapGestureRecognizer -> Tapped event

            _host = null;
            _risingBackingStore = null;
            ((Button)host).BindingContextChanged -= _host_BindingContextChanged;

            base.OnDetachingFrom(host);
        }

        private void _host_BindingContextChanged(object sender, EventArgs e)
        {
            // this.BindingContext is needed for the BindingContext of the TapAction.
            // TODO: Use a binding object.
            this.BindingContext = _host.BindingContext;

            // Get the backing store from the Rising and Falling instances ...
            _risingBackingStore = TapAction?.SourceGetter(_host);
        }

        private void DoAction(object sender, EventArgs e)
        {
            try
            {
                TapAction?.Tree?.Evaluate(_risingBackingStore);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        public static readonly BindableProperty TapActionProperty = BindableProperty.Create(nameof(TapAction), typeof(TreeAndSource), typeof(TapTrigger), null, BindingMode.OneWay, null, TapTriggerChanged);

        public TreeAndSource TapAction
        {
            get { return (TreeAndSource)GetValue(TapActionProperty); }
            set { SetValue(TapActionProperty, value); }
        }

        private static void TapTriggerChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            try
            {
                var self = (TapTrigger)bindable;

                self._risingBackingStore = self.TapAction.SourceGetter(self._host);
            }
            catch (Exception ex)
            { }
        }
    }
}
