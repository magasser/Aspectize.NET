using System.Reflection;

using Castle.DynamicProxy;

using FakeItEasy;

namespace Aspectize.NET.Tests;

public class AspectInterceptorTests
{
    private static AspectInterceptor SetupSubject(Type targetType, IAspect aspect)
    {
        return new AspectInterceptor(targetType, aspect);
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

    public class TestAspect : IBeforeAspect, IAfterAspect
    {
        private readonly Action<IBeforeInvocationContext> _beforeAction;
        private readonly Action<IAfterInvocationContext> _afterAction;

        public TestAspect(Action<IBeforeInvocationContext> beforeAction, Action<IAfterInvocationContext> afterAction)
        {
            _beforeAction = beforeAction;
            _afterAction = afterAction;
        }

        /// <inheritdoc />
        public void After(IAfterInvocationContext context)
        {
            _afterAction.Invoke(context);
        }

        /// <inheritdoc />
        public void Before(IBeforeInvocationContext context)
        {
            _beforeAction.Invoke(context);
        }
    }

    public class TestBeforeAspect : IBeforeAspect
    {
        private readonly Action<IBeforeInvocationContext> _beforeAction;

        public TestBeforeAspect(Action<IBeforeInvocationContext> beforeAction)
        {
            _beforeAction = beforeAction;
        }

        /// <inheritdoc />
        public void Before(IBeforeInvocationContext context)
        {
            _beforeAction.Invoke(context);
        }
    }

    public class TestAfterAspect : IAfterAspect
    {
        private readonly Action<IAfterInvocationContext> _afterAction;

        public TestAfterAspect(Action<IAfterInvocationContext> afterAction)
        {
            _afterAction = afterAction;
        }

        /// <inheritdoc />
        public void After(IAfterInvocationContext context)
        {
            _afterAction.Invoke(context);
        }
    }

    [Fact]
    public void GivenTargetTypeHasAfterAspect_Intercept_ShouldCallAfterOnAspect()
    {
        // Arrange
        var executed = false;
        Action<IAfterInvocationContext> afterAction = _ => executed = true;

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
        Action<IBeforeInvocationContext> beforeAction = _ => beforeExecuted = true;
        Action<IAfterInvocationContext> afterAction = _ => afterExecuted = true;

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
        Action<IBeforeInvocationContext> beforeAction = _ => executed = true;

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