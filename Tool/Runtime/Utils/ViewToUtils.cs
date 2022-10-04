#region

using System;
using System.Collections.Generic;
using UnityEngine;
using ViewObjects;
using ViewObjects.Unity;
using ViewTo.Connector.Unity.Tests;
using Object = UnityEngine.Object;

#endregion

namespace ViewTo.Connector.Unity
{

	[Serializable]
	public class DebugPointValues : DebugValues
	{
		public int index;

		public override string ToString() => $"{index}";

		public bool Compare(RigStage stage, double inputVal)
		{
			var baseVal = 0.0;
			switch (stage)
			{
				case RigStage.Target:
					baseVal = potential;
					break;
				case RigStage.Blocker:
					baseVal = existing;
					break;
				case RigStage.Design:
					baseVal = proposed;
					break;
				case RigStage.Complete:
					Debug.Log("TEST-Complete Value compared");
					return false;
				default:
					throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
			}

			var res = inputVal.Equals(baseVal);
			Debug.Log($"TEST-Comparing-{ViewToUtils.Point9911} at {stage}. Result={res}\n"
			          + $"Input Value={inputVal}\n"
			          + $"Stored Value={baseVal}");

			return res;
		}
	}

	public static partial class ViewToUtils
	{
		public static readonly DebugPointValues Point9911 = new()
		{
			index = 9911,
			potential = 0.002952709,
			existing = 0.000889157,
			proposed = 0.0005028107
		};


		public static void DrawFrustum(this Camera cam)
		{
			var nearCorners = new Vector3[4]; //Approx'd nearplane corners
			var farCorners = new Vector3[4]; //Approx'd farplane corners
			var camPlanes = GeometryUtility.CalculateFrustumPlanes(cam); //get planes from matrix
			(camPlanes[1], camPlanes[2]) = (camPlanes[2], camPlanes[1]);

			for (var i = 0; i < 4; i++)
			{
				nearCorners[i] = Plane3Intersect(camPlanes[4], camPlanes[i], camPlanes[(i + 1) % 4]); //near corners on the created projection matrix
				farCorners[i] = Plane3Intersect(camPlanes[5], camPlanes[i], camPlanes[(i + 1) % 4]); //far corners on the created projection matrix
			}

			for (var i = 0; i < 4; i++)
			{
				Debug.DrawLine(nearCorners[i], nearCorners[(i + 1) % 4], Color.red, Time.deltaTime, true); //near corners on the created projection matrix
				Debug.DrawLine(farCorners[i], farCorners[(i + 1) % 4], Color.blue, Time.deltaTime, true); //far corners on the created projection matrix
				Debug.DrawLine(nearCorners[i], farCorners[i], Color.green, Time.deltaTime, true); //sides of the created projection matrix
			}
		}

		static Vector3 Plane3Intersect(Plane p1, Plane p2, Plane p3) =>
			//get the intersection point of 3 planes
			(-p1.distance * Vector3.Cross(p2.normal, p3.normal)
			 + -p2.distance * Vector3.Cross(p3.normal, p1.normal)
			 + -p3.distance * Vector3.Cross(p1.normal, p2.normal))
			/ Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal));

		public static RigStage GetNextStage(this RigStage s)
		{
			return s switch
			{
				RigStage.Target => RigStage.Blocker,
				RigStage.Blocker => RigStage.Design,
				RigStage.Design => RigStage.Complete,
				RigStage.Complete => RigStage.Complete,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public static bool Sim(this ViewColor a, ViewColor b) => a.R == b.R && a.G == b.G && a.B == b.B;


		public static List<ViewCloud> LocateViewCloud(string[] ids)
		{
			var clouds = new List<ViewCloud>();
			foreach (var id in ids)
			{
				var sceneObj = ViewObject.TryFetchInScene<ViewCloud>(id);

				if (sceneObj != null)
					clouds.Add(sceneObj);
			}

			return clouds;
		}

		public static IViewCloud TryFetchCloud(string idToFind)
		{
			foreach (var monoToCheck in Object.FindObjectsOfType<ViewObjectMono>())
				if (monoToCheck.GetType().CheckForInterface<IViewCloud>())
					try
					{
						if (monoToCheck is IViewCloud obj
						    && obj.ViewId.Valid()
						    && obj.ViewId.Equals(idToFind))
							return obj;
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						throw;
					}

			return default;
		}

		public static Texture2D DrawPixelLine(this Color32[] c, bool readAlpha = false)
		{
			var tempTexture = new Texture2D(c.Length, 1);

			for (var x = 0; x < tempTexture.width; x++)
			{
				var temp = !readAlpha ? new Color32(c[x].r, c[x].g, c[x].b, 255) : new Color32(c[x].r, c[x].g, c[x].b, c[x].a);

				tempTexture.SetPixel(x, 0, temp);
			}

			tempTexture.Apply();
			return tempTexture;
		}

		public static void DebugDraw(this Bounds b, float delay = 0)
		{
			// bottom
			var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
			var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
			var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
			var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

			Debug.DrawLine(p1, p2, Color.blue, delay);
			Debug.DrawLine(p2, p3, Color.red, delay);
			Debug.DrawLine(p3, p4, Color.yellow, delay);
			Debug.DrawLine(p4, p1, Color.magenta, delay);

			// top
			var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
			var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
			var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
			var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

			Debug.DrawLine(p5, p6, Color.blue, delay);
			Debug.DrawLine(p6, p7, Color.red, delay);
			Debug.DrawLine(p7, p8, Color.yellow, delay);
			Debug.DrawLine(p8, p5, Color.magenta, delay);

			// sides
			Debug.DrawLine(p1, p5, Color.white, delay);
			Debug.DrawLine(p2, p6, Color.gray, delay);
			Debug.DrawLine(p3, p7, Color.green, delay);
			Debug.DrawLine(p4, p8, Color.cyan, delay);
		}
	}
}