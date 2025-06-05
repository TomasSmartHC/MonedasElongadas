using System.Xml.Serialization;
using System.Diagnostics;

[XmlRoot("ColeccionMonedas")]
public class ColeccionMonedas
{
    [XmlElement("Moneda")]
    public List<Moneda> Moneda { get; set; } = new List<Moneda>(); // Initialize to avoid null issues  
}

public class MonedaXmlService
{
    public static List<Moneda> LeerMonedasAsync()
    {
        try
        {
            var LocalPath = Path.Combine(FileSystem.AppDataDirectory, "Monedas.xml");
            if (!File.Exists(LocalPath))
                CrearXML(LocalPath);

            using var stream = File.OpenRead(LocalPath);
            var serializer = new XmlSerializer(typeof(ColeccionMonedas));
            var coleccion = serializer.Deserialize(stream) as ColeccionMonedas; // Use safe cast to avoid null issues  
            return coleccion?.Moneda ?? new List<Moneda>(); // Use null-coalescing operator to handle null  
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error leyendo el XML: {ex.Message}");
            return new List<Moneda>();
        }
    }

    private static async void CrearXML(string LocalPath)
    {
        using var assetStream = await FileSystem.OpenAppPackageFileAsync("Monedas.xml");
        using var localStream = File.Create(LocalPath);
        await assetStream.CopyToAsync(localStream);
    }

    public static void Guardar(List<Moneda> monedas)
    {
        var coleccion = new ColeccionMonedas { Moneda = monedas };
        var serializer = new XmlSerializer(typeof(ColeccionMonedas));
        var filePath = Path.Combine(FileSystem.AppDataDirectory, "Monedas.xml");

        using (var stream = File.Create(filePath))
        {
            serializer.Serialize(stream, coleccion);
        }
    }

    internal static string? SerializarMonedas(List<Moneda> monedas)
    {
        var coleccion = new ColeccionMonedas { Moneda = monedas };
        var serializer = new XmlSerializer(typeof(ColeccionMonedas));
        using var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, coleccion);
        return stringWriter.ToString();
    }

    public List<Moneda> DeserializarMonedas(string xml)
    {
        var serializer = new XmlSerializer(typeof(ColeccionMonedas));
        using var reader = new StringReader(xml);
        var coleccion = serializer.Deserialize(reader) as ColeccionMonedas; // Use safe cast to avoid null issues  
        return coleccion?.Moneda ?? new List<Moneda>(); // Use null-coalescing operator to handle null  
    }
}
