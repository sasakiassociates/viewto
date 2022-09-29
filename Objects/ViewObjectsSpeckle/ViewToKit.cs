using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Speckle.Core.Kits;

namespace ViewObjects.Speckle
{

	/// <inheritdoc />
	public class ViewToKit : ISpeckleKit
	{

		/// <summary>
		/// Quick property for getting the full assembly name
		/// </summary>
		public static string AssemblyFullName => AssemblyName.FullName;

		/// <summary>
		/// Gets the assembly name from <see cref="ViewObjectBase_v1"/>
		/// </summary>
		public static AssemblyName AssemblyName => typeof(ViewObjectBase_v1).GetTypeInfo().Assembly.GetName();

		/// <inheritdoc />
		public string Description => "View To kit for converting";

		/// <inheritdoc />
		public string Name => nameof(ViewToKit);

		/// <inheritdoc />
		public string Author => "David Morgan";

		/// <inheritdoc />
		public string WebsiteOrEmail => "https://sasaki.com";

		/// <summary>
		/// Returns a list of supported kit types.
		/// Currently this contains <see cref="ViewObjectBase_v2"/>, <see cref="ViewObjectBase_v1"/>, <see cref="Container"/>
		/// </summary>
		public IEnumerable<Type> Types
		{
			get =>
				Assembly.GetExecutingAssembly().GetTypes().Where
				(t =>
					 t.IsSubclassOf(typeof(ViewObjectBase_v2))
					 || t.IsSubclassOf(typeof(ViewObjectBase_v1))
					 || t.IsSubclassOf(typeof(Container))
					 && !t.IsAbstract
				);
		}

		/// <inheritdoc />
		public ISpeckleConverter LoadConverter(string app)
		{
			if (_loadedConverters.ContainsKey(app) && _loadedConverters[app] != null)
				return Activator.CreateInstance(_loadedConverters[app]) as ISpeckleConverter;

			_converters = GetAvailableConverters();

			try
			{
				var path = Path.Combine(KitLocations.Desktop, CONVERTER_BASE_NAME + "." + $"{app}.dll");
				if (File.Exists(path))
				{
					foreach (var t in Assembly.LoadFrom(path).GetTypes())
					foreach (var i in t.GetInterfaces())
						if (i.Name == nameof(ISpeckleConverter)
						    && Activator.CreateInstance(t) is ISpeckleConverter c
						    && c.GetServicedApplications().Contains(app))
						{
							_loadedConverters[app] = t;
							return c;
						}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return null;
			}

			return null;
		}

		Dictionary<string, Type> _loadedConverters = new Dictionary<string, Type>();

		List<string> _converters;

		const string CONVERTER_BASE_NAME = "ViewObjects.Converter";

		/// <inheritdoc />
		public IEnumerable<string> Converters
		{
			get => _converters ??= GetAvailableConverters();
		}

		/// <summary>
		/// Location for Kits 
		/// </summary>
		public static class KitLocations
		{
			/// <summary>
			/// Located in the typical <see cref="Environment.SpecialFolder.ApplicationData"/>
			/// </summary>
			public static string Desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Speckle\\Kits\\ViewTo");

			/// <summary>
			/// Not yet specified in the kit
			/// </summary>
			[Obsolete] public static string VirtualDesktop = "Not yet established";
		}

		List<string> GetAvailableConverters() => Directory.EnumerateFiles(KitLocations.Desktop, CONVERTER_BASE_NAME + ".*").ToList()
			.Select(dllPath => dllPath.Split('.').Reverse().ToList()[1])
			.ToList();

	}
}