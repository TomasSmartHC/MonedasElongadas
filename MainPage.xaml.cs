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
            monedasObservable = [];
            CargarMonedas();
        }

        private void MonedasCollectionView_ReorderCompleted(object sender, EventArgs e)
        {
            // Guarda el nuevo orden si lo necesitas
            MonedaXmlService.Guardar(monedasObservable.ToList());
        }

        private void CargarMonedas()
        {
            var monedas = MonedaXmlService.LeerMonedasAsync();
            monedasObservable = [.. monedas];
            MonedasCollectionView.ItemsSource = monedasObservable;
        }

        private async void BorrarMoneda_Clicked(object sender, EventArgs e)
        {
            if (sender is ImageButton btn && btn.BindingContext is Moneda moneda)
            {
                bool confirm = await DisplayAlert("Confirmar", $"¿Borrar la moneda \"{moneda.Titulo}\"?", "Sí", "No");
                if (!confirm) return;

                monedasObservable.Remove(moneda);
                MonedaXmlService.Guardar(monedasObservable.ToList());
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RecargarMonedas();
        }

        private void RecargarMonedas()
        {
            var monedas = MonedaXmlService.LeerMonedasAsync();
            if (monedasObservable == null)
                monedasObservable = [.. monedas];
            else
            {
                monedasObservable.Clear();
                foreach (var m in monedas)
                    monedasObservable.Add(m);
            }
            MonedasCollectionView.ItemsSource = monedasObservable;
        }

        private async void OnMonedaTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.BindingContext is Moneda monedaSeleccionada)
            {
                // Usa la colección enlazada al CollectionView para obtener el índice
                var collection = MonedasCollectionView.ItemsSource as IList<Moneda>;
                int indice = collection?.IndexOf(monedaSeleccionada) ?? -1;

                if (indice >= 0)
                {
                    await Shell.Current.GoToAsync(nameof(NuevaMonedaPage), true, new Dictionary<string, object>
                    {
                        { "IndiceMoneda", indice }
                    });
                }
            }
        }
    }
}
