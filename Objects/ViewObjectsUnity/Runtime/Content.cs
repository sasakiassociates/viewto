using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViewObjects.Common;
using ViewObjects.Contents;

namespace ViewObjects.Unity
{


  public class Content : ViewObjectMono,
    IContent,
    IContentObjects<GameObject>,
    IStreamReference
  {


    [HideInInspector][SerializeField] int layerMask;

    [SerializeField] List<GameObject> objects;

    [SerializeField] ContentType contentType;

    [SerializeField] Color32 color;

    [SerializeField] string viewId;

    [SerializeField] string viewName;

    [SerializeField, HideInInspector] List<string> references;

    public int ContentLayerMask
    {
      get => layerMask;
      set => layerMask = value;
    }

    public string FullName => $"Content {ContentType.ToString().Split('.').LastOrDefault()} - {ViewName}";

    static int DiffuseColor => Shader.PropertyToID("_diffuseColor");

    public bool Show
    {
      set
      {
        if(!objects.Valid())
          return;

        foreach(var obj in objects)
          obj.gameObject.SetActive(value);
      }
    }

    public ViewColor Color
    {
      get => new(color.r, color.g, color.b, color.a);
      set
      {
        if(value == null)
          return;

        Debug.Log($"new assigned to {viewName}:" + value.ToUnity());

        color = value.ToUnity();

        ApplyColor();
      }
    }

    public string ViewId
    {
      get => viewId;
      set => viewId = value;
    }

    public ContentType ContentType
    {
      get => contentType;
      set
      {
        contentType = value;
        ContentLayerMask = value.GetLayerMask();
      }
    }

    public string ViewName
    {
      get => viewName;
      set
      {
        viewName = value;
        gameObject.name = FullName;
      }
    }

    public List<GameObject> Objects
    {
      get => objects;
      set { objects = value; }
    }

    public List<string> References
    {
      get => references;
      set => references = value;
    }

    void ApplyColor()
    {
      if(!objects.Valid())
      {
        Debug.Log("No Objects to apply");
        return;
      }

      foreach(var contentObj in objects)
      {
        var meshRend = contentObj.GetComponent<MeshRenderer>();
        if(meshRend != null)
        {
          if(Application.isPlaying)
          {
            meshRend.material.SetColor(DiffuseColor, color);
          }
          else
          {
            meshRend.sharedMaterial.SetColor(DiffuseColor, color);
          }
        }
      }
    }

    /// <summary>
    ///   references the objects converted to the view content list and imports them
    /// </summary>
    public void PrimeMeshData(Material material, Action<GameObject> onAfterPrime = null)
    {
      if(!Objects.Valid())
      {
        Debug.Log($"No objects for {name} are ready to be primed ");
        return;
      }

      if(material == null)
      {
        Debug.LogError($"Material is needed to prime mesh data on {name}");
        return;
      }

      var c = color;

      if(material.HasProperty(DiffuseColor))
      {
        material.SetColor(DiffuseColor, c);
      }
      else
      {
        Debug.Log($"No property {DiffuseColor} on shader");
      }

      Debug.Log($"Combinding {objects.Count} Mesh(es)");
      objects = new List<GameObject>() {gameObject.CombineMeshes(material)};

      gameObject.ApplyAll(material);
      gameObject.SetLayerRecursively(ContentLayerMask);

      Debug.Log($"{ViewName} is primed!\nview color {Color.ToUnity()}");
    }
  }

}
