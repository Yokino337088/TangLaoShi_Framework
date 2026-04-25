using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// DOTween UGUI控件动画扩展类
/// 为UGUI的各种控件提供丰富的动画视觉效果
/// </summary>
public static class DOTweenUGUIAnimationExtension
{
    #region 通用动画配置
    // 基础动画持续时间
    private const float BASE_ANIM_DURATION = 0.2f;
    
    // 按钮动画配置
    private const float BUTTON_CLICK_SCALE = 0.6f;
    private const float BUTTON_HOVER_SCALE = 1.05f;
    private const float BUTTON_CLICK_DURATION = 0.3f;
    
    // 文本动画配置
    private const float TEXT_POP_DURATION = 0.4f;
    
    // 图片动画配置
    private const float IMAGE_FLASH_DURATION = 0.2f;
    
    // 滑动动画配置
    private const float SLIDE_DURATION = 0.3f;
    #endregion

    #region 按钮动画效果
    /// <summary>
    /// 为按钮添加高级点击动画效果
    /// </summary>
    /// <param name="button">目标按钮</param>
    /// <param name="clickScale">点击时的缩放比例</param>
    /// <param name="scaleDuration">缩放动画持续时间</param>
    /// <param name="useColorEffect">是否使用颜色效果</param>
    /// <param name="usePunchEffect">是否使用冲击效果</param>
    public static void AddAdvancedButtonAnimation(this Button button, 
                                                  float clickScale = BUTTON_CLICK_SCALE, 
                                                  float scaleDuration = BUTTON_CLICK_DURATION,
                                                  bool useColorEffect = true,
                                                  bool usePunchEffect = false)
    {
        if (button == null) return;

        // 保存原始状态
        Vector3 originalScale = button.transform.localScale;
        Image buttonImage = button.GetComponent<Image>();
        Color originalColor = buttonImage != null ? buttonImage.color : Color.white;

        // 添加点击事件
        button.onClick.AddListener(() =>
        {
            // 停止之前的所有动画
            button.transform.DOKill();
            if (buttonImage != null) buttonImage.DOKill();

            // 点击缩放动画
            button.transform.DOScale(clickScale, scaleDuration).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    // 恢复原始缩放
                    button.transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutQuad);
                });

