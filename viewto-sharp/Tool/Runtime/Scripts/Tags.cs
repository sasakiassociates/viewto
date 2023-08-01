namespace ViewTo.Connector.Unity
{
	public enum RigStage
	{
		Target,
		Blocker,
		Design,
		Complete

	}

	public enum ObstructedFilters
	{
		Potential,
		Existing,
		Proposed,
		ExistingOverPotential,
		ProposedOverExisting,
		ProposedOverPotential
	}
}