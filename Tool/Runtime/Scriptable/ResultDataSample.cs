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
		potential = SafeGetValues(obj, ResultStage.Potential);
		existing = SafeGetValues(obj, ResultStage.Existing);
		proposed = SafeGetValues(obj, ResultStage.Proposed);
	}

	public double Get(ResultStage type, int index)
	{
		return type switch
		{
			ResultStage.Potential => potential.Valid(index) ? potential[index] : 0,
			ResultStage.Existing => existing.Valid(index) ? existing[index] : 0,
			ResultStage.Proposed => proposed.Valid(index) ? proposed[index] : 0,
			_ => 0
		};
	}

	public static List<double> SafeGetValues(IResultExplorer obj, ResultStage type)
	{
		var data = new List<double>();

		obj.TryGetValues(type, ref data);

		return data.Valid() ? data : new List<double>();
	}
}