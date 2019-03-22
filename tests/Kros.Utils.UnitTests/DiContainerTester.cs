using FluentAssertions;
using Xunit;

namespace Kros.Utils.UnitTests
{
    public abstract class DiContainerTester<T> where T : IDiContainer, new()
    {
        #region Helpers

        protected interface IFoo { }
        protected interface IBar { }
        protected class Foo : IFoo { }
        protected class FooChild : IFoo { }
        protected class Bar : IBar
        {
            public Bar(IFoo foo)
            {
                Foo = foo;
            }

            public IFoo Foo { get; }
        }

        protected IDiContainer CreateConntainer()
        {
            return new T();
        }

        #endregion

        #region Tests

        [Fact]
        public void RegisterClassTypeAndReturnNewInstanceOnEachResolve()
        {
            const string info = "container.Register<Foo>();";

            IDiContainer container = CreateConntainer();

            container.Register<Foo>();

            Foo instance1 = container.GetInstance<Foo>();
            instance1.Should().NotBeNull(info);

            Foo instance2 = container.GetInstance<Foo>();
            instance2.Should().NotBeSameAs(instance1, info);
        }

        [Fact]
        public void RegisterNamedClassTypeAndReturnNewInstanceOnEachResolve()
        {
            const string info =
@"container
    .Register<Foo>()
    .Register<Foo>(""foo2"");
";

            IDiContainer container = CreateConntainer();

            container
                .Register<Foo>()
                .Register<Foo>("foo2");

            Foo instance1 = container.GetInstance<Foo>();
            instance1.Should().NotBeNull(info + "container.GetInstance<Foo>();");

            Foo instance2 = container.GetInstance<Foo>("foo2");
            instance2.Should().NotBeSameAs(instance1, info + "container.GetInstance<Foo>(\"foo2\");");
        }

        [Fact]
        public void RegisterInterfaceWithClassAndReturnNewInstanceOnEachResolve()
        {
            const string info = "container.Register<IFoo, Foo>();";

            IDiContainer container = CreateConntainer();

            container.Register<IFoo, Foo>();

            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should()
                .NotBeNull(info)
                .And.BeOfType<Foo>(info);

            IFoo instance2 = container.GetInstance<IFoo>();
            instance2.Should()
                .BeOfType<Foo>(info)
                .And.NotBeSameAs(instance1, info);
        }

        [Fact]
        public void RegisterNamedInterfaceWithClassAndReturnNewInstanceOnEachResolve()
        {
            const string info =
@"container
    .Register<IFoo, Foo>()
    .Register<IFoo, Foo>(""foo2"");
";

            IDiContainer container = CreateConntainer();

            container
                .Register<IFoo, Foo>()
                .Register<IFoo, Foo>("foo2");

            const string additionalInfo1 = "container.GetInstance<IFoo>();";
            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should()
                .NotBeNull(info + additionalInfo1)
                .And.BeOfType<Foo>(info + additionalInfo1);

            const string additionalInfo2 = "container.GetInstance<IFoo>(\"foo2\");";
            IFoo instance2 = container.GetInstance<IFoo>("foo2");
            instance2.Should()
                .BeOfType<Foo>(info + additionalInfo2)
                .And.NotBeSameAs(instance1, info + additionalInfo2);
        }

        [Fact]
        public void RegisterClassTypeUsingLambdaAndReturnNewInstanceOnEachResolve()
        {
            const string info = "container.Register<IFoo>(c => new Foo());";

            IDiContainer container = CreateConntainer();

            container.Register<IFoo>(c => new Foo());

            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should()
                .NotBeNull(info)
                .And.BeOfType<Foo>(info);

            IFoo instance2 = container.GetInstance<IFoo>();
            instance2.Should()
                .BeOfType<Foo>(info)
                .And.NotBeSameAs(instance1, info);
        }

        [Fact]
        public void RegisterNamedClassTypeUsingLambdaAndReturnNewInstanceOnEachResolve()
        {
            const string info =
@"container
    .Register<IFoo>(c => new Foo())
    .Register<IFoo>(""foo2"", c => new Foo());
";

            IDiContainer container = CreateConntainer();

            container
                .Register<IFoo>(c => new Foo())
                .Register<IFoo>("foo2", c => new Foo());

            const string additionalInfo1 = "container.GetInstance<IFoo>();";
            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should()
                .NotBeNull(info + additionalInfo1)
                .And.BeOfType<Foo>(info + additionalInfo1);

            const string additionalInfo2 = "container.GetInstance<IFoo>(\"foo2\");";
            IFoo instance2 = container.GetInstance<IFoo>("foo2");
            instance2.Should()
                .BeOfType<Foo>(info + additionalInfo2)
                .And.NotBeSameAs(instance1, info + additionalInfo2);
        }

