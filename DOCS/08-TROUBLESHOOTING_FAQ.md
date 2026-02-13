# Troubleshooting & FAQ - AdvancedDevSample

## 📌 Table des matières
- [Problèmes courants](#problèmes-courants)
- [Erreurs de démarrage](#erreurs-de-démarrage)
- [Erreurs de base de données](#erreurs-de-base-de-données)
- [Erreurs d'API](#erreurs-dapi)
- [Erreurs de déploiement](#erreurs-de-déploiement)
- [Questions fréquemment posées](#questions-fréquemment-posées)
- [Aide supplémentaire](#aide-supplémentaire)

---

## Problèmes courants

### ❌ Port déjà utilisé

**Symptôme** : `Address already in use` ou `Port 7000 is already in use`

**Cause** : Un autre processus utilise le port 7000 ou 5000

**Solution** :

```powershell
# Trouver le processus utilisant le port
netstat -ano | findstr :7000

# Terminer le processus
taskkill /PID <PID> /F

# Ou lancer sur un port différent
dotnet run --urls "https://localhost:7001;http://localhost:5001"
```

**Alternative** : Modifier `launchSettings.json`

```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7001;http://localhost:5001"
    }
  }
}
```

---

### ❌ .NET SDK non trouvé

**Symptôme** : `No matching version found for the SDK version 10.0.0`

**Cause** : .NET 10.0 SDK n'est pas installé

**Solution** :

```powershell
# Vérifier les versions installées
dotnet --list-sdks

# Installer .NET 10.0
# Télécharger depuis https://dot.net

# Vérifier après installation
dotnet --version
```

**Note** : Vous pouvez changer la version requise dans `global.json`

```json
{
  "sdk": {
    "version": "9.0.0"  // Utiliser une version disponible
  }
}
```

---

### ❌ Dépendances NuGet non résolues

**Symptôme** : `The project either doesn't target a framework...` ou erreurs de compilation

**Cause** : NuGet packages non restaurés ou incompatibilité de version

**Solution** :

```powershell
# Nettoyer le cache NuGet
dotnet nuget locals all --clear

# Restaurer les dépendances
dotnet restore

# Ou forcer une restauration complète
dotnet restore --force
```

---

## Erreurs de démarrage

### ❌ L'application se lance mais s'arrête immédiatement

**Symptôme** : La fenêtre se ferme sans message d'erreur

**Cause** : Exception non interceptée au démarrage

**Solution** :

```powershell
# Lancer en mode debug pour voir les erreurs
dotnet run --configuration Debug

# Vérifier les logs de démarrage
# Chercher Exception ou Error dans la sortie

# Vérifier appsettings.json
# Assurez-vous que la configuration est valide JSON
```

**Causes communes** :
- Connection string invalide
- Fichier de configuration manquant
- Permission d'accès insuffisante

---

### ❌ "Configuration value 'ConnectionStrings:DefaultConnection' not found"

**Symptôme** : Erreur au démarrage, pas de connection string

**Cause** : `appsettings.json` n'a pas de section `ConnectionStrings`

**Solution** :

Vérifier `appsettings.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AdvancedDevSample;Trusted_Connection=true;"
  }
}
```

---

### ❌ "Unable to load the service index for source"

**Symptôme** : Impossible de restaurer les packages NuGet

**Cause** : Problème de connexion internet ou proxy

**Solution** :

```powershell
# Vérifier la connexion internet
Invoke-WebRequest https://api.nuget.org

# Configurer le proxy si nécessaire
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# Ou utiliser le cache NuGet local
dotnet restore --no-cache
```

---

## Erreurs de base de données

### ❌ "The database 'AdvancedDevSample' does not exist"

**Symptôme** : Erreur de connection à la base de données

**Cause** : La base de données n'a pas été créée

**Solution** :

```powershell
# Appliquer les migrations pour créer la base
dotnet ef database update --project ..\AdvancedDevSample.Infrastructure

# Si ça ne marche pas, créer manuellement
# Pour LocalDB:
sqlcmd -S (localdb)\mssqllocaldb -i create_db.sql
```

Créer `create_db.sql` :
```sql
CREATE DATABASE AdvancedDevSample;
GO
```

---

### ❌ "A network-related or instance-specific error occurred"

**Symptôme** : Impossible de se connecter à la base de données

**Cause** : Serveur SQL n'est pas accessible

**Solution** :

```powershell
# Vérifier que LocalDB est running
sqllocaldb info

# Démarrer LocalDB
sqllocaldb start mssqllocaldb

# Tester la connexion
sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT @@VERSION"

# Ou pour SQL Server distant, vérifier:
# 1. Le serveur est accessible
# 2. Les credentials sont corrects
# 3. Le pare-feu autorise la connexion (port 1433)
```

---

### ❌ Migration échoue avec "There is already an object named..."

**Symptôme** : Migration fail, table existe déjà

**Cause** : Conflit entre les migrations

**Solution** :

```powershell
# Voir l'historique des migrations
dotnet ef migrations list --project ..\AdvancedDevSample.Infrastructure

# Supprimer la dernière migration (si pas appliquée)
dotnet ef migrations remove --project ..\AdvancedDevSample.Infrastructure

# Revenir à une migration précédente
dotnet ef database update 20240101000000_PreviousMigration --project ..\AdvancedDevSample.Infrastructure

# Supprimer et recréer la base (développement uniquement)
dotnet ef database drop --force --project ..\AdvancedDevSample.Infrastructure
dotnet ef database update --project ..\AdvancedDevSample.Infrastructure
```

---

### ❌ "Login failed for user 'sa'"

**Symptôme** : Erreur d'authentification SQL Server

**Cause** : Mot de passe incorrect ou utilisateur inexistant

**Solution** :

```powershell
# Vérifier la connection string dans appsettings.json
# Pour LocalDB avec authentification Windows:
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AdvancedDevSample;Trusted_Connection=true;"

# Pour SQL Server avec credentials:
"DefaultConnection": "Server=localhost;Database=AdvancedDevSample;User Id=sa;Password=YourPassword123;"
```

---

## Erreurs d'API

### ❌ "401 Unauthorized"

**Symptôme** : Endpoint refuse la requête

**Cause** : Token JWT manquant ou invalide

**Solution** :

```powershell
# 1. Obtenir un token
$loginResponse = Invoke-RestMethod -Uri "https://localhost:7000/api/auth/login" `
  -Method Post `
  -Headers @{"Content-Type" = "application/json"} `
  -Body '{"username":"admin","password":"Admin123!"}' `
  -SkipCertificateCheck

$token = $loginResponse.token

# 2. Inclure le token dans les requêtes suivantes
$headers = @{"Authorization" = "Bearer $token"}
Invoke-RestMethod -Uri "https://localhost:7000/api/products" `
  -Headers $headers `
  -SkipCertificateCheck
```

**À vérifier** :
- JWT Settings dans `appsettings.json`
- Credentials valides pour login
- Expiration du token (60 min par défaut)

---

### ❌ "400 Bad Request"

**Symptôme** : Requête rejetée, données invalides

**Cause** : DTO incomplet ou format invalide

**Solution** :

```powershell
# Vérifier le format JSON
$body = @{
    name = "Product Name"
    price = 99.99
    supplierId = "550e8400-e29b-41d4-a716-446655440000"
} | ConvertTo-Json

# Vérifier que tous les champs requis sont présents
Invoke-RestMethod -Uri "https://localhost:7000/api/products" `
  -Method Post `
  -Headers @{"Content-Type" = "application/json"; "Authorization" = "Bearer $token"} `
  -Body $body `
  -SkipCertificateCheck
```

**À vérifier** :
- Format JSON valide
- Tous les champs obligatoires présents
- Types de données corrects
- Enums avec les bonnes valeurs

---

### ❌ "404 Not Found"

**Symptôme** : Ressource introuvable

**Cause** : ID invalide ou ressource supprimée

**Solution** :

```powershell
# Vérifier que la ressource existe
Invoke-RestMethod -Uri "https://localhost:7000/api/products" `
  -Headers @{"Authorization" = "Bearer $token"} `
  -SkipCertificateCheck

# Chercher l'ID correct
$products = $response.items
$productId = $products[0].id

# Utiliser le bon ID
Invoke-RestMethod -Uri "https://localhost:7000/api/products/$productId" `
  -Headers @{"Authorization" = "Bearer $token"} `
  -SkipCertificateCheck
```

---

### ❌ "500 Internal Server Error"

**Symptôme** : Erreur serveur non documentée

**Cause** : Exception non gérée

**Solution** :

```powershell
# 1. Vérifier les logs de la console
# Chercher "Exception" ou "Error"

# 2. Activer les logs détaillés
# Dans appsettings.Development.json:
"Logging": {
  "LogLevel": {
    "Default": "Debug",
    "Microsoft": "Debug"
  }
}

# 3. Relancer l'application
dotnet run --configuration Debug

# 4. Reproduire l'erreur et analyser les logs
```

**Erreurs courantes** :
- Entité non trouvée en base
- Violation de règles métier
- Conflit de données unique
- Erreur lors de la sauvegarde

---

### ❌ "CORS error: Access-Control-Allow-Origin header missing"

**Symptôme** : La requête est bloquée par le navigateur

**Cause** : CORS non configuré ou origine non autorisée

**Solution** :

Vérifier `Program.cs` :

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });

    options.AddPolicy("Production", policy =>
    {
        policy
            .WithOrigins("https://app.example.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

app.UseCors("Development");  // Ou "Production"
```

---

## Erreurs de déploiement

### ❌ "502 Bad Gateway"

**Symptôme** : L'application déployée ne répond pas

**Cause** : L'app ne démarre pas ou les ports ne correspondent pas

**Solution** :

```powershell
# Sur Azure App Service
az webapp log tail --resource-group mygroup --name myapp

# Vérifier les settings
az webapp config appsettings list --resource-group mygroup --name myapp

# Redémarrer l'app
az webapp restart --resource-group mygroup --name myapp

# Vérifier ASPNETCORE_ENVIRONMENT
# Doit être "Production" (sans typo)
```

---

### ❌ "Docker build fails"

**Symptôme** : Erreur lors de la construction de l'image Docker

**Cause** : Dépendances manquantes, version incompatible

**Solution** :

```bash
# Build avec verbosité
docker build --no-cache -t myapp:latest . 2>&1 | tee build.log

# Vérifier le Dockerfile
# S'assurer que tous les fichiers .csproj sont copiés
# Vérifier la version de base image (.NET version)

# Nettoyer et relancer
docker system prune -a
docker build -t myapp:latest .
```

---

### ❌ "Health check failed"

**Symptôme** : L'application démarre mais l'health check échoue

**Cause** : Endpoint `/health` indisponible ou erreur de configuration

**Solution** :

```csharp
// Ajouter à Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AdvancedDevSampleDbContext>();

app.MapHealthChecks("/health");

// Tester localement
curl https://localhost:7000/health
```

---

## Questions fréquemment posées

### Q: Comment changer la base de données par défaut?

**R:** Modifier la connection string dans `appsettings.json` :

```json
// LocalDB
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AdvancedDevSample;Trusted_Connection=true;"

// SQL Server
"DefaultConnection": "Server=localhost,1433;Database=AdvancedDevSample;User Id=sa;Password=Pwd123!;"

// Azure SQL
"DefaultConnection": "Server=tcp:myserver.database.windows.net,1433;Initial Catalog=AdvancedDevSample;Persist Security Info=False;User ID=admin;Password=Pwd123!;Encrypt=True;Connection Timeout=30;"

// SQLite (léger, pour dev)
"DefaultConnection": "Data Source=advanceddevsample.db"
```

Puis mettre à jour le DbContext :

```csharp
// Pour SQLite
options.UseSqlite(connection)

// Pour SQL Server (défaut)
options.UseSqlServer(connection)
```

---

### Q: Comment réinitialiser la base de données?

**R:** Pour le développement uniquement :

```powershell
# Supprimer et recréer
dotnet ef database drop --force --project ..\AdvancedDevSample.Infrastructure
dotnet ef database update --project ..\AdvancedDevSample.Infrastructure

# Ou supprimer les migrations et recommencer
dotnet ef migrations remove --project ..\AdvancedDevSample.Infrastructure
dotnet ef migrations add InitialCreate --project ..\AdvancedDevSample.Infrastructure
dotnet ef database update --project ..\AdvancedDevSample.Infrastructure
```

---

### Q: Comment ajouter un nouvel utilisateur Admin?

**R:** Créer une migration avec les données de seed :

```csharp
// Dans OnModelCreating du DbContext
modelBuilder.Entity<User>().HasData(
    new User 
    { 
        Id = Guid.NewGuid(),
        Username = "admin",
        Email = "admin@example.com",
        PasswordHash = BCrypt.HashPassword("Admin123!"),
        IsActive = true,
        Role = UserRole.Admin,
        CreatedAt = DateTime.UtcNow
    }
);
```

---

### Q: Comment désactiver HTTPS en développement?

**R:** Dans `launchSettings.json` :

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5000"
    }
  }
}
```

Ou via la ligne de commande :

```powershell
set ASPNETCORE_ENVIRONMENT=Development
dotnet run --no-https
```

---

### Q: Comment obtenir les logs détaillés?

**R:** Modifier `appsettings.json` :

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

Ou via une variable d'environnement :

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
```

---

### Q: Comment tester l'API sans Swagger?

**R:** Utiliser PowerShell, curl ou Postman :

```powershell
# PowerShell
$response = Invoke-RestMethod -Uri "https://localhost:7000/api/products" `
  -Headers @{"Authorization" = "Bearer $token"} `
  -SkipCertificateCheck
$response | ConvertTo-Json | Write-Host

# curl
curl -k -H "Authorization: Bearer $token" https://localhost:7000/api/products

# Postman
# Créer une requête GET
# URL: https://localhost:7000/api/products
# Header: Authorization: Bearer {token}
```

---

### Q: Comment configurer le timeout des requêtes?

**R:** Dans `Program.cs` :

```csharp
builder.Services.Configure<HttpClientFactoryOptions>(options =>
{
    options.HttpClientActions.Add(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });
});

// Ou configurer Kestrel
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});
```

---

### Q: Comment activer le HTTPS avec un certificat auto-signé?

**R:** 

```powershell
# Générer un certificat (Windows)
$cert = New-SelfSignedCertificate -CertStoreLocation cert:\CurrentUser\My `
  -DnsName localhost -FriendlyName "Dev Certificate"

# Exporter en fichier .pfx
$password = ConvertTo-SecureString -String "MyPassword123!" -AsPlainText -Force
Export-PfxCertificate -Cert $cert -FilePath "localhost.pfx" -Password $password

# Importer dans le trusted store
Import-PfxCertificate -FilePath "localhost.pfx" -CertStoreLocation Cert:\CurrentUser\Root -Password $password
```

Puis configurer dans `appsettings.Development.json` :

```json
{
  "Kestrel": {
    "Certificates": {
      "Default": {
        "Path": "localhost.pfx",
        "Password": "MyPassword123!"
      }
    }
  }
}
```

---

### Q: Comment déboguer une requête lente?

**R:** Ajouter un middleware de performance :

```csharp
// Program.cs
app.Use(async (context, next) =>
{
    var watch = System.Diagnostics.Stopwatch.StartNew();
    await next();
    watch.Stop();
    
    if (watch.ElapsedMilliseconds > 1000)
    {
        _logger.LogWarning("Slow request: {path} took {ms}ms", 
            context.Request.Path, watch.ElapsedMilliseconds);
    }
});

// Ou utiliser LINQ to SQL profiler
// SELECT * FROM sys.dm_exec_requests
```

---

## Aide supplémentaire

### Avant de demander de l'aide

✅ **Vérifiez** :
- [ ] Vous avez la dernière version du code (`git pull`)
- [ ] Vous avez exécuté `dotnet restore`
- [ ] La base de données a les migrations à jour
- [ ] Aucune erreur dans la console
- [ ] Les logs contiennent plus d'informations

### Pour signaler un bug

Fournissez :
1. **Étapes pour reproduire** : Comment l'erreur survient?
2. **Résultat attendu** : Qu'est-ce qui devrait se passer?
3. **Résultat actuel** : Qu'est-ce qui se passe réellement?
4. **Logs d'erreur** : Copiez les messages d'erreur complets
5. **Version** : .NET version, OS, etc.

### Ressources utiles

- [Documentation Microsoft .NET](https://learn.microsoft.com/dotnet)
- [Stack Overflow - Tag: .net](https://stackoverflow.com/questions/tagged/.net)
- [GitHub Issues du projet](https://github.com/FRANCK359/Developpement-Avanc-_gestion_produit/issues)
- [Entity Framework Core Docs](https://learn.microsoft.com/ef/core)

---

## Résumé

Ce guide couvre 90% des problèmes rencontrés. Si vous avez :

❌ **Un problème non listé** : Consultez les logs détaillés et recherchez sur Google  
❌ **Une erreur complexe** : Ouvrez une issue sur GitHub avec les logs complets  
✅ **Une solution** : Contribuez à ce guide!

Bon débogage! 🐛🔧
