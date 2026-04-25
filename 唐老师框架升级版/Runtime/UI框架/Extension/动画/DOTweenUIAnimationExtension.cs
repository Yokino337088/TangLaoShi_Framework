using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.Events; // 添加TextMeshPro支持

/// <summary>
/// DOTween UI控件动画扩展类
/// 为UI控件添加点击、悬停和离开动画
/// </summary>
public static class DOTweenUIAnimationExtension
{
    // 动画持续时间配置
    private const float CLICK_ANIM_DURATION = 0.25f;
    private const float HOVER_ANIM_DURATION = 0.3f;
    private const float SCALE_FACTOR_CLICK = 0.85f;
    private const float SCALE_FACTOR_HOVER = 1.15f;

    /// <summary>
    /// 为Image组件添加鼠标进入和离开的颜色变换动画
    /// </summary>
    /// <param name="image">目标Image组件</param>
    /// <param name="hoverColor">鼠标悬停时的颜色</param>
    /// <param name="duration">颜色过渡动画的持续时间</param>
    public static void AddImageHoverColorEffect(this Image image, 
                                               Color hoverColor, 
                                               float duration = HOVER_ANIM_DURATION)
    {
        if (image == null) return;

        // 保存原始颜色
        Color originalColor = image.color;

        // 添加事件触发器以捕获鼠标进入和离开事件
        EventTrigger trigger = image.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = image.gameObject.AddComponent<EventTrigger>();
        }

