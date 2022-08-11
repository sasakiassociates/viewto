using System;

namespace ViewObjects
{
	public struct ResultPoint
	{
		public string meta;

		public double x, y, z, xn, yn, zn;

		public ResultPoint(double x, double y, double z, double xn = 0, double yn = 0, double zn = 0, string meta = "")
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.xn = xn;
			this.yn = yn;
			this.zn = zn;
			this.meta = meta;
		}
	}
	
	[Serializable]
	public struct CloudPoint
	{
		public string meta;

		public double x, y, z, xn, yn, zn;

		public CloudPoint(double x, double y, double z, double xn = 0, double yn = 0, double zn = 0, string meta = "")
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.xn = xn;
			this.yn = yn;
			this.zn = zn;
			this.meta = meta;
		}
	}

}