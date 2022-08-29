#region

using System;
using UnityEngine;

#endregion

namespace ViewTo.Connector.Unity.Tests
{
	[AttributeUsage(AttributeTargets.Field)]
	public class MultiLineDisplayAttribute : PropertyAttribute
	{
		public MultiLineDisplayAttribute(int value) => ValueCount = value;

		public int ValueCount { get; }
	}
}