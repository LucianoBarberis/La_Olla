-- La Olla: schema + demo seed.
-- Run once on SQL Server (SSMS or sqlcmd) before the demo:
--   sqlcmd -S localhost -E -i sql/01_schema_seed.sql
-- Then set the same Data Source in src/LaOlla.Vista/App.config.

IF DB_ID('LaOlla') IS NULL
    CREATE DATABASE LaOlla;
GO

USE LaOlla;
GO

IF OBJECT_ID('dbo.Reserva', 'U') IS NOT NULL DROP TABLE dbo.Reserva;
IF OBJECT_ID('dbo.Vianda', 'U') IS NOT NULL DROP TABLE dbo.Vianda;
IF OBJECT_ID('dbo.Cliente', 'U') IS NOT NULL DROP TABLE dbo.Cliente;
GO

CREATE TABLE dbo.Cliente (
    Dni     CHAR(8)      NOT NULL PRIMARY KEY,
    Nombre  NVARCHAR(80) NOT NULL
);

-- One row per date: a single daily batch (S1).
CREATE TABLE dbo.Vianda (
    Fecha       DATE         NOT NULL PRIMARY KEY,
    NombreMenu  NVARCHAR(120) NOT NULL,
    Cupo        INT           NOT NULL CHECK (Cupo > 0 AND Cupo <= 150) -- R1 cap
);

-- One reservation = one (Fecha, Dni), quantity always 1 (R2).
CREATE TABLE dbo.Reserva (
    Fecha       DATE         NOT NULL,
    Dni         CHAR(8)      NOT NULL,
    NombreMenu  NVARCHAR(120) NOT NULL,
    Estado      NVARCHAR(20)  NOT NULL DEFAULT 'No entregado'
        CHECK (Estado IN ('No entregado', 'Entregado', 'No retirado', 'Cancelada')), -- R5
    CONSTRAINT PK_Reserva PRIMARY KEY (Fecha, Dni),
    CONSTRAINT FK_Reserva_Cliente FOREIGN KEY (Dni) REFERENCES dbo.Cliente (Dni),
    CONSTRAINT FK_Reserva_Vianda FOREIGN KEY (Fecha) REFERENCES dbo.Vianda (Fecha)
);
GO

-- Demo seed (day+1, quota 10): counter admin + one student.
DECLARE @demo DATE = DATEADD(DAY, 1, CAST(GETDATE() AS DATE));

INSERT INTO dbo.Cliente (Dni, Nombre) VALUES
    ('12345678', 'Mostrador'),
    ('87654321', 'Estudiante Demo');

INSERT INTO dbo.Vianda (Fecha, NombreMenu, Cupo) VALUES
    (@demo, 'Menu del dia', 10);
GO
