using System;
using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewObjects.Explorer;
using ViewTo.Cmd;

namespace ViewTo
{

	public static partial class ViewCoreExtensions
	{
		/// <summary>
		///   Retrieves the active point result data
		/// </summary>
		/// <returns></returns>
		public static ResultPoint GetResultPoint(this IExplorer explorer) => new ResultPoint();

		/// <summary>
		/// Get the valuess 
		/// </summary>
		/// <param name="exp"></param>
		/// <param name="valueType"></param>
		/// <param name="results"></param>
		/// <returns></returns>
		public static bool TryGetValues(this IExplorer exp, ExplorerValueType valueType, out IEnumerable<double> results)
		{
			results = Array.Empty<double>();
			valueType.GetStages(out var stageA, out var stageB);

			var getValueCmdA = new TryGetValuesCmd(exp.Data, exp.ActiveOption.Id, stageA);
			var getValueCmdB = new TryGetValuesCmd(exp.Data, exp.ActiveOption.Id, stageB);
			getValueCmdA.Run();
			getValueCmdB.Run();

			if (!getValueCmdA.args.IsValid() || !getValueCmdB.args.IsValid())
			{
				// TODO: return issue
				return false;
			}

			var normalizeCmd = new NormalizeValuesCmd(getValueCmdA.args.values, getValueCmdB.args.values);
			normalizeCmd.Run();

			if (!normalizeCmd.args.IsValid())
			{
				// TODO: report
				return false;
			}

			results = normalizeCmd.args.values;

			return results.Any();
		}

		public static void GetStages(this ExplorerValueType type, out ResultStage stageA, out ResultStage stageB)
		{
			switch (type)
			{
				case ExplorerValueType.ExistingOverPotential:
					stageA = ResultStage.Existing;
					stageB = ResultStage.Potential;
					break;
				case ExplorerValueType.ProposedOverExisting:
					stageA = ResultStage.Proposed;
					stageB = ResultStage.Existing;
					break;
				case ExplorerValueType.ProposedOverPotential:
					stageA = ResultStage.Proposed;
					stageB = ResultStage.Potential;
					break;
				default:
					stageA = ResultStage.Potential;
					stageB = ResultStage.Potential;
					break;
			}
		}

		public static bool InRange(this IExploreRange obj, double value) => value >= obj.min && value <= obj.max;

	}

}