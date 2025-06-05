namespace MonedasElongadas
{
    public partial class NuevaMonedaPage : ContentPage, IQueryAttributable
    {
        private int? indiceMonedaEditando = null;
        private string? imagenBase64Temp = null;

        public NuevaMonedaPage()
        {
            InitializeComponent();
            ImagenMoneda.Source = null;
            BorrarImagenButton.IsVisible = false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Si NO estás editando una moneda, limpia los campos
            if (!indiceMonedaEditando.HasValue)
                InicializarElementos();
        }

        private void InicializarElementos()
        {
            TituloEntry.Text = string.Empty;
            DescripcionEntry.Text = string.Empty;
            LugarEntry.Text = string.Empty;
            FechaObtencionPicker.Date = DateTime.Today;
            ImagenMoneda.Source = null;
            BorrarImagenButton.IsVisible = false;
            CargarImagenButton.IsVisible = true;
            imagenBase64Temp = null;
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

                // Si estamos editando, guarda en la moneda existente
                var monedas = MonedaXmlService.LeerMonedasAsync();
                if (indiceMonedaEditando.HasValue && indiceMonedaEditando.Value >= 0 && indiceMonedaEditando.Value < monedas.Count)
                {
                    var moneda = monedas[indiceMonedaEditando.Value];
                    moneda.Imagen = base64;
                    ImagenMoneda.Source = moneda.ImagenSource;
                    BorrarImagenButton.IsVisible = true;
                    CargarImagenButton.IsVisible = false;
                    MonedaXmlService.Guardar(monedas);
                }
                else
                {
                    // Si es nueva moneda, guarda la imagen temporalmente y muéstrala
                    imagenBase64Temp = base64;
                    ImagenMoneda.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
                    BorrarImagenButton.IsVisible = true;
                    CargarImagenButton.IsVisible = false;
                }
            }
        }

        private void BorrarImagen_Clicked(object sender, EventArgs e)
        {
            if (indiceMonedaEditando.HasValue)
            {
                var monedas = MonedaXmlService.LeerMonedasAsync();
                if (indiceMonedaEditando.Value >= 0 && indiceMonedaEditando.Value < monedas.Count)
                {
                    var moneda = monedas[indiceMonedaEditando.Value];
                    moneda.Imagen = null;
                    MonedaXmlService.Guardar(monedas);
                    ImagenMoneda.Source = null;
                    BorrarImagenButton.IsVisible = false;
                }
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("IndiceMoneda", out var indiceObj) && indiceObj is int indice && indice >= 0)
            {
                indiceMonedaEditando = indice;
                var monedas = MonedaXmlService.LeerMonedasAsync();
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

            var monedas = MonedaXmlService.LeerMonedasAsync();

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
                    FechaModificacion = DateTime.Now,
                    Imagen = imagenBase64Temp // Asigna la imagen temporal
                };
                monedas.Insert(0, nuevaMoneda);
                imagenBase64Temp = null; // Limpia la imagen temporal tras guardar
            }

            MonedaXmlService.Guardar(monedas);

            await DisplayAlert("Éxito", indiceMonedaEditando.HasValue ? "Moneda modificada correctamente." : "Moneda añadida correctamente.", "OK");
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}