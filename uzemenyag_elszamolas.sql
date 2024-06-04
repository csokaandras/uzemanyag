-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2024. Jún 04. 17:12
-- Kiszolgáló verziója: 10.4.28-MariaDB
-- PHP verzió: 8.0.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `uzemenyag_elszamolas`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `cars`
--

CREATE TABLE `cars` (
  `ID` int(11) NOT NULL,
  `type` varchar(50) NOT NULL,
  `license` varchar(10) NOT NULL,
  `consumption` float NOT NULL,
  `fuelID` int(11) NOT NULL,
  `enable` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `cars`
--

INSERT INTO `cars` (`ID`, `type`, `license`, `consumption`, `fuelID`, `enable`) VALUES
(4, 'Panzerkampfwagen I', 'KYS-420', 2, 2, 0),
(5, 'Königstiger', 'NZI-001', 3.5, 2, 0),
(6, 'PzKpfw V Panther', 'PZW-256', 2.5, 1, 1),
(7, 'Leichttraktor', 'PZW-531', 1.8, 2, 0),
(8, 'Neubaufahrzeug', 'PZW-967', 5, 1, 1),
(9, 'Panzer VII Löwe', 'PZW-542', 0.9, 1, 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `fuels`
--

CREATE TABLE `fuels` (
  `ID` int(11) NOT NULL,
  `type` varchar(20) NOT NULL,
  `price` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `fuels`
--

INSERT INTO `fuels` (`ID`, `type`, `price`) VALUES
(1, 'Benzin', 635),
(2, 'Diesel', 622);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `routes`
--

CREATE TABLE `routes` (
  `ID` int(11) NOT NULL,
  `start` varchar(50) NOT NULL,
  `end` varchar(50) NOT NULL,
  `km` int(11) NOT NULL,
  `date` date NOT NULL,
  `userID` int(11) NOT NULL,
  `carID` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `routes`
--

INSERT INTO `routes` (`ID`, `start`, `end`, `km`, `date`, `userID`, `carID`) VALUES
(1, 'Innen', 'Oda', 35, '2024-05-28', 2, 5),
(2, 'Áronkától', 'Janihoz', 100, '2024-05-27', 2, 7),
(3, 'Janitól', 'Jani apjához', 5, '2024-05-27', 3, 8),
(4, 'Susu', 'Haza', 85, '2024-04-04', 2, 6),
(5, 'Bótbó', 'Mamához', 40, '2024-06-04', 2, 9),
(6, 'Bótbó', 'Mamához', 10, '2024-06-04', 2, 8),
(7, 'Janitól', 'Fagyizni', 8, '2023-06-07', 3, 6);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `users`
--

CREATE TABLE `users` (
  `ID` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `pass` varchar(100) NOT NULL,
  `perm` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`ID`, `name`, `pass`, `perm`) VALUES
(1, 'admin', 'admin', 0),
(2, 'Jani', 'jani', 1),
(3, 'Áron', 'aron', 1),
(6, 'asd', 'asd', 1);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `cars`
--
ALTER TABLE `cars`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `fuelID` (`fuelID`);

--
-- A tábla indexei `fuels`
--
ALTER TABLE `fuels`
  ADD PRIMARY KEY (`ID`);

--
-- A tábla indexei `routes`
--
ALTER TABLE `routes`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `userID` (`userID`),
  ADD KEY `carID` (`carID`);

--
-- A tábla indexei `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `perm` (`perm`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `cars`
--
ALTER TABLE `cars`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT a táblához `fuels`
--
ALTER TABLE `fuels`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT a táblához `routes`
--
ALTER TABLE `routes`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `cars`
--
ALTER TABLE `cars`
  ADD CONSTRAINT `cars_ibfk_1` FOREIGN KEY (`fuelID`) REFERENCES `fuels` (`ID`);

--
-- Megkötések a táblához `routes`
--
ALTER TABLE `routes`
  ADD CONSTRAINT `routes_ibfk_2` FOREIGN KEY (`userID`) REFERENCES `users` (`ID`),
  ADD CONSTRAINT `routes_ibfk_3` FOREIGN KEY (`carID`) REFERENCES `cars` (`ID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
