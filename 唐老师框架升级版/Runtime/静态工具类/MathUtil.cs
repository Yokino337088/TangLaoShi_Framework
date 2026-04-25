using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MathUtil
{
    #region 角度和弧度
    /// <summary>
    /// 角度转弧度的方法
    /// </summary>
    /// <param name="deg">角度值</param>
    /// <returns>弧度值</returns>
    public static float Deg2Rad(float deg)
    {
        return deg * Mathf.Deg2Rad;
    }

    /// <summary>
    /// 弧度转角度的方法
    /// </summary>
    /// <param name="rad">弧度值</param>
    /// <returns>角度值</returns>
    public static float Rad2Deg(float rad)
    {
        return rad * Mathf.Rad2Deg;
    }
    #endregion

    #region 距离计算相关的
    /// <summary>
    /// 获取XZ平面上 两点的距离
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns></returns>
    public static float GetObjDistanceXZ(Vector3 srcPos, Vector3 targetPos)
    {
        srcPos.y = 0;
        targetPos.y = 0;
        return Vector3.Distance(srcPos, targetPos);
    }

    /// <summary>
    /// 判断两点之间距离 是否小于等于目标距离 XZ平面
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <param name="dis">距离</param>
    /// <returns></returns>
    public static bool CheckObjDistanceXZ(Vector3 srcPos, Vector3 targetPos, float dis)
    {
        return GetObjDistanceXZ(srcPos, targetPos) <= dis;
    }

    /// <summary>
    /// 获取XY平面上 两点的距离
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <returns></returns>
    public static float GetObjDistanceXY(Vector3 srcPos, Vector3 targetPos)
    {
        srcPos.z = 0;
        targetPos.z = 0;
        return Vector3.Distance(srcPos, targetPos);
    }

    /// <summary>
    /// 判断两点之间距离 是否小于等于目标距离 XY平面
    /// </summary>
    /// <param name="srcPos">点1</param>
    /// <param name="targetPos">点2</param>
    /// <param name="dis">距离</param>
    /// <returns></returns>
    public static bool CheckObjDistanceXY(Vector3 srcPos, Vector3 targetPos, float dis)
    {
        return GetObjDistanceXY(srcPos, targetPos) <= dis;
    }

    #endregion

    #region 位置判断相关
    /// <summary>
    /// 判断世界坐标系下的某一个点 是否在屏幕可见范围外
    /// </summary>
    /// <param name="pos">世界坐标系下的一个点的位置</param>
    /// <returns>如果在可见范围外返回true，否则返回false</returns>
    public static bool IsWorldPosOutScreen(Vector3 pos)
    {
        //将世界坐标转为屏幕坐标
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        //判断是否在屏幕范围内
        if (screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height)
            return false;
        return true;
    }

    /// <summary>
    /// 判断某一个位置 是否在指定扇形范围内（注意：传入的坐标向量都必须是基于同一个坐标系下的）
    /// </summary>
    /// <param name="pos">扇形中心点位置</param>
    /// <param name="forward">自己的面朝向</param>
    /// <param name="targetPos">目标对象</param>
    /// <param name="radius">半径</param>
    /// <param name="angle">扇形的角度</param>
    /// <returns></returns>
    public static bool IsInSectorRangeXZ(Vector3 pos, Vector3 forward, Vector3 targetPos, float radius, float angle)
    {
        pos.y = 0;
        forward.y = 0;
        targetPos.y = 0;
        //距离 + 角度
        return Vector3.Distance(pos, targetPos) <= radius && Vector3.Angle(forward, targetPos - pos) <= angle / 2f;
    }
    #endregion

    #region 射线检测相关

    /// <summary>
    /// 射线检测 获取一个对象 指定距离 指定层级的
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数（会把碰到的RayCastHit信息传递出去）</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast(Ray ray, Action<RaycastHit> callBack, float maxDistance, int layerMask)
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
            callBack.Invoke(hitInfo);
    }

    /// <summary>
    /// 射线检测 获取一个对象 指定距离 指定层级的
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数（会把碰到的GameObject信息传递出去）</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast(Ray ray, Action<GameObject> callBack, float maxDistance, int layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
            callBack.Invoke(hitInfo.collider.gameObject);
    }

    /// <summary>
    /// 射线检测 获取一个对象 指定距离 指定层级的
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数（会把碰到的对象信息上挂在的指定脚本传递出去）</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCast<T>(Ray ray, Action<T> callBack, float maxDistance, int layerMask)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
            callBack.Invoke(hitInfo.collider.gameObject.GetComponent<T>());
    }

    /// <summary>
    /// 射线检测 获取到多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数（会把碰到的RayCastHit信息传递出去） 每一个对象都会调用一次</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCastAll(Ray ray, Action<RaycastHit> callBack, float maxDistance, int layerMask)
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
            callBack.Invoke(hitInfos[i]);
    }

    /// <summary>
    /// 射线检测 获取到多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数（会把碰到的GameObject信息传递出去） 每一个对象都会调用一次</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCastAll(Ray ray, Action<GameObject> callBack, float maxDistance, int layerMask)
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
            callBack.Invoke(hitInfos[i].collider.gameObject);
    }

    /// <summary>
    /// 射线检测 获取到多个对象 指定距离 指定层级
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数（会把碰到的对象信息上依附的脚本传递出去） 每一个对象都会调用一次</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级筛选</param>
    public static void RayCastAll<T>(Ray ray, Action<T> callBack, float maxDistance, int layerMask)
    {
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
        for (int i = 0; i < hitInfos.Length; i++)
            callBack.Invoke(hitInfos[i].collider.gameObject.GetComponent<T>());
    }
    #endregion

    #region 范围检测相关
    /// <summary>
    /// 进行盒装范围检测
    /// </summary>
    /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
    /// <param name="center">盒装中心点</param>
    /// <param name="rotation">盒子的角度</param>
    /// <param name="halfExtents">长宽高的一半</param>
    /// <param name="layerMask">层级筛选</param>
    /// <param name="callBack">回调函数 </param>
    public static void OverlapBox<T>(Vector3 center, Quaternion rotation, Vector3 halfExtents, int layerMask, Action<T> callBack) where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapBox(center, halfExtents, rotation, layerMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider))
                callBack.Invoke(colliders[i] as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(colliders[i].gameObject as T);
            else
                callBack.Invoke(colliders[i].gameObject.GetComponent<T>());
        }
    }

    /// <summary>
    /// 进行球体范围检测
    /// </summary>
    /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
    /// <param name="center">球体的中心点</param>
    /// <param name="radius">球体的半径</param>
    /// <param name="layerMask">层级筛选</param>
    /// <param name="callBack">回调函数</param>
    public static void OverlapSphere<T>(Vector3 center, float radius, int layerMask, Action<T> callBack) where T:class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask, QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider))
                callBack.Invoke(colliders[i] as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(colliders[i].gameObject as T);
            else
                callBack.Invoke(colliders[i].gameObject.GetComponent<T>());
        }
    }
    #endregion

    #region 方向判断相关
    /// <summary>
    /// 表示两个物体之间的相对方向
    /// </summary>
    public enum RelativeDirection
    {
        None,
        Left,
        Right,
        Front,
        Back
    }

    /// <summary>
    /// 判断两个物体在XZ平面上的相对方向（基于第一个物体的朝向）
    /// </summary>
    /// <param name="srcPos">源物体位置</param>
    /// <param name="srcForward">源物体朝向</param>
    /// <param name="targetPos">目标物体位置</param>
    /// <param name="angleThreshold">方向判断的角度阈值（默认为45度）</param>
    /// <returns>目标物体相对于源物体的方向</returns>
    public static RelativeDirection GetRelativeDirectionXZ(Vector3 srcPos, Vector3 srcForward, Vector3 targetPos, float angleThreshold = 45f)
    {
        // 确保在XZ平面上进行计算
        srcPos.y = 0;
        srcForward.y = 0;
        targetPos.y = 0;

        // 计算目标物体相对于源物体的方向向量
        Vector3 directionToTarget = (targetPos - srcPos).normalized;

        // 计算与前方的夹角
        float forwardAngle = Vector3.Angle(srcForward, directionToTarget);
        if (forwardAngle < angleThreshold)
        {
            return RelativeDirection.Front;
        }

        // 计算与后方的夹角
        float backAngle = Vector3.Angle(-srcForward, directionToTarget);
        if (backAngle < angleThreshold)
        {
            return RelativeDirection.Back;
        }

        // 计算与右方的夹角（使用叉乘确定左右）
        Vector3 rightDir = Vector3.Cross(srcForward, Vector3.up);
        float rightAngle = Vector3.Angle(rightDir, directionToTarget);
        if (rightAngle < angleThreshold)
        {
            return RelativeDirection.Right;
        }

        // 计算与左方的夹角
        Vector3 leftDir = -rightDir;
        float leftAngle = Vector3.Angle(leftDir, directionToTarget);
        if (leftAngle < angleThreshold)
        {
            return RelativeDirection.Left;
        }

        return RelativeDirection.None;
    }

    /// <summary>
    /// 判断两个物体在XY平面上的相对方向（基于第一个物体的朝向）
    /// </summary>
    /// <param name="srcPos">源物体位置</param>
    /// <param name="srcForward">源物体朝向</param>
    /// <param name="targetPos">目标物体位置</param>
    /// <param name="angleThreshold">方向判断的角度阈值（默认为45度）</param>
    /// <returns>目标物体相对于源物体的方向</returns>
    public static RelativeDirection GetRelativeDirectionXY(Vector3 srcPos, Vector3 srcForward, Vector3 targetPos, float angleThreshold = 45f)
    {
        // 确保在XY平面上进行计算
        srcPos.z = 0;
        srcForward.z = 0;
        targetPos.z = 0;

        // 计算目标物体相对于源物体的方向向量
        Vector3 directionToTarget = (targetPos - srcPos).normalized;

        // 计算与前方的夹角
        float forwardAngle = Vector3.Angle(srcForward, directionToTarget);
        if (forwardAngle < angleThreshold)
        {
            return RelativeDirection.Front;
        }

        // 计算与后方的夹角
        float backAngle = Vector3.Angle(-srcForward, directionToTarget);
        if (backAngle < angleThreshold)
        {
            return RelativeDirection.Back;
        }

        // 计算与右方的夹角（使用叉乘确定左右）
        Vector3 rightDir = Vector3.Cross(srcForward, Vector3.back);
        float rightAngle = Vector3.Angle(rightDir, directionToTarget);
        if (rightAngle < angleThreshold)
        {
            return RelativeDirection.Right;
        }

        // 计算与左方的夹角
        Vector3 leftDir = -rightDir;
        float leftAngle = Vector3.Angle(leftDir, directionToTarget);
        if (leftAngle < angleThreshold)
        {
            return RelativeDirection.Left;
        }

        return RelativeDirection.None;
    }

    /// <summary>
    /// 获取两个物体之间所有可能的相对方向（更详细的方向判断）
    /// </summary>
    /// <param name="srcPos">源物体位置</param>
    /// <param name="srcForward">源物体朝向</param>
    /// <param name="targetPos">目标物体位置</param>
    /// <returns>包含所有符合条件的相对方向的列表</returns>
    public static List<RelativeDirection> GetAllRelativeDirectionsXZ(Vector3 srcPos, Vector3 srcForward, Vector3 targetPos)
    {
        List<RelativeDirection> directions = new List<RelativeDirection>();

        // 确保在XZ平面上进行计算
        srcPos.y = 0;
        srcForward.y = 0;
        targetPos.y = 0;

        // 计算目标物体相对于源物体的方向向量
        Vector3 directionToTarget = targetPos - srcPos;
        float distance = directionToTarget.magnitude;
        
        // 如果两个物体位置重合，返回空列表
        if (distance < 0.01f)
        {
            return directions;
        }
        
        directionToTarget.Normalize();

        // 计算参考方向
        Vector3 rightDir = Vector3.Cross(srcForward, Vector3.up);
        Vector3 leftDir = -rightDir;
        Vector3 backDir = -srcForward;

        // 判断各个方向
        if (Vector3.Dot(directionToTarget, srcForward) > 0.707f) // 大约45度
        {
            directions.Add(RelativeDirection.Front);
        }
        
        if (Vector3.Dot(directionToTarget, backDir) > 0.707f)
        {
            directions.Add(RelativeDirection.Back);
        }
        
        if (Vector3.Dot(directionToTarget, rightDir) > 0.707f)
        {
            directions.Add(RelativeDirection.Right);
        }
        
        if (Vector3.Dot(directionToTarget, leftDir) > 0.707f)
        {
            directions.Add(RelativeDirection.Left);
        }

        return directions;
    }
    #endregion

    #region 向量运算相关
    /// <summary>
    /// 将向量限制在指定长度范围内
    /// </summary>
    /// <param name="vector">原始向量</param>
    /// <param name="maxLength">最大长度</param>
    /// <returns>限制后的向量</returns>
    public static Vector3 ClampVector3(Vector3 vector, float maxLength)
    {
        if (vector.sqrMagnitude > maxLength * maxLength)
        {
            return vector.normalized * maxLength;
        }
        return vector;
    }

    /// <summary>
    /// 计算向量在指定平面上的投影
    /// </summary>
    /// <param name="vector">要投影的向量</param>
    /// <param name="normal">平面法线</param>
    /// <returns>投影后的向量</returns>
    public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 normal)
    {
        return vector - Vector3.Project(vector, normal);
    }

    /// <summary>
    /// 计算向量在另一向量上的投影
    /// </summary>
    /// <param name="vector">要投影的向量</param>
    /// <param name="onNormal">投影到的向量</param>
    /// <returns>投影后的向量</returns>
    public static Vector3 ProjectOnVector(Vector3 vector, Vector3 onNormal)
    {
        return Vector3.Project(vector, onNormal);
    }

    /// <summary>
    /// 计算反射向量
    /// </summary>
    /// <param name="vector">入射向量</param>
    /// <param name="normal">法线向量</param>
    /// <returns>反射向量</returns>
    public static Vector3 Reflect(Vector3 vector, Vector3 normal)
    {
        return Vector3.Reflect(vector, normal);
    }

    /// <summary>
    /// 计算两个向量之间的夹角（0到180度）
    /// </summary>
    /// <param name="from">起始向量</param>
    /// <param name="to">目标向量</param>
    /// <returns>夹角（角度值）</returns>
    public static float GetAngle(Vector3 from, Vector3 to)
    {
        return Vector3.Angle(from, to);
    }

    /// <summary>
    /// 通过三个点计算平面法线
    /// </summary>
    /// <param name="pointA">第一个点</param>
    /// <param name="pointB">第二个点</param>
    /// <param name="pointC">第三个点</param>
    /// <returns>归一化的平面法线向量</returns>
    public static Vector3 CalculatePlaneNormal(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        // 计算两个边向量
        Vector3 vectorAB = pointB - pointA;
        Vector3 vectorAC = pointC - pointA;
        
        // 通过叉乘计算法线
        Vector3 normal = Vector3.Cross(vectorAB, vectorAC);
        
        // 归一化法线
        if (normal.sqrMagnitude > 0.0001f)
        {
            normal.Normalize();
        }
        
        return normal;
    }

    /// <summary>
    /// 通过两个向量计算平面法线
    /// </summary>
    /// <param name="vector1">第一个向量</param>
    /// <param name="vector2">第二个向量</param>
    /// <returns>归一化的平面法线向量</returns>
    public static Vector3 CalculatePlaneNormalFromVectors(Vector3 vector1, Vector3 vector2)
    {
        // 通过叉乘计算法线
        Vector3 normal = Vector3.Cross(vector1, vector2);
        
        // 归一化法线
        if (normal.sqrMagnitude > 0.0001f)
        {
            normal.Normalize();
        }
        
        return normal;
    }

    /// <summary>
    /// 判断两个Vector3是否共线且方向相反
    /// </summary>
    /// <param name="vecA">第一个向量</param>
    /// <param name="vecB">第二个向量</param>
    /// <param name="epsilon">精度阈值（应对浮点误差，默认0.0001）</param>
    /// <returns>true=共线且方向相反，false=否则</returns>
    public static bool IsCollinearAndOpposite(Vector3 vecA, Vector3 vecB, float epsilon = 0.0001f)
    {
        // 1. 先排除零向量（零向量无方向，不参与判断）
        if (vecA.sqrMagnitude < epsilon * epsilon || vecB.sqrMagnitude < epsilon * epsilon)
        {
            UnityEngine.Debug.LogWarning("不能传入零向量，零向量无方向属性");
            return false;
        }

        // 2. 验证共线：叉积结果的模长接近0（用平方模长避免开方，效率更高）
        Vector3 crossResult = Vector3.Cross(vecA, vecB);
        bool isCollinear = crossResult.sqrMagnitude < epsilon * epsilon;

        // 3. 验证方向相反：点积结果小于0
        bool isOppositeDirection = Vector3.Dot(vecA, vecB) < 0;

        // 4. 同时满足才返回true
        return isCollinear && isOppositeDirection;
    }
    #endregion

    #region 插值计算
    /// <summary>
    /// 线性插值（Vector3）
    /// </summary>
    /// <param name="from">起始值</param>
    /// <param name="to">目标值</param>
    /// <param name="t">插值因子（0-1）</param>
    /// <returns>插值结果</returns>
    public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
    {
        t = Mathf.Clamp01(t);
        return Vector3.Lerp(from, to, t);
    }

    /// <summary>
    /// 球形插值（Vector3），适用于平滑旋转
    /// </summary>
    /// <param name="from">起始值</param>
    /// <param name="to">目标值</param>
    /// <param name="t">插值因子（0-1）</param>
    /// <returns>插值结果</returns>
    public static Vector3 Slerp(Vector3 from, Vector3 to, float t)
    {
        t = Mathf.Clamp01(t);
        return Vector3.Slerp(from, to, t);
    }

    /// <summary>
    /// 平滑插值（Vector3），具有缓动效果
    /// </summary>
    /// <param name="from">起始值</param>
    /// <param name="to">目标值</param>
    /// <param name="t">插值因子（0-1）</param>
    /// <returns>插值结果</returns>
    public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
    {
        return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime);
    }

    /// <summary>
    /// 线性插值（float）
    /// </summary>
    /// <param name="from">起始值</param>
    /// <param name="to">目标值</param>
    /// <param name="t">插值因子（0-1）</param>
    /// <returns>插值结果</returns>
    public static float LerpFloat(float from, float to, float t)
    {
        t = Mathf.Clamp01(t);
        return from + (to - from) * t;
    }

    /// <summary>
    /// 平滑插值（float），具有缓动效果
    /// </summary>
    /// <param name="current">当前值</param>
    /// <param name="target">目标值</param>
    /// <param name="currentVelocity">当前速度（引用参数）</param>
    /// <param name="smoothTime">平滑时间</param>
    /// <returns>插值结果</returns>
    public static float SmoothDampFloat(float current, float target, ref float currentVelocity, float smoothTime)
    {
        return Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime);
    }
    #endregion

    #region 随机数生成
    /// <summary>
    /// 生成指定范围内的随机整数
    /// </summary>
    /// <param name="min">最小值（包含）</param>
    /// <param name="max">最大值（包含）</param>
    /// <returns>随机整数</returns>
    public static int RandomRangeInt(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    /// <summary>
    /// 生成指定范围内的随机浮点数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机浮点数</returns>
    public static float RandomRangeFloat(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// 生成随机向量
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机向量</returns>
    public static Vector3 RandomVector3(float min, float max)
    {
        return new Vector3(
            UnityEngine.Random.Range(min, max),
            UnityEngine.Random.Range(min, max),
            UnityEngine.Random.Range(min, max)
        );
    }

    /// <summary>
    /// 生成随机单位向量
    /// </summary>
    /// <returns>随机单位向量</returns>
    public static Vector3 RandomUnitVector3()
    {
        return new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
        ).normalized;
    }
    #endregion

    #region 矩形和边界框计算
    /// <summary>
    /// 创建包含多个点的矩形边界
    /// </summary>
    /// <param name="points">点集合</param>
    /// <returns>包含所有点的矩形</returns>
    public static Bounds CreateBoundsFromPoints(IEnumerable<Vector3> points)
    {
        Bounds bounds = new Bounds();
        bool first = true;
        foreach (Vector3 point in points)
        {
            if (first)
            {
                bounds = new Bounds(point, Vector3.zero);
                first = false;
            }
            else
            {
                bounds.Encapsulate(point);
            }
        }
        return bounds;
    }

    /// <summary>
    /// 检查点是否在矩形内
    /// </summary>
    /// <param name="point">要检查的点</param>
    /// <param name="rect">矩形</param>
    /// <returns>是否在矩形内</returns>
    public static bool IsPointInRect(Vector2 point, Rect rect)
    {
        return rect.Contains(point);
    }

    /// <summary>
    /// 检查点是否在矩形内（Vector3版本）
    /// </summary>
    /// <param name="point">要检查的点</param>
    /// <param name="rect">矩形</param>
    /// <returns>是否在矩形内</returns>
    public static bool IsPointInRect(Vector3 point, Rect rect)
    {
        return rect.Contains(new Vector2(point.x, point.y));
    }

    /// <summary>
    /// 计算两个矩形的交集
    /// </summary>
    /// <param name="a">第一个矩形</param>
    /// <param name="b">第二个矩形</param>
    /// <returns>相交的矩形，如果不相交则返回空矩形</returns>
    public static Rect GetRectIntersection(Rect a, Rect b)
    {
        if (!a.Overlaps(b))
        {
            return Rect.zero;
        }

        float x = Mathf.Max(a.xMin, b.xMin);
        float y = Mathf.Max(a.yMin, b.yMin);
        float width = Mathf.Min(a.xMax, b.xMax) - x;
        float height = Mathf.Min(a.yMax, b.yMax) - y;

        return new Rect(x, y, width, height);
    }
    #endregion

    #region 数学常量
    /// <summary>
    /// 黄金比例 (φ = (1+√5)/2 ≈ 1.618)
    /// </summary>
    public const float GoldenRatio = 1.61803398875f;

    /// <summary>
    /// 欧拉数 (e ≈ 2.718)
    /// </summary>
    public const float EulerNumber = 2.71828182846f;

    /// <summary>
    /// 平方根2 (√2 ≈ 1.414)
    /// </summary>
    public const float Sqrt2 = 1.41421356237f;

    /// <summary>
    /// 平方根3 (√3 ≈ 1.732)
    /// </summary>
    public const float Sqrt3 = 1.73205080757f;
    #endregion

    #region 高级角度计算
    /// <summary>
    /// 将角度限制在-180到180度之间
    /// </summary>
    /// <param name="angle">原始角度</param>
    /// <returns>限制后的角度</returns>
    public static float ClampAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f)
        {
            angle -= 360f;
        }
        else if (angle < -180f)
        {
            angle += 360f;
        }
        return angle;
    }

    /// <summary>
    /// 计算最短旋转方向的角度差
    /// </summary>
    /// <param name="fromAngle">起始角度</param>
    /// <param name="toAngle">目标角度</param>
    /// <returns>最短旋转方向的角度差（-180到180之间）</returns>
    public static float ShortestAngleDifference(float fromAngle, float toAngle)
    {
        float difference = ClampAngle(toAngle - fromAngle);
        return difference;
    }

    /// <summary>
    /// 将向量转换为角度（XZ平面）
    /// </summary>
    /// <param name="direction">方向向量</param>
    /// <returns>角度值（度）</returns>
    public static float VectorToAngleXZ(Vector3 direction)
    {
        direction.y = 0;
        if (direction.sqrMagnitude < 0.001f)
            return 0;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        return angle;
    }

    /// <summary>
    /// 将角度转换为向量（XZ平面）
    /// </summary>
    /// <param name="angleDeg">角度值（度）</param>
    /// <returns>方向向量</returns>
    public static Vector3 AngleToVectorXZ(float angleDeg)
    {
        float angleRad = angleDeg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }
    #endregion
}
