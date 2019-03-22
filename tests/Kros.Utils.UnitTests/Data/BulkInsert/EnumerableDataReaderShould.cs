using FluentAssertions;
using Kros.Data.BulkActions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    public class EnumerableDataReaderShould
    {
        #region Nested types

        private class DataItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
        }

        #endregion

        #region Tests

        [Fact]
        public void ThrowArgumentNullExceptionWhenDataIsNull()
        {
            Action createInstance = () =>
            {
                var instance = new EnumerableDataReader<DataItem>(null, new string[] { "Id" });
            };
            createInstance.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("data");
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenColumnNamesIsNull()
        {
            Action createInstance = () =>
            {
                var instance = new EnumerableDataReader<DataItem>(new List<DataItem>(), null);
            };
            createInstance.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("columnNames");
        }

        [Fact]
        public void ThrowArgumentExceptionWhenColumnNamesIsEmpty()
        {
            Action createInstance = () =>
            {
                var instance = new EnumerableDataReader<DataItem>(new List<DataItem>(), new string[] { });
            };
            createInstance.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("columnNames");
        }

        [Fact]
        public void ThrowInvalidOperationExceptionWhenColumnNameIsInvalid()
        {
            const string invalidColumn = "Lorem";

            Action createInstance = () =>
            {
                var instance = new EnumerableDataReader<DataItem>(new List<DataItem>(), new string[] { "Id", invalidColumn });
            };
            createInstance.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{typeof(DataItem).FullName}*{invalidColumn}*");
        }

        [Fact]
        public void CreateInstanceCorrectly()
        {
            var reader = new EnumerableDataReader<DataItem>(new List<DataItem>(), new string[] { "Id", "Name" });
            reader.GetOrdinal("Id").Should().Be(0, "Id column must have ordinal 0.");
            reader.GetOrdinal("Name").Should().Be(1, "Name column must have ordinal 1.");
            reader.GetName(0).Should().Be("Id", "Column at index 0 must be Id.");
            reader.GetName(1).Should().Be("Name", "Column at index 1 must be Name.");
        }

        [Fact]
        public void ReturnCorrectData()
        {
            var item1 = new DataItem() { Id = 1, Name = "Item 1" };
            var item2 = new DataItem() { Id = 2, Name = "Item 2" };
            var data = new List<DataItem>(new DataItem[] { item1, item2 });

            using (var reader = new EnumerableDataReader<DataItem>(new List<DataItem>(), new string[] { "Id", "Name" }))
            {
                int itemIndex = 0;
                while (reader.Read())
                {
                    reader.GetValue(0).Should().Be(data[itemIndex].Id);
                    reader.GetValue(1).Should().Be(data[itemIndex].Name);
                    itemIndex++;
                }
            }
        }

        #endregion
    }
}
