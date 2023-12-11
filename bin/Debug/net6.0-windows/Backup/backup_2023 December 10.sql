-- MySqlBackup.NET 2.3.8.0
-- Dump Time: 2023-12-10 02:16:34
-- --------------------------------------
-- Server version 8.0.35 MySQL Community Server - GPL


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- 
-- Definition of __efmigrationshistory
-- 

DROP TABLE IF EXISTS `__efmigrationshistory`;
CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table __efmigrationshistory
-- 

/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory`(`MigrationId`,`ProductVersion`) VALUES('20231208173057_Initial','7.0.7');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;

-- 
-- Definition of carrier
-- 

DROP TABLE IF EXISTS `carrier`;
CREATE TABLE IF NOT EXISTS `carrier` (
  `CarrierId` int NOT NULL AUTO_INCREMENT,
  `CompanyName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DepotCity` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Ftla` int NOT NULL,
  `Ltla` int NOT NULL,
  `FtlRate` double NOT NULL,
  `LtlRate` double NOT NULL,
  `ReefCharge` double NOT NULL,
  PRIMARY KEY (`CarrierId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table carrier
-- 

/*!40000 ALTER TABLE `carrier` DISABLE KEYS */;

/*!40000 ALTER TABLE `carrier` ENABLE KEYS */;

-- 
-- Definition of city
-- 

DROP TABLE IF EXISTS `city`;
CREATE TABLE IF NOT EXISTS `city` (
  `CityId` int NOT NULL AUTO_INCREMENT,
  `CityName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`CityId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table city
-- 

/*!40000 ALTER TABLE `city` DISABLE KEYS */;

/*!40000 ALTER TABLE `city` ENABLE KEYS */;

-- 
-- Definition of rate
-- 

DROP TABLE IF EXISTS `rate`;
CREATE TABLE IF NOT EXISTS `rate` (
  `RateId` int NOT NULL AUTO_INCREMENT,
  `RateType` int NOT NULL,
  `Amount` decimal(65,30) NOT NULL,
  PRIMARY KEY (`RateId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table rate
-- 

/*!40000 ALTER TABLE `rate` DISABLE KEYS */;

/*!40000 ALTER TABLE `rate` ENABLE KEYS */;

-- 
-- Definition of invoice
-- 

DROP TABLE IF EXISTS `invoice`;
CREATE TABLE IF NOT EXISTS `invoice` (
  `InvoiceId` int NOT NULL AUTO_INCREMENT,
  `OrderId` int NOT NULL,
  `RateId` int NOT NULL,
  `Quantity` int NOT NULL,
  `Amount` decimal(65,30) NOT NULL,
  `InvoiceDate` datetime(6) NOT NULL,
  `CustomerId` int NOT NULL,
  PRIMARY KEY (`InvoiceId`),
  UNIQUE KEY `IX_Invoice_OrderId` (`OrderId`),
  KEY `IX_Invoice_CustomerId` (`CustomerId`),
  KEY `IX_Invoice_RateId` (`RateId`),
  CONSTRAINT `FK_Invoice_Customer_CustomerId` FOREIGN KEY (`CustomerId`) REFERENCES `customer` (`CustomerId`) ON DELETE CASCADE,
  CONSTRAINT `FK_Invoice_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`OrderId`) ON DELETE CASCADE,
  CONSTRAINT `FK_Invoice_Rate_RateId` FOREIGN KEY (`RateId`) REFERENCES `rate` (`RateId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table invoice
-- 

/*!40000 ALTER TABLE `invoice` DISABLE KEYS */;

/*!40000 ALTER TABLE `invoice` ENABLE KEYS */;

-- 
-- Definition of route
-- 

DROP TABLE IF EXISTS `route`;
CREATE TABLE IF NOT EXISTS `route` (
  `RouteId` int NOT NULL AUTO_INCREMENT,
  `SourceCityId` int NOT NULL,
  `DestinationCityId` int NOT NULL,
  `Distance` decimal(65,30) NOT NULL,
  `Duration` decimal(65,30) NOT NULL,
  PRIMARY KEY (`RouteId`),
  KEY `IX_Route_DestinationCityId` (`DestinationCityId`),
  KEY `IX_Route_SourceCityId` (`SourceCityId`),
  CONSTRAINT `FK_Route_City_DestinationCityId` FOREIGN KEY (`DestinationCityId`) REFERENCES `city` (`CityId`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Route_City_SourceCityId` FOREIGN KEY (`SourceCityId`) REFERENCES `city` (`CityId`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table route
-- 

/*!40000 ALTER TABLE `route` DISABLE KEYS */;

/*!40000 ALTER TABLE `route` ENABLE KEYS */;

-- 
-- Definition of user
-- 

DROP TABLE IF EXISTS `user`;
CREATE TABLE IF NOT EXISTS `user` (
  `UserId` int NOT NULL AUTO_INCREMENT,
  `Username` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Password` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserType` int NOT NULL,
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table user
-- 

/*!40000 ALTER TABLE `user` DISABLE KEYS */;

/*!40000 ALTER TABLE `user` ENABLE KEYS */;

-- 
-- Definition of customer
-- 

DROP TABLE IF EXISTS `customer`;
CREATE TABLE IF NOT EXISTS `customer` (
  `CustomerId` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`CustomerId`),
  KEY `IX_Customer_UserId` (`UserId`),
  CONSTRAINT `FK_Customer_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table customer
-- 

/*!40000 ALTER TABLE `customer` DISABLE KEYS */;

/*!40000 ALTER TABLE `customer` ENABLE KEYS */;

-- 
-- Definition of logfile
-- 

DROP TABLE IF EXISTS `logfile`;
CREATE TABLE IF NOT EXISTS `logfile` (
  `LogId` int NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL,
  `LogDetails` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LogTimeStamp` datetime(6) NOT NULL,
  PRIMARY KEY (`LogId`),
  KEY `IX_LogFile_UserId` (`UserId`),
  CONSTRAINT `FK_LogFile_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table logfile
-- 

/*!40000 ALTER TABLE `logfile` DISABLE KEYS */;

/*!40000 ALTER TABLE `logfile` ENABLE KEYS */;

-- 
-- Definition of orders
-- 

DROP TABLE IF EXISTS `orders`;
CREATE TABLE IF NOT EXISTS `orders` (
  `OrderId` int NOT NULL AUTO_INCREMENT,
  `CustomerId` int NOT NULL,
  `OrderStatus` int NOT NULL,
  `DateInitiated` datetime(6) NOT NULL,
  `DateCompleted` datetime(6) DEFAULT NULL,
  `SourceCityId` int NOT NULL,
  `DestinationCityId` int NOT NULL,
  `UserId` int DEFAULT NULL,
  `JobType` int DEFAULT NULL,
  PRIMARY KEY (`OrderId`),
  KEY `IX_Orders_CustomerId` (`CustomerId`),
  KEY `IX_Orders_DestinationCityId` (`DestinationCityId`),
  KEY `IX_Orders_SourceCityId` (`SourceCityId`),
  KEY `IX_Orders_UserId` (`UserId`),
  CONSTRAINT `FK_Orders_City_DestinationCityId` FOREIGN KEY (`DestinationCityId`) REFERENCES `city` (`CityId`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Orders_City_SourceCityId` FOREIGN KEY (`SourceCityId`) REFERENCES `city` (`CityId`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Orders_Customer_CustomerId` FOREIGN KEY (`CustomerId`) REFERENCES `customer` (`CustomerId`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Orders_User_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table orders
-- 

/*!40000 ALTER TABLE `orders` DISABLE KEYS */;

/*!40000 ALTER TABLE `orders` ENABLE KEYS */;

-- 
-- Definition of trip
-- 

DROP TABLE IF EXISTS `trip`;
CREATE TABLE IF NOT EXISTS `trip` (
  `TripId` int NOT NULL AUTO_INCREMENT,
  `OrderId` int NOT NULL,
  `CarrierId` int NOT NULL,
  `TripStatus` int NOT NULL,
  PRIMARY KEY (`TripId`),
  KEY `IX_Trip_CarrierId` (`CarrierId`),
  KEY `IX_Trip_OrderId` (`OrderId`),
  CONSTRAINT `FK_Trip_Carrier_CarrierId` FOREIGN KEY (`CarrierId`) REFERENCES `carrier` (`CarrierId`) ON DELETE CASCADE,
  CONSTRAINT `FK_Trip_Orders_OrderId` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`OrderId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table trip
-- 

/*!40000 ALTER TABLE `trip` DISABLE KEYS */;

/*!40000 ALTER TABLE `trip` ENABLE KEYS */;


/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;


-- Dump completed on 2023-12-10 02:16:34
-- Total time: 0:0:0:0:299 (d:h:m:s:ms)
