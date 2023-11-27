using System.Text.Json.Serialization;
using static App.API.ModulosRES;

namespace App.API
{
    // ConstitucionCooperativa ConstitucionCooperativaDeserializada = JsonSerializer.Deserialize<ConstitucionCooperativa>(myJsonResponse);
    public class ConstitucionCooperativa
    {
        [JsonPropertyName("objetoSocial")]
        public ObjetoSocial ObjetoSocial { get; set; }

        [JsonPropertyName("nombreCooperativa")]
        public NombreCooperativa NombreCooperativa { get; set; }

        [JsonPropertyName("direccionDeLaCooperativa")]
        public DireccionDeLaCooperativa DireccionDeLaCooperativa { get; set; }

        [JsonPropertyName("contactoDeLaCooperativa")]
        public ContactoDeLaCooperativa ContactoDeLaCooperativa { get; set; }

        [JsonPropertyName("otrosAcuerdos")]
        public OtrosAcuerdos OtrosAcuerdos { get; set; }

        [JsonPropertyName("documentos")]
        public Documentos Documentos { get; set; }

        [JsonPropertyName("datosDelSistema")]
        public DatosDelSistema DatosDelSistema { get; set; }
        public bool NullParameter()
        {
            return ((this.ObjetoSocial is null || this.ObjetoSocial.NullParameter()) ||
                    (this.NombreCooperativa is null || this.NombreCooperativa.NullParameter()) ||
                    (this.DireccionDeLaCooperativa is null || this.DireccionDeLaCooperativa.NullParameter()) ||
                    (this.ContactoDeLaCooperativa is null || this.ContactoDeLaCooperativa.NullParameter()) ||
                    (this.OtrosAcuerdos is null || this.OtrosAcuerdos.NullParameter()) ||
                    (this.Documentos is null || this.Documentos.NullParameter()) ||
                    (this.DatosDelSistema is null || this.DatosDelSistema.NullParameter()));
        }
    }
}
