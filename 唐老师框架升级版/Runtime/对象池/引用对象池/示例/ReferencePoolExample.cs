using UnityEngine;

/// <summary>
/// 引用对象池使用示例
/// 展示如何创建和使用可被引用对象池管理的类
/// </summary>
public class ReferencePoolExample : MonoBehaviour
{
    private void Start()
    {
        // 初始化引用对象池管理器
        ReferencePoolMgr.Instance.Initialize();
        
        // 示例1: 基本使用方法
        ExampleBasicUsage();
        
        // 示例2: 设置最大容量
        ExampleSetMaxCapacity();
        
        // 示例3: 使用命名空间区分同类型对象
        ExampleUseNameSpace();
        
        // 示例4: 清理对象池
        ExampleClearPool();
    }
    
    /// <summary>
    /// 示例1: 基本使用方法
    /// </summary>
    private void ExampleBasicUsage()
    {
        LogSystem.Info("=== 示例1: 基本使用方法 ===");
        
        // 从对象池获取对象
        TestObject obj1 = ReferencePoolMgr.Instance.Get<TestObject>();
        obj1.SetData(1, "Test Object 1");
        obj1.DoSomething();
        
        // 从对象池获取另一个对象
        TestObject obj2 = ReferencePoolMgr.Instance.Get<TestObject>();
        obj2.SetData(2, "Test Object 2");
        obj2.DoSomething();
        
        // 归还对象到对象池
        ReferencePoolMgr.Instance.Return(obj1);
        ReferencePoolMgr.Instance.Return(obj2);
        
        // 再次获取对象，应该复用之前归还的对象
        TestObject obj3 = ReferencePoolMgr.Instance.Get<TestObject>();
        obj3.SetData(3, "Test Object 3");
        obj3.DoSomething();
        
        // 归还对象
        ReferencePoolMgr.Instance.Return(obj3);
        
        LogSystem.Info($"对象池大小: {ReferencePoolMgr.Instance.GetPoolSize<TestObject>()}");
    }
    
    /// <summary>
    /// 示例2: 设置最大容量
    /// </summary>
    private void ExampleSetMaxCapacity()
    {
        LogSystem.Info("\n=== 示例2: 设置最大容量 ===");
        
        // 设置最大容量为2
        ReferencePoolMgr.Instance.SetMaxCapacity<TestObject>(2, "Limited");
        
        // 获取多个对象
        TestObject obj1 = ReferencePoolMgr.Instance.Get<TestObject>("Limited");
        obj1.SetData(1, "Limited Object 1");
        
        TestObject obj2 = ReferencePoolMgr.Instance.Get<TestObject>("Limited");
        obj2.SetData(2, "Limited Object 2");
        
        TestObject obj3 = ReferencePoolMgr.Instance.Get<TestObject>("Limited");
        obj3.SetData(3, "Limited Object 3");
        
        // 归还所有对象
        ReferencePoolMgr.Instance.Return(obj1, "Limited");
        ReferencePoolMgr.Instance.Return(obj2, "Limited");
        ReferencePoolMgr.Instance.Return(obj3, "Limited");
        
        // 由于设置了最大容量为2，第三个对象应该被丢弃
        LogSystem.Info($"限制容量的对象池大小: {ReferencePoolMgr.Instance.GetPoolSize<TestObject>("Limited")}");
    }
    
    /// <summary>
    /// 示例3: 使用命名空间区分同类型对象
    /// </summary>
    private void ExampleUseNameSpace()
    {
        LogSystem.Info("\n=== 示例3: 使用命名空间区分同类型对象 ===");
        
        // 从不同命名空间获取对象
        TestObject uiObj = ReferencePoolMgr.Instance.Get<TestObject>("UI");
        uiObj.SetData(1, "UI Object");
        
        TestObject gameObj = ReferencePoolMgr.Instance.Get<TestObject>("Game");
        gameObj.SetData(2, "Game Object");
        
        // 归还对象
        ReferencePoolMgr.Instance.Return(uiObj, "UI");
        ReferencePoolMgr.Instance.Return(gameObj, "Game");
        
        // 检查不同命名空间的对象池大小
        LogSystem.Info($"UI命名空间对象池大小: {ReferencePoolMgr.Instance.GetPoolSize<TestObject>("UI")}");
        LogSystem.Info($"Game命名空间对象池大小: {ReferencePoolMgr.Instance.GetPoolSize<TestObject>("Game")}");
    }
    
    /// <summary>
    /// 示例4: 清理对象池
    /// </summary>
    private void ExampleClearPool()
    {
        LogSystem.Info("\n=== 示例4: 清理对象池 ===");
        
        // 清理特定类型的对象池
        ReferencePoolMgr.Instance.ClearPool<TestObject>();
        LogSystem.Info($"清理后对象池大小: {ReferencePoolMgr.Instance.GetPoolSize<TestObject>()}");
        
        // 清理所有对象池
        ReferencePoolMgr.Instance.ClearAllPools();
        LogSystem.Info("清理所有对象池完成");
    }
}

/// <summary>
/// 测试对象类
/// 展示如何实现可被引用对象池管理的类
/// </summary>
public class TestObject : IReferencePoolObject
{
    // 测试数据
    public int Id { get; private set; }
    public string Name { get; private set; }
    public float CreateTime { get; private set; }
    
    /// <summary>
    /// 设置对象数据
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="name">名称</param>
    public void SetData(int id, string name)
    {
        Id = id;
        Name = name;
    }
    
    /// <summary>
    /// 执行一些操作
    /// </summary>
    public void DoSomething()
    {
        LogSystem.Info($"TestObject [{Id}] - {Name} - 创建时间: {CreateTime}");
    }
    
    /// <summary>
    /// 对象被获取时调用
    /// </summary>
    public void OnGet()
    {
        // 记录对象被获取的时间
        CreateTime = Time.time;
        LogSystem.Info($"TestObject被获取 - 创建时间: {CreateTime}");
    }
    
    /// <summary>
    /// 对象被归还时调用
    /// </summary>
    public void OnReturn()
    {
        // 重置对象状态
        Id = 0;
        Name = string.Empty;
        LogSystem.Info("TestObject被归还，状态已重置");
    }
}

/// <summary>
/// 另一个测试对象类
/// 展示不同类型的对象如何被引用对象池管理
/// </summary>
public class AnotherTestObject : IReferencePoolObject
{
    public string Data { get; private set; }
    public int Counter { get; private set; }
    
    public void SetData(string data)
    {
        Data = data;
        Counter = 0;
    }
    
    public void IncrementCounter()
    {
        Counter++;
        LogSystem.Info($"AnotherTestObject - Data: {Data}, Counter: {Counter}");
    }
    
    public void OnGet()
    {
        LogSystem.Info("AnotherTestObject被获取");
    }
    
    public void OnReturn()
    {
        Data = string.Empty;
        Counter = 0;
        LogSystem.Info("AnotherTestObject被归还，状态已重置");
    }
}
