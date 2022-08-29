#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViewObjects;

#endregion

namespace ViewTo.Connector.Unity
{
	// TODO: Test getting the same value 3 times from the same point
	// TODO: Test getting data from a point before and after moving the camera
	// TODO: Test how to control the flog

	[RequireComponent(typeof(Camera))]
	// [RequireComponent(typeof(ViewProcessor))]
	[ExecuteAlways]
	public class ViewerDebugger : MonoBehaviour, IViewCam
	{

		const int ViewSize = 512;
		const int Depth = 16;
		const int RerunCount = 3;
		[Header("|| Runtime ||")]
		[SerializeField]
		bool start;
		[SerializeField]
		[Range(90, 300)]
		int fps = 90;
		[Space]
		[Header("|| Setup ||")]
		[SerializeField]
		bool rerunOnPoint;
		[SerializeField]
		bool trashFirstPoint;
		[SerializeField]
		bool forceRender;
		[SerializeField]
		[Range(1, 10)]
		int amountToRerun = 3;
		[SerializeField]
		[Range(-10, 0)]
		int amountToOffset = -3;
		[SerializeField]
		ComputeShader shader;
		[SerializeField]
		Color32 color;
		[SerializeField]
		int pointCount;

		[SerializeField]
		[HideInInspector]
		bool isRunning, cameraDone;
		[SerializeField]
		[HideInInspector]
		Camera cam;
		// [SerializeField, HideInInspector] private ViewProcessor processor;
		[SerializeField]
		[HideInInspector]
		ResultType activeResultType;

		[SerializeField]
		List<ViewerPointDebug> debugPoints;

		[SerializeField]
		[HideInInspector]
		Vector3[] points;
		[SerializeField]
		[HideInInspector]
		int pointIndex;
		[SerializeField]
		[HideInInspector]
		int repeatedStepIndex;

		int checkIndex;

		bool isTrashed;
		Action OnProcessEvent;

		bool triggerRender;

		public float far
		{
			get => cam.farClipPlane;
			private set => cam.farClipPlane = value;
		}

		public float near
		{
			get => cam.nearClipPlane;
			private set => cam.nearClipPlane = value;
		}

		public float aspect
		{
			get => cam.aspect;
			private set => cam.aspect = value;
		}

		public float fov
		{
			get => cam.fieldOfView;
			private set => cam.fieldOfView = value;
		}

		public Color background
		{
			set
			{
				if (cam != null)
					cam.backgroundColor = value;
			}
		}

		void Update()
		{
			if (triggerRender)
			{
				triggerRender = false;
				Debug.Log("STACK-Update render");
				cam.Render();
			}

			if (start)
			{
				start = false;
				StartSequence();
			}
		}

		void OnEnable()
		{
			Setup();
		}

		void OnDisable() => SafeClean();

		void OnDestroy() => SafeClean();

		void OnPostRender()
		{
			// Debug.Log("STACK-OnPost");

			if (isRunning && !cameraDone && renderText != null)
			{
				Debug.Log("STACK-OnPost Is Running");
				cameraDone = true;
				OnProcessEvent?.Invoke();
			}
		}

		void OnPreRender()
		{
			// Debug.Log("STACK-OnPre");

			if (isRunning && cam != null)
			{
				cam.targetTexture = renderText;
				Debug.Log("STACK-OnPre Is Running");
			}
		}

		void OnValidate()
		{
			Application.targetFrameRate = fps;
		}

		public RenderTexture renderText { get; private set; }

		void Process()
		{
			// if (processor != null)
			//   processor.Process(renderText);
		}

		void StartSequence(ResultType seq = ResultType.Potential)
		{
			if (seq == ResultType.Potential)
				Setup();

			activeResultType = seq;
			cam.cullingMask = activeResultType.GetCullingMask();

			pointIndex = amountToOffset;
			transform.position = points[Math.Max(0, pointIndex)];

			isTrashed = false;
			cameraDone = false;
			IsRunning(true);
		}

		public void IsRunning(bool value)
		{
			Debug.Log("STACK-" + (value ? "Running" : "Paused"));
			isRunning = value;
			// processor.isRunning = value;

			if (value && forceRender)
			{
				Debug.Log("STACK-Trigger marked for render");
				triggerRender = true;
			}
		}

		bool CheckOnThird()
		{
			if (checkIndex >= RerunCount)
			{
				checkIndex = 0;
				return true;
			}

			checkIndex++;
			return false;
		}

