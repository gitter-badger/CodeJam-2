﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess;

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace

namespace BenchmarkDotNet.Validators
{
	/// <summary>
	/// Validator to be used together with <see cref="InProcessToolchain"/>
	/// to proof that the config matches the environment.
	/// </summary>
	/// <seealso cref="IValidator"/>
	[PublicAPI]
	[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
	public class InProcessValidator : IValidator
	{
		#region Validation rules
		// ReSharper disable HeapView.DelegateAllocation
		private static readonly IReadOnlyDictionary<string, Func<IJob, EnvironmentInfo, string>> _validationRules =
			new Dictionary<string, Func<IJob, EnvironmentInfo, string>>
			{
				{ nameof(IJob.Affinity), DontValidate },
				{ nameof(IJob.Framework), ValidateFramework },
				{ nameof(IJob.IterationTime), DontValidate },
				{ nameof(IJob.Jit), ValidateJit },
				{ nameof(IJob.LaunchCount), DontValidate },
				{ nameof(IJob.Mode), DontValidate },
				{ nameof(IJob.Platform), ValidatePlatform },
				{ nameof(IJob.Runtime), ValidateRuntime },
				{ nameof(IJob.TargetCount), DontValidate },
				{ nameof(IJob.Toolchain), ValidateToolchain },
				{ nameof(IJob.WarmupCount), DontValidate }
			};

		// ReSharper restore HeapView.DelegateAllocation

		private static string DontValidate(IJob job, EnvironmentInfo env) => null;

		// TODO: detect framework
		private static string ValidateFramework(IJob job, EnvironmentInfo env)
		{
			switch (job.Framework)
			{
				case Framework.Host:
					return null;
				case Framework.V40:
				case Framework.V45:
				case Framework.V451:
				case Framework.V452:
				case Framework.V46:
				case Framework.V461:
				case Framework.V462:
					return $"Should be set to {nameof(Framework.Host)}.";
				default:
					throw new ArgumentOutOfRangeException(nameof(job.Framework), job.Framework, null);
			}
		}

		private static string ValidateJit(IJob job, EnvironmentInfo env)
		{
			bool isX64 = env.Architecture == "64-bit";
			switch (job.Jit)
			{
				case Jit.Host:
					return null;
				case Jit.LegacyJit:
					return !isX64 || !env.HasRyuJit
						? null
						: "The current setup does not support legacy Jit.";
				case Jit.RyuJit:
					return isX64 && env.HasRyuJit
						? null
						: "The current setup does not support RyuJit.";
				default:
					throw new ArgumentOutOfRangeException(nameof(job.Jit), job.Jit, null);
			}
		}

		private static string ValidatePlatform(IJob job, EnvironmentInfo env)
		{
			bool isX64 = env.Architecture == "64-bit";
			switch (job.Platform)
			{
				case Platform.Host:
				case Platform.AnyCpu:
					return null;
				case Platform.X86:
					return !isX64
						? null
						: "The code should be run as x86.";
				case Platform.X64:
					return isX64
						? null
						: "The code should be run as x64.";
				default:
					throw new ArgumentOutOfRangeException(nameof(job.Platform), job.Platform, null);
			}
		}

		// TODO: detect runtime
		private static string ValidateRuntime(IJob job, EnvironmentInfo env)
		{
			switch (job.Runtime)
			{
				case Runtime.Host:
					return null;
				case Runtime.Clr:
				case Runtime.Mono:
				case Runtime.Dnx:
				case Runtime.Core:
					return $"Should be set to {nameof(Runtime.Host)}.";
				default:
					throw new ArgumentOutOfRangeException(nameof(job.Runtime), job.Runtime, null);
			}
		}

		private static string ValidateToolchain(IJob job, EnvironmentInfo env) =>
			job.Toolchain is InProcessToolchain
				? null
				: $"Should be instance of {nameof(InProcessToolchain)}.";
		#endregion

		/// <summary>The instance of validator that does NOT fail on error.</summary>
		public static readonly IValidator DontFailOnError = new InProcessValidator(false);

		/// <summary>The instance of validator that DOES fail on error.</summary>
		public static readonly IValidator FailOnError = new InProcessValidator(true);

		private InProcessValidator(bool failOnErrors)
		{
			TreatsWarningsAsErrors = failOnErrors;
		}

		/// <summary>Gets a value indicating whether warnings are treated as errors.</summary>
		/// <value>
		/// <c>true</c> if treats warnings as errors; otherwise, <c>false</c>.
		/// </value>
		public bool TreatsWarningsAsErrors { get; }

		// TODO: check that all diagnosers can be run in-process
		/// <summary>Proofs that benchmarks' jobs match the environment.</summary>
		/// <param name="benchmarks">The benchmarks to validate.</param>
		/// <returns>Enumerable of validation errors.</returns>
		public IEnumerable<ValidationError> Validate(IList<Benchmark> benchmarks)
		{
			var result = new List<ValidationError>();

			var env = EnvironmentInfo.GetCurrent();
			foreach (var job in benchmarks.GetJobs())
			{
				foreach (var jobProperty in job.AllProperties)
				{
					Func<IJob, EnvironmentInfo, string> validationRule;
					if (_validationRules.TryGetValue(jobProperty.Name, out validationRule))
					{
						var message = validationRule(job, env);
						if (!string.IsNullOrEmpty(message))
						{
							result.Add(
								new ValidationError(
									TreatsWarningsAsErrors,
									$"Job {job.GetShortInfo()}, property {jobProperty.Name}: {message}"));
						}
					}
					else
					{
						result.Add(
							new ValidationError(
								false,
								$"Job {job.GetShortInfo()}, property {jobProperty.Name}: no validation rule specified."));
					}
				}
			}

			return result.ToArray();
		}
	}
}