        [Fact]
        public void RegisterClassTypeAsSingleton()
        {
            const string info = "container.RegisterInstance<Foo>();";

            IDiContainer container = CreateConntainer();

            container.RegisterInstance<Foo>();

            Foo instance1 = container.GetInstance<Foo>();
            instance1.Should().NotBeNull(info);

            Foo instance2 = container.GetInstance<Foo>();
            instance2.Should().BeSameAs(instance1, info);
        }

        [Fact]
        public void RegisterNamedClassTypeAsSingleton()
        {
            const string info =
@"container
    .RegisterInstance<Foo>()
    .RegisterInstance<Foo>(""foo2"");
";

            IDiContainer container = CreateConntainer();

            container
                .RegisterInstance<Foo>()
                .RegisterInstance<Foo>("foo2");

            const string additionalInfo1 = "container.GetInstance<Foo>()";
            Foo instance1 = container.GetInstance<Foo>();
            instance1.Should().NotBeNull(info + additionalInfo1);

            Foo instance2 = container.GetInstance<Foo>();
            instance2.Should().BeSameAs(instance1, info + additionalInfo1);

            const string additionalInfo2 = "container.GetInstance<Foo>(\"foo2\")";
            Foo instance3 = container.GetInstance<Foo>("foo2");
            instance3.Should().NotBeSameAs(instance2, info + additionalInfo2);

            Foo instance4 = container.GetInstance<Foo>("foo2");
            instance4.Should().BeSameAs(instance3, info + additionalInfo2);
        }

        [Fact]
        public void RegisterSpecificInstanceAsSingleton()
        {
            const string info = "container.RegisterInstance<IFoo>(specificInstance);";

            Foo specificInstance = new Foo();
            IDiContainer container = CreateConntainer();

            container.RegisterInstance<IFoo>(specificInstance);

            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should().BeSameAs(specificInstance, info);

            IFoo instance2 = container.GetInstance<IFoo>();
            instance2.Should().BeSameAs(specificInstance, info);
        }

        [Fact]
        public void RegisterNamedSpecificInstanceAsSingleton()
        {
            const string info =
@"container
    .RegisterInstance<IFoo>(specificInstance1)
    .RegisterInstance<IFoo>(""foo2"", specificInstance2);
";

            Foo specificInstance1 = new Foo();
            Foo specificInstance2 = new Foo();
            IDiContainer container = CreateConntainer();

            container
                .RegisterInstance<IFoo>(specificInstance1)
                .RegisterInstance<IFoo>("foo2", specificInstance2);

            const string additionalInfo1 = "container.GetInstance<IFoo>();";
            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should().BeSameAs(specificInstance1, info + additionalInfo1);

            IFoo instance2 = container.GetInstance<IFoo>();
            instance2.Should().BeSameAs(specificInstance1, info + additionalInfo1);

            const string additionalInfo2 = "container.GetInstance<IFoo>(\"foo2\");";
            IFoo instance3 = container.GetInstance<IFoo>("foo2");
            instance3.Should().BeSameAs(specificInstance2, info + additionalInfo2);

            IFoo instance4 = container.GetInstance<IFoo>("foo2");
            instance4.Should().BeSameAs(specificInstance2, info + additionalInfo2);
        }

        [Fact]
        public void RegisterInterfaceWithClassAsSingleton()
        {
            const string info = "container.RegisterInstance<IFoo, Foo>();";

            IDiContainer container = CreateConntainer();

            container.RegisterInstance<IFoo, Foo>();

            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should().NotBeNull(info);

            IFoo instance2 = container.GetInstance<IFoo>();
            instance2.Should().BeSameAs(instance1, info);
        }

