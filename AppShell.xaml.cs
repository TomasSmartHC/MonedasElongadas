namespace MonedasElongadas
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Routing.RegisterRoute(nameof(NuevaMonedaPage), typeof(NuevaMonedaPage));
            Routing.RegisterRoute(nameof(SugerenciasPage), typeof(SugerenciasPage));
            InitializeComponent();
        }
        private async void OnSugerenciasMenuClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SugerenciasPage));
        }
    }
}
