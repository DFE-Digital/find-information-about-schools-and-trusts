-- FindInformationAcademiesTrustsContext previously tracked its migrations (the Watchlist table) in its own
-- history table, separate from FiatDbContext's [__EFMigrationsHistory]. The two contexts have been merged,
-- so copy any rows across before FiatDbMigrationScript.sql runs, otherwise it will try to create Watchlist again.
-- Safe to run repeatedly; does nothing on a new database.
IF OBJECT_ID(N'[__FindInformationAcademiesTrustsContextMigrationsHistory]') IS NOT NULL
    AND OBJECT_ID(N'[__EFMigrationsHistory]') IS NOT NULL
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    SELECT [h].[MigrationId], [h].[ProductVersion]
    FROM [__FindInformationAcademiesTrustsContextMigrationsHistory] AS [h]
    WHERE NOT EXISTS (
        SELECT * FROM [__EFMigrationsHistory] AS [e]
        WHERE [e].[MigrationId] = [h].[MigrationId]
    );
END;
GO
