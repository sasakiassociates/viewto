#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ViewObjects;
using ViewObjects.Unity;
using ViewTo.Connector.Unity.Commands;
using ViewTo.Events.Report;
using VU = ViewObjects.Unity;
using VO = ViewObjects;

#endregion

namespace ViewTo.Connector.Unity
{

	public interface IRigSystem : IRig
	{
		public ViewerSystem ActiveViewer { get; }

		public bool IsReady { get; }
	}

	public class Rig : MonoBehaviour, IRigSystem
	{

		#region unity fields

		[SerializeField] VO.ResultStage _stage;

		[SerializeField, HideInInspector] int _activeViewerIndex;

		[SerializeField] List<RigParameters> _parameters;

		Stopwatch _timer;

		#endregion

		public List<RigParameters> RigParams
		{
			get => _parameters;
			protected set => _parameters = value;
		}

		public VO.ResultStage Stage
		{
			get => _stage;
			private set => _stage = value;
		}

		public ViewerSystem ActiveViewer { get; protected set; }

		public bool IsReady
		{
			get => Application.isPlaying && RigParams.Valid() && ActiveViewer && ActiveViewer != null;
		}

		/// <inheritdoc />
		public void Initialize(List<RigParameters> parameters)
		{
			RigParams = parameters;
		}

		/// <inheritdoc />
		public void Build()
		{
			name = "Rig";

			if (TryCreateNewViewer())
			{
				OnReady?.Invoke();
			}
		}

		bool TryCreateNewViewer()
		{
			if (!RigParams.Valid())
			{
				return false;
			}

			if (ActiveViewer == null)
			{
				ActiveViewer = new GameObject().AddComponent<ViewerSystem>();
			}

			ActiveViewer.Init(new ViewerSetupData(RigParams[0]));

			ViewConsole.Log($"Active Viewer: {ActiveViewer.name}");
			OnActiveViewerSystem?.Invoke(ActiveViewer);

			if (RigParams.Count <= 1)
			{
				RigParams.Clear();
			}
			else
			{
				var rigParams = new RigParameters[RigParams.Count - 1];
				RigParams.CopyTo(rigParams, 1);
				RigParams = rigParams.ToList();
			}

			return true;
		}

		public void Run(int startPoint = 0, bool autoRun = true)
		{
			if (!IsReady)
			{
				ViewConsole.Log("Not Ready");
				return;
			}

			ActiveViewer.OnStageChange += SetStageChange;
			ActiveViewer.OnComplete += CompileViewerSystem;
			ActiveViewer.OnDataReadyForCloud += ResultDataCompleted;

			_timer ??= new Stopwatch();
			_timer.Start();

			if (autoRun)
				ActiveViewer.Run();
			else
				ActiveViewer.Capture(startPoint);
		}

		public void TrySetPoint(int index)
		{
			if (ActiveViewer != null)
				ActiveViewer.Capture(index);
		}

		void CompileViewerSystem()
		{
			_timer.Stop();
			ViewConsole.Log($"Total Time for {ActiveViewer.name}-{_timer.Elapsed}");

			ActiveViewer.OnStageChange -= SetStageChange;
			ActiveViewer.OnComplete -= CompileViewerSystem;
			ActiveViewer.OnDataReadyForCloud -= ResultDataCompleted;

			if (TryCreateNewViewer())
			{
				Run();
				return;
			}

			ViewConsole.Log("Rig Complete");
			OnComplete?.Invoke();
		}

		void SetStageChange(VO.ResultStage arg)
		{
			Stage = arg;
			OnStageChange?.Invoke(Stage);
		}

		void ResultDataCompleted(ResultsForCloud data)
		{
			// let any other subscriptions know of the data being completed 
			OnDataReadyForCloud?.Invoke(data);
		}

		#region events

		public event UnityAction OnReady;

		public event UnityAction OnComplete;

		public event UnityAction<ViewContentLoadedArgs> OnContentLoaded;

		public event UnityAction<ResultStage> OnStageChange;

		public event UnityAction<StudyLoadedArgs> OnStudyLoaded;

		public event UnityAction<ResultsForCloud> OnDataReadyForCloud;

		public event UnityAction<ViewerSystem> OnActiveViewerSystem;

		#endregion

	}
}