using System.Reflection;

namespace Aspectize.NET.Tests.Extensions;

public class ReflectionExtensionsTests
{
    public static IEnumerable<object[]> GetDelegateTypeTestData()
    {
        yield return new object[]
            { typeof(TestImplementation).GetMethod(nameof(TestImplementation.Action))!, DelegateType.Action };
        yield return new object[]
            { typeof(TestImplementation).GetMethod(nameof(TestImplementation.Func))!, DelegateType.Func };
        yield return new object[]
            { typeof(TestImplementation).GetMethod(nameof(TestImplementation.AsyncAction))!, DelegateType.AsyncAction };
        yield return new object[]
            { typeof(TestImplementation).GetMethod(nameof(TestImplementation.AsyncFunc))!, DelegateType.AsyncFunc };
    }

    [Theory]
    [MemberData(nameof(GetDelegateTypeTestData))]
    internal void GetDelegateType_ShouldReturnExpectedType(MethodInfo method, DelegateType expectedType)
    {
        // Act
        var result = method.GetDelegateType();

        // Assert
        Assert.Equal(expectedType, result);
    }

    [Aspect<TestInterfaceAspect>]
    public interface ITestInterface
    {
        [Aspect<TestInterfaceMethodAspect>]
        void Action();

        [Aspect<TestInterfaceMethodAspect>]
        int Func();

        [Aspect<TestInterfaceMethodAspect>]
        Task AsyncAction();

        [Aspect<TestInterfaceMethodAspect>]
        Task<int> AsyncFunc();
    }

    public class TestImplementation
    {
        public void Action() { }

        public int Func()
        {
            return 0;
        }

        public Task AsyncAction()
        {
            return Task.CompletedTask;
        }

        public Task<int> AsyncFunc()
        {
            return Task.FromResult(0);
        }
    }

    public class TestInterfaceAspect : IAspect { }

    public class TestInterfaceMethodAspect : IAspect { }

    [Fact]
    internal void GivenInterfaceWithAspectAttribute_GetAspectAttributes_ShouldReturnTheAspectAttribute()
    {
        // Act
        var result = typeof(ITestInterface).GetCustomAttributes<AspectAttribute>();
    }
}