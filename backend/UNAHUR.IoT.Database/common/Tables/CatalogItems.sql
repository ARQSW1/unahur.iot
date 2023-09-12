CREATE TABLE [common].[CatalogItems]
(
	[CatalogItemId] BIGINT IDENTITY(1,1) NOT NULL,
	[PartitionId]   INT  NOT NULL,
	[Name]			   VARCHAR(100) NOT NULL,
	[Description]      VARCHAR(255) NULL, 
	[LastModified]     DATETIME      CONSTRAINT [DF_CatalogItems_LastModified] DEFAULT (getdate()) NOT NULL,
    [LastModifiedBy]   VARCHAR (128) CONSTRAINT [DF_CatalogItems_ModifiedBy] DEFAULT (system_user) NOT NULL,
	CONSTRAINT [PK_CatalogItems] PRIMARY KEY CLUSTERED ([CatalogItemId] ASC), 
    CONSTRAINT [FK_CatalogItems_Partitions] FOREIGN KEY ([PartitionId]) REFERENCES [common].[Partitions]([PartitionId])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Nombre para mostrar',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'CatalogItems',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Partition ID',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'CatalogItems',
    @level2type = N'COLUMN',
    @level2name = N'PartitionId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique identifier',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'CatalogItems',
    @level2type = N'COLUMN',
    @level2name = N'CatalogItemId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Usuario de ultima modificacion',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'CatalogItems',
    @level2type = N'COLUMN',
    @level2name = N'LastModifiedBy'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Fecha de ultima modificacion',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'CatalogItems',
    @level2type = N'COLUMN',
    @level2name = N'LastModified'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'descripcion larga',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'CatalogItems',
    @level2type = N'COLUMN',
    @level2name = N'Description'