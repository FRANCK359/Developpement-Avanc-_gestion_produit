# Guide complet de l'API - AdvancedDevSample

## 📌 Table des matières
- [Introduction](#introduction)
- [Base de l'API REST](#base-de-lapi-rest)
- [Authentification JWT](#authentification-jwt)
- [Endpoints disponibles](#endpoints-disponibles)
- [Codes de réponse](#codes-de-réponse)
- [Formats des données](#formats-des-données)
- [Gestion des erreurs](#gestion-des-erreurs)
- [Exemples complets](#exemples-complets)

---

## Introduction

L'API AdvancedDevSample fournit une interface REST complète pour :
- ✅ Gérer les clients (CRUD)
- ✅ Gérer les commandes (CRUD + gestion du statut)
- ✅ Gérer les produits (CRUD + recherche)
- ✅ Gérer les fournisseurs (CRUD)
- ✅ Authentification sécurisée avec JWT

### URL de base

```
http://localhost:5000/api       (HTTP)
https://localhost:7000/api      (HTTPS)
https://localhost:7000/swagger  (Documentation Swagger)
```

### Headers requis

```http
Content-Type: application/json
Accept: application/json
Authorization: Bearer {JWT_TOKEN}  (pour endpoints protégés)
```

---

## Base de l'API REST

### Principes RESTful

| Méthode | Opération | Exemple |
|---------|-----------|---------|
| **GET** | Récupérer une ressource | `GET /api/products/123` |
| **GET** | Lister des ressources | `GET /api/products` |
| **POST** | Créer une ressource | `POST /api/products` |
| **PUT** | Remplacer une ressource | `PUT /api/products/123` |
| **PATCH** | Modifier une ressource | `PATCH /api/products/123` |
| **DELETE** | Supprimer une ressource | `DELETE /api/products/123` |

### Structure de réponse

**Succès (200)** :
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Laptop",
  "price": 999.99,
  "isActive": true
}
```

**Erreur (400+)** :
```json
{
  "error": {
    "code": "PRODUCT_NOT_FOUND",
    "message": "Le produit spécifié n'existe pas",
    "details": {
      "productId": "550e8400-e29b-41d4-a716-446655440000"
    }
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## Authentification JWT

### Obtenir un token JWT

**Endpoint** :
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}
```

**Réponse** :
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "tokenType": "Bearer",
  "user": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "username": "admin",
    "email": "admin@example.com",
    "roles": ["Admin"]
  }
}
```

### Utiliser le token

Incluez le token dans le header `Authorization` :

```http
GET /api/products
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Renouveler le token

**Endpoint** :
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "..."
}
```

### Décodage du JWT

Structure d'un JWT :
```
[Header].[Payload].[Signature]

Header: {"alg": "HS256", "typ": "JWT"}

Payload: {
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "username": "admin",
  "email": "admin@example.com",
  "roles": ["Admin"],
  "exp": 1705318200,
  "iat": 1705314600
}
```

Expiration : 60 minutes par défaut (configurable dans `appsettings.json`)

---

## Endpoints disponibles

### 👥 Customers (Clients)

#### 1. Lister tous les clients

```http
GET /api/customers
Authorization: Bearer {token}
```

**Paramètres de requête** :
```
?pageNumber=1&pageSize=10
?search=John
?isActive=true
```

**Réponse** (200 OK) :
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "firstName": "Jean",
      "lastName": "Dupont",
      "email": "jean.dupont@example.com",
      "isActive": true,
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "totalCount": 42,
  "pageNumber": 1,
  "pageSize": 10
}
```

#### 2. Récupérer un client

```http
GET /api/customers/{id}
Authorization: Bearer {token}
```

**Réponse** (200 OK) :
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "firstName": "Jean",
  "lastName": "Dupont",
  "email": "jean.dupont@example.com",
  "isActive": true,
  "createdAt": "2024-01-15T10:30:00Z",
  "orders": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440000",
      "orderDate": "2024-01-15T10:35:00Z",
      "status": "Pending",
      "totalAmount": 1500.00
    }
  ]
}
```

#### 3. Créer un client

```http
POST /api/customers
Content-Type: application/json
Authorization: Bearer {token}

{
  "firstName": "Pierre",
  "lastName": "Martin",
  "email": "pierre.martin@example.com"
}
```

**Réponse** (201 Created) :
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440000",
  "firstName": "Pierre",
  "lastName": "Martin",
  "email": "pierre.martin@example.com",
  "isActive": true,
  "createdAt": "2024-01-15T11:00:00Z"
}
```

#### 4. Mettre à jour un client

```http
PUT /api/customers/{id}
Content-Type: application/json
Authorization: Bearer {token}

{
  "firstName": "Pierre",
  "lastName": "Martin",
  "email": "pierre.martin.new@example.com"
}
```

#### 5. Supprimer un client

```http
DELETE /api/customers/{id}
Authorization: Bearer {token}
```

**Réponse** (204 No Content)

---

### 📦 Products (Produits)

#### 1. Lister tous les produits

```http
GET /api/products
```

**Paramètres** :
```
?pageNumber=1&pageSize=10
?search=laptop
?supplierId={id}
?minPrice=100
?maxPrice=5000
?isActive=true
```

**Réponse** (200 OK) :
```json
{
  "items": [
    {
      "id": "880e8400-e29b-41d4-a716-446655440000",
      "name": "Laptop Pro",
      "description": "Professional laptop",
      "price": 1999.99,
      "isActive": true,
      "supplierId": "990e8400-e29b-41d4-a716-446655440000",
      "supplierName": "TechCorp",
      "createdAt": "2024-01-10T09:00:00Z"
    }
  ],
  "totalCount": 156,
  "pageNumber": 1,
  "pageSize": 10
}
```

#### 2. Récupérer un produit

```http
GET /api/products/{id}
```

**Réponse** (200 OK) :
```json
{
  "id": "880e8400-e29b-41d4-a716-446655440000",
  "name": "Laptop Pro",
  "description": "Professional laptop with 16GB RAM",
  "price": 1999.99,
  "isActive": true,
  "supplierId": "990e8400-e29b-41d4-a716-446655440000",
  "supplierName": "TechCorp",
  "createdAt": "2024-01-10T09:00:00Z",
  "updatedAt": "2024-01-15T10:00:00Z"
}
```

#### 3. Créer un produit (Admin uniquement)

```http
POST /api/products
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Monitor 4K",
  "description": "Ultra HD 4K Monitor",
  "price": 499.99,
  "supplierId": "990e8400-e29b-41d4-a716-446655440000"
}
```

**Réponse** (201 Created) :
```json
{
  "id": "aa0e8400-e29b-41d4-a716-446655440000",
  "name": "Monitor 4K",
  "description": "Ultra HD 4K Monitor",
  "price": 499.99,
  "isActive": true,
  "supplierId": "990e8400-e29b-41d4-a716-446655440000",
  "createdAt": "2024-01-15T11:30:00Z"
}
```

#### 4. Mettre à jour un produit (Admin uniquement)

```http
PUT /api/products/{id}
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Monitor 4K Pro",
  "description": "Premium Ultra HD 4K Monitor",
  "price": 599.99,
  "supplierId": "990e8400-e29b-41d4-a716-446655440000"
}
```

#### 5. Supprimer un produit (Admin uniquement)

```http
DELETE /api/products/{id}
Authorization: Bearer {token}
```

---

### 📋 Orders (Commandes)

#### 1. Lister toutes les commandes

```http
GET /api/orders
Authorization: Bearer {token}
```

**Paramètres** :
```
?customerId={id}
?status=Pending
?fromDate=2024-01-01
?toDate=2024-01-31
```

**Réponse** (200 OK) :
```json
{
  "items": [
    {
      "id": "bb0e8400-e29b-41d4-a716-446655440000",
      "customerId": "550e8400-e29b-41d4-a716-446655440000",
      "customerName": "Jean Dupont",
      "orderDate": "2024-01-15T10:35:00Z",
      "status": "Pending",
      "totalAmount": 2499.98,
      "itemCount": 2,
      "createdAt": "2024-01-15T10:35:00Z"
    }
  ],
  "totalCount": 25,
  "pageNumber": 1,
  "pageSize": 10
}
```

#### 2. Récupérer une commande

```http
GET /api/orders/{id}
Authorization: Bearer {token}
```

**Réponse** (200 OK) :
```json
{
  "id": "bb0e8400-e29b-41d4-a716-446655440000",
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "customerName": "Jean Dupont",
  "orderDate": "2024-01-15T10:35:00Z",
  "status": "Pending",
  "totalAmount": 2499.98,
  "items": [
    {
      "id": "cc0e8400-e29b-41d4-a716-446655440000",
      "productId": "880e8400-e29b-41d4-a716-446655440000",
      "productName": "Laptop Pro",
      "quantity": 1,
      "unitPrice": 1999.99,
      "subtotal": 1999.99
    },
    {
      "id": "dd0e8400-e29b-41d4-a716-446655440000",
      "productId": "aa0e8400-e29b-41d4-a716-446655440000",
      "productName": "Monitor 4K",
      "quantity": 1,
      "unitPrice": 499.99,
      "subtotal": 499.99
    }
  ],
  "createdAt": "2024-01-15T10:35:00Z"
}
```

#### 3. Créer une commande

```http
POST /api/orders
Content-Type: application/json
Authorization: Bearer {token}

{
  "customerId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Réponse** (201 Created) :
```json
{
  "id": "ee0e8400-e29b-41d4-a716-446655440000",
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "orderDate": "2024-01-15T12:00:00Z",
  "status": "Pending",
  "totalAmount": 0,
  "items": [],
  "createdAt": "2024-01-15T12:00:00Z"
}
```

#### 4. Ajouter un produit à une commande

```http
POST /api/orders/{orderId}/items
Content-Type: application/json
Authorization: Bearer {token}

{
  "productId": "880e8400-e29b-41d4-a716-446655440000",
  "quantity": 2
}
```

**Réponse** (200 OK) : Commande mise à jour

#### 5. Retirer un produit d'une commande

```http
DELETE /api/orders/{orderId}/items/{productId}
Authorization: Bearer {token}
```

#### 6. Confirmer une commande

```http
POST /api/orders/{id}/confirm
Authorization: Bearer {token}
```

**Réponse** (200 OK) :
```json
{
  "id": "bb0e8400-e29b-41d4-a716-446655440000",
  "status": "Confirmed",
  "totalAmount": 2499.98,
  // ...
}
```

#### 7. Expédier une commande (Admin uniquement)

```http
POST /api/orders/{id}/ship
Authorization: Bearer {token}
```

**Réponse** (200 OK) :
```json
{
  "id": "bb0e8400-e29b-41d4-a716-446655440000",
  "status": "Shipped",
  "shippedAt": "2024-01-15T14:30:00Z"
}
```

#### 8. Annuler une commande

```http
POST /api/orders/{id}/cancel
Authorization: Bearer {token}

{
  "reason": "Client request"
}
```

---

### 🏢 Suppliers (Fournisseurs)

#### 1. Lister tous les fournisseurs

```http
GET /api/suppliers
Authorization: Bearer {token}
```

**Réponse** (200 OK) :
```json
{
  "items": [
    {
      "id": "990e8400-e29b-41d4-a716-446655440000",
      "name": "TechCorp",
      "email": "contact@techcorp.com",
      "isActive": true,
      "productCount": 25,
      "createdAt": "2024-01-01T09:00:00Z"
    }
  ],
  "totalCount": 12,
  "pageNumber": 1,
  "pageSize": 10
}
```

#### 2. Récupérer un fournisseur

```http
GET /api/suppliers/{id}
Authorization: Bearer {token}
```

**Réponse** (200 OK) :
```json
{
  "id": "990e8400-e29b-41d4-a716-446655440000",
  "name": "TechCorp",
  "email": "contact@techcorp.com",
  "isActive": true,
  "createdAt": "2024-01-01T09:00:00Z",
  "products": [
    {
      "id": "880e8400-e29b-41d4-a716-446655440000",
      "name": "Laptop Pro",
      "price": 1999.99
    }
  ]
}
```

#### 3. Créer un fournisseur (Admin uniquement)

```http
POST /api/suppliers
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "ElectroWorld",
  "email": "info@electroworld.com"
}
```

#### 4. Mettre à jour un fournisseur (Admin uniquement)

```http
PUT /api/suppliers/{id}
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "ElectroWorld Inc",
  "email": "contact@electroworld.com"
}
```

#### 5. Supprimer un fournisseur (Admin uniquement)

```http
DELETE /api/suppliers/{id}
Authorization: Bearer {token}
```

---

## Codes de réponse

| Code | Signification | Exemple |
|------|--------------|---------|
| **200 OK** | Succès - GET, PUT, PATCH | Récupération réussie |
| **201 Created** | Ressource créée - POST | Nouveau produit créé |
| **204 No Content** | Succès sans contenu - DELETE | Suppression réussie |
| **400 Bad Request** | Requête invalide | DTO incomplet ou invalide |
| **401 Unauthorized** | Non authentifié | Token JWT manquant/invalide |
| **403 Forbidden** | Non autorisé | Utilisateur sans permission |
| **404 Not Found** | Ressource introuvable | Produit n'existe pas |
| **409 Conflict** | Conflit métier | Email déjà utilisé |
| **500 Internal Server Error** | Erreur serveur | Exception non gérée |

---

## Formats des données

### ProductDto

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "string",
  "description": "string",
  "price": 0.00,
  "isActive": true,
  "supplierId": "550e8400-e29b-41d4-a716-446655440000",
  "supplierName": "string",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

### CustomerDto

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "firstName": "string",
  "lastName": "string",
  "email": "user@example.com",
  "isActive": true,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### OrderDto

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "customerName": "string",
  "orderDate": "2024-01-15T10:30:00Z",
  "status": "Pending|Confirmed|Shipped|Delivered|Cancelled",
  "totalAmount": 0.00,
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "productId": "550e8400-e29b-41d4-a716-446655440000",
      "productName": "string",
      "quantity": 0,
      "unitPrice": 0.00,
      "subtotal": 0.00
    }
  ],
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### SupplierDto

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "string",
  "email": "contact@example.com",
  "isActive": true,
  "productCount": 0,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

---

## Gestion des erreurs

### Erreur API standard

```json
{
  "error": {
    "code": "PRODUCT_NOT_FOUND",
    "message": "Le produit spécifié n'existe pas",
    "details": {
      "productId": "550e8400-e29b-41d4-a716-446655440000"
    }
  },
  "timestamp": "2024-01-15T10:30:00Z",
  "traceId": "0HMVVFQR52B5F:00000001"
}
```

### Codes d'erreur métier

| Code | Message | Cause |
|------|---------|-------|
| `CUSTOMER_NOT_FOUND` | Client non trouvé | ID invalide |
| `PRODUCT_NOT_FOUND` | Produit non trouvé | ID invalide |
| `ORDER_NOT_FOUND` | Commande non trouvée | ID invalide |
| `SUPPLIER_NOT_FOUND` | Fournisseur non trouvé | ID invalide |
| `INVALID_ORDER_STATUS` | Statut de commande invalide | Transition non autorisée |
| `EMAIL_ALREADY_EXISTS` | Email déjà utilisé | Email en doublon |
| `PRODUCT_INACTIVE` | Produit inactif | Produit désactivé |
| `INVALID_CREDENTIALS` | Identifiants invalides | Login/password incorrect |
| `UNAUTHORIZED` | Non autorisé | Token invalide |

---

## Exemples complets

### Exemple 1 : Créer une commande complète

```powershell
# 1. S'authentifier
$loginResponse = Invoke-RestMethod -Uri "https://localhost:7000/api/auth/login" `
  -Method Post `
  -Headers @{"Content-Type" = "application/json"} `
  -Body '{"username":"admin","password":"Admin123!"}' `
  -SkipCertificateCheck

$token = $loginResponse.token

# 2. Récupérer les ID des produits
$products = Invoke-RestMethod -Uri "https://localhost:7000/api/products?pageSize=2" `
  -Headers @{"Authorization" = "Bearer $token"} `
  -SkipCertificateCheck

# 3. Créer une commande
$orderResponse = Invoke-RestMethod -Uri "https://localhost:7000/api/orders" `
  -Method Post `
  -Headers @{"Authorization" = "Bearer $token"; "Content-Type" = "application/json"} `
  -Body '{"customerId":"550e8400-e29b-41d4-a716-446655440000"}' `
  -SkipCertificateCheck

$orderId = $orderResponse.id

# 4. Ajouter des produits à la commande
foreach ($product in $products.items) {
    Invoke-RestMethod -Uri "https://localhost:7000/api/orders/$orderId/items" `
      -Method Post `
      -Headers @{"Authorization" = "Bearer $token"; "Content-Type" = "application/json"} `
      -Body "{`"productId`":`"$($product.id)`",`"quantity`":1}" `
      -SkipCertificateCheck
}

# 5. Confirmer la commande
$confirmedOrder = Invoke-RestMethod -Uri "https://localhost:7000/api/orders/$orderId/confirm" `
  -Method Post `
  -Headers @{"Authorization" = "Bearer $token"} `
  -SkipCertificateCheck

Write-Host "Commande créée et confirmée : $($confirmedOrder.id)"
```

### Exemple 2 : Lister les produits avec filtres

```bash
curl -k -H "Authorization: Bearer $TOKEN" \
  "https://localhost:7000/api/products?pageNumber=1&pageSize=20&minPrice=100&maxPrice=2000&search=laptop"
```

Réponse :
```json
{
  "items": [
    {
      "id": "880e8400-e29b-41d4-a716-446655440000",
      "name": "Laptop Pro",
      "price": 1999.99
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 20
}
```

### Exemple 3 : Gestion des erreurs

```csharp
try
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);

    var response = await client.GetAsync("https://localhost:7000/api/products/invalid-id");
    
    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        var error = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
        
        Console.WriteLine($"Error Code: {error.Error.Code}");
        Console.WriteLine($"Error Message: {error.Error.Message}");
    }
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Request failed: {ex.Message}");
}
```

---

## Bonnes pratiques

✅ **À faire** :
- Toujours inclure le token JWT dans le header `Authorization`
- Gérer les codes de réponse HTTP appropriés
- Paginer les résultats pour les listes (pageNumber, pageSize)
- Utiliser les bons verbes HTTP (GET, POST, PUT, DELETE)
- Valider les données côté client avant d'envoyer

❌ **À éviter** :
- Envoyer des mots de passe en clair
- Oublier le bearer dans le token JWT
- Faire des requêtes en boucle sans pagination
- Utiliser GET pour les modifications

Pour d'autres questions, consultez la documentation technique complète.
