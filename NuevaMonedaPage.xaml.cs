using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Globalization;

namespace MonedasElongadas
{
    public partial class NuevaMonedaPage : ContentPage, IQueryAttributable
    {
        private MonedaXmlService servicio = new MonedaXmlService();
        private int? indiceMonedaEditando = null;

        public NuevaMonedaPage()
        {
            InitializeComponent();
        }

        private async void CargarImagen_Clicked(object sender, EventArgs e)
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Selecciona una imagen",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var bytes = ms.ToArray();
                var base64 = Convert.ToBase64String(bytes);

                // Asigna la imagen a la moneda en edición
                var monedas = await servicio.LeerMonedasAsync();
                if (indiceMonedaEditando.HasValue && indiceMonedaEditando.Value >= 0 && indiceMonedaEditando.Value < monedas.Count)
                {
                    var moneda = monedas[indiceMonedaEditando.Value];
                    moneda.Imagen = base64;
                    ImagenMoneda.Source = moneda.ImagenSource;
                    BorrarImagenButton.IsVisible = true;
                    CargarImagenButton.IsVisible = false;
                    await servicio.GuardarMonedasAsync(monedas);
                }
            }
        }

        private async void BorrarImagen_Clicked(object sender, EventArgs e)
        {
            if (indiceMonedaEditando.HasValue)
            {
                var monedas = await servicio.LeerMonedasAsync();
                if (indiceMonedaEditando.Value >= 0 && indiceMonedaEditando.Value < monedas.Count)
                {
                    var moneda = monedas[indiceMonedaEditando.Value];
                    moneda.Imagen = null;
                    await servicio.GuardarMonedasAsync(monedas);
                    ImagenMoneda.Source = null;
                    BorrarImagenButton.IsVisible = false;
                }
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("IndiceMoneda", out var indiceObj) && indiceObj is int indice && indice >= 0)
            {
                indiceMonedaEditando = indice;
                var monedas = await servicio.LeerMonedasAsync();
                if (indice < monedas.Count)
                {
                    var moneda = monedas[indice];
                    TituloEntry.Text = moneda.Titulo;
                    DescripcionEntry.Text = moneda.Descripcion;
                    LugarEntry.Text = moneda.LugarObtencion;
                    FechaObtencionPicker.Date = moneda.FechaObtencion;
                    AnadirModificarButton.Text = "Modificar";
                    Title = "Modificar moneda";
                    // Mostrar la imagen en grande
                    ImagenMoneda.Source = moneda.ImagenSource;
                    BorrarImagenButton.IsVisible = !string.IsNullOrEmpty(moneda.Imagen);
                }
            }
            else
            {
                AnadirModificarButton.Text = "Añadir";
                ImagenMoneda.Source = null;
                BorrarImagenButton.IsVisible = false;
            }
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

            var monedas = await servicio.LeerMonedasAsync();

            if (indiceMonedaEditando.HasValue && indiceMonedaEditando.Value >= 0 && indiceMonedaEditando.Value < monedas.Count)
            {
                // Modificar moneda existente por índice
                var moneda = monedas[indiceMonedaEditando.Value];
                moneda.Titulo = TituloEntry.Text;
                moneda.Descripcion = DescripcionEntry.Text;
                moneda.LugarObtencion = LugarEntry.Text;
                moneda.FechaObtencion = FechaObtencionPicker.Date;
                moneda.FechaModificacion = DateTime.Now;
            }
            else
            {
                // Añadir nueva moneda
                var nuevaMoneda = new Moneda
                {
                    Titulo = TituloEntry.Text,
                    Descripcion = DescripcionEntry.Text,
                    LugarObtencion = LugarEntry.Text,
                    FechaObtencion = FechaObtencionPicker.Date,
                    FechaAlta = DateTime.Now,
                    FechaModificacion = DateTime.Now
                };
                monedas.Insert(0, nuevaMoneda);
            }

            await servicio.GuardarMonedasAsync(monedas);

            await DisplayAlert("Éxito", indiceMonedaEditando.HasValue ? "Moneda modificada correctamente." : "Moneda añadida correctamente.", "OK");
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}