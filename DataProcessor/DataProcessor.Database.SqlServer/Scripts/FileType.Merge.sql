
MERGE INTO [dbo].[FileType] AS target
USING (VALUES
	(1, 'File-Type-A', ''),
	(2, 'File-Type-B', '')
) 
AS source (FileTypeID, FileTypeName, FileTypeDescription)
ON target.FileTypeID = source.FileTypeID

WHEN MATCHED THEN
	UPDATE SET FileTypeName = source.FileTypeName, FileTypeDescription = source.FileTypeDescription
	
WHEN NOT MATCHED BY TARGET THEN
	INSERT (FileTypeID, FileTypeName, FileTypeDescription)
	VALUES (FileTypeID, FileTypeName, FileTypeDescription)

WHEN NOT MATCHED BY SOURCE THEN
	DELETE;