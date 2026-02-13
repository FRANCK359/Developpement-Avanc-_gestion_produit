# Guide de Démarrage - AdvancedDevSample

## 📌 Table des matières
- [Introduction](#introduction)
- [Prérequis](#prérequis)
- [Installation](#installation)
- [Configuration](#configuration)
- [Premier lancement](#premier-lancement)
- [Vérification](#vérification)

---

## Introduction

**AdvancedDevSample** est une API REST .NET 10.0 complète et professionnelle pour la gestion d'un système commercial. Elle fournit une base solide pour apprendre ou déployer une architecture d'application .NET modernes avec bonnes pratiques.

### Qu'est-ce que vous obtenez ?
- ✅ Une API REST sécurisée avec authentification JWT
- ✅ Gestion complète des entités métier (Clients, Commandes, Produits, Fournisseurs)
- ✅ Architecture multi-couches (Domain, Application, Infrastructure, API)
- ✅ Tests unitaires et d'intégration
- ✅ Documentation interactive Swagger/OpenAPI
- ✅ Gestion centralisée des erreurs et logs
- ✅ Middlewares personnalisés pour performance et sécurité

---

## Prérequis

Avant de commencer, assurez-vous d'avoir installé :

| Élément | Version | Télécharger |
|--------|---------|------------|
| **.NET SDK** | 10.0+ | [dot.net](https://dot.net) |
| **Visual Studio** | 2026 ou plus récent | [VS Community](https://visualstudio.microsoft.com) |
| **Git** | Dernière version | [git-scm.com](https://git-scm.com) |
| **SQL Server** (optionnel) | 2019+ | [SQL Server](https://www.microsoft.com/sql-server) |

### Vérifier votre installation

Ouvrez un terminal et exécutez :

```powershell
# Vérifier .NET
dotnet --version

# Vérifier Git
git --version

# Vérifier Visual Studio (optionnel)
"C:\Program Files\Microsoft Visual Studio\2026\Community\Common7\IDE\devenv.exe" /? 2>$null
```

---

## Installation

### Étape 1 : Cloner le dépôt

```powershell
# Naviguer vers votre dossier de développement
cd C:\Dev

# Cloner le dépôt
git clone https://github.com/FRANCK359/Developpement-Avanc-_gestion_produit.git

# Entrer dans le dossier
cd Developpement-Avanc-_gestion_produit
```

### Étape 2 : Restaurer les dépendances NuGet

```powershell
# Restaurer tous les packages NuGet
dotnet restore

# Ou avec une architecture spécifique
dotnet restore --runtime win-x64
```

### Étape 3 : Vérifier la structure

```powershell
# Lister les projets dans la solution
dotnet sln Developpement-Avanc-_gestion_produit.sln list
```

Vous devriez voir :
```
AdvancedDevSample.Api
AdvancedDevSample.Application
AdvancedDevSample.Domain
AdvancedDevSample.Infrastructure
AdvancedDevSample.Test
```

---

## Configuration

### Fichier appsettings.json

Le fichier `AdvancedDevSample.Api/appsettings.json` contient la configuration globale :

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "JwtSettings": {
    "SecretKey": "AdvancedDevSampleSecretKey2024SecureKeyForJWTGeneration",
    "Issuer": "AdvancedDevSample",
    "Audience": "AdvancedDevSampleClients",
    "ExpirationMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AdvancedDevSample;Trusted_Connection=true;"
  }
}
```

### Fichier appsettings.Development.json

Pour le développement local :

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug"
    }
  },
  "JwtSettings": {
    "SecretKey": "AdvancedDevSampleSecretKey2024SecureKeyForJWTGenerationDevelopment"
  }
}
```

### Configuration de la base de données

#### Option 1 : LocalDB (recommandé pour le développement)
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AdvancedDevSample;Trusted_Connection=true;"
}
```

#### Option 2 : SQL Server distant
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=votre-serveur.database.windows.net;Database=AdvancedDevSample;User Id=sa;Password=YourPassword123!;"
}
```

#### Option 3 : SQLite (léger, idéal pour tests)
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=advanceddevsample.db"
}
```

---

## Premier lancement

### Étape 1 : Appliquer les migrations

```powershell
# Naviguer vers le dossier de l'API
cd AdvancedDevSample.Api

# Appliquer les migrations de base de données
dotnet ef database update --project ..\AdvancedDevSample.Infrastructure

# Vérifier que la base est créée
# La base de données AdvancedDevSample doit être visible dans SQL Server
```

### Étape 2 : Lancer l'API

#### Option A : Via ligne de commande
```powershell
# Depuis le dossier AdvancedDevSample.Api
dotnet run

# Ou depuis le dossier racine
dotnet run --project AdvancedDevSample.Api
```

#### Option B : Via Visual Studio
```powershell
# Ouvrir la solution dans Visual Studio
start Developpement-Avanc-_gestion_produit.sln

# Puis cliquer sur "▶ Run" ou appuyer sur F5
```

#### Option C : Via le terminal de Visual Studio
```
Debug → Start Debugging (F5)
```

### Étape 3 : Vérifier le démarrage

L'API devrait démarrer sur : `https://localhost:7000` ou `http://localhost:5000`

Vous devriez voir un message similaire :
```
Now listening on: https://localhost:7000
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

---

## Vérification

### Tester l'API avec Swagger

1. Ouvrez votre navigateur
2. Allez à : `https://localhost:7000/swagger/index.html`
3. Vous devriez voir l'interface Swagger avec tous les endpoints

### Tester un endpoint simple

```powershell
# Récupérer la liste des produits (sans authentification)
Invoke-WebRequest -Uri "https://localhost:7000/api/products" `
  -Headers @{"Content-Type" = "application/json"} `
  -SkipCertificateCheck

# Ou avec curl
curl -k https://localhost:7000/api/products
```

### Vérifier les logs

Les logs sont affichés dans la console. Cherchez les messages d'information :
```
[INFO] AdvancedDevSample.Api started successfully
[INFO] Swagger documentation available at /swagger
```

---

## Troubleshooting

### ❌ Erreur : "Port 7000 is already in use"

```powershell
# Trouver le processus utilisant le port
netstat -ano | findstr :7000

# Lancer sur un port différent
dotnet run --urls "https://localhost:7001"
```

### ❌ Erreur : "Database connection failed"

```powershell
# Vérifier la chaîne de connexion dans appsettings.json
# Créer/réparer LocalDB
sqllocaldb create mssqllocaldb
sqllocaldb start mssqllocaldb

# Relancer la migration
dotnet ef database update --project ..\AdvancedDevSample.Infrastructure
```

### ❌ Erreur : "No migrations pending"

```powershell
# Vérifier les migrations disponibles
dotnet ef migrations list --project ..\AdvancedDevSample.Infrastructure

# Créer une nouvelle migration si nécessaire
dotnet ef migrations add InitialCreate --project ..\AdvancedDevSample.Infrastructure
```

### ❌ Erreur : ".NET SDK not found"

```powershell
# Installer le SDK .NET 10.0
# Ou vérifier la version requise dans global.json
cat global.json
```

---

## Étapes suivantes

Une fois la configuration réussie, vous pouvez :

1. **Consulter la documentation technique** : Voir `02-ARCHITECTURE.md`
2. **Explorer l'API** : Voir `03-GUIDE_API.md`
3. **Comprendre les entités métier** : Voir `04-MODELES_DOMAINE.md`
4. **Développer des features** : Voir `05-GUIDE_DEVELOPPEMENT.md`
5. **Lancer les tests** : Voir `06-GUIDE_TESTS.md`

---

## Support et ressources

| Sujet | Lien |
|-------|------|
| Documentation .NET | [docs.microsoft.com](https://docs.microsoft.com/dotnet) |
| ASP.NET Core | [aspnetcore.readthedocs.io](https://aspnetcore.readthedocs.io) |
| Entity Framework Core | [docs.microsoft.com/ef](https://docs.microsoft.com/en-us/ef) |
| JWT Authentication | [jwt.io](https://jwt.io) |
| Repository GitHub | [github.com/FRANCK359](https://github.com/FRANCK359) |

---

**Besoin d'aide ?** Consultez le fichier `TROUBLESHOOTING.md` ou ouvrez une issue sur GitHub.

Bon développement ! 🚀
