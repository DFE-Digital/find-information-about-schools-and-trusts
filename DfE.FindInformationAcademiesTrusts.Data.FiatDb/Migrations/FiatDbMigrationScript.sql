IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240918134511_InitialCreate'
)
BEGIN
    DECLARE @historyTableSchema sysname = SCHEMA_NAME()
    EXEC(N'CREATE TABLE [Contacts] (
        [Id] int NOT NULL IDENTITY,
        [Uid] int NOT NULL,
        [Role] nvarchar(450) NOT NULL,
        [Name] nvarchar(500) NOT NULL,
        [Email] nvarchar(320) NOT NULL,
        [PeriodEnd] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
        [PeriodStart] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
        [LastModifiedByName] nvarchar(500) NOT NULL,
        [LastModifiedByEmail] nvarchar(320) NOT NULL,
        [LastModifiedAtTime] AS [PeriodStart],
        CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id]),
        PERIOD FOR SYSTEM_TIME([PeriodStart], [PeriodEnd])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[ContactsHistory]))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240918134511_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Contacts_Uid] ON [Contacts] ([Uid]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240918134511_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Contacts_Uid_Role] ON [Contacts] ([Uid], [Role]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240918134511_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240918134511_InitialCreate', N'8.0.22');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250605162516_CreateSchoolContactEntity'
)
BEGIN
    DECLARE @historyTableSchema sysname = SCHEMA_NAME()
    EXEC(N'CREATE TABLE [SchoolContacts] (
        [Id] int NOT NULL IDENTITY,
        [Urn] int NOT NULL,
        [Role] nvarchar(450) NOT NULL,
        [Name] nvarchar(500) NOT NULL,
        [Email] nvarchar(320) NOT NULL,
        [PeriodEnd] datetime2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
        [PeriodStart] datetime2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
        [LastModifiedByName] nvarchar(500) NOT NULL,
        [LastModifiedByEmail] nvarchar(320) NOT NULL,
        [LastModifiedAtTime] AS [PeriodStart],
        CONSTRAINT [PK_SchoolContacts] PRIMARY KEY ([Id]),
        PERIOD FOR SYSTEM_TIME([PeriodStart], [PeriodEnd])
    ) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + N'].[SchoolContactsHistory]))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250605162516_CreateSchoolContactEntity'
)
BEGIN
    CREATE INDEX [IX_SchoolContacts_Urn] ON [SchoolContacts] ([Urn]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250605162516_CreateSchoolContactEntity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SchoolContacts_Urn_Role] ON [SchoolContacts] ([Urn], [Role]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250605162516_CreateSchoolContactEntity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250605162516_CreateSchoolContactEntity', N'8.0.22');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
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
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909123351_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Watchlist_User_EstablishmentId] ON [Watchlist] ([User], [EstablishmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909123351_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Watchlist_User_TrustId] ON [Watchlist] ([User], [TrustId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909123351_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909123351_InitialCreate', N'8.0.22');
END;
GO

COMMIT;
GO

