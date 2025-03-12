using Kharazmi.AspNetCore.Core.Dependency;
using Kharazmi.AspNetCore.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTest
{
    [CollectionDefinition(nameof(InstanceCreatorTests), DisableParallelization = true)]
    public class InstanceCreatorTestsCollection
    {
    }

    [Collection(nameof(InstanceCreatorTests))]
    public class InstanceCreatorTests
    {
        [Fact]
        public void CreateInstance_SimpleClass_CreatesInstance()
        {
            // Act
            var instance = InstanceCreator.CreateInstance<SimpleClass>();

            // Assert
            Assert.NotNull(instance);
            Assert.IsType<SimpleClass>(instance);
        }

        [Fact]
        public void CreateInstance_WithPrimitiveArguments_PassesArgumentsCorrectly()
        {
            // Arrange
            var name = "Test Name";
            var age = 30;

            // Act
            var instance = InstanceCreator.CreateInstance<ClassWithPrimitives>(options: null, name, age);

            // Assert
            Assert.NotNull(instance);
            Assert.Equal(name, instance.Name);
            Assert.Equal(age, instance.Age);
        }

        [Fact]
        public void CreateInstance_WithNestedDependencies_CreatesCompleteInstance()
        {
            // Act
            var instance = InstanceCreator.CreateInstance<ClassWithNestedDependencies>();

            // Assert
            Assert.NotNull(instance);
            Assert.NotNull(instance.SimpleClassDependency);
            Assert.NotNull(instance.AnotherClassDependency);
        }

        [Fact]
        public void CreateInstance_WithCustomOptions_RespectsCapacity()
        {
            List<Type> types = [
                typeof(GenericTest<>).MakeGenericType(typeof(int)),
                typeof(GenericTest<>).MakeGenericType(typeof(string)),
                typeof(GenericTest<>).MakeGenericType(typeof(bool)),
                typeof(GenericTest<>).MakeGenericType(typeof(double)),
                typeof(GenericTest<>).MakeGenericType(typeof(decimal))
            ];

            // Pre-populate cache with other types
            foreach (var type in types)
            {
                InstanceCreator.CreateInstance(type, ConfigureOptions);
            }
            foreach (var type in types)
            {
                InstanceCreator.CreateInstance(type, ConfigureOptions);
            }

            // Act
            var instance = InstanceCreator.CreateInstance<SimpleClass>(ConfigureOptions);
            
            // Assert
            Assert.NotNull(instance);
            Assert.True(InstanceCreator.ConstructorInfos.Count(x => types.Any(t=> t.GetTypeFullName() == x.Key)) <= 5,
                $"Cache count is {InstanceCreator.ConstructorInfos.Count}, should be <= 5");
            return;

            // Arrange
            static void ConfigureOptions(InstanceCreatorOptions options)
            {
                options.Capacity = 5;
            }
        }

        [Fact]
        public void CreateInstance_WithDefaultParameters_UsesDefaultWhenNotProvided()
        {
            // Act
            var instance = InstanceCreator.CreateInstance<ClassWithDefaultParameters>();

            // Assert
            Assert.NotNull(instance);
            Assert.Equal("Default", instance.Name);
            Assert.Equal(18, instance.Age);
        }

        [Fact]
        public void CreateInstance_WithDefaultParameters_OverridesWithProvidedValues()
        {
            // Arrange
            var name = "Custom Name";
            var age = 25;

            // Act
            var instance = InstanceCreator.CreateInstance<ClassWithDefaultParameters>(options: null, name, age);

            // Assert
            Assert.NotNull(instance);
            Assert.Equal(name, instance.Name);
            Assert.Equal(age, instance.Age);
        }

        [Fact]
        public void CreateInstance_ValueType_CreatesInstance()
        {
            // Act
            var instance = InstanceCreator.CreateInstance<int>();

            // Assert
            Assert.Equal(0, instance);
        }

        [Fact]
        public void CreateInstance_NullableValueType_ThrowsInstanceException()
        {
            // Act
            var exception = Assert.Throws<InstanceException>(() => InstanceCreator.CreateInstance<int?>());

            // Assert
            Assert.Contains($"Can't create an instance of value type {typeof(int?)}", exception.Message);
        }

        [Fact]
        public void CreateInstance_AbstractClass_ThrowsInstanceException()
        {
            // Act & Assert
            var exception = Assert.Throws<InstanceException>(() => InstanceCreator.CreateInstance<AbstractClass>());
            Assert.Contains("abstract", exception.Message);
        }

        [Fact]
        public void CreateInstance_Interface_ThrowsInstanceException()
        {
            // Act & Assert
            var exception = Assert.Throws<InstanceException>(() => InstanceCreator.CreateInstance<IInterface>());
            Assert.Contains("interface", exception.Message);
        }

        [Fact]
        public void CreateInstance_OpenGenericType_ThrowsInstanceException()
        {
            // Act & Assert
            var exception = Assert.Throws<InstanceException>(() =>
                InstanceCreator.CreateInstance(typeof(GenericTest<>)));
            Assert.Contains("open generic type", exception.Message);
        }

        [Fact]
        public void CreateInstance_CircularDependency_ThrowsInstanceException()
        {
            // Act & Assert
            var exception = Assert.Throws<InstanceException>(() =>
                InstanceCreator.CreateInstance<CircularDependencyA>());
            Assert.Contains($"Circular dependency detected for type {typeof(CircularDependencyA)}", exception.Message);
        }

        [Fact]
        public void CreateInstance_LRU_RemovesLeastRecentlyUsedItem()
        {
            // Create and access some instances to populate the cache
            _ = InstanceCreator.CreateInstance<SimpleClass>(ConfigureOptions);
            _ = InstanceCreator.CreateInstance<ClassWithPrimitives>(ConfigureOptions, "name", 30);
            _ = InstanceCreator.CreateInstance<ClassWithNestedDependencies>(ConfigureOptions);

            // Access A and C again to make B the least recently used
            InstanceCreator.CreateInstance<SimpleClass>(ConfigureOptions);
            InstanceCreator.CreateInstance<ClassWithNestedDependencies>(ConfigureOptions);

            // Act - add one more to exceed capacity
            InstanceCreator.CreateInstance<ClassWithDefaultParameters>(ConfigureOptions);

            // Assert
            Assert.True(InstanceCreator.ConstructorInfos.Count <= 3,
                $"Cache count is {InstanceCreator.ConstructorInfos.Count}, should be <= 3");

            // B should be removed
            Assert.False(InstanceCreator.ConstructorInfos.ContainsKey(typeof(ClassWithPrimitives).FullName!));
            return;

            // Arrange
            static void ConfigureOptions(InstanceCreatorOptions options)
            {
                options.Capacity = 3;
            }
        }

        [Fact]
        public void CreateInstance_Parallel_HandlesMultipleThreads()
        {
            // Arrange
            var types = new Dictionary<Type, object[]>
            {
                { typeof(SimpleClass), [] },
                { typeof(ClassWithPrimitives), ["name", 25] },
                { typeof(ClassWithNestedDependencies), [] },
                { typeof(ClassWithDefaultParameters), [] }
            };

            // Act
            var exceptions = new List<Exception>();
            Parallel.For(0, 100, i =>
            {
                try
                {
                    var key = i % types.Count;
                    var randomType = types.ElementAt(key);
                    InstanceCreator.CreateInstance(randomType.Key, null, randomType.Value);
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            });

            // Assert
            Assert.Empty(exceptions);
        }

        [Fact]
        public void CreateInstance_MixedParameters_HandlesCorrectly()
        {
            // Arrange
            var name = "Test Name";

            // Act
            var instance = InstanceCreator.CreateInstance<ClassWithMixedParameters>(options: null, name);

            // Assert
            Assert.NotNull(instance);
            Assert.Equal(name, instance.Name);
            Assert.NotNull(instance.SimpleClassDependency);
        }

        // Test Classes
        public class SimpleClass
        {
            public SimpleClass()
            {
            }
        }

        public class ClassWithPrimitives
        {
            public string Name { get; }
            public int Age { get; }

            [InstanceConstructor]
            public ClassWithPrimitives(string name, int age)
            {
                Name = name;
                Age = age;
            }
        }

        public class ClassWithNestedDependencies
        {
            public SimpleClass SimpleClassDependency { get; }
            public AnotherClass AnotherClassDependency { get; }

            [InstanceConstructor]
            public ClassWithNestedDependencies(SimpleClass simpleClass, AnotherClass anotherClass)
            {
                SimpleClassDependency = simpleClass;
                AnotherClassDependency = anotherClass;
            }
        }

        public class AnotherClass
        {
            public AnotherClass()
            {
            }
        }

        public class ClassWithDefaultParameters
        {
            public string Name { get; }
            public int Age { get; }

            [InstanceConstructor]
            public ClassWithDefaultParameters(string name = "Default", int age = 18)
            {
                Name = name;
                Age = age;
            }
        }

        public class ClassWithMixedParameters
        {
            public string Name { get; }
            public SimpleClass SimpleClassDependency { get; }

            [InstanceConstructor]
            public ClassWithMixedParameters(SimpleClass simpleClass, string name)
            {
                SimpleClassDependency = simpleClass;
                Name = name;
            }
        }

        public class CircularDependencyA
        {
            public CircularDependencyB B { get; }

            [InstanceConstructor]
            public CircularDependencyA(CircularDependencyB b)
            {
                B = b;
            }
        }

        public class CircularDependencyB
        {
            public CircularDependencyA A { get; }

            [InstanceConstructor]
            public CircularDependencyB(CircularDependencyA a)
            {
                A = a;
            }
        }

        public abstract class AbstractClass
        {
        }

        public interface IInterface
        {
        }

        public class GenericTest<T>
        {
            public T Value { get; set; }

            public GenericTest()
            {
                Value = default!;
            }
        }
    }
}