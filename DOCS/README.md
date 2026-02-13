# 📚 Documentation - AdvancedDevSample

## Bienvenue!

Ce dossier contient la **documentation complète** du projet AdvancedDevSample - une API REST .NET professionnelle.

---

## 📖 Documents disponibles

| # | Document | Durée | Pour qui? |
|---|----------|-------|----------|
| **00** | [INDEX.md](00-INDEX.md) | 15 min | **COMMENCEZ ICI** - Navigation complète |
| **01** | [GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md) | 30 min | Vous découvrez le projet |
| **02** | [ARCHITECTURE.md](02-ARCHITECTURE.md) | 45 min | Vous voulez comprendre le design |
| **03** | [GUIDE_API.md](03-GUIDE_API.md) | 40 min | Vous testez l'API REST |
| **04** | [MODELES_DOMAINE.md](04-MODELES_DOMAINE.md) | 60 min | Vous voulez comprendre les métier |
| **05** | [GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) | 90 min | Vous développez une feature |
| **06** | [GUIDE_TESTS.md](06-GUIDE_TESTS.md) | 60 min | Vous écrivez des tests |
| **07** | [GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md) | 90 min | Vous déployez en production |
| **08** | [TROUBLESHOOTING_FAQ.md](08-TROUBLESHOOTING_FAQ.md) | 20 min | Vous avez un problème |

---

## 🚀 Démarrage rapide

### 1️⃣ Installation (5 minutes)

```bash
# Cloner
git clone https://github.com/FRANCK359/Developpement-Avanc-_gestion_produit.git
cd Developpement-Avanc-_gestion_produit

# Restaurer les dépendances
dotnet restore

# Appliquer les migrations
dotnet ef database update --project .\AdvancedDevSample.Infrastructure

# Lancer
dotnet run --project .\AdvancedDevSample.Api
```

L'API est accessible à : `https://localhost:7000`

### 2️⃣ Explorer l'API (5 minutes)

Ouvrez Swagger : `https://localhost:7000/swagger`

ou testez via PowerShell :

```powershell
# S'authentifier
$login = Invoke-RestMethod -Uri "https://localhost:7000/api/auth/login" `
  -Method Post -Body '{"username":"admin","password":"Admin123!"}' `
  -Headers @{"Content-Type"="application/json"} -SkipCertificateCheck

# Récupérer les produits
$products = Invoke-RestMethod -Uri "https://localhost:7000/api/products" `
  -Headers @{"Authorization"="Bearer $($login.token)"} -SkipCertificateCheck

$products | ConvertTo-Json
```

### 3️⃣ Comprendre l'architecture (10 minutes)

Lisez [ARCHITECTURE.md](02-ARCHITECTURE.md) pour voir comment le projet est organisé en 4 couches.

---

## 📚 Guides par usage

### Je suis nouveau sur le projet

1. Lisez [INDEX.md](00-INDEX.md) pour la navigation
2. Suivez [GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md)
3. Explorez [GUIDE_API.md](03-GUIDE_API.md)
4. Comprenez [ARCHITECTURE.md](02-ARCHITECTURE.md)

**Temps total** : 2-3 heures

### Je dois ajouter une fonctionnalité

1. Comprenez [MODELES_DOMAINE.md](04-MODELES_DOMAINE.md)
2. Suivez [GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md)
3. Écrivez les tests [GUIDE_TESTS.md](06-GUIDE_TESTS.md)

**Temps total** : 3-4 heures

### Je dois déployer l'application

1. Préparez avec [GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md)
2. Résolvez les problèmes via [TROUBLESHOOTING_FAQ.md](08-TROUBLESHOOTING_FAQ.md)

**Temps total** : 2-3 heures

### J'ai un problème

Consultez [TROUBLESHOOTING_FAQ.md](08-TROUBLESHOOTING_FAQ.md) en priorité!

**Temps pour trouver la solution** : 10-30 minutes

---

## 🎯 Points clés du projet

### Architecture en couches

```
API Layer (Controllers)
    ↓
Application Layer (Services)
    ↓
Domain Layer (Entities)
    ↓
Infrastructure Layer (Database)
```

### Principales entités

- **Customer** : Client
- **Product** : Produit
- **Order** : Commande
- **OrderItem** : Élément de commande
- **Supplier** : Fournisseur
- **User** : Utilisateur

### Endpoints principaux

```
GET    /api/products              → Lister les produits
POST   /api/products              → Créer un produit
GET    /api/customers/{id}        → Récupérer un client
POST   /api/orders                → Créer une commande
POST   /api/auth/login            → S'authentifier
```

Voir [GUIDE_API.md](03-GUIDE_API.md) pour tous les endpoints.

---

## 🔧 Commandes essentielles

```powershell
# Développement
dotnet restore                                              # Restaurer les dépendances
dotnet build                                                # Compiler
dotnet run --project .\AdvancedDevSample.Api               # Lancer l'API
dotnet watch run --project .\AdvancedDevSample.Api         # Lancer avec rechargement auto

