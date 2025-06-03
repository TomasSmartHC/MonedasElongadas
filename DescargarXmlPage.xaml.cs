using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace MonedasElongadas
{
    public partial class DescargarXmlPage : ContentPage
    {
        private MonedaXmlService servicio = new MonedaXmlService();

        public DescargarXmlPage()
        {
            InitializeComponent();
        }

        private async void OnDescargarXmlClicked(object sender, EventArgs e)
        {
            try
            {
                // Obtén el XML como string (ajusta según tu implementación)
                var monedas = await servicio.LeerMonedasAsync();
                var xml = servicio.SerializarMonedas(monedas); // Debes tener este método

                // Define la ruta de guardado
                var fileName = $"monedas_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
                var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                // Guarda el archivo
                File.WriteAllText(filePath, xml);

                ResultadoLabel.Text = $"XML guardado en:\n{filePath}";
            }
            catch (Exception ex)
            {
                ResultadoLabel.Text = $"Error: {ex.Message}";
                ResultadoLabel.TextColor = Colors.Red;
            }
        }
    }
}
