using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMousePan : MonoBehaviour
{
    private Vector3 Origin;
    private Vector3 Difference;
    private Vector3 ResetCamera;

    private bool drag = false;


    [SerializeField] private float _zoomChange;
    [SerializeField] private float _smoothChange;
    [SerializeField] private float _minSize;
    [SerializeField] private float _maxSize;


    private void Start()
    {
        ResetCamera = Camera.main.transform.position;
    }


    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (drag == false)
            {
                drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            Camera.main.transform.position = Origin - Difference * 0.5f;
        }

        if (Input.GetMouseButton(1))
            Camera.main.transform.position = ResetCamera;

        //Zoom out
        if (Input.mouseScrollDelta.y > 0)
        {
            Camera.main.orthographicSize -= _zoomChange * Time.deltaTime * _smoothChange;
        }

        //zoom in
        if (Input.mouseScrollDelta.y < 0)
        {
            Camera.main.orthographicSize += _zoomChange * Time.deltaTime * _smoothChange;
        }

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, _minSize, _maxSize);
    }
}
