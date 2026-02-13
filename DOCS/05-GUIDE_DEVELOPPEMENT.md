# Guide de Développement - AdvancedDevSample

## 📌 Table des matières
- [Environnement de développement](#environnement-de-développement)
- [Conventions de code](#conventions-de-code)
- [Structure de fichiers](#structure-de-fichiers)
- [Ajouter une nouvelle fonctionnalité](#ajouter-une-nouvelle-fonctionnalité)
- [Gérer la base de données](#gérer-la-base-de-données)
- [Workflow Git](#workflow-git)
- [Debugging](#debugging)
- [Performance](#performance)

---

## Environnement de développement

### Configuration Visual Studio

#### 1. Extensions recommandées

```
Tools → Extensions and Updates → Search Online

- Visual Studio IntelliCode
- GitHub Copilot (optionnel)
- Prettier - code formatter
- EditorConfig Language Support
- REST Client
```

#### 2. Paramètres Visual Studio

**Tools → Options → C# → Code Style**

```
- Indentation: Tabs (size 4)
- Line length: 100-120 chars
- Naming rules: PascalCase (public), _camelCase (private)
```

**Tools → Options → Source Control → Git**

```
- Default repository location: C:\Dev
- Auto-load solution when opening Git repository: ✓
```

### Configuration du projet

#### .editorconfig

Fichier de configuration d'éditeur pour la cohérence du code :

```ini
root = true

[*.cs]
# Indentation
indent_style = space
indent_size = 4
tab_width = 4

# Conventions de nommage
dotnet_naming_rule.interfaces_should_be_begins_with_i.severity = suggestion
dotnet_naming_convention.begins_with_i.style = begins_with_i_style
dotnet_naming_style.begins_with_i_style.required_prefix = I

# Style de code
csharp_indent_case_contents = true
csharp_indent_switch_labels = true
csharp_space_after_cast = false
```

#### launch.json

Configuration de lancement (VS Code) :

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "API",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/AdvancedDevSample.Api/bin/Debug/net10.0/AdvancedDevSample.Api.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\blisten on.*:([0-9]+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

---

## Conventions de code

### Nommage

| Type | Convention | Exemple |
|------|-----------|---------|
| **Classe** | PascalCase | `ProductService`, `OrderRepository` |
| **Interface** | PascalCase (commence par I) | `IProductService`, `IRepository<T>` |
| **Méthode** | PascalCase | `GetProductByIdAsync()`, `CreateOrderAsync()` |
| **Propriété publique** | PascalCase | `ProductId`, `TotalAmount` |
| **Propriété privée** | _camelCase | `_productRepository`, `_logger` |
| **Variable locale** | camelCase | `productId`, `totalAmount` |
| **Constante** | UPPER_SNAKE_CASE | `MAX_ORDER_ITEMS`, `DEFAULT_TIMEOUT` |
| **Énumération** | PascalCase | `OrderStatus`, `UserRole` |

### Async/Await

Toujours utiliser async/await pour les opérations I/O :

```csharp
// ✅ Bon
public async Task<ProductDto> GetProductAsync(Guid id)
{
    return await _repository.GetByIdAsync(id);
}

// ❌ Mauvais
public ProductDto GetProduct(Guid id)
{
    return _repository.GetById(id).Result; // Peut causer un deadlock
}

// ✅ Bon - avec Task complétée
public Task<int> SaveAsync()
{
    return Task.FromResult(_context.SaveChanges());
}
```

### Null checking

Utiliser les patterns C# modernes :

```csharp
// ✅ Bon (C# 8+)
if (customer is null)
    throw new ArgumentNullException(nameof(customer));

// ✅ Bon (C# 6+)
var product = productDto ?? throw new InvalidOperationException();

// ❌ Ancien
if (customer == null)
    throw new ArgumentNullException(nameof(customer));
```

### Logging

Utiliser ILogger injecté :

```csharp
public class ProductService
{
    private readonly ILogger<ProductService> _logger;

    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        _logger.LogInformation("Creating product: {productName}", dto.Name);
        
        try
        {
            var product = new Product(dto.Name, dto.Price, dto.SupplierId);
            await _repository.AddAsync(product);
            
            _logger.LogInformation("Product created successfully: {productId}", product.Id);
            return MapToDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {productName}", dto.Name);
            throw;
        }
    }
}
```

### Documentation XML

Documenter les méthodes publiques :

```csharp
/// <summary>
/// Crée un nouveau produit dans le système.
/// </summary>
/// <param name="createDto">Les données du produit à créer</param>
/// <returns>Le produit créé avec ses propriétés complètes</returns>
/// <exception cref="ArgumentNullException">Si createDto est null</exception>
/// <exception cref="ApplicationException">Si le fournisseur n'existe pas</exception>
public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
{
    // ...
}
```

---

## Structure de fichiers

### Organisation recommandée

```
AdvancedDevSample.Api/
├── Controllers/
│   └── ProductsController.cs           (Entrypoint)
├── Filters/
│   ├── GlobalExceptionFilter.cs        (Exception handling)
│   ├── LoggingActionFilter.cs          (Logging)
│   └── ValidationFilter.cs             (Validation)
├── Middlewares/
│   ├── ExceptionHandlingMiddleware.cs
│   ├── PerformanceMiddleware.cs
│   └── RequestLoggingMiddleware.cs
└── Program.cs                          (Configuration)

AdvancedDevSample.Application/
├── Interfaces/Services/
│   └── IProductService.cs              (Contrat)
├── Services/
│   └── ProductService.cs               (Implémentation)
├── DTOs/
│   ├── CreateProductDto.cs             (Création)
│   ├── UpdateProductDto.cs             (Mise à jour)
│   └── ProductDto.cs                   (Lecture)
├── Exceptions/
│   └── ApplicationException.cs
└── DependencyInjection.cs              (Configuration)

AdvancedDevSample.Domain/
├── Entities/
│   └── Product.cs                      (Entité métier)
├── Enums/
│   └── OrderStatus.cs                  (Énumérations)
├── Events/
│   └── ProductEvents.cs                (Événements métier)
├── Exceptions/
│   └── DomainExceptions.cs             (Exceptions métier)
├── Interfaces/
│   └── IProductRepository.cs           (Contrat repos)
└── Common/
    └── BaseEntity.cs                   (Classe de base)

AdvancedDevSample.Infrastructure/
├── DbContext/
│   └── AdvancedDevSampleDbContext.cs
├── Repositories/
│   └── EfProductRepository.cs          (Implémentation)
├── Migrations/
│   └── 20240115000000_AddProducts.cs
└── DependencyInjection.cs              (Configuration)
```

---

## Ajouter une nouvelle fonctionnalité

### Exemple : Ajouter un nouveau domaine "Category" (Catégorie)

#### Étape 1 : Créer l'entité domaine

`AdvancedDevSample.Domain/Entities/Category.cs`

```csharp
using AdvancedDevSample.Domain.Common;
using System;

namespace AdvancedDevSample.Domain.Entities
{
    /// <summary>
    /// Représente une catégorie de produits
    /// </summary>
    public class Category : BaseEntity
    {
        private Category() { }

        public Category(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Le nom est requis", nameof(name));

            Id = Guid.NewGuid();
            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public void Update(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Le nom est requis", nameof(name));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
```

#### Étape 2 : Créer l'interface repository

`AdvancedDevSample.Domain/Interfaces/ICategoryRepository.cs`

```csharp
namespace AdvancedDevSample.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetActiveAsync();
    }
}
```

#### Étape 3 : Créer les DTOs

`AdvancedDevSample.Application/DTOs/CategoryDto.cs`

```csharp
namespace AdvancedDevSample.Application.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
```

#### Étape 4 : Créer l'interface service

`AdvancedDevSample.Application/Interfaces/Services/ICategoryService.cs`

```csharp
namespace AdvancedDevSample.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetByIdAsync(Guid id);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto);
        Task DeleteAsync(Guid id);
    }
}
```

#### Étape 5 : Implémenter le service

`AdvancedDevSample.Application/Services/CategoryService.cs`

```csharp
using AdvancedDevSample.Application.DTOs;
using AdvancedDevSample.Application.Interfaces.Services;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Interfaces;

namespace AdvancedDevSample.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category(dto.Name, dto.Description);
            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();
            
            _logger.LogInformation("Category created: {categoryId}", category.Id);
            return MapToDto(category);
        }

        public async Task<CategoryDto> GetByIdAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id)
                ?? throw new ApplicationException("Category not found");
            
            return MapToDto(category);
        }

        // Autres méthodes...

        private static CategoryDto MapToDto(Category category) => new()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }
}
```

#### Étape 6 : Créer le repository

`AdvancedDevSample.Infrastructure/Repositories/EfCategoryRepository.cs`

```csharp
namespace AdvancedDevSample.Infrastructure.Repositories
{
    public class EfCategoryRepository : ICategoryRepository
    {
        private readonly AdvancedDevSampleDbContext _context;

        public EfCategoryRepository(AdvancedDevSampleDbContext context) => _context = context;

        public async Task<Category?> GetByIdAsync(Guid id)
            => await _context.Categories.FindAsync(id);

        public async Task<Category?> GetByNameAsync(string name)
            => await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);

        public async Task<IEnumerable<Category>> GetActiveAsync()
            => await _context.Categories.Where(c => c.IsActive).ToListAsync();

        public async Task AddAsync(Category category)
            => await _context.Categories.AddAsync(category);

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
                _context.Categories.Remove(category);
        }

        // Autres méthodes...
    }
}
```

#### Étape 7 : Créer le contrôleur

`AdvancedDevSample.Api/Controllers/CategoriesController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
        => _categoryService = categoryService;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    // Autres actions...
}
```

#### Étape 8 : Mettre à jour DbContext

Ajouter au `DbContext` :

```csharp
public DbSet<Category> Categories { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Category>()
        .HasKey(c => c.Id);

    modelBuilder.Entity<Category>()
        .Property(c => c.Name)
        .IsRequired()
        .HasMaxLength(100);

    // Configuration supplémentaire...
}
```

#### Étape 9 : Créer la migration

```powershell
dotnet ef migrations add AddCategories --project ..\AdvancedDevSample.Infrastructure
dotnet ef database update
```

---

## Gérer la base de données

### Créer une migration

```powershell
cd AdvancedDevSample.Api

# Créer une migration
dotnet ef migrations add AddNewFeature --project ..\AdvancedDevSample.Infrastructure

# Voir les migrations
dotnet ef migrations list --project ..\AdvancedDevSample.Infrastructure
```

### Appliquer les migrations

```powershell
# Appliquer toutes les migrations en attente
dotnet ef database update --project ..\AdvancedDevSample.Infrastructure

# Appliquer jusqu'à une migration spécifique
dotnet ef database update 20240115000000_AddProducts --project ..\AdvancedDevSample.Infrastructure
```

### Annuler une migration

```powershell
# Revenir à la migration précédente
dotnet ef database update 20240114000000_InitialCreate --project ..\AdvancedDevSample.Infrastructure

# Supprimer la dernière migration du code
dotnet ef migrations remove --project ..\AdvancedDevSample.Infrastructure
```

### Générer le script SQL

```powershell
# Script SQL pour la migration
dotnet ef migrations script --project ..\AdvancedDevSample.Infrastructure --output migration.sql
```

---

## Workflow Git

### Créer une branche pour une feature

```bash
# Récupérer les derniers changements
git fetch origin

# Créer et basculer vers une nouvelle branche
git checkout -b feature/add-categories

# Ou pour un bug fix
git checkout -b bugfix/fix-order-total
```

### Committer les changements

```bash
# Voir l'état
git status

# Ajouter les fichiers
git add .

# Ou ajouter des fichiers spécifiques
git add AdvancedDevSample.Api/Controllers/

# Committer avec un message descriptif
git commit -m "feat: add category management feature

- Add Category entity to domain
- Implement ICategoryService
- Add CategoriesController
- Create database migration"
```

### Pousser et créer une Pull Request

```bash
# Pousser la branche
git push origin feature/add-categories

# Sur GitHub, créer une Pull Request
# 1. Aller sur le repo
# 2. Cliquer sur "Pull Requests"
# 3. Cliquer sur "New Pull Request"
# 4. Sélectionner votre branche
# 5. Décrire les changements
# 6. Cliquer sur "Create Pull Request"
```

### Fusionner après review

```bash
# Après approbation, fusionner dans main
git checkout main
git pull origin main
git merge feature/add-categories
git push origin main
```

---

## Debugging

### Breakpoints

1. **Ajouter un breakpoint** : Cliquer sur la marge gauche du code
2. **Breakpoint conditionnel** : Clic droit → Filter
3. **Logpoint** : Clic droit → Insert Logpoint

### Inspecting Variables

```csharp
// Dans un breakpoint, vérifier les valeurs
Debug.WriteLine($"Product: {product.Name}, Price: {product.Price}");

// Ou utiliser la fenêtre Watch
// Debug → Windows → Watch
```

### Console de débogage

```powershell
# Lancer en mode debug
dotnet run --configuration Debug

# Voir les logs détaillés
# Output Window → Debug
```

---

## Performance

### Optimisations courantes

#### 1. Utiliser `.AsNoTracking()` pour les lectures seules

```csharp
// ✅ Bon - pas de suivi EF Core
public async Task<ProductDto> GetProductAsync(Guid id)
{
    var product = await _context.Products
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id);
    
    return MapToDto(product);
}

// ❌ Mauvais - suivi inutile
var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
```

#### 2. Utiliser `.SelectAsync()` pour les projections

```csharp
// ✅ Bon - récupère uniquement les champs nécessaires
var products = await _context.Products
    .AsNoTracking()
    .Select(p => new ProductDto 
    { 
        Id = p.Id, 
        Name = p.Name, 
        Price = p.Price 
    })
    .ToListAsync();

// ❌ Mauvais - récupère tout du produit
var products = await _context.Products
    .AsNoTracking()
    .ToListAsync()
    .Select(p => MapToDto(p));
```

#### 3. Utiliser `.Include()` pour les relations

```csharp
// ✅ Bon - une seule requête
var order = await _context.Orders
    .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
    .FirstOrDefaultAsync(o => o.Id == orderId);

// ❌ Mauvais - N+1 queries
var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
var items = await _context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
```

#### 4. Caching des données fréquemment accédées

```csharp
public class CategoryService
{
    private readonly IMemoryCache _cache;
    private const string CATEGORIES_CACHE_KEY = "all_categories";

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        if (_cache.TryGetValue(CATEGORIES_CACHE_KEY, out IEnumerable<CategoryDto> categories))
            return categories;

        categories = await _unitOfWork.CategoryRepository.GetAllAsync();
        _cache.Set(CATEGORIES_CACHE_KEY, categories, TimeSpan.FromHours(1));

        return categories;
    }
}
```

---

## Résumé des bonnes pratiques

✅ **À faire** :
- Utiliser les conventions de nommage
- Documenter les méthodes publiques
- Utiliser async/await
- Créer des branches pour les features
- Ecrire des messages de commit descriptifs
- Tester le code avant de pousser
- Optimiser les requêtes DB

❌ **À éviter** :
- Utiliser `.Result` ou `.Wait()`
- Faire du code spaghetti sans séparation
- Utiliser des strings magiques
- Ignorer les avertissements du compilateur
- Committer du code sans tester
- Mettre les secrets en dur dans le code

Pour des questions spécifiques, consultez les autres documents de documentation.
