CREATE TABLE [dbo].[ValidationResult] (
    [ValidationResultID]          INT            NOT NULL,
    [ValidationResultName]        VARCHAR(50) NOT NULL,
    [ValidationResultDescription] VARCHAR (255)  NULL,
    CONSTRAINT [PK_ValidationResult] PRIMARY KEY CLUSTERED ([ValidationResultID] ASC)
);

