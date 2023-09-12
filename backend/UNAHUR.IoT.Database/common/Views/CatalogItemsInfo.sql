CREATE VIEW [common].[CatalogItemsInfo]
AS 

SELECT C.[CatalogItemId]
      ,C.[PartitionId]
      ,C.[Name]
      ,C.[Description]
      ,P.[PartitionLevel]

FROM [common].[CatalogItems] AS C
INNER JOIN [common].Partitions AS P on C.PartitionId =P.PartitionId

GO


