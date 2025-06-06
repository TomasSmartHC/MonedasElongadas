namespace MonedasElongadas
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Routing.RegisterRoute(nameof(NuevaMonedaPage), typeof(NuevaMonedaPage));
            InitializeComponent();
        }
    }
}
