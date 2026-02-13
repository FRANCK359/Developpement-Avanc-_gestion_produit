# Guide de Déploiement - AdvancedDevSample

## 📌 Table des matières
- [Vue d'ensemble](#vue-densemble)
- [Déploiement local](#déploiement-local)
- [Déploiement en production](#déploiement-en-production)
- [Azure App Service](#azure-app-service)
- [Docker & Conteneurisation](#docker--conteneurisation)
- [CI/CD avec GitHub Actions](#cicd-avec-github-actions)
- [Monitoring et logging](#monitoring-et-logging)
- [Troubleshooting](#troubleshooting)

---

## Vue d'ensemble

### Environnements

| Environnement | Durée de vie | Configuration | Accès |
|--------------|-------------|--------------|-------|
| **Development** | Développement | Base locale, logs détaillés | Localhost |
| **Staging** | Tests pré-prod | Config produit, logs réduits | URL de test |
| **Production** | Utilisateurs finaux | Config sécurisée, logs critiques | URL publique |

### Architecture de déploiement

```
┌─────────────────────────────────────────────────────┐
│          GitHub Repository (Source)                 │
├─────────────────────────────────────────────────────┤
│                                                     │
│ Commit → Push → GitHub Actions (CI/CD)             │
│         ↓                                           │
│  ├─ Build & Test                                   │
│  ├─ Create Docker Image                            │
│  └─ Push to Registry                               │
│         ↓                                           │
│ ┌──────────────────┐  ┌──────────────────┐         │
│ │  Azure Container │  │  Azure App       │         │
│ │  Registry        │  │  Service         │         │
│ │ (Registry)       │→→→│ (Hosting)        │         │
│ └──────────────────┘  └──────────────────┘         │
│         ↓                       ↓                   │
│   Docker Image          Deployed Application       │
│                                                     │
└─────────────────────────────────────────────────────┘
```

---

## Déploiement local

### Build et lancement

#### Option 1 : Sans Docker

```powershell
# Nettoyer les builds précédentes
dotnet clean

# Restaurer les dépendances
dotnet restore

# Build en Release
dotnet build --configuration Release

# Lancer l'application
cd AdvancedDevSample.Api
dotnet run --configuration Release
```

#### Option 2 : Avec publication

```powershell
# Publier en mode Release
dotnet publish -c Release -o ./publish

# Naviguer vers le dossier publié
cd publish

# Lancer l'application
./AdvancedDevSample.Api.exe
```

#### Vérification

```powershell
# Tester l'endpoint
$response = Invoke-WebRequest -Uri "https://localhost:7000/api/products" `
  -SkipCertificateCheck

$response.StatusCode  # Doit afficher 200
```

---

## Déploiement en production

### Considérations de sécurité

#### 1. Secrets et configuration

**NE JAMAIS** mettre en dur :
- Chaînes de connexion DB
- Clés JWT
- Tokens d'API
- Identifiants utilisateurs

**À utiliser** :
- Environment variables
- Azure Key Vault
- Docker Secrets
- appsettings.{Environment}.json (git-ignored)

#### 2. HTTPS obligatoire

```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://0.0.0.0:443",
        "Certificate": {
          "Path": "/etc/ssl/certs/cert.pem",
          "KeyPath": "/etc/ssl/certs/key.pem"
        }
      }
    }
  }
}
```

#### 3. CORS configuré

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy
            .WithOrigins("https://app.example.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

app.UseCors("Production");
```

---

## Azure App Service

### Créer et déployer sur Azure

#### Prérequis

```powershell
# Installer Azure CLI
# https://learn.microsoft.com/cli/azure/install-azure-cli

az --version

# Se connecter à Azure
az login

# Lister les souscriptions
az account list
az account set --subscription "ID-SUBSCRIPTION"
```

#### Étape 1 : Créer un Resource Group

```powershell
$resourceGroup = "advanceddevsample-rg"
$location = "eastus"

az group create --name $resourceGroup --location $location
```

#### Étape 2 : Créer un App Service Plan

```powershell
$appServicePlan = "advanceddevsample-plan"
$sku = "B1"  # B1 = Basique (développement), P1V2 = Production

az appservice plan create `
  --name $appServicePlan `
  --resource-group $resourceGroup `
  --sku $sku `
  --is-linux
```

#### Étape 3 : Créer l'App Service

```powershell
$appName = "advanceddevsample-api"
$runtime = "DOTNETCORE|10.0"

az webapp create `
  --resource-group $resourceGroup `
  --plan $appServicePlan `
  --name $appName `
  --runtime $runtime
```

#### Étape 4 : Configurer les variables d'environnement

```powershell
az webapp config appsettings set `
  --resource-group $resourceGroup `
  --name $appName `
  --settings `
    ASPNETCORE_ENVIRONMENT=Production `
    ConnectionStrings__DefaultConnection="Server=tcp:..." `
    JwtSettings__SecretKey="..." `
    JwtSettings__Issuer="AdvancedDevSample" `
    JwtSettings__Audience="AdvancedDevSampleClients"
```

#### Étape 5 : Déployer le code

```powershell
# Option A : Deployment via Git
az webapp deployment source config-local-git `
  --resource-group $resourceGroup `
  --name $appName

# Ajouter le remote Git
git remote add azure $(az webapp deployment source config-local-git `
  --resource-group $resourceGroup `
  --name $appName --query url --output tsv)

# Pousser vers Azure
git push azure main

# Option B : Publication directe
cd AdvancedDevSample.Api
dotnet publish -c Release -o ./publish

# Compresser
7z a -tzip ../publish.zip ./publish/*

# Uploader
az webapp deployment source config-zip `
  --resource-group $resourceGroup `
  --name $appName `
  --src-path ../publish.zip
```

#### Étape 6 : Vérifier le déploiement

```powershell
# Consulter les logs
az webapp log tail --resource-group $resourceGroup --name $appName

# Obtenir l'URL de l'app
az webapp show --resource-group $resourceGroup --name $appName `
  --query "hostNames[0]"

# Tester
$appUrl = "https://advanceddevsample-api.azurewebsites.net"
Invoke-WebRequest -Uri "$appUrl/api/products" -SkipCertificateCheck
```

---

## Docker & Conteneurisation

### Créer un Dockerfile

`Dockerfile` à la racine du projet :

```dockerfile
# Stage 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copier les fichiers de projet
COPY ["AdvancedDevSample.Api/AdvancedDevSample.Api.csproj", "AdvancedDevSample.Api/"]
COPY ["AdvancedDevSample.Application/AdvancedDevSample.Application.csproj", "AdvancedDevSample.Application/"]
COPY ["AdvancedDevSample.Domain/AdvancedDevSample.Domain.csproj", "AdvancedDevSample.Domain/"]
COPY ["AdvancedDevSample.Infrastructure/AdvancedDevSample.Infrastructure.csproj", "AdvancedDevSample.Infrastructure/"]

# Restaurer les dépendances
RUN dotnet restore "AdvancedDevSample.Api/AdvancedDevSample.Api.csproj"

# Copier tout le code
COPY . .

# Build
RUN dotnet build "AdvancedDevSample.Api/AdvancedDevSample.Api.csproj" -c Release -o /app/build

# Publier
RUN dotnet publish "AdvancedDevSample.Api/AdvancedDevSample.Api.csproj" -c Release -o /app/publish

# Stage 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copier les fichiers publiés
COPY --from=build /app/publish .

# Exposer le port
EXPOSE 80
EXPOSE 443

# Définir l'entrypoint
ENTRYPOINT ["dotnet", "AdvancedDevSample.Api.dll"]
```

### Fichier .dockerignore

```
.git
.gitignore
README.md
DOCS
.github
.vs
.vscode
bin
obj
*.user
*.suo
publish
```

### Créer l'image Docker

```powershell
# Build l'image
docker build -t advanceddevsample-api:latest .

# Vérifier l'image
docker images | findstr advanceddevsample

# Lancer le conteneur localement
docker run -p 5000:80 -p 7000:443 `
  -e ASPNETCORE_ENVIRONMENT=Production `
  -e ConnectionStrings__DefaultConnection="..." `
  advanceddevsample-api:latest

# Tester
curl http://localhost:5000/api/products
```

### Docker Compose

`docker-compose.yml` :

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:80"
      - "7000:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=AdvancedDevSample;User Id=sa;Password=YourPassword123;
    depends_on:
      - db
    networks:
      - advanceddev-network

  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123
    ports:
      - "1433:1433"
    networks:
      - advanceddev-network
    volumes:
      - sql_data:/var/opt/mssql

networks:
  advanceddev-network:
    driver: bridge

volumes:
  sql_data:
```

Lancer :
```powershell
docker-compose up -d
docker-compose logs -f api
docker-compose down
```

---

## CI/CD avec GitHub Actions

### Créer un workflow

`.github/workflows/deploy.yml` :

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

env:
  REGISTRY: myregistry.azurecr.io
  IMAGE_NAME: advanceddevsample-api

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Run tests
      run: dotnet test --configuration Release --no-build --verbosity normal
    
    - name: Publish
      run: dotnet publish -c Release -o ${{env.GITHUB_WORKSPACE}}/publish

  deploy:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main' && github.event_name == 'push'
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Deploy to Azure App Service
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'advanceddevsample-api'
        slot-name: 'production'
        package: ${{ env.GITHUB_WORKSPACE }}/publish
```

### Configurer les secrets

Sur GitHub (Settings → Secrets → Actions) :

```
AZURE_CREDENTIALS = 
{
  "clientId": "...",
  "clientSecret": "...",
  "subscriptionId": "...",
  "tenantId": "..."
}
```

Générer les credentials :

```bash
az ad sp create-for-rbac --name "github-action" --role contributor \
  --scopes /subscriptions/{subscription-id} \
  --output json
```

---

## Monitoring et logging

### Azure Application Insights

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();

// OU

builder.Logging.AddApplicationInsights();
```

Configuration `appsettings.json` :

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "YOUR_KEY_HERE"
  }
}
```

### Logging structuré

```csharp
// Utiliser ILogger
_logger.LogInformation("Product created: {productId} by {userId}", 
    productId, userId);

_logger.LogError(ex, "Error processing order {orderId}", orderId);

_logger.LogWarning("Order total exceeds limit: {total}", totalAmount);
```

### Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AdvancedDevSampleDbContext>();

app.MapHealthChecks("/health");
```

Tester :
```bash
curl https://api.example.com/health
```

---

## Troubleshooting

### ❌ Erreur : "502 Bad Gateway"

```
Causes possibles:
1. L'app ne démarre pas
2. Ports mal configurés
3. Timeout au démarrage
```

Solutions :
```powershell
# Voir les logs
az webapp log tail --resource-group mygroup --name myapp

# Redémarrer l'app
az webapp restart --resource-group mygroup --name myapp

# Vérifier les settings
az webapp config appsettings list --resource-group mygroup --name myapp
```

### ❌ Erreur : "Connection to database failed"

```
Vérifier:
1. Connection string
2. Database exist
3. Firewall rules
4. Network connectivity
```

```powershell
# Tester la connexion SQL
sqlcmd -S server.database.windows.net -U admin -P password
```

### ❌ Erreur : "403 Forbidden"

```
Vérifier:
1. CORS configuration
2. Authentication token
3. User permissions
4. IP restrictions
```

---

## Checklist de production

✅ **Avant déploiement** :
- [ ] Tous les tests passent
- [ ] Pas de secrets en dur
- [ ] HTTPS activé
- [ ] Logging configuré
- [ ] CORS restreint
- [ ] Base de données sécurisée
- [ ] Backup strategy en place
- [ ] Documentation à jour

✅ **Après déploiement** :
- [ ] Tester les endpoints principaux
- [ ] Vérifier les logs
- [ ] Monitorer la performance
- [ ] Tester le rollback
- [ ] Documenter les URLs de production

---

## Résumé

Le déploiement de AdvancedDevSample peut se faire via :

🟢 **Azure App Service** : Pour les applications web simples  
🟢 **Docker + Container Registry** : Pour la portabilité  
🟢 **Kubernetes (AKS)** : Pour le scale et l'orchestration  
🟢 **GitHub Actions** : Pour l'automatisation CI/CD  

Chaque environnement requiert une configuration appropriée et des mesures de sécurité.

Pour plus de détails, consultez la documentation Azure.
