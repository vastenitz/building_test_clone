using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera MainCamera;

    private Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

    private float _maxZoomFactor = 30;
    private float _minZoomFactor = 5;

    private float _clampZoomOffset = 2.0f;

    private float _oldZoom = -1;

    private bool _touchCountChanged;

    private int _previousTouchCount = 0;

    private Vector2 _touchPosition;

    private bool _canPan;

    private Vector3 _tapGroundStartPosition;

    private int _layerMaskGroundCollider;

    private bool _isPanningSceneStarted;

    private Vector3 _previousPanPoint;

    private bool _isPanningScene;

    private void Awake()
    {
        this._layerMaskGroundCollider = LayerMask.GetMask("GroundItemCollider");
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateBaseItemTap();
        this.UpdateScenePan();
        this.UpdateSceneZoom();
    }

    private void _RefreshTouchValues()
    {
        this._touchCountChanged = false;
        int touchCount = 0;
        bool isInEditor = false;

        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                touchCount = 1;
                isInEditor = true;
            }
        } else
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                touchCount = 0;
                isInEditor = false;
            } else
            {
                touchCount = Input.touchCount;
            }
        }

        if (touchCount != this._previousTouchCount)
        {
            if (touchCount != 0)
            {
                this._touchCountChanged = true;
            }
        }

        if (isInEditor)
        {
            this._touchPosition = (Vector2)Input.mousePosition;
        } else
        {
            if (touchCount == 1)
            {
                this._touchPosition = Input.GetTouch(0).position;
            }
        }

        this._canPan = (touchCount > 0);
        this._previousTouchCount = touchCount;
    }

    public void UpdateBaseItemTap()
    {
        if (!Input.GetMouseButtonUp(0))
        {
            return;
        }
    }

    public void UpdateScenePan()
    {
        this._RefreshTouchValues();

        if (this._touchCountChanged)
        {
            this._tapGroundStartPosition = this._TryGetRaycastHitBaseGround(this._touchPosition);
        }

        if (this._canPan)
        {
            Vector3 currentTapPosition = this._TryGetRaycastHitBaseGround(this._touchPosition);

            if (!this._isPanningSceneStarted && Vector3.Distance(this._tapGroundStartPosition, currentTapPosition) > 1f)
            {
                this._isPanningSceneStarted = true;
                this._previousPanPoint = currentTapPosition;
            }

            if (this._isPanningSceneStarted)
            {
                this._isPanningScene = true;
                this.OnScenePan(currentTapPosition);
            }
        } else
        {
            this._isPanningScene = false;

            if (this._isPanningSceneStarted)
            {
                this._isPanningSceneStarted = false;
            }
        }
    }

    public void OnScenePan(Vector3 tapPosition)
    {
        Vector3 delta = this._previousPanPoint - tapPosition;
        this.MainCamera.transform.localPosition += delta;
        this.ClampCamera();
    }

    private Vector3 _TryGetRaycastHitBaseGround(Vector2 touch)
    {
        RaycastHit hit;
        if (_TryGetRaycastHit(touch, out hit))
        {
            return hit.point;
        }
        return positiveInfinityVector;
    }

    private bool _TryGetRaycastHit(Vector2 touch, out RaycastHit hit)
    {
        Ray ray = MainCamera.ScreenPointToRay(touch);
        return (Physics.Raycast(ray, out hit, 1000, _layerMaskGroundCollider));
    }

    public void UpdateSceneZoom()
    {
        float newZoom = this.MainCamera.orthographicSize; //20 in default
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");

        if (scrollAmount != 0)
        {
            newZoom = newZoom - scrollAmount;
        }

        // clamp function: given value >= min, <= max => return given value, if > max, return max, < min, return min
        newZoom = Mathf.Clamp(newZoom - scrollAmount, this._minZoomFactor, this._maxZoomFactor);

        if (newZoom < this._minZoomFactor + _clampZoomOffset)
        {
            newZoom = Mathf.Lerp(newZoom, this._minZoomFactor + _clampZoomOffset, Time.deltaTime * 2);

        }
        else if (newZoom > this._maxZoomFactor - _clampZoomOffset)
        {
            newZoom = Mathf.Lerp(newZoom, this._maxZoomFactor - _clampZoomOffset, Time.deltaTime * 2);
        }

        if (this._oldZoom != newZoom)
        {
            this.MainCamera.orthographicSize = newZoom;
            this.ClampCamera();
            this._oldZoom = newZoom;
        }
    }

    public void ClampCamera()
    {
        //		return;
        float worldSizePerPixel = 2 * this.MainCamera.orthographicSize / (float)Screen.height;

        //clamp camera left and top
        Vector3 leftClampScreenPos = this.MainCamera.WorldToScreenPoint(CameraBoundScript.instance.CameraClampTopLeftPosition);
        if (leftClampScreenPos.x > 0)
        {
            float deltaFactor = leftClampScreenPos.x * worldSizePerPixel;
            Vector3 delta = new Vector3(deltaFactor, 0, 0);
            delta = this.MainCamera.transform.TransformVector(delta);
            this.MainCamera.transform.localPosition += delta;
        }

        if (leftClampScreenPos.y < Screen.height)
        {
            float deltaFactor = (Screen.height - leftClampScreenPos.y) * worldSizePerPixel;
            Vector3 delta = new Vector3(-deltaFactor, 0, -deltaFactor);
            this.MainCamera.transform.localPosition += delta;
        }
        //clamp camera right and bottom
        Vector3 rightClampScreenPos = this.MainCamera.WorldToScreenPoint(CameraBoundScript.instance.CameraClampBottomRightPosition);

        if (rightClampScreenPos.x < Screen.width)
        {
            float deltaFactor = (rightClampScreenPos.x - Screen.width) * worldSizePerPixel;
            Vector3 delta = new Vector3(deltaFactor, 0, 0);
            delta = this.MainCamera.transform.TransformVector(delta);
            this.MainCamera.transform.localPosition += delta;
        }

        if (rightClampScreenPos.y > 0)
        {
            float deltaFactor = rightClampScreenPos.y * worldSizePerPixel;
            Vector3 delta = new Vector3(deltaFactor, 0, deltaFactor);
            this.MainCamera.transform.localPosition += delta;
        }
    }
}
