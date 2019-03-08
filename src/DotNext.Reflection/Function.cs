using System;
using System.Runtime.CompilerServices;

namespace DotNext
{
    /// <summary>
    /// Represents a static function with arbitrary number of arguments
    /// allocated on the stack.
    /// </summary>
    /// <param name="arguments">Function arguments in the form of public structure fields.</param>
    /// <typeparam name="A">Type of structure with function arguments allocated on the stack.</typeparam>
    /// <typeparam name="R">Type of function return value.</typeparam>
    /// <returns>Function return value.</returns>
    public delegate R Function<A, R>(in A arguments)
        where A: struct;

    /// <summary>
    /// Represents an instance function with arbitrary number of arguments
    /// allocated on the stack.
    /// </summary>
    /// <param name="this">Hidden <see langword="this"/> parameter.</param>
    /// <param name="arguments">Function arguments in the form of public structure fields.</param>
    /// <typeparam name="T">Type of instance to be passed into underlying method.</typeparam>
    /// <typeparam name="A">Type of structure with function arguments allocated on the stack.</typeparam>
    /// <typeparam name="R">Type of function return value.</typeparam>
    /// <returns>Function return value.</returns>
    public delegate R Function<T, A, R>(in T @this, in A arguments);

    /// <summary>
    /// Provides extension methods for delegates <see cref="Function{A, R}"/> and <see cref="Function{T, A, R}"/>.
    /// </summary>
    public static class Function
    {
        /// <summary>
        /// Allocates list of arguments on the stack.
        /// </summary>
        /// <typeparam name="A">The type representing list of arguments.</typeparam>
        /// <typeparam name="R">The return type of the function.</typeparam>
        /// <param name="function">The function instance.</param>
        /// <returns>Allocated list of arguments.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A ArgList<A, R>(this Function<A, R> function)
            where A: struct
            => new A();

