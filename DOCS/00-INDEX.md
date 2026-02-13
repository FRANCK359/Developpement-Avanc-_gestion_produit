# INDEX - Documentation Complète AdvancedDevSample

## 📚 Navigation

Bienvenue dans la documentation complète du projet **AdvancedDevSample** - une API REST .NET professionnelle pour la gestion commerciale.

---

## 🚀 Pour les débutants

**Commencez par** :

1. **[01-GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md)** 🔌
   - Installation et configuration initiale
   - Premier lancement de l'application
   - Vérification du bon fonctionnement
   - **Durée estimée** : 30 minutes

2. **[03-GUIDE_API.md](03-GUIDE_API.md)** 📡
   - Vue d'ensemble de l'API
   - Authentification JWT
   - Exemples de requêtes simples
   - **Durée estimée** : 20 minutes

---

## 🏗️ Pour comprendre l'architecture

**Lisez dans cet ordre** :

1. **[02-ARCHITECTURE.md](02-ARCHITECTURE.md)** 🎯
   - Architecture en couches
   - Flux de données
   - Diagrammes des dépendances
   - Patterns et principes SOLID
   - **Durée estimée** : 45 minutes

2. **[04-MODELES_DOMAINE.md](04-MODELES_DOMAINE.md)** 📊
   - Entités métier (Customer, Product, Order, etc.)
   - Règles métier
   - Énumérations et événements de domaine
   - Interfaces et repositories
   - **Durée estimée** : 60 minutes

---

## 👨‍💻 Pour développer des nouvelles fonctionnalités

**Consultez** :

