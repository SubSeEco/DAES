using static App.API.ModulosRES;
using System.Text.Json.Serialization;

namespace App.API
{
    // API APIDeserializada = JsonSerializer.Deserialize<API>(myJsonResponse);
    public class API
    {
        [JsonPropertyName("objetoSocial")]
        public ObjetoSocial ObjetoSocial { get; set; }

        [JsonPropertyName("nombreCooperativa")]
        public NombreCooperativa NombreCooperativa { get; set; }

        [JsonPropertyName("direccionDeLaCooperativa")]
        public DireccionDeLaCooperativa DireccionDeLaCooperativa { get; set; }

        [JsonPropertyName("contactoDeLaCooperativa")]
        public ContactoDeLaCooperativa ContactoDeLaCooperativa { get; set; }

        [JsonPropertyName("capitalDeLaCooperativa")]
        public CapitalDeLaCooperativa CapitalDeLaCooperativa { get; set; }

        [JsonPropertyName("duracionDeLaCooperativa")]
        public DuracionDeLaCooperativa DuracionDeLaCooperativa { get; set; }

        [JsonPropertyName("administracionDeLaCooperativa")]
        public AdministracionDeLaCooperativa AdministracionDeLaCooperativa { get; set; }

        [JsonPropertyName("consejoAdministracion")]
        public ConsejoAdministracion ConsejoAdministracion { get; set; }

        [JsonPropertyName("juntaDeVigilancia")]
        public JuntaDeVigilancia JuntaDeVigilancia { get; set; }

        [JsonPropertyName("derechosYObligacionesSocios")]
        public DerechosYObligacionesSocios DerechosYObligacionesSocios { get; set; }

        [JsonPropertyName("otrosAcuerdos")]
        public OtrosAcuerdos OtrosAcuerdos { get; set; }

        [JsonPropertyName("cooperadosYAdministradores")]
        public List<CooperadoYAdministrador> CooperadosYAdministradores { get; set; }

        [JsonPropertyName("documentos")]
        public Documentos Documentos { get; set; }

        [JsonPropertyName("datosDelSistema")]
        public DatosDelSistema DatosDelSistema { get; set; }
    }
}
