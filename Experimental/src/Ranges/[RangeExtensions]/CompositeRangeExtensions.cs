﻿using System.Collections.Generic;

namespace CodeJam.Ranges
{
	/// <summary>Extension methods for <seealso cref="CompositeRange{T}"/>.</summary>
	public static partial class CompositeRangeExtensions
	{
		/// <summary>Converts sequence of elements to the composite range.</summary>
		/// <typeparam name="T">The type of the range values.</typeparam>
		/// <param name="ranges">The ranges.</param>
		/// <returns>A new composite range.</returns>
		public static CompositeRange<T> ToCompositeRange<T>(this IEnumerable<Range<T>> ranges) => new CompositeRange<T>(ranges);
	}
}