        /// <summary>
        /// Allocates list of arguments on the stack.
        /// </summary>
        /// <typeparam name="T">Type of explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="A">The type representing list of arguments.</typeparam>
        /// <typeparam name="R">The return type of the function.</typeparam>
        /// <param name="function">The function instance.</param>
        /// <returns>Allocated list of arguments.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A ArgList<T, A, R>(this Function<T, A, R> function)
            where A: struct
            => new A();

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, R>(this Function<T, ValueTuple, R> function, in T instance)
			=> function(in instance, in EmptyTuple.Value);

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<R>(this Function<ValueTuple, R> function)
			=> function(in EmptyTuple.Value);

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P">The type of the first function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg">The first function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P, R>(this Function<ValueTuple<P>, R> function, P arg)
            => function(new ValueTuple<P>(arg));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P">The type of the first function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg">The first function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P, R>(this Function<T, ValueTuple<P>, R> function, in T instance, P arg)
            => function(in instance, new ValueTuple<P>(arg));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P1, P2, R>(this Function<(P1, P2), R> function, P1 arg1, P2 arg2)
            => function((arg1, arg2));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P1, P2, R>(this Function<T, (P1, P2), R> function, in T instance, P1 arg1, P2 arg2)
            => function(in instance, (arg1, arg2));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P1, P2, P3, R>(this Function<T, (P1, P2, P3), R> function, in T instance, P1 arg1, P2 arg2, P3 arg3)
            => function(in instance, (arg1, arg2, arg3));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P1, P2, P3, R>(this Function<(P1, P2, P3), R> function, P1 arg1, P2 arg2, P3 arg3)
            => function((arg1, arg2, arg3));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P1, P2, P3, P4, R>(this Function<T, (P1, P2, P3, P4), R> function, in T instance, P1 arg1, P2 arg2, P3 arg3, P4 arg4)
            => function(in instance, (arg1, arg2, arg3, arg4));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P1, P2, P3, P4, R>(this Function<(P1, P2, P3, P4), R> function, P1 arg1, P2 arg2, P3 arg3, P4 arg4)
            => function((arg1, arg2, arg3, arg4));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P1, P2, P3, P4, P5, R>(this Function<T, (P1, P2, P3, P4, P5), R> function, in T instance, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5)
            => function(in instance, (arg1, arg2, arg3, arg4, arg5));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P1, P2, P3, P4, P5, R>(this Function<(P1, P2, P3, P4, P5), R> function, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5)
            => function((arg1, arg2, arg3, arg4, arg5));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P1, P2, P3, P4, P5, P6, R>(this Function<T, (P1, P2, P3, P4, P5, P6), R> function, in T instance, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6)
            => function(in instance, (arg1, arg2, arg3, arg4, arg5, arg6));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P1, P2, P3, P4, P5, P6, R>(this Function<(P1, P2, P3, P4, P5, P6), R> function, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6)
            => function((arg1, arg2, arg3, arg4, arg5, arg6));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <typeparam name="P7">The type of the seventh function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <param name="arg7">The seventh function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P1, P2, P3, P4, P5, P6, P7, R>(this Function<T, (P1, P2, P3, P4, P5, P6, P7), R> function, in T instance, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6, P7 arg7)
            => function(in instance, (arg1, arg2, arg3, arg4, arg5, arg6, arg7));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <typeparam name="P7">The type of the seventh function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <param name="arg7">The seventh function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P1, P2, P3, P4, P5, P6, P7, R>(this Function<(P1, P2, P3, P4, P5, P6, P7), R> function, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6, P7 arg7)
            => function((arg1, arg2, arg3, arg4, arg5, arg6, arg7));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <typeparam name="P7">The type of the seventh function argument.</typeparam>
        /// <typeparam name="P8">The type of the eighth function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <param name="arg7">The seventh function argument.</param>
        /// <param name="arg8">The eighth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P1, P2, P3, P4, P5, P6, P7, P8, R>(this Function<T, (P1, P2, P3, P4, P5, P6, P7, P8), R> function, in T instance, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6, P7 arg7, P8 arg8)
            => function(in instance, (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <typeparam name="P7">The type of the seventh function argument.</typeparam>
        /// <typeparam name="P8">The type of the eighth function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <param name="arg7">The seventh function argument.</param>
        /// <param name="arg8">The eighth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P1, P2, P3, P4, P5, P6, P7, P8, R>(this Function<(P1, P2, P3, P4, P5, P6, P7, P8), R> function, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6, P7 arg7, P8 arg8)
            => function((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <typeparam name="P7">The type of the seventh function argument.</typeparam>
        /// <typeparam name="P8">The type of the eighth function argument.</typeparam>
        /// <typeparam name="P9">The type of the ninth function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <param name="arg7">The seventh function argument.</param>
        /// <param name="arg8">The eighth function argument.</param>
        /// <param name="arg9">The ninth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, R>(this Function<T, (P1, P2, P3, P4, P5, P6, P7, P8, P9), R> function, in T instance, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6, P7 arg7, P8 arg8, P9 arg9)
            => function(in instance, (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <typeparam name="P7">The type of the seventh function argument.</typeparam>
        /// <typeparam name="P8">The type of the eighth function argument.</typeparam>
        /// <typeparam name="P9">The type of the ninth function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <param name="arg7">The seventh function argument.</param>
        /// <param name="arg8">The eighth function argument.</param>
        /// <param name="arg9">The ninth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P1, P2, P3, P4, P5, P6, P7, P8, P9, R>(this Function<(P1, P2, P3, P4, P5, P6, P7, P8, P9), R> function, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6, P7 arg7, P8 arg8, P9 arg9)
            => function((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="T">The type of the explicit <see langword="this"/> argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <typeparam name="P7">The type of the seventh function argument.</typeparam>
        /// <typeparam name="P8">The type of the eighth function argument.</typeparam>
        /// <typeparam name="P9">The type of the ninth function argument.</typeparam>
        /// <typeparam name="P10">The type of the tenth function argument.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="instance">Explicit <see langword="this"/> argument.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <param name="arg7">The seventh function argument.</param>
        /// <param name="arg8">The eighth function argument.</param>
        /// <param name="arg9">The ninth function argument.</param>
        /// <param name="arg10">The tenth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, R>(this Function<T, (P1, P2, P3, P4, P5, P6, P7, P8, P9, P10), R> function, in T instance, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6, P7 arg7, P8 arg8, P9 arg9, P10 arg10)
            => function(in instance, (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));

        /// <summary>
        /// Invokes function.
        /// </summary>
        /// <typeparam name="P1">The type of the first function argument.</typeparam>
        /// <typeparam name="P2">The type of the second function argument.</typeparam>
        /// <typeparam name="P3">The type of the third function argument.</typeparam>
        /// <typeparam name="P4">The type of the fourth function argument.</typeparam>
        /// <typeparam name="P5">The type of the fifth function argument.</typeparam>
        /// <typeparam name="P6">The type of the sixth function argument.</typeparam>
        /// <typeparam name="P7">The type of the seventh function argument.</typeparam>
        /// <typeparam name="P8">The type of the eighth function argument.</typeparam>
        /// <typeparam name="P9">The type of the ninth function argument.</typeparam>
        /// <typeparam name="P10">The type of the tenth function argument.</typeparam>
        /// <typeparam name="R">The type of function return value.</typeparam>
        /// <param name="function">The function to be invoked.</param>
        /// <param name="arg1">The first function argument.</param>
        /// <param name="arg2">The second function argument.</param>
        /// <param name="arg3">The third function argument.</param>
        /// <param name="arg4">The fourth function argument.</param>
        /// <param name="arg5">The fifth function argument.</param>
        /// <param name="arg6">The sixth function argument.</param>
        /// <param name="arg7">The seventh function argument.</param>
        /// <param name="arg8">The eighth function argument.</param>
        /// <param name="arg9">The ninth function argument.</param>
        /// <param name="arg10">The tenth function argument.</param>
        /// <returns>Function return value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R Invoke<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, R>(this Function<(P1, P2, P3, P4, P5, P6, P7, P8, P9, P10), R> function, P1 arg1, P2 arg2, P3 arg3, P4 arg4, P5 arg5, P6 arg6, P7 arg7, P8 arg8, P9 arg9, P10 arg10)
            => function((arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));
    }
}