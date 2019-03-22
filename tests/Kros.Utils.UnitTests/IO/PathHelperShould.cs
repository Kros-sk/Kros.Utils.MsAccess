using FluentAssertions;
using Kros.IO;
using System;
using Xunit;

namespace Kros.Utils.UnitTest.IO
{
    public class PathHelperShould
    {
        #region BuildPath

        [Fact]
        public void ThrowArgumentNullExceptionWhenInputIsNull()
        {
            Action action = () => PathHelper.BuildPath(null);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenAnyPartIsNull()
        {
            Action action = () => PathHelper.BuildPath("lorem", null, "ipsum");
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowArgumentExceptionWhenAnyPartContainsInvalidPathCharacters()
        {
            Action action = () => PathHelper.BuildPath("lorem", "ips|um");
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CombinePathParts()
        {
            PathHelper.BuildPath("lorem", "ipsum", "dolor", "sit", "amet").Should().Be(@"lorem\ipsum\dolor\sit\amet");
        }

        [Fact]
        public void CombinePathPartsWithDirectorySeparatorAtBeginning()
        {
            PathHelper.BuildPath("\\lorem", "\\ipsum", "\\dolor\\sit", "\\amet").Should().Be(@"\lorem\ipsum\dolor\sit\amet");
        }

        [Fact]
        public void CombinePathPartsWithVolumeInfo()
        {
            PathHelper.BuildPath("c:", "lorem", "ipsum", "dolor", "sit", "amet").Should().Be(@"c:\lorem\ipsum\dolor\sit\amet");
        }

        [Fact]
        public void CombinePathPartsWithVolumeInfoAndDirectorySeparator()
        {
            PathHelper.BuildPath("c:\\", "lorem", "ipsum", "dolor", "sit", "amet").Should().Be(@"c:\lorem\ipsum\dolor\sit\amet");
        }

        [Fact]
        public void InsertOnlyOneSeparatorBetweenParts()
        {
            PathHelper.BuildPath("\\lorem\\", "\\ipsum\\", "\\dolor\\", "\\sit\\", "\\amet\\")
                .Should().Be(@"\lorem\ipsum\dolor\sit\amet\");
        }

        #endregion

        #region ReplaceInvalidPathChars

        [Fact]
        public void ReturnEmptyStringWhenPathNameIsNull()
        {
            PathHelper.ReplaceInvalidPathChars(null).Should().Be(string.Empty);
        }

        [Fact]
        public void ReplaceWithEmptyStringInPathNameWhenReplacementIsNull()
        {
            PathHelper.ReplaceInvalidPathChars("a*z", null).Should().Be("az");
        }

        [Fact]
        public void ReplaceInvalidCharsInPathName()
        {
            PathHelper.ReplaceInvalidPathChars("a\\b/c*d<e>f>g").Should().Be("a-b-c-d-e-f-g");
        }

        [Fact]
        public void ReplaceInvalidCharGroupsWithSingleReplacementInPathName()
        {
            PathHelper.ReplaceInvalidPathChars("a\\/*b<>c").Should().Be("a-b-c");
        }

        [Fact]
        public void ReplaceInvalidCharsInPathNameWithCustomReplacement()
        {
            PathHelper.ReplaceInvalidPathChars("a\\b/c*d<e>f>g", "=").Should().Be("a=b=c=d=e=f=g");
        }

        #endregion

        #region GetTempPath

        [Fact]
        public void GetTempPathDoesntEndWithSlash()
        {
            string systemTempPath = System.IO.Path.GetTempPath();
            string expected = systemTempPath.Remove(systemTempPath.Length - 1, 1);
            string actual = PathHelper.GetTempPath();

            actual.Should().Be(expected);
        }

        #endregion
    }
}
