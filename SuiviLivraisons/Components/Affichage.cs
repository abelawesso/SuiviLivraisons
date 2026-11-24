using System.Globalization;
using SuiviLivraisons.Data;

namespace SuiviLivraisons.Components;

/// <summary>Libellés et couleurs d'affichage, regroupés pour rester cohérents dans toutes les pages.</summary>
public static class Affichage
{
    private static readonly CultureInfo FrCa = new("fr-CA");

    public static (string Css, string Libelle) Statut(StatutCommande statut) => statut switch
    {
        StatutCommande.Commandee => ("bg-secondary", "Commandée"),
        StatutCommande.Expediee => ("bg-primary", "Expédiée"),
        StatutCommande.RecueComplete => ("bg-success", "Reçue (complète)"),
        StatutCommande.RecuePartielle => ("bg-warning text-dark", "Reçue (partielle)"),
        StatutCommande.Close => ("bg-dark", "Close"),
        _ => ("bg-light text-dark", statut.ToString())
    };

    public static string Mode(ModeTransport mode) =>
        mode == ModeTransport.Maritime ? "Maritime (sealift)" : "Aérien";

    public static string Etat(EtatReception etat) => etat switch
    {
        EtatReception.Conforme => "Conforme",
        EtatReception.Endommage => "Endommagé",
        EtatReception.Manquant => "Manquant",
        _ => etat.ToString()
    };

    public static string Argent(decimal montant) => montant.ToString("C", FrCa);

    public static string Date(DateTime date) => date.ToString("d MMMM yyyy", FrCa);
}
