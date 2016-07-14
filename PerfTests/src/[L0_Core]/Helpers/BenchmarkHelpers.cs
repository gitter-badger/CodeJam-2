﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Portability;

using CodeJam;

using JetBrains.Annotations;

// ReSharper disable once CheckNamespace

namespace BenchmarkDotNet.Helpers
{
	// TODO: move to different classes
	/// <summary>
	/// Helper methods for benchmark infrastructure.
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "ArrangeBraces_using")]
	[SuppressMessage("ReSharper", "ArrangeBraces_while")]
	public static class BenchmarkHelpers
	{
		#region Benchmark-related
		/// <summary>Get benchmark types defined in the assembly.</summary>
		/// <param name="assembly">The assembly to get benchmarks from.</param>
		/// <returns>Benchmark types from the assembly</returns>
		public static Type[] GetBenchmarkTypes([NotNull] System.Reflection.Assembly assembly) =>
			// Use reflection for a more maintainable way of creating the benchmark switcher,
			// Benchmarks are listed in namespace order first (e.g. BenchmarkDotNet.Samples.CPU,
			// BenchmarkDotNet.Samples.IL, etc) then by name, so the output is easy to understand.
			assembly
				.GetTypes()
				.Where(
					t =>
						t.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
							.Any(m => m.GetCustomAttributes<BenchmarkAttribute>(true).Any()))
				.Where(t => !t.IsGenericType && !t.IsAbstract)
				.OrderBy(t => t.Namespace)
				.ThenBy(t => t.Name)
				.ToArray();

		/// <summary>Creates read-only wrapper for the config.</summary>
		/// <param name="config">The config to wrap.</param>
		/// <returns>Read-only wrapper for the config.</returns>
		public static IConfig AsReadOnly(this IConfig config) => new ReadOnlyConfig(config);

		#region Selects
		/// <summary>Returns jobs from the benchmarks.</summary>
		/// <param name="benchmarks">The benchmarks to select jobs from.</param>
		/// <returns>Jobs from the benchmarks.</returns>
		// ReSharper disable once ReturnTypeCanBeEnumerable.Global
		public static IOrderedEnumerable<IJob> GetJobs([NotNull] this IEnumerable<Benchmark> benchmarks) =>
			benchmarks
				.Select(d => d.Job)
				.Distinct()
				.OrderBy(d => d.GetShortInfo());

		/// <summary>Returns benchmarks for the summary sorted by execution order.</summary>
		/// <param name="summary">Summary for the run.</param>
		/// <returns>Benchmarks for the summary.</returns>
		public static IEnumerable<Benchmark> GetExecutionOrderBenchmarks([NotNull] this Summary summary)
		{
			var orderProvider = summary.Config.GetOrderProvider() ?? DefaultOrderProvider.Instance;
			return orderProvider.GetExecutionOrder(summary.Benchmarks);
		}

		/// <summary>Returns benchmarks for the summary sorted by display order.</summary>
		/// <param name="summary">Summary for the run.</param>
		/// <returns>Benchmarks for the summary.</returns>
		public static IEnumerable<Benchmark> GetSummaryOrderBenchmarks([NotNull] this Summary summary)
		{
			var orderProvider = summary.Config.GetOrderProvider() ?? DefaultOrderProvider.Instance;
			return orderProvider.GetSummaryOrder(summary.Benchmarks, summary);
		}

		/// <summary>Returns the baseline for the benchmark.</summary>
		/// <param name="summary">Summary for the run.</param>
		/// <param name="benchmark">The benchmark.</param>
		/// <returns>Baseline for the benchmark</returns>
		public static Benchmark TryGetBaseline(
			[NotNull] this Summary summary,
			[NotNull] Benchmark benchmark) =>
				summary.Benchmarks
					.Where(b => b.Job == benchmark.Job && b.Parameters == benchmark.Parameters)
					.FirstOrDefault(b => b.Target.Baseline);

		/// <summary>Returns the report for the benchmark or <c>null</c> if there's no report.</summary>
		/// <param name="summary">Summary for the run.</param>
		/// <param name="benchmark">The benchmark.</param>
		/// <returns>The report for the benchmark or <c>null</c> if there's no report.</returns>
		public static BenchmarkReport TryGetBenchmarkReport(
			[NotNull] this Summary summary,
			[NotNull] Benchmark benchmark) =>
				summary.Reports.SingleOrDefault(r => r.Benchmark == benchmark);
		#endregion

		/// <summary>Gets the value of the current TimeSpan structure expressed in nanoseconds.</summary>
		/// <param name="timeSpan">The timespan.</param>
		/// <returns>The total number of nanoseconds represented by this instance.</returns>
		public static double TotalNanoseconds(this TimeSpan timeSpan) => timeSpan.Ticks * (1.0e9 / TimeSpan.TicksPerSecond);
		#endregion

		#region Loggers
		private const int SeparatorLength = 42;

		/// <summary>Helper method that writes separator log line.</summary>
		/// <param name="logger">The logger.</param>
		public static void WriteSeparatorLine([NotNull] this ILogger logger) =>
			WriteSeparatorLine(logger, null, false);

		/// <summary>Helper method that writes separator log line.</summary>
		/// <param name="logger">The logger.</param>
		/// <param name="header">The separator line text.</param>
		public static void WriteSeparatorLine([NotNull] this ILogger logger, [CanBeNull] string header) =>
			WriteSeparatorLine(logger, header, false);

		/// <summary>Helper method that writes separator log line.</summary>
		/// <param name="logger">The logger.</param>
		/// <param name="header">The separator line text.</param>
		/// <param name="topHeader">Write top-level header.</param>
		public static void WriteSeparatorLine(
			[NotNull] this ILogger logger, [CanBeNull] string header, bool topHeader)
		{
			var separatorChar = topHeader ? '=' : '-';
			var logKind = topHeader ? LogKind.Header : LogKind.Help;

			var result = new StringBuilder(SeparatorLength);

			if (!string.IsNullOrEmpty(header))
			{
				var temp = (SeparatorLength - header.Length - 2) / 2;
				if (temp > 0)
				{
					result.Append(separatorChar, temp);
				}
				result.Append(' ').Append(header).Append(' ');
			}

			var temp2 = SeparatorLength - result.Length;
			if (temp2 > 0)
			{
				result.Append(separatorChar, temp2);
			}

			logger.WriteLine();
			logger.WriteLine(logKind, result.ToString());
		}
		#endregion

		#region Reflection
		/// <summary>Checks that the assembly is build in debug mode.</summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns><c>true</c> if the assembly was build with optimizations disabled.</returns>
		public static bool IsDebugAssembly([NotNull] this System.Reflection.Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			var optAtt = (DebuggableAttribute)Attribute.GetCustomAttribute(assembly, typeof(DebuggableAttribute));
			return optAtt != null && optAtt.IsJITOptimizerDisabled;
		}

		/// <summary>Gets the short form of assembly qualified type name.</summary>
		/// <param name="type">The type to get the name for.</param>
		/// <returns>The short form of assembly qualified type name.</returns>
		public static string GetShortAssemblyQualifiedName([NotNull] this Type type) =>
			type.FullName + ", " + type.Assembly.GetName().Name;

		/// <summary>
		/// Performs search for metadata attribute in the following order:
		/// * type attributes
		/// * container type attributes (if the <paramref name="type"/> is nested type)
		/// * assembly attributes.
		/// </summary>
		/// <typeparam name="TAttribute">Type of the attribute to search for.</typeparam>
		/// <param name="type">Type the attribute is searched for.</param>
		/// <returns>Metadata attribute for the type.</returns>
		public static TAttribute TryGetMetadataAttribute<TAttribute>([NotNull] this Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			var tempType = type;
			while (tempType != null)
			{
				var typeAttribute = tempType
					.GetCustomAttributes<TAttribute>(true)
					.FirstOrDefault();

				if (typeAttribute != null)
					return typeAttribute;

				tempType = tempType.DeclaringType;
			}

			return type.Assembly
				.GetCustomAttributes<TAttribute>(true)
				.FirstOrDefault();
		}
		#endregion

		#region Process
		/// <summary>Changes the priority of the process.</summary>
		/// <param name="process">The target process.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="logger">The logger.</param>
		public static void SetPriority(
			[NotNull] this Process process,
			ProcessPriorityClass priority,
			[NotNull] ILogger logger)
		{
			if (process == null)
				throw new ArgumentNullException(nameof(process));

			if (logger == null)
				throw new ArgumentNullException(nameof(logger));

			try
			{
				process.PriorityClass = priority;
			}
			catch (Exception ex)
			{
				logger.WriteLineError(
					string.Format(
						"// ! Failed to set up priority {1}. Make sure you have the right permissions. Message: {0}", ex.Message,
						priority));
			}
		}

		/// <summary>Changes the cpu affinity of the process.</summary>
		/// <param name="process">The target process.</param>
		/// <param name="processorAffinity">The processor affinity.</param>
		/// <param name="logger">The logger.</param>
		public static void SetAffinity(
			[NotNull] this Process process,
			IntPtr processorAffinity,
			[NotNull] ILogger logger)
		{
			if (process == null)
				throw new ArgumentNullException(nameof(process));

			if (logger == null)
				throw new ArgumentNullException(nameof(logger));

			try
			{
				process.ProcessorAffinity = processorAffinity;
			}
			catch (Exception ex)
			{
				logger.WriteLineError(
					string.Format(
						"// ! Failed to set up processor affinity 0x{1:X}. Make sure you have the right permissions. Message: {0}",
						ex.Message,
						(long)processorAffinity));
			}
		}
		#endregion

		#region IO
		/// <summary>Writes file content without empty line at the end.</summary>
		/// <param name="path">The path.</param>
		/// <param name="lines">The lines to write.</param>
		// THANKSTO: http://stackoverflow.com/a/11689630
		public static void WriteFileContent(string path, string[] lines)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));
			if (lines == null)
				throw new ArgumentNullException(nameof(lines));

			using (var writer = File.CreateText(path))
			{
				if (lines.Length > 0)
				{
					for (var i = 0; i < lines.Length - 1; i++)
					{
						writer.WriteLine(lines[i]);
					}
					writer.Write(lines[lines.Length - 1]);
				}
			}
		}

		/// <summary>Tries to obtain text from the given URI.</summary>
		/// <param name="uri">The URI to geth the text from.</param>
		/// <returns>The text reader or <c>null</c> if none.</returns>
		public static TextReader TryGetTextFromUri(string uri) =>
			TryGetTextFromUri(uri, null);

		/// <summary>Tries to obtain text from the given URI.</summary>
		/// <param name="uri">The URI to geth the text from.</param>
		/// <param name="timeOut">The timeout.</param>
		/// <returns>The text reader or <c>null</c> if none.</returns>
		public static TextReader TryGetTextFromUri(string uri, TimeSpan? timeOut)
		{
			if (uri == null)
				throw new ArgumentNullException(nameof(uri));

			var uriInst = new Uri(uri, UriKind.RelativeOrAbsolute);
			if (uriInst.IsAbsoluteUri && !uriInst.IsFile)
			{
				try
				{
					var webRequest = WebRequest.Create(uriInst);
					if (timeOut != null)
					{
						webRequest.Timeout = (int)timeOut.Value.TotalMilliseconds;
					}
					using (var response = webRequest.GetResponse())
					using (var content = response.GetResponseStream())
					{
						if (content == null)
							return null;

						using (var reader = new StreamReader(content))
						{
							return new StringReader(reader.ReadToEnd());
						}
					}
				}
				catch (WebException)
				{
					return null;
				}
			}

			var path = uriInst.IsAbsoluteUri ? uriInst.LocalPath : uri;

			return File.Exists(path) ? File.OpenText(path) : null;
		}
		#endregion

		#region Console
		private static readonly Stack<TextWriter> _outputStack = new Stack<TextWriter>();
		private static readonly HashSet<TextWriter> _popCollection = new HashSet<TextWriter>();

		/// <summary>
		/// Sets the console output in a thread-safe manner.
		/// WARNING: DO use only this method to override the output. Job will fail otherwise.
		/// </summary>
		/// <param name="output">The new console output.</param>
		/// <returns><see cref="IDisposable"/> to restore the output</returns>
		public static IDisposable CaptureConsoleOutput(TextWriter output)
		{
			if (output == null)
				throw new ArgumentNullException(nameof(output));

			var old = Console.Out;
			Console.SetOut(output);

			return Disposable.Create(() => Console.SetOut(old));

			var newWriter = TextWriter.Synchronized(output);
			if (ReferenceEquals(newWriter, output))
				throw new ArgumentException("Please pass non-synchronized test writer.", nameof(output));

			SetOutputCore(output);
			return Disposable.Create(() => RestoreOutputCore(output, old));
		}

		private static void SetOutputCore(TextWriter output)
		{
			lock (_outputStack)
			{
				Console.SetOut(output);
				if (!ReferenceEquals(output, Console.Out))
					throw new InvalidOperationException("Internal logic bug.");

				if (_outputStack.Count == 0)
				{
					_outputStack.Push(Console.Out);
				}
				_outputStack.Push(output);
			}
		}

		private static void RestoreOutputCore(TextWriter output, TextWriter old)
		{
			lock (_outputStack)
			{
				_outputStack.Pop();

				Console.SetOut(old);
				return; 
				if (_outputStack.Peek() == output)
				{
					_outputStack.Pop();

					Console.SetOut(_outputStack.Peek());
				}
				else
				{
					_popCollection.Add(output);

					while (_popCollection.Contains(_outputStack.Peek()))
					{
						_popCollection.Remove(_outputStack.Pop());
					}

					Console.SetOut(_outputStack.Peek());
				}
			}
		}
		#endregion
	}
}