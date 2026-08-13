using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControl : MonoBehaviour
{
    [SerializeField] RectTransform gameArea;
    [SerializeField] float zoomStep, minSize, maxSize;
    [SerializeField] RectTransform viewport;
    private Vector2 originPosition;
    
    private float previousDistance;
    private float minX, maxX, minY, maxY;

    void Start()
    { 
        Canvas.ForceUpdateCanvases();
      ScaleGameAreaToViewport();
    }
    void Update()
    {
        PanCamera();
        ZoomIn();
        ZoomOut();
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
    
    public void ZoomMobile()
    {
        if (Input.touchCount >= 2)
        {
            Vector2 touch0 = Input.GetTouch(0).position;
            Vector2 touch1 = Input.GetTouch(1).position;
            float currentDistance = Vector2.Distance(touch0, touch1);
            if (previousDistance != 0f)
            {
                float difference = currentDistance - previousDistance;
                float newScale = gameArea.localScale.x + zoomStep * difference;
                float minScale = Mathf.Max(minSize, GetMinimumScale());
                newScale = Mathf.Clamp(newScale, minScale, maxSize);
                gameArea.localScale = new Vector3(newScale, newScale, 1f);
                gameArea.anchoredPosition = ClampCamera(gameArea.anchoredPosition);
            }
            previousDistance = currentDistance;
        }
        else
        {
            previousDistance = 0f;
        }
    }
    
    public void ZoomIn()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            float newScale = gameArea.localScale.x + zoomStep;
            float minScale = Mathf.Max(minSize, GetMinimumScale());
            newScale = Mathf.Clamp(newScale, minScale, maxSize);
            gameArea.localScale = new Vector3(newScale, newScale, 1f);
            gameArea.anchoredPosition = ClampCamera(gameArea.anchoredPosition);
        }
    }
    public void ZoomOut()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            float newScale = gameArea.localScale.x - zoomStep;
            float minScale = Mathf.Max(minSize, GetMinimumScale());
            newScale = Mathf.Clamp(newScale, minScale, maxSize);
            gameArea.localScale = new Vector3(newScale, newScale, 1f);
            gameArea.anchoredPosition = ClampCamera(gameArea.anchoredPosition);
        }
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
}
