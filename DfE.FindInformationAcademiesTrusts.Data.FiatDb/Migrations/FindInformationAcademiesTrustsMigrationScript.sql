IF OBJECT_ID(N'[__FindInformationAcademiesTrustsContextMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__FindInformationAcademiesTrustsContextMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___FindInformationAcademiesTrustsContextMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__FindInformationAcademiesTrustsContextMigrationsHistory]
    WHERE [MigrationId] = N'20260909123351_InitialCreate'
)
BEGIN
    CREATE TABLE [Watchlist] (
        [Id] uniqueidentifier NOT NULL,
        [ReadableId] int NOT NULL IDENTITY,
        [EstablishmentId] nvarchar(20) NULL,
        [TrustId] nvarchar(20) NULL,
        [IsTrust] bit NOT NULL,
        [User] nvarchar(320) NULL,
        [CreatedOn] datetime2 NOT NULL,
        [CreatedBy] nvarchar(320) NOT NULL,
        [LastModifiedOn] datetime2 NULL,
        [LastModifiedBy] nvarchar(320) NULL,
        CONSTRAINT [PK_Watchlist] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__FindInformationAcademiesTrustsContextMigrationsHistory]
    WHERE [MigrationId] = N'20260909123351_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Watchlist_User_EstablishmentId] ON [Watchlist] ([User], [EstablishmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__FindInformationAcademiesTrustsContextMigrationsHistory]
    WHERE [MigrationId] = N'20260909123351_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Watchlist_User_TrustId] ON [Watchlist] ([User], [TrustId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__FindInformationAcademiesTrustsContextMigrationsHistory]
    WHERE [MigrationId] = N'20260909123351_InitialCreate'
)
BEGIN
    INSERT INTO [__FindInformationAcademiesTrustsContextMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909123351_InitialCreate', N'8.0.22');
END;
GO

COMMIT;
GO

