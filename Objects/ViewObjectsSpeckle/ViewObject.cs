using Speckle.Core.Kits;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace ViewObjects.Speckle
{

  /// <inheritdoc />
  public class ViewObject : ISpeckleKit
  {

    private const string CONVERTER_BASE_NAME = "ViewObjects.Converter";

    private List<string> _converters;

    private Dictionary<string, Type> _loadedConverters = new Dictionary<string, Type>();

    /// <summary>
    ///   Quick property for getting the full assembly name
    /// </summary>
    public static string AssemblyFullName => AssemblyName.FullName;

    /// <summary>
    ///   Gets the assembly name from <see cref="ViewObjectBase" />
    /// </summary>
    public static AssemblyName AssemblyName => typeof(ViewObjectBase).GetTypeInfo().Assembly.GetName();

    /// <inheritdoc />
    public string Description => "View To kit for converting";

    /// <inheritdoc />
    public string Name => nameof(ViewObject);

    /// <inheritdoc />
    public string Author => "David Morgan";

    /// <inheritdoc />
    public string WebsiteOrEmail => "https://sasaki.com";

    /// <summary>
    ///   Returns a list of supported kit types.
    ///   Currently this contains <see cref="ViewObjectBase" />, <see cref="Container" />
    /// </summary>
    public IEnumerable<Type> Types
    {
      get
      {
        var types = new List<Type>();
        types.AddRange(Assembly.GetExecutingAssembly().GetTypes().Where
          (t => t.IsSubclassOf(typeof(ViewObjectBase)) || !t.IsAbstract));

        var asm = Assembly.Load(typeof(IViewObject).GetTypeInfo().Assembly.GetName());

        var exported = asm.GetExportedTypes();
        foreach (var t in exported)
        {
          if (t.IsAbstract)
          {
            continue;
          }

          if (t.IsInterface)
          {
            continue;
          }

          if (t.IsSubclassOf(typeof(Container)))
          {
            types.Add(t);
          }
          else if (typeof(IViewObject).IsAssignableFrom(t))
          {
            types.Add(t);
          }
        }

        return types;
      }
    }

    /// <inheritdoc />
    public ISpeckleConverter LoadConverter(string app)
    {
      if (_loadedConverters.ContainsKey(app) && _loadedConverters[app] != null)
      {
        return Activator.CreateInstance(_loadedConverters[app]) as ISpeckleConverter;
      }

      _converters = GetAvailableConverters();

      try
      {
        var path = Path.Combine(KitLocations.Desktop, CONVERTER_BASE_NAME + "." + $"{app}.dll");
        if (File.Exists(path))
        {
          foreach (var t in Assembly.LoadFrom(path).GetTypes())
            foreach (var i in t.GetInterfaces())
            {
              if (i.Name == nameof(ISpeckleConverter)
                  && Activator.CreateInstance(t) is ISpeckleConverter c
                  && c.GetServicedApplications().Contains(app))
              {
                _loadedConverters[app] = t;
                return c;
              }
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

    /// <inheritdoc />
    public IEnumerable<string> Converters => _converters ??= GetAvailableConverters();

    private List<string> GetAvailableConverters()
    {
      return Directory.EnumerateFiles(KitLocations.Desktop, CONVERTER_BASE_NAME + ".*").ToList()
        .Select(dllPath => dllPath.Split('.').Reverse().ToList()[1])
        .ToList();
    }

    /// <summary>
    ///   Location for Kits
    /// </summary>
    public static class KitLocations
    {
      /// <summary>
      ///   Located in the typical <see cref="Environment.SpecialFolder.ApplicationData" />
      /// </summary>
      public static string Desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Speckle\\Kits\\ViewTo");

      /// <summary>
      ///   Not yet specified in the kit
      /// </summary>
      [Obsolete] public static string VirtualDesktop = "Not yet established";
    }

    internal static class Schema
    {
      internal const string Category = "ViewObjects";
    }
  }
}
