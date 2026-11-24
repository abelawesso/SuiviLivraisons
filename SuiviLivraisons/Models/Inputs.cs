using System.ComponentModel.DataAnnotations;
using SuiviLivraisons.Data;

namespace SuiviLivraisons.Models;

public class CommandeInput
{
    [Range(1, int.MaxValue, ErrorMessage = "Choisissez un établissement.")]
    public int EtablissementId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Choisissez un fournisseur.")]
    public int FournisseurId { get; set; }

    // Trois lignes fixes : suffisant pour un prototype en rendu statique (sans JavaScript).
    public List<LigneInput> Lignes { get; set; } = [new(), new(), new()];
}

public class LigneInput
{
    public string? Article { get; set; }
    public int Quantite { get; set; }
    public decimal CoutUnitaire { get; set; }
}

public class ExpeditionInput
{
    public ModeTransport Mode { get; set; } = ModeTransport.Aerien;

    [Required(ErrorMessage = "Le transporteur est obligatoire.")]
    public string? Transporteur { get; set; }

    [Required(ErrorMessage = "Le numéro de suivi est obligatoire.")]
    public string? NoSuivi { get; set; }

    public DateTime DateDepart { get; set; } = DateTime.Today;
    public DateTime DateArriveePrevue { get; set; } = DateTime.Today.AddDays(7);
}
