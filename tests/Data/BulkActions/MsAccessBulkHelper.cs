using System.Data;
using Xunit;

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

            Assert.True(primaryKeysAreTheSame, "Primárne kľúče tabuliek musia byť rovnaké. " +
                $"actual = {GetPk(actualTable)}; expected = {GetPk(expectedTable)}");
        }

        private static string GetPk(DataTable table) =>
            table.PrimaryKey == null ? "null" : string.Join(", ", table.PrimaryKey.Select((col) => col.ColumnName));

        private static void CompareColumns(DataTable actualTable, DataTable expectedTable)
        {
            Assert.Equal(expectedTable.Columns.Count, actualTable.Columns.Count);

            foreach (DataColumn column in expectedTable.Columns)
            {
                Assert.NotNull(actualTable.Columns[column.ColumnName]);
            }
        }

        private static void CompareRowCounts(DataTable actualTable, DataTable expectedTable) =>
            Assert.Equal(expectedTable.Rows.Count, actualTable.Rows.Count);

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

            Assert.NotNull(actualRow);

            foreach (DataColumn column in expectedRow.Table.Columns)
            {
                string expectedValue = expectedRow[column] == DBNull.Value ? "NULL" : expectedRow[column].ToString();
                string actualValue = actualRow[column.ColumnName] == DBNull.Value ? "NULL" : actualRow[column.ColumnName].ToString();

                Assert.Equal(expectedValue, actualValue);
            }
        }

        #endregion
    }
}
