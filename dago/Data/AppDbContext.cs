using dago.Models;
using Microsoft.EntityFrameworkCore;

namespace dago.Data;

public class AppDbContext : DbContext

{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
    }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Cargo> Cargos { get; set; }    



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

        modelBuilder.Entity<Cliente>()
            .HasOne(c=> c.Usuario)
            .WithMany(u => u.Clientes)
            .HasForeignKey(c=> c.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Cargo)
                .WithMany(c => c.Usuarios)
                .HasForeignKey(u => u.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

    }
}
