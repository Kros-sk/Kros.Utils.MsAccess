# Kros.Utils.MsAccess [![Build Status](https://dev.azure.com/krossk/DevShared/_apis/build/status/Kros.Utils/Kros.Utils.MsAccess?branchName=features/buildAndPush)](https://dev.azure.com/krossk/DevShared/_build/latest?definitionId=65&branchName=features/buildAndPush)

__Kros.Utils.MsAccess__ is a general library of various utilities to simplify the work of a programmer with Microsoft Access databases.

For some (especially database) stuff to work properly, the library needs to be initialized when the program starts by calling [LibraryInitializer.InitLibrary](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.Utils.MsAccess.LibraryInitializer.html#Kros_Utils_MsAccess_LibraryInitializer_InitLibrary "LibraryInitializer InitLibrary").

Library is compiled for .NET Framework 4.7.

## Documentation

For configuration, general information and examples [see the documentation](https://kros-sk.github.io/docs/Kros.Utils.MsAccess/).

## Download

Kros.Libs is available from __Nuget__ [__Kros.Utils.MsAccess__](https://www.nuget.org/packages/Kros.Utils.MsAccess/)

## Contributing Guide

To contribute with new topics/information or make changes, see [contributing](https://github.com/Kros-sk/Kros.Utils.MsAccess/blob/master/CONTRIBUTING.md) for instructions and guidelines.

## This topic contains following sections

__Kros.Utils.MsAccess__

* [General Utilities](#msaccess-general-utilities)
* [Database Schema](#msaccess-database-schema)
* [Bulk Operations - Bulk Insert and Bulk Update](#msaccess-bulk-operations---bulk-insert-and-bulk-update)
* [Unit Testing Helpers](#msaccess-unit-testing-helpers)

### MsAccess General Utilities

The [MsAccessDataHelper](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.Data.MsAccess.MsAccessDataHelper.html "MsAccessDataHelper") class contains general utilities for working with the MS Access database connection.

* Retrieve current MS Access provider: [MsAccessProvider](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.Data.MsAccess.MsAccessDataHelper.html#Kros_Data_MsAccess_MsAccessDataHelper_MsAccessAceProvider "MsAccessProvider")
* Determining whether the connection to the MS Access database is exclusive: [IsExclusiveMsAccessConnection](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.Data.MsAccess.MsAccessDataHelper.html#Kros_Data_MsAccess_MsAccessDataHelper_IsExclusiveMsAccessConnection_System_Data_IDbConnection_ "IsExclusiveMsAccessConnection")
* Determining whether the connection is a connection to the MS Access database: [IsMsAccessConnection](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.Data.MsAccess.MsAccessDataHelper.html#Kros_Data_MsAccess_MsAccessDataHelper_IsMsAccessConnection_System_Data_IDbConnection_ "IsMsAccessConnection")

### MsAccess Database Schema

It is very easy to get a database schema. Since the acquisition of the schema is a time-consuming operation the loaded scheme is held in a cache and the next schema is retrieved. The database schema includes the [TableSchema](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.Schema.TableSchema.html "TableSchema") tables, their [ColumnSchema](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.Schema.ColumnSchema.html "ColumnSchema") columns and [IndexSchema](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.Schema.IndexSchema.html "IndexSchema") indexes.

```c#
OleDbConnection cn = new OleDbConnection("MS Access Connection String");

DatabaseSchema schema = DatabaseSchemaLoader.Default.LoadSchema(cn);
```

### MsAccess Bulk Operations - Bulk Insert and Bulk Update

Inserting (`INSERT`) and updating (`UPDATE`) large amounts of data in a database are time-consuming. Therefore, support for rapid mass insertion, `Bulk Insert` and a fast bulk update, `Bulk Update`. The [IBulkInsert](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.IBulkInsert.html "IBulkInsert") and [IBulkUpdate](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.IBulkUpdate.html "IBulkUpdate") interfaces are used. They are implemented for MsAccess database in the [MsAccessBulkInsert](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.Data.BulkActions.MsAccess.MsAccessBulkInsert.html "MsAccessBulkInsert") and [MsAccessBulkUpdate](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.Data.BulkActions.MsAccess.MsAccessBulkUpdate.html "MsAccessBulkUpdate") classes. As a data source, it serves any [IDataReader](https://msdn.microsoft.com/en-us/library/sh674a6a "IDataReader") or [DataTable](https://msdn.microsoft.com/en-us/library/9186hy08 "DataTable") table.

Because `IDataReader` is an intricate interface, you just need to implement the simplier interface [IBulkActionDataReader](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.IBulkActionDataReader.html "IBulkActionDataReader"). If the source is a list (`IEnumerable`), it is sufficient to use the [`EnumerableDataReader<T>`](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils/Kros.Data.BulkActions.EnumerableDataReader-1.html "EnumerableDataReader<T>") class for its bulk insertion.

```c#
private class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public void InsertManyItems()
{
    IEnumerable<Item> data = GetData();

    using (var reader = new EnumerableDataReader<Item>(data, new string[] { "Id", "Name" }))
    {
        using (var bulkInsert = new MsAccessBulkInsert("connection string"))
        {
            bulkInsert.Insert(reader);
        }
    }
}
```

### MsAccess Unit Testing Helpers

Standard unit tests should be database-independent. But sometimes it is necessary to test the actual database because the test items are directly related to it. To test the actual database you can use the [MsAccessTestHelper](https://kros-sk.github.io/Kros.Libs.Documentation/api/Kros.Utils.MsAccess/Kros.UnitTests.MsAccessTestHelper.html "MsAccessTestHelper") class. It creates a database for testing purposes on the server and runs tests over it. When tests are finished the database is deleted.

```c#
private const string BaseDatabasePath = "C:\testfiles\testdatabase.accdb";

private const string CreateTestTableScript =
@"CREATE TABLE [TestTable] (
    [Id] number NOT NULL,
    [Name] text(255) NULL,

    CONSTRAINT [PK_TestTable] PRIMARY KEY ([Id])
)";

[Fact]
public void DoSomeTestWithDatabase()
{
    using (var helper = new MsAccessTestHelper(ProviderType.Ace, BaseDatabasePath, CreateTestTableScript))
    {
        // Do tests with connection helper.Connection.
    }
}
```
