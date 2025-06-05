using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MonedasElongadas
{
    public partial class ImportarXmlPage : ContentPage
    {
        private MonedaXmlService servicio = new MonedaXmlService();

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ResultadoLabel.Text = "";
            ResultadoLabel.TextColor = Colors.Green; // Opcional: restablece el color
        }

        // Define static readonly field for XML file types to address CA1861  
        private static readonly FilePickerFileType XmlFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
           {
               { DevicePlatform.iOS, new[] { "public.xml" } },
               { DevicePlatform.Android, new[] { "application/xml", "text/xml" } },
               { DevicePlatform.WinUI, new[] { ".xml" } },
               { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
           });

        public ImportarXmlPage()
        {
            InitializeComponent();
        }

        private async void OnImportarXmlClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona el archivo XML",
                    FileTypes = XmlFileType // Use the static readonly field  
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var xmlContent = await reader.ReadToEndAsync();

                    // Deserializa el XML a la lista de monedas  
                    var monedas = servicio.DeserializarMonedas(xmlContent); // Debes tener este método  

                    // Guarda las monedas importadas  
                    MonedaXmlService.Guardar(monedas);

                    ResultadoLabel.Text = "Importación completada correctamente.";
                    ResultadoLabel.TextColor = Colors.Green;
                }
                else
                {
                    ResultadoLabel.Text = "No se seleccionó ningún archivo.";
                    ResultadoLabel.TextColor = Colors.Orange;
                }
            }
            catch (Exception ex)
            {
                ResultadoLabel.Text = $"Error: {ex.Message}";
                ResultadoLabel.TextColor = Colors.Red;
            }
        }
    }
}
