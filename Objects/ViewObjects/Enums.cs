namespace ViewObjects
{
	public enum ContentType
	{
		Target = 0,
		Blocker = 1,
		Option = 2,
	}

	public enum ResultStage
	{
		Potential = 0,
		Existing = 1,
		Proposed = 2
	}

	public enum ExplorerValueType
	{
		ExistingOverPotential,
		ProposedOverExisting,
		ProposedOverPotential,

	}

}