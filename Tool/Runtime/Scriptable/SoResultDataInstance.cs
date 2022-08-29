#region

using System.Collections.Generic;
using UnityEngine;
using ViewObjects;

#endregion

public class SoResultDataInstance : ScriptableObject
{

	[SerializeField]
	List<ResultDataUnity> data;

	public List<ResultDataUnity> GetData
	{
		get => data;
	}

	public void Init(List<IResultData> results)
	{
		data = new List<ResultDataUnity>();

		foreach (var r in results)
			data.Add(new ResultDataUnity
			{
				values = r.values,
				stage = r.stage,
				content = r.content,
				color = r.color,
				meta = r.meta,
				layout = r.layout
			});
	}
}