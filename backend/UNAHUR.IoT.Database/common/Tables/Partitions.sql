CREATE TABLE [common].[Partitions]
(
	[PartitionId]      INT  NOT NULL,
	[PartitionLevel]   HIERARCHYID  NOT NULL,
	[Name]			   VARCHAR(100) NOT NULL,
	[Description]      VARCHAR(255) NULL, 
	[LastModified]     DATETIME      CONSTRAINT [DF_Partitions_LastModified] DEFAULT (getdate()) NOT NULL,
    [LastModifiedBy]   VARCHAR (128) CONSTRAINT [DF_Partitions_CreatedBy] DEFAULT (system_user) NOT NULL,
    CONSTRAINT [PK_Partitions] PRIMARY KEY CLUSTERED ([PartitionId] ASC)
)

GO




CREATE UNIQUE INDEX [IX_Partitions_PartitionLevel] ON [common].[Partitions] ([PartitionLevel])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique identifier',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'Partitions',
    @level2type = N'COLUMN',
    @level2name = N'PartitionId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Hieracy Id of the partition',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'Partitions',
    @level2type = N'COLUMN',
    @level2name = N'PartitionLevel'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Name ',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'Partitions',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Description',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'Partitions',
    @level2type = N'COLUMN',
    @level2name = N'Description'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Last modified date',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'Partitions',
    @level2type = N'COLUMN',
    @level2name = N'LastModified'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Last modification user',
    @level0type = N'SCHEMA',
    @level0name = N'common',
    @level1type = N'TABLE',
    @level1name = N'Partitions',
    @level2type = N'COLUMN',
    @level2name = N'LastModifiedBy'