# Suivi des livraisons — prototype

Prototype personnel de réflexion autour d’un besoin décrit dans une offre d’emploi : suivre les marchandises
commandées et livrées dans des établissements scolaires répartis dans 14 communautés éloignées (Nunavik).
**Toutes les données sont fictives**, seuls les noms des communautés sont réels.

## Ce que fait le prototype

| Écran | Utilisateurs visés | Rendu |
|---|---|---|
| Tableau de bord : expéditions en transit, en retard, réceptions avec écarts, valeur en transit par communauté | Direction, achats, finances | Statique côté serveur |
| Commandes : liste filtrable par statut, détail avec expéditions, réceptions et écarts | Achats, finances | Statique côté serveur |
| Nouvelle commande, enregistrement d’une expédition (bateau ou avion) | Achats, transports | Statique côté serveur (formulaires) |
| Réception : saisie des quantités reçues et de l’état, file d’attente hors ligne | Personnel des établissements | Interactif WebAssembly |

Parcours d’une marchandise : **Commandée → Expédiée (en transit) → Reçue complète / Reçue partielle**.

## Choix techniques et raisons

- **Blazor en rendu hybride (.NET 10).** Les pages de gestion sont rendues côté serveur sans connexion
  permanente : elles restent légères et fonctionnent comme des pages Web classiques. Blazor Server aurait
  exigé une connexion SignalR continue, fragile sur les liaisons satellite des communautés.
- **Seul l’écran de réception est interactif, en WebAssembly.** Il s’exécute dans le navigateur et appelle
  une API. Si la connexion est coupée, la réception est conservée dans le navigateur (`localStorage`) et
  renvoyée automatiquement au retour de la connexion.
- **API idempotente.** Une réception renvoyée deux fois (par exemple après une coupure) n’est pas dupliquée :
  l’API répond 409 et le client la considère comme déjà traitée.
- **SQL Server + Entity Framework Core**, énumérations stockées en texte pour des données lisibles dans les rapports.
  La suppression en cascade est désactivée sur une relation pour éviter les chemins de cascade multiples refusés par SQL Server.

## Lancer le projet

Prérequis : SDK .NET 10 et SQL Server LocalDB (installé avec Visual Studio).

```powershell
cd SuiviLivraisons
dotnet run
```

Puis ouvrir http://localhost:5080. La base `SuiviLivraisons` est créée au premier démarrage avec des données de démonstration.
Pour repartir de zéro : `sqllocaldb` ou SSMS, supprimer la base `SuiviLivraisons`, puis relancer.

### Tester le mode hors ligne

1. Ouvrir **Réception**, choisir une expédition.
2. Dans les outils de développement du navigateur (F12 → Réseau), passer en **Hors connexion**.
3. Confirmer la réception : elle passe en file d’attente sur l’appareil.
4. Repasser en ligne, puis cliquer sur **Réessayer l’envoi** (ou recharger la page) : la réception est envoyée.

## Limites assumées (prototype) et suites possibles

- Pas d’authentification : en production, connexion avec les comptes Office 365 (Microsoft Entra ID) et rôles
  (achats, transports, finances, établissement).
- `EnsureCreated` au lieu des migrations EF Core.
- Pas encore de gestion du traitement des écarts (réclamation, renvoi, remplacement), ni de rapprochement
  commande / réception / facture pour les finances.
- Pas de tests automatisés ; la file hors ligne pourrait évoluer vers une application Web progressive (PWA) avec service worker.
- Intégration à prévoir avec le système financier existant et, si possible, avec le suivi des transporteurs.
