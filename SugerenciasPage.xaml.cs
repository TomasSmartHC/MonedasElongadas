using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel.Communication;

namespace MonedasElongadas
{
    public partial class SugerenciasPage : ContentPage
    {
        private const string EmailDestino = "tomasdv@gmail.com";

        public SugerenciasPage()
        {
            InitializeComponent();
        }

        private async void EnviarSugerencia_Clicked(object sender, EventArgs e)
        {
            var texto = SugerenciaEditor.Text?.Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                await DisplayAlert("Error", "Por favor, escribe una sugerencia antes de enviar.", "OK");
                return;
            }

            try
            {
                var message = new EmailMessage
                {
                    Subject = "Sugerencia desde la app",
                    Body = texto,
                    To = new List<string> { EmailDestino }
                };
                await Email.Default.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No se pudo abrir el cliente de correo en este dispositivo.", "OK");
            }
        }
    }
}
