using System.Reflection;

namespace Aspectize.NET.Tests.Extensions;

public class ReflectionExtensionsTests
{
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

    public class TestInterfaceAspect : Aspect { }

    public class TestInterfaceMethodAspect : Aspect { }

    [Fact]
    internal void GivenInterfaceWithAspectAttribute_GetAspectAttributes_ShouldReturnTheAspectAttribute()
    {
        // Act
        var result = typeof(ITestInterface).GetCustomAttributes<AspectAttribute>();
    }
}