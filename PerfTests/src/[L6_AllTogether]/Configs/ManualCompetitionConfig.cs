﻿using System;

using BenchmarkDotNet.Configs;

using CodeJam.PerfTests.Running.Core;
using CodeJam.PerfTests.Running.Limits;

using JetBrains.Annotations;

namespace CodeJam.PerfTests.Configs
{
	/// <summary>Class for competition config config creation</summary>
	[PublicAPI]
	public sealed class ManualCompetitionConfig : ManualConfig, ICompetitionConfig
	{
		#region Ctor & Add()
		/// <summary>Initializes a new instance of the <see cref="ManualCompetitionConfig"/> class.</summary>
		public ManualCompetitionConfig() { }

		/// <summary>Initializes a new instance of the <see cref="ManualCompetitionConfig"/> class.</summary>
		/// <param name="config">The config to init from.</param>
		public ManualCompetitionConfig([CanBeNull] IConfig config)
		{
			Add(config);
		}

		// TODO: as override
		/// <summary>Fills properties from the specified config.</summary>
		/// <param name="config">The config to init from.</param>
		public new void Add([CanBeNull] IConfig config)
		{
			if (config == null)
				return;

			var competitionConfig = config as ICompetitionConfig;
			if (competitionConfig != null)
			{
				base.Add(config);
				AddCompetitionProperties(competitionConfig);
			}
			else
			{
				base.Add(config);
			}
		}

		private void AddCompetitionProperties([NotNull] ICompetitionConfig config)
		{
			//Runner config - troubleshooting
			AllowDebugBuilds = config.AllowDebugBuilds;
			DetailedLogging = config.DetailedLogging;

			// Runner config
			MaxRunsAllowed = config.MaxRunsAllowed;
			ReportWarningsAsErrors = config.ReportWarningsAsErrors;
			ConcurrentRunBehavior = config.ConcurrentRunBehavior;

			// Validation config
			IgnoreExistingAnnotations = config.IgnoreExistingAnnotations;
			AllowLongRunningBenchmarks = config.AllowLongRunningBenchmarks;
			RerunIfLimitsFailed = config.RerunIfLimitsFailed;
			LogCompetitionLimits = config.LogCompetitionLimits;
			CompetitionLimitProvider = config.CompetitionLimitProvider;

			// Annotation config
			UpdateSourceAnnotations = config.UpdateSourceAnnotations;
			PreviousRunLogUri = config.PreviousRunLogUri;
		}
		#endregion

		#region Runner config - troubleshooting
		/// <summary>Allow debug builds to be used in competitions.</summary>
		/// <value><c>true</c> if debug builds allowed; otherwise, <c>false</c>.</value>
		public bool AllowDebugBuilds { get; set; }

		/// <summary>Enable detailed logging.</summary>
		/// <value><c>true</c> if detailed logging enabled.</value>
		public bool DetailedLogging { get; set; }
		#endregion

		#region Runner config
		/// <summary>
		/// Total count of reruns allowed. Set this to zero to use default limit of 10 runs.
		/// (limit value can be overriden by <see cref="CodeJam.PerfTests.Running.Core.CompetitionRunnerBase"/> implementation).
		/// </summary>
		/// <value>The upper limits of rerun count.</value>
		public int MaxRunsAllowed { get; set; }

		/// <summary>Report warnings as errors.</summary>
		/// <value><c>true</c> if competition warnings should be reported as errors; otherwise, <c>false</c>.</value>
		public bool ReportWarningsAsErrors { get; set; }

		/// <summary>Behavior for concurrent competition runs.</summary>
		/// <value>Behavior for concurrent competition runs.</value>
		public ConcurrentRunBehavior ConcurrentRunBehavior { get; set; }
		#endregion

		#region Validation config
		/// <summary>The analyser should ignore existing limit annotations.</summary>
		/// <value><c>true</c> if the analyser should ignore existing limit annotations.</value>
		public bool IgnoreExistingAnnotations { get; set; }

		/// <summary>
		/// The analyser should not warn on benchmark runs that take longer than 0.5 sec to complete.
		/// (limit value can be overriden by <see cref="CodeJam.PerfTests.Running.Core.CompetitionRunnerBase"/> implementation).
		/// </summary>
		/// <value>
		/// True if the analyser should not warn on benchmark runs that take longer than 0.5 sec to complete.
		/// </value>
		public bool AllowLongRunningBenchmarks { get; set; }

		/// <summary>Rerun competition if competition limits check failed.</summary>
		/// <value><c>true</c> if reruns should be performed if competition limits check failed.</value>
		public bool RerunIfLimitsFailed { get; set; }

		/// <summary>Log competition limits.</summary>
		/// <value><c>true</c> if competition limits should be logged; otherwise, <c>false</c>.</value>
		public bool LogCompetitionLimits { get; set; }

		/// <summary>Competition limit provider.</summary>
		/// <value>The competition limit provider.</value>
		public ICompetitionLimitProvider CompetitionLimitProvider { get; set; }
		#endregion

		#region Annotation config
		/// <summary>Try to update source annotations if competition limits check failed.</summary>
		/// <value>
		/// <c>true</c> if the analyser should update source annotations if competition limits check failed; otherwise, <c>false</c>.
		/// </value>
		public bool UpdateSourceAnnotations { get; set; }

		/// <summary>
		/// URI of the log that contains competition limits from previous run(s).
		/// Relative paths, absolute paths and web URLs are supported.
		/// If <see cref="UpdateSourceAnnotations"/> set to <c>true</c>, the annotations will be updated with limits from the log.
		/// Set <see cref="LogCompetitionLimits"/> <c>true</c> to log the limits.
		/// </summary>
		/// <value>The URI of the log that contains competition limits from previous run(s).</value>
		public string PreviousRunLogUri { get; set; }
		#endregion

		/// <summary>Returns read-only wrapper for the config.</summary>
		/// <returns>Read-only wrapper for the config</returns>
		public ICompetitionConfig AsReadOnly() => new ReadOnlyCompetitionConfig(this);
	}
}