		void DataReady(double[] values)
		{
			IsRunning(false);

			if (pointIndex == 0 && !isTrashed && trashFirstPoint)
			{
				// NOTE: Testing if throwing out the first point fixes the issue of the camera not ready
				Debug.Log("STACK-Point Trashed");
				cameraDone = false;
				isTrashed = true;
				IsRunning(true);
				return;
			}

			// If we are not on the third value 
			if (!CheckOnThird())
			{
				cameraDone = false;
				IsRunning(true);
				return;
			}

			if (activeResultType == ResultType.Existing && repeatedStepIndex == 0) Debug.Log(values.FirstOrDefault());

			debugPoints ??= new List<ViewerPointDebug>();
			debugPoints.Add(new ViewerPointDebug
			{
				index = pointIndex,
				value = values.FirstOrDefault(),
				type = activeResultType
			});

			Debug.Log($"STACK-Data Added at {pointIndex}");
			if (rerunOnPoint)
			{
				if (repeatedStepIndex < amountToRerun - 1)
				{
					repeatedStepIndex++;
				}
				else
				{
					repeatedStepIndex = 0;
					pointIndex++;
				}
			}
			else
			{
				pointIndex++;
			}

			// if there are points to check still 
			if (pointIndex < points.Length)
			{
				// start of loop so move to new pos
				if (repeatedStepIndex == 0)
					transform.position = points[Math.Max(0, pointIndex)];

				// NOTE: this should toggle the camera and viewer to process data
				cameraDone = false;
				// NOTE : test when the camera should be valid for running
				IsRunning(true);
			}
			else if (activeResultType != ResultType.Proposed)
			{
				if (activeResultType == ResultType.Potential)
					StartSequence(ResultType.Existing);
				else if (activeResultType == ResultType.Existing)
					StartSequence(ResultType.Proposed);
			}
			else
			{
				IsRunning(false);
				Debug.Log("Checking values now");
				for (var i = 0; i < debugPoints.Count; i += amountToRerun)
				{
					if (!debugPoints.Valid(i))
					{
						Debug.LogWarning($"Skipped to far on first loop {i}");
						continue;
					}

					var basePoint = debugPoints[i];
					repeatedStepIndex = 0;
					for (var j = i + 1; repeatedStepIndex < amountToRerun; j++, repeatedStepIndex++)
					{
						if (!debugPoints.Valid(j))
						{
							Debug.LogWarning($"Skipped on second loop {j}");
							continue;
						}

						var inputPoint = debugPoints[j];
						if (inputPoint.index == basePoint.index && inputPoint.type == basePoint.type)
							// same index, so values should be the same
							if (!basePoint.value.Equals(inputPoint.value))
								Debug.LogWarning($"Value not aligned. index={i} repeat={repeatedStepIndex}\n"
								                 + $"InputA-{basePoint.index} InputB-{inputPoint.index}\n"
								                 + $"InputA-{basePoint.value}\n"
								                 + $"InputB-{inputPoint.value}");
					}
				}
			}
		}

		void SafeClean()
		{
			if (renderText != null) renderText.Release();
		}

		void Setup()
		{
			points = new Vector3[30];
			var startPos = new Vector3(-15f, 0f, 0f);
			points[0] = startPos;

			for (var i = 1; i < points.Length; i++)
				points[i] = new Vector3(startPos.x + i, 0f, 0f);

			pointCount = points.Length;
			// 3 = amount of stages
			Debug.Log($"Check for {pointCount * amountToRerun * 3}");
			//
			// processor = gameObject.GetComponent<ViewProcessor>();
			// if (processor == null)
			//   processor = gameObject.AddComponent<ViewProcessor>();
			//
			// processor.pixelShader = shader;
			// processor.Init(new[] { color }, DataReady);

			IsRunning(isRunning);
			OnProcessEvent = Process;

			cam = gameObject.GetComponent<Camera>();
			if (cam == null)
				cam = gameObject.AddComponent<Camera>();

			// disable for testing force render
			cam.enabled = !forceRender;

			far = 1000;
			aspect = 1;
			fov = 90f;
			cam.clearFlags = CameraClearFlags.Color;

			renderText = RenderTexture.GetTemporary(ViewSize, ViewSize, Depth);
			renderText.name = $"{gameObject.name}-CameraTexture";

			// TODO : Test if using a method vs event changes things
		}
	}

	[Serializable]
	public class ViewerPointDebug
	{
		public int index;
		public double value;
		public ResultType type;
	}
}