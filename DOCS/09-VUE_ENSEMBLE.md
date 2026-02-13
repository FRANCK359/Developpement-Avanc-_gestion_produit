# 📊 Vue d'ensemble visuelle - AdvancedDevSample

## 🎯 En une page

### Qu'est-ce que c'est?

**AdvancedDevSample** est une **API REST .NET 10.0 professionnelle** pour gérer :
- 👥 Clients
- 📦 Produits
- 📋 Commandes
- 🏢 Fournisseurs
- 🔐 Authentification (JWT)

### Architecture

```
┌─────────────────────────────────────┐
│    API REST (Controllers)           │  ← Endpoints HTTP
├─────────────────────────────────────┤
│   Application (Services)            │  ← Logique métier
├─────────────────────────────────────┤
│   Domain (Entities)                 │  ← Règles métier
├─────────────────────────────────────┤
│   Infrastructure (Database)         │  ← Persistance
└─────────────────────────────────────┘
```

### Starter - 5 minutes

```bash
git clone <repo>
cd Developpement-Avanc-_gestion_produit
dotnet restore
dotnet ef database update --project .\AdvancedDevSample.Infrastructure
dotnet run --project .\AdvancedDevSample.Api
```

→ `https://localhost:7000/swagger`

---

## 🗺️ Roadmap d'apprentissage

```
START HERE
    ↓
[00-INDEX.md]
    ↓
┌─────────────────────┐
│ Nouveau sur projet? │
└─────────────────────┘
    ↓
[01-GUIDE_DEMARRAGE.md]
    ↓
[03-GUIDE_API.md]
    ↓
    └─→ [02-ARCHITECTURE.md] (optionnel mais recommandé)
         ↓
    ┌─────────────────────┐
    │ Prêt à coder?       │
    └─────────────────────┘
         ↓
    [04-MODELES_DOMAINE.md]
         ↓
    [05-GUIDE_DEVELOPPEMENT.md]
         ↓
    [06-GUIDE_TESTS.md]
         ↓
    [07-GUIDE_DEPLOIEMENT.md]
         ↓
    Professionnel! 🎓
```

---

## 📚 Les 8 documents

### 📌 00 - INDEX
**Navigation complète du projet**
- Tous les liens
- Quick reference
- Commandes essentielles

### 🚀 01 - DÉMARRAGE
**Installation et configuration**
- Prérequis (.NET, Visual Studio)
- Installation étape par étape
- Configuration basique
- Premier lancement

### 🏗️ 02 - ARCHITECTURE
**Comment le projet est structuré**
- 4 couches expliquées
- Flux de données
- Patterns SOLID
- Diagrammes

### 📡 03 - API
**Guide complet de l'API REST**
- Endpoints complets
- Authentication JWT
- Exemples de code
- Tous les DTOs

### 📊 04 - MODÈLES MÉTIER
**Entités et règles de domaine**
- Customer, Product, Order, etc.
- Règles métier
- Énumérations
- Événements domaine

### 👨‍💻 05 - DÉVELOPPEMENT
**Ajouter des fonctionnalités**
- Conventions de code
- Workflow Git
- Migrations BD
- Debugging

### ✅ 06 - TESTS
**Écrire des tests**
- Unit tests
- Integration tests
- Fixtures et patterns
- Best practices

### ☁️ 07 - DÉPLOIEMENT
**Mettre en production**
- Azure App Service
- Docker
- CI/CD (GitHub Actions)
- Monitoring

### 🔧 08 - TROUBLESHOOTING
**Résoudre les problèmes**
- Erreurs courantes
- Solutions
- FAQ
- Débogage

---

## 🎯 Cas d'usage - Par profil

### Je suis étudiant / Junior Dev
1. [00-INDEX.md](00-INDEX.md) - Comprenez la doc
2. [01-GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md) - Installez
3. [03-GUIDE_API.md](03-GUIDE_API.md) - Testez l'API
4. [02-ARCHITECTURE.md](02-ARCHITECTURE.md) - Comprenez le design
5. [04-MODELES_DOMAINE.md](04-MODELES_DOMAINE.md) - Apprenez le métier
6. [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) - Codez
7. [06-GUIDE_TESTS.md](06-GUIDE_TESTS.md) - Testez