# Tests
dotnet test                                                 # Lancer tous les tests
dotnet test --filter "ProductServiceTests"                 # Lancer des tests spécifiques

# Base de données
dotnet ef migrations add MigrationName                      # Créer une migration
dotnet ef database update                                   # Appliquer les migrations
dotnet ef database drop                                     # Supprimer la BD

# Déploiement
dotnet publish -c Release -o ./publish                     # Publier l'application
docker build -t myapp:latest .                            # Créer une image Docker
```

Voir [GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) pour plus de commandes.

---

## 📋 Structure de la documentation

Chaque document suit cette structure :

```markdown
# Titre

## 📌 Table des matières

## 👁️ Vue d'ensemble

## 📚 Sections principales
  - Sous-section 1
  - Sous-section 2
  
## 🔧 Exemples pratiques

## ⚠️ Bonnes pratiques / Pièges courants

## 📞 Pour plus d'informations
```

---

## 🎓 Parcours d'apprentissage

### Jour 1 : Découverte (3-4 heures)
- [ ] Installer et démarrer ([GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md))
- [ ] Tester l'API ([GUIDE_API.md](03-GUIDE_API.md))
- [ ] Comprendre l'architecture ([ARCHITECTURE.md](02-ARCHITECTURE.md))

### Jour 2 : Compréhension (4-5 heures)
- [ ] Étudier les modèles métier ([MODELES_DOMAINE.md](04-MODELES_DOMAINE.md))
- [ ] Lire les conventions de code ([GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md))

### Jour 3 : Pratique (5-6 heures)
- [ ] Ajouter une nouvelle fonctionnalité ([GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md))
- [ ] Écrire des tests ([GUIDE_TESTS.md](06-GUIDE_TESTS.md))

### Jour 4 : Déploiement (4-5 heures)
- [ ] Comprendre le déploiement ([GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md))
- [ ] Déployer sur Azure ou Docker

**Total** : ~20 heures pour une maîtrise de base

---

## ❓ FAQ Rapide

**Q: Par où commencer?**
→ Lisez d'abord [00-INDEX.md](00-INDEX.md), puis [01-GUIDE_DEMARRAGE.md](01-GUIDE_DEMARRAGE.md)

**Q: Comment l'API fonctionne?**
→ Consultez [03-GUIDE_API.md](03-GUIDE_API.md)

**Q: Comment ajouter une entité?**
→ Suivez [05-GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) - "Ajouter une nouvelle fonctionnalité"

**Q: J'ai une erreur...**
→ Consultez [08-TROUBLESHOOTING_FAQ.md](08-TROUBLESHOOTING_FAQ.md)

**Q: Où déployer?**
→ Consultez [07-GUIDE_DEPLOIEMENT.md](07-GUIDE_DEPLOIEMENT.md)

---

## 📊 Vue d'ensemble de la documentation

```
00-INDEX.md
├── Navigation et orientation
└── Pour tous

01-GUIDE_DEMARRAGE.md
├── Installation
├── Configuration
├── Premier lancement
└── Pour : Nouveaux développeurs

02-ARCHITECTURE.md
├── Architecture en couches
├── Diagrammes
├── Flux de données
└── Pour : Comprendre le design

03-GUIDE_API.md
├── Endpoints
├── Authentification JWT
├── Exemples
└── Pour : Utiliser/tester l'API

04-MODELES_DOMAINE.md
├── Entités métier
├── Règles de domaine
├── Énumérations
└── Pour : Comprendre le métier

05-GUIDE_DEVELOPPEMENT.md
├── Conventions
├── Ajouter une feature
├── Gestion DB
└── Pour : Développer

06-GUIDE_TESTS.md
├── Unittaires
├── Intégration
├── Best practices
└── Pour : Tester

07-GUIDE_DEPLOIEMENT.md
├── Local/Production
├── Azure/Docker
├── CI/CD
└── Pour : Déployer

08-TROUBLESHOOTING_FAQ.md
├── Problèmes courants
├── Solutions
├── FAQ
└── Pour : Déboguer
```

---

## 🤝 Contribution à la documentation

Si vous trouvez une erreur ou une imprécision :

1. Signalez-la via une issue GitHub
2. Ou créez une Pull Request avec la correction
3. Merci de maintenir la documentation à jour!

---

## 📞 Besoin d'aide?

| Ressource | Utilité |
|-----------|---------|
| [INDEX.md](00-INDEX.md) | Navigation complète |
| [TROUBLESHOOTING_FAQ.md](08-TROUBLESHOOTING_FAQ.md) | Problèmes courants |
| [GUIDE_DEVELOPPEMENT.md](05-GUIDE_DEVELOPPEMENT.md) | Questions techniques |
| [Microsoft Docs](https://learn.microsoft.com/dotnet) | Documentation officielle .NET |

---

## 📌 Licence

Cette documentation est fournie avec le projet AdvancedDevSample et peut être utilisée librement.

---

**Bonne chance dans votre apprentissage!** 🚀

Commencez par [00-INDEX.md](00-INDEX.md) →
