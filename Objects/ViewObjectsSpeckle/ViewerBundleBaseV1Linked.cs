using System.Collections.Generic;
using Speckle.Core.Models;
using Speckle.Newtonsoft.Json;

namespace ViewObjects.Speckle
{
	public class ViewerBundleBaseV1Linked : ViewerBundleBaseV1
	{

		public ViewerBundleBaseV1Linked()
		{ }

		[JsonIgnore] public override bool isValid => base.isValid && linkedClouds.Valid();

		[DetachProperty] public List<ViewCloudBaseV1> linkedClouds { get; set; }

		/// <summary>
		///   temporary list for storing view cloud info to find
		/// </summary>
		public List<string> cloudsToFind { get; set; }
	}
}