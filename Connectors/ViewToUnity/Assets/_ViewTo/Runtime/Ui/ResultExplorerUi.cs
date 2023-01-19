﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using ViewObjects.Unity;

namespace ViewTo.Connector.Unity
{

  public class ResultExplorerUi : MonoBehaviour
  {

    [SerializeField] ResultExplorer source;
    [SerializeField] UIDocument uiDoc;

    DropdownField _contentOptions;
    SliderInt _pointSlider;


    void Awake()
    {
      if(source == null)
      {
        source = GetComponent<ResultExplorer>();
      }
      if(uiDoc == null)
      {
        Debug.Log($"No UI Doc is found for {name}({nameof(ResultExplorerUi)}.\nSet a UI Doc in order to use the explorer ui");
        return;
      }

      var root = uiDoc.rootVisualElement;

      // hacky way of setting the ui doc shit 
      _contentOptions = root.Q<DropdownField>();
      _pointSlider = root.Q<SliderInt>();
    }

    void Start()
    {
      if(source == null)
      {
        Debug.Log($"No source of {nameof(ResultExplorer)} was found. Please make sure to add this component so the ui can connect with it");
        return;
      }

      source.onStudyLoaded += HandleNewStudy;
      _pointSlider.RegisterValueChangedCallback(e => source.activePoint = e.newValue);

    }

    void HandleNewStudy()
    {
      Debug.Log("New Study Loaded");

      if(_contentOptions != null)
      {
        _contentOptions.choices = source.Options.Select(x => x == null ? "null" : x.Name).ToList();
        _contentOptions.index = 0;
      }

      if(_pointSlider != null)
      {
        _pointSlider.lowValue = 0;
        _pointSlider.highValue = source.Cloud.GetCount();
        _pointSlider.value = 0;
      }

    }


  }

}