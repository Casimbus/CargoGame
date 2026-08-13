using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControl : MonoBehaviour
{
    [SerializeField] RectTransform gameArea;
    [SerializeField] float zoomStep, minSize, maxSize;
    [SerializeField] RectTransform viewport;
    private Vector2 originPosition;
    private bool isPinching;
    private float pinchStartDistance;
    private float pinchStartScale;

    private Camera uiCamera;

    void Start()
    {
        Canvas.ForceUpdateCanvases();
        ScaleGameAreaToViewport();

        Canvas canvas = viewport.GetComponentInParent<Canvas>();
        uiCamera = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? canvas.worldCamera
            : null;
    }
    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 2)
        {
            ZoomMobile();
        }
        else
        {
            isPinching = false;
            if (Input.touchCount == 1)
            {
                PanMobile();
            }
        }

#else
    PanCamera();
    ZoomIn();
    ZoomOut();
#endif
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            originPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 currentPosition = Input.mousePosition;
            Vector2 difference = currentPosition - originPosition;
            Vector2 targetPosition = gameArea.anchoredPosition + difference;
            gameArea.anchoredPosition = ClampCamera(targetPosition);
            originPosition = currentPosition;
        }
    }
    private void ZoomMobile()
    {
        if (Input.touchCount != 2)
        {
            isPinching = false;
            return;
        }
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);
        float currentDistance =
            Vector2.Distance(touch0.position, touch1.position);
        Vector2 pinchCenter = (touch0.position + touch1.position) * 0.5f;

        if (!isPinching)
        {
            isPinching = true;
            pinchStartDistance = currentDistance;
            pinchStartScale = gameArea.localScale.x;

            return;
        }
        if (pinchStartDistance <= 0f)
            return;

        float zoomFactor = currentDistance / pinchStartDistance;
        float newScale = Mathf.Clamp(pinchStartScale * zoomFactor, GetMinimumScale(), maxSize);
        float oldScale = gameArea.localScale.x;

        if (!Mathf.Approximately(newScale, oldScale))
        {
            gameArea.anchoredPosition = GetZoomedPosition(pinchCenter, oldScale, newScale);
            gameArea.localScale = new Vector3(newScale, newScale, 1f);
        }
        gameArea.anchoredPosition = ClampCamera(gameArea.anchoredPosition);
    }

    public void ZoomIn()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            ApplyZoom(zoomStep, Input.mousePosition);
        }
    }
    public void ZoomOut()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            ApplyZoom(-zoomStep, Input.mousePosition);
        }
    }

    private void ApplyZoom(float delta, Vector2 screenPoint)
    {
        float oldScale = gameArea.localScale.x;
        float minScale = Mathf.Max(minSize, GetMinimumScale());
        float newScale = Mathf.Clamp(oldScale + delta, minScale, maxSize);

        if (!Mathf.Approximately(newScale, oldScale))
        {
            gameArea.anchoredPosition = GetZoomedPosition(screenPoint, oldScale, newScale);
            gameArea.localScale = new Vector3(newScale, newScale, 1f);
        }
        gameArea.anchoredPosition = ClampCamera(gameArea.anchoredPosition);
    }

    /// <summary>
    /// Computes the anchoredPosition needed so that the world point currently under
    /// screenPoint stays under screenPoint after scaling from oldScale to newScale.
    /// </summary>
    private Vector2 GetZoomedPosition(Vector2 screenPoint, float oldScale, float newScale)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, screenPoint, uiCamera, out localPoint);

        Vector2 oldAnchoredPosition = gameArea.anchoredPosition;
        float ratio = newScale / oldScale;

        return localPoint - (localPoint - oldAnchoredPosition) * ratio;
    }

    private Vector2 ClampCamera(Vector2 targetPosition)
    {
        Vector2 oldPosition = gameArea.anchoredPosition;
        gameArea.anchoredPosition = targetPosition;
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, gameArea);
        Rect viewRect = viewport.rect;
        Vector2 correction = Vector2.zero;
        if (bounds.size.x > viewRect.width)
        {
            if (bounds.min.x > viewRect.xMin)
            {
                correction.x = viewRect.xMin - bounds.min.x;
            }
            else if (bounds.max.x < viewRect.xMax)
            {
                correction.x = viewRect.xMax - bounds.max.x;
            }
        }
        else
        {
            correction.x = viewRect.center.x - bounds.center.x;
        }
        if (bounds.size.y > viewRect.height)
        {
            if (bounds.min.y > viewRect.yMin)
            {
                correction.y = viewRect.yMin - bounds.min.y;
            }
            else if (bounds.max.y < viewRect.yMax)
            {
                correction.y = viewRect.yMax - bounds.max.y;
            }
        }
        else
        {
            correction.y = viewRect.center.y - bounds.center.y;
        }
        gameArea.anchoredPosition = oldPosition;
        return targetPosition + correction;
    }

    private float GetMinimumScale()
    {
        float scaleX = viewport.rect.width / gameArea.rect.width;
        float scaleY = viewport.rect.height / gameArea.rect.height;
        float requiredScale = Mathf.Max(scaleX, scaleY);
        return Mathf.Max(minSize, requiredScale);
    }

    private void ScaleGameAreaToViewport()
    {
        float startScale = GetMinimumScale();
        gameArea.localScale = new Vector3(startScale, startScale, 1f);
        gameArea.anchoredPosition = Vector2.zero;
    }
    private void PanMobile()
    {
        if (Input.touchCount != 1)
            return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            originPosition = touch.position;
        }
        if (touch.phase == TouchPhase.Moved)
        {
            Vector2 currentPosition = touch.position;
            Vector2 difference = currentPosition - originPosition;
            Vector2 targetPosition = gameArea.anchoredPosition + difference;
            gameArea.anchoredPosition = ClampCamera(targetPosition);
            originPosition = currentPosition;
        }
    }
}