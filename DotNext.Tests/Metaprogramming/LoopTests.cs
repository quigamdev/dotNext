using System;
using Xunit;

namespace DotNext.Metaprogramming
{
    public sealed class LoopTests: Assert
    {
        public delegate long SpanDelegate(ReadOnlySpan<long> values);

        [Fact]
        public void ForEachSpanTest()
        {
            var sum = LambdaBuilder<SpanDelegate>.Build(fun =>
            {
                ExpressionView result = fun.DeclareVariable("result", 0L);
                fun.ForEach(fun.Parameters[0], loop =>
                {
                    loop.Assign(result, result + loop.Element);
                });
                fun.Return(result);
            })
            .Compile();
            Equal(10L, sum(new[] { 1L, 5L, 4L }));
        }

        [Fact]
        public void ForEachTest()
        {
            var sum = LambdaBuilder<Func<long[], long>>.Build(fun =>
            {
                ExpressionView result = fun.DeclareVariable("result", 0L);
                fun.ForEach(fun.Parameters[0], loop =>
                {
                    loop.Assign(result, result + loop.Element);
                });
                fun.Return(result);
            })
            .Compile();
            Equal(10L, sum(new[] { 1L, 5L, 4L }));
        }

        [Fact]
        public void SumTest()
        {
            var sum = LambdaBuilder<Func<long, long>>.Build(fun =>
            {
                ExpressionView arg = fun.Parameters[0];
                ExpressionView result = fun.DeclareVariable("result", 0L);
                fun.DoWhile(arg > 0, loop =>
                {
                    loop.Assign(result, result + arg);
                    loop.Assign(arg, arg - 1L);
                });
                fun.Return(result);
            })
            .Compile();
            Equal(6, sum(3));
        }

        [Fact]
        public void FactorialTest()
        {
            var factorial = LambdaBuilder<Func<long, long>>.Build(fun => 
            {
                ExpressionView arg = fun.Parameters[0];
                ExpressionView result = fun.DeclareVariable("result", 1L);
                fun.While(arg > 1L, loop =>
                {
                    loop.Assign(result, result * arg);
                    loop.Assign(arg, arg - 1L);
                });
                fun.Return(result);
            })
            .Compile();
            Equal(6, factorial(3));
        }

        [Fact]
        public void Factorial2Test()
        {
            var factorial = LambdaBuilder<Func<long, long>>.Build(fun =>
            {
                ExpressionView arg = fun.Parameters[0];
                ExpressionView result = fun.DeclareVariable("result", 1L);
                fun.Loop(loop =>
                {
                    loop.If(arg > 1L)
                        .Then(then =>
                        {
                            then.Assign(result, result * arg);
                            then.Assign(arg, arg - 1L);
                        })
                        .Else(@else => @else.Break(loop))
                        .EndIf();
                });
                fun.Return(result);
            })
            .Compile();
            Equal(6, factorial(3));
        }
    }
}