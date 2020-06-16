CREATE TABLE [dbo].[FileType] (
    [FileTypeID]          INT            NOT NULL,
    [FileTypeName]        VARCHAR (50)   NOT NULL,
    [FileTypeDescription] NVARCHAR (255) NULL,
    CONSTRAINT [PK_FileType] PRIMARY KEY CLUSTERED ([FileTypeID] ASC)
);

