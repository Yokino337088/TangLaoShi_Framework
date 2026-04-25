// 导入Unity引擎核心命名空间
using UnityEngine;
using UnityEngine.UI;
// 导入DOTween命名空间
using DG.Tweening;
// 导入泛型集合命名空间
using System.Collections.Generic;
// 导入系统命名空间
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 使用DOTween插件实现的UI面板动画静态扩展类
/// 提供各种面板和控件动画效果的扩展方法
/// </summary>
public static class DOTweenPanelAnimationExtension
{
    // 存储动画ID和Tweener的映射，用于管理和停止动画
    private static Dictionary<int, Tweener> activeTweeners = new Dictionary<int, Tweener>();
    // 动画ID计数器
    private static int tweenerIdCounter = 0;

    // 静态构造函数，初始化DOTween设置
    static DOTweenPanelAnimationExtension()
    {
        // 初始化DOTween全局设置
        DOTween.defaultEaseType = Ease.OutQuad;
        DOTween.SetTweensCapacity(1000, 200); // 设置初始容量，提高性能
    }

    #region 核心动画方法


    #endregion

    #region 面板动画API
    /// <summary>
    /// 面板淡入动画
    /// </summary>
    public static void DoPanelFadeInAnimation(this BasePanel basePanel, float fadeInTime = 0.3f, Action callBack = null)
    {
        // 尝试获取CanvasGroup组件，如果没有则添加
        CanvasGroup canvasGroup = basePanel.gameObject.GetComponent<CanvasGroup>();
        bool addedCanvasGroup = false;
        
        if (canvasGroup == null)
        {
            canvasGroup = basePanel.gameObject.AddComponent<CanvasGroup>();
            addedCanvasGroup = true;
        }

        // 保存原始alpha值，以便动画完成后恢复
        float originalAlpha = canvasGroup.alpha;
        canvasGroup.alpha = 0f;

        // 创建淡入动画
        canvasGroup.DOFade(1f, fadeInTime).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // 如果alpha原本就是1，则可以考虑移除CanvasGroup组件
                if (originalAlpha == 1f && addedCanvasGroup)
                {
                    UnityEngine.Object.Destroy(canvasGroup);
                }
                else
                {
                    canvasGroup.alpha = originalAlpha;
                }
                callBack?.Invoke();
            });
    }

    /// <summary>
    /// 面板淡出动画
    /// </summary>
    public static void DoPanelFadeOutAnimation(this BasePanel basePanel, float fadeOutTime = 0.3f, Action callBack = null)
    {
        // 尝试获取CanvasGroup组件，如果没有则添加
        CanvasGroup canvasGroup = basePanel.GetComponent<CanvasGroup>();
        bool addedCanvasGroup = false;

        if (canvasGroup == null)
        {
            canvasGroup = basePanel.gameObject.AddComponent<CanvasGroup>();
            addedCanvasGroup = true;
        }

        // 保存原始alpha值，以便动画完成后恢复
        float originalAlpha = canvasGroup.alpha;

        // 创建淡出动画
        canvasGroup.DOFade(0f, fadeOutTime).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                
                // 如果是临时添加的CanvasGroup，则移除
                if (addedCanvasGroup)
                {
                    UnityEngine.Object.Destroy(canvasGroup);
                }
                else
                {
                    canvasGroup.alpha = originalAlpha;
                }
                callBack?.Invoke();
                //隐藏面板
                basePanel.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// 面板缩放进入动画
    /// </summary>
    public static void DoPanelScaleInAnimation(this BasePanel basePanel, float scaleInTime = 0.3f, Action callBack = null)
    {
        // 保存原始缩放值
        Vector3 originalScale = basePanel.transform.localScale;
        basePanel.transform.localScale = Vector3.one * 0.2f;

        // 创建缩放动画
        basePanel.transform.DOScale(Vector3.one, scaleInTime).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // 恢复原始缩放值
                basePanel.transform.localScale = originalScale;
                callBack?.Invoke();
            });
    }

    /// <summary>
    /// 面板缩放退出动画
    /// </summary>
    public static void DoPanelScaleOutAnimation(this BasePanel basePanel, float fadeOutTime = 0.3f, Action callBack = null)
    {
        // 保存原始缩放值
        Vector3 originalScale = basePanel.transform.localScale;

        // 创建缩放动画
        basePanel.transform.DOScale(Vector3.one * 0.1f, fadeOutTime).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                // 恢复原始缩放值
                basePanel.transform.localScale = originalScale;
                callBack?.Invoke();
                basePanel.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// 面板顶部滑入动画
    /// </summary>
    public static void DoPanelSlideInFromTop(this BasePanel panel, float duration = 0.5f, Action onComplete = null)
    {
        if (panel == null)
            return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;
        
        // 设置起始位置（屏幕顶部外）
        rectTransform.anchoredPosition = new Vector2(originalPos.x, originalPos.y + 1000f);

        // 创建并播放动画
        rectTransform.DOAnchorPos(originalPos, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 面板底部滑入动画
    /// </summary>
    public static void DoPanelSlideInFromBottom(this BasePanel panel, float duration = 0.5f, Action onComplete = null)
    {
        if (panel == null)
            return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;
        
        // 设置起始位置（屏幕底部外）
        rectTransform.anchoredPosition = new Vector2(originalPos.x, originalPos.y - 1000f);

        // 创建并播放动画
        rectTransform.DOAnchorPos(originalPos, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 面板左侧滑入动画
    /// </summary>
    public static void DoPanelSlideInFromLeft(this BasePanel panel, float duration = 0.5f, Action onComplete = null)
    {
        if (panel == null)
            return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;
        
        // 设置起始位置（屏幕左侧外）
        rectTransform.anchoredPosition = new Vector2(originalPos.x - 1000f, originalPos.y);

        // 创建并播放动画
        rectTransform.DOAnchorPos(originalPos, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 面板右侧滑入动画
    /// </summary>
    public static void DoPanelSlideInFromRight(this BasePanel panel, float duration = 0.5f, Action onComplete = null)
    {
        if (panel == null)
            return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;
        
        // 设置起始位置（屏幕右侧外）
        rectTransform.anchoredPosition = new Vector2(originalPos.x + 1000f, originalPos.y);

        // 创建并播放动画
        rectTransform.DOAnchorPos(originalPos, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => onComplete?.Invoke());
    }
    #endregion

    #region 面板控件动画扩展方法

    /// <summary>
    /// 从不同方向滑入的枚举
    /// </summary>
    public enum SlideDirection
    {
        Top,    // 从上方向滑入
        Bottom, // 从下方向滑入
        Left,   // 从左方向滑入
        Right   // 从右方向滑入
    }

    /// <summary>
    /// 为面板中的所有控件应用滑入动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="baseDuration">基础动画持续时间</param>
    /// <param name="direction">滑入方向</param>
    /// <param name="spreadFactor">动画扩散因子（值越大，子控件动画延迟差异越大）</param>
    /// <param name="onComplete">所有动画完成后的回调</param>
    public static void PlayControlsSlideInAnimation(this BasePanel panel, float baseDuration = 0.5f,
        SlideDirection direction = SlideDirection.Top, float spreadFactor = 0.1f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 获取面板中所有可交互和可视的UI控件
        List<UIBehaviour> controls = new List<UIBehaviour>();
        panel.GetComponentsInChildren<UIBehaviour>(true, controls);

        // 过滤掉一些不需要动画的默认控件
        List<UIBehaviour> filteredControls = new List<UIBehaviour>();
        foreach (var control in controls)
        {
            if (control.gameObject.activeSelf && ShouldAnimateControl(control))
            {
                filteredControls.Add(control);
            }
        }

        // 如果没有需要动画的控件，直接调用回调
        if (filteredControls.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 动画完成计数器
        int animationCompleteCount = 0;
        int totalAnimations = filteredControls.Count;

        // 为每个控件应用动画
        for (int i = 0; i < filteredControls.Count; i++)
        {
            UIBehaviour control = filteredControls[i];
            float delay = i * spreadFactor; // 计算延迟时间

            // 使用DOTween的延迟方法
            DOVirtual.DelayedCall(delay, () =>
            {
                PlayControlSlideInAnimation(control.gameObject, direction, baseDuration, () =>
                {
                    animationCompleteCount++;
                    if (animationCompleteCount >= totalAnimations)
                    {
                        onComplete?.Invoke();
                    }
                });
            });
        }
    }

    /// <summary>
    /// 为面板中的所有控件应用随机方向滑入动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="baseDuration">基础动画持续时间</param>
    /// <param name="spreadFactor">动画扩散因子</param>
    /// <param name="onComplete">所有动画完成后的回调</param>
    public static void PlayControlsRandomSlideInAnimation(this BasePanel panel, float baseDuration = 0.5f,
        float spreadFactor = 0.1f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 获取面板中所有可交互和可视的UI控件
        List<UIBehaviour> controls = new List<UIBehaviour>();
        panel.GetComponentsInChildren<UIBehaviour>(true, controls);

        // 过滤掉一些不需要动画的默认控件
        List<UIBehaviour> filteredControls = new List<UIBehaviour>();
        foreach (var control in controls)
        {
            if (control.gameObject.activeSelf && ShouldAnimateControl(control))
            {
                filteredControls.Add(control);
            }
        }

        // 如果没有需要动画的控件，直接调用回调
        if (filteredControls.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 动画完成计数器
        int animationCompleteCount = 0;
        int totalAnimations = filteredControls.Count;

        // 随机方向数组
        SlideDirection[] directions = { SlideDirection.Top, SlideDirection.Bottom, SlideDirection.Left, SlideDirection.Right };

        // 为每个控件应用随机方向的动画
        for (int i = 0; i < filteredControls.Count; i++)
        {
            UIBehaviour control = filteredControls[i];
            float delay = i * spreadFactor; // 计算延迟时间
            SlideDirection randomDirection = directions[UnityEngine.Random.Range(0, directions.Length)]; // 随机选择方向

            // 使用DOTween的延迟方法
            DOVirtual.DelayedCall(delay, () =>
            {
                PlayControlSlideInAnimation(control.gameObject, randomDirection, baseDuration, () =>
                {
                    animationCompleteCount++;
                    if (animationCompleteCount >= totalAnimations)
                    {
                        onComplete?.Invoke();
                    }
                });
            });
        }
    }

    /// <summary>
    /// 增强版：带距离加权的基于中心点的控件滑入动画
    /// 离中心点越远的控件，动画效果越明显
    /// </summary>
    public static void PlayControlsEnhancedRadialSlideInAnimation(this BasePanel panel, float baseDuration = 0.5f,
        float spreadFactor = 0.1f, float distanceMultiplier = 1.0f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 获取面板的RectTransform组件以计算中心点
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            LogSystem.Error("面板没有RectTransform组件，无法计算中心点");
            return;
        }

        // 计算面板的中心点（使用世界坐标）
        Vector3 panelCenter = panelRect.position;

        // 计算面板的对角线长度，用于标准化距离
        float panelDiagonal = panelRect.rect.size.magnitude;

        // 获取面板中所有可交互和可视的UI控件
        List<UIBehaviour> controls = new List<UIBehaviour>();
        panel.GetComponentsInChildren<UIBehaviour>(true, controls);

        // 过滤掉一些不需要动画的默认控件
        List<UIBehaviour> filteredControls = new List<UIBehaviour>();
        foreach (var control in controls)
        {
            if (control.gameObject.activeSelf && ShouldAnimateControl(control))
            {
                filteredControls.Add(control);
            }
        }

        // 如果没有需要动画的控件，直接调用回调
        if (filteredControls.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 动画完成计数器
        int animationCompleteCount = 0;
        int totalAnimations = filteredControls.Count;

        // 为每个控件应用基于位置和距离的动画
        for (int i = 0; i < filteredControls.Count; i++)
        {
            UIBehaviour control = filteredControls[i];
            RectTransform controlRect = control.GetComponent<RectTransform>();
            if (controlRect == null)
                continue;

            float delay = i * spreadFactor; // 计算延迟时间

            // 计算控件相对于面板中心点的位置，确定滑入方向
            SlideDirection direction = DetermineSlideDirection(panelCenter, controlRect.position);

            // 计算控件与中心点的距离，并将其标准化
            float distance = Vector3.Distance(panelCenter, controlRect.position);
            float normalizedDistance = Mathf.Clamp01(distance / panelDiagonal);

            // 根据距离调整动画参数
            float adjustedDuration = baseDuration * (1 + normalizedDistance * 0.5f); // 距离越远，动画越长
            float adjustedOffset = 1000f * (1 + normalizedDistance * distanceMultiplier); // 距离越远，起始位置越远

            // 使用DOTween的延迟方法
            DOVirtual.DelayedCall(delay, () =>
            {
                PlayControlEnhancedSlideInAnimation(control.gameObject, direction, adjustedDuration, adjustedOffset, () =>
                {
                    animationCompleteCount++;
                    if (animationCompleteCount >= totalAnimations)
                    {
                        onComplete?.Invoke();
                    }
                });
            });
        }
    }

    /// <summary>
    /// 为面板中的所有控件应用基于中心点的方向滑入动画
    /// 上方控件从上往下滑入，下方控件从下往上滑入
    /// 左方控件从左往右滑入，右方控件从右往左滑入
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="baseDuration">基础动画持续时间</param>
    /// <param name="spreadFactor">动画扩散因子（值越大，子控件动画延迟差异越大）</param>
    /// <param name="onComplete">所有动画完成后的回调</param>
    public static void PlayControlsRadialSlideInAnimation(this BasePanel panel, float baseDuration = 0.5f,
        float spreadFactor = 0.1f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 获取面板的RectTransform组件以计算中心点
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            LogSystem.Error("面板没有RectTransform组件，无法计算中心点");
            return;
        }

        // 计算面板的中心点（使用世界坐标）
        Vector3 panelCenter = panelRect.position;

        // 获取面板中所有可交互和可视的UI控件
        List<UIBehaviour> controls = new List<UIBehaviour>();
        panel.GetComponentsInChildren<UIBehaviour>(true, controls);

        // 过滤掉一些不需要动画的默认控件
        List<UIBehaviour> filteredControls = new List<UIBehaviour>();
        foreach (var control in controls)
        {
            if (control.gameObject.activeSelf && ShouldAnimateControl(control))
            {
                filteredControls.Add(control);
            }
        }

        // 如果没有需要动画的控件，直接调用回调
        if (filteredControls.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 动画完成计数器
        int animationCompleteCount = 0;
        int totalAnimations = filteredControls.Count;

        // 为每个控件应用基于位置的动画
        for (int i = 0; i < filteredControls.Count; i++)
        {
            UIBehaviour control = filteredControls[i];
            RectTransform controlRect = control.GetComponent<RectTransform>();
            if (controlRect == null)
                continue;

            float delay = i * spreadFactor; // 计算延迟时间

            // 计算控件相对于面板中心点的位置，确定滑入方向
            SlideDirection direction = DetermineSlideDirection(panelCenter, controlRect.position);

            // 使用DOTween的延迟方法
            DOVirtual.DelayedCall(delay, () =>
            {
                PlayControlSlideInAnimation(control.gameObject, direction, baseDuration, () =>
                {
                    animationCompleteCount++;
                    if (animationCompleteCount >= totalAnimations)
                    {
                        onComplete?.Invoke();
                    }
                });
            });
        }
    }

    #endregion

    #region 私有辅助方法

    /// <summary>
    /// 判断控件是否应该应用动画
    /// </summary>
    /// <param name="control">要判断的控件</param>
    /// <returns>是否应该应用动画</returns>
    private static bool ShouldAnimateControl(UIBehaviour control)
    {
        // 排除一些不需要动画的控件类型或名称
        string name = control.gameObject.name;
        // 这里可以根据实际项目需求调整过滤规则
        if (name.Contains("Background") || name.Contains("Mask") || name.Contains("Viewport") ||
            name.Contains("Scrollbar") || name.Contains("Image") && name.Contains("(TMP)") == false ||
            name.Contains("Text (Legacy)") || name.Contains("Text (TMP)") && name.Contains("Label"))
            return false;

        return true;
    }

    /// <summary>
    /// 根据方向获取控件的起始位置
    /// </summary>
    /// <param name="control">控件游戏对象</param>
    /// <param name="direction">滑入方向</param>
    /// <returns>起始位置向量</returns>
    private static Vector3 GetStartPosition(GameObject control, SlideDirection direction)
    {
        // 根据屏幕尺寸和方向计算起始位置
        Canvas canvas = control.GetComponentInParent<Canvas>();
        if (canvas == null)
            return Vector3.zero;

        // 计算屏幕外的起始位置
        Vector3 startPos = Vector3.zero;
        float offset = 1000f; // 足够大的偏移量，确保控件开始时在屏幕外

        switch (direction)
        {
            case SlideDirection.Top:
                startPos = new Vector3(0, offset, 0);
                break;
            case SlideDirection.Bottom:
                startPos = new Vector3(0, -offset, 0);
                break;
            case SlideDirection.Left:
                startPos = new Vector3(-offset, 0, 0);
                break;
            case SlideDirection.Right:
                startPos = new Vector3(offset, 0, 0);
                break;
        }

        return startPos;
    }

    /// <summary>
    /// 根据方向和偏移量获取增强版的起始位置
    /// </summary>
    private static Vector3 GetEnhancedStartPosition(GameObject control, SlideDirection direction, float offset)
    {
        Vector3 startPos = Vector3.zero;

        switch (direction)
        {
            case SlideDirection.Top:
                startPos = new Vector3(0, offset, 0);
                break;
            case SlideDirection.Bottom:
                startPos = new Vector3(0, -offset, 0);
                break;
            case SlideDirection.Left:
                startPos = new Vector3(-offset, 0, 0);
                break;
            case SlideDirection.Right:
                startPos = new Vector3(offset, 0, 0);
                break;
        }

        return startPos;
    }

    /// <summary>
    /// 根据控件相对于面板中心点的位置确定滑入方向
    /// </summary>
    /// <param name="panelCenter">面板中心点世界坐标</param>
    /// <param name="controlPosition">控件位置世界坐标</param>
    /// <returns>确定的滑入方向</returns>
    private static SlideDirection DetermineSlideDirection(Vector3 panelCenter, Vector3 controlPosition)
    {
        // 计算控件相对于面板中心点的偏移
        Vector3 offset = controlPosition - panelCenter;

        // 比较水平和垂直偏移的绝对值，确定主要方向
        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
        {
            // 水平方向为主
            return offset.x < 0 ? SlideDirection.Left : SlideDirection.Right;
        }
        else
        {
            // 垂直方向为主
            return offset.y < 0 ? SlideDirection.Bottom : SlideDirection.Top;
        }
    }

    /// <summary>
    /// 播放单个控件的滑入动画
    /// </summary>
    private static void PlayControlSlideInAnimation(GameObject control, SlideDirection direction, float duration, Action onComplete)
    {
        RectTransform rectTransform = control.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;
        // 获取起始位置
        Vector3 startPos = GetStartPosition(control, direction);

        // 设置起始位置
        rectTransform.anchoredPosition = new Vector2(originalPos.x + startPos.x, originalPos.y + startPos.y);

        // 创建并播放动画
        rectTransform.DOAnchorPos(originalPos, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 播放单个控件的增强版滑入动画
    /// </summary>
    private static void PlayControlEnhancedSlideInAnimation(GameObject control, SlideDirection direction,
        float duration, float offset, Action onComplete)
    {
        RectTransform rectTransform = control.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;
        // 获取起始位置
        Vector3 startPos = GetEnhancedStartPosition(control, direction, offset);

        // 设置起始位置
        rectTransform.anchoredPosition = new Vector2(originalPos.x + startPos.x, originalPos.y + startPos.y);

        // 创建并播放动画
        rectTransform.DOAnchorPos(originalPos, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => onComplete?.Invoke());
    }

    #endregion


    #region 中心缩放动画
    /// <summary>
    /// 为面板应用从中心点缩放变大进入的动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="onComplete">动画完成后的回调</param>
    public static void PlayPanelCenterScaleInAnimation(this BasePanel panel, float duration = 0.5f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 直接使用DOTween API创建并播放动画，不再依赖PlayAnimation
        Vector3 originalScale = panel.transform.localScale;
        panel.transform.localScale = Vector3.zero;

        panel.transform.DOScale(Vector3.one, duration)
            .SetEase(Ease.OutBack, 1.2f, 0.3f)
            .OnComplete(() =>
            {
                // 恢复原始缩放值
                panel.transform.localScale = originalScale;
                // 调用完成回调
                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// 为面板及其所有子控件应用从中心点缩放变大进入的动画
    /// 面板先从中心点缩放进入，然后子控件按顺序从中心点缩放进入
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="panelDuration">面板动画持续时间</param>
    /// <param name="controlsDuration">控件动画持续时间</param>
    /// <param name="spreadFactor">子控件动画扩散因子</param>
    /// <param name="onComplete">所有动画完成后的回调</param>
    public static void PlayPanelWithControlsCenterScaleInAnimation(this BasePanel panel, float panelDuration = 0.5f,
        float controlsDuration = 0.3f, float spreadFactor = 0.1f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 先播放面板的中心缩放动画
        PlayPanelCenterScaleInAnimation(panel, panelDuration, () =>
        {
            // 面板动画完成后，播放子控件的中心缩放动画
            PlayControlsCenterScaleInAnimation(panel, controlsDuration, spreadFactor, onComplete);
        });
    }

    /// <summary>
    /// 为面板中的所有控件应用从中心点缩放变大进入的动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="spreadFactor">动画扩散因子（值越大，子控件动画延迟差异越大）</param>
    /// <param name="onComplete">所有动画完成后的回调</param>
    private static void PlayControlsCenterScaleInAnimation(BasePanel panel, float duration = 0.3f,
        float spreadFactor = 0.1f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 获取面板中所有可交互和可视的UI控件
        List<UIBehaviour> controls = new List<UIBehaviour>();
        panel.GetComponentsInChildren<UIBehaviour>(true, controls);

        // 过滤掉一些不需要动画的默认控件
        List<UIBehaviour> filteredControls = new List<UIBehaviour>();
        foreach (var control in controls)
        {
            if (control.gameObject.activeSelf && ShouldAnimateControl(control))
            {
                filteredControls.Add(control);
            }
        }

        // 如果没有需要动画的控件，直接调用回调
        if (filteredControls.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 动画完成计数器
        int animationCompleteCount = 0;
        int totalAnimations = filteredControls.Count;

        // 为每个控件应用从中心点缩放进入的动画
        for (int i = 0; i < filteredControls.Count; i++)
        {
            UIBehaviour control = filteredControls[i];
            float delay = i * spreadFactor; // 计算延迟时间

            // 使用DOTween的延迟方法
            DOVirtual.DelayedCall(delay, () =>
            {
                // 直接为控件播放中心缩放动画，不依赖PlayAnimation
                Vector3 originalScale = control.transform.localScale;
                control.transform.localScale = Vector3.zero;

                control.transform.DOScale(Vector3.one, duration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        // 恢复原始缩放值
                        control.transform.localScale = originalScale;

                        animationCompleteCount++;
                        if (animationCompleteCount >= totalAnimations)
                        {
                            onComplete?.Invoke();
                        }
                    });
            });
        }
    }

    #endregion

    #region 面板淡入淡出动画
    /// <summary>
    /// 为面板应用淡入动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="onComplete">动画完成后的回调</param>
    public static void PlayPanelFadeInAnimation(this BasePanel panel, float duration = 0.3f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 确保面板可见
        panel.gameObject.SetActive(true);
        // 直接使用DOTween实现淡入动画，不再依赖PlayAnimation
        DoPanelFadeInAnimation(panel,duration, onComplete);
    }

    /// <summary>
    /// 为面板应用淡出动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="setInactiveOnComplete">动画完成后是否设置面板为非激活状态</param>
    /// <param name="onComplete">动画完成后的回调</param>
    public static void PlayPanelFadeOutAnimation(this BasePanel panel, float duration = 0.3f,
        bool setInactiveOnComplete = true, Action onComplete = null)
    {
        if (panel == null || !panel.gameObject.activeSelf)
            return;

        // 直接使用DOTween实现淡出动画，并添加额外的完成逻辑
        DoPanelFadeOutAnimation(panel,duration, () =>
        {
            // 如果需要，在动画完成后设置面板为非激活状态
            if (setInactiveOnComplete)
            {
                panel.gameObject.SetActive(false);
            }
            // 调用用户提供的回调
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 为面板及其所有子控件应用淡入动画
    /// 面板先淡入，然后子控件按顺序淡入
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="panelDuration">面板动画持续时间</param>
    /// <param name="controlsDuration">控件动画持续时间</param>
    /// <param name="spreadFactor">子控件动画扩散因子</param>
    /// <param name="onComplete">所有动画完成后的回调</param>
    public static void PlayPanelWithControlsFadeInAnimation(this BasePanel panel, float panelDuration = 0.3f,
        float controlsDuration = 0.2f, float spreadFactor = 0.1f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 确保面板可见
        panel.gameObject.SetActive(true);

        // 先播放面板的淡入动画
        PlayPanelFadeInAnimation(panel, panelDuration, () =>
        {
            // 面板动画完成后，播放子控件的淡入动画
            PlayControlsFadeInAnimation(panel, controlsDuration, spreadFactor, onComplete);
        });
    }

    /// <summary>
    /// 为面板及其所有子控件应用淡出动画
    /// 子控件先按顺序淡出，然后面板淡出
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="controlsDuration">控件动画持续时间</param>
    /// <param name="panelDuration">面板动画持续时间</param>
    /// <param name="spreadFactor">子控件动画扩散因子</param>
    /// <param name="setInactiveOnComplete">动画完成后是否设置面板为非激活状态</param>
    /// <param name="onComplete">所有动画完成后的回调</param>
    public static void PlayPanelWithControlsFadeOutAnimation(this BasePanel panel, float controlsDuration = 0.2f,
        float panelDuration = 0.3f, float spreadFactor = 0.1f, bool setInactiveOnComplete = true, Action onComplete = null)
    {
        if (panel == null || !panel.gameObject.activeSelf)
            return;

        // 先播放子控件的淡出动画
        PlayControlsFadeOutAnimation(panel, controlsDuration, spreadFactor, () =>
        {
            // 子控件动画完成后，播放面板的淡出动画
            PlayPanelFadeOutAnimation(panel, panelDuration, setInactiveOnComplete, onComplete);
        });
    }

    /// <summary>
    /// 为面板中的所有控件应用淡入动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="spreadFactor">动画扩散因子（值越大，子控件动画延迟差异越大）</param>
    /// <param name="onComplete">所有动画完成后的回调</param>
    private static void PlayControlsFadeInAnimation(BasePanel panel, float duration = 0.2f,
        float spreadFactor = 0.1f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 获取面板中所有可交互和可视的UI控件
        List<UIBehaviour> controls = new List<UIBehaviour>();
        panel.GetComponentsInChildren<UIBehaviour>(true, controls);

        // 过滤掉一些不需要动画的默认控件
        List<UIBehaviour> filteredControls = new List<UIBehaviour>();
        foreach (var control in controls)
        {
            if (control.gameObject.activeSelf && ShouldAnimateControl(control))
            {
                filteredControls.Add(control);
            }
        }

        // 如果没有需要动画的控件，直接调用回调
        if (filteredControls.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 动画完成计数器
        int animationCompleteCount = 0;
        int totalAnimations = filteredControls.Count;

        // 为每个控件应用淡入动画
        for (int i = 0; i < filteredControls.Count; i++)
        {
            UIBehaviour control = filteredControls[i];
            float delay = i * spreadFactor; // 计算延迟时间
            GameObject controlGO = control.gameObject;

            // 使用DOTween的延迟方法
            DOVirtual.DelayedCall(delay, () =>
            {
                // 直接使用DOTween API实现淡入动画
                CanvasGroup canvasGroup = controlGO.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = controlGO.AddComponent<CanvasGroup>();
                }
                float startAlpha = canvasGroup.alpha;
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(startAlpha > 0 ? startAlpha : 1, duration).OnComplete(() =>
                {
                    animationCompleteCount++;
                    if (animationCompleteCount >= totalAnimations)
                    {
                        onComplete?.Invoke();
                    }
                });
            });
        }
    }

    /// <summary>
    /// 为面板中的所有控件应用淡出动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="spreadFactor">动画扩散因子（值越大，子控件动画延迟差异越大）</param>
    /// <param name="onComplete">所有动画完成后的回调</param>
    private static void PlayControlsFadeOutAnimation(BasePanel panel, float duration = 0.2f,
        float spreadFactor = 0.1f, Action onComplete = null)
    {
        if (panel == null)
            return;

        // 获取面板中所有可交互和可视的UI控件
        List<UIBehaviour> controls = new List<UIBehaviour>();
        panel.GetComponentsInChildren<UIBehaviour>(true, controls);

        // 过滤掉一些不需要动画的默认控件
        List<UIBehaviour> filteredControls = new List<UIBehaviour>();
        foreach (var control in controls)
        {
            if (control.gameObject.activeSelf && ShouldAnimateControl(control))
            {
                filteredControls.Add(control);
            }
        }

        // 如果没有需要动画的控件，直接调用回调
        if (filteredControls.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        // 动画完成计数器
        int animationCompleteCount = 0;
        int totalAnimations = filteredControls.Count;

        // 为每个控件应用淡出动画
        for (int i = 0; i < filteredControls.Count; i++)
        {
            UIBehaviour control = filteredControls[i];
            float delay = i * spreadFactor; // 计算延迟时间
            GameObject controlGO = control.gameObject;

            // 使用DOTween的延迟方法
            DOVirtual.DelayedCall(delay, () =>
            {
                // 直接使用DOTween API实现淡出动画
                CanvasGroup canvasGroup = controlGO.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = controlGO.AddComponent<CanvasGroup>();
                }
                canvasGroup.DOFade(0, duration).OnComplete(() =>
                {
                    animationCompleteCount++;
                    if (animationCompleteCount >= totalAnimations)
                    {
                        onComplete?.Invoke();
                    }
                });
            });
        }
    }

    #endregion

    #region 面板滑出动画
    /// <summary>
    /// 面板顶部滑出动画
    /// </summary>
    public static void DoPanelSlideOutToTop(this BasePanel panel, float duration = 0.5f, Action onComplete = null)
    {
        if (panel == null)
            return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;

        // 设置目标位置（屏幕顶部外）
        Vector2 targetPos = new Vector2(originalPos.x, originalPos.y + 2000f);

        // 创建并播放动画
        rectTransform.DOAnchorPos(targetPos, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => 
            {
                rectTransform.anchoredPosition = originalPos;
                onComplete?.Invoke();
                panel.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// 面板底部滑出动画
    /// </summary>
    public static void DoPanelSlideOutToBottom(this BasePanel panel, float duration = 0.5f, Action onComplete = null)
    {
        if (panel == null)
            return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;

        // 设置目标位置（屏幕底部外）
        Vector2 targetPos = new Vector2(originalPos.x, originalPos.y - 2000f);

        // 创建并播放动画
        rectTransform.DOAnchorPos(targetPos, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                rectTransform.anchoredPosition = originalPos;
                onComplete?.Invoke();
                panel.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// 面板左侧滑出动画
    /// </summary>
    public static void DoPanelSlideOutToLeft(this BasePanel panel, float duration = 0.5f, Action onComplete = null)
    {
        if (panel == null)
            return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;

        // 设置目标位置（屏幕左侧外）
        Vector2 targetPos = new Vector2(originalPos.x - 2000f, originalPos.y);

        // 创建并播放动画
        rectTransform.DOAnchorPos(targetPos, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                rectTransform.anchoredPosition = originalPos;
                onComplete?.Invoke();
                panel.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// 面板右侧滑出动画
    /// </summary>
    public static void DoPanelSlideOutToRight(this BasePanel panel, float duration = 0.5f, Action onComplete = null)
    {
        if (panel == null)
            return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
            return;

        // 保存原始位置
        Vector2 originalPos = rectTransform.anchoredPosition;

        // 设置目标位置（屏幕右侧外）
        Vector2 targetPos = new Vector2(originalPos.x + 2000f, originalPos.y);

        // 创建并播放动画
        rectTransform.DOAnchorPos(targetPos, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                rectTransform.anchoredPosition = originalPos;
                onComplete?.Invoke();
                panel.gameObject.SetActive(false);
            });
    }
#endregion
}