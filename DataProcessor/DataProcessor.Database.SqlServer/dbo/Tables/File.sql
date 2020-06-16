CREATE TABLE [dbo].[File] (
    [FileID]             BIGINT          IDENTITY (1, 1) NOT NULL,
    [Path]               NVARCHAR (1000) NOT NULL,
    [FileStatusID]       INT             NOT NULL,
    [ValidationResultID] INT             NOT NULL,
    [CustomData]         VARCHAR (MAX)   NOT NULL,
    [CreatedOn]          DATETIME2 (7)   NOT NULL,
    [CreatedBy]          INT             NOT NULL,
    [LastModifiedOn]     DATETIME2 (7)   NOT NULL,
    [LastModifiedBy]     INT             NOT NULL,
    CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([FileID] ASC),
    CONSTRAINT [FK_File_User_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[User] ([UserID]),
    CONSTRAINT [FK_File_User_LastModifiedBy] FOREIGN KEY ([LastModifiedBy]) REFERENCES [dbo].[User] ([UserID])
);