            // 颜色闪烁效果
            if (useColorEffect && buttonImage != null)
            {
                buttonImage.DOColor(Color.gray, scaleDuration * 0.5f).SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        buttonImage.DOColor(originalColor, scaleDuration * 0.5f).SetEase(Ease.OutQuad);
                    });
            }

            // 冲击效果
            if (usePunchEffect)
            {
                button.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), scaleDuration * 2f, 2, 0.5f);
            }
        });
    }

    /// <summary>
    /// 为按钮添加波纹效果
    /// </summary>
    /// <param name="button">目标按钮</param>
    /// <param name="rippleColor">波纹颜色</param>
    /// <param name="duration">动画持续时间</param>
    public static void AddRippleEffect(this Button button, Color rippleColor = default, float duration = 0.5f)
    {
        if (button == null) return;

        // 如果没有指定波纹颜色，使用半透明白色
        if (rippleColor == default) rippleColor = new Color(1f, 1f, 1f, 0.5f);

        button.onClick.AddListener(() =>
        {
            // 创建波纹效果
            Image rippleImage = CreateRippleImage(button, rippleColor);
            if (rippleImage == null) return;

            // 波纹动画
            RectTransform rippleRect = rippleImage.GetComponent<RectTransform>();
            float maxSize = Mathf.Max(button.GetComponent<RectTransform>().sizeDelta.x, 
                                     button.GetComponent<RectTransform>().sizeDelta.y) * 2f;

            rippleRect.localScale = Vector3.zero;
            rippleImage.color = new Color(rippleColor.r, rippleColor.g, rippleColor.b, 0f);

            rippleImage.DOColor(new Color(rippleColor.r, rippleColor.g, rippleColor.b, rippleColor.a), duration * 0.2f)
                .OnComplete(() =>
                {
                    rippleImage.DOColor(new Color(rippleColor.r, rippleColor.g, rippleColor.b, 0f), duration * 0.8f);
                });

            rippleRect.DOScale(maxSize, duration).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    UnityEngine.Object.Destroy(rippleImage.gameObject);
                });
        });
    }

    /// <summary>
    /// 创建波纹效果的Image对象
    /// </summary>
    private static Image CreateRippleImage(Button button, Color color)
    {
        try
        {
            GameObject rippleGO = new GameObject("RippleEffect");
            rippleGO.transform.SetParent(button.transform, false);
            rippleGO.transform.SetAsLastSibling();

            // 设置RectTransform
            RectTransform rippleRect = rippleGO.AddComponent<RectTransform>();
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            rippleRect.anchorMin = Vector2.zero;
            rippleRect.anchorMax = Vector2.one;
            rippleRect.sizeDelta = Vector2.zero;
            rippleRect.pivot = Vector2.one * 0.5f;

            // 添加Image组件
            Image rippleImage = rippleGO.AddComponent<Image>();
            rippleImage.color = color;
            rippleImage.raycastTarget = false;

            // 使用圆形精灵或创建一个圆形纹理
            if (Resources.Load<Sprite>("Circle") is Sprite circleSprite)
            {
                rippleImage.sprite = circleSprite;
            }
            else
            {
                // 如果没有圆形精灵，使用默认的Image
                rippleImage.type = Image.Type.Filled;
                rippleImage.fillMethod = Image.FillMethod.Radial360;
                rippleImage.fillAmount = 1f;
            }

            return rippleImage;
        }
        catch (Exception e)
        {
            LogSystem.Error("创建波纹效果失败: " + e.Message);
            return null;
        }
    }
    #endregion

    #region 文本动画效果
    /// <summary>
    /// 为文本添加弹出动画效果
    /// </summary>
    /// <param name="text">目标文本</param>
    /// <param name="scaleFactor">缩放因子</param>
    /// <param name="duration">动画持续时间</param>
    public static void DoTextPopAnimation(this Text text, float scaleFactor = 1.5f, float duration = TEXT_POP_DURATION)
    {
        if (text == null) return;

        text.transform.DOKill();
        text.transform.localScale = Vector3.zero;
        text.transform.DOScale(scaleFactor, duration * 0.6f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                text.transform.DOScale(1f, duration * 0.4f).SetEase(Ease.InBack);
            });
    }

    /// <summary>
    /// 为文本添加渐入动画效果
    /// </summary>
    /// <param name="text">目标文本</param>
    /// <param name="duration">动画持续时间</param>
    public static void DoTextFadeIn(this Text text, float duration = BASE_ANIM_DURATION)
    {
        if (text == null) return;

        text.canvasRenderer.SetAlpha(0f);
        text.CrossFadeAlpha(1f, duration, false);
    }

    /// <summary>
    /// 为文本添加渐出动画效果
    /// </summary>
    /// <param name="text">目标文本</param>
    /// <param name="duration">动画持续时间</param>
    public static void DoTextFadeOut(this Text text, float duration = BASE_ANIM_DURATION)
    {
        if (text == null) return;

        text.canvasRenderer.SetAlpha(1f);
        text.CrossFadeAlpha(0f, duration, false);
    }
    #endregion

    #region 图片动画效果
    /// <summary>
    /// 为图片添加闪烁效果
    /// </summary>
    /// <param name="image">目标图片</param>
    /// <param name="flashCount">闪烁次数</param>
    /// <param name="duration">总动画持续时间</param>
    /// <param name="finalAlpha">最终透明度</param>
    public static void DoImageFlash(this Image image, int flashCount = 2, float duration = IMAGE_FLASH_DURATION, float finalAlpha = 1f)
    {
        if (image == null) return;

        image.DOKill();
        Color originalColor = image.color;
        float flashInterval = duration / (flashCount * 2);

        Sequence flashSequence = DOTween.Sequence();

        for (int i = 0; i < flashCount; i++)
        {
            flashSequence.Append(image.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0f), flashInterval));
            flashSequence.Append(image.DOColor(originalColor, flashInterval));
        }

        flashSequence.OnComplete(() =>
        {
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, finalAlpha);
        });
    }

    /// <summary>
    /// 为图片添加旋转动画
    /// </summary>
    /// <param name="image">目标图片</param>
    /// <param name="degrees">旋转角度</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="isLoop">是否循环</param>
    public static void DoImageRotate(this Image image, float degrees, float duration = BASE_ANIM_DURATION, bool isLoop = false)
    {
        if (image == null) return;

        image.transform.DOKill();
        Tween rotateTween = image.transform.DORotate(new Vector3(0, 0, degrees), duration, RotateMode.Fast);
        
        if (isLoop)
        {
            rotateTween.SetLoops(-1, LoopType.Restart);
        }
    }
    #endregion

    #region 滑动动画效果
    /// <summary>
    /// 从左侧滑入动画
    /// </summary>
    /// <param name="rectTransform">目标RectTransform</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="offset">偏移量</param>
    public static void DoSlideInFromLeft(this RectTransform rectTransform, float duration = SLIDE_DURATION, float offset = 200f)
    {
        if (rectTransform == null) return;

        Vector2 originalPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(-offset, originalPosition.y);
        
        rectTransform.DOKill();
        rectTransform.DOAnchorPos(originalPosition, duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 从右侧滑入动画
    /// </summary>
    /// <param name="rectTransform">目标RectTransform</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="offset">偏移量</param>
    public static void DoSlideInFromRight(this RectTransform rectTransform, float duration = SLIDE_DURATION, float offset = 200f)
    {
        if (rectTransform == null) return;

        Vector2 originalPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(offset, originalPosition.y);
        
        rectTransform.DOKill();
        rectTransform.DOAnchorPos(originalPosition, duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 从上方滑入动画
    /// </summary>
    /// <param name="rectTransform">目标RectTransform</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="offset">偏移量</param>
    public static void DoSlideInFromTop(this RectTransform rectTransform, float duration = SLIDE_DURATION, float offset = 200f)
    {
        if (rectTransform == null) return;

        Vector2 originalPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(originalPosition.x, offset);
        
        rectTransform.DOKill();
        rectTransform.DOAnchorPos(originalPosition, duration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 从下方滑入动画
    /// </summary>
    /// <param name="rectTransform">目标RectTransform</param>
    /// <param name="duration">动画持续时间</param>
    /// <param name="offset">偏移量</param>
    public static void DoSlideInFromBottom(this RectTransform rectTransform, float duration = SLIDE_DURATION, float offset = 200f)
    {
        if (rectTransform == null) return;

        Vector2 originalPosition = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = new Vector2(originalPosition.x, -offset);
        
        rectTransform.DOKill();
        rectTransform.DOAnchorPos(originalPosition, duration).SetEase(Ease.OutQuad);
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 停止目标对象的所有DOTween动画
    /// </summary>
    /// <param name="target">目标对象</param>
    public static void StopAllTweens(this GameObject target)
    {
        if (target == null) return;
        
        target.transform.DOKill();
        
        // 停止所有子对象的动画
        foreach (Transform child in target.transform)
        {
            child.gameObject.StopAllTweens();
        }
    }

    /// <summary>
    /// 为事件触发器添加事件
    /// </summary>
    private static void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, Action<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => action(data));
        trigger.triggers.Add(entry);
    }
    #endregion
}