CREATE TABLE [IdStore] (
	[TableName] char(100) NOT NULL,
	[LastId] int NOT NULL
);

CREATE UNIQUE INDEX  [PK_IdStore] ON [IdStore] ([TableName] ASC) WITH PRIMARY;
