# AdvancedDevSample

## Présentation

**AdvancedDevSample** est une solution .NET 10.0 en architecture multi-couches, fournissant une API REST pour la gestion de données (clients, commandes, produits, fournisseurs, utilisateurs), avec infrastructure de tests et prise en charge de l'authentification JWT.

---

## Table des matières

- [Structure du projet](#structure-du-projet)
- [Technologies utilisées](#technologies-utilisées)
- [Installation & démarrage](#installation--démarrage)
- [Utilisation de l'API](#utilisation-de-lapi)
- [Tests](#tests)
- [Bonnes pratiques et conventions](#bonnes-pratiques-et-conventions)
- [Contribution](#contribution)
- [Licence](#licence)

---

## Structure du projet

```
├── AdvancedDevSample.slnx
├── .github/workflows/build.yml        # CI/CD
├── AdvancedDevSample.Api/             # API ASP.NET Core
│   ├── Controllers/                   # Contrôleurs REST
│   ├── Filters/                       # Filtres MVC personnalisés
│   ├── Middlewares/                   # Middlewares personnalisés
│   ├── Properties/launchSettings.json # Config lancement
│   ├── appsettings.json               # Config globale
│   ├── appsettings.Development.json   # Config dev
├── AdvancedDevSample.Application/     # Logique métier (Services, DTOs)
│   ├── Interfaces/Services/           # Interfaces & Implémentations des services
│   ├── DTOs/                          # Objets de transfert
│   ├── Exceptions/                    # Exceptions métier
├── AdvancedDevSample.Domain/          # Modèles de domaine (Entités, événements, enums)
│   ├── Entities/                      # Entités principales
│   ├── Events/                        # Événements de domaine
│   ├── Exceptions/                    # Exceptions de domaine
│   ├── Interfaces/                    # Interfaces des repos
├── AdvancedDevSample.Infrastructure/  # Persistance (EF Core, migrations)
│   ├── DbContext/                     # DbContext EF Core
│   ├── Repositories/                  # Implémentations des repos Entity Framework
│   ├── Migrations/                    # Scripts de migration EF
├── AdvancedDevSample.Test/            # Tests unitaires & intégration
│   ├── Application/                   # Tests des services
│   ├── Domain/                        # Tests des entités/contrôleurs
│   ├── Integration/                   # Tests d'intégration API
│   ├── Components/                    # Tests composants
```

---

## Technologies utilisées

- **.NET 10.0**
- **ASP.NET Core** (API REST, contrôleurs)
- **Entity Framework Core** (ORM, migrations)
- **JWT Authentication**
- **xUnit** (tests unitaires, intégrations)
- **Swashbuckle/Swagger** (documentation API)
- **Moq** (mocking pour tests)
- **CI/CD avec GitHub Actions**

---

## Installation & démarrage

### Prérequis

- Visual Studio 2026 ou plus récent
- .NET SDK 10.0+

### Étapes

1. **Cloner le dépôt :**
   ```bash
   git clone <URL>
   cd AdvancedDevSample
   ```

2. **Restaurer les packages NuGet :**
   ```bash
   dotnet restore
   ```

3. **Lancer la base de données (si applicable, voir `appsettings.json`)**

4. **Lancer l'API :**
   ```bash
   dotnet run --project AdvancedDevSample.Api
   ```
   L'API sera accessible sur [http://localhost:5000](http://localhost:5000) (selon configuration).

---

## Utilisation de l'API

### Documentation interactive

- Accès à Swagger UI : [http://localhost:5000/swagger](http://localhost:5000/swagger)

### Exemple : Créer un client

```http
POST /api/customers
Content-Type: application/json

{
  "name": "Nouveau Client",
  "email": "client@example.com"
}
```
**Réponse :**
```json
{
  "id": 1,
  "name": "Nouveau Client",
  "email": "client@example.com"
}
```

**Endpoints principaux :**
- `/api/auth` : Authentification
- `/api/customers` : Gestion des clients
- `/api/orders` : Gestion des commandes
- `/api/products` : Gestion des produits
- `/api/suppliers` : Gestion des fournisseurs

Voir Swagger pour tous les endpoints.

---

## Tests

- **Test unitaire Services/Application**
- **Test de composants**
- **Test d’intégration API**
- Pour exécuter tous les tests :
  ```bash
  dotnet test
  ```

---

## Bonnes pratiques et conventions

- **Architecture en couches séparées** (Domain, Application, Infrastructure, API)
- **Utilisation de DTOs pour l’échange de données**
- **Exceptions personnalisées pour chaque couche**
- **Filtres et middlewares pour la gestion des erreurs, logs et validation**
- **Utilisation du pattern Repository & UnitOfWork**
- **Tests couvrant logique métier et endpoints**

---

## Contribution

1. Forkez le repo, puis créez une branche : `feature/<nom>`
2. Faites vos modifications
3. Ouvrez un Pull Request
4. Respectez le format des commits et la structure du projet

---

## Licence

Ce projet est sous licence MIT (Adapter selon votre cas).

---

## Contact

Pour toute question ou suggestion :
- [Votre email/contact]
- [Issues GitHub]
