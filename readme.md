# EasySave

[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=MVinualez_EasySave-Project&metric=alert_status)](https://sonarcloud.io/dashboard?id=MVinualez_EasySave-Project)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=MVinualez_EasySave-Project&metric=coverage)](https://sonarcloud.io/dashboard?id=MVinualez_EasySave-Project)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=MVinualez_EasySave-Project&metric=bugs)](https://sonarcloud.io/dashboard?id=MVinualez_EasySave-Project)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=MVinualez_EasySave-Project&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=MVinualez_EasySave-Project)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=MVinualez_EasySave-Project&metric=code_smells)](https://sonarcloud.io/dashboard?id=MVinualez_EasySave-Project)

## Description

EasySave est un logiciel de sauvegarde développé par ProSoft, permettant aux utilisateurs d'effectuer des sauvegardes complètes ou différentielles de leurs fichiers. Initialement conçu comme une application console, EasySave évolue vers une interface graphique en utilisant .NET 8 et WinUI tout en adoptant l'architecture MVVM.

## Fonctionnalités principales

### Version 1.0

- **Création et gestion de jusqu'à 5 travaux de sauvegarde**
- **Types de sauvegarde supportés** :
  - Complète : Copie intégrale des fichiers
  - Différentielle : Copie uniquement des fichiers modifiés depuis la dernière sauvegarde complète
- **Support des périphériques suivants** :
  - Disques locaux
  - Disques externes
- **Exécution des sauvegardes** :
  - Individuellement
  - Séquentiellement
- **Logs en temps réel** au format JSON
- **Fichier d'état** indiquant la progression des sauvegardes
- **Support multilingue** : Anglais et Français

### Version 1.1

- **Choix du format des logs** : JSON ou XML

### Version 2.0

- **Interface graphique** (WinUI)
- **Nombre de travaux de sauvegarde illimité**
- **Intégration du cryptage via CryptoSoft** (cryptage des fichiers avec extensions définies par l'utilisateur)
- **Ajout du temps de cryptage dans les logs journaliers**
- **Détection et arrêt des sauvegardes en cas d'exécution d'un logiciel métier**

### Version 3.0

- **Sauvegarde en parallèle** (abandon du mode séquentiel)
- **Gestion des fichiers prioritaires** : priorité aux fichiers avec certaines extensions définies par l'utilisateur
- **Interdiction de transfert simultané des fichiers volumineux** (seuil paramétrable)
- **Interaction en temps réel avec les sauvegardes** : Pause, Reprise, Arrêt
- **Pause automatique des sauvegardes en cas d'exécution d'un logiciel métier, reprise automatique après arrêt du logiciel**
- **Console déportée** : suivi et gestion des sauvegardes à distance via sockets
- **Mono-instance pour CryptoSoft**
- **Réduction automatique du nombre de tâches parallèles en cas de surcharge réseau** (option activable)

## Technologies utilisées

- **Langage** : C#
- **Framework** : .NET 8
- **Interface graphique** : WinUI
- **Architecture** : MVVM
- **Fichiers journaux** : JSON, XML
- **Communication réseau** : Sockets (version 3.0)

## Installation

1. **Pré-requis** :
   - Windows 10/11
   - .NET 8 installé
   - Accès aux répertoires source et destination
2. **Téléchargement et exécution** :
   - Clonez le dépôt :
     ```sh
     git clone https://github.com/mvinualez/EasySave-Project.git
     ```
   - Compilez et lancez l'application :
     ```sh
     cd "EasySave-Project/src/EasySave - WinUI"
     dotnet build
     dotnet run
     ```

## Utilisation

L'application permet de configurer des tâches de sauvegarde, de les exécuter et de suivre leur progression en temps réel grâce aux fichiers d'état et aux logs générés.

## Fichiers générés

- **Log journalier** : Liste des actions réalisées, incluant horodatage, fichiers traités, durée de transfert et temps de cryptage.
- **Fichier d'état** : Informations en temps réel sur les tâches de sauvegarde en cours.


## Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus d’informations.

## Contact

Pour toute question ou support technique, veuillez contacter **ProSoft** ou ouvrir une issue sur GitHub.