1. **[05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md)** 🛠️
   - Conventions de code (.NET et C#)
   - Structure de fichiers
   - Ajouter une nouvelle fonctionnalité (guide complet)
   - Gestion de la base de données (migrations)
   - Workflow Git
   - Debugging
   - **Durée estimée** : 90 minutes

2. **[06-GUIDE_TESTS.md](06-GUIDE_TESTS.md)** ✅
   - Architecture des tests (AAA Pattern)
   - Tests unitaires
   - Tests d'intégration
   - Tests de composants
   - Comment exécuter les tests
   - Bonnes pratiques et coverage
   - **Durée estimée** : 60 minutes

---

## 🚢 Pour déployer l'application

**Consultez** :

1. **[07-GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md)** ☁️
   - Déploiement local
   - Déploiement en production
   - Azure App Service
   - Docker & Conteneurisation
   - CI/CD avec GitHub Actions
   - Monitoring et logging
   - **Durée estimée** : 90 minutes

---

## 📖 Utilisations courantes

### Je veux...

#### ...démarrer rapidement
→ Allez à [01-GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md)

#### ...explorer les endpoints de l'API
→ Allez à [03-GUIDE_API.md](03-GUIDE_API.md)

#### ...comprendre comment les données circulent
→ Allez à [02-ARCHITECTURE.md](02-ARCHITECTURE.md)

#### ...ajouter une nouvelle entité (ex: Category)
→ Allez à [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) → Section "Ajouter une nouvelle fonctionnalité"

#### ...écrire des tests
→ Allez à [06-GUIDE_TESTS.md](06-GUIDE_TESTS.md)

#### ...déployer sur Azure
→ Allez à [07-GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md)

#### ...comprendre les règles métier
→ Allez à [04-MODELES_DOMAINE.md](04-MODELES_DOMAINE.md)

#### ...configurer le JWT
→ Allez à [03-GUIDE_API.md](03-GUIDE_API.md) → Section "Authentification JWT"

#### ...débugguer une erreur
→ Allez à [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) → Section "Debugging"

#### ...améliorer les performances
→ Allez à [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) → Section "Performance"

---

## 📋 Quick Reference

### Structure du projet

```
AdvancedDevSample/
├── AdvancedDevSample.Api/              # Couche présentation (Controllers, Filters, Middlewares)
├── AdvancedDevSample.Application/      # Couche application (Services, DTOs)
├── AdvancedDevSample.Domain/           # Couche métier (Entités, Enums, Events)
├── AdvancedDevSample.Infrastructure/   # Couche persistance (DbContext, Repositories)
├── AdvancedDevSample.Test/             # Tests (Unit, Integration, Component)
└── DOCS/                               # Documentation (ce projet)
```

### Endpoints principaux

| Entité | Méthode | Endpoint | Doc |
|--------|---------|----------|-----|
| **Produits** | GET | `/api/products` | [03-GUIDE_API.md](03-GUIDE_API.md#products-produits) |
| **Clients** | POST | `/api/customers` | [03-GUIDE_API.md](03-GUIDE_API.md#customers-clients) |
| **Commandes** | GET | `/api/orders` | [03-GUIDE_API.md](03-GUIDE_API.md#orders-commandes) |
| **Fournisseurs** | PUT | `/api/suppliers/{id}` | [03-GUIDE_API.md](03-GUIDE_API.md#suppliers-fournisseurs) |
| **Auth** | POST | `/api/auth/login` | [03-GUIDE_API.md](03-GUIDE_API.md#authentification-jwt) |

### Commandes essentielles

```powershell
# Setup initial
dotnet restore
dotnet ef database update --project ..\AdvancedDevSample.Infrastructure

# Développement
dotnet run --project AdvancedDevSample.Api
dotnet watch run --project AdvancedDevSample.Api

# Tests
dotnet test

# Base de données
dotnet ef migrations add MigrationName --project ..\AdvancedDevSample.Infrastructure
dotnet ef database update --project ..\AdvancedDevSample.Infrastructure

# Déploiement
dotnet publish -c Release -o ./publish
docker build -t advanceddevsample-api:latest .
```

### Configuration clés

| Fichier | Utilité | Voir |
|---------|---------|------|
| `appsettings.json` | Config globale | [01-GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md#configuration) |
| `appsettings.Development.json` | Config développement | [01-GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md#configuration-de-la-base-de-données) |
| `Program.cs` | Setup application | [02-ARCHITECTURE.md](02-ARCHITECTURE.md#configuration-de-programcs) |
| `.editorconfig` | Conventions code | [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md#editorconfig) |
| `Dockerfile` | Conteneurisation | [07-GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md#créer-un-dockerfile) |

---

## 🎯 Points clés à retenir

### Architecture
- ✅ **4 couches** : API, Application, Domain, Infrastructure
- ✅ **Dépendances unidirectionnelles** : API → Application → Domain ← Infrastructure
- ✅ **Séparation des responsabilités** : Chaque couche a un rôle bien défini
- ✅ **Testabilité** : Chaque couche peut être testée indépendamment

### Conventions
- ✅ **Classes publiques** : PascalCase
- ✅ **Champs privés** : _camelCase
- ✅ **Variables locales** : camelCase
- ✅ **Interfaces** : Commencent par I (IProductService)
- ✅ **Async/Await** : Obligatoire pour les opérations I/O

### Bonnes pratiques
- ✅ **Tests** : Coverage > 75%
- ✅ **Git** : Feature branches + Pull Requests
- ✅ **Commits** : Messages descriptifs
- ✅ **Code** : Clean code + SOLID principles
- ✅ **Documentation** : Toujours à jour

### Sécurité
- ✅ **Secrets** : Jamais en dur (Key Vault / Environment)
- ✅ **HTTPS** : Obligatoire en production
- ✅ **JWT** : Token-based authentication
- ✅ **CORS** : Restreint aux domaines autorisés
- ✅ **Validation** : Toutes les entrées validées

---

## 🔍 Dépannage rapide

### Problème : L'API ne démarre pas

1. Vérifier `appsettings.json` → Vérifier la ConnectionString
2. Vérifier la base de données → `dotnet ef database update`
3. Vérifier les ports → Modifier dans `launchSettings.json`
4. Vérifier les logs → Consulter la sortie de la console

**Document** : [01-GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md#troubleshooting)

### Problème : Les tests échouent

1. Lancer `dotnet test` pour voir les erreurs
2. Vérifier les mocks → Voir [06-GUIDE_TESTS.md](06-GUIDE_TESTS.md)
3. Vérifier la base de données → Utiliser InMemory pour les tests
4. Consulter les logs du test

**Document** : [06-GUIDE_TESTS.md](06-GUIDE_TESTS.md#troubleshooting)

### Problème : Erreur de migration

1. Vérifier les migrations → `dotnet ef migrations list`
2. Annuler une migration → `dotnet ef migrations remove`
3. Créer une nouvelle → `dotnet ef migrations add MigrationName`

**Document** : [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md#gérer-la-base-de-données)

---

## 📚 Ressources externes

### Documentation officielle
- [Microsoft .NET](https://learn.microsoft.com/dotnet)
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [JWT.io](https://jwt.io)

### Outils recommandés
- [Postman](https://www.postman.com) : Tester l'API
- [Visual Studio Community](https://visualstudio.microsoft.com) : IDE
- [SQL Server Management Studio](https://learn.microsoft.com/sql/ssms) : Gérer la DB
- [Azure Portal](https://portal.azure.com) : Déploiement cloud

### Exemple de projet complet
- [eShopOnWeb](https://github.com/dotnet-architecture/eShopOnWeb)
- [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

---

## 📞 Support et contribution

### Besoin d'aide ?
1. Consultez d'abord la documentation pertinente
2. Cherchez sur Google / Stack Overflow
3. Ouvrez une issue sur GitHub
4. Contactez l'équipe développement

### Contribuer
1. Créez une branche : `git checkout -b feature/my-feature`
2. Committez : `git commit -m "feat: description"`
3. Poussez : `git push origin feature/my-feature`
4. Ouvrez une Pull Request

---

## 📝 Versions de la documentation

| Version | Date | Changements |
|---------|------|-------------|
| 1.0 | Jan 2024 | Documentation initiale complète |
| 1.1 | Fév 2024 | Ajout du guide de déploiement |
| 1.2 | Mar 2024 | Mise à jour pour .NET 10.0 |

---

## 🎓 Parcours d'apprentissage suggéré

### Jour 1 : Découverte (3-4 heures)
- [01-GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md) : Installation et démarrage
- [03-GUIDE_API.md](03-GUIDE_API.md) : Exploration de l'API (sections principales)

### Jour 2 : Compréhension (4-5 heures)
- [02-ARCHITECTURE.md](02-ARCHITECTURE.md) : Architecture générale
- [04-MODELES_DOMAINE.md](04-MODELES_DOMAINE.md) : Modèles métier

### Jour 3 : Développement (5-6 heures)
- [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) : Ajouter une feature
- [06-GUIDE_TESTS.md](06-GUIDE_TESTS.md) : Écrire des tests

### Jour 4 : Déploiement (4-5 heures)
- [07-GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md) : Déployer l'application

**Total estimé** : 16-20 heures pour une maîtrise basique

---

## ✅ Checklist pour démarrer

- [ ] J'ai installé .NET 10.0 SDK
- [ ] J'ai cloné le repository
- [ ] J'ai restauré les dépendances (`dotnet restore`)
- [ ] J'ai appliqué les migrations (`dotnet ef database update`)
- [ ] Je peux lancer l'API (`dotnet run`)
- [ ] Je peux accéder à Swagger (`https://localhost:7000/swagger`)
- [ ] Je comprends l'architecture en 4 couches
- [ ] Je connais les entités principales (Customer, Order, Product, Supplier)
- [ ] Je sais comment créer une nouvelle migration
- [ ] Je peux lancer les tests (`dotnet test`)

---

## 🎉 Conclusion

Vous avez maintenant accès à une **documentation complète et professionnelle** couvrant :

✅ Installation et démarrage  
✅ Architecture et design patterns  
✅ Guide complet de l'API REST  
✅ Modèles métier et règles métier  
✅ Développement et bonnes pratiques  
✅ Tests unitaires et d'intégration  
✅ Déploiement et DevOps  

Commencez par le guide de démarrage, puis explorez les sections qui vous intéressent.

**Bon développement !** 🚀
