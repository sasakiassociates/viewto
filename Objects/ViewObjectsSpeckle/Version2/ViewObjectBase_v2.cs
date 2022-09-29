using Speckle.Core.Models;

namespace ViewObjects.Speckle
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ViewObjectBase_v2 : Base, IViewObj
	{
		/// <summary>
		/// Returns the assembly type
		/// </summary>
		public override string speckle_type => this.GetType().ToString();

		/// <summary>
		/// 
		/// </summary>
		public ViewObjectBase_v2()
		{ }
	}

}