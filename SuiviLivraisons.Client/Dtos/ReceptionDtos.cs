namespace SuiviLivraisons.Client.Dtos;

// Objets échangés entre l'écran de réception (navigateur) et l'API (serveur).

public record ExpeditionARecevoirDto(
    int Id,
    string NoSuivi,
    string NumeroCommande,
    string Etablissement,
    string Communaute,
    string Mode,
    DateTime DateArriveePrevue);

public record LigneAReceptionnerDto(int LigneCommandeId, string Article, int QteCommandee);

public class ReceptionDto
{
    public int ExpeditionId { get; set; }
    public string NoSuivi { get; set; } = "";          // affichage seulement (file d'attente hors ligne)
    public string RecuPar { get; set; } = "";
    public string? Commentaire { get; set; }
    public DateTime SaisieLe { get; set; } = DateTime.Now;
    public List<LigneReceptionDto> Lignes { get; set; } = new();
}

public class LigneReceptionDto
{
    public int LigneCommandeId { get; set; }
    public string Article { get; set; } = "";
    public int QteCommandee { get; set; }
    public int QteRecue { get; set; }
    public string Etat { get; set; } = "Conforme";     // Conforme, Endommage ou Manquant
}
