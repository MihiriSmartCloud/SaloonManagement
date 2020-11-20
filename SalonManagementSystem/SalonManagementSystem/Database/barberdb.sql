-- phpMyAdmin SQL Dump
-- version 4.7.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 13, 2020 at 07:29 AM
-- Server version: 10.1.25-MariaDB
-- PHP Version: 7.1.7

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `barberdb`
--

-- --------------------------------------------------------

--
-- Table structure for table `tblappointment`
--

CREATE TABLE `tblappointment` (
  `appointment_id` int(11) NOT NULL,
  `appointment_no_for_day` int(11) NOT NULL,
  `salon_id` int(11) NOT NULL,
  `due_date` date NOT NULL,
  `start_time` time NOT NULL,
  `expected_end_time` time NOT NULL,
  `end_time` time DEFAULT NULL,
  `barber_Id` int(11) NOT NULL,
  `customer_id` int(11) NOT NULL,
  `canceled` tinyint(1) NOT NULL,
  `status` varchar(15) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `tblappointment`
--

INSERT INTO `tblappointment` (`appointment_id`, `appointment_no_for_day`, `salon_id`, `due_date`, `start_time`, `expected_end_time`, `end_time`, `barber_Id`, `customer_id`, `canceled`, `status`) VALUES
(17, 1, 2, '2020-11-01', '12:30:00', '14:00:00', '14:44:06', 5, 13, 0, 'COMPLETED'),
(20, 2, 2, '2020-11-01', '15:00:00', '16:30:00', '16:02:07', 5, 10, 0, 'COMPLETED');

-- --------------------------------------------------------

--
-- Table structure for table `tblbarber`
--

CREATE TABLE `tblbarber` (
  `barber_id` int(11) NOT NULL,
  `barber_name` varchar(45) NOT NULL,
  `salon_id` int(11) NOT NULL,
  `allocated_seat_no` int(11) NOT NULL,
  `is_available` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `tblbarber`
--

INSERT INTO `tblbarber` (`barber_id`, `barber_name`, `salon_id`, `allocated_seat_no`, `is_available`) VALUES
(1, 'Bob', 1, 10, 1),
(3, 'Jack', 1, 2, 1),
(5, 'BOB', 2, 3, 1),
(6, 'Sam', 1, 5, 0),
(7, 'Rom', 2, 1, 1),
(10, 'Jackie', 4, 1, 1);

-- --------------------------------------------------------

--
-- Table structure for table `tblbarber_service`
--

CREATE TABLE `tblbarber_service` (
  `barber_service_id` int(11) NOT NULL,
  `barber_id` int(11) NOT NULL,
  `service_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `tblbarber_service`
--

INSERT INTO `tblbarber_service` (`barber_service_id`, `barber_id`, `service_id`) VALUES
(1, 1, 1),
(2, 1, 2),
(3, 3, 1),
(5, 3, 2),
(7, 5, 4),
(8, 7, 3),
(9, 5, 3),
(13, 10, 5);

-- --------------------------------------------------------

--
-- Table structure for table `tblcurrent_appointments`
--

CREATE TABLE `tblcurrent_appointments` (
  `current_appointment_id` int(11) NOT NULL,
  `appointment_id` int(11) NOT NULL,
  `salon_id` int(11) NOT NULL,
  `barber_id` int(11) NOT NULL,
  `current_date` date NOT NULL,
  `last_appointment_no` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `tblcurrent_appointments`
--

INSERT INTO `tblcurrent_appointments` (`current_appointment_id`, `appointment_id`, `salon_id`, `barber_id`, `current_date`, `last_appointment_no`) VALUES
(1, 20, 2, 5, '2020-11-01', 2);

-- --------------------------------------------------------

--
-- Table structure for table `tblcustomer`
--

CREATE TABLE `tblcustomer` (
  `customer_id` int(11) NOT NULL,
  `name` varchar(20) NOT NULL,
  `mobile_no` int(11) NOT NULL,
  `salon_id` int(11) NOT NULL,
  `login_time` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `tblcustomer`
--

INSERT INTO `tblcustomer` (`customer_id`, `name`, `mobile_no`, `salon_id`, `login_time`) VALUES
(10, 'Sanushi', 767567755, 2, '2020-10-28 15:19:38'),
(11, 'Romela Salgado', 767567778, 2, '2020-10-28 15:20:06'),
(13, 'sds', 767567755, 2, '2020-10-28 17:52:33'),
(14, 'Spot Delgado', 767567755, 4, '2020-11-10 19:37:41');

-- --------------------------------------------------------

--
-- Table structure for table `tblinvoice`
--

CREATE TABLE `tblinvoice` (
  `invoice_id` int(11) NOT NULL,
  `salon_id` int(11) NOT NULL,
  `appointment_id` int(11) NOT NULL,
  `total_price` decimal(10,2) NOT NULL,
  `discount` decimal(5,2) NOT NULL DEFAULT '0.00',
  `final_price` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `tblinvoice`
--

INSERT INTO `tblinvoice` (`invoice_id`, `salon_id`, `appointment_id`, `total_price`, `discount`, `final_price`) VALUES
(6, 2, 17, '800.00', '300.00', '500.00'),
(7, 2, 20, '1300.00', '100.00', '1200.00');

-- --------------------------------------------------------

--
-- Table structure for table `tbllog`
--

CREATE TABLE `tbllog` (
  `log_id` int(11) NOT NULL,
  `ref_table` varchar(30) NOT NULL,
  `ref_id` varchar(10) NOT NULL,
  `updated_date_time` datetime NOT NULL,
  `action_type` varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `tbllog`
--

INSERT INTO `tbllog` (`log_id`, `ref_table`, `ref_id`, `updated_date_time`, `action_type`) VALUES
(15, 'tblcustomer', '2', '2020-10-24 21:19:35', 'UPDATE'),
(16, 'tblcustomer', '2', '2020-10-24 21:19:59', 'UPDATE'),
(17, 'tblcustomer', '8', '2020-10-24 21:21:26', 'INSERT'),
(18, 'tblcustomer', '2', '2020-10-24 21:21:59', 'UPDATE'),
(19, 'tblcustomer', '2', '2020-10-24 21:25:04', 'UPDATE'),
(20, 'tblcustomer', '2', '2020-10-24 21:25:07', 'UPDATE'),
(21, 'tblcustomer', '2', '2020-10-24 21:25:08', 'UPDATE'),
(22, 'tblcustomer', '8', '2020-10-24 21:25:55', 'DELETE'),
(23, 'tblcustomer', '9', '2020-10-24 21:31:15', 'INSERT'),
(24, 'tblcustomer', '2', '2020-10-24 21:32:58', 'UPDATE'),
(25, 'tblcustomer', '2', '2020-10-24 21:33:19', 'UPDATE'),
(26, 'tblservice', '1', '2020-10-24 22:50:32', 'INSERT'),
(27, 'tblservice', '2', '2020-10-24 22:55:08', 'INSERT'),
(28, 'tblservice', '3', '2020-10-24 22:56:48', 'INSERT'),
(29, 'tblservice', '3', '2020-10-24 23:30:40', 'UPDATE'),
(30, 'tblservice', '3', '2020-10-24 23:32:53', 'UPDATE'),
(31, 'tblservice', '3', '2020-10-24 23:36:41', 'UPDATE'),
(32, 'tblservice', '3', '2020-10-24 23:43:15', 'DELETE'),
(33, 'tblbarber', '1', '2020-10-25 11:55:01', 'INSERT'),
(34, 'tblbarber', '2', '2020-10-25 12:02:41', 'INSERT'),
(35, 'tblbarber', '3', '2020-10-25 12:04:10', 'INSERT'),
(36, 'tblbarber', '4', '2020-10-25 12:24:02', 'INSERT'),
(37, 'tblbarber', '5', '2020-10-25 12:27:18', 'INSERT'),
(38, 'tblbarber', '6', '2020-10-25 12:57:00', 'INSERT'),
(39, 'tblbarber', '6', '2020-10-25 13:00:20', 'UPDATE'),
(40, 'tblbarber', '2', '2020-10-25 13:09:57', 'DELETE'),
(41, 'tblbarber', '1', '2020-10-25 13:24:28', 'INSERT'),
(42, 'tblbarber', '2', '2020-10-25 13:25:46', 'INSERT'),
(43, 'tblbarber', '3', '2020-10-25 13:26:30', 'INSERT'),
(44, 'tblbarber', '4', '2020-10-25 13:27:11', 'INSERT'),
(45, 'tblbarber', '4', '2020-10-25 13:27:38', 'DELETE'),
(46, 'tblappointment', '2', '2020-10-25 22:47:44', 'DELETE'),
(47, 'tblservice', '3', '2020-10-26 00:54:41', 'INSERT'),
(48, 'tblappointment', '-1', '2020-10-26 14:54:22', 'INSERT'),
(49, 'tblappointment', '1', '2020-10-26 15:07:38', 'INSERT'),
(50, 'tblappointment', '2', '2020-10-26 15:09:51', 'INSERT'),
(51, 'tblbarber', '4', '2020-10-26 15:55:34', 'INSERT'),
(52, 'tblappointment', '1', '2020-10-26 15:55:56', 'INSERT'),
(53, 'tblappointment', '1', '2020-10-26 18:30:02', 'INSERT'),
(54, 'tblappointment', '2', '2020-10-26 18:34:25', 'INSERT'),
(55, 'tblappointment', '3', '2020-10-26 18:34:53', 'INSERT'),
(56, 'tblappointment', '4', '2020-10-26 18:35:42', 'INSERT'),
(57, 'tblservice', '3', '2020-10-26 19:19:07', 'UPDATE'),
(58, 'tblservice', '1', '2020-10-26 19:20:09', 'UPDATE'),
(59, 'tblcustomer', '10', '2020-10-26 20:02:36', 'INSERT'),
(60, 'tblcustomer', '2', '2020-10-26 20:03:18', 'UPDATE'),
(61, 'tblcustomer', '10', '2020-10-26 20:03:38', 'DELETE'),
(62, 'tblsalon', '3', '2020-10-26 20:05:23', 'INSERT'),
(63, 'tblbarber', '4', '2020-10-27 05:35:57', 'UPDATE'),
(64, 'tblbarber', '4', '2020-10-27 05:36:42', 'UPDATE'),
(65, 'tblbarber', '5', '2020-10-27 05:39:17', 'UPDATE'),
(66, 'tblappointment', '4', '2020-10-27 06:34:04', 'UPDATE'),
(67, 'tblappointment', '4', '2020-10-27 06:41:26', 'UPDATE'),
(68, 'tblappointment', '5', '2020-10-27 08:46:48', 'INSERT'),
(69, 'tblappointment', '6', '2020-10-27 08:48:52', 'INSERT'),
(70, 'tblappointment', '7', '2020-10-27 08:51:25', 'INSERT'),
(71, 'tblappointment', '8', '2020-10-27 08:51:50', 'INSERT'),
(72, 'tblappointment', '9', '2020-10-27 08:53:51', 'INSERT'),
(73, 'tblappointment', '10', '2020-10-27 08:54:34', 'INSERT'),
(74, 'tblappointment', '11', '2020-10-27 08:55:21', 'INSERT'),
(75, 'tblappointment', '12', '2020-10-27 08:56:17', 'INSERT'),
(76, 'tblappointment', '13', '2020-10-27 08:59:34', 'INSERT'),
(77, 'tblappointment', '14', '2020-10-27 09:00:26', 'INSERT'),
(78, 'tblshop_owner', '2', '2020-10-27 13:44:18', 'INSERT'),
(79, 'tblshop_owner', '3', '2020-10-27 13:45:06', 'INSERT'),
(80, 'tblshop_owner', '1', '2020-10-27 13:47:26', 'DELETE'),
(81, 'tblbarber', '2', '2020-10-27 13:51:43', 'UPDATE'),
(82, 'tblbarber', '2', '2020-10-27 13:56:00', 'UPDATE'),
(83, 'tblbarber', '2', '2020-10-27 13:56:17', 'UPDATE'),
(84, 'tblbarber', '2', '2020-10-27 13:56:36', 'UPDATE'),
(85, 'tblbarber', '2', '2020-10-27 13:56:58', 'UPDATE'),
(86, 'tblshop_owner', '3', '2020-10-27 14:18:47', 'LOGIN'),
(87, 'tblshop_owner', '3', '2020-10-27 14:21:11', 'LOGIN'),
(88, 'tblcurrent_appointments', '9', '2020-10-28 10:56:55', 'INSERT'),
(89, 'tblappointment', '9', '2020-10-28 10:56:56', 'UPDATE'),
(90, 'tblcurrent_appointments', '9', '2020-10-28 10:58:02', 'INSERT'),
(91, 'tblappointment', '9', '2020-10-28 10:58:54', 'UPDATE'),
(92, 'tblcurrent_appointments', '9', '2020-10-28 11:00:12', 'INSERT'),
(93, 'tblcurrent_appointments', '9', '2020-10-28 11:01:11', 'INSERT'),
(94, 'tblcurrent_appointments', '9', '2020-10-28 11:15:03', 'INSERT'),
(95, 'tblcurrent_appointments', '8', '2020-10-28 11:50:05', 'INSERT'),
(96, 'tblcurrent_appointments', '8', '2020-10-28 11:54:26', 'UPDATE'),
(97, 'tblcurrent_appointments', '9', '2020-10-28 11:57:35', 'INSERT'),
(98, 'tblcurrent_appointments', '9', '2020-10-28 11:58:16', 'UPDATE'),
(99, 'tblinvoice', '1', '2020-10-28 12:30:09', 'INSERT'),
(100, 'tblinvoice', '1', '2020-10-28 12:31:25', 'DELETE'),
(101, 'tblinvoice', '2', '2020-10-28 12:31:51', 'INSERT'),
(102, 'tblshop_owner', '4', '2020-10-28 14:46:25', 'INSERT'),
(103, 'tblbarber', '4', '2020-10-28 14:47:49', 'UPDATE'),
(104, 'tblbarber', '4', '2020-10-28 14:47:58', 'UPDATE'),
(105, 'tblshop_owner', '4', '2020-10-28 14:48:26', 'DELETE'),
(106, 'tblsalon', '3', '2020-10-28 14:52:29', 'DELETE'),
(107, 'tblbarber', '2', '2020-10-28 14:53:15', 'UPDATE'),
(108, 'tblsalon', '4', '2020-10-28 14:57:14', 'INSERT'),
(109, 'tblsalon', '4', '2020-10-28 14:59:45', 'UPDATE'),
(110, 'tblsalon', '4', '2020-10-28 15:00:02', 'UPDATE'),
(111, 'tblsalon', '4', '2020-10-28 15:05:22', 'DELETE'),
(112, 'tblcustomer', '10', '2020-10-28 15:19:38', 'INSERT'),
(113, 'tblcustomer', '11', '2020-10-28 15:20:06', 'INSERT'),
(114, 'tblshop_owner', '5', '2020-10-28 17:09:27', 'INSERT'),
(115, 'tblcustomer', '12', '2020-10-28 17:21:41', 'INSERT'),
(116, 'tblcustomer', '13', '2020-10-28 17:52:33', 'INSERT'),
(117, 'tblcustomer', '11', '2020-10-28 18:00:09', 'UPDATE'),
(118, 'tblcustomer', '11', '2020-10-28 18:00:25', 'UPDATE'),
(119, 'tblcustomer', '11', '2020-10-28 18:00:49', 'UPDATE'),
(120, 'tblcustomer', '12', '2020-10-28 18:01:14', 'DELETE'),
(121, 'tblservice', '4', '2020-10-28 18:12:10', 'INSERT'),
(122, 'tblservice', '5', '2020-10-28 18:12:25', 'INSERT'),
(123, 'tblservice', '5', '2020-10-28 18:12:39', 'DELETE'),
(124, 'tblservice', '4', '2020-10-28 18:13:29', 'UPDATE'),
(125, 'tblservice', '4', '2020-10-28 18:15:21', 'UPDATE'),
(126, 'tblappointment', '3', '2020-10-28 19:25:54', 'INSERT'),
(127, 'tblappointment', '4', '2020-10-28 19:32:24', 'INSERT'),
(128, 'tblappointment', '5', '2020-10-28 19:51:34', 'INSERT'),
(129, 'tblcurrent_appointments', '2', '2020-10-28 20:02:04', 'INSERT'),
(130, 'tblcurrent_appointments', '2', '2020-10-28 20:03:07', 'UPDATE'),
(131, 'tblbarber', '8', '2020-10-28 20:29:45', 'INSERT'),
(132, 'tblbarber', '9', '2020-10-28 20:30:27', 'INSERT'),
(133, 'tblbarber', '9', '2020-10-28 20:33:15', 'UPDATE'),
(134, 'tblbarber', '9', '2020-10-28 20:34:44', 'DELETE'),
(135, 'tblbarber', '9', '2020-10-28 21:00:46', 'INSERT'),
(136, 'tblbarber', '10', '2020-10-28 21:02:11', 'INSERT'),
(137, 'tblbarber', '10', '2020-10-28 21:03:48', 'DELETE'),
(138, 'tblbarber', '11', '2020-10-28 21:05:59', 'INSERT'),
(139, 'tblinvoice', '1', '2020-10-28 21:51:46', 'INSERT'),
(140, 'tblinvoice', '1', '2020-10-28 22:16:35', 'UPDATE'),
(141, 'tblinvoice', '1', '2020-10-28 22:17:14', 'UPDATE'),
(142, 'tblinvoice', '1', '2020-10-28 22:17:38', 'UPDATE'),
(143, 'tblinvoice', '1', '2020-10-28 22:34:55', 'UPDATE'),
(144, 'tblinvoice', '1', '2020-10-28 22:35:31', 'DELETE'),
(145, 'tblappointment', '6', '2020-10-28 22:48:10', 'INSERT'),
(146, 'tblappointment', '7', '2020-10-28 22:53:46', 'INSERT'),
(147, 'tblappointment', '8', '2020-10-28 23:05:22', 'INSERT'),
(148, 'tblappointment', '9', '2020-10-28 23:33:57', 'INSERT'),
(149, 'tblappointment', '10', '2020-10-28 23:38:07', 'INSERT'),
(150, 'tblappointment', '11', '2020-10-28 23:41:25', 'INSERT'),
(151, 'tblappointment', '11', '2020-10-29 00:31:02', 'UPDATE'),
(152, 'tblappointment', '10', '2020-10-29 00:34:32', 'UPDATE'),
(153, 'tblappointment', '11', '2020-10-29 00:41:13', 'UPDATE'),
(154, 'tblappointment', '9', '2020-10-29 00:45:22', 'UPDATE'),
(155, 'tblappointment', '9', '2020-10-29 01:00:48', 'UPDATE'),
(156, 'tblcurrent_appointments', '4', '2020-10-29 06:19:44', 'INSERT'),
(157, 'tblcurrent_appointments', '4', '2020-10-29 06:20:40', 'UPDATE'),
(158, 'tblcurrent_appointments', '4', '2020-10-29 06:22:06', 'UPDATE'),
(159, 'tblappointment', '9', '2020-10-29 06:52:21', 'UPDATE'),
(160, 'tblappointment', '9', '2020-10-29 06:56:27', 'UPDATE'),
(161, 'tblappointment', '9', '2020-10-29 07:29:39', 'UPDATE'),
(162, 'tblappointment', '9', '2020-10-29 07:34:24', 'UPDATE'),
(163, 'tblappointment', '3', '2020-10-29 07:45:13', 'UPDATE'),
(164, 'tblappointment', '3', '2020-10-29 07:45:13', 'UPDATE'),
(165, 'tblappointment', '9', '2020-10-29 07:46:32', 'UPDATE'),
(166, 'tblappointment', '11', '2020-10-29 07:50:09', 'UPDATE'),
(167, 'tblappointment', '11', '2020-10-29 08:04:22', 'UPDATE'),
(168, 'tblappointment', '3', '2020-10-29 08:35:30', 'UPDATE'),
(169, 'tblappointment', '11', '2020-10-29 08:50:46', 'UPDATE'),
(170, 'tblappointment', '3', '2020-10-29 08:55:50', 'UPDATE'),
(171, 'tblappointment', '3', '2020-10-29 08:56:26', 'UPDATE'),
(172, 'tblcurrent_appointments', '7', '2020-10-29 13:08:11', 'INSERT'),
(173, 'tblcurrent_appointments', '8', '2020-10-29 13:12:18', 'INSERT'),
(174, 'tblcurrent_appointments', '1', '2020-10-29 13:23:17', 'INSERT'),
(175, 'tblsalon', '4', '2020-10-29 15:48:08', 'INSERT'),
(176, 'tblbarber', '8', '2020-10-29 15:51:40', 'INSERT'),
(177, 'tblbarber', '12', '2020-10-29 15:52:43', 'INSERT'),
(178, 'tblappointment', '12', '2020-10-29 15:53:00', 'INSERT'),
(179, 'tblsalon', '2', '2020-10-30 13:48:29', 'UPDATE'),
(180, 'tblappointment', '12', '2020-10-30 13:52:26', 'INSERT'),
(181, 'tblservice', '4', '2020-10-30 14:08:51', 'UPDATE'),
(182, 'tblservice', '4', '2020-10-30 14:15:47', 'UPDATE'),
(183, 'tblservice', '4', '2020-10-30 15:05:22', 'UPDATE'),
(184, 'tblservice', '3', '2020-10-30 15:12:03', 'UPDATE'),
(185, 'tblappointment', '13', '2020-10-30 15:17:18', 'INSERT'),
(186, 'tblappointment', '14', '2020-10-30 16:40:06', 'INSERT'),
(187, 'tblappointment', '15', '2020-10-30 19:25:31', 'INSERT'),
(188, 'tblappointment', '16', '2020-10-30 19:28:04', 'INSERT'),
(189, 'tblappointment', '17', '2020-10-30 19:30:48', 'INSERT'),
(190, 'tblshop_owner', '5', '2020-10-30 23:27:09', 'UPDATE'),
(191, 'tblshop_owner', '5', '2020-10-30 23:27:35', 'LOGIN'),
(192, 'tblshop_owner', '5', '2020-10-30 23:28:00', 'LOGIN'),
(193, 'tblshop_owner', '2', '2020-10-30 23:28:21', 'LOGIN'),
(194, 'tblshop_owner', '5', '2020-10-30 23:54:09', 'UPDATE'),
(195, 'tblshop_owner', '5', '2020-10-30 23:54:41', 'UPDATE'),
(196, 'tblshop_owner', '5', '2020-10-30 23:55:09', 'LOGIN'),
(197, 'tblshop_owner', '5', '2020-10-31 11:19:40', 'UPDATE'),
(198, 'tblshop_owner', '5', '2020-10-31 11:20:01', 'LOGIN'),
(199, 'tblbarber_service', '12', '2020-10-31 13:59:24', 'DELETE'),
(200, 'tblservice_booked', '1', '2020-10-31 15:03:04', 'INSERT'),
(201, 'tblservice_booked', '2', '2020-10-31 15:05:41', 'INSERT'),
(202, 'tblservice_booked', '2', '2020-10-31 15:09:54', 'DELETE'),
(203, 'tblservice_booked', '4', '2020-10-31 15:11:21', 'INSERT'),
(204, 'tblservice_booked', '1', '2020-10-31 15:11:53', 'DELETE'),
(205, 'tblservice_booked', '5', '2020-10-31 15:13:21', 'INSERT'),
(206, 'tblappointment', '15', '2020-11-01 10:21:30', 'INSERT'),
(207, 'tblservice_booked', '6', '2020-11-01 10:22:00', 'INSERT'),
(208, 'tblappointment', '16', '2020-11-01 10:24:48', 'INSERT'),
(209, 'tblservice_booked', '7', '2020-11-01 10:24:49', 'INSERT'),
(210, 'tblappointment', '17', '2020-11-01 10:34:07', 'INSERT'),
(211, 'tblservice_booked', '8', '2020-11-01 10:34:17', 'INSERT'),
(212, 'tblservice_booked', '9', '2020-11-01 10:34:20', 'INSERT'),
(213, 'tblappointment', '18', '2020-11-01 11:18:20', 'INSERT'),
(214, 'tblservice_booked', '10', '2020-11-01 11:18:20', 'INSERT'),
(215, 'tblappointment', '17', '2020-11-01 12:14:19', 'UPDATE'),
(216, 'tblservice_booked', '8', '2020-11-01 12:14:46', 'DELETE'),
(217, 'tblservice_booked', '9', '2020-11-01 12:14:53', 'DELETE'),
(218, 'tblservice_booked', '11', '2020-11-01 12:15:25', 'UPDATE'),
(219, 'tblappointment', '17', '2020-11-01 12:17:06', 'UPDATE'),
(220, 'tblservice_booked', '11', '2020-11-01 12:17:07', 'DELETE'),
(221, 'tblservice_booked', '12', '2020-11-01 12:17:08', 'UPDATE'),
(222, 'tblservice_booked', '13', '2020-11-01 12:17:09', 'UPDATE'),
(223, 'tblappointment', '17', '2020-11-01 12:26:45', 'UPDATE'),
(224, 'tblservice_booked', '12', '2020-11-01 12:28:14', 'DELETE'),
(225, 'tblservice_booked', '13', '2020-11-01 12:28:15', 'DELETE'),
(226, 'tblservice_booked', '14', '2020-11-01 12:28:28', 'UPDATE'),
(227, 'tblappointment', '17', '2020-11-01 13:25:11', 'UPDATE'),
(228, 'tblservice_booked', '14', '2020-11-01 13:25:11', 'DELETE'),
(229, 'tblservice_booked', '15', '2020-11-01 13:25:11', 'UPDATE'),
(230, 'tblappointment', '19', '2020-11-01 13:27:48', 'INSERT'),
(231, 'tblservice_booked', '16', '2020-11-01 13:27:48', 'INSERT'),
(232, 'tblappointment', '20', '2020-11-01 13:29:33', 'INSERT'),
(233, 'tblservice_booked', '17', '2020-11-01 13:29:33', 'INSERT'),
(234, 'tblappointment', '17', '2020-11-01 13:45:08', 'UPDATE'),
(235, 'tblappointment', '17', '2020-11-01 13:52:12', 'UPDATE'),
(236, 'tblbarber', '5', '2020-11-01 13:52:13', 'UPDATE'),
(237, 'tblappointment', '17', '2020-11-01 14:43:15', 'UPDATE'),
(238, 'tblbarber', '5', '2020-11-01 14:43:15', 'UPDATE'),
(239, 'tblcurrent_appointments', '1', '2020-11-01 14:44:06', 'INSERT'),
(240, 'tblappointment', '17', '2020-11-01 14:44:06', 'UPDATE'),
(241, 'tblbarber', '5', '2020-11-01 14:44:06', 'UPDATE'),
(242, 'tblbarber', '5', '2020-11-01 15:44:10', 'UPDATE'),
(243, 'tblappointment', '20', '2020-11-01 15:45:51', 'UPDATE'),
(244, 'tblbarber', '5', '2020-11-01 15:45:55', 'UPDATE'),
(245, 'tblcurrent_appointments', '1', '2020-11-01 15:47:24', 'UPDATE'),
(246, 'tblappointment', '20', '2020-11-01 15:47:24', 'UPDATE'),
(247, 'tblbarber', '5', '2020-11-01 15:47:24', 'UPDATE'),
(248, 'tblbarber', '5', '2020-11-01 15:49:44', 'UPDATE'),
(249, 'tblcurrent_appointments', '1', '2020-11-01 16:02:07', 'UPDATE'),
(250, 'tblappointment', '20', '2020-11-01 16:02:07', 'UPDATE'),
(251, 'tblbarber', '5', '2020-11-01 16:02:07', 'UPDATE'),
(252, 'tblservice_booked', '17', '2020-11-01 18:56:17', 'DELETE'),
(253, 'tblservice_booked', '18', '2020-11-01 18:56:35', 'INSERT'),
(254, 'tblinvoice', '1', '2020-11-01 19:49:35', 'INSERT'),
(255, 'tblinvoice', '2', '2020-11-01 19:54:50', 'INSERT'),
(256, 'tblinvoice', '3', '2020-11-01 19:55:37', 'INSERT'),
(257, 'tblinvoice', '4', '2020-11-01 20:49:11', 'INSERT'),
(258, 'tblinvoice', '4', '2020-11-01 21:34:10', 'UPDATE'),
(259, 'tblinvoice', '5', '2020-11-01 21:34:52', 'INSERT'),
(260, 'tblinvoice', '6', '2020-11-01 21:35:21', 'INSERT'),
(261, 'tblshop_owner', '5', '2020-11-02 16:22:37', 'UPDATE'),
(262, 'tblshop_owner', '3', '2020-11-10 10:26:46', 'LOGIN'),
(263, 'tblshop_owner', '5', '2020-11-10 11:05:21', 'UPDATE'),
(264, 'tblshop_owner', '5', '2020-11-10 11:06:01', 'LOGIN'),
(265, 'tblsalon', '3', '2020-11-10 13:07:48', 'DELETE'),
(266, 'tblshop_owner', '5', '2020-11-10 13:08:18', 'DELETE'),
(267, 'tblshop_owner', '6', '2020-11-10 13:18:57', 'INSERT'),
(268, 'tblshop_owner', '7', '2020-11-10 16:42:19', 'INSERT'),
(269, 'tblbarber', '7', '2020-11-10 16:48:04', 'UPDATE'),
(270, 'tblbarber', '7', '2020-11-10 16:48:48', 'UPDATE'),
(271, 'tblbarber', '7', '2020-11-10 16:48:58', 'UPDATE'),
(272, 'tblbarber', '7', '2020-11-10 16:49:01', 'UPDATE'),
(273, 'tblbarber', '7', '2020-11-10 16:49:32', 'UPDATE'),
(274, 'tblbarber', '7', '2020-11-10 16:49:51', 'UPDATE'),
(275, 'tblbarber', '7', '2020-11-10 16:50:18', 'UPDATE'),
(276, 'tblbarber', '7', '2020-11-10 16:50:19', 'UPDATE'),
(277, 'tblbarber', '7', '2020-11-10 16:50:20', 'UPDATE'),
(278, 'tblbarber', '7', '2020-11-10 16:50:25', 'UPDATE'),
(279, 'tblshop_owner', '7', '2020-11-10 16:55:10', 'UPDATE'),
(280, 'tblshop_owner', '7', '2020-11-10 16:57:34', 'LOGIN'),
(281, 'tblshop_owner', '7', '2020-11-10 16:58:03', 'UPDATE'),
(282, 'tblshop_owner', '7', '2020-11-10 16:58:10', 'LOGIN'),
(283, 'tblshop_owner', '7', '2020-11-10 16:58:12', 'LOGIN'),
(284, 'tblsalon', '5', '2020-11-10 17:20:16', 'INSERT'),
(285, 'tblsalon', '6', '2020-11-10 17:21:23', 'INSERT'),
(286, 'tblsalon', '7', '2020-11-10 17:38:51', 'INSERT'),
(287, 'tblsalon', '8', '2020-11-10 17:44:14', 'INSERT'),
(288, 'tblsalon', '9', '2020-11-10 17:57:54', 'INSERT'),
(289, 'tblsalon', '9', '2020-11-10 18:03:20', 'UPDATE'),
(290, 'tblsalon', '9', '2020-11-10 18:03:36', 'UPDATE'),
(291, 'tblsalon', '9', '2020-11-10 18:03:53', 'UPDATE'),
(292, 'tblsalon', '9', '2020-11-10 18:05:52', 'UPDATE'),
(293, 'tblsalon', '9', '2020-11-10 18:05:57', 'UPDATE'),
(294, 'tblsalon', '9', '2020-11-10 18:06:06', 'UPDATE'),
(295, 'tblsalon', '10', '2020-11-10 18:14:16', 'INSERT'),
(296, 'tblsalon', '10', '2020-11-10 18:14:56', 'UPDATE'),
(297, 'tblbarber', '9', '2020-11-10 19:09:33', 'INSERT'),
(298, 'tblbarber', '9', '2020-11-10 19:22:51', 'UPDATE'),
(299, 'tblbarber', '9', '2020-11-10 19:23:22', 'UPDATE'),
(300, 'tblbarber', '9', '2020-11-10 19:23:30', 'UPDATE'),
(301, 'tblbarber', '9', '2020-11-10 19:24:38', 'UPDATE'),
(302, 'tblbarber', '9', '2020-11-10 19:25:45', 'DELETE'),
(303, 'tblcustomer', '14', '2020-11-10 19:37:41', 'INSERT'),
(304, 'tblcustomer', '15', '2020-11-10 19:38:41', 'INSERT'),
(305, 'tblcustomer', '15', '2020-11-10 19:44:23', 'UPDATE'),
(306, 'tblcustomer', '15', '2020-11-10 19:45:12', 'UPDATE'),
(307, 'tblcustomer', '15', '2020-11-10 19:45:14', 'UPDATE'),
(308, 'tblcustomer', '15', '2020-11-10 19:45:15', 'UPDATE'),
(309, 'tblcustomer', '15', '2020-11-10 19:45:20', 'UPDATE'),
(310, 'tblcustomer', '15', '2020-11-10 19:46:42', 'DELETE'),
(311, 'tblservice', '5', '2020-11-10 20:25:17', 'INSERT'),
(312, 'tblservice', '6', '2020-11-10 20:26:40', 'INSERT'),
(313, 'tblservice', '6', '2020-11-10 20:27:06', 'DELETE'),
(314, 'tblservice', '4', '2020-11-10 20:30:21', 'UPDATE'),
(315, 'tblservice', '4', '2020-11-10 20:30:51', 'UPDATE'),
(316, 'tblservice', '4', '2020-11-10 20:31:13', 'UPDATE'),
(317, 'tblbarber', '11', '2020-11-10 20:55:35', 'INSERT'),
(318, 'tblbarber', '12', '2020-11-10 20:57:15', 'INSERT'),
(319, 'tblbarber_service', '12', '2020-11-10 20:59:02', 'DELETE'),
(320, 'tblbarber', '11', '2020-11-10 21:02:37', 'DELETE'),
(321, 'tblbarber', '13', '2020-11-10 21:03:19', 'INSERT'),
(322, 'tblshop_owner', '8', '2020-11-11 11:35:49', 'INSERT'),
(323, 'tblshop_owner', '8', '2020-11-11 11:36:33', 'LOGIN'),
(324, 'tblshop_owner', '8', '2020-11-11 11:36:34', 'LOGIN'),
(325, 'tblshop_owner', '8', '2020-11-11 11:36:35', 'LOGIN'),
(326, 'tblshop_owner', '8', '2020-11-11 11:36:36', 'LOGIN'),
(327, 'tblshop_owner', '8', '2020-11-11 11:36:36', 'LOGIN'),
(328, 'tblshop_owner', '8', '2020-11-11 11:47:18', 'LOGIN'),
(329, 'tblshop_owner', '8', '2020-11-11 11:47:37', 'LOGIN'),
(330, 'tblshop_owner', '8', '2020-11-11 11:47:38', 'LOGIN'),
(331, 'tblshop_owner', '8', '2020-11-11 11:47:39', 'LOGIN'),
(332, 'tblshop_owner', '8', '2020-11-11 11:47:39', 'LOGIN'),
(333, 'tblshop_owner', '8', '2020-11-11 11:47:40', 'LOGIN'),
(334, 'tblshop_owner', '9', '2020-11-11 11:50:20', 'INSERT'),
(335, 'tblshop_owner', '9', '2020-11-11 11:51:02', 'LOGIN'),
(336, 'tblshop_owner', '10', '2020-11-11 15:11:13', 'INSERT'),
(337, 'tblshop_owner', '9', '2020-11-11 15:12:17', 'LOGIN'),
(338, 'tblshop_owner', '9', '2020-11-11 19:27:52', 'LOGIN'),
(339, 'tblshop_owner', '9', '2020-11-11 19:28:24', 'LOGIN'),
(340, 'tblshop_owner', '9', '2020-11-11 20:02:56', 'LOGIN'),
(341, 'tblshop_owner', '9', '2020-11-11 20:05:45', 'LOGIN'),
(342, 'tblshop_owner', '9', '2020-11-11 20:06:36', 'LOGIN'),
(343, 'tblshop_owner', '9', '2020-11-11 20:07:03', 'LOGIN'),
(344, 'tblshop_owner', '9', '2020-11-11 20:07:40', 'LOGIN'),
(345, 'tblshop_owner', '9', '2020-11-11 20:27:17', 'LOGIN'),
(346, 'tblshop_owner', '9', '2020-11-11 20:29:35', 'LOGIN'),
(347, 'tblshop_owner', '9', '2020-11-11 20:47:00', 'LOGIN'),
(348, 'tblshop_owner', '9', '2020-11-11 20:54:50', 'LOGIN'),
(349, 'tblshop_owner', '8', '2020-11-11 21:02:28', 'LOGIN'),
(350, 'tblshop_owner', '9', '2020-11-12 07:22:02', 'LOGIN'),
(351, 'tblshop_owner', '9', '2020-11-12 10:44:03', 'LOGIN'),
(352, 'tblshop_owner', '9', '2020-11-12 10:51:07', 'LOGIN'),
(353, 'tblshop_owner', '9', '2020-11-12 12:18:36', 'LOGIN'),
(354, 'tblshop_owner', '9', '2020-11-12 12:31:47', 'LOGIN'),
(355, 'tblshop_owner', '9', '2020-11-12 12:59:15', 'LOGIN'),
(356, 'tblshop_owner', '9', '2020-11-12 13:18:35', 'LOGIN'),
(357, 'tblshop_owner', '9', '2020-11-12 13:25:56', 'LOGIN'),
(358, 'tblshop_owner', '9', '2020-11-12 13:28:56', 'LOGIN'),
(359, 'tblshop_owner', '9', '2020-11-12 13:32:07', 'LOGIN'),
(360, 'tblshop_owner', '9', '2020-11-12 13:41:42', 'LOGIN'),
(361, 'tblshop_owner', '9', '2020-11-12 13:44:05', 'LOGIN'),
(362, 'tblshop_owner', '9', '2020-11-12 14:03:14', 'LOGIN'),
(363, 'tblshop_owner', '9', '2020-11-12 14:04:10', 'LOGIN'),
(364, 'tblshop_owner', '9', '2020-11-12 14:04:16', 'LOGIN'),
(365, 'tblshop_owner', '9', '2020-11-12 14:04:19', 'LOGIN'),
(366, 'tblshop_owner', '9', '2020-11-12 14:04:55', 'LOGIN'),
(367, 'tblshop_owner', '9', '2020-11-12 14:13:57', 'LOGIN'),
(368, 'tblshop_owner', '9', '2020-11-12 14:16:36', 'LOGIN'),
(369, 'tblshop_owner', '9', '2020-11-12 14:52:02', 'LOGIN'),
(370, 'tblshop_owner', '9', '2020-11-12 14:53:30', 'LOGIN'),
(371, 'tblshop_owner', '9', '2020-11-12 14:59:03', 'LOGIN'),
(372, 'tblshop_owner', '9', '2020-11-12 15:02:47', 'LOGIN'),
(373, 'tblshop_owner', '9', '2020-11-12 15:05:30', 'LOGIN'),
(374, 'tblshop_owner', '8', '2020-11-12 15:10:34', 'LOGIN'),
(375, 'tblshop_owner', '8', '2020-11-12 15:13:21', 'LOGIN'),
(376, 'tblshop_owner', '11', '2020-11-12 17:29:27', 'INSERT'),
(377, 'tblshop_owner', '11', '2020-11-12 17:41:37', 'LOGIN'),
(378, 'tblshop_owner', '11', '2020-11-12 17:48:01', 'LOGIN'),
(379, 'tblshop_owner', '11', '2020-11-12 17:50:13', 'LOGIN'),
(380, 'tblshop_owner', '11', '2020-11-12 17:55:02', 'LOGIN'),
(381, 'tblshop_owner', '11', '2020-11-12 18:04:58', 'LOGIN'),
(382, 'tblshop_owner', '11', '2020-11-12 18:08:51', 'LOGIN'),
(383, 'tblshop_owner', '11', '2020-11-12 18:10:58', 'LOGIN'),
(384, 'tblshop_owner', '11', '2020-11-12 18:13:47', 'LOGIN'),
(385, 'tblshop_owner', '11', '2020-11-12 18:14:02', 'LOGIN'),
(386, 'tblshop_owner', '11', '2020-11-12 18:16:54', 'LOGIN'),
(387, 'tblshop_owner', '11', '2020-11-12 18:19:13', 'LOGIN'),
(388, 'tblshop_owner', '11', '2020-11-12 18:21:59', 'LOGIN'),
(389, 'tblshop_owner', '11', '2020-11-12 18:25:19', 'LOGIN'),
(390, 'tblinvoice', '7', '2020-11-12 18:26:03', 'INSERT'),
(391, 'tblshop_owner', '12', '2020-11-13 09:08:12', 'INSERT'),
(392, 'tblshop_owner', '12', '2020-11-13 09:09:43', 'LOGIN'),
(393, 'tblshop_owner', '12', '2020-11-13 09:14:52', 'LOGIN'),
(394, 'tblshop_owner', '12', '2020-11-13 09:54:27', 'LOGIN'),
(395, 'tblshop_owner', '12', '2020-11-13 09:55:43', 'LOGIN'),
(396, 'tblshop_owner', '12', '2020-11-13 09:57:45', 'LOGIN'),
(397, 'tblshop_owner', '12', '2020-11-13 09:58:24', 'LOGIN'),
(398, 'tblshop_owner', '11', '2020-11-13 09:58:48', 'LOGIN'),
(399, 'tblshop_owner', '11', '2020-11-13 09:59:24', 'LOGIN'),
(400, 'tblshop_owner', '11', '2020-11-13 09:59:55', 'LOGIN'),
(401, 'tblshop_owner', '13', '2020-11-13 10:00:43', 'INSERT'),
(402, 'tblshop_owner', '14', '2020-11-13 10:37:47', 'INSERT');

-- --------------------------------------------------------

--
-- Table structure for table `tblsalon`
--

CREATE TABLE `tblsalon` (
  `salon_id` int(11) NOT NULL,
  `owner_id` int(11) NOT NULL,
  `salon_name` varchar(45) NOT NULL,
  `salon_location` varchar(45) NOT NULL,
  `contact_no` int(11) NOT NULL,
  `email` varchar(45) DEFAULT NULL,
  `seating_capacity` int(11) NOT NULL,
  `opening_time` time NOT NULL,
  `closing_time` time NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `tblsalon`
--

INSERT INTO `tblsalon` (`salon_id`, `owner_id`, `salon_name`, `salon_location`, `contact_no`, `email`, `seating_capacity`, `opening_time`, `closing_time`) VALUES
(1, 2, 'Cutting point', 'asdjshbjh', 767567731, 'new@gmail.com', 8, '00:00:00', '00:00:00'),
(2, 3, 'Scissors', 'asdsd', 767567744, 'scissors@gmail.com', 5, '09:00:00', '18:00:00'),
(4, 2, 'New Salon', 'klmlkm', 771122112, 'newsalon@gmail.com', 5, '09:00:00', '18:00:00'),
(5, 7, 'Scissors', 'Main Rd, Negombo', 767567755, 'scissor@gmail.com', 5, '09:00:00', '18:00:00'),
(6, 7, 'Scissors', 'Main Rd, Negombo', 767567752, 'scisso@gmail.com', 5, '09:00:00', '18:00:00'),
(8, 7, 'new', 'Main Rd, Negombo', 767567756, 'sd@gmail.com', 5, '09:00:00', '18:00:00'),
(9, 7, 'Scissors', 'Main Rd, Negombo', 767567795, 'scissorsds@gmail.com', 10, '09:00:00', '18:00:00'),
(10, 7, 'Scissors', 'Main Rd, Negombo', 767567787, 'scissorswe@gmail.com', 5, '09:00:00', '18:00:00');

-- --------------------------------------------------------

--
-- Table structure for table `tblservice`
--

CREATE TABLE `tblservice` (
  `service_id` int(11) NOT NULL,
  `service_name` varchar(45) NOT NULL,
  `salon_id` int(11) NOT NULL,
  `price` decimal(10,2) NOT NULL,
  `duration` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `tblservice`
--

INSERT INTO `tblservice` (`service_id`, `service_name`, `salon_id`, `price`, `duration`) VALUES
(1, 'HAIR TRIMMING', 1, '400.00', 0),
(2, 'Dressing', 1, '500.00', 0),
(3, 'Hair trimming', 2, '800.00', 5400),
(4, 'Facial', 2, '1300.00', 5400),
(5, 'Waxing', 4, '5000.00', 7200);

-- --------------------------------------------------------

--
-- Table structure for table `tblservice_booked`
--

CREATE TABLE `tblservice_booked` (
  `id` int(11) NOT NULL,
  `appointment_id` int(11) NOT NULL,
  `service_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `tblservice_booked`
--

INSERT INTO `tblservice_booked` (`id`, `appointment_id`, `service_id`) VALUES
(15, 17, 3),
(18, 20, 3);

-- --------------------------------------------------------

--
-- Table structure for table `tblshop_owner`
--

CREATE TABLE `tblshop_owner` (
  `owner_id` int(11) NOT NULL,
  `name` varchar(30) NOT NULL,
  `contact_no` int(11) NOT NULL,
  `email` varchar(40) DEFAULT NULL,
  `password` varchar(100) NOT NULL,
  `pin` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `tblshop_owner`
--

INSERT INTO `tblshop_owner` (`owner_id`, `name`, `contact_no`, `email`, `password`, `pin`) VALUES
(2, 'Romela', 767567711, 'rom@gmail.com', 'cXdlcnR5MTIzIw==', 'MTIzNDU='),
(3, 'sam', 767567733, 'sam@ymail.com', 'cXdlcnR5MTIzIw==', 'MTIzNDU='),
(7, 'Sanushi Salgdo', 767567732, 'san@gmail.com', 'YWJjZDEyMyM=', 'MTIzNDU='),
(11, 'Mike', 767567799, 'mike@gmail.com', 'YWJjZDEyMyM=', 'MTIzNDU=');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `tblappointment`
--
ALTER TABLE `tblappointment`
  ADD PRIMARY KEY (`appointment_id`) USING BTREE,
  ADD KEY `tblJob_tblBarber_idx` (`barber_Id`),
  ADD KEY `tblJob_tblSalon` (`salon_id`),
  ADD KEY `tblJob_tblCustomer` (`customer_id`);

--
-- Indexes for table `tblbarber`
--
ALTER TABLE `tblbarber`
  ADD PRIMARY KEY (`barber_id`),
  ADD KEY `tblBarber_tblSalon_idx` (`salon_id`) USING BTREE;

--
-- Indexes for table `tblbarber_service`
--
ALTER TABLE `tblbarber_service`
  ADD PRIMARY KEY (`barber_service_id`),
  ADD KEY `tblBarberRole_tblBarber` (`barber_id`) USING BTREE,
  ADD KEY `tblBarberRole_tblService` (`service_id`) USING BTREE;

--
-- Indexes for table `tblcurrent_appointments`
--
ALTER TABLE `tblcurrent_appointments`
  ADD PRIMARY KEY (`current_appointment_id`),
  ADD KEY `tblCurrentAppointments_tblSalon` (`salon_id`),
  ADD KEY `tblCurrentAppointments_tblBarber` (`barber_id`),
  ADD KEY `tblCurrentAppointments_tblAppointment` (`appointment_id`);

--
-- Indexes for table `tblcustomer`
--
ALTER TABLE `tblcustomer`
  ADD PRIMARY KEY (`customer_id`),
  ADD KEY `tblCustomer_tblSalon` (`salon_id`);

--
-- Indexes for table `tblinvoice`
--
ALTER TABLE `tblinvoice`
  ADD PRIMARY KEY (`invoice_id`),
  ADD KEY `tblInvoice_tblAppointment` (`appointment_id`),
  ADD KEY `tblInvoice_tblSalon` (`salon_id`);

--
-- Indexes for table `tbllog`
--
ALTER TABLE `tbllog`
  ADD PRIMARY KEY (`log_id`);

--
-- Indexes for table `tblsalon`
--
ALTER TABLE `tblsalon`
  ADD PRIMARY KEY (`salon_id`),
  ADD KEY `tblSalon_tblShopOwner` (`owner_id`);

--
-- Indexes for table `tblservice`
--
ALTER TABLE `tblservice`
  ADD PRIMARY KEY (`service_id`),
  ADD KEY `tblService_tblSalon_idx` (`salon_id`) USING BTREE;

--
-- Indexes for table `tblservice_booked`
--
ALTER TABLE `tblservice_booked`
  ADD PRIMARY KEY (`id`),
  ADD KEY `tblServiceBooked_tblService` (`service_id`),
  ADD KEY `tblServiceBooked_tblAppointment` (`appointment_id`);

--
-- Indexes for table `tblshop_owner`
--
ALTER TABLE `tblshop_owner`
  ADD PRIMARY KEY (`owner_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `tblappointment`
--
ALTER TABLE `tblappointment`
  MODIFY `appointment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;
--
-- AUTO_INCREMENT for table `tblbarber`
--
ALTER TABLE `tblbarber`
  MODIFY `barber_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;
--
-- AUTO_INCREMENT for table `tblbarber_service`
--
ALTER TABLE `tblbarber_service`
  MODIFY `barber_service_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;
--
-- AUTO_INCREMENT for table `tblcurrent_appointments`
--
ALTER TABLE `tblcurrent_appointments`
  MODIFY `current_appointment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;
--
-- AUTO_INCREMENT for table `tblcustomer`
--
ALTER TABLE `tblcustomer`
  MODIFY `customer_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;
--
-- AUTO_INCREMENT for table `tblinvoice`
--
ALTER TABLE `tblinvoice`
  MODIFY `invoice_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;
--
-- AUTO_INCREMENT for table `tbllog`
--
ALTER TABLE `tbllog`
  MODIFY `log_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=403;
--
-- AUTO_INCREMENT for table `tblsalon`
--
ALTER TABLE `tblsalon`
  MODIFY `salon_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;
--
-- AUTO_INCREMENT for table `tblservice`
--
ALTER TABLE `tblservice`
  MODIFY `service_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
--
-- AUTO_INCREMENT for table `tblservice_booked`
--
ALTER TABLE `tblservice_booked`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;
--
-- AUTO_INCREMENT for table `tblshop_owner`
--
ALTER TABLE `tblshop_owner`
  MODIFY `owner_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `tblappointment`
--
ALTER TABLE `tblappointment`
  ADD CONSTRAINT `tblJob_tblBarber` FOREIGN KEY (`barber_Id`) REFERENCES `tblbarber` (`barber_id`),
  ADD CONSTRAINT `tblJob_tblCustomer` FOREIGN KEY (`customer_id`) REFERENCES `tblcustomer` (`customer_id`),
  ADD CONSTRAINT `tblJob_tblSalon` FOREIGN KEY (`salon_id`) REFERENCES `tblsalon` (`salon_id`);

--
-- Constraints for table `tblbarber`
--
ALTER TABLE `tblbarber`
  ADD CONSTRAINT `tblBarber_tblSalon` FOREIGN KEY (`salon_id`) REFERENCES `tblsalon` (`salon_id`);

--
-- Constraints for table `tblbarber_service`
--
ALTER TABLE `tblbarber_service`
  ADD CONSTRAINT `tblBaberRole_tblBarber` FOREIGN KEY (`barber_id`) REFERENCES `tblbarber` (`barber_id`),
  ADD CONSTRAINT `tblBaberRole_tblService` FOREIGN KEY (`service_id`) REFERENCES `tblservice` (`service_id`);

--
-- Constraints for table `tblcurrent_appointments`
--
ALTER TABLE `tblcurrent_appointments`
  ADD CONSTRAINT `tblCurrentAppointments_tblAppointment` FOREIGN KEY (`appointment_id`) REFERENCES `tblappointment` (`appointment_id`),
  ADD CONSTRAINT `tblCurrentAppointments_tblBarber` FOREIGN KEY (`barber_id`) REFERENCES `tblbarber` (`barber_id`),
  ADD CONSTRAINT `tblCurrentAppointments_tblSalon` FOREIGN KEY (`salon_id`) REFERENCES `tblsalon` (`salon_id`);

--
-- Constraints for table `tblcustomer`
--
ALTER TABLE `tblcustomer`
  ADD CONSTRAINT `tblCustomer_tblSalon` FOREIGN KEY (`salon_id`) REFERENCES `tblsalon` (`salon_id`);

--
-- Constraints for table `tblinvoice`
--
ALTER TABLE `tblinvoice`
  ADD CONSTRAINT `tblInvoice_tblAppointment` FOREIGN KEY (`appointment_id`) REFERENCES `tblappointment` (`appointment_id`),
  ADD CONSTRAINT `tblInvoice_tblSalon` FOREIGN KEY (`salon_id`) REFERENCES `tblsalon` (`salon_id`);

--
-- Constraints for table `tblsalon`
--
ALTER TABLE `tblsalon`
  ADD CONSTRAINT `tblSalon_tblShopOwner` FOREIGN KEY (`owner_id`) REFERENCES `tblshop_owner` (`owner_id`);

--
-- Constraints for table `tblservice`
--
ALTER TABLE `tblservice`
  ADD CONSTRAINT `tblService_tblShop` FOREIGN KEY (`salon_id`) REFERENCES `tblsalon` (`salon_id`);

--
-- Constraints for table `tblservice_booked`
--
ALTER TABLE `tblservice_booked`
  ADD CONSTRAINT `tblServiceBooked_tblAppointment` FOREIGN KEY (`appointment_id`) REFERENCES `tblappointment` (`appointment_id`),
  ADD CONSTRAINT `tblServiceBooked_tblService` FOREIGN KEY (`service_id`) REFERENCES `tblservice` (`service_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
