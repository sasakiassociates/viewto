using System;
using System.Collections.Generic;
using UnityEngine;
using ViewObjects.Clouds;
using ViewObjects.Common;

namespace ViewObjects.Unity
{

  public class ViewCloud : ViewObjectMono, IViewCloud, IStreamReference
  {

    [SerializeField] string id;

    [SerializeField] CloudPoint[] cloudPoints;

    [SerializeField, HideInInspector] List<string> references;

    public int count
    {
      get => this.GetCount();
    }

    void Awake()
    {
      id ??= Guid.NewGuid().ToString();
    }

    public List<string> References
    {
      get => references;
      set => references = value;
    }

    public string ViewId
    {
      get => id;
      set => id = value;
    }

    public CloudPoint[] Points
    {
      get => cloudPoints;
      set => cloudPoints = value;
    }
  }

}
