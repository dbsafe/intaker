CREATE TABLE [dbo].[User] (
    [UserID]   INT            NOT NULL,
    [UserName] NVARCHAR (150) NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_User]
    ON [dbo].[User]([UserName] ASC);

