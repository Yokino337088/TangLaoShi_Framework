using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool dragEnabled = true;
    public bool restrictToCanvas = true;
    public RectTransform targetRectTransform;
    public Vector2 dragOffset = Vector2.zero;

    private RectTransform canvasRectTransform;
    private Vector2 originalPosition;
    private bool isDragging = false;

    public delegate void DragEventHandler(GameObject draggedObject, Vector2 position);
    public event DragEventHandler OnDragStart;
    public event DragEventHandler OnDragging;
    public event DragEventHandler OnDragEnd;

    private void Awake()
    {
        if (targetRectTransform == null)
            targetRectTransform = GetComponent<RectTransform>();
        
        // 获取Canvas的RectTransform
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!dragEnabled) return;
        
        isDragging = true;
        originalPosition = targetRectTransform.anchoredPosition;
        
        // 隐藏拖拽对象在Raycast中的检测
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;
        
        OnDragStart?.Invoke(gameObject, targetRectTransform.anchoredPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            targetRectTransform.localPosition = localPointerPosition - dragOffset;
            
            // 如果限制在Canvas内
            if (restrictToCanvas)
                ConstrainToCanvas();
        }
        
        OnDragging?.Invoke(gameObject, targetRectTransform.anchoredPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        
        // 恢复拖拽对象在Raycast中的检测
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
        
        OnDragEnd?.Invoke(gameObject, targetRectTransform.anchoredPosition);
    }

    /// <summary>
    /// 限制对象在Canvas内
    /// </summary>
    private void ConstrainToCanvas()
    {
        Vector3[] canvasCorners = new Vector3[4];
        Vector3[] rectCorners = new Vector3[4];
        
        canvasRectTransform.GetWorldCorners(canvasCorners);
        targetRectTransform.GetWorldCorners(rectCorners);
        
        float canvasMinX = canvasCorners[0].x;
        float canvasMaxX = canvasCorners[2].x;
        float canvasMinY = canvasCorners[0].y;
        float canvasMaxY = canvasCorners[2].y;
        
        float rectWidth = targetRectTransform.rect.width * targetRectTransform.localScale.x;
        float rectHeight = targetRectTransform.rect.height * targetRectTransform.localScale.y;
        
        Vector3 pos = targetRectTransform.position;
        pos.x = Mathf.Clamp(pos.x, canvasMinX + rectWidth / 2, canvasMaxX - rectWidth / 2);
        pos.y = Mathf.Clamp(pos.y, canvasMinY + rectHeight / 2, canvasMaxY - rectHeight / 2);
        
        targetRectTransform.position = pos;
    }
}