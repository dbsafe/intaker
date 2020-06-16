
MERGE INTO [dbo].[FileStatus] AS target
USING 
(VALUES
	(1, 'Loading', 'A process is loading the file'),
	(2, 'Loaded', 'A loading process completed'),
	(3, 'LoadingError', 'A loading process completed')
) AS source (FileStatusID, FileStatusName, FileStatusDescription)
ON target.FileStatusID = source.FileStatusID

WHEN MATCHED THEN
	UPDATE SET FileStatusName = source.FileStatusName, FileStatusDescription = source.FileStatusDescription
	
WHEN NOT MATCHED BY TARGET THEN
	INSERT (FileStatusID, FileStatusName, FileStatusDescription)
	VALUES (FileStatusID, FileStatusName, FileStatusDescription)

WHEN NOT MATCHED BY SOURCE THEN
	DELETE;