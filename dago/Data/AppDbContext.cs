using Microsoft.EntityFrameworkCore;
using dago.Models;

namespace dago.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // === DbSets ===
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<TipoRegiao> TiposRegiao { get; set; }
        public DbSet<LeadTimeCliente> LeadTimesCliente { get; set; }
        public DbSet<StatusEntrega> StatusesEntrega { get; set; }
        public DbSet<Ctrc> Ctrcs { get; set; }
        public DbSet<TipoAgenda> TiposAgenda { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<TipoOcorrencia> TiposOcorrencia { get; set; }
        public DbSet<OcorrenciaSistema> OcorrenciasSistema { get; set; }
        public DbSet<OcorrenciaAtendimento> OcorrenciasAtendimento { get; set; }
        public DbSet<ParticularidadeCliente> ParticularidadesCliente { get; set; }
        public DbSet<RegiaoEstado> RegioesEstados { get; set; }
        public DbSet<Unidade> Unidades { get; set; }

        // === Configurações de relacionamento ===
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacionamento 1:1 entre CTRC e ParticularidadeCliente
            modelBuilder.Entity<Ctrc>()
                .HasOne(c => c.ParticularidadeCliente)
                .WithOne(p => p.Ctrc)
                .HasForeignKey<ParticularidadeCliente>(p => p.CtrcId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N entre StatusEntrega e CTRC
            modelBuilder.Entity<StatusEntrega>()
                .HasMany(s => s.Ctrcs)
                .WithOne(c => c.StatusEntrega)
                .HasForeignKey(c => c.StatusEntregaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre Cliente e Ctrc
            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Ctrcs)
                .WithOne(ct => ct.Cliente)
                .HasForeignKey(ct => ct.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre Cidade e Ctrc
            modelBuilder.Entity<Cidade>()
                .HasMany(c => c.Ctrcs)
                .WithOne(ct => ct.CidadeDestino)
                .HasForeignKey(ct => ct.CidadeDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre Estado e Ctrc
            modelBuilder.Entity<Estado>()
                .HasMany(e => e.Ctrcs)
                .WithOne(ct => ct.EstadoDestino)
                .HasForeignKey(ct => ct.EstadoDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre CTRC e OcorrenciasSistema
            modelBuilder.Entity<Ctrc>()
                .HasMany(c => c.OcorrenciasSistema)
                .WithOne(o => o.Ctrc)
                .HasForeignKey(o => o.CtrcId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N entre CTRC e OcorrenciasAtendimento
            modelBuilder.Entity<Ctrc>()
                .HasMany(c => c.OcorrenciasAtendimento)
                .WithOne(o => o.Ctrc)
                .HasForeignKey(o => o.CtrcId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N entre CTRC e Agenda
            modelBuilder.Entity<Ctrc>()
                .HasMany(c => c.Agendas)
                .WithOne(a => a.Ctrc)
                .HasForeignKey(a => a.CtrcId)
                .OnDelete(DeleteBehavior.Cascade);

            // Estado (1) -> (N) Unidades
            modelBuilder.Entity<Estado>()
                .HasMany(e => e.Unidades)
                .WithOne(u => u.Estado)
                .HasForeignKey(u => u.EstadoId)
                .OnDelete(DeleteBehavior.Restrict);


            // Unidade (1) -> (N) CTRCs
            modelBuilder.Entity<Unidade>()
                .HasMany(u => u.Ctrcs)
                .WithOne(c => c.Unidade)
                .HasForeignKey(c => c.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RegiaoEstado>()
                .HasMany(r => r.Estados)
                .WithOne(e => e.RegiaoEstado)
                .HasForeignKey(e => e.RegiaoEstadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeadTimeCliente>()
                .HasOne(l => l.RegiaoEstado)
                .WithMany()
                .HasForeignKey(l => l.RegiaoEstadoId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
