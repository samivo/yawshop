CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE TABLE `Checkouts` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Reference` longtext CHARACTER SET utf8mb4 NOT NULL,
        `TotalAmount` int NOT NULL,
        `ClientId` int NOT NULL,
        `TransactionId` longtext CHARACTER SET utf8mb4 NULL,
        `PaymentStatus` int NOT NULL,
        `PaymentMethod` longtext CHARACTER SET utf8mb4 NULL,
        `InternalComment` longtext CHARACTER SET utf8mb4 NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `UpdatetAt` datetime(6) NOT NULL,
        `ModifierName` longtext CHARACTER SET utf8mb4 NULL,
        `Hash` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Checkouts` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE TABLE `Clients` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NOT NULL,
        `FirstName` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
        `LastName` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
        `Email` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `Phone` varchar(16) CHARACTER SET utf8mb4 NOT NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `InternalComment` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Clients` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE TABLE `Discounts` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Description` longtext CHARACTER SET utf8mb4 NOT NULL,
        `TargetProductCode` longtext CHARACTER SET utf8mb4 NOT NULL,
        `DiscountAmountInMinorUnits` int NOT NULL,
        `QuantityTotal` int NULL,
        `ValidFrom` datetime(6) NULL,
        `ValidTo` datetime(6) NULL,
        `QuantityUsed` int NOT NULL,
        `InternalDescription` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Discounts` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE TABLE `Events` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ProductCode` longtext CHARACTER SET utf8mb4 NOT NULL,
        `RegistrationsQuantityTotal` int NULL,
        `RegistrationsQuantityUsed` int NOT NULL,
        `EventStart` datetime(6) NOT NULL,
        `EventEnd` datetime(6) NOT NULL,
        `HoursBeforeEventUnavailable` int NOT NULL,
        `Status` int NOT NULL,
        `ClientId` int NULL,
        CONSTRAINT `PK_Events` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE TABLE `Giftcards` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ValueLeftInMinorUnits` int NOT NULL,
        `IsValueBased` tinyint(1) NOT NULL,
        `TargetProductCode` longtext CHARACTER SET utf8mb4 NOT NULL,
        `InternalComment` longtext CHARACTER SET utf8mb4 NULL,
        `PurchaseDate` datetime(6) NOT NULL,
        `ExpireDate` datetime(6) NOT NULL,
        `UsedDate` datetime(6) NULL,
        `OwnerClientId` int NOT NULL,
        `UserClientId` int NULL,
        CONSTRAINT `PK_Giftcards` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE TABLE `Products` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Code` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsActive` tinyint(1) NOT NULL,
        `IsVisibleToPublic` tinyint(1) NOT NULL,
        `MaxQuantityPerPurchase` int NULL,
        `QuantityTotal` int NULL,
        `QuantityUsed` int NOT NULL,
        `PriceInMinorUnitsIncludingVat` int NOT NULL,
        `VatPercentage` decimal(65,30) NOT NULL,
        `ShortDescription` longtext CHARACTER SET utf8mb4 NOT NULL,
        `DescriptionOrInnerHtml` longtext CHARACTER SET utf8mb4 NOT NULL,
        `InternalComment` longtext CHARACTER SET utf8mb4 NULL,
        `AvailableFrom` datetime(6) NOT NULL,
        `AvailableTo` datetime(6) NULL,
        `ProductType` int NOT NULL,
        `ProductGroupId` int NULL,
        `CreatedAt` datetime(6) NOT NULL,
        `ModifiedAt` datetime(6) NOT NULL,
        `Modifier` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_Products` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE TABLE `CheckoutItemModel` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `CheckoutModelId` int NOT NULL,
        `UnitPrice` int NOT NULL,
        `Units` int NOT NULL,
        `VatPercentage` decimal(65,30) NOT NULL,
        `ProductCode` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ProductName` longtext CHARACTER SET utf8mb4 NOT NULL,
        `EventCode` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_CheckoutItemModel` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_CheckoutItemModel_Checkouts_CheckoutModelId` FOREIGN KEY (`CheckoutModelId`) REFERENCES `Checkouts` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE TABLE `CustomClientFields` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ClientModelId` int NOT NULL,
        `FieldName` longtext CHARACTER SET utf8mb4 NOT NULL,
        `FieldValue` longtext CHARACTER SET utf8mb4 NOT NULL,
        `FieldType` int NOT NULL,
        CONSTRAINT `PK_CustomClientFields` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_CustomClientFields_Clients_ClientModelId` FOREIGN KEY (`ClientModelId`) REFERENCES `Clients` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE TABLE `CustomerField` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ProductModelId` int NOT NULL,
        `FieldName` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsRequired` tinyint(1) NOT NULL,
        `Href` longtext CHARACTER SET utf8mb4 NULL,
        `FieldType` int NOT NULL,
        CONSTRAINT `PK_CustomerField` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_CustomerField_Products_ProductModelId` FOREIGN KEY (`ProductModelId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE INDEX `IX_CheckoutItemModel_CheckoutModelId` ON `CheckoutItemModel` (`CheckoutModelId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE INDEX `IX_CustomClientFields_ClientModelId` ON `CustomClientFields` (`ClientModelId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    CREATE INDEX `IX_CustomerField_ProductModelId` ON `CustomerField` (`ProductModelId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212131057_init') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241212131057_init', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132041_mig1') THEN

    DROP TABLE `CustomClientFields`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132041_mig1') THEN

    DROP TABLE `CustomerField`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132041_mig1') THEN

    CREATE TABLE `AdditionalClientFields` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ClientModelId` int NOT NULL,
        `FieldName` longtext CHARACTER SET utf8mb4 NOT NULL,
        `FieldValue` longtext CHARACTER SET utf8mb4 NOT NULL,
        `FieldType` int NOT NULL,
        CONSTRAINT `PK_AdditionalClientFields` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_AdditionalClientFields_Clients_ClientModelId` FOREIGN KEY (`ClientModelId`) REFERENCES `Clients` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132041_mig1') THEN

    CREATE TABLE `ProductSpesificClientFields` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ProductModelId` int NOT NULL,
        `FieldName` longtext CHARACTER SET utf8mb4 NOT NULL,
        `IsRequired` tinyint(1) NOT NULL,
        `Href` longtext CHARACTER SET utf8mb4 NULL,
        `FieldType` int NOT NULL,
        CONSTRAINT `PK_ProductSpesificClientFields` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ProductSpesificClientFields_Products_ProductModelId` FOREIGN KEY (`ProductModelId`) REFERENCES `Products` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132041_mig1') THEN

    CREATE INDEX `IX_AdditionalClientFields_ClientModelId` ON `AdditionalClientFields` (`ClientModelId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132041_mig1') THEN

    CREATE INDEX `IX_ProductSpesificClientFields_ProductModelId` ON `ProductSpesificClientFields` (`ProductModelId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132041_mig1') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241212132041_mig1', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132208_mig2') THEN

    DROP TABLE `CheckoutItemModel`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132208_mig2') THEN

    CREATE TABLE `CheckoutItem` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `CheckoutModelId` int NOT NULL,
        `UnitPrice` int NOT NULL,
        `Units` int NOT NULL,
        `VatPercentage` decimal(65,30) NOT NULL,
        `ProductCode` longtext CHARACTER SET utf8mb4 NOT NULL,
        `ProductName` longtext CHARACTER SET utf8mb4 NOT NULL,
        `EventCode` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_CheckoutItem` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_CheckoutItem_Checkouts_CheckoutModelId` FOREIGN KEY (`CheckoutModelId`) REFERENCES `Checkouts` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132208_mig2') THEN

    CREATE INDEX `IX_CheckoutItem_CheckoutModelId` ON `CheckoutItem` (`CheckoutModelId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241212132208_mig2') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241212132208_mig2', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241218094704_mig3') THEN

    ALTER TABLE `ProductSpesificClientFields` ADD `FieldNamePublic` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241218094704_mig3') THEN

    ALTER TABLE `Clients` MODIFY COLUMN `Phone` varchar(16) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241218094704_mig3') THEN

    ALTER TABLE `AdditionalClientFields` MODIFY COLUMN `FieldValue` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241218094704_mig3') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241218094704_mig3', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241230114423_mig4') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241230114423_mig4', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241230143259_mig5') THEN

    ALTER TABLE `Products` ADD `GiftcardPeriodInDays` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241230143259_mig5') THEN

    ALTER TABLE `Products` ADD `GiftcardTargetProductCode` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241230143259_mig5') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241230143259_mig5', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE TABLE `AspNetRoles` (
        `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(256) CHARACTER SET utf8mb4 NULL,
        `NormalizedName` varchar(256) CHARACTER SET utf8mb4 NULL,
        `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE TABLE `AspNetUsers` (
        `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `UserName` varchar(256) CHARACTER SET utf8mb4 NULL,
        `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 NULL,
        `Email` varchar(256) CHARACTER SET utf8mb4 NULL,
        `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 NULL,
        `EmailConfirmed` tinyint(1) NOT NULL,
        `PasswordHash` longtext CHARACTER SET utf8mb4 NULL,
        `SecurityStamp` longtext CHARACTER SET utf8mb4 NULL,
        `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 NULL,
        `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL,
        `PhoneNumberConfirmed` tinyint(1) NOT NULL,
        `TwoFactorEnabled` tinyint(1) NOT NULL,
        `LockoutEnd` datetime(6) NULL,
        `LockoutEnabled` tinyint(1) NOT NULL,
        `AccessFailedCount` int NOT NULL,
        CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE TABLE `AspNetRoleClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
        `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE TABLE `AspNetUserClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ClaimType` longtext CHARACTER SET utf8mb4 NULL,
        `ClaimValue` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE TABLE `AspNetUserLogins` (
        `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `ProviderDisplayName` longtext CHARACTER SET utf8mb4 NULL,
        `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
        CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE TABLE `AspNetUserRoles` (
        `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `RoleId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
        CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE TABLE `AspNetUserTokens` (
        `UserId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Value` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
        CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250106142843_mig6') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250106142843_mig6', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107124205_mig7') THEN

    ALTER TABLE `AspNetUserTokens` MODIFY COLUMN `Name` varchar(128) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107124205_mig7') THEN

    ALTER TABLE `AspNetUserTokens` MODIFY COLUMN `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107124205_mig7') THEN

    ALTER TABLE `AspNetUserLogins` MODIFY COLUMN `ProviderKey` varchar(128) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107124205_mig7') THEN

    ALTER TABLE `AspNetUserLogins` MODIFY COLUMN `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107124205_mig7') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250107124205_mig7', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107125032_mig8') THEN

    ALTER TABLE `AspNetUserTokens` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107125032_mig8') THEN

    ALTER TABLE `AspNetUserTokens` MODIFY COLUMN `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107125032_mig8') THEN

    ALTER TABLE `AspNetUserLogins` MODIFY COLUMN `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107125032_mig8') THEN

    ALTER TABLE `AspNetUserLogins` MODIFY COLUMN `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107125032_mig8') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250107125032_mig8', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107125400_mig9') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250107125400_mig9', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107134204_mig10') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250107134204_mig10', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107142220_mig11') THEN

    ALTER TABLE `AspNetUserTokens` MODIFY COLUMN `Name` varchar(128) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107142220_mig11') THEN

    ALTER TABLE `AspNetUserTokens` MODIFY COLUMN `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107142220_mig11') THEN

    ALTER TABLE `AspNetUserLogins` MODIFY COLUMN `ProviderKey` varchar(128) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107142220_mig11') THEN

    ALTER TABLE `AspNetUserLogins` MODIFY COLUMN `LoginProvider` varchar(128) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107142220_mig11') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250107142220_mig11', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107144244_mig12') THEN

    ALTER TABLE `AspNetUserTokens` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107144244_mig12') THEN

    ALTER TABLE `AspNetUserTokens` MODIFY COLUMN `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107144244_mig12') THEN

    ALTER TABLE `AspNetUserLogins` MODIFY COLUMN `ProviderKey` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107144244_mig12') THEN

    ALTER TABLE `AspNetUserLogins` MODIFY COLUMN `LoginProvider` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250107144244_mig12') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250107144244_mig12', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250113073215_mig13') THEN

    ALTER TABLE `Products` ADD `AvatarImage` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250113073215_mig13') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250113073215_mig13', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250114095109_mig14') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250114095109_mig14', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250115090204_mig15') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250115090204_mig15', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250115144254_mig16') THEN

    ALTER TABLE `Clients` DROP COLUMN `Phone`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250115144254_mig16') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250115144254_mig16', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250120091218_mig17') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250120091218_mig17', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204065040_mig18') THEN

    ALTER TABLE `ProductSpesificClientFields` DROP COLUMN `FieldNamePublic`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204065040_mig18') THEN

    ALTER TABLE `Products` MODIFY COLUMN `ShortDescription` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204065040_mig18') THEN

    ALTER TABLE `Products` MODIFY COLUMN `DescriptionOrInnerHtml` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204065040_mig18') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250204065040_mig18', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204141135_mig19') THEN

    ALTER TABLE `CheckoutItem` ADD `DiscountCodes` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204141135_mig19') THEN

    ALTER TABLE `CheckoutItem` ADD `GiftcardCodes` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204141135_mig19') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250204141135_mig19', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204141342_mig20') THEN

    ALTER TABLE `CheckoutItem` RENAME COLUMN `GiftcardCodes` TO `GiftcardCode`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204141342_mig20') THEN

    ALTER TABLE `CheckoutItem` RENAME COLUMN `DiscountCodes` TO `DiscountCode`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250204141342_mig20') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250204141342_mig20', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250207071235_mig21') THEN

    ALTER TABLE `Events` DROP COLUMN `ClientId`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250207071235_mig21') THEN

    ALTER TABLE `Events` ADD `ClientCodes` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250207071235_mig21') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250207071235_mig21', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250211122829_mig22') THEN

    ALTER TABLE `Events` RENAME COLUMN `ClientCodes` TO `ClientCode`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250211122829_mig22') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250211122829_mig22', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250217111205_mig23') THEN

    ALTER TABLE `Events` DROP COLUMN `RegistrationsQuantityTotal`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250217111205_mig23') THEN

    ALTER TABLE `Events` DROP COLUMN `RegistrationsQuantityUsed`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250217111205_mig23') THEN

    ALTER TABLE `Events` DROP COLUMN `Status`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250217111205_mig23') THEN

    ALTER TABLE `Events` ADD `IsVisible` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20250217111205_mig23') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20250217111205_mig23', '8.0.10');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

