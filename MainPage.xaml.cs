using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MonedasElongadas
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Moneda> monedasObservable;
        private MonedaXmlService servicio = new MonedaXmlService();

        public MainPage()
        {
            InitializeComponent();
            CargarMonedas();
        }

        private async void MonedasCollectionView_ReorderCompleted(object sender, EventArgs e)
        {
            // Guarda el nuevo orden si lo necesitas
            await servicio.GuardarMonedasAsync(monedasObservable.ToList());
        }

        private async void CargarMonedas()
        {
            var monedas = await servicio.LeerMonedasAsync();
            monedasObservable = new ObservableCollection<Moneda>(monedas);
            MonedasCollectionView.ItemsSource = monedasObservable;
        }

        private async void BorrarMoneda_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Moneda moneda)
            {
                bool confirm = await DisplayAlert("Confirmar", $"¿Borrar la moneda \"{moneda.Titulo}\"?", "Sí", "No");
                if (!confirm) return;

                monedasObservable.Remove(moneda);
                await servicio.GuardarMonedasAsync(monedasObservable.ToList());
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RecargarMonedas();
        }

        private async Task RecargarMonedas()
        {
            var monedas = await servicio.LeerMonedasAsync();
            if (monedasObservable == null)
                monedasObservable = new ObservableCollection<Moneda>(monedas);
            else
            {
                monedasObservable.Clear();
                foreach (var m in monedas)
                    monedasObservable.Add(m);
            }
            MonedasCollectionView.ItemsSource = monedasObservable;
        }
    }
}
