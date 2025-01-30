using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Cam : MonoBehaviour
{
    #region Variables

    [SerializeField] private List<GameObject> targets = new List<GameObject>();
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private float _SmoothTime = .5f;

    [SerializeField] private float _minZoom = 5;
    [SerializeField] private float _maxZoom = 20;
    [SerializeField] private float _zoomLimiter = 50;
    [SerializeField] private float _reduceZoomYLimiter = 20;
    [SerializeField] private AnimationCurve _zoomCurve;

    [SerializeField] private bool _fixeOnZ = true;


    #endregion

    private bool _fixOnRoom = false;
    private Room _room;
    

    private Camera _camera;

    private float _yMinusZoomLimiter;

    #region ACCESSEUR
    public List<GameObject> Targets { get => targets; set => targets = value; }
    public bool FixOnRoom { get => _fixOnRoom; set => _fixOnRoom = value; }

    #endregion

    public void Start()
    {
        _camera = GetComponent<Camera>();
    }
    private void FixedUpdate()
    {
        if(targets.Count == 0) { return; }
        if (!_fixOnRoom)
        {
            Move();
            Zoom();
        }else
        {
            ZoomOnRoom(_room);
            MoveOnRoom(_room);
        }


    }

    public void FixOnRoomVoid(Room r)
    {
        //Debug.Log("LE CACA DE LA CAMERA");  // -_-    CACA
        _fixOnRoom = true;
        _room = r;
    }


    private void ZoomOnRoom(Room room)
    {
        room._boxCollider.enabled = true;
        Bounds bounds = room._boxCollider.bounds;
        room._boxCollider.enabled = false;

        float newZoom = Mathf.Lerp(_minZoom, _maxZoom, _zoomCurve.Evaluate(bounds.extents.x/25 /*/ (_zoomLimiter - _reduceZoomYLimiter)*/));
        //print(MaxDist / _zoomLimiter + " ::::::::: " + _zoomCurve.Evaluate(MaxDist / _zoomLimiter));
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, newZoom, Time.fixedDeltaTime);
    }

    
    private void MoveOnRoom(Room room)
    {
        room._boxCollider.enabled = true;
        Bounds bounds = room._boxCollider.bounds;
        room._boxCollider.enabled = false;
        
        float newZoom = Mathf.Lerp(_minZoom, _maxZoom, _zoomCurve.Evaluate(bounds.extents.x / 30 /*/ (_zoomLimiter - _reduceZoomYLimiter)*/));
        Vector3 centralPoint = bounds.center;
        //print(MaxDist / _zoomLimiter + " ::::::::: " + _zoomCurve.Evaluate(MaxDist / _zoomLimiter));
        transform.position = Vector3.SmoothDamp(transform.position, centralPoint, ref _velocity, _SmoothTime);
        if (_fixeOnZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }


    private void Zoom()
    {
        float MaxDist = GetMaxDist();
        float newZoom = Mathf.Lerp(_minZoom, _maxZoom, _zoomCurve.Evaluate(MaxDist / (_zoomLimiter - _reduceZoomYLimiter)));
        //print(MaxDist / _zoomLimiter + " ::::::::: " + _zoomCurve.Evaluate(MaxDist / _zoomLimiter));
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, newZoom, Time.fixedDeltaTime);
    }
    float GetMaxDist()
    {
        var bounds = new Bounds(targets[0].transform.position, Vector3.zero);
        foreach (GameObject target in targets)
        {
            bounds.Encapsulate(target.transform.position);
        }
        if(bounds.size.x > bounds.size.y)
        {
            _yMinusZoomLimiter = 0;
            return bounds.size.x;

        }
        else
        {
            _yMinusZoomLimiter = _reduceZoomYLimiter;
            return bounds.size.y;
        }
    }
    private void Move()
    {
        Vector3 centralPoint = GetCentralPoint();
        Vector3 NewPos = centralPoint + _offset;

        transform.position = Vector3.SmoothDamp(transform.position, NewPos, ref _velocity, _SmoothTime);
        if (_fixeOnZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }

    Vector3 GetCentralPoint()
    {
        if(targets.Count == 1) { return targets[0].transform.position; }

        var bounds = new Bounds(targets[0].transform.position, Vector3.zero);
        foreach (GameObject target in targets)
        {
            bounds.Encapsulate(target.transform.position);
        }
        return bounds.center;
    }

/*    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawLine(_camera.ScreenToWorldPoint(new Vector2(_camera.scaledPixelWidth / 10,_camera.scaledPixelHeight/10)), _camera.ScreenToWorldPoint(new Vector2(_camera.scaledPixelWidth / 10, _camera.scaledPixelHeight - _camera.scaledPixelHeight / 10)));
        Handles.DrawLine(_camera.ScreenToWorldPoint(new Vector2(_camera.scaledPixelWidth / 10,_camera.scaledPixelHeight/10)), _camera.ScreenToWorldPoint(new Vector2(_camera.scaledPixelWidth - _camera.scaledPixelWidth / 10,_camera.scaledPixelHeight / 10)));

        Handles.DrawLine(_camera.ScreenToWorldPoint(new Vector2(_camera.scaledPixelWidth - _camera.scaledPixelWidth / 10, _camera.scaledPixelHeight / 10)), _camera.ScreenToWorldPoint(new Vector2(_camera.scaledPixelWidth - _camera.scaledPixelWidth / 10, _camera.scaledPixelHeight - _camera.scaledPixelHeight / 10)));
        Handles.DrawLine(_camera.ScreenToWorldPoint(new Vector2(_camera.scaledPixelWidth / 10, _camera.scaledPixelHeight - _camera.scaledPixelHeight / 10)), _camera.ScreenToWorldPoint(new Vector2(_camera.scaledPixelWidth - _camera.scaledPixelWidth / 10, _camera.scaledPixelHeight - _camera.scaledPixelHeight / 10)));
    }*/

    void gameOverCamAnimation(GameObject targetPlayer)
    {
        /* identifi coordinates of target plager
         * transform camera to match coordinates
         * zoom in camera
         */
    }

}
