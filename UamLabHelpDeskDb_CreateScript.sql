-- =============================================
-- UAM Lab Help Desk - Script de Base de Datos
-- Ejecutar en SQL Server Management Studio
-- =============================================

USE master;
GO

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'UamLabHelpDeskDb')
BEGIN
    CREATE DATABASE UamLabHelpDeskDb;
    PRINT 'Base de datos UamLabHelpDeskDb creada.';
END
ELSE
BEGIN
    PRINT 'La base de datos UamLabHelpDeskDb ya existe.';
END
GO

USE UamLabHelpDeskDb;
GO

-- =============================================
-- Tabla: Laboratories
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Laboratories')
BEGIN
    CREATE TABLE Laboratories (
        Id            INT            NOT NULL IDENTITY(1,1),
        Name          NVARCHAR(100)  NOT NULL,
        Building      NVARCHAR(50)   NOT NULL,
        Floor         INT            NOT NULL,
        Capacity      INT            NOT NULL,
        IsActive      BIT            NOT NULL CONSTRAINT DF_Laboratories_IsActive DEFAULT (1),
        CreatedAtUtc  DATETIME2      NOT NULL,
        UpdatedAtUtc  DATETIME2      NOT NULL,
        CONSTRAINT PK_Laboratories PRIMARY KEY (Id),
        CONSTRAINT UQ_Laboratories_Name UNIQUE (Name),
        CONSTRAINT CK_Laboratories_Capacity CHECK (Capacity > 0)
    );
    PRINT 'Tabla Laboratories creada.';
END
GO

-- =============================================
-- Tabla: Equipment
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Equipment')
BEGIN
    CREATE TABLE Equipment (
        Id            INT            NOT NULL IDENTITY(1,1),
        LaboratoryId  INT            NOT NULL,
        Code          NVARCHAR(20)   NOT NULL,
        Brand         NVARCHAR(50)   NOT NULL,
        Model         NVARCHAR(50)   NOT NULL,
        SerialNumber  NVARCHAR(50)   NOT NULL,
        Type          NVARCHAR(30)   NOT NULL,
        Status        NVARCHAR(20)   NOT NULL,
        PurchaseDate  DATE           NULL,
        IsActive      BIT            NOT NULL CONSTRAINT DF_Equipment_IsActive DEFAULT (1),
        CreatedAtUtc  DATETIME2      NOT NULL,
        UpdatedAtUtc  DATETIME2      NOT NULL,
        CONSTRAINT PK_Equipment PRIMARY KEY (Id),
        CONSTRAINT UQ_Equipment_Code UNIQUE (Code),
        CONSTRAINT UQ_Equipment_SerialNumber UNIQUE (SerialNumber),
        CONSTRAINT FK_Equipment_Laboratories FOREIGN KEY (LaboratoryId)
            REFERENCES Laboratories (Id) ON DELETE NO ACTION ON UPDATE NO ACTION,
        CONSTRAINT CK_Equipment_Type CHECK (Type IN ('Desktop','Laptop','Printer','Projector','Other')),
        CONSTRAINT CK_Equipment_Status CHECK (Status IN ('Operational','UnderRepair','Decommissioned'))
    );

    CREATE INDEX IX_Equipment_LaboratoryId ON Equipment (LaboratoryId);
    PRINT 'Tabla Equipment creada.';
END
GO

-- =============================================
-- Tabla de historial de migraciones EF Core
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL,
        ProductVersion NVARCHAR(32)  NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );
    PRINT 'Tabla __EFMigrationsHistory creada.';
END
GO

-- Registrar migración para que EF Core no la vuelva a aplicar
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260529000000_InitialCreate')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260529000000_InitialCreate', '10.0.7');
    PRINT 'Migración registrada en __EFMigrationsHistory.';
END
GO

-- =============================================
-- Datos de ejemplo (opcional)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Laboratories)
BEGIN
    INSERT INTO Laboratories (Name, Building, Floor, Capacity, IsActive, CreatedAtUtc, UpdatedAtUtc)
    VALUES
        ('Laboratorio de Redes', 'Edificio A', 1, 30, 1, GETUTCDATE(), GETUTCDATE()),
        ('Laboratorio de Software', 'Edificio B', 2, 25, 1, GETUTCDATE(), GETUTCDATE()),
        ('Laboratorio de Hardware', 'Edificio A', 3, 20, 1, GETUTCDATE(), GETUTCDATE());
    PRINT 'Datos de ejemplo insertados en Laboratories.';
END
GO

IF NOT EXISTS (SELECT 1 FROM Equipment)
BEGIN
    DECLARE @LabRedes INT = (SELECT Id FROM Laboratories WHERE Name = 'Laboratorio de Redes');
    DECLARE @LabSw    INT = (SELECT Id FROM Laboratories WHERE Name = 'Laboratorio de Software');

    INSERT INTO Equipment (LaboratoryId, Code, Brand, Model, SerialNumber, Type, Status, PurchaseDate, IsActive, CreatedAtUtc, UpdatedAtUtc)
    VALUES
        (@LabRedes, 'EQ-001', 'Dell',   'OptiPlex 7090', 'SN-DELL-001', 'Desktop',  'Operational',    '2023-01-15', 1, GETUTCDATE(), GETUTCDATE()),
        (@LabRedes, 'EQ-002', 'HP',     'LaserJet Pro',  'SN-HP-002',   'Printer',  'Operational',    '2022-06-10', 1, GETUTCDATE(), GETUTCDATE()),
        (@LabSw,    'EQ-003', 'Lenovo', 'ThinkPad E15',  'SN-LEN-003',  'Laptop',   'UnderRepair',    '2021-09-20', 1, GETUTCDATE(), GETUTCDATE()),
        (@LabSw,    'EQ-004', 'Epson',  'EB-2250U',      'SN-EPS-004',  'Projector','Operational',    '2023-03-01', 1, GETUTCDATE(), GETUTCDATE());
    PRINT 'Datos de ejemplo insertados en Equipment.';
END
GO

PRINT '=== Script ejecutado correctamente. ===';
GO
