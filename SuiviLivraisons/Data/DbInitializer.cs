using Microsoft.EntityFrameworkCore;

namespace SuiviLivraisons.Data;

/// <summary>
/// Données fictives de démonstration. Seuls les noms des 14 communautés du Nunavik sont réels.
/// </summary>
public static class DbInitializer
{
    private static readonly string[] Communautes =
    [
        "Kuujjuarapik","Umiujaq", "Inukjuak", "Puvirnituq", "Akulivik", "Ivujivik", "Salluit",
        "Kangiqsujuaq", "Quaqtaq", "Kangirsuk", "Aupaluk", "Tasiujaq", "Kuujjuaq", "Kangiqsualujjuaq"
    ];

    public static void Initialiser(AppDbContext db)
    {
        
        db.Database.EnsureCreated();
        if (db.Etablissements.Any()) return;

        var etabs = Communautes
            .Select(c => new Etablissement { Nom = $"École de {c}", Communaute = c })
            .ToList();
        var centreAdultes = new Etablissement
        {
            Nom = "Centre d’éducation des adultes de Kuujjuaq",
            Communaute = "Kuujjuaq",
            TypeEtab = "Centre adultes"
        };
        etabs.Add(centreAdultes);
        db.Etablissements.AddRange(etabs);

        var fournitures = new Fournisseur { Nom = "Fournitures scolaires Nord (fictif)", Contact = "ventes@fournitures-nord.example" };
        var informatique = new Fournisseur { Nom = "Informatique Boréale (fictif)", Contact = "commandes@boreale.example" };
        var mobilier = new Fournisseur { Nom = "Mobilier Laurentien (fictif)", Contact = "service@mobilier.example" };
        db.Fournisseurs.AddRange(fournitures, informatique, mobilier);

        Etablissement E(string communaute) => etabs.First(e => e.Communaute == communaute && e.TypeEtab == "École");
        var aujourdhui = DateTime.Today;
        var annee = aujourdhui.Year;

        // 1. Salluit : expédiée par avion, en retard
        var bc1 = new BonCommande
        {
            Numero = $"BC-{annee}-0001", Fournisseur = informatique, Etablissement = E("Salluit"),
            DateCommande = aujourdhui.AddDays(-30), Statut = StatutCommande.Expediee,
            Lignes =
            [
                new() { Article = "Ordinateurs portables", QteCommandee = 20, CoutUnitaire = 650m },
                new() { Article = "Chargeurs de rechange", QteCommandee = 20, CoutUnitaire = 40m }
            ]
        };
        bc1.Expeditions.Add(new Expedition
        {
            Mode = ModeTransport.Aerien, Transporteur = "Cargo aérien (fictif)", NoSuivi = "AIR-48213",
            DateDepart = aujourdhui.AddDays(-12), DateArriveePrevue = aujourdhui.AddDays(-5)
        });

        // 2. Inukjuak : expédiée par bateau, dans les délais
        var bc2 = new BonCommande
        {
            Numero = $"BC-{annee}-0002", Fournisseur = fournitures, Etablissement = E("Inukjuak"),
            DateCommande = aujourdhui.AddDays(-45), Statut = StatutCommande.Expediee,
            Lignes =
            [
                new() { Article = "Cahiers d’exercices", QteCommandee = 500, CoutUnitaire = 2.50m },
                new() { Article = "Boîtes de crayons", QteCommandee = 200, CoutUnitaire = 6m }
            ]
        };
        bc2.Expeditions.Add(new Expedition
        {
            Mode = ModeTransport.Maritime, Transporteur = "Ravitaillement maritime (fictif)", NoSuivi = "SEA-2026-117",
            DateDepart = aujourdhui.AddDays(-8), DateArriveePrevue = aujourdhui.AddDays(10)
        });

        // 3. Centre adultes de Kuujjuaq : reçue partiellement (chaises manquantes)
        var lTables = new LigneCommande { Article = "Tables de travail", QteCommandee = 15, CoutUnitaire = 180m };
        var lChaises = new LigneCommande { Article = "Chaises", QteCommandee = 30, CoutUnitaire = 75m };
        var bc3 = new BonCommande
        {
            Numero = $"BC-{annee}-0003", Fournisseur = mobilier, Etablissement = centreAdultes,
            DateCommande = aujourdhui.AddDays(-60), Statut = StatutCommande.RecuePartielle,
            Lignes = [lTables, lChaises]
        };
        var exp3 = new Expedition
        {
            Mode = ModeTransport.Maritime, Transporteur = "Ravitaillement maritime (fictif)", NoSuivi = "SEA-2026-089",
            DateDepart = aujourdhui.AddDays(-40), DateArriveePrevue = aujourdhui.AddDays(-20), Statut = StatutExpedition.Recue
        };
        exp3.Receptions.Add(new Reception
        {
            DateReception = aujourdhui.AddDays(-19), RecuPar = "Responsable du centre",
            Commentaire = "4 chaises manquantes à la livraison.",
            Lignes =
            [
                new() { LigneCommande = lTables, QteRecue = 15, Etat = EtatReception.Conforme },
                new() { LigneCommande = lChaises, QteRecue = 26, Etat = EtatReception.Manquant }
            ]
        });
        bc3.Expeditions.Add(exp3);

        // 4. Puvirnituq : commandée, pas encore expédiée
        var bc4 = new BonCommande
        {
            Numero = $"BC-{annee}-0004", Fournisseur = fournitures, Etablissement = E("Puvirnituq"),
            DateCommande = aujourdhui.AddDays(-3), Statut = StatutCommande.Commandee,
            Lignes = [new() { Article = "Manuels de mathématiques", QteCommandee = 60, CoutUnitaire = 45m }]
        };

        // 5. Kangiqsualujjuaq : expédiée par avion, en retard
        var bc5 = new BonCommande
        {
            Numero = $"BC-{annee}-0005", Fournisseur = informatique, Etablissement = E("Kangiqsualujjuaq"),
            DateCommande = aujourdhui.AddDays(-20), Statut = StatutCommande.Expediee,
            Lignes = [new() { Article = "Projecteurs", QteCommandee = 4, CoutUnitaire = 900m }]
        };
        bc5.Expeditions.Add(new Expedition
        {
            Mode = ModeTransport.Aerien, Transporteur = "Cargo aérien (fictif)", NoSuivi = "AIR-48377",
            DateDepart = aujourdhui.AddDays(-6), DateArriveePrevue = aujourdhui.AddDays(-2)
        });

        // 6. Ivujivik : reçue complète
        var lBallons = new LigneCommande { Article = "Ballons et matériel d’éducation physique", QteCommandee = 10, CoutUnitaire = 35m };
        var bc6 = new BonCommande
        {
            Numero = $"BC-{annee}-0006", Fournisseur = fournitures, Etablissement = E("Ivujivik"),
            DateCommande = aujourdhui.AddDays(-50), Statut = StatutCommande.RecueComplete,
            Lignes = [lBallons]
        };
        var exp6 = new Expedition
        {
            Mode = ModeTransport.Aerien, Transporteur = "Cargo aérien (fictif)", NoSuivi = "AIR-47950",
            DateDepart = aujourdhui.AddDays(-35), DateArriveePrevue = aujourdhui.AddDays(-30), Statut = StatutExpedition.Recue
        };
        exp6.Receptions.Add(new Reception
        {
            DateReception = aujourdhui.AddDays(-30), RecuPar = "Direction de l’école",
            Lignes = [new() { LigneCommande = lBallons, QteRecue = 10, Etat = EtatReception.Conforme }]
        });
        bc6.Expeditions.Add(exp6);

        db.BonsCommande.AddRange(bc1, bc2, bc3, bc4, bc5, bc6);
        db.SaveChanges();
    }
}
