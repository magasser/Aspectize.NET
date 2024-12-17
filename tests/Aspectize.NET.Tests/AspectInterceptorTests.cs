using System.Reflection;

using Castle.DynamicProxy;

using FakeItEasy;

namespace Aspectize.NET.Tests;

public class AspectInterceptorTests
{
    private static AspectInterceptor SetupSubject(Type targetType, IAspect aspect)
    {
        var aspectProvider = A.Fake<IAspectProvider>(options => options.Strict());

        A.CallTo(() => aspectProvider.GetAspect(targetType)).Returns(aspect);

        return new AspectInterceptor(targetType, aspectProvider);
    }

    private static IInvocation SetupInvocation(MethodInfo method, object returnValue)
    {
        var invocation = A.Fake<IInvocation>(options => options.Strict());

        A.CallTo(() => invocation.Method).Returns(method);
        A.CallTo(() => invocation.ReturnValue).Returns(returnValue);
        A.CallTo(() => invocation.Proceed()).DoesNothing();

        return invocation;
    }

    [Aspect<TestAspect>]
    [Aspect<TestBeforeAspect>]
    [Aspect<TestAfterAspect>]
    public interface ITargetInterface
    {
        void TestMethod();
    }

    public class TargetImplementation : ITargetInterface
    {
        /// <inheritdoc />
        public void TestMethod() { }
    }

    public class TestAspect : Aspect
    {
        private readonly Action<IInvocationContext> _beforeAction;
        private readonly Action<IInvocationContext> _afterAction;

        public TestAspect(Action<IInvocationContext> beforeAction, Action<IInvocationContext> afterAction)
        {
            _beforeAction = beforeAction;
            _afterAction = afterAction;
        }

        /// <inheritdoc />
        public override void After(IInvocationContext context)
        {
            _afterAction.Invoke(context);
        }

        /// <inheritdoc />
        public override void Before(IInvocationContext context)
        {
            _beforeAction.Invoke(context);
        }
    }

    public class TestBeforeAspect : Aspect
    {
        private readonly Action<IInvocationContext> _beforeAction;

        public TestBeforeAspect(Action<IInvocationContext> beforeAction)
        {
            _beforeAction = beforeAction;
        }

        /// <inheritdoc />
        public override void Before(IInvocationContext context)
        {
            _beforeAction.Invoke(context);
        }
    }

    public class TestAfterAspect : Aspect
    {
        private readonly Action<IInvocationContext> _afterAction;

        public TestAfterAspect(Action<IInvocationContext> afterAction)
        {
            _afterAction = afterAction;
        }

        /// <inheritdoc />
        public override void After(IInvocationContext context)
        {
            _afterAction.Invoke(context);
        }
    }

    [Fact]
    public void GivenTargetTypeHasAfterAspect_Intercept_ShouldCallAfterOnAspect()
    {
        // Arrange
        var executed = false;
        Action<IInvocationContext> afterAction = _ => executed = true;

        var invocation = SetupInvocation(
            typeof(ITargetInterface).GetMethod(nameof(ITargetInterface.TestMethod))!,
            null!);

        var subject = SetupSubject(typeof(ITargetInterface), new TestAfterAspect(afterAction));

        // Act
        subject.Intercept(invocation);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void GivenTargetTypeHasAspect_Intercept_ShouldCallMethodsOnAspect()
    {
        // Arrange
        var beforeExecuted = false;
        var afterExecuted = false;
        Action<IInvocationContext> beforeAction = _ => beforeExecuted = true;
        Action<IInvocationContext> afterAction = _ => afterExecuted = true;

        var invocation = SetupInvocation(
            typeof(ITargetInterface).GetMethod(nameof(ITargetInterface.TestMethod))!,
            null!);

        var subject = SetupSubject(typeof(ITargetInterface), new TestAspect(beforeAction, afterAction));

        // Act
        subject.Intercept(invocation);

        // Assert
        Assert.True(beforeExecuted);
        Assert.True(afterExecuted);
    }

    [Fact]
    public void GivenTargetTypeHasBeforeAspect_Intercept_ShouldCallBeforeOnAspect()
    {
        // Arrange
        var executed = false;
        Action<IInvocationContext> beforeAction = _ => executed = true;

        var invocation = SetupInvocation(
            typeof(ITargetInterface).GetMethod(nameof(ITargetInterface.TestMethod))!,
            null!);

        var subject = SetupSubject(typeof(ITargetInterface), new TestBeforeAspect(beforeAction));

        // Act
        subject.Intercept(invocation);

        // Assert
        Assert.True(executed);
    }
}