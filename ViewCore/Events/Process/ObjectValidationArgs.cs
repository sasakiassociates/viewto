using ViewTo.Events.Args;

namespace ViewTo.Events.Process
{

	public class ObjectValidationArgs : AEventArgs
	{
		public readonly string message;
		public readonly bool result;

		public ObjectValidationArgs(object objType, bool value)
		{
			result = value;
			message = $"Object Validation Check : {objType.GetType()}" + (value ? "Valid" : "Not Valid");
		}
	}
}