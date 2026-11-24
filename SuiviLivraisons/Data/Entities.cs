using System.ComponentModel.DataAnnotations.Schema;

namespace SuiviLivraisons.Data;

public enum StatutCommande { Commandee, Expediee, RecueComplete, RecuePartielle, Close }
public enum StatutExpedition { EnTransit, Recue }
public enum ModeTransport { Maritime, Aerien }
public enum EtatReception { Conforme, Endommage, Manquant }

public class Fournisseur
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public string? Contact { get; set; }
}

public class Etablissement
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public string Communaute { get; set; } = "";
    public string TypeEtab { get; set; } = "École";
}

public class BonCommande
{
    public int Id { get; set; }
    public string Numero { get; set; } = "";
    public int FournisseurId { get; set; }
    public Fournisseur? Fournisseur { get; set; }
    public int EtablissementId { get; set; }
    public Etablissement? Etablissement { get; set; }
    public DateTime DateCommande { get; set; }
    public StatutCommande Statut { get; set; } = StatutCommande.Commandee;
    public List<LigneCommande> Lignes { get; set; } = new();
    public List<Expedition> Expeditions { get; set; } = new();

    [NotMapped]
    public decimal MontantTotal => Lignes.Sum(l => l.QteCommandee * l.CoutUnitaire);
}

public class LigneCommande
{
    public int Id { get; set; }
    public int BonCommandeId { get; set; }
    public BonCommande? BonCommande { get; set; }
    public string Article { get; set; } = "";
    public int QteCommandee { get; set; }
    public decimal CoutUnitaire { get; set; }
}

public class Expedition
{
    public int Id { get; set; }
    public int BonCommandeId { get; set; }
    public BonCommande? BonCommande { get; set; }
    public ModeTransport Mode { get; set; }
    public string Transporteur { get; set; } = "";
    public string NoSuivi { get; set; } = "";
    public DateTime DateDepart { get; set; }
    public DateTime DateArriveePrevue { get; set; }
    public StatutExpedition Statut { get; set; } = StatutExpedition.EnTransit;
    public List<Reception> Receptions { get; set; } = new();
}

public class Reception
{
    public int Id { get; set; }
    public int ExpeditionId { get; set; }
    public Expedition? Expedition { get; set; }
    public DateTime DateReception { get; set; }
    public string RecuPar { get; set; } = "";
    public string? Commentaire { get; set; }
    public List<LigneReception> Lignes { get; set; } = new();
}

public class LigneReception
{
    public int Id { get; set; }
    public int ReceptionId { get; set; }
    public Reception? Reception { get; set; }
    public int LigneCommandeId { get; set; }
    public LigneCommande? LigneCommande { get; set; }
    public int QteRecue { get; set; }
    public EtatReception Etat { get; set; } = EtatReception.Conforme;
}
