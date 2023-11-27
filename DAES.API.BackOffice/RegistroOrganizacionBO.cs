using System.Text.Json.Serialization;
using MessagePack;
using Newtonsoft.Json;
using static App.API.ModulosRES;

namespace App.API
{
    // RegistroOrganizacionBO GestionProcesosDeserializada = JsonSerializer.Deserialize<RegistroOrganizacionBO>(myJsonResponse);
    public class RegistroOrganizacionBO
    {
        [JsonPropertyName("nombreCooperativa")]
        public NombreCooperativa NombreCooperativa { get; set; }

        [JsonPropertyName("direccionDeLaCooperativa")]
        public DireccionDeLaCooperativa DireccionDeLaCooperativa { get; set; }

        [JsonPropertyName("contactoDeLaCooperativa")]
        public ContactoDeLaCooperativa ContactoDeLaCooperativa { get; set; } = null!;

        [JsonPropertyName("documentos")]
        public Documentos Documentos { get; set; }

        [JsonPropertyName("datosDelSistema")]
        public DatosDelSistema DatosDelSistema { get; set; }

        public bool NullParameter()
        {
            return ((this.NombreCooperativa is null || this.NombreCooperativa.NullParameter()) ||
                    (this.DireccionDeLaCooperativa is null || this.DireccionDeLaCooperativa.NullParameter()) ||
                    (this.ContactoDeLaCooperativa is null || this.ContactoDeLaCooperativa.NullParameter()) ||
                    (this.Documentos is null || this.Documentos.NullParameter()) ||
                    (this.DatosDelSistema is null || this.DatosDelSistema.NullParameter()));
        }
    }
}
