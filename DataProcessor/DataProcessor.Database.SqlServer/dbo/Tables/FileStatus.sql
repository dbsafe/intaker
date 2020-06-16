CREATE TABLE [dbo].[FileStatus] (
    [FileStatusID]          INT            NOT NULL,
    [FileStatusName]        VARCHAR(50) NOT NULL,
    [FileStatusDescription] NVARCHAR (255) NULL,
    CONSTRAINT [PK_FileStatus] PRIMARY KEY CLUSTERED ([FileStatusID] ASC)
);

