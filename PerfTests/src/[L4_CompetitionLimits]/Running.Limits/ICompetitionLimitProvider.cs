﻿using System;

using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

using JetBrains.Annotations;

namespace CodeJam.PerfTests.Running.Limits
{
	/// <summary>Interface for competition limit provider.</summary>
	public interface ICompetitionLimitProvider
	{
		/// <summary>Short description for the provider.</summary>
		/// <value>The short description for the provider.</value>
		string ShortInfo { get; }

		/// <summary>Actual values for the benchmark.</summary>
		/// <param name="summary">Summary for the run.</param>
		/// <param name="benchmark">The benchmark.</param>
		/// <returns>Actual values for the benchmark or <c>null</c> if none.</returns>
		[CanBeNull]
		CompetitionLimit TryGetActualValues(Benchmark benchmark, Summary summary);

		/// <summary>Limits for the benchmark.</summary>
		/// <param name="benchmark">The benchmark.</param>
		/// <param name="summary">Summary for the run.</param>
		/// <returns>Limits for the benchmark or <c>null</c> if none.</returns>
		[CanBeNull]
		CompetitionLimit TryGetCompetitionLimit(Benchmark benchmark, Summary summary);
	}
}