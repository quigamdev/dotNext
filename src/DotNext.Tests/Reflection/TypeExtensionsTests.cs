using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace DotNext.Reflection
{
	public sealed class TypeExtensionsTests : Assert
	{
        public sealed class MyList: List<string>
        {

        }

		[Fact]
		public void DelegateSignatureTest()
		{
			var signature = DelegateType.GetInvokeMethod<Func<int, string>>();
			NotNull(signature);
			Equal(typeof(int), signature.GetParameters()[0].ParameterType);
			Equal(typeof(string), signature.ReturnParameter.ParameterType);
		}

        [Fact]
        public void IsGenericInstanceOfTest()
        {
            True(typeof(Func<string>).IsGenericInstanceOf(typeof(Func<>)));
            False(typeof(Func<string>).IsGenericInstanceOf(typeof(Func<int>)));
            True(typeof(List<int>).IsGenericInstanceOf(typeof(List<>)));
            True(typeof(MyList).IsGenericInstanceOf(typeof(IEnumerable<>)));
        }

        [Fact]
        public void CollectionElementTest()
        {
            Equal(typeof(string), typeof(MyList).GetItemType(out var enumerable));
            Equal(typeof(IEnumerable<string>), enumerable);
        }

        private static void GenericMethod<T>(T arg, int i)
        {

        }

        [Fact]
        public void GetGenericMethodTest()
        {
            var method = typeof(Task).GetMethod(nameof(Task.FromException), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, 0, typeof(Exception));
            NotNull(method);
            method = typeof(Task).GetMethod(nameof(Task.FromException), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, 1, typeof(Exception));
            NotNull(method);
            method = typeof(TypeExtensionsTests).GetMethod(nameof(GenericMethod), BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly, 1, null, typeof(int));
            NotNull(method);
        }

        private struct ManagedStruct
        {
            internal int value;
            internal string name;

            internal ManagedStruct(int value, string name)
            {
                this.value = value;
                this.name = name;
            }
        }

        [Fact]
        public void IsUnmanagedTest()
        {
            True(typeof(IntPtr).IsUnmanaged());
            True(typeof(UIntPtr).IsUnmanaged());
            True(typeof(bool).IsUnmanaged());
            True(typeof(Guid).IsUnmanaged());
            True(typeof(DateTime).IsUnmanaged());
            False(typeof(Runtime.InteropServices.Pointer<int>).IsUnmanaged());
            False(typeof(ManagedStruct).IsUnmanaged());
        }
    }
}
