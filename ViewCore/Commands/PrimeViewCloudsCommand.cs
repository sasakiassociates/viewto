using System.Collections.Generic;
using System.Linq;
using ViewObjects;
using ViewTo.Events.Prime;

namespace ViewTo.Commands
{

  /// <summary>
  ///   Grabs the data from a cloud object to be sent for setup
  /// </summary>
  internal class PrimeViewCloudsCommand : PrimeObjectCommand<List<IViewCloud>>
  {

    public PrimeViewCloudsCommand(List<IViewCloud> obj) : base(obj)
    { }

    public Dictionary<string, CloudPoint[]> Points { get; private set; }

    public override void Run()
    {
      Points = new Dictionary<string, CloudPoint[]>();
      foreach (var cloud in Obj) args.Add(new PrimeCloudArgs(cloud.viewID, cloud.points));

      greatSuccess = args.Any();
    }
  }

}
