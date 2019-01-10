﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cheats
{
	public static class Objects
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static B Upcast<B, D>(this D obj)
			where D: class, B
			=> obj;
		
		public static bool OneOf<T>(this T value, IEnumerable<T> values)
			where T:class
		{
			foreach (var v in values)
				if (Equals(value, v))
					return true;
			return false;
		}

		public static bool OneOf<T>(this T value, params T[] values)
			where T: class
			=> value.OneOf(values.Upcast<IEnumerable<T>, T[]>());
		
		public static void Decompose<T, R1, R2>(this T obj, Func<T, R1> decomposer1, Func<T, R2> decomposer2, out R1 result1, out R2 result2)
			where T: class
		{
			result1 = decomposer1(obj);
			result2 = decomposer2(obj);
		}

		public static (R1, R2) Decompose<T, R1, R2>(this T obj, Func<T, R1> decomposer1, Func<T, R2> decomposer2)
			where T: class
		{
			var tuple = default((R1 resul1, R2 resul2));
			obj.Decompose(decomposer1, decomposer2, out tuple.resul1, out tuple.resul2);
			return tuple;
		}
	}
}
