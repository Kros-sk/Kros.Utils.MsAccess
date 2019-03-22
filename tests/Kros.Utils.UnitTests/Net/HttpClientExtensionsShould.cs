using FluentAssertions;
using Kros.Net;
using System.Collections.Generic;
using Xunit;

namespace Kros.Utils.UnitTests.Net
{
    public class HttpClientExtensionsShould
    {
        #region Nested types

        private class TestData
        {
            private string LastName { get; set; } = "Lorem ipsum";
            public string Name { get; set; }
            public int Age { get; set; }
            public List<string> Children { get; } = new List<string>();

            public string this[int index]
            {
                get => index.ToString();
                set { }
            }
        }

        #endregion

        [Fact]
        public void CreateCorrectFormPostData()
        {
            var data = new TestData()
            {
                Name = "Gabriel",
                Age = 40
            };
            data.Children.AddRange(new[] { "value 1", "value 2", "value 3" });

            var expectedData = new List<KeyValuePair<string, string>>(new[]
            {
                new KeyValuePair<string, string>("Name", "Gabriel"),
                new KeyValuePair<string, string>("Age", "40"),
                new KeyValuePair<string, string>("Children", "value 1"),
                new KeyValuePair<string, string>("Children", "value 2"),
                new KeyValuePair<string, string>("Children", "value 3"),
            });

            List<KeyValuePair<string, string>> actualData = HttpClientExtensions.CreateFormPostData(data);
            actualData.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public void CreateCorrectFormPostDataWithAntiForgeryToken()
        {
            var data = new TestData()
            {
                Name = "Gabriel",
                Age = 40
            };
            data.Children.AddRange(new[] { "value 1", "value 2", "value 3" });

            var expectedData = new List<KeyValuePair<string, string>>(new[]
            {
                new KeyValuePair<string, string>("Name", "Gabriel"),
                new KeyValuePair<string, string>("Age", "40"),
                new KeyValuePair<string, string>("Children", "value 1"),
                new KeyValuePair<string, string>("Children", "value 2"),
                new KeyValuePair<string, string>("Children", "value 3"),
                new KeyValuePair<string, string>(HttpClientExtensions.AntiForgeryTokenFieldName, "anti-forgery-token"),
            });

            List<KeyValuePair<string, string>> actualData = HttpClientExtensions.CreateFormPostData(data, "anti-forgery-token");
            actualData.Should().BeEquivalentTo(expectedData);
        }
    }
}
