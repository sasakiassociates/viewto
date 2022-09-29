using Speckle.Core.Models;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// </summary>
	public abstract class ViewObjectBase : Base, IViewObj
	{

		/// <summary>
		/// </summary>
		public ViewObjectBase()
		{ }

		/// <summary>
		///   Returns the assembly type
		/// </summary>
		public override string speckle_type
		{
			get => GetType().ToString();
		}
	}

}