using UnityEngine;
using ViewTo.Connector.Unity.Commands;

namespace ViewTo.Connector.Unity
{

  [RequireComponent(typeof(Camera))]
  public class FirstPersonViewer : MonoBehaviour
  {
    [SerializeField] Camera viewer;
    [SerializeField] GameObject icon;

    [SerializeField] float lookSpeed = 2.0f;
    [SerializeField] float lookXLimit = 45.0f;

    [SerializeField] ResultExplorer source;

    float _rotationX;
    float _rotationY;
    Vector3 _savedIconRotation;
    float _saveIconEyeLevel;

    /// <summary>
    ///   Toggle for allowing camera to move
    /// </summary>
    public bool Lock { get; set; }

    void Awake()
    {
      viewer = gameObject.GetComponent<Camera>();
      if(icon == null) return;

      _savedIconRotation = icon.transform.localRotation.eulerAngles;
      _saveIconEyeLevel = icon.transform.localPosition.y;
    }

    void Start()
    {
      source.onPointSet += () => SetViewerPos(source.Point);
    }

    void Update()
    {
      if(Input.GetMouseButton(1)) Move();
    }


    public void SetViewerPos(Vector3 position)
    {
      viewer.transform.position = position;
      SetPos();
    }

    public void SetViewerPos(ActivePointArgs args)
    {
      viewer.transform.position = args.position;
      viewer.transform.LookAt(args.center);

      SetPos();
    }

    void SetPos()
    {
      // reset rotation param
      _rotationX = viewer.transform.localRotation.eulerAngles.x;
      _rotationY = viewer.transform.localRotation.eulerAngles.y;

      if(icon == null) return;

      icon.transform.rotation = Quaternion.Euler(
        new Vector3(
          _savedIconRotation.x,
          _savedIconRotation.y + _rotationY,
          _savedIconRotation.z)
      );
    }

    public void Move()
    {
      var yAxis = Input.GetAxis("Mouse Y");
      var xAxis = Input.GetAxis("Mouse X");

      _rotationX += yAxis * lookSpeed;
      _rotationY += xAxis * lookSpeed;

      // Debug.Log($"xAxis={xAxis}  yAxis={yAxis}");
      // Debug.Log($"rotationX={rotationX}  rotationY={rotationY}");

      viewer.transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0);

      if(icon == null) return;

      icon.transform.position = new Vector3(
        viewer.transform.position.x,
        viewer.transform.position.y + _saveIconEyeLevel,
        viewer.transform.position.z
      );

      icon.transform.rotation = Quaternion.Euler(
        new Vector3(
          _savedIconRotation.x,
          _savedIconRotation.y + viewer.transform.localRotation.eulerAngles.y,
          _savedIconRotation.z)
      );
    }
  }

}