        [Fact]
        public void RegisterNamedInterfaceWithClassAsSingleton()
        {
            const string info =
@"container
    .RegisterInstance<IFoo, Foo>()
    .RegisterInstance<IFoo, Foo>(""foo2"");
";

            IDiContainer container = CreateConntainer();

            container
                .RegisterInstance<IFoo, Foo>()
                .RegisterInstance<IFoo, Foo>("foo2");

            const string additionalInfo1 = "container.GetInstance<IFoo>();";
            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should().NotBeNull(info + additionalInfo1);

            IFoo instance2 = container.GetInstance<IFoo>();
            instance2.Should().BeSameAs(instance1, info + additionalInfo1);

            const string additionalInfo2 = "container.GetInstance<IFoo>(\"foo2\")";
            IFoo instance3 = container.GetInstance<IFoo>("foo2");
            instance3.Should()
                .NotBeNull(info + container.GetInstance<IFoo>("foo2"))
                .And.NotBeSameAs(instance1, info + additionalInfo2);
            IFoo instance4 = container.GetInstance<IFoo>("foo2");
            instance4.Should().BeSameAs(instance3, info + additionalInfo2);
        }

        [Fact]
        public void RegisterInterfaceWithClassUsingLambdaAsSingleton()
        {
            const string info = "container.RegisterInstance<IFoo>(c => new Foo());";

            IDiContainer container = CreateConntainer();

            container.RegisterInstance<IFoo>(c => new Foo());

            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should()
                .NotBeNull(info)
                .And.BeOfType<Foo>(info);

            IFoo instance2 = container.GetInstance<IFoo>();
            instance2.Should().BeSameAs(instance1, info);
        }

        [Fact]
        public void RegisterNamedInterfaceWithClassUsingLambdaAsSingleton()
        {
            const string info =
@"container
    .RegisterInstance<IFoo>(c => new Foo())
    .RegisterInstance<IFoo>(""foo2"", c => new Foo());
";

            IDiContainer container = CreateConntainer();

            container
                .RegisterInstance<IFoo>(c => new Foo())
                .RegisterInstance<IFoo>("foo2", c => new Foo());

            const string additionalInfo1 = "container.GetInstance<IFoo>();";
            IFoo instance1 = container.GetInstance<IFoo>();
            instance1.Should()
                .NotBeNull(info + additionalInfo1)
                .And.BeOfType<Foo>(info + additionalInfo1);

            IFoo instance2 = container.GetInstance<IFoo>();
            instance2.Should().BeSameAs(instance1, info + additionalInfo1);

            const string additionalInfo2 = "container.GetInstance<IFoo>(\"foo2\");";
            IFoo instance3 = container.GetInstance<IFoo>("foo2");
            instance3.Should()
                .NotBeNull(info + additionalInfo2)
                .And.BeOfType<Foo>(info + additionalInfo2)
                .And.NotBeSameAs(instance1, info + additionalInfo2);

            IFoo instance4 = container.GetInstance<IFoo>("foo2");
            instance3.Should().BeSameAs(instance3, info + additionalInfo2);
        }

        [Fact]
        public void ResolveConstructorDependencies()
        {
            IDiContainer container = CreateConntainer();

            container
                .Register<IFoo, Foo>()
                .Register<IBar, Bar>();

            IBar instance = container.GetInstance<IBar>();
            instance.Should()
                .NotBeNull()
                .And.BeOfType<Bar>();
            (instance as Bar).Foo.Should()
                .NotBeNull()
                .And.BeOfType<Foo>();
        }

        [Fact]
        public void ResolveItemsFormChildContainer()
        {
            IDiContainer container = CreateConntainer();

            container
                .Register<IFoo, Foo>()
                .Register<IBar, Bar>();

            IDiContainer childContainer = container.CreateChildContainer();
            childContainer.Register<IFoo, FooChild>();

            IFoo instance = container.GetInstance<IFoo>();
            instance.Should()
                .NotBeNull()
                .And.BeOfType<Foo>();

            IFoo childInstance = childContainer.GetInstance<IFoo>();
            childInstance.Should()
                .NotBeNull()
                .And.BeOfType<FooChild>();

            IBar parentInstance = childContainer.GetInstance<IBar>();
            parentInstance.Should()
                .NotBeNull()
                .And.BeOfType<Bar>();
            (parentInstance as Bar).Foo.Should()
                .NotBeNull()
                .And.BeOfType<FooChild>();
        }

        #endregion
    }
}
