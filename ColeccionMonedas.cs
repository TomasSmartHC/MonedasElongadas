using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Maui.Storage;
using System.Diagnostics;

[XmlRoot("ColeccionMonedas")]
public class ColeccionMonedas
{
    [XmlElement("Moneda")]
    public List<Moneda> Moneda { get; set; }
}

public class MonedaXmlService
{
    public async Task<List<Moneda>> LeerMonedasAsync()
    {
        
        try
        {
            var LocalPath = Path.Combine(FileSystem.AppDataDirectory, "Monedas.xml");
            if (!File.Exists(LocalPath))
            {
                // Copia desde recursos si no existe
                using var assetStream = await FileSystem.OpenAppPackageFileAsync("Monedas.xml");
                using var localStream = File.Create(LocalPath);
                await assetStream.CopyToAsync(localStream);
            }

            using var stream = File.OpenRead(LocalPath);
            var serializer = new XmlSerializer(typeof(ColeccionMonedas));
            var coleccion = (ColeccionMonedas)serializer.Deserialize(stream);
            return coleccion?.Moneda ?? new List<Moneda>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error leyendo el XML: {ex.Message}");
            return new List<Moneda>();
        }
    }

    public async Task GuardarMonedasAsync(List<Moneda> monedas)
    {
        var coleccion = new ColeccionMonedas { Moneda = monedas };
        var serializer = new XmlSerializer(typeof(ColeccionMonedas));
        var filePath = Path.Combine(FileSystem.AppDataDirectory, "Monedas.xml");

        using (var stream = File.Create(filePath))
        {
            serializer.Serialize(stream, coleccion);
        }
    }

    internal string? SerializarMonedas(List<Moneda> monedas)
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
        var coleccion = (ColeccionMonedas)serializer.Deserialize(reader);
        return coleccion?.Moneda ?? new List<Moneda>();
    }

}
