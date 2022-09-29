using System;
using ViewTo.Events.Args;

namespace ViewTo.Events.Process
{
	public class InvalidObjectConversion : AEventArgs
	{
		public readonly string message;

		public InvalidObjectConversion(object input, Type toType) => message = $"Invalid Conversion! Tried Casting: {input.GetType()} To Type: {toType} ";
	}
}