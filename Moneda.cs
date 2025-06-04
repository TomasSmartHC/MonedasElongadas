public class Moneda
{
    public required string Titulo { get; set; }
    public required string Descripcion { get; set; }
    public required string LugarObtencion { get; set; }
    public DateTime FechaObtencion { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime FechaModificacion { get; set; }

    public string? Imagen { get; set; }

    public string LugarYFecha =>
    $"{LugarObtencion} ({FechaObtencion:yyyy-MM-dd})";

    // Propiedad para mostrar la imagen desde base64
    public ImageSource? ImagenSource
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Imagen))
                return null;
            try
            {
                // Elimina espacios y saltos de línea si los hay
                var base64 = Imagen.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                byte[] bytes = Convert.FromBase64String(base64);
                return ImageSource.FromStream(() => new System.IO.MemoryStream(bytes));
            }
            catch
            {
                return null;
            }
        }
    }
}
