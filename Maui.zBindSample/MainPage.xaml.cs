namespace Maui.zBindSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            BindingContext = new MainPageVm();
            InitializeComponent();
        }
    }
}