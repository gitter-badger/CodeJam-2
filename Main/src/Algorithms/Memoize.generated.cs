﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

using CodeJam.Collections;

using JetBrains.Annotations;

namespace CodeJam
{
	partial class Algorithms
	{
		/// <summary>
		/// Caches function value for specific arguments.
		/// </summary>
		/// <param name="func">Function to memoize.</param>
		/// <param name="threadSafe">If true - returns thread safe implementation</param>
		/// <typeparam name="TArg1">Type of argument 1</typeparam>
		/// <typeparam name="TArg2">Type of argument 2</typeparam>
		/// <typeparam name="TResult">Type of result</typeparam>
		/// <returns>Memoized function</returns>
		[NotNull]
		[Pure]
		public static Func<TArg1, TArg2, TResult> Memoize<TArg1, TArg2, TResult>(
			[NotNull] this Func<TArg1, TArg2, TResult> func,
			bool threadSafe = false)
		{
			var map =
				LazyDictionary.Create<ValueTuple<TArg1, TArg2>, TResult>(
					key => func(key.Item1, key.Item2),
					threadSafe);
			return (arg1, arg2) => map[ValueTuple.Create(arg1, arg2)];
		}

		/// <summary>
		/// Caches function value for specific arguments.
		/// </summary>
		/// <param name="func">Function to memoize.</param>
		/// <param name="threadSafe">If true - returns thread safe implementation</param>
		/// <typeparam name="TArg1">Type of argument 1</typeparam>
		/// <typeparam name="TArg2">Type of argument 2</typeparam>
		/// <typeparam name="TArg3">Type of argument 3</typeparam>
		/// <typeparam name="TResult">Type of result</typeparam>
		/// <returns>Memoized function</returns>
		[NotNull]
		[Pure]
		public static Func<TArg1, TArg2, TArg3, TResult> Memoize<TArg1, TArg2, TArg3, TResult>(
			[NotNull] this Func<TArg1, TArg2, TArg3, TResult> func,
			bool threadSafe = false)
		{
			var map =
				LazyDictionary.Create<ValueTuple<TArg1, TArg2, TArg3>, TResult>(
					key => func(key.Item1, key.Item2, key.Item3),
					threadSafe);
			return (arg1, arg2, arg3) => map[ValueTuple.Create(arg1, arg2, arg3)];
		}

		/// <summary>
		/// Caches function value for specific arguments.
		/// </summary>
		/// <param name="func">Function to memoize.</param>
		/// <param name="threadSafe">If true - returns thread safe implementation</param>
		/// <typeparam name="TArg1">Type of argument 1</typeparam>
		/// <typeparam name="TArg2">Type of argument 2</typeparam>
		/// <typeparam name="TArg3">Type of argument 3</typeparam>
		/// <typeparam name="TArg4">Type of argument 4</typeparam>
		/// <typeparam name="TResult">Type of result</typeparam>
		/// <returns>Memoized function</returns>
		[NotNull]
		[Pure]
		public static Func<TArg1, TArg2, TArg3, TArg4, TResult> Memoize<TArg1, TArg2, TArg3, TArg4, TResult>(
			[NotNull] this Func<TArg1, TArg2, TArg3, TArg4, TResult> func,
			bool threadSafe = false)
		{
			var map =
				LazyDictionary.Create<ValueTuple<TArg1, TArg2, TArg3, TArg4>, TResult>(
					key => func(key.Item1, key.Item2, key.Item3, key.Item4),
					threadSafe);
			return (arg1, arg2, arg3, arg4) => map[ValueTuple.Create(arg1, arg2, arg3, arg4)];
		}

		/// <summary>
		/// Caches function value for specific arguments.
		/// </summary>
		/// <param name="func">Function to memoize.</param>
		/// <param name="threadSafe">If true - returns thread safe implementation</param>
		/// <typeparam name="TArg1">Type of argument 1</typeparam>
		/// <typeparam name="TArg2">Type of argument 2</typeparam>
		/// <typeparam name="TArg3">Type of argument 3</typeparam>
		/// <typeparam name="TArg4">Type of argument 4</typeparam>
		/// <typeparam name="TArg5">Type of argument 5</typeparam>
		/// <typeparam name="TResult">Type of result</typeparam>
		/// <returns>Memoized function</returns>
		[NotNull]
		[Pure]
		public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Memoize<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
			[NotNull] this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> func,
			bool threadSafe = false)
		{
			var map =
				LazyDictionary.Create<ValueTuple<TArg1, TArg2, TArg3, TArg4, TArg5>, TResult>(
					key => func(key.Item1, key.Item2, key.Item3, key.Item4, key.Item5),
					threadSafe);
			return (arg1, arg2, arg3, arg4, arg5) => map[ValueTuple.Create(arg1, arg2, arg3, arg4, arg5)];
		}

