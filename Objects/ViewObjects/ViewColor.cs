using System;

namespace ViewObjects
{
	[Serializable]
	public class ViewColor
	{
		public byte A;
		public byte B;
		public byte G;
		public byte R;

		public ViewColor()
		{ }

		public ViewColor(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}
	}
}