        // 鼠标进入事件
        AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) =>
        {
            // 停止之前的动画并应用新的颜色动画
            image.DOKill();
            image.DOColor(hoverColor, duration).SetEase(Ease.OutQuad);
        });

        // 鼠标离开事件
        AddEventTrigger(trigger, EventTriggerType.PointerExit, (data) =>
        {
            // 停止之前的动画并恢复原始颜色
            image.DOKill();
            image.DOColor(originalColor, duration).SetEase(Ease.OutQuad);
        });
    }

    /// <summary>
    /// 为Image组件添加鼠标进入和离开的颜色变换动画（使用颜色偏移量）
    /// </summary>
    /// <param name="image">目标Image组件</param>
    /// <param name="colorOffset">颜色偏移量（相对于原始颜色）</param>
    /// <param name="duration">颜色过渡动画的持续时间</param>
    public static void AddImageHoverColorEffect(this Image image, 
                                               float colorOffset = 1.1f, 
                                               float duration = HOVER_ANIM_DURATION)
    {
        if (image == null) return;

        // 计算悬停颜色（基于原始颜色乘以偏移量）
        Color hoverColor = image.color * colorOffset;
        hoverColor.a = image.color.a; // 保持原始透明度

        // 调用主要的颜色变换方法
        AddImageHoverColorEffect(image, hoverColor, duration);
    }
    
    /// <summary>
    /// 为Image组件添加鼠标进入和离开的缩放动画
    /// </summary>
    /// <param name="image">目标Image组件</param>
    /// <param name="hoverScale">鼠标悬停时的缩放比例</param>
    /// <param name="duration">缩放动画的持续时间</param>
    public static void AddImageHoverScaleEffect(this Image image, 
                                              float hoverScale = SCALE_FACTOR_HOVER, 
                                              float duration = HOVER_ANIM_DURATION)
    {
        if (image == null) return;

        // 保存原始缩放
        Vector3 originalScale = image.transform.localScale;

        // 添加事件触发器以捕获鼠标进入和离开事件
        EventTrigger trigger = image.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = image.gameObject.AddComponent<EventTrigger>();
        }

        // 鼠标进入事件 - 放大
        AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) =>
        {
            // 停止之前的动画并应用新的缩放动画
            image.transform.DOKill();
            image.transform.DOScale(hoverScale, duration).SetEase(Ease.OutQuad);
        });

        // 鼠标离开事件 - 复原
        AddEventTrigger(trigger, EventTriggerType.PointerExit, (data) =>
        {
            // 停止之前的动画并恢复原始缩放
            image.transform.DOKill();
            image.transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad);
        });
    }

    /// <summary>
    /// 为面板中的所有Button、Toggle和Image控件添加动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    /// <param name="autoAddImageEffects">是否自动为Image添加默认悬停效果</param>
    /// <param name="imageHoverColorOffset">Image悬停时的颜色偏移量</param>
    /// <param name="addScaleEffect">是否为Image添加缩放效果</param>
    /// <param name="imageHoverScale">Image悬停时的缩放比例</param>
    public static void AddAllControlsAnimation1(this BasePanel panel, 
                                              bool autoAddImageEffects = false, 
                                              float imageHoverColorOffset = 1.25f,
                                              bool addScaleEffect = false,
                                              float imageHoverScale = SCALE_FACTOR_HOVER)
    {
        if (panel == null) return;

        // 为所有Button添加动画
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            // 跳过默认命名的控件
            if (!IsDefaultControl(button.gameObject.name))
            {
                button.AddButtonAnimation();
            }
        }

        // 为所有Toggle添加动画
        Toggle[] toggles = panel.GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles)
        {
            // 跳过默认命名的控件
            if (!IsDefaultControl(toggle.gameObject.name))
            {
                toggle.AddToggleAnimation();
            }
        }

        // 可选：为所有Image添加悬停效果
        if (autoAddImageEffects)
        {
            Image[] images = panel.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                // 跳过默认命名的控件和已经是Button或Toggle一部分的Image
                if (!IsDefaultControl(image.gameObject.name) &&
                    image.GetComponent<Button>() == null &&
                    image.GetComponent<Toggle>() == null)
                {
                    image.AddImageHoverColorEffect(imageHoverColorOffset);
                    
                    // 如果需要添加缩放效果
                    if (addScaleEffect)
                    {
                        image.AddImageHoverScaleEffect(imageHoverScale);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 为按钮添加点击和悬停动画
    /// </summary>
    /// <param name="button">目标按钮</param>
    /// <param name="clickScale">点击时的缩放比例</param>
    /// <param name="hoverScale">悬停时的缩放比例</param>
    /// <param name="useColorChange">是否使用颜色变化</param>
    public static void AddButtonAnimation(this Button button, 
                                          float clickScale = SCALE_FACTOR_CLICK, 
                                          float hoverScale = SCALE_FACTOR_HOVER, 
                                          bool useColorChange = true)
    {
        if (button == null) return;

        // 保存原始缩放
        Vector3 originalScale = button.transform.localScale;
        Image buttonImage = button.GetComponent<Image>();
        Color originalColor = buttonImage != null ? buttonImage.color : Color.white;
        Color hoverColor = originalColor * 1.1f; // 悬停时稍微变亮

        // 添加事件触发器以捕获鼠标进入和离开事件
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // 鼠标进入事件
        AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) =>
        {
            // 悬停缩放动画
            button.transform.DOKill();
            button.transform.DOScale(hoverScale, HOVER_ANIM_DURATION).SetEase(Ease.OutQuad);

            // 悬停颜色动画
            if (useColorChange && buttonImage != null)
            {
                buttonImage.DOKill();
                buttonImage.DOColor(hoverColor, HOVER_ANIM_DURATION).SetEase(Ease.OutQuad);
            }
        });

        // 鼠标离开事件
        AddEventTrigger(trigger, EventTriggerType.PointerExit, (data) =>
        {
            // 恢复原始缩放
            button.transform.DOKill();
            button.transform.DOScale(originalScale, HOVER_ANIM_DURATION).SetEase(Ease.OutQuad);

            // 恢复原始颜色
            if (useColorChange && buttonImage != null)
            {
                buttonImage.DOKill();
                buttonImage.DOColor(originalColor, HOVER_ANIM_DURATION).SetEase(Ease.OutQuad);
            }
        });

        // 点击事件（在原有点击逻辑基础上添加动画）
        button.onClick.AddListener(() =>
        {
            // 点击缩放动画
            button.transform.DOKill();
            button.transform.DOScale(clickScale, CLICK_ANIM_DURATION).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    // 动画完成后恢复原始缩放
                    button.transform.DOScale(originalScale, CLICK_ANIM_DURATION).SetEase(Ease.OutQuad);
                });
        });
    }

    /// <summary>
    /// 为Toggle添加点击和悬停动画
    /// </summary>
    /// <param name="toggle">目标Toggle</param>
    /// <param name="hoverScale">悬停时的缩放比例</param>
    public static void AddToggleAnimation(this Toggle toggle, float hoverScale = SCALE_FACTOR_HOVER)
    {
        if (toggle == null) return;

        // 保存原始缩放
        Vector3 originalScale = toggle.transform.localScale;

        // 添加事件触发器
        EventTrigger trigger = toggle.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = toggle.gameObject.AddComponent<EventTrigger>();
        }

        // 鼠标进入事件
        AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) =>
        {
            toggle.transform.DOKill();
            toggle.transform.DOScale(hoverScale, HOVER_ANIM_DURATION).SetEase(Ease.OutQuad);
        });

        // 鼠标离开事件
        AddEventTrigger(trigger, EventTriggerType.PointerExit, (data) =>
        {
            toggle.transform.DOKill();
            toggle.transform.DOScale(originalScale, HOVER_ANIM_DURATION).SetEase(Ease.OutQuad);
        });
    }

    /// <summary>
    /// 为面板中的所有Button和Toggle控件添加动画
    /// </summary>
    /// <param name="panel">目标面板</param>
    public static void AddAllControlsAnimation(this BasePanel panel)
    {
        if (panel == null) return;

        // 为所有Button添加动画
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            // 跳过默认命名的控件
            if (!IsDefaultControl(button.gameObject.name))
            {
                button.AddButtonAnimation();
            }
        }

        // 为所有Toggle添加动画
        Toggle[] toggles = panel.GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles)
        {
            // 跳过默认命名的控件
            if (!IsDefaultControl(toggle.gameObject.name))
            {
                toggle.AddToggleAnimation();
            }
        }
    }

    /// <summary>
    /// 检查控件是否为默认命名的控件
    /// </summary>
    /// <param name="controlName">控件名称</param>
    /// <returns>是否为默认控件</returns>
    private static bool IsDefaultControl(string controlName)
    {
        // 这里使用BasePanel中的默认名字列表
        return new List<string>() 
        {
            "Image", "Text (TMP)", "RawImage", "Background", 
            "Checkmark", "Label", "Text (Legacy)", "Arrow", 
            "Placeholder", "Fill", "Handle", "Viewport",
            "Scrollbar Horizontal", "Scrollbar Vertical"
        }.Contains(controlName);
    }

    /// <summary>
    /// 向EventTrigger添加事件
    /// </summary>
    /// <param name="trigger">EventTrigger组件</param>
    /// <param name="eventType">事件类型</param>
    /// <param name="callback">回调函数</param>
    private static void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityAction<BaseEventData> callback)
    {
        // 检查是否已存在相同类型的事件
        foreach (EventTrigger.Entry entry in trigger.triggers)
        {
            if (entry.eventID == eventType)
            {
                // 如果已存在，添加回调而不是创建新条目
                entry.callback.AddListener(new UnityAction<BaseEventData>(callback));
                return;
            }
        }

        // 创建新的事件条目
        EventTrigger.Entry newEntry = new EventTrigger.Entry();
        newEntry.eventID = eventType;
        newEntry.callback.AddListener(new UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(newEntry);
    }

    #region GameObject淡入淡出功能
    
    /// <summary>
    /// 为GameObject及其子物体添加淡入淡出效果
    /// </summary>
    /// <param name="gameObject">目标GameObject</param>
    /// <param name="targetAlpha">目标透明度（0-1）</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="includeChildren">是否包含子物体</param>
    /// <param name="ease">动画缓动函数</param>
    /// <param name="onComplete">动画完成回调</param>
    /// <returns>Tween对象，可用于控制动画</returns>
    public static Tween SetAlpha(this GameObject gameObject, float targetAlpha, 
                                float duration = 0.3f, bool includeChildren = true,
                                Ease ease = Ease.OutQuad, Action onComplete = null)
    {
        if (gameObject == null || duration <= 0)
        {
            onComplete?.Invoke();
            return null;
        }

        // 收集所有需要处理的组件
        List<object> renderComponents = new List<object>();
        
        // 获取当前GameObject的渲染组件
        CollectRenderComponents(gameObject, renderComponents);
        
        // 如果需要包含子物体
        if (includeChildren)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                // 跳过已添加的组件
                if (!renderComponents.Contains(renderer))
                    renderComponents.Add(renderer);
            }
            
            Image[] images = gameObject.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
            {
                if (!renderComponents.Contains(image))
                    renderComponents.Add(image);
            }
            
            Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
            {
                if (!renderComponents.Contains(text))
                    renderComponents.Add(text);
            }
            
            TextMeshProUGUI[] tmpTexts = gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var tmpText in tmpTexts)
            {
                if (!renderComponents.Contains(tmpText))
                    renderComponents.Add(tmpText);
            }
            
            RawImage[] rawImages = gameObject.GetComponentsInChildren<RawImage>(true);
            foreach (var rawImage in rawImages)
            {
                if (!renderComponents.Contains(rawImage))
                    renderComponents.Add(rawImage);
            }
        }
        
        // 如果没有找到任何渲染组件
        if (renderComponents.Count == 0)
        {
            onComplete?.Invoke();
            return null;
        }
        
        // 为每个组件创建动画
        Sequence sequence = DOTween.Sequence();
        
        foreach (var component in renderComponents)
        {
            switch (component)
            {
                case Image image:
                    sequence.Join(t: image.DOFade(targetAlpha, duration).SetEase(ease));
                    break;
                case Text text:
                    sequence.Join(text.DOFade(targetAlpha, duration).SetEase(ease));
                    break;
                case TextMeshProUGUI tmpText:
                    sequence.Join(tmpText.DOFade(targetAlpha, duration).SetEase(ease));
                    break;
                case RawImage rawImage:
                    sequence.Join(rawImage.DOFade(targetAlpha, duration).SetEase(ease));
                    break;
                case Renderer renderer:
                    // 处理3D渲染器
                    foreach (Material material in renderer.materials)
                    {
                        if (material.HasProperty("_Color"))
                        {
                            sequence.Join(material.DOFade(targetAlpha, duration).SetEase(ease));
                        }
                    }
                    break;
            }
        }
        
        // 设置完成回调
        if (onComplete != null)
        {
            sequence.OnComplete(() => onComplete());
        }
        
        return sequence;
    }
    
    /// <summary>
    /// 为GameObject及其子物体添加淡入效果
    /// </summary>
    /// <param name="gameObject">目标GameObject</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="includeChildren">是否包含子物体</param>
    /// <param name="ease">动画缓动函数</param>
    /// <param name="onComplete">动画完成回调</param>
    /// <returns>Tween对象，可用于控制动画</returns>
    public static Tween FadeIn(this GameObject gameObject, 
                             float duration = 0.3f, Action onComplete = null,
                             bool includeChildren = true,
                             Ease ease = Ease.OutQuad)
    {
        // 确保GameObject是激活的
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        
        return gameObject.SetAlpha(1f, duration, includeChildren, ease, onComplete);
    }
    
    /// <summary>
    /// 为GameObject及其子物体添加淡出效果
    /// </summary>
    /// <param name="gameObject">目标GameObject</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="includeChildren">是否包含子物体</param>
    /// <param name="ease">动画缓动函数</param>
    /// <param name="deactivateOnComplete">动画完成后是否禁用GameObject</param>
    /// <param name="onComplete">动画完成回调</param>
    /// <returns>Tween对象，可用于控制动画</returns>
    public static Tween FadeOut(this GameObject gameObject, 
                              float duration = 0.3f,
                              Action onComplete = null,bool includeChildren = true,
                              Ease ease = Ease.InQuad, bool deactivateOnComplete = true
                              )
    {
        return gameObject.SetAlpha(0f, duration, includeChildren, ease, () =>
        {
            // 动画完成后禁用GameObject（如果需要）
            if (deactivateOnComplete && gameObject != null)
            {
                gameObject.SetActive(false);
            }
            
            // 调用用户提供的回调
            onComplete?.Invoke();
        });
    }
    
    /// <summary>
    /// 收集GameObject上的渲染组件
    /// </summary>
    /// <param name="gameObject">目标GameObject</param>
    /// <param name="components">收集组件的列表</param>
    private static void CollectRenderComponents(GameObject gameObject, List<object> components)
    {
        Image image = gameObject.GetComponent<Image>();
        if (image != null) components.Add(image);
        
        Text text = gameObject.GetComponent<Text>();
        if (text != null) components.Add(text);
        
        TextMeshProUGUI tmpText = gameObject.GetComponent<TextMeshProUGUI>();
        if (tmpText != null) components.Add(tmpText);
        
        RawImage rawImage = gameObject.GetComponent<RawImage>();
        if (rawImage != null) components.Add(rawImage);
        
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null) components.Add(renderer);
    }
    
    #endregion
}