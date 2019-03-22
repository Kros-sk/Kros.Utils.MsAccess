using FluentAssertions;
using System;
using System.Data;
using System.Linq;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    public class MsAccessBulkHelper
    {
        #region DataTable compare

        public static void CompareTables(DataTable actualTable, DataTable expectedTable)
        {
            ComparePrimaryKeys(actualTable, expectedTable);
            CompareColumns(actualTable, expectedTable);
            CompareRowCounts(actualTable, expectedTable);
            CompareData(actualTable, expectedTable);
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
                    if (!actualTable.PrimaryKey[i].ColumnName.Equals(expectedTable.PrimaryKey[i].ColumnName))
                    {
                        primaryKeysAreTheSame = false;
                        break;
                    }
                }
            }

            primaryKeysAreTheSame.Should().BeTrue("Primárne kľúče tabuliek musia byť rovnaké. " +
                $"actual = {GetPk(actualTable)}; expected = {GetPk(expectedTable)}");
        }

        private static string GetPk(DataTable table) =>
            table.PrimaryKey == null ? "null" : string.Join(", ", table.PrimaryKey.Select((col) => col.ColumnName));

        private static void CompareColumns(DataTable actualTable, DataTable expectedTable)
        {
            actualTable.Columns.Count.Should().Be(expectedTable.Columns.Count,
                $"Tabuľky majú rôzny počet stĺpcov: actual = {actualTable.Columns.Count}, " +
                $"expected = {expectedTable.Columns.Count}.");

            foreach (DataColumn column in expectedTable.Columns)
            {
                actualTable.Columns[column.ColumnName].Should().NotBeNull($"Tabuľke chýba stĺpec [{column.ColumnName}].");
            }
        }

        private static void CompareRowCounts(DataTable actualTable, DataTable expectedTable) =>
            actualTable.Rows.Count.Should().Be(expectedTable.Rows.Count,
                $"Tabuľky majú rôzny počet riadkov: actual = {actualTable.Rows.Count}, " +
                $"expected = {expectedTable.Rows.Count}.");

        private static void CompareData(DataTable actualTable, DataTable expectedTable)
        {
            object[] primaryKey = new object[expectedTable.PrimaryKey.Length];

            foreach (DataRow expectedRow in expectedTable.Rows)
            {
                for (int i = 0; i < expectedTable.PrimaryKey.Length; i++)
                {
                    primaryKey[i] = expectedRow[expectedTable.PrimaryKey[i]];
                }
                CompareRowData(actualTable.Rows.Find(primaryKey), expectedRow);
            }
        }

        private static void CompareRowData(DataRow actualRow, DataRow expectedRow)
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

            actualRow.Should().NotBeNull($"V tabuľke neexistuje riadok s primárnym kľúčom \"{pk}\".");

            foreach (DataColumn column in expectedRow.Table.Columns)
            {
                string expectedValue = expectedRow[column] == DBNull.Value ? "NULL" : expectedRow[column].ToString();
                string actualValue = actualRow[column.ColumnName] == DBNull.Value ? "NULL" : actualRow[column.ColumnName].ToString();

                actualValue.Should().Be(expectedValue, $"Riadok s primárnym kľúčom \"{pk}\" nemá požadované dáta. " +
                    $"V stĺpci [{column.ColumnName}] je hodnota \"{actualValue}\", " +
                    $"očakávaná hodnota je \"{expectedValue}\".");
            }
        }

        #endregion
    }
}
