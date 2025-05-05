-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: premierrendupsi
-- ------------------------------------------------------
-- Server version	8.0.41

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `administrateur`
--

DROP TABLE IF EXISTS `administrateur`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `administrateur` (
  `idAdmin` int NOT NULL AUTO_INCREMENT,
  `nom` varchar(50) NOT NULL,
  `prenom` varchar(50) NOT NULL,
  `email` varchar(50) NOT NULL,
  `motDePasse` varchar(255) NOT NULL,
  PRIMARY KEY (`idAdmin`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `administrateur`
--

LOCK TABLES `administrateur` WRITE;
/*!40000 ALTER TABLE `administrateur` DISABLE KEYS */;
/*!40000 ALTER TABLE `administrateur` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `client`
--

DROP TABLE IF EXISTS `client`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `client`
--

LOCK TABLES `client` WRITE;
/*!40000 ALTER TABLE `client` DISABLE KEYS */;
INSERT INTO `client` VALUES (18,'Martinn','Jean','2 rue de la Fontaine',2,75015,'0787898789','Jean@gmail.com','Paris',0,'Argentine','Jean'),(19,'Négrié','Camille','Boulevard de Clichy',54,75900,'0651234595','camillenegrie@hotmail.com','Paris',0,'Place de Clichy','camille'),(20,'Moysan','Yann','rue des anges',4,75412,'0654859544','yann@gmail.com','Paris',0,'Pigalle','yann'),(21,'Bernard','Colin','rue de france',33,75000,'0654859655','colin@gmail.com','Paris',0,'Bercy','colin'),(22,'Michel','Boris','impasse des moineaux',22,75400,'0654859565','boris@gmail.com','Paris',0,'Tolbiac','boris'),(23,'samos','emile','rue des plantes',9,75601,'0654857495','emile@gmail.com','Paris',0,'Opéra','emile'),(24,'dupuis','marc','rue des pyrénées',45,75985,'0654859565','marc@gmail.com','Paris',0,'Cadet','marc');
/*!40000 ALTER TABLE `client` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `commande`
--

DROP TABLE IF EXISTS `commande`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `commande`
--

LOCK TABLES `commande` WRITE;
/*!40000 ALTER TABLE `commande` DISABLE KEYS */;
INSERT INTO `commande` VALUES (24,'couscous',15.00,30,'prêt','2025-04-09',18,'sans harissas ',15),(25,'couscous',15.00,30,'en attente','2025-04-09',19,'',15),(26,'makis thon',8.00,30,'en attente','2025-04-09',20,'',18),(27,'sushi saumon',8.00,30,'en attente','2025-04-09',20,'supplément sauce soja sucrée',18),(28,'burger',12.00,30,'en attente','2025-04-09',21,'sans oignons',16),(29,'pizza orientale',10.00,30,'en attente','2025-04-09',19,'',17),(30,'kebab',7.00,30,'en attente','2025-04-09',22,'sans tomates',19),(31,'pizza 4 fromages',10.00,30,'en attente','2025-04-09',23,'',17),(32,'makis thon',8.00,30,'en attente','2025-04-09',24,'',18),(33,'kebab',7.00,30,'préparé','2025-05-02',18,'sans oignons',19);
/*!40000 ALTER TABLE `commande` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cuisinier`
--

DROP TABLE IF EXISTS `cuisinier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cuisinier`
--

LOCK TABLES `cuisinier` WRITE;
/*!40000 ALTER TABLE `cuisinier` DISABLE KEYS */;
INSERT INTO `cuisinier` VALUES (15,'Bernard','Christophe','rue de la place',75018,'0678984858','christophe@gmail.com','Paris',0,'Oberkampf','christophe',1),(16,'Jean','Jacques','Rue de Rivoli',75000,'0632954645','jacques@gmail.com','Paris',0,'Auber','jacques',36),(17,'Jolie','Sandrine','rue des Tilleuls',75120,'0654859665','sandrine@gmail.com','Paris',0,'Châtelet','sandrine',22),(18,'Demousse','Jules','rue des prés*',75620,'0654857496','jules@gmail.com','Paris',0,'nation','jules',2),(19,'Maurice','Jeanne','rue des rosiers',75148,'0654859545','jeanne@gmail.com','Paris',0,'Trocadéro','jeanne',45);
/*!40000 ALTER TABLE `cuisinier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ingredient`
--

DROP TABLE IF EXISTS `ingredient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient` (
  `idIngredient` int NOT NULL AUTO_INCREMENT,
  `nom` varchar(50) NOT NULL,
  `origine` varchar(50) NOT NULL,
  `idPlat` int DEFAULT NULL,
  PRIMARY KEY (`idIngredient`)
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient`
--

LOCK TABLES `ingredient` WRITE;
/*!40000 ALTER TABLE `ingredient` DISABLE KEYS */;
INSERT INTO `ingredient` VALUES (12,'semoule','france',21),(13,'tomate','espagne',21),(14,'courgettes','france',21),(15,'steak pur boeuf ','france',22),(16,'tomates','espagne',22),(17,'pain','france',22),(18,'cheddar','france',22),(19,'salade','france',22),(20,'cornichons','france',22),(21,'moutarde','france',22),(22,'pain','france',23),(23,'saucisse','france',23),(24,'oignons','france',23),(25,'ketchup','france',23),(26,'moutarde','france',23),(27,'chèvre','france',25),(28,'mozarella','italie',25),(29,'brie','france',25),(30,'gruyère','france',25),(31,'merguez','france',26),(32,'poivrons','france',26),(33,'mozarella','italie',26),(34,'thon','france',27),(35,'avocat','brésil',27),(36,'riz','france',27),(37,'saumon','france',28),(38,'riz','france',28),(39,'agneau','france',29),(40,'salade','france',29),(41,'tomates','espagne',29),(42,'oignons','france',29),(43,'pain pita','france',29);
/*!40000 ALTER TABLE `ingredient` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plat`
--

DROP TABLE IF EXISTS `plat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plat`
--

LOCK TABLES `plat` WRITE;
/*!40000 ALTER TABLE `plat` DISABLE KEYS */;
INSERT INTO `plat` VALUES (21,'végétarien','2025-04-09','2025-04-11',15,'couscous','maghreb',15),(22,'omnivore','2025-04-09','2025-04-11',12,'burger','américain',16),(23,'omnivore','2025-04-09','2025-04-13',7,'hotdog','américain',16),(25,'végétarien','2025-04-09','2025-04-12',10,'pizza 4 fromages','italie',17),(26,'omnivore','2025-04-09','2025-04-12',10,'pizza orientale','italie',17),(27,'omnivore','2025-04-09','2025-04-11',8,'makis thon','japonais',18),(28,'omnivore','2025-04-09','2025-04-11',8,'sushi saumon','japonais',18),(29,'halal','2025-04-09','2024-04-11',7,'kebab','maghreb',19);
/*!40000 ALTER TABLE `plat` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-05-05 16:46:10
