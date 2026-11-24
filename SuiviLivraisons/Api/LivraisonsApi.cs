using Microsoft.EntityFrameworkCore;
using SuiviLivraisons.Client.Dtos;
using SuiviLivraisons.Data;

namespace SuiviLivraisons.Api;

/// <summary>
/// API utilisée par l'écran de réception (composant WebAssembly exécuté dans le navigateur).
/// </summary>
public static class LivraisonsApi
{
    public static void MapLivraisonsApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapGet("/expeditions/a-recevoir", async (AppDbContext db) =>
        {
            var expeditions = await db.Expeditions
                .Include(e => e.BonCommande!).ThenInclude(b => b.Etablissement)
                .Where(e => e.Statut == StatutExpedition.EnTransit)
                .OrderBy(e => e.DateArriveePrevue)
                .ToListAsync();

            return expeditions.Select(e => new ExpeditionARecevoirDto(
                e.Id,
                e.NoSuivi,
                e.BonCommande!.Numero,
                e.BonCommande.Etablissement!.Nom,
                e.BonCommande.Etablissement.Communaute,
                e.Mode == ModeTransport.Maritime ? "Maritime" : "Aérien",
                e.DateArriveePrevue));
        });

        api.MapGet("/expeditions/{id:int}/lignes", async (int id, AppDbContext db) =>
        {
            var expedition = await db.Expeditions
                .Include(e => e.BonCommande!).ThenInclude(b => b.Lignes)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expedition is null) return Results.NotFound();

            var lignes = expedition.BonCommande!.Lignes
                .Select(l => new LigneAReceptionnerDto(l.Id, l.Article, l.QteCommandee));
            return Results.Ok(lignes);
        });

        api.MapPost("/receptions", async (ReceptionDto dto, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.RecuPar))
                return Results.BadRequest("Le nom de la personne qui reçoit est obligatoire.");

            var expedition = await db.Expeditions
                .Include(e => e.BonCommande!).ThenInclude(b => b.Lignes)
                .FirstOrDefaultAsync(e => e.Id == dto.ExpeditionId);

            if (expedition is null) return Results.NotFound();

            // Idempotence : une réception déjà enregistrée (par exemple renvoyée après une coupure) n'est pas dupliquée.
            if (expedition.Statut == StatutExpedition.Recue)
                return Results.Conflict("Cette expédition a déjà été réceptionnée.");

            var lignesCommande = expedition.BonCommande!.Lignes;
            var lignesValides = dto.Lignes.Where(l => lignesCommande.Any(lc => lc.Id == l.LigneCommandeId)).ToList();

            var reception = new Reception
            {
                ExpeditionId = expedition.Id,
                DateReception = DateTime.Now,
                RecuPar = dto.RecuPar.Trim(),
                Commentaire = string.IsNullOrWhiteSpace(dto.Commentaire) ? null : dto.Commentaire.Trim(),
                Lignes = lignesValides.Select(l => new LigneReception
                {
                    LigneCommandeId = l.LigneCommandeId,
                    QteRecue = Math.Max(0, l.QteRecue),
                    Etat = Enum.TryParse<EtatReception>(l.Etat, out var etat) ? etat : EtatReception.Conforme
                }).ToList()
            };

            var complete = lignesCommande.All(lc => reception.Lignes.Any(lr =>
                lr.LigneCommandeId == lc.Id && lr.QteRecue >= lc.QteCommandee && lr.Etat == EtatReception.Conforme));

            db.Receptions.Add(reception);
            expedition.Statut = StatutExpedition.Recue;
            expedition.BonCommande.Statut = complete ? StatutCommande.RecueComplete : StatutCommande.RecuePartielle;
            await db.SaveChangesAsync();

            return Results.Created($"/api/receptions/{reception.Id}", new { reception.Id, Complete = complete });
        });
    }
}
