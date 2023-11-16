using DAES.Model.GestionProcesos;
using System.Data.Entity;

namespace DAES.Infrastructure.GestionProcesos
{
    public class GestionProcesosContext : DbContext
    {
        public GestionProcesosContext() : base("name=GestionProcesos")
        {
            System.Diagnostics.Debug.WriteLine("New GestionProcesosContext...");
            this.Database.CommandTimeout = 180;
        }

        public virtual DbSet<DocumentoGP> DocumentoGP { get; set; }
        public virtual DbSet<ProcesoGP> ProcesoGP { get; set; }
        public virtual DbSet<TipoDocumentoGP> TipoDocumentoGP { get; set; }
        public virtual DbSet<DefinicionProcesoGP> DefinicionProcesoGP { get; set; }
        public virtual DbSet<WorkflowGP> WorkflowGP { get; set; }
        public virtual DbSet<GrupoGP> GrupoGP { get; set; }
        public virtual DbSet<GDGP> GDGP { get; set; }
        public virtual DbSet<TipoAprobacionGP> TipoAprobacionGP { get; set; }
        public virtual DbSet<DefinicionWorkflowGP> DefinicionWorkflowGP { get; set; }
        public virtual DbSet<EstadoProcesoGP> EstadoProcesoGP { get; set; }

    }
}
