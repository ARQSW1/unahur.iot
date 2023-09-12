/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


/*
PERMISOS 
--------------------------------------------------------------------------------------
Asigna los permisso a WEB_USER
--------------------------------------------------------------------------------------
*/


DECLARE @OBJECTNAME nvarchar(128);
DECLARE @CMDEXEC nvarchar(2000);

DECLARE db_cursor CURSOR FOR  
SELECT cmd
FROM 
(
SELECT  CAST('GRANT SELECT ON  [' + o.TABLE_SCHEMA + '].[' + o.TABLE_NAME + '] TO [WEB_USER];' AS nvarchar(2000)) AS CMD 
FROM INFORMATION_SCHEMA.VIEWS o
WHERE 
o.TABLE_SCHEMA='common'
UNION
SELECT  CAST('GRANT SELECT, INSERT, UPDATE,DELETE ON [' + o.TABLE_SCHEMA + '].[' + o.TABLE_NAME + '] TO [WEB_USER];' AS nvarchar(2000)) AS CMD 
FROM INFORMATION_SCHEMA.TABLES o
WHERE 
o.TABLE_SCHEMA='common'
AND o.TABLE_TYPE != 'VIEW' 
UNION
SELECT  CAST('GRANT EXECUTE ON [' + o.SPECIFIC_SCHEMA + '].[' + o.SPECIFIC_NAME + '] TO [WEB_USER];' AS nvarchar(2000)) AS CMD 
FROM INFORMATION_SCHEMA.ROUTINES AS o 
WHERE 
o.SPECIFIC_SCHEMA='common'
AND o.ROUTINE_TYPE = 'PROCEDURE'
) as x;

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @CMDEXEC;

WHILE @@FETCH_STATUS = 0   
BEGIN
 EXEC(@CMDEXEC)
 FETCH NEXT FROM db_cursor INTO @CMDEXEC
END

CLOSE db_cursor;  
DEALLOCATE db_cursor;
GO

/*
DATOS
--------------------------------------------------------------------------------------
INSERTA DATOS BASICOS CUANDO LA BASE DE DATOS ESTA VACIA
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT TOP 1 1 FROM [common].[Partitions])
BEGIN 
  INSERT INTO [common].[Partitions]
               ([PartitionId]
               ,[PartitionLevel]
               ,[Name]
               ,[Description])
    VALUES
        (0
        ,'/'
        ,'UNAHUR'
        ,'UNIVERSIDAD NACIONAL DE HURLINGHAM');

END
IF NOT EXISTS (SELECT TOP 1 1 FROM [common].[CatalogItems])
BEGIN 
  INSERT INTO [common].[CatalogItems]
               ([PartitionId]
               ,[Name]
               ,[Description])
    VALUES
        (0
        ,'DUMMY ITEM'
        ,'DUMMY CATALOG ITEM FOR TESTING');

END


