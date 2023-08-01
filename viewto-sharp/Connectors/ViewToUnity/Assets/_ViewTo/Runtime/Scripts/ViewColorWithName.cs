#region

using System;
using ViewObjects;
using ViewObjects.Contents;

#endregion

namespace ViewTo.Connector.Unity
{
	[Serializable]
	public class ViewColorWithName : ViewColor
	{

		public string name, id;

		public ViewColorWithName()
		{ }

		public ViewColorWithName(ViewColor color, string name, string id)
		{
			this.name = name;
			this.id = id;

			R = color.R;
			G = color.G;
			B = color.B;
			A = color.A;
		}

		public ViewColorWithName(IContent viewContent)
		{
			id = viewContent.ViewId;
			name = viewContent.ViewName;

			R = viewContent.Color.R;
			G = viewContent.Color.G;
			B = viewContent.Color.B;
			A = viewContent.Color.A;
		}
	}

}