using Microsoft.EntityFrameworkCore;

namespace SuiviLivraisons.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Fournisseur> Fournisseurs => Set<Fournisseur>();
    public DbSet<Etablissement> Etablissements => Set<Etablissement>();
    public DbSet<BonCommande> BonsCommande => Set<BonCommande>();
    public DbSet<LigneCommande> LignesCommande => Set<LigneCommande>();
    public DbSet<Expedition> Expeditions => Set<Expedition>();
    public DbSet<Reception> Receptions => Set<Reception>();
    public DbSet<LigneReception> LignesReception => Set<LigneReception>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Les énumérations sont stockées en texte : plus lisible dans SQL Server et dans les rapports.
        mb.Entity<BonCommande>().Property(b => b.Statut).HasConversion<string>().HasMaxLength(20);
        mb.Entity<BonCommande>().HasIndex(b => b.Numero).IsUnique();
        mb.Entity<BonCommande>().Property(b => b.Numero).HasMaxLength(20);

        mb.Entity<LigneCommande>().Property(l => l.CoutUnitaire).HasPrecision(12, 2);

        mb.Entity<Expedition>().Property(e => e.Statut).HasConversion<string>().HasMaxLength(20);
        mb.Entity<Expedition>().Property(e => e.Mode).HasConversion<string>().HasMaxLength(20);
        mb.Entity<Expedition>().HasIndex(e => e.NoSuivi);

        mb.Entity<LigneReception>().Property(l => l.Etat).HasConversion<string>().HasMaxLength(20);

        // SQL Server refuse plusieurs chemins de suppression en cascade vers LigneReception
        // (via Reception et via LigneCommande) : on désactive la cascade sur LigneCommande.
        mb.Entity<LigneReception>()
            .HasOne(l => l.LigneCommande)
            .WithMany()
            .HasForeignKey(l => l.LigneCommandeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
