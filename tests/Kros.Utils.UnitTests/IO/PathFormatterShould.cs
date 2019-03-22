using FluentAssertions;
using FluentAssertions.Collections;
using Kros.IO;
using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace Kros.Utils.UnitTest.IO
{
    public class PathFormatterShould
    {
        [Fact]
        public void FormatPath()
        {
            var formatter = new PathFormatter();

            const string expected = @"C:\lorem\ipsum\file.txt";

            var actual = formatter.FormatPath(@"C:\lorem\ipsum", "file.txt");
            actual.Should().Be(expected);

            actual = formatter.FormatPath(@"C:\lorem\ipsum\", "file.txt");
            actual.Should().Be(expected);
        }

        [Fact]
        public void FormatPathWithAdditionalInfo()
        {
            var formatter = new PathFormatter();

            const string expected = @"C:\lorem\ipsum\file (some info).txt";

            var actual = formatter.FormatPath(@"C:\lorem\ipsum", "file.txt", "some info");
            actual.Should().Be(expected);

            actual = formatter.FormatPath(@"C:\lorem\ipsum\", "file.txt", "some info");
            actual.Should().Be(expected);
        }

        [Fact]
        public void FormatPathWithCustomOpeningAndClosingStringsInInfo()
        {
            var formatter = new PathFormatter();
            formatter.InfoOpeningString = "-,-";
            formatter.InfoClosingString = "=;=";

            const string expected = @"C:\lorem\ipsum\file -,-some info=;=.txt";

            var actual = formatter.FormatPath(@"C:\lorem\ipsum", "file.txt", "some info");
            actual.Should().Be(expected);
        }

        [Fact]
        public void FormatPathWithCustomOpeningAndClosingStringsInInfoAndCounter()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.CounterOpeningString = "=,=";
            formatter.CounterClosingString = "=;=";
            formatter.InfoOpeningString = "-,-";
            formatter.InfoClosingString = "-;-";

            int counter = 0;
            formatter.FileExists(Arg.Any<string>()).Returns(
                (callInfo) =>
                {
                    counter += 1;
                    return counter < 5;
                });

            const string expected = @"C:\lorem\ipsum\file -,-some info-;- =,=4=;=.txt";

            var actual = formatter.FormatNewPath(@"C:\lorem\ipsum", "file.txt", "some info");
            actual.Should().Be(expected);
        }

        [Fact]
        public void FormatPathWithCounterWhenFileMustNotExist()
        {
            var formatter = Substitute.For<PathFormatter>();
            int counter = 0;
            formatter.FileExists(Arg.Any<string>()).Returns(
                (callInfo) =>
                {
                    counter += 1;
                    return counter < 5;
                });

            const string expected = @"C:\lorem\ipsum\file (4).txt";

            var actual = formatter.FormatNewPath(@"C:\lorem\ipsum", "file.txt");
            actual.Should().Be(expected);
        }

        [Fact]
        public void FormatPathWithAdditionalInfoAndCounterWhenFileMustNotExist()
        {
            var formatter = Substitute.For<PathFormatter>();
            int counter = 0;
            formatter.FileExists(Arg.Any<string>()).Returns(
                (callInfo) =>
                {
                    counter += 1;
                    return counter < 5;
                });

            const string fileName = "file.txt";
            const string info = "some info";
            const string expected = @"C:\lorem\ipsum\file (some info) (4).txt";

            var actual = formatter.FormatNewPath(@"C:\lorem\ipsum", fileName, info);
            actual.Should().Be(expected);
        }

        [Fact]
        public void FormatPathAndChangeInvalidCharsInInfo()
        {
            var formatter = new PathFormatter();

            const string expected = @"C:\lorem\ipsum\file (some-info-with-invalid-chars).txt";

            var actual = formatter.FormatPath(@"C:\lorem\ipsum", "file.txt", "some*info|with?invalid<chars");
            actual.Should().Be(expected);
        }

        [Fact]
        public void FormatPathsWithoutSubfolder()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.FolderExists(Arg.Any<string>()).Returns(true);

            const string folder = @"C:\lorem\ipsum";
            const string fileName = "file.txt";

            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 2"},
                {3, "info 3"},
                {4, "info 4"}
            };
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\file (info 1).txt"},
                {2, @"C:\lorem\ipsum\file (info 2).txt"},
                {3, @"C:\lorem\ipsum\file (info 3).txt"},
                {4, @"C:\lorem\ipsum\file (info 4).txt"}
            };
            var actual = formatter.FormatPaths(folder, fileName, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void TruncatePathsWithoutSubfolderWhenPathIsTooLong()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.FolderExists(Arg.Any<string>()).Returns(true);
            formatter.MaxPathLength = 40;

            const string folder = @"C:\lorem\ipsum\"; // 15 znakov
            const string fileName = "too long filename.txt"; // 17 znakov názov súboru, 4 znaky prípona = 21 znakov

            // Nnajdlhšie info má 10 znakov vo výslednej ceste však dokopy 13 - zátvorky a medzera.
            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 22"},
                {3, "info 333"},
                {4, "info 4444"},
                {5, "info 55555"}
            };

            // Výsledná dĺžka cesty najdlhšieho súboru by bola 15 + 21 + 13 = 49 znakov.
            // Limit je 40, preto musím cestu skrátiť o 9 znakov.
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\too long (info 1).txt"},
                {2, @"C:\lorem\ipsum\too long (info 22).txt"},
                {3, @"C:\lorem\ipsum\too long (info 333).txt"},
                {4, @"C:\lorem\ipsum\too long (info 4444).txt"},
                {5, @"C:\lorem\ipsum\too long (info 55555).txt"}
            };
            var actual = formatter.FormatPaths(folder, fileName, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void FormatPathsInSubfolder()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.FolderExists(Arg.Any<string>()).Returns(false);

            const string folder = @"C:\lorem\ipsum";
            const string fileName = "file.txt";

            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 2"},
                {3, "info 3"},
                {4, "info 4"}
            };
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\file\file (info 1).txt"},
                {2, @"C:\lorem\ipsum\file\file (info 2).txt"},
                {3, @"C:\lorem\ipsum\file\file (info 3).txt"},
                {4, @"C:\lorem\ipsum\file\file (info 4).txt"}
            };
            var actual = formatter.FormatPathsInSubfolder(folder, fileName, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void FormatPathsInSubfolderWithSubfolderInfo()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.FolderExists(Arg.Any<string>()).Returns(false);

            const string folder = @"C:\lorem\ipsum";
            const string subfolderInfo = "some info";
            const string fileName = "file.txt";

            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 2"},
                {3, "info 3"},
                {4, "info 4"}
            };
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\file some info\file (info 1).txt"},
                {2, @"C:\lorem\ipsum\file some info\file (info 2).txt"},
                {3, @"C:\lorem\ipsum\file some info\file (info 3).txt"},
                {4, @"C:\lorem\ipsum\file some info\file (info 4).txt"}
            };
            var actual = formatter.FormatPathsInSubfolder(folder, fileName, subfolderInfo, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void FormatPathsInNonExistingSubfolder()
        {
            int counter = 0;
            var formatter = Substitute.For<PathFormatter>();

            formatter.FolderExists(Arg.Any<string>()).Returns(
                (x) =>
                {
                    counter += 1;
                    return counter < 5;
                });

            const string folder = @"C:\lorem\ipsum";
            const string fileName = "file.txt";

            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 2"},
                {3, "info 3"},
                {4, "info 4"}
            };
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\file (4)\file (info 1).txt"},
                {2, @"C:\lorem\ipsum\file (4)\file (info 2).txt"},
                {3, @"C:\lorem\ipsum\file (4)\file (info 3).txt"},
                {4, @"C:\lorem\ipsum\file (4)\file (info 4).txt"}
            };
            var actual = formatter.FormatPathsInSubfolder(folder, fileName, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void FormatPathsInSubfolderWithCustomOpeningAndClosingStrings()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.FolderExists(Arg.Any<string>()).Returns(false);
            formatter.InfoOpeningString = "-,-";
            formatter.InfoClosingString = "-;-";
            formatter.CounterOpeningString = "=,=";
            formatter.CounterClosingString = "=;=";

            int counter = 0;
            formatter.FolderExists(Arg.Any<string>()).Returns(
                (x) =>
                {
                    counter += 1;
                    return counter < 5;
                });

            const string folder = @"C:\lorem\ipsum";
            const string fileName = "file.txt";

            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 2"},
                {3, "info 3"},
                {4, "info 4"}
            };
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\file =,=4=;=\file -,-info 1-;-.txt"},
                {2, @"C:\lorem\ipsum\file =,=4=;=\file -,-info 2-;-.txt"},
                {3, @"C:\lorem\ipsum\file =,=4=;=\file -,-info 3-;-.txt"},
                {4, @"C:\lorem\ipsum\file =,=4=;=\file -,-info 4-;-.txt"}
            };
            var actual = formatter.FormatPathsInSubfolder(folder, fileName, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void FormatPathsInNonExistingSubfolderWithSubfolderInfo()
        {
            int counter = 0;
            var formatter = Substitute.For<PathFormatter>();

            formatter.FolderExists(Arg.Any<string>()).Returns(
                (x) =>
                {
                    counter += 1;
                    return counter < 5;
                });

            const string folder = @"C:\lorem\ipsum";
            const string SubfolderInfo = "some info";
            const string fileName = "file.txt";

            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 2"},
                {3, "info 3"},
                {4, "info 4"}
            };
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\file some info (4)\file (info 1).txt"},
                {2, @"C:\lorem\ipsum\file some info (4)\file (info 2).txt"},
                {3, @"C:\lorem\ipsum\file some info (4)\file (info 3).txt"},
                {4, @"C:\lorem\ipsum\file some info (4)\file (info 4).txt"}
            };
            var actual = formatter.FormatPathsInSubfolder(folder, fileName, SubfolderInfo, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void FormatUniquePathsWhenInfosHasDuplicates()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.FolderExists(Arg.Any<string>()).Returns(false);

            const string folder = @"C:\lorem\ipsum";
            const string fileName = "file.txt";

            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 1"},
                {3, "info 1"},
                {4, "info 2"},
                {5, "info 2"}
            };
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\file (info 1).txt"},
                {2, @"C:\lorem\ipsum\file (info 1) (1).txt"},
                {3, @"C:\lorem\ipsum\file (info 1) (2).txt"},
                {4, @"C:\lorem\ipsum\file (info 2).txt"},
                {5, @"C:\lorem\ipsum\file (info 2) (1).txt"}
            };
            var actual = formatter.FormatPaths(folder, fileName, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void FormatUniquePathsInSubfolderWhenInfosHasDuplicates()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.FolderExists(Arg.Any<string>()).Returns(false);

            const string folder = @"C:\lorem\ipsum";
            const string fileName = "file.txt";

            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 1"},
                {3, "info 1"},
                {4, "info 2"},
                {5, "info 2"}
            };
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\file\file (info 1).txt"},
                {2, @"C:\lorem\ipsum\file\file (info 1) (1).txt"},
                {3, @"C:\lorem\ipsum\file\file (info 1) (2).txt"},
                {4, @"C:\lorem\ipsum\file\file (info 2).txt"},
                {5, @"C:\lorem\ipsum\file\file (info 2) (1).txt"}
            };
            var actual = formatter.FormatPathsInSubfolder(folder, fileName, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void TruncateFileNameWhenPathIsTooLong()
        {
            var formatter = new PathFormatter();
            formatter.MaxPathLength = 25;

            const string folder = @"C:\lorem\ipsum\"; // 15 znakov
            const string fileName = "too long filename.txt"; // 17 znakov názov súboru, 4 znaky prípona
            const string expected = @"C:\lorem\ipsum\too lo.txt"; // 25 znakov - nastavené maximum

            var actual = formatter.FormatPath(folder, fileName);
            actual.Should().Be(expected);
        }

        [Fact]
        public void TruncateFileNameButNotInfoWhenPathIsTooLong()
        {
            var formatter = new PathFormatter();
            formatter.MaxPathLength = 40;

            const string folder = @"C:\lorem\ipsum\"; // 15 znakov
            const string fileName = "too long filename.txt"; // 17 znakov názov súboru, 4 znaky prípona
            const string info = "info"; // 4 znaky, ale dokopy bude 7 - zátvorky a medzera
            const string expected = @"C:\lorem\ipsum\too long filen (info).txt"; // 40 znakov - nastavené maximum

            var actual = formatter.FormatPath(folder, fileName, info);
            actual.Should().Be(expected);
        }

        [Fact]
        public void TruncateSubfolderNameAndFileNames()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.FolderExists(Arg.Any<string>()).Returns(false);
            formatter.MaxPathLength = 60;

            const string folder = @"C:\lorem\ipsum\"; // 15 znakov
            const string fileName = "too long filename.txt"; // 17 znakov názov súboru, 4 znaky prípona = 21 znakov

            // Nnajdlhšie info má 10 znakov vo výslednej ceste však dokopy 13 - zátvorky a medzera.
            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 22"},
                {3, "info 333"},
                {4, "info 4444"},
                {5, "info 55555"}
            };

            // Výsledná dĺžka cesty najdlhšieho súboru by bola 15 + 18 + 21 + 13 = 67 znakov.
            // 18 = podadresár ktorý vznikne z názvu súboru "too long filename" + lomítko.
            // Limit je 60, preto musím cestu skrátiť o 7 znakov.
            // Číslo sa rozdelí na polovicu (v tomto prípade 4) a o tento počet sa skracuje aj podadresár aj názov súboru.
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\too long file\too long file (info 1).txt"},
                {2, @"C:\lorem\ipsum\too long file\too long file (info 22).txt"},
                {3, @"C:\lorem\ipsum\too long file\too long file (info 333).txt"},
                {4, @"C:\lorem\ipsum\too long file\too long file (info 4444).txt"},
                {5, @"C:\lorem\ipsum\too long file\too long file (info 55555).txt"}
            };
            var actual = formatter.FormatPathsInSubfolder(folder, fileName, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }

        [Fact]
        public void TruncateSubfolderNameAndFileNamesButNotSubfolderInfo()
        {
            var formatter = Substitute.For<PathFormatter>();
            formatter.FolderExists(Arg.Any<string>()).Returns(false);
            formatter.MaxPathLength = 70;

            const string folder = @"C:\lorem\ipsum\"; // 15 znakov
            const string fileName = "too long filename.txt"; // 17 znakov názov súboru, 4 znaky prípona = 21 znakov
            const string subfolderInfo = "sfinfo"; // 6 znakov, ale vo výsledku bude 7 - medzera

            // Nnajdlhšie info má 10 znakov vo výslednej ceste však dokopy 13 - zátvorky a medzera.
            var fileInfos = new Dictionary<int, string>() {
                {1, "info 1"},
                {2, "info 22"},
                {3, "info 333"},
                {4, "info 4444"},
                {5, "info 55555"}
            };

            // Výsledná dĺžka cesty najdlhšieho súboru by bola 15 + 18 + 7 + 21 + 13 = 74 znakov.
            // 18 = podadresár ktorý vznikne z názvu súboru "too long filename" + lomítko.
            // Limit je 70, preto musím cestu skrátiť o 4 znaky.
            // Číslo sa rozdelí na polovicu (v tomto prípade 2) a o tento počet sa skracuje aj podadresár aj názov súboru.
            var expected = new Dictionary<int, string>() {
                {1, @"C:\lorem\ipsum\too long filena sfinfo\too long filena (info 1).txt"},
                {2, @"C:\lorem\ipsum\too long filena sfinfo\too long filena (info 22).txt"},
                {3, @"C:\lorem\ipsum\too long filena sfinfo\too long filena (info 333).txt"},
                {4, @"C:\lorem\ipsum\too long filena sfinfo\too long filena (info 4444).txt"},
                {5, @"C:\lorem\ipsum\too long filena sfinfo\too long filena (info 55555).txt"}
            };
            var actual = formatter.FormatPathsInSubfolder(folder, fileName, subfolderInfo, fileInfos);

            var comparer = new GenericDictionaryAssertions<int, string>(actual);
            comparer.Equal(expected);
        }
    }
}
