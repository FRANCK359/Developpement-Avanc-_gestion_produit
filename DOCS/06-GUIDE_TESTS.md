# Guide des Tests - AdvancedDevSample

## 📌 Table des matières
- [Vue d'ensemble](#vue-densemble)
- [Architecture des tests](#architecture-des-tests)
- [Tests unitaires](#tests-unitaires)
- [Tests d'intégration](#tests-dintégration)
- [Tests de composants](#tests-de-composants)
- [Exécuter les tests](#exécuter-les-tests)
- [Bonnes pratiques](#bonnes-pratiques)

---

## Vue d'ensemble

L'application utilise **xUnit** comme framework de test avec **Moq** pour les mocks.

### Types de tests

| Type | Scope | Framework | Exemple |
|------|-------|-----------|---------|
| **Unitaires** | Une classe seule | xUnit + Moq | Tester ProductService |
| **Intégration** | Plusieurs couches | xUnit | Tester le flux complet API→DB |
| **Composants** | Controllers | xUnit | Tester ProductsController |
| **E2E** | Système complet | Postman/Playwright | Test complet via API |

### Structure des tests

```
AdvancedDevSample.Test/
├── Application/
│   ├── ProductServiceTests.cs       # Tests du service
│   ├── CustomerServiceTests.cs
│   └── OrderServiceTests.cs
├── Domain/
│   ├── CustomerTests.cs             # Tests de l'entité
│   ├── OrderTests.cs
│   └── ProductTests.cs
├── Components/
│   ├── ProductsControllerTests.cs   # Tests du contrôleur
│   ├── OrdersControllerTests.cs
│   └── AuthControllerTests.cs
└── Integration/
    ├── ProductApiTests.cs           # Tests d'intégration API
    └── OrderApiTests.cs
```

---

## Architecture des tests

### Arrange-Act-Assert (AAA)

Chaque test suit le pattern AAA :

```csharp
[Fact]
public async Task CreateProduct_WithValidData_ReturnsSuccessResponse()
{
    // ARRANGE : Préparer les données et les mocks
    var productDto = new CreateProductDto 
    { 
        Name = "Laptop", 
        Price = 999.99M,
        SupplierId = Guid.NewGuid()
    };
    var mockRepository = new Mock<IUnitOfWork>();
    var service = new ProductService(mockRepository.Object, null);

    // ACT : Exécuter l'action
    var result = await service.CreateAsync(productDto);

    // ASSERT : Vérifier le résultat
    Assert.NotNull(result);
    Assert.Equal("Laptop", result.Name);
    Assert.Equal(999.99M, result.Price);
}
```

### Fixtures et Helpers

Pour partager la configuration entre tests :

```csharp
public class ProductServiceFixture : IDisposable
{
    public Mock<IUnitOfWork> MockUnitOfWork { get; }
    public Mock<ILogger<ProductService>> MockLogger { get; }
    public ProductService ProductService { get; }

    public ProductServiceFixture()
    {
        MockUnitOfWork = new Mock<IUnitOfWork>();
        MockLogger = new Mock<ILogger<ProductService>>();
        ProductService = new ProductService(MockUnitOfWork.Object, MockLogger.Object);
    }

    public void Dispose()
    {
        // Nettoyage
    }
}

// Utilisation
[Collection("Product Service Collection")]
public class ProductServiceTests : IClassFixture<ProductServiceFixture>
{
    private readonly ProductServiceFixture _fixture;

    public ProductServiceTests(ProductServiceFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CreateProduct_ReturnsDto()
    {
        var result = await _fixture.ProductService.CreateAsync(/* ... */);
        Assert.NotNull(result);
    }
}
```

---

## Tests unitaires

### Test d'un Service

```csharp
namespace AdvancedDevSample.Test.Application
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
        {
            // ARRANGE
            var productId = Guid.NewGuid();
            var product = new Product("Test Product", 99.99M, Guid.NewGuid());
            
            _mockUnitOfWork
                .Setup(x => x.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(product);

            // ACT
            var result = await _service.GetProductByIdAsync(productId);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("Test Product", result.Name);
            _mockUnitOfWork.Verify(x => x.ProductRepository.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithInvalidId_ThrowsException()
        {
            // ARRANGE
            var productId = Guid.NewGuid();
            
            _mockUnitOfWork
                .Setup(x => x.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync((Product)null);

            // ACT & ASSERT
            await Assert.ThrowsAsync<ApplicationException>(
                () => _service.GetProductByIdAsync(productId)
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateProductAsync_WithInvalidName_ThrowsException(string invalidName)
        {
            // ARRANGE
            var dto = new CreateProductDto 
            { 
                Name = invalidName, 
                Price = 99.99M,
                SupplierId = Guid.NewGuid()
            };

            // ACT & ASSERT
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(dto)
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task CreateProductAsync_WithNegativePrice_ThrowsException(decimal invalidPrice)
        {
            // ARRANGE
            var dto = new CreateProductDto 
            { 
                Name = "Product", 
                Price = invalidPrice,
                SupplierId = Guid.NewGuid()
            };

            // ACT & ASSERT
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(dto)
            );
        }

        [Fact]
        public async Task CreateProductAsync_WithValidData_CallsSaveAsync()
        {
            // ARRANGE
            var dto = new CreateProductDto 
            { 
                Name = "New Product", 
                Price = 199.99M,
                SupplierId = Guid.NewGuid()
            };

            _mockUnitOfWork
                .Setup(x => x.ProductRepository.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync())
                .ReturnsAsync(1);

            // ACT
            var result = await _service.CreateAsync(dto);

            // ASSERT
            Assert.NotNull(result);
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}
```

### Test d'une Entité Domain

```csharp
namespace AdvancedDevSample.Test.Domain
{
    public class OrderTests
    {
        [Fact]
        public void Order_Constructor_CreatesOrderWithPendingStatus()
        {
            // ARRANGE & ACT
            var customerId = Guid.NewGuid();
            var order = new Order(customerId);

            // ASSERT
            Assert.Equal(customerId, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(0, order.TotalAmount);
            Assert.Empty(order.OrderItems);
        }

        [Fact]
        public void Order_AddProduct_AddsProductToOrder()
        {
            // ARRANGE
            var order = new Order(Guid.NewGuid());
            var product = new Product("Laptop", 999.99M, Guid.NewGuid());

            // ACT
            order.AddProduct(product, 1);

            // ASSERT
            Assert.Single(order.OrderItems);
            Assert.Equal(999.99M, order.TotalAmount);
        }

        [Fact]
        public void Order_AddProduct_WithInactiveProduct_ThrowsException()
        {
            // ARRANGE
            var order = new Order(Guid.NewGuid());
            var product = new Product("Laptop", 999.99M, Guid.NewGuid());
            product.Deactivate();

            // ACT & ASSERT
            Assert.Throws<DomainException>(
                () => order.AddProduct(product, 1)
            );
        }

        [Fact]
        public void Order_AddProduct_ToConfirmedOrder_ThrowsException()
        {
            // ARRANGE
            var order = new Order(Guid.NewGuid());
            var product = new Product("Laptop", 999.99M, Guid.NewGuid());
            
            order.AddProduct(product, 1);
            order.Confirm();

            var anotherProduct = new Product("Monitor", 299.99M, Guid.NewGuid());

            // ACT & ASSERT
            Assert.Throws<DomainException>(
                () => order.AddProduct(anotherProduct, 1)
            );
        }

        [Fact]
        public void Order_RemoveProduct_ReducesTotalAmount()
        {
            // ARRANGE
            var order = new Order(Guid.NewGuid());
            var product1 = new Product("Laptop", 999.99M, Guid.NewGuid());
            var product2 = new Product("Monitor", 299.99M, Guid.NewGuid());

            order.AddProduct(product1, 1);
            order.AddProduct(product2, 1);

            var totalBefore = order.TotalAmount; // 1299.98

            // ACT
            order.RemoveProduct(product1.Id);

            // ASSERT
            Assert.Equal(299.99M, order.TotalAmount);
            Assert.Single(order.OrderItems);
        }

        [Fact]
        public void Order_Confirm_WithNoItems_ThrowsException()
        {
            // ARRANGE
            var order = new Order(Guid.NewGuid());

            // ACT & ASSERT
            Assert.Throws<DomainException>(() => order.Confirm());
        }

        [Fact]
        public void Order_Confirm_ChangeStatusToConfirmed()
        {
            // ARRANGE
            var order = new Order(Guid.NewGuid());
            var product = new Product("Laptop", 999.99M, Guid.NewGuid());
            order.AddProduct(product, 1);

            // ACT
            order.Confirm();

            // ASSERT
            Assert.Equal(OrderStatus.Confirmed, order.Status);
        }
    }
}
```

---

## Tests d'intégration

### Test d'intégration API

```csharp
namespace AdvancedDevSample.Test.Integration
{
    public class ProductApiTests : IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public async Task InitializeAsync()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remplacer le DbContext par InMemory
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AdvancedDevSampleDbContext>));
                        
                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<AdvancedDevSampleDbContext>(options =>
                            options.UseInMemoryDatabase("TestDb")
                        );
                    });
                });

            _client = _factory.CreateClient();
            await InitializeDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            await _factory.DisposeAsync();
            _client.Dispose();
        }

        private async Task InitializeDatabaseAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AdvancedDevSampleDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }

        [Fact]
        public async Task GetProducts_ReturnsSuccessStatusCode()
        {
            // ACT
            var response = await _client.GetAsync("/api/products");

            // ASSERT
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("items", content);
        }

        [Fact]
        public async Task CreateProduct_WithValidData_ReturnsCreatedStatusCode()
        {
            // ARRANGE
            var createDto = new CreateProductDto
            {
                Name = "Test Product",
                Price = 99.99M,
                SupplierId = Guid.NewGuid()
            };
            var content = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json"
            );

            // ACT
            var response = await _client.PostAsync("/api/products", content);

            // ASSERT
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetProduct_WithInvalidId_ReturnsNotFoundStatusCode()
        {
            // ACT
            var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

            // ASSERT
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
```

---

## Tests de composants

### Test de Contrôleur

```csharp
namespace AdvancedDevSample.Test.Components
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductsController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetProduct_WithValidId_ReturnsOkResult()
        {
            // ARRANGE
            var productId = Guid.NewGuid();
            var productDto = new ProductDto 
            { 
                Id = productId, 
                Name = "Laptop", 
                Price = 999.99M 
            };

            _mockProductService
                .Setup(x => x.GetProductByIdAsync(productId))
                .ReturnsAsync(productDto);

            // ACT
            var result = await _controller.GetProduct(productId);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(productId, returnedDto.Id);
        }

        [Fact]
        public async Task CreateProduct_WithValidData_ReturnsCreatedAtActionResult()
        {
            // ARRANGE
            var createDto = new CreateProductDto 
            { 
                Name = "Laptop", 
                Price = 999.99M,
                SupplierId = Guid.NewGuid()
            };
            
            var createdDto = new ProductDto 
            { 
                Id = Guid.NewGuid(), 
                Name = "Laptop", 
                Price = 999.99M 
            };

            _mockProductService
                .Setup(x => x.CreateProductAsync(createDto))
                .ReturnsAsync(createdDto);

            // ACT
            var result = await _controller.CreateProduct(createDto);

            // ASSERT
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ProductsController.GetProduct), createdResult.ActionName);
            Assert.Equal(createdDto.Id, ((ProductDto)createdResult.Value).Id);
        }

        [Fact]
        public async Task DeleteProduct_WithValidId_ReturnsNoContentResult()
        {
            // ARRANGE
            var productId = Guid.NewGuid();

            _mockProductService
                .Setup(x => x.DeleteProductAsync(productId))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _controller.DeleteProduct(productId);

            // ASSERT
            Assert.IsType<NoContentResult>(result);
            _mockProductService.Verify(x => x.DeleteProductAsync(productId), Times.Once);
        }
    }
}
```

---

## Exécuter les tests

### Via ligne de commande

```powershell
# Lancer tous les tests
dotnet test

# Lancer les tests avec verbosité
dotnet test --verbosity detailed

# Lancer les tests d'un projet spécifique
dotnet test AdvancedDevSample.Test.csproj

# Lancer les tests avec un pattern
dotnet test --filter "FullyQualifiedName~ProductServiceTests"

# Générer un rapport de couverture
dotnet test /p:CollectCoverageEnabled=true /p:CoverageFormat=lcov
```

### Via Visual Studio

1. **Ouvrir Test Explorer** : Test → Test Explorer (Ctrl + E, T)
2. **Lancer les tests** :
   - Cliquer sur "Run All" pour tous les tests
   - Cliquer sur une classe pour lancer ses tests
   - Clic droit sur un test pour le lancer seul

3. **Debugging** : Clic droit → Debug

### Via Visual Studio Code

```json
// .vscode/tasks.json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Run Tests",
      "command": "dotnet",
      "type": "shell",
      "args": ["test"],
      "group": {
        "kind": "test",
        "isDefault": true
      }
    }
  ]
}
```

Puis appuyer sur Ctrl + Shift + D pour lancer.

---

## Bonnes pratiques

### ✅ À faire

1. **Suivre le pattern AAA** : Arrange, Act, Assert
2. **Un test = un comportement** : Ne tester qu'une chose par test
3. **Noms descriptifs** : `CreateProduct_WithInvalidPrice_ThrowsException`
4. **Utiliser [Theory] et [InlineData]** pour les tests paramétrés
5. **Utiliser les fixtures** pour partager la configuration
6. **Vérifier les appels** avec `.Verify()`
7. **Tester les cas d'erreur** aussi bien que les cas de succès
8. **Isoler les tests** : Chaque test doit être indépendant

### ❌ À éviter

1. **Plusieurs assertions** : Un test, une affirmation principale
2. **Dépendances entre tests** : Les tests doivent s'exécuter dans n'importe quel ordre
3. **Utiliser le vrai base de données** : Utiliser les mocks ou InMemory
4. **Tester l'implémentation** : Tester le comportement, pas le code
5. **Tests flaky** : Éviter les delais et les hasards
6. **Négliger les cas limites** : Tester les valeurs null, vides, négatives
7. **Commentaires inutiles** : Les noms de tests doivent être explicites
8. **Utiliser des données aléatoires** sans prévisibilité

### Exemples d'assertions

```csharp
// Assertions de base
Assert.True(condition);
Assert.False(condition);
Assert.Null(value);
Assert.NotNull(value);

// Assertions de valeur
Assert.Equal(expected, actual);
Assert.NotEqual(expected, actual);
Assert.Contains(item, collection);
Assert.DoesNotContain(item, collection);

// Assertions de plage
Assert.InRange(value, min, max);

// Assertions d'exception
Assert.Throws<ArgumentException>(() => { /* code */ });
await Assert.ThrowsAsync<ArgumentException>(() => { /* code async */ });

// Assertions de collection
Assert.Empty(collection);
Assert.NotEmpty(collection);
Assert.Single(collection);
Assert.Equal(expectedCount, collection.Count());

// Assertions d'égalité stricte
Assert.StrictEqual(expected, actual);
Assert.NotStrictEqual(expected, actual);
```

---

## Coverage des tests

### Générer un rapport

```powershell
# Installer Coverlet
dotnet add package Coverlet.Collector --version 3.2.0

# Lancer les tests avec coverage
dotnet test /p:CollectCoverageEnabled=true /p:CoverageFormat=lcov

# Générer un rapport HTML
dotnet test /p:CollectCoverageEnabled=true /p:CoverageFormat=opencover
```

### Objectifs de coverage

| Cible | Pourcentage |
|-------|-----------|
| **Domain** | 85%+ (logique métier critique) |
| **Services** | 80%+ (logique applicative) |
| **Controllers** | 70%+ (routage et validation) |
| **Infrastructure** | 75%+ (repos, migrations) |
| **Global** | 75%+ |

---

## Résumé

Une suite de tests robuste comprend :

✅ Tests unitaires des services et entités  
✅ Tests d'intégration du flux API complet  
✅ Tests des contrôleurs avec mocks  
✅ Tests des cas d'erreur et limites  
✅ Coverage > 75%  
✅ CI/CD avec tests automatiques  

Pour plus d'informations, consultez la documentation xUnit officielle.
