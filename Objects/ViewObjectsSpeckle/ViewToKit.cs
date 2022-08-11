using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Speckle.Core.Kits;

namespace ViewObjects.Speckle
{
	public class ViewToKit : ISpeckleKit
	{
		private Dictionary<string, Type> _LoadedConverters = new Dictionary<string, Type>();

		public static ViewToKit LoadKit => KitManager.GetKit(AssemblyFullName) as ViewToKit;

		public static string AssemblyFullName => AssemblyName.FullName;

		public string ConverterBaseName => "ViewObjects.Converter";

		public static AssemblyName AssemblyName => typeof(ViewObjBase).GetTypeInfo().Assembly.GetName();

		public string Description => "View To kit for converting";

		public string Name => nameof(ViewToKit);

		public string Author => "David Morgan";

		public string WebsiteOrEmail => "https://sasaki.com";

		public IEnumerable<Type> Types => Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ViewObjBase)) && !t.IsAbstract);

		public ISpeckleConverter LoadConverter(string app)
		{
			if (_LoadedConverters.ContainsKey(app) && _LoadedConverters[app] != null)
				return Activator.CreateInstance(_LoadedConverters[app]) as ISpeckleConverter;

			converters = GetAvailableConverters();

			try
			{
				var path = Path.Combine(KitLocation, ConverterBaseName + "." + $"{app}.dll");
				if (File.Exists(path))
				{
					foreach (var t in Assembly.LoadFrom(path).GetTypes())
					foreach (var i in t.GetInterfaces())
						if (i.Name == nameof(ISpeckleConverter)
						    && Activator.CreateInstance(t) is ISpeckleConverter c
						    && c.GetServicedApplications().Contains(app))
						{
							_LoadedConverters[app] = t;
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

		private List<string> converters;

		public IEnumerable<string> Converters
		{
			get => converters ?? (converters = GetAvailableConverters());
		}

		public string KitLocation => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Speckle\\Kits\\ViewTo");

		public List<string> GetAvailableConverters()
		{
			var list = Directory.EnumerateFiles(KitLocation, ConverterBaseName + ".*");
			return list.ToList().Select(dllPath => dllPath.Split('.').Reverse().ToList()[1]).ToList();
		}
	}
}