**Temps**: ~20 heures

### Je suis développeur expérimenté
1. [00-INDEX.md](00-INDEX.md) - Navigation rapide
2. [02-ARCHITECTURE.md](02-ARCHITECTURE.md) - Vue d'ensemble
3. [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) - Conventions
4. [07-GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md) - Déploiement

**Temps**: ~5 heures

### Je dois déployer en prod
1. [01-GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md) - Configuration
2. [07-GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md) - Déploiement
3. [08-TROUBLESHOOTING_FAQ.md](08-TROUBLESHOOTING_FAQ.md) - Problèmes

**Temps**: ~3 heures

### J'ai un bug à fixer
1. [08-TROUBLESHOOTING_FAQ.md](08-TROUBLESHOOTING_FAQ.md) - Solutions rapides
2. [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md#debugging) - Debugging
3. [06-GUIDE_TESTS.md](06-GUIDE_TESTS.md) - Écrire un test

**Temps**: 30 minutes à 2 heures selon la complexité

---

## 💡 Concepts clés

### Architecture en couches

| Couche | Responsabilité | Exemple |
|--------|----------------|---------|
| **API** | Endpoints HTTP | `/api/products` |
| **Application** | Logique métier | `ProductService` |
| **Domain** | Règles domaine | Entité `Product` |
| **Infrastructure** | Persistance | EF Core + SQL |

### Flux d'une requête

```
Client HTTP
    ↓
POST /api/products
    ↓
[ProductsController]
- Valide les données
- Vérifie l'auth
    ↓
[ProductService]
- Applique les règles métier
- Appelle le repo
    ↓
[EfProductRepository]
- Utilise EF Core
- Accède à la BD
    ↓
[Database]
- SQL Server
- Retour des données
    ↓
JSON Response
```

### Entités principales et relations

```
Customer ←→ Order ←→ OrderItem ←→ Product ←→ Supplier
   1:N       1:N        1:N         N:1
```

### Les rôles/permissions

```
Admin
├─ Gérer tous les produits
├─ Gérer tous les clients
├─ Gérer tous les fournisseurs
└─ Voir tous les rapports

Manager
├─ Gérer les commandes
├─ Voir les clients
└─ Générrer des rapports

User
└─ Consulter les produits
```

---

## ⚡ Quick Commands

### Démarrer
```bash
dotnet run --project .\AdvancedDevSample.Api
```

### Tester
```bash
dotnet test
```

### Ajouter une migration
```bash
dotnet ef migrations add MigrationName --project .\AdvancedDevSample.Infrastructure
```

### Déployer (Docker)
```bash
docker build -t myapp:latest .
docker run -p 5000:80 myapp:latest
```

---

## 📖 Structure du projet

```
AdvancedDevSample/
│
├── 📚 DOCS/                          ← VOUS ÊTES ICI
│   ├── 00-INDEX.md                   ← START HERE
│   ├── 01-GUIDE_DEMARRAGE.md
│   ├── 02-ARCHITECTURE.md
│   ├── 03-GUIDE_API.md
│   ├── 04-MODELES_DOMAINE.md
│   ├── 05-GUIDE_DEVELOPPEMENT.md
│   ├── 06-GUIDE_TESTS.md
│   ├── 07-GUIDE_DEPLOIEMENT.md
│   ├── 08-TROUBLESHOOTING_FAQ.md
│   ├── README.md
│   └── 09-VUE_ENSEMBLE.md            ← VOUS ÊTES ICI
│
├── AdvancedDevSample.Api/            ← Controllers, Endpoints
│   ├── Controllers/
│   ├── Filters/
│   ├── Middlewares/
│   └── Program.cs
│
├── AdvancedDevSample.Application/    ← Services, DTOs
│   ├── Services/
│   ├── Interfaces/
│   ├── DTOs/
│   └── DependencyInjection.cs
│
├── AdvancedDevSample.Domain/         ← Entités, Règles métier
│   ├── Entities/
│   ├── Interfaces/
│   ├── Events/
│   └── Enums/
│
├── AdvancedDevSample.Infrastructure/ ← Base de données
│   ├── DbContext/
│   ├── Repositories/
│   ├── Migrations/
│   └── DependencyInjection.cs
│
├── AdvancedDevSample.Test/           ← Tests
│   ├── Application/
│   ├── Domain/
│   ├── Components/
│   └── Integration/
│
└── README.md (racine)
```

---

## 🔍 Trouver ce que vous cherchez

| Je veux... | Lire... | Temps |
|-----------|---------|-------|
| Installer le projet | 01 | 15 min |
| Tester l'API | 03 | 20 min |
| Comprendre le design | 02 | 45 min |
| Ajouter une fonctionnalité | 05 + 06 | 3-4 h |
| Déployer | 07 | 2-3 h |
| Déboguer une erreur | 08 | 30 min - 2 h |

---

## ✅ Checklist d'intégration

- [ ] Télécharger et installer le projet
- [ ] Lancer l'API localement
- [ ] Accéder à Swagger (`/swagger`)
- [ ] Tester les endpoints principaux
- [ ] Lire la doc architecture
- [ ] Comprendre les 4 couches
- [ ] Lancer les tests (`dotnet test`)
- [ ] Voir passer les tests
- [ ] Modifier du code et relancer les tests
- [ ] Vous êtes prêt à contribuer! 🎉

---

## 🚀 Prochaines étapes

### Après avoir lu la documentation

1. **Ajouter une feature** : Suivez le guide complet [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md)
2. **Écrire des tests** : Pratiquez avec [06-GUIDE_TESTS.md](06-GUIDE_TESTS.md)
3. **Déployer** : Testez en staging avec [07-GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md)
4. **Contribuer** : Créez des Pull Requests sur GitHub

### Ressources d'apprentissage complémentaires

- **Microsoft Learn** : https://learn.microsoft.com/dotnet
- **ASP.NET Core** : https://learn.microsoft.com/aspnet/core
- **Entity Framework** : https://learn.microsoft.com/ef/core
- **C# Best Practices** : https://learn.microsoft.com/en-us/dotnet/csharp

---

## 🎓 Formation par domaine

### Backend / API REST
1. 02-ARCHITECTURE.md
2. 03-GUIDE_API.md
3. 05-GUIDE_DEVELOPPEMENT.md

### Gestion de données / BD
1. 04-MODELES_DOMAINE.md
2. 05-GUIDE_DEVELOPPEMENT.md (migrations)

### Qualité du code / Tests
1. 06-GUIDE_TESTS.md
2. 05-GUIDE_DEVELOPPEMENT.md (conventions)

### Déploiement / DevOps
1. 07-GUIDE_DEPLOIEMENT.md

---

## 📞 Support

Avez-vous des questions?

1. **Consultez d'abord** : [00-INDEX.md](00-INDEX.md) → Navigation
2. **Cherchez** : Ctrl+F sur le document pertinent
3. **FAQ** : [08-TROUBLESHOOTING_FAQ.md](08-TROUBLESHOOTING_FAQ.md)
4. **Google** : "site:learn.microsoft.com" + votre question
5. **Stack Overflow** : Tag [.net] ou [asp.net-core]

---

## 📈 Progression estimée

```
Jour 1 : Découverte (4 heures)
├─ Installation
├─ Exploration de l'API
└─ Compréhension de l'architecture

Jour 2 : Compréhension (4 heures)
├─ Modèles métier
├─ Conventions de code
└─ Workflow de développement

Jour 3 : Pratique (5 heures)
├─ Ajouter une feature
├─ Écrire des tests
└─ Déboguer des erreurs

Semaine 1 : Productif!
└─ Prêt à contribuer au projet
```

---

## 🎯 Objectifs d'apprentissage

À la fin, vous saurez :

✅ Comment fonctionne une API REST .NET  
✅ Architecture en couches et ses bénéfices  
✅ Écrire du code professionnel .NET  
✅ Tester correctement (unit + integration)  
✅ Déployer sur Azure ou Docker  
✅ Déboguer et résoudre les problèmes  
✅ Contribuer au projet professionnel  

---

## 🏆 Bravo!

Vous avez accès à une documentation professionnelle complète.

**Commencez par** : [00-INDEX.md](00-INDEX.md)

Bon apprentissage! 🚀
