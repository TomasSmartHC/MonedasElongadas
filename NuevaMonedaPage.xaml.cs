using System;
using Microsoft.Maui.Controls;

namespace MonedasElongadas
{
    public partial class NuevaMonedaPage : ContentPage
    {
        private MonedaXmlService servicio = new MonedaXmlService();

        public NuevaMonedaPage()
        {
            InitializeComponent();
        }

        private async void AnadirMoneda_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TituloEntry.Text) ||
                string.IsNullOrWhiteSpace(DescripcionEntry.Text) ||
                string.IsNullOrWhiteSpace(LugarEntry.Text))
            {
                await DisplayAlert("Error", "Por favor, rellena todos los campos.", "OK");
                return;
            }

            var nuevaMoneda = new Moneda
            {
                Titulo = TituloEntry.Text,
                Descripcion = DescripcionEntry.Text,
                LugarObtencion = LugarEntry.Text,
                FechaObtencion = FechaObtencionPicker.Date,
                FechaAlta = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            var monedas = await servicio.LeerMonedasAsync();
            monedas.Add(nuevaMoneda);
            await servicio.GuardarMonedasAsync(monedas);

            await DisplayAlert("Éxito", "Moneda añadida correctamente.", "OK");
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
