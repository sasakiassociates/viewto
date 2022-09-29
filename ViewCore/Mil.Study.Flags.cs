using System;

namespace ViewTo
{

	public static partial class Study
	{

		public enum LoadError
		{
			MissingObjects,
			UnlinkedClouds,
			MissingTargets,
			ObjectSetup,
			BuildingRig,
			PrimeObjects
		}

		public enum ProgressCheck
		{
			Rig,
			ViewClouds,
			TargetContent,
			ViewerBundle
		}

		internal static string Message(this ProgressCheck input, bool value)
		{
			return input switch
			{
				ProgressCheck.Rig => $"Study rig was built? {value} ",
				ProgressCheck.ViewClouds => $"Study has proper clouds? {value}",
				ProgressCheck.TargetContent => $"Study Targets are valid? {value}",
				ProgressCheck.ViewerBundle => $"Study Bundles are correct? {value}",
				_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
			};
		}

		internal static string Message(this LoadError error)
		{
			return error switch
			{
				LoadError.UnlinkedClouds => "Study had unlinked clouds",
				LoadError.MissingTargets => "Missing Targets Content that is specified in Content Case",
				LoadError.ObjectSetup => "Study did not complete object setup",
				LoadError.BuildingRig => "Rig was not completed during build process",
				LoadError.MissingObjects => "Study is missing objects",
				LoadError.PrimeObjects => "Study did not Prime objects for setup",
				_ => throw new ArgumentOutOfRangeException(nameof(error), error, null)
			};
		}
	}
}