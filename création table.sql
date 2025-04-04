-- Ceci est la dernière version de notre création de table, celle du rendu n°2
create database premierRenduPSI;
use premierRenduPSI;

CREATE TABLE `client` (
   `idClient` int NOT NULL AUTO_INCREMENT,
   `nom` varchar(50) NOT NULL,
   `prenom` varchar(50) NOT NULL,
   `rue` varchar(50) NOT NULL,
   `numMaison` int NOT NULL,
   `codePostal` int NOT NULL,
   `numTel` varchar(50) DEFAULT NULL,
   `email` varchar(50) NOT NULL,
   `ville` varchar(50) NOT NULL,
   `totalCommande` int NOT NULL,
   `metroProche` varchar(50) NOT NULL,
   `motDePasse` varchar(255) NOT NULL,
   PRIMARY KEY (`idClient`)
 ) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
 CREATE TABLE `commande` (
   `idCommande` int NOT NULL AUTO_INCREMENT,
   `nom` varchar(50) NOT NULL,
   `prix` decimal(10,2) DEFAULT NULL,
   `tempsPreparation` int NOT NULL,
   `statut` varchar(50) NOT NULL,
   `date` date NOT NULL,
   `idClient` int NOT NULL,
   `commentaire` varchar(250) DEFAULT NULL,
   `idCuisinier` int DEFAULT NULL,
   PRIMARY KEY (`idCommande`),
   KEY `Commande_Client_FK` (`idClient`),
   CONSTRAINT `Commande_Client_FK` FOREIGN KEY (`idClient`) REFERENCES `client` (`idClient`)
 ) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
 CREATE TABLE `cuisinier` (
   `idCuisinier` int NOT NULL AUTO_INCREMENT,
   `nom` varchar(50) NOT NULL,
   `prenom` varchar(50) NOT NULL,
   `rue` varchar(50) NOT NULL,
   `codePostal` int NOT NULL,
   `numTel` varchar(50) DEFAULT NULL,
   `email` varchar(50) NOT NULL,
   `ville` varchar(50) NOT NULL,
   `totalCommande` int NOT NULL,
   `metroProche` varchar(50) NOT NULL,
   `motDePasse` varchar(255) NOT NULL,
   `numMaison` int DEFAULT NULL,
   PRIMARY KEY (`idCuisinier`)
 ) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
 CREATE TABLE `ingredient` (
   `idIngredient` int NOT NULL AUTO_INCREMENT,
   `nom` varchar(50) NOT NULL,
   `origine` varchar(50) NOT NULL,
   `idPlat` int DEFAULT NULL,
   PRIMARY KEY (`idIngredient`)
 ) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
 
 CREATE TABLE `plat` (
   `idPlat` int NOT NULL AUTO_INCREMENT,
   `regime` varchar(50) NOT NULL,
   `dateFabrication` date NOT NULL,
   `datePeremption` date NOT NULL,
   `prix` float NOT NULL,
   `nomPlat` varchar(50) NOT NULL,
   `nationalite` varchar(50) NOT NULL,
   `idCuisinier` int DEFAULT NULL,
   PRIMARY KEY (`idPlat`)
 ) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
