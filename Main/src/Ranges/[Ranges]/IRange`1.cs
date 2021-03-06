﻿using System;

using JetBrains.Annotations;

namespace CodeJam.Ranges
{
	/// <summary>Common interface for different range implementations</summary>
	/// <typeparam name="T">
	/// The type of the value. Should implement <seealso cref="IComparable{T}"/> or <seealso cref="IComparable"/>.
	/// </typeparam>
	[PublicAPI]
	public interface IRange<T>
	{
		/// <summary>Boundary From. Limits the values from the left.</summary>
		/// <value>Boundary From.</value>
		RangeBoundaryFrom<T> From { get; }

		/// <summary>Boundary To. Limits the values from the right.</summary>
		/// <value>Boundary To.</value>
		RangeBoundaryTo<T> To { get; }

		/// <summary>The range is empty, ∅.</summary>
		/// <value><c>true</c> if the range is empty; otherwise, <c>false</c>.</value>
		bool IsEmpty { get; }

		/// <summary>The range is NOT empty, ≠ ∅</summary>
		/// <value><c>true</c> if the range is not empty; otherwise, <c>false</c>.</value>
		bool IsNotEmpty { get; }
	}
}