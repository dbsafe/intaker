
MERGE INTO [dbo].[ValidationResult] AS target
USING 
(VALUES
	(1, 'Valid', 'File, Row, or Field is valid'),
	(2, 'InvalidFixable', 'File, Row, or Field is not valid but is fixable'),
	(3, 'InvalidCritical', 'File, Row, or Field is not valid and not fixable')
) AS source (ValidationResultID, ValidationResultName, ValidationResultDescription)
ON target.ValidationResultID = source.ValidationResultID

WHEN MATCHED THEN
	UPDATE SET ValidationResultName = source.ValidationResultName, ValidationResultDescription = source.ValidationResultDescription
	
WHEN NOT MATCHED BY TARGET THEN
	INSERT (ValidationResultID, ValidationResultName, ValidationResultDescription)
	VALUES (ValidationResultID, ValidationResultName, ValidationResultDescription)

WHEN NOT MATCHED BY SOURCE THEN
	DELETE;