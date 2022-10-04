﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewObjects.Explorer
{
	public class Explorer : IExplorer
	{

		/// <summary>
		/// </summary>
		public Explorer()
		{ }

		/// <inheritdoc />
		public IViewStudy Study { get; internal set; }

		/// <inheritdoc />
		public IResultCloud Source { get; internal set; }

		/// <inheritdoc />
		public ExplorerSettings Settings { get; set; } = new();

		/// <inheritdoc />
		public ContentOption ActiveOption { get; internal set; }

		/// <inheritdoc />
		public List<ContentOption> Options { get; internal set; }

		/// <inheritdoc />
		public double[] ActiveValues { get; internal set; }

		/// <inheritdoc />
		public List<IResultCloudData> Data
		{
			get => Source?.Data ?? new List<IResultCloudData>();
		}

		/// <inheritdoc />
		public void Load(IViewStudy viewObj)
		{
			if (viewObj == default)
				return;

			Source = viewObj.FindObject<ResultCloud>();

			if (Source == null)
				return;

			this.Options = Source.Data.Where(x => x != null).Select(x => x.Option).Cast<ContentOption>().ToList();
			this.ActiveOption = Options.FirstOrDefault();

			Settings ??= new ExplorerSettings();
			// Data.ActiveValues = this.Fetch();
		}

		/// <inheritdoc />
		public ResultPoint GetResultPoint() => throw new NotImplementedException();
	}

	public interface IExplorer
	{
		/// <summary>
		///   The active view study being used with the source cloud.
		/// </summary>
		public IViewStudy Study { get; }

		/// <summary>
		///   The heart and soul of the data being explored
		/// </summary>
		public IResultCloud Source { get; }

		/// <summary>
		///   Set of data settings for the explorer to use
		/// </summary>
		public ExplorerSettings Settings { get; set; }

		/// <summary>
		///   Container for result values being explored
		/// </summary>
		public List<IResultCloudData> Data { get; }

		/// <summary>
		///   List of options to use for fetching values from <see cref="IExplorer" />. Multiple options will combine the values
		/// </summary>
		public List<ContentOption> Options { get; }

		/// <summary>
		/// Normalized values 
		/// </summary>
		public double[] ActiveValues { get; }

		/// <summary>
		/// 
		/// </summary>
		public ContentOption ActiveOption { get; }

		/// <summary>
		///   Load in a new view study for the explorer to explore!
		/// </summary>
		/// <param name="viewObj">The view study to load in</param>
		public void Load(IViewStudy viewObj);

		/// <summary>
		///   Retrieves the active point result data
		/// </summary>
		/// <returns></returns>
		public ResultPoint GetResultPoint();

	}

	public struct ExplorerData
	{
		public double[] ActiveValues { get; set; }

	}

}