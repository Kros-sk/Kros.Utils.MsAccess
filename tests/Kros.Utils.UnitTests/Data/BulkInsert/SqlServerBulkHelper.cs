using FluentAssertions;
using System;
using System.Data;
using System.Linq;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    public class SqlServerBulkHelper
    {
        #region DataTable compare

        public static void CompareTables(DataTable actualTable, DataTable expectedTable)
        {
            ComparePrimaryKeys(actualTable, expectedTable);
            CompareColumns(actualTable, expectedTable);
            CompareRowCounts(actualTable, expectedTable);
            CompareData(actualTable, expectedTable);
        }

        public static void CompareColumnValues(DataTable actualTable, DataTable expectedTable, string columnName)
        {
            actualTable.Columns[columnName].Should().NotBeNull($"Missing column [{columnName}].");

            actualTable.Rows.Count.Should().Be(expectedTable.Rows.Count,
                $"Tables have different number of columns: actual = {actualTable.Rows.Count}, " +
                $"expected = {expectedTable.Rows.Count}.");

            foreach (DataRow actual in expectedTable.Rows)
            {
                CompareRowOneColumn(actualTable, expectedTable, actual, columnName);
            }
        }

        private static void ComparePrimaryKeys(DataTable actualTable, DataTable expectedTable)
        {
            bool primaryKeysAreTheSame = false;

            if ((actualTable.PrimaryKey != null) && (expectedTable.PrimaryKey != null) &&
                (actualTable.PrimaryKey.Length == expectedTable.PrimaryKey.Length))
            {
                primaryKeysAreTheSame = true;
                for (int i = 0; i < actualTable.PrimaryKey.Length; i++)
                {
                    if (!actualTable.PrimaryKey[i].ColumnName.Equals(
                        expectedTable.PrimaryKey[i].ColumnName, StringComparison.OrdinalIgnoreCase))
                    {
                        primaryKeysAreTheSame = false;
                        break;
                    }
                }
            }

            primaryKeysAreTheSame.Should().BeTrue("Tables primary keys must be equal. " +
                $"actual = {GetPk(actualTable)}; expected = {GetPk(expectedTable)}");
        }

        private static string GetPk(DataTable table) =>
            table.PrimaryKey == null ? "null" : string.Join(", ", table.PrimaryKey.Select((col) => col.ColumnName));

        private static void CompareColumns(DataTable actualTable, DataTable expectedTable)
        {
            actualTable.Columns.Count.Should().Be(expectedTable.Columns.Count,
                $"Tables have different number of columns: actual = {actualTable.Columns.Count}, " +
                $"expected = {expectedTable.Columns.Count}.");

            foreach (DataColumn column in expectedTable.Columns)
            {
                actualTable.Columns[column.ColumnName].Should().NotBeNull($"Missing column [{column.ColumnName}].");
            }
        }

        private static void CompareRowCounts(DataTable actualTable, DataTable expectedTable) =>
            actualTable.Rows.Count.Should().Be(expectedTable.Rows.Count,
                $"Tables have different number of rows: actual = {actualTable.Rows.Count}, " +
                $"expected = {expectedTable.Rows.Count}.");

        private static void CompareData(DataTable actualTable, DataTable expectedTable)
        {
            foreach (DataRow expectedRow in expectedTable.Rows)
            {
                CompareRowOneColumn(actualTable, expectedTable, expectedRow);
            }
        }

        public static void CompareRowOneColumn(DataTable actualTable, DataTable expectedTable, DataRow expectedRow)
            => CompareRowOneColumn(actualTable, expectedTable, expectedRow, string.Empty);

        public static void CompareRowOneColumn(
            DataTable actualTable,
            DataTable expectedTable,
            DataRow expectedRow,
            string columnName)
        {
            object[] primaryKey = new object[expectedTable.PrimaryKey.Length];

            for (int i = 0; i < expectedTable.PrimaryKey.Length; i++)
            {
                primaryKey[i] = expectedRow[expectedTable.PrimaryKey[i]];
            }
            CompareRowData(actualTable.Rows.Find(primaryKey), expectedRow, columnName);
        }

        private static void CompareRowData(DataRow actualRow, DataRow expectedRow, string columnName)
        {
            string pk = string.Empty;
            for (int i = 0; i < expectedRow.Table.PrimaryKey.Length; i++)
            {
                if (!string.IsNullOrEmpty(pk))
                {
                    pk += ", ";
                }
                pk += expectedRow[expectedRow.Table.PrimaryKey[i]].ToString();
            }

            actualRow.Should().NotBeNull($"Table does not contain row with primary key \"{pk}\".");

            foreach (DataColumn column in expectedRow.Table.Columns)
            {
                if (string.IsNullOrEmpty(columnName) || columnName.Equals(column.ColumnName, StringComparison.OrdinalIgnoreCase))
                {
                    object expectedValue = expectedRow[column] == DBNull.Value ? "NULL" : expectedRow[column];
                    object actualValue = actualRow[column.ColumnName] == DBNull.Value ? "NULL" : actualRow[column.ColumnName];

                    actualValue.Should().Be(expectedValue, $"Row with primary key \"{pk}\" does not contain expected data. " +
                        $"In column [{column.ColumnName}] is expected value \"{expectedValue}\", " +
                        $"but found value \"{actualValue}\".");
                }
            }
        }

        #endregion
    }
}
