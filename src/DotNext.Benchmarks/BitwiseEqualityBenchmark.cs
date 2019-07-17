﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using System;

namespace DotNext
{
    [SimpleJob(runStrategy: RunStrategy.Throughput, launchCount: 1)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class BitwiseEqualityBenchmark
    {
        public struct BigStructure
        {
            public decimal X, Y, Z, C, A, B;
            public short F;

            public bool Equals(BigStructure other)
                => X == other.X &&
                    Y == other.Y &&
                    Z == other.Z &&
                    C == other.C &&
                    A == other.A &&
                    B == other.B &&
                    F == other.F;

            public override int GetHashCode()
            {
                var hash = unchecked((int)2166136261);
                hash = (hash ^ X.GetHashCode()) * 16777619;
                hash = (hash ^ Y.GetHashCode()) * 16777619;
                hash = (hash ^ Z.GetHashCode()) * 16777619;
                hash = (hash ^ C.GetHashCode()) * 16777619;
                hash = (hash ^ A.GetHashCode()) * 16777619;
                hash = (hash ^ B.GetHashCode()) * 16777619;
                hash = (hash ^ F) * 16777619;
                return hash;
            }
        }

        private static readonly Guid NonEmptyGuid = Guid.NewGuid();
        private static readonly BigStructure NonEmptyBigStruct = new BigStructure { C = 30 };

        [Benchmark]
        public void GuidEqualsMethod()
        {
            var value = NonEmptyGuid;
            value.Equals(default);
        }

        [Benchmark]
        public void GuidBitwiseEqualsMethod()
        {
            var value = NonEmptyGuid;
            ValueType<Guid>.BitwiseEquals(value, default);
        }

        [Benchmark]
        public unsafe void GuidBitwiseEqualsUsingSpan()
        {
            var value = NonEmptyGuid;
            var span1 = new ReadOnlySpan<byte>(&value, sizeof(Guid));
            var empty = default(Guid);
            var span2 = new ReadOnlySpan<byte>(&empty, sizeof(Guid));
            span1.SequenceEqual(span2);
        }

        [Benchmark]
        public void BigStructEqualsMethod()
        {
            var value = NonEmptyBigStruct;
            value.Equals(default);
        }

        [Benchmark]
        public void BigStructBitwiseEqualsMethod()
        {
            var value = NonEmptyBigStruct;
            ValueType<BigStructure>.BitwiseEquals(value, default);
        }

        [Benchmark]
        public unsafe void BigStructEqualsUsingSpan()
        {
            var value = NonEmptyBigStruct;
            var span1 = new ReadOnlySpan<byte>(&value, sizeof(BigStructure));
            var empty = default(BigStructure);
            var span2 = new ReadOnlySpan<byte>(&empty, sizeof(BigStructure));
            span1.SequenceEqual(span2);
        }
    }
}
