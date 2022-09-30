using System;

namespace ViewObjects.Cloud
{

	[Serializable]
	public readonly struct CloudShell
	{
		public CloudShell(Type objType, string objId, int count)
		{
			this.count = count;
			this.objId = objId;
			this.objType = objType;
		}

		public CloudShell(object objType, string objId, int count)
		{
			this.count = count;
			this.objId = objId;
			this.objType = objType?.GetType();
		}

		public Type objType { get; }

		public string objId { get; }

		public int count { get; }
	}

}