		/// <summary>
		/// Caches function value for specific arguments.
		/// </summary>
		/// <param name="func">Function to memoize.</param>
		/// <param name="threadSafe">If true - returns thread safe implementation</param>
		/// <typeparam name="TArg1">Type of argument 1</typeparam>
		/// <typeparam name="TArg2">Type of argument 2</typeparam>
		/// <typeparam name="TArg3">Type of argument 3</typeparam>
		/// <typeparam name="TArg4">Type of argument 4</typeparam>
		/// <typeparam name="TArg5">Type of argument 5</typeparam>
		/// <typeparam name="TArg6">Type of argument 6</typeparam>
		/// <typeparam name="TResult">Type of result</typeparam>
		/// <returns>Memoized function</returns>
		[NotNull]
		[Pure]
		public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Memoize<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(
			[NotNull] this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> func,
			bool threadSafe = false)
		{
			var map =
				LazyDictionary.Create<ValueTuple<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>, TResult>(
					key => func(key.Item1, key.Item2, key.Item3, key.Item4, key.Item5, key.Item6),
					threadSafe);
			return (arg1, arg2, arg3, arg4, arg5, arg6) => map[ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6)];
		}

		/// <summary>
		/// Caches function value for specific arguments.
		/// </summary>
		/// <param name="func">Function to memoize.</param>
		/// <param name="threadSafe">If true - returns thread safe implementation</param>
		/// <typeparam name="TArg1">Type of argument 1</typeparam>
		/// <typeparam name="TArg2">Type of argument 2</typeparam>
		/// <typeparam name="TArg3">Type of argument 3</typeparam>
		/// <typeparam name="TArg4">Type of argument 4</typeparam>
		/// <typeparam name="TArg5">Type of argument 5</typeparam>
		/// <typeparam name="TArg6">Type of argument 6</typeparam>
		/// <typeparam name="TArg7">Type of argument 7</typeparam>
		/// <typeparam name="TResult">Type of result</typeparam>
		/// <returns>Memoized function</returns>
		[NotNull]
		[Pure]
		public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> Memoize<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(
			[NotNull] this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> func,
			bool threadSafe = false)
		{
			var map =
				LazyDictionary.Create<ValueTuple<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>, TResult>(
					key => func(key.Item1, key.Item2, key.Item3, key.Item4, key.Item5, key.Item6, key.Item7),
					threadSafe);
			return (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => map[ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7)];
		}

		/// <summary>
		/// Caches function value for specific arguments.
		/// </summary>
		/// <param name="func">Function to memoize.</param>
		/// <param name="threadSafe">If true - returns thread safe implementation</param>
		/// <typeparam name="TArg1">Type of argument 1</typeparam>
		/// <typeparam name="TArg2">Type of argument 2</typeparam>
		/// <typeparam name="TArg3">Type of argument 3</typeparam>
		/// <typeparam name="TArg4">Type of argument 4</typeparam>
		/// <typeparam name="TArg5">Type of argument 5</typeparam>
		/// <typeparam name="TArg6">Type of argument 6</typeparam>
		/// <typeparam name="TArg7">Type of argument 7</typeparam>
		/// <typeparam name="TArg8">Type of argument 8</typeparam>
		/// <typeparam name="TResult">Type of result</typeparam>
		/// <returns>Memoized function</returns>
		[NotNull]
		[Pure]
		public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> Memoize<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(
			[NotNull] this Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> func,
			bool threadSafe = false)
		{
			var map =
				LazyDictionary.Create<ValueTuple<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>, TResult>(
					key => func(key.Item1, key.Item2, key.Item3, key.Item4, key.Item5, key.Item6, key.Item7, key.Item8),
					threadSafe);
			return (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => map[ValueTuple.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8)];
		}

	}
}
