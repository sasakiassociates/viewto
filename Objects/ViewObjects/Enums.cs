namespace ViewObjects
{
	public enum ContentType
	{
		Undefined = 0,
		Target = 1,
		Blocker = 2,
		Option = 3,
	}

	public enum ResultType
	{
		Undefined = 0,
		Potential = 1,
		Existing = 2,
		Proposed = 3
	}
	
	public enum ExplorerValueType
	{
		ExistingOverPotential,
		ProposedOverExisting,
		ProposedOverPotential,
		
	}

}