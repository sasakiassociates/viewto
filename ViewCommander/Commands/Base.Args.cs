using System;
using System.Collections.Generic;
using System.Linq;

namespace ViewTo.Cmd
{

  public abstract class CommandArgs : ICommandArgs
  {
    public CommandArgs(string message)
    {
      Message = message;
    }

    public string Message {get;protected set;}

    public abstract bool IsValid();
  }

  internal class SimpleResultArgs : CommandArgs
  {
    private bool _result;

    public SimpleResultArgs(bool result, string message) : base(message)
    {
      _result = result;
    }

    public override bool IsValid()
    {
      return _result;
    }
  }

  internal class ValuesForExplorerArgs : CommandArgs
  {

    public readonly IEnumerable<double> values = Array.Empty<double>();

    public ValuesForExplorerArgs(string message) : base(message)
    { }

    public ValuesForExplorerArgs(IEnumerable<double> values, string message) : base(message)
    {
      this.values = values;
    }

    public override bool IsValid()
    {
      return values != null && values.Any();
    }
  }

  internal class ValuesRawForExplorerArgs : CommandArgs
  {

    public readonly IEnumerable<int> values = Array.Empty<int>();

    public ValuesRawForExplorerArgs(string message) : base(message)
    { }

    public ValuesRawForExplorerArgs(IEnumerable<int> values, string message) : base(message)
    {
      this.values = values;
    }

    public override bool IsValid()
    {
      return values != null && values.Any();
    }
  }


  public class ValuesRawExplorerArgs<T> : CommandArgs where T : IComparable<T>
  {

    public readonly IEnumerable<T> values = Array.Empty<T>();

    public ValuesRawExplorerArgs(string message) : base(message)
    { }

    public ValuesRawExplorerArgs(IEnumerable<T> values, string message) : base(message)
    {
      this.values = values;
    }

    public override bool IsValid()
    {
      return values != null && values.Any();
    }
  }


  internal class PointWithValueArgs : CommandArgs
  {
    public readonly int index = -1;

    public PointWithValueArgs(string message) : base(message)
    { }

    public PointWithValueArgs(int index, string message) : base(message)
    {
      this.index = index;
    }

    public override bool IsValid()
    {
      return index>=0;
    }
  }

}
