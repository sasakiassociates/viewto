#region

using System;
using System.Collections.Generic;
using ViewObjects;
using ViewTo;

#endregion

[Serializable]
public struct ResultDataSample
{

	public readonly List<double> existing;

	public readonly List<double> potential;

	public readonly List<double> proposed;

	public readonly string target;

	public ResultDataSample(IResultExplorer obj)
	{
		target = obj.activeTarget;
		potential = SafeGetValues(obj, ResultType.Potential);
		existing = SafeGetValues(obj, ResultType.Existing);
		proposed = SafeGetValues(obj, ResultType.Proposed);
	}

	public double Get(ResultType type, int index)
	{
		return type switch
		{
			ResultType.Potential => potential.Valid(index) ? potential[index] : 0,
			ResultType.Existing => existing.Valid(index) ? existing[index] : 0,
			ResultType.Proposed => proposed.Valid(index) ? proposed[index] : 0,
			_ => 0
		};
	}

	public static List<double> SafeGetValues(IResultExplorer obj, ResultType type)
	{
		var data = new List<double>();

		obj.TryGetValues(type, ref data);

		return data.Valid() ? data : new List<double>();
	}
}