using FluentAssertions;
using System;
using Xunit;

namespace Kros.Utils.UnitTests.Utils
{
    public class CheckShould
    {
        #region Nested types

        private enum DummyEnum
        {
            Value1,
            Value2,
            Value3,
            Value4,
            Value5
        }

        private class DummyClass : IComparable<DummyClass>
        {
            public int Id { get; set; }
            public string Text { get; set; }

            public int CompareTo(DummyClass other)
            {
                return Id.CompareTo(other.Id);
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() == typeof(DummyClass))
                {
                    return (obj as DummyClass).Id == this.Id;
                }
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private class DummyClass2
        {
            public int Id { get; set; }
            public string Text { get; set; }
        }

        #endregion

        #region Object

        [Fact]
        public void ThrowArgumentNullExceptionForNullableValueTypeWithoutValue()
        {
            int? value = null;
            Action action = () => Check.NotNull(value, nameof(value));
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NotThrowExceptionForNullableValueTypeWithValue()
        {
            int? value = 123;
            Action action = () => Check.NotNull(value, nameof(value));
            action.Should().NotThrow();
        }

        [Fact]
        public void NotThrowExceptionForNullableValueTypeWithDefaultValue()
        {
            int? value = default(int);
            Action action = () => Check.NotNull(value, nameof(value));
            action.Should().NotThrow();
        }

        [Fact]
        public void NotThrowExceptionForValueTypeWithDefaultValue()
        {
            int value = default(int);
            Action action = () => Check.NotNull(value, nameof(value));
            action.Should().NotThrow();
        }

        [Fact]
        public void NotThrowExceptionForValueTypeWithValue()
        {
            int value = 123;
            Action action = () => Check.NotNull(value, nameof(value));
            action.Should().NotThrow();
        }

        [Fact]
        public void ThrowArgumentNullExceptionForReferenceType()
        {
            string value = null;
            Action action = () => Check.NotNull(value, nameof(value));
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowArgumentNullExceptionWithParamName()
        {
            const string paramName = "arg";
            Action action = () => Check.NotNull((object)null, paramName);
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(paramName);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWithParamNameAndMessage()
        {
            const string paramName = "arg";
            const string message = "Exception message.*";
            Action action = () => Check.NotNull((object)null, paramName, message);
            action.Should().Throw<ArgumentNullException>()
                .WithMessage(message)
                .And.ParamName.Should().Be(paramName);
        }

        #endregion

        #region Type

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenInvalidTypeGeneric()
        {
            const string paramName = "arg";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsOfType<DummyClass2>(param, paramName);
            action.Should().Throw<ArgumentException>()
                .WithMessage("*" + typeof(DummyClass2).FullName + "*" + param.GetType().FullName + "*")
                .And.ParamName.Should().Be(paramName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenInvalidTypeGeneric()
        {
            const string paramName = "arg";
            const string message = "Exception message.*";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsOfType<DummyClass2>(param, paramName, message);
            action.Should().Throw<ArgumentException>()
                .WithMessage(message)
                .And.ParamName.Should().Be(paramName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenInvalidType()
        {
            const string paramName = "arg";
            DummyClass param = new DummyClass();
            string expectedTypeName = typeof(DummyClass2).FullName;
            string argTypeName = param.GetType().FullName;
            Action action = () => Check.IsOfType(param, typeof(DummyClass2), paramName);
            action.Should().Throw<ArgumentException>()
                .WithMessage("*" + typeof(DummyClass2).FullName + "*" + param.GetType().FullName + "*")
                .And.ParamName.Should().Be(paramName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenInvalidType()
        {
            const string paramName = "arg";
            const string message = "Exception message.*";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsOfType(param, typeof(DummyClass2), paramName, message);
            action.Should().Throw<ArgumentException>()
                .WithMessage(message)
                .And.ParamName.Should().Be(paramName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenNotExpectedTypeGeneric()
        {
            const string paramName = "arg";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsNotOfType<DummyClass>(param, paramName);
            action.Should().Throw<ArgumentException>()
                .WithMessage("*" + typeof(DummyClass).FullName + "*")
                .And.ParamName.Should().Be(paramName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenNotExpectedTypeGeneric()
        {
            const string paramName = "arg";
            const string message = "Exception message.*";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsNotOfType<DummyClass>(param, paramName, message);
            action.Should().Throw<ArgumentException>()
                .WithMessage(message)
                .And.ParamName.Should().Be(paramName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenNotExpectedType()
        {
            const string paramName = "arg";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsNotOfType(param, typeof(DummyClass), paramName);
            action.Should().Throw<ArgumentException>()
                .WithMessage("*" + typeof(DummyClass).FullName + "*")
                .And.ParamName.Should().Be(paramName);
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenNotExpectedType()
        {
            const string paramName = "arg";
            const string message = "Exception message.*";
            DummyClass param = new DummyClass();
            Action action = () => Check.IsNotOfType(param, typeof(DummyClass), paramName, message);
            action.Should().Throw<ArgumentException>()
                .WithMessage(message)
                .And.ParamName.Should().Be(paramName);
        }

        #endregion

        #region String

        [Fact]
        public void ThrowArgumentNullExceptionWithParamNameWhenNullString()
        {
            Action action = () => Check.NotNullOrEmpty(null, "arg");
            action.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenEmptyString()
        {
            Action action = () => Check.NotNullOrEmpty(string.Empty, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentNullExceptionWithParamNameAndMessageWhenNullString()
        {
            Action action = () => Check.NotNullOrEmpty(null, "arg", "Exception message.");
            action.Should().Throw<ArgumentNullException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenEmptyString()
        {
            Action action = () => Check.NotNullOrEmpty(string.Empty, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentNullExceptionWithParamNameWhenWhiteSpaceString()
        {
            Action action = () => Check.NotNullOrWhiteSpace(" \t \r \n ", "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenWhiteSpaceString()
        {
            Action action = () => Check.NotNullOrWhiteSpace(" \t \r \n ", "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        #endregion

        #region Values

        #region Equal

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotEqual()
        {
            Action action = () => Check.Equal(1, 2, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotEqual()
        {
            Action action = () => Check.Equal(1, 2, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotEqual()
        {
            DummyClass value1 = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.Equal(value1, value2, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotEqual()
        {
            DummyClass value1 = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.Equal(value1, value2, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        #endregion

        #region Not equal

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueEquals()
        {
            Action action = () => Check.NotEqual(2, 2, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueEquals()
        {
            Action action = () => Check.NotEqual(2, 2, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueEquals()
        {
            DummyClass value1 = new DummyClass() { Id = 2 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.NotEqual(value1, value2, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueEquals()
        {
            DummyClass value1 = new DummyClass() { Id = 2 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.NotEqual(value1, value2, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        #endregion

        #region LessThan

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotLessThanExpected()
        {
            Action action = () => Check.LessThan(1, 1, "arg");
            action.Should().Throw<ArgumentException>("LessThan: 1, 1")
                .And.ParamName.Should().Be("arg");

            action = () => Check.LessThan(2, 1, "arg");
            action.Should().Throw<ArgumentException>("LessThan: 2, 1")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotLessThanExpected()
        {
            Action action = () => Check.LessThan(1, 1, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>("LessThan: 1, 1")
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");

            action = () => Check.LessThan(2, 1, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>("LessThan: 2, 1")
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotLessThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.LessThan(value1a, value1b, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");

            action = () => Check.LessThan(value2, value1a, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotLessThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.LessThan(value1a, value1b, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");

            action = () => Check.LessThan(value2, value1a, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        #endregion

        #region LessOrEqualThan

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotLessOrEqualThanExpected()
        {
            Action action = () => Check.LessOrEqualThan(1, 1, "arg");
            action.Should().NotThrow("LessOrEqualThan: 1, 1");

            action = () => Check.LessOrEqualThan(2, 1, "arg");
            action.Should().Throw<ArgumentException>("LessOrEqualThan: 2, 1")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotLessOrEqualThanExpected()
        {
            Action action = () => Check.LessOrEqualThan(1, 1, "arg", "Exception message.");
            action.Should().NotThrow("LessOrEqualThan: 1, 1");

            action = () => Check.LessOrEqualThan(2, 1, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>("LessOrEqualThan: 2, 1")
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotLessOrEqualThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.LessOrEqualThan(value1a, value1b, "arg");
            action.Should().NotThrow<ArgumentException>();

            action = () => Check.LessOrEqualThan(value2, value1a, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotLessOrEqualThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.LessOrEqualThan(value1a, value1b, "arg", "Exception message.");
            action.Should().NotThrow<ArgumentException>();

            action = () => Check.LessOrEqualThan(value2, value1a, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        #endregion

        #region GreaterThan

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotGreaterThanExpected()
        {
            Action action = () => Check.GreaterThan(1, 1, "arg");
            action.Should().Throw<ArgumentException>("GreaterThan: 1, 1")
                .And.ParamName.Should().Be("arg");

            action = () => Check.GreaterThan(1, 2, "arg");
            action.Should().Throw<ArgumentException>("GreaterThan: 2, 1")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotGreaterThanExpected()
        {
            Action action = () => Check.GreaterThan(1, 1, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>("GreaterThan: 1, 1")
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");

            action = () => Check.GreaterThan(1, 2, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>("GreaterThan: 2, 1")
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotGreaterThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.GreaterThan(value1a, value1b, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");

            action = () => Check.GreaterThan(value1a, value2, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotGreaterThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.GreaterThan(value1a, value1b, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");

            action = () => Check.GreaterThan(value1a, value2, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        #endregion

        #region GreaterOrEqualThan

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenPrimitiveValueNotGreaterOrEqualThanExpected()
        {
            Action action = () => Check.GreaterOrEqualThan(1, 1, "arg");
            action.Should().NotThrow("GreaterOrEqualThan: 1, 1");

            action = () => Check.GreaterOrEqualThan(1, 2, "arg");
            action.Should().Throw<ArgumentException>("GreaterOrEqualThan: 2, 1")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenPrimitiveValueNotGreaterOrEqualThanExpected()
        {
            Action action = () => Check.GreaterOrEqualThan(1, 1, "arg", "Exception message.");
            action.Should().NotThrow("GreaterOrEqualThan: 1, 1");

            action = () => Check.GreaterOrEqualThan(1, 2, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>("GreaterOrEqualThan: 2, 1")
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenClassValueNotGreaterOrEqualThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.GreaterOrEqualThan(value1a, value1b, "arg");
            action.Should().NotThrow<ArgumentException>();

            action = () => Check.GreaterOrEqualThan(value1a, value2, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenClassValueNotGreaterOrEqualThanExpected()
        {
            DummyClass value1a = new DummyClass() { Id = 1 };
            DummyClass value1b = new DummyClass() { Id = 1 };
            DummyClass value2 = new DummyClass() { Id = 2 };

            Action action = () => Check.GreaterOrEqualThan(value1a, value1b, "arg", "Exception message.");
            action.Should().NotThrow<ArgumentException>();

            action = () => Check.GreaterOrEqualThan(value1a, value2, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        #endregion

        #region IsInList

        [Fact]
        public void ThrowArgumentExceptionWhenEnumArgumentIsNotInListWithParamName()
        {
            DummyEnum value = DummyEnum.Value1;
            Action action = () => Check.IsInList(value, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 }, "arg");
            action.Should().Throw<ArgumentException>()
                .WithMessage($"*Value1*Value2, Value3*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWhenEnumArgumentIsNotInListWithParamNameAndCustomMessage()
        {
            DummyEnum value = DummyEnum.Value1;
            Action action = () => Check.IsInList(value, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 }, "arg", "msg");
            action.Should().Throw<ArgumentException>()
                .WithMessage("msg*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenEnumArgumentIsInListWithParamName()
        {
            DummyEnum value = DummyEnum.Value3;
            Action action = () => Check.IsInList(value, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 }, "arg");
            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenEnumArgumentIsInListWithParamNameAndCustomMessage()
        {
            DummyEnum value = DummyEnum.Value3;
            Action action = () => Check.IsInList(value, new DummyEnum[] { DummyEnum.Value2, DummyEnum.Value3 }, "arg", "msg");
            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStringArgumentIsNotInListWithParamName()
        {
            string value = "a";
            Action action = () => Check.IsInList(value, new string[] { "b", "c", "d" }, "arg");
            action.Should().Throw<ArgumentException>()
                .WithMessage($"*a*b, c, d*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStringArgumentIsNotInListWithParamNameAndCustomMessage()
        {
            string value = "a";
            Action action = () => Check.IsInList(value, new string[] { "b", "c", "d" }, "arg", "msg");
            action.Should().Throw<ArgumentException>()
                .WithMessage("msg*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenStringArgumentIsInListWithParamName()
        {
            string value = "d";
            Action action = () => Check.IsInList(value, new string[] { "b", "c", "d" }, "arg");
            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenStringArgumentIsInListWithParamNameAndCustomMessage()
        {
            string value = "d";
            Action action = () => Check.IsInList(value, new string[] { "b", "c", "d" }, "arg", "msg");
            action.Should().NotThrow<ArgumentException>();
        }

        #endregion

        #region IsNotInList

        [Fact]
        public void ThrowArgumentExceptionWhenEnumArgumentIsInListWithParamName()
        {
            DummyEnum value = DummyEnum.Value1;
            Action action = () => Check.IsNotInList(value, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 }, "arg");
            action.Should().Throw<ArgumentException>()
                .WithMessage($"*Value1*Value1, Value2*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWhenEnumArgumentIsInListWithParamNameAndCustomMessage()
        {
            DummyEnum value = DummyEnum.Value1;
            Action action = () => Check.IsNotInList(value, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 }, "arg", "msg");
            action.Should().Throw<ArgumentException>()
                .WithMessage("msg*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenEnumArgumentIsNotInListWithParamName()
        {
            DummyEnum value = DummyEnum.Value3;
            Action action = () => Check.IsNotInList(value, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 }, "arg");
            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenEnumArgumentIsNotInListWithParamNameAndCustomMessage()
        {
            DummyEnum value = DummyEnum.Value3;
            Action action = () => Check.IsNotInList(value, new DummyEnum[] { DummyEnum.Value1, DummyEnum.Value2 }, "arg", "msg");
            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStringArgumentIsInListWithParamName()
        {
            string value = "a";
            Action action = () => Check.IsNotInList(value, new string[] { "a", "b", "c" }, "arg");
            action.Should().Throw<ArgumentException>()
                .WithMessage($"*a*a, b, c*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWhenStringArgumentIsInListWithParamNameAndCustomMessage()
        {
            string value = "a";
            Action action = () => Check.IsNotInList(value, new string[] { "a", "b", "c" }, "arg", "msg");
            action.Should().Throw<ArgumentException>()
                .WithMessage("msg*")
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenStringArgumentIsNotInListWithParamName()
        {
            string value = "d";
            Action action = () => Check.IsNotInList(value, new string[] { "a", "b", "c" }, "arg");
            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void NotThrowArgumentExceptionWhenStringArgumentIsNotInListWithParamNameAndCustomMessage()
        {
            string value = "d";
            Action action = () => Check.IsNotInList(value, new string[] { "a", "b", "c" }, "arg", "msg");
            action.Should().NotThrow<ArgumentException>();
        }

        #endregion

        #endregion

        #region Guid

        [Fact]
        public void ThrowArgumentExceptionWithParamNameWhenEmptyGuid()
        {
            Guid value = Guid.Empty;
            Action action = () => Check.NotEmptyGuid(value, "arg");
            action.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("arg");
        }

        [Fact]
        public void ThrowArgumentExceptionWithParamNameAndMessageWhenEmptyGuid()
        {
            Guid value = Guid.Empty;
            Action action = () => Check.NotEmptyGuid(value, "arg", "Exception message.");
            action.Should().Throw<ArgumentException>()
                .WithMessage("Exception message.*")
                .And.ParamName.Should().Be("arg");
        }

        #endregion
    }
}
