using FluentAssertions;
using System;
using Xunit;

namespace Kros.Utils.UnitTest.Utils
{
    public class SuspenderShould
    {
        #region Nested classes

        private class CustomSuspenderState
        {
            public bool? SuspendCoreState { get; set; } = null;
            public bool? ResumeCoreState { get; set; } = null;
            public int SuspendCoreCallCount { get; set; }
            public int ResumeCoreCallCount { get; set; }
        }

        private class CustomSuspender : Suspender
        {
            private readonly CustomSuspenderState _state;

            public CustomSuspender(CustomSuspenderState state)
            {
                _state = state;
            }

            protected override void SuspendCore()
            {
                base.SuspendCore();
                _state.SuspendCoreState = IsSuspended;
                _state.SuspendCoreCallCount++;
            }

            protected override void ResumeCore()
            {
                base.ResumeCore();
                _state.ResumeCoreState = IsSuspended;
                _state.ResumeCoreCallCount++;
            }
        }

        #endregion

        #region Tests

        [Fact]
        public void SuspendWork()
        {
            var suspender = new Suspender();
            using (suspender.Suspend())
            {
                suspender.IsSuspended.Should().BeTrue();
            }
        }

        [Fact]
        public void ResumeWork()
        {
            var suspender = new Suspender();
            using (suspender.Suspend())
            {
            }
            suspender.IsSuspended.Should().BeFalse();
        }

        [Fact]
        public void ResumeWorkAfterException()
        {
            var suspender = new Suspender();
            try
            {
                using (suspender.Suspend())
                {
                    throw new Exception();
                }
            }
            catch (Exception) { }
            suspender.IsSuspended.Should().BeFalse();
        }

        [Fact]
        public void MultiSuspend()
        {
            var suspender = new Suspender();

            using (suspender.Suspend())
            {
                using (suspender.Suspend())
                {
                }

                suspender.IsSuspended.Should().BeTrue();
            }
        }

        [Fact]
        public void HaveFalseStateInCoreMethods()
        {
            var state = new CustomSuspenderState();
            var suspender = new CustomSuspender(state);

            using (suspender.Suspend())
            {
            }

            state.SuspendCoreState.Should().BeFalse();
            state.ResumeCoreState.Should().BeFalse();
        }

        [Fact]
        public void CallCoreMethodsOnlyOnce()
        {
            var state = new CustomSuspenderState();
            var suspender = new CustomSuspender(state);

            using (suspender.Suspend())
            {
                using (suspender.Suspend())
                {
                    using (suspender.Suspend())
                    {
                    }
                }
            }

            state.SuspendCoreCallCount.Should().Be(1);
            state.ResumeCoreCallCount.Should().Be(1);
        }

        #endregion
    }
}
