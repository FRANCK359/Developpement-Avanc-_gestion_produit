# Présentation du projet AdvancedDevSample

## Objectif

AdvancedDevSample est une solution logicielle destinée à **illustrer une architecture d'application professionnelle en .NET**.  
Elle sert de base pour développer, tester et déployer une API REST sécurisée, gérant les principales entités d'un système d'information commercial (clients, commandes, produits, fournisseurs, utilisateurs).

---

## Ce que fait le projet

- **Expose une API REST** pour la gestion de données métiers (CRUD sur clients, produits, commandes, fournisseurs).
- **Authentification sécurisée** : gestion des utilisateurs et authentification via JWT (JSON Web Token).
- **Gestion des erreurs et logs** : intégration de middlewares pour enregistrer les requêtes et gérer les exceptions de façon centralisée.
- **Validation automatique des données** : filtres pour valider les inputs au niveau des endpoints.
- **Persistance des données** : utilisations d'Entity Framework Core pour accéder, écrire et migrer des données en base SQL.
- **Architecture propre, modulaire** : séparation claire des couches (Domain, Application, Infrastructure, Api).
- **Documentation interactive** : Swagger pour tester et explorer l'API.
- **Tests automatisés** : tests unitaires et d'intégration pour garantir le bon fonctionnement du code métier et des endpoints.

---

## Scénarios illustrés

- Un administrateur ou opérateur peut **gérer les clients** (ajouter, modifier, supprimer, rechercher).
- Il peut également **gérer les commandes**, les produits associés à chaque commande, et suivre l’évolution du statut.
- La gestion des fournisseurs permet de rattacher différents produits à des fournisseurs.
- Les utilisateurs peuvent s’authentifier pour obtenir un jeton d’accès (JWT) et accéder à des endpoints protégés par autorisation.
- En cas d’erreur ou de mauvaise saisie, le système **renvoie des messages structurés** et logue l’événement.

---

## Utilisation type

- Intégration dans un système commercial, de gestion ou de formation à la conception de REST APIs modernes en .NET.
- Base pour projets professionnels avec mise en place de bonnes pratiques et d’un workflow DevOps (tests, CI/CD).

---

## Pourquoi utiliser ce projet ?

- Pour apprendre à **structurer un projet .NET multi-couches**.
- Pour disposer d’un starter kit orienté entreprise.
- Pour tester des techniques modernes d’authentification, de persistance et de validation dans un environnement industriel.

---
