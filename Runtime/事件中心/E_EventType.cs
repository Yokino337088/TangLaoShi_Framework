using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型 枚举
/// </summary>
public enum E_EventType 
{
    E_SceneLoadChange,

    /// <summary>
    /// 水平热键 -1~1的事件监听
    /// </summary>
    E_Input_Horizontal,

    /// <summary>
    /// 竖直热键 -1~1的事件监听
    /// </summary>
    E_Input_Vertical,

    /// <summary>
    /// 选择框选择项改变事件
    /// </summary>
    OnSelectChange,

    显示背包物件信息,

    显示AB包大小,
    显示更新信息,
    更新进度条,
}
