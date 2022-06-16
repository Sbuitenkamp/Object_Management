-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 16, 2022 at 12:25 PM
-- Server version: 10.7.3-MariaDB
-- PHP Version: 8.1.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `Object_management`
--
CREATE DATABASE IF NOT EXISTS `Object_management` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `Object_management`;

-- --------------------------------------------------------

--
-- Table structure for table `Customer`
--

CREATE TABLE IF NOT EXISTS `Customer` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `telephone` int(11) NOT NULL,
  `email` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `adres` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `is_applied_to`
--

CREATE TABLE IF NOT EXISTS `is_applied_to` (
  `sale_id` int(11) NOT NULL,
  `object_type_id` int(11) NOT NULL,
  PRIMARY KEY (`sale_id`,`object_type_id`),
  KEY `is_applied_to_fk1` (`object_type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `is_reserved_at`
--

CREATE TABLE IF NOT EXISTS `is_reserved_at` (
  `object_type_id` int(11) NOT NULL,
  `reservation_number` int(11) NOT NULL,
  `amount` int(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`object_type_id`,`reservation_number`),
  KEY `is_reserved_at_fk1` (`reservation_number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Object`
--

CREATE TABLE IF NOT EXISTS `Object` (
  `object_number` int(11) NOT NULL,
  `object_type_id` int(11) NOT NULL,
  `loaned_out` tinyint(1) NOT NULL DEFAULT 0,
  `in_service` tinyint(1) NOT NULL DEFAULT 0,
  `size` varchar(3) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`object_number`),
  KEY `Object_fk0` (`object_type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `ObjectType`
--

CREATE TABLE IF NOT EXISTS `ObjectType` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `description` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `price` float NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `constraint_name` (`description`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Reservation`
--

CREATE TABLE IF NOT EXISTS `Reservation` (
  `reservation_number` int(11) NOT NULL AUTO_INCREMENT,
  `customer_id` int(11) NOT NULL,
  `start_date` date NOT NULL,
  `return_date` date NOT NULL,
  `residence` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `comment` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `payment_method` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `paid` tinyint(1) NOT NULL DEFAULT 0,
  `returned` tinyint(1) NOT NULL,
  PRIMARY KEY (`reservation_number`),
  KEY `Reservation_fk0` (`customer_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `ReservationDate`
--

CREATE TABLE IF NOT EXISTS `ReservationDate` (
  `object_number` int(11) NOT NULL,
  `day` datetime NOT NULL,
  `reservation_number` int(11) NOT NULL,
  PRIMARY KEY (`object_number`,`day`),
  KEY `reservation_number` (`reservation_number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `Sale`
--

CREATE TABLE IF NOT EXISTS `Sale` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `days_to_rent` int(11) NOT NULL,
  `days_to_pay` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `is_applied_to`
--
ALTER TABLE `is_applied_to`
  ADD CONSTRAINT `is_applied_to_fk0` FOREIGN KEY (`sale_id`) REFERENCES `Sale` (`id`),
  ADD CONSTRAINT `is_applied_to_fk1` FOREIGN KEY (`object_type_id`) REFERENCES `ObjectType` (`id`);

--
-- Constraints for table `is_reserved_at`
--
ALTER TABLE `is_reserved_at`
  ADD CONSTRAINT `is_reserved_at_fk0` FOREIGN KEY (`object_type_id`) REFERENCES `ObjectType` (`id`),
  ADD CONSTRAINT `is_reserved_at_fk1` FOREIGN KEY (`reservation_number`) REFERENCES `Reservation` (`reservation_number`);

--
-- Constraints for table `Object`
--
ALTER TABLE `Object`
  ADD CONSTRAINT `Object_fk0` FOREIGN KEY (`object_type_id`) REFERENCES `ObjectType` (`id`);

--
-- Constraints for table `Reservation`
--
ALTER TABLE `Reservation`
  ADD CONSTRAINT `Reservation_fk0` FOREIGN KEY (`customer_id`) REFERENCES `Customer` (`id`);

--
-- Constraints for table `ReservationDate`
--
ALTER TABLE `ReservationDate`
  ADD CONSTRAINT `ReservationDate_fk0` FOREIGN KEY (`object_number`) REFERENCES `Object` (`object_number`),
  ADD CONSTRAINT `ReservationDate_ibfk_1` FOREIGN KEY (`reservation_number`) REFERENCES `Reservation` (`reservation_number`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
