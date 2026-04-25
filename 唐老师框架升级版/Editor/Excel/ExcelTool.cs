
// 引入Excel相关库
using Excel;
// 引入系统命名空间
using System;
using System.Data;
using System.IO;
using System.Text;
// 引入Unity编辑器相关命名空间
using UnityEditor;
using UnityEngine;
using LitJson;
using Newtonsoft.Json;
using System.Collections.Generic;  // 添加命名空间引用



// Excel工具类
public class ExcelTool
{
    // 配置实例
    private static PathConfig config;
    
    // 获取配置实例
    private static PathConfig Config
    {
        get
        {
            if (config == null)
            {
                config = PathConfig.Load();
            }
            return config;
        }
    }
    
    // Excel文件夹路径
    public static string EXCEL_PATH { get { return Config.excelPath; } }
    // 数据类输出路径
    public static string DATA_CLASS_PATH { get { return Config.dataClassPath; } }
    // 数据容器输出路径
    public static string DATA_CONTAINER_PATH { get { return Config.dataContainerPath; } }
    // 二进制文件输出路径
    public static string BINARY_OUTPUT_PATH { get { return Config.binaryOutputPath; } }
    // Excel数据起始行索引
    public static int BEGIN_INDEX = 4;
    // 添加 JSON 输出路径常量
    public static string JSON_OUTPUT_PATH { get { return Config.jsonOutputPath; } }


    // Unity菜单项，生成Excel相关数据
    [MenuItem("唐老师工具/读取Excel表数据/生成2进制数据")]
    private static void GenerateExcelBinaryInfo()
    {
        // 创建Excel目录（如果不存在）
        DirectoryInfo dInfo = Directory.CreateDirectory(EXCEL_PATH);
        // 获取目录下所有文件
        FileInfo[] files = dInfo.GetFiles();

        // 遍历所有文件
        foreach (FileInfo file in files)
        {
            // 只处理Excel文件
            if (file.Extension != ".xlsx" && file.Extension != ".xls")
                continue;

            DataTableCollection tableCollection;
            // 打开Excel文件流
            using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
            {
                // 创建Excel读取器
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                // 获取所有表格集合
                tableCollection = excelReader.AsDataSet().Tables;
            }

            // 遍历每个表格
            foreach (DataTable table in tableCollection)
            {
                // 生成数据类
                GenerateExcelDataClass(table);
                // 生成数据容器类
                GenerateExcelContainer(table);
                // 生成二进制数据文件
                GenerateExcelBinary(table);
                
                
            }
        }
        // 刷新Unity资源数据库
        AssetDatabase.Refresh();
    }


    [MenuItem("唐老师工具/读取Excel表数据/生成Json数据(LitJson)")]
    private static void GenerateExcelJsonInfo()
    {
        GenerateExcelJsonInfo(false);
    }

    [MenuItem("唐老师工具/读取Excel表数据/生成Json数据(Newtonsoft)")]
    private static void GenerateExcelNewtonsoftJsonInfo()
    {
        GenerateExcelJsonInfo(true);
    }

    private static void GenerateExcelJsonInfo(bool useNewtonsoft)
    {
        // 创建Excel目录（如果不存在）
        DirectoryInfo dInfo = Directory.CreateDirectory(EXCEL_PATH);
        // 获取目录下所有文件
        FileInfo[] files = dInfo.GetFiles();

        // 遍历所有文件
        foreach (FileInfo file in files)
        {
            // 只处理Excel文件
            if (file.Extension != ".xlsx" && file.Extension != ".xls")
                continue;

            DataTableCollection tableCollection;
            // 打开Excel文件流
            using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
            {
                // 创建Excel读取器
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                // 获取所有表格集合
                tableCollection = excelReader.AsDataSet().Tables;
            }

            // 遍历每个表格
            foreach (DataTable table in tableCollection)
            {
                // 生成数据类
                GenerateExcelDataClass(table);
                // 生成数据容器类
                GenerateExcelContainer(table);
                // 生成json数据文件
                GenerateExcelJson(table,useNewtonsoft);

            }
        }
        // 刷新Unity资源数据库
        AssetDatabase.Refresh();
    }

    // 生成Excel数据类
    private static void GenerateExcelDataClass(DataTable table)
    {
        // 如果数据类目录不存在则创建
        if (!Directory.Exists(DATA_CLASS_PATH))
            Directory.CreateDirectory(DATA_CLASS_PATH);
        // 清空数据类目录中的所有文件
        foreach (string file in Directory.GetFiles(DATA_CLASS_PATH))
        {
            File.Delete(file);
        }

        // 用于拼接类代码
        StringBuilder sb = new StringBuilder();
        // 添加序列化特性
        sb.AppendLine("[System.Serializable]");
        // 添加类声明
        sb.AppendLine($"public class {table.TableName}");
        sb.AppendLine("{");

        // 第一行：字段名
        DataRow rowName = table.Rows[0];
        // 第二行：字段类型
        DataRow rowType = table.Rows[1];

        // 遍历所有列，生成字段声明
        for (int i = 0; i < table.Columns.Count; i++)
        {
            // 跳过空字段名
            if (string.IsNullOrEmpty(rowName[i].ToString()))
                continue;

            // 默认类型为int
            string type = string.IsNullOrEmpty(rowType[i].ToString()) ? "int" : rowType[i].ToString();
            // 添加字段声明
            sb.AppendLine($"    public {type} {rowName[i]};");
        }

        // 类结束
        sb.AppendLine("}");

        // 写入到文件
        string filePath = Path.Combine(DATA_CLASS_PATH, table.TableName + ".cs");
        File.WriteAllText(filePath, sb.ToString());
        Debug.Log($"生成数据类文件: {filePath}");
    }


    // 生成Excel数据容器类
    private static void GenerateExcelContainer(DataTable table)
    {
        // 如果容器目录不存在则创建
        if (!Directory.Exists(DATA_CONTAINER_PATH))
            Directory.CreateDirectory(DATA_CONTAINER_PATH);
        // 清空容器目录中的所有文件
        foreach (string file in Directory.GetFiles(DATA_CONTAINER_PATH))
        {
            File.Delete(file);
        }

        // 获取主键索引
        int keyIndex = GetKeyIndex(table);
        // 获取主键类型
        string keyType = GetVariableTypeRow(table)[keyIndex].ToString();
        keyType = string.IsNullOrEmpty(keyType) ? "int" : keyType;

        // 拼接容器类代码
        string str = $@"using System.Collections.Generic;

public class {table.TableName}Container
{{
    public Dictionary<{keyType}, {table.TableName}> dataDic = new Dictionary<{keyType}, {table.TableName}>();
}}";

        // 写入到文件
        string filePath = Path.Combine(DATA_CONTAINER_PATH, table.TableName + "Container.cs");
        File.WriteAllText(filePath, str);
        Debug.Log($"生成数据容器类文件: {filePath}");
    }


    // 生成Excel二进制数据文件
    private static void GenerateExcelBinary(DataTable table)
    {
        // 如果二进制目录不存在则创建
        if (!Directory.Exists(BINARY_OUTPUT_PATH))
            Directory.CreateDirectory(BINARY_OUTPUT_PATH);
        // 清空二进制目录中的所有文件
        foreach (string file in Directory.GetFiles(BINARY_OUTPUT_PATH))
        {
            File.Delete(file);
        }

        // 拼接输出文件路径
        string filePath = Path.Combine(BINARY_OUTPUT_PATH, table.TableName + ".tang");
        // 创建文件流
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            // 计算有效数据行数
            int rowCount = table.Rows.Count - BEGIN_INDEX;
            // 写入行数
            fs.Write(BitConverter.GetBytes(rowCount), 0, 4);

            // 获取主键索引和主键名
            int keyIndex = GetKeyIndex(table);
            string keyName = GetVariableNameRow(table)[keyIndex].ToString();
            // 主键名转字节
            byte[] keyNameBytes = Encoding.UTF8.GetBytes(keyName);
            // 写入主键名长度
            fs.Write(BitConverter.GetBytes(keyNameBytes.Length), 0, 4);
            // 写入主键名内容
            fs.Write(keyNameBytes, 0, keyNameBytes.Length);

            // 获取类型行和字段名行
            DataRow rowType = GetVariableTypeRow(table);
            DataRow rowName = GetVariableNameRow(table);
            // 有效行计数
            int validRowCount = 0;

            // 遍历所有数据行
            for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                object keyValue = row[keyIndex];

                // 跳过主键为空的行
                if (keyValue == null || string.IsNullOrEmpty(keyValue.ToString()))
                {
                    Debug.LogError($"表 {table.TableName} 第{i + 1}行键值为空，已跳过");
                    continue;
                }

                // 遍历所有字段
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    // 跳过空字段名
                    if (string.IsNullOrEmpty(rowName[j].ToString()))
                        continue;

                    try
                    {
                        // 写入字段值
                        WriteFieldValue(fs, row[j], rowType[j].ToString());
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"表 {table.TableName} 第{i + 1}行第{j + 1}列数据写入失败: {ex.Message}");
                    }
                }
                // 有效行+1
                validRowCount++;
            }

            // 如果有效行数和原始行数不一致，修正文件头
            if (validRowCount != rowCount)
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(BitConverter.GetBytes(validRowCount), 0, 4);
                Debug.LogWarning($"表 {table.TableName} 行数修正: {rowCount} -> {validRowCount}");
            }
        }
    }


    // 写入字段值到二进制文件
    private static void WriteFieldValue(FileStream fs, object value, string fieldType)
    {
        // 空值处理
        if (value == null) value = "";

        // 根据字段类型写入不同格式
        switch (fieldType.ToLower())
        {
            case "int":
            case "":
                // 写入int类型
                int intValue;
                if (!int.TryParse(value.ToString(), out intValue))
                    intValue = 0;
                fs.Write(BitConverter.GetBytes(intValue), 0, 4);
                break;

            case "float":
                // 写入float类型
                float floatValue;
                if (!float.TryParse(value.ToString(), out floatValue))
                    floatValue = 0f;
                fs.Write(BitConverter.GetBytes(floatValue), 0, 4);
                break;

            case "bool":
                // 写入bool类型
                bool boolValue = IsTrueValue(value.ToString());
                fs.Write(BitConverter.GetBytes(boolValue), 0, 1);
                break;

            case "string":
                // 写入string类型（先写长度再写内容）
                byte[] strBytes = Encoding.UTF8.GetBytes(value.ToString());
                fs.Write(BitConverter.GetBytes(strBytes.Length), 0, 4);
                fs.Write(strBytes, 0, strBytes.Length);
                break;

            default:
                // 不支持的类型抛异常
                throw new ArgumentException($"不支持的类型: {fieldType}");
        }
    }


    // 判断字符串是否为true值
    private static bool IsTrueValue(string value)
    {
        value = value.Trim().ToLower();
        return value == "true" || value == "1" || value == "是" || value == "yes";
    }

    // 获取字段名行
    private static DataRow GetVariableNameRow(DataTable table)
    {
        return table.Rows[0];
    }

    // 获取字段类型行
    private static DataRow GetVariableTypeRow(DataTable table)
    {
        return table.Rows[1];
    }

    // 获取主键索引
    private static int GetKeyIndex(DataTable table)
    {
        DataRow keyMarkRow = table.Rows[2];
        for (int i = 0; i < table.Columns.Count; i++)
        {
            // 如果该列标记为key则返回索引
            if (keyMarkRow[i].ToString().Trim().ToLower() == "key")
                return i;
        }
        // 未找到主键则默认第一列
        Debug.LogError($"表 {table.TableName} 未找到主键列，默认使用第一列");
        return 0;
    }

    // 新增：将 Excel 数据转换为 JSON 文件
    private static void GenerateExcelJson(DataTable table, bool useNewtonsoft = true)
    {
        // 创建 JSON 目录（如果不存在）
        if (!Directory.Exists(JSON_OUTPUT_PATH))
            Directory.CreateDirectory(JSON_OUTPUT_PATH);
        // 清空JSON目录中的所有文件
        foreach (string file in Directory.GetFiles(JSON_OUTPUT_PATH))
        {
            File.Delete(file);
        }

        // 获取主键索引和字段信息
        int keyIndex = GetKeyIndex(table);
        DataRow rowType = GetVariableTypeRow(table);
        DataRow rowName = GetVariableNameRow(table);
        
        // 创建存储所有数据的字典（主键 -> 行数据）
        Dictionary<object, Dictionary<string, object>> dataDict = new Dictionary<object, Dictionary<string, object>>();
        int validRowCount = 0;

        // 遍历所有数据行
        for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
        {
            DataRow row = table.Rows[i];
            object keyValue = row[keyIndex];

            // 跳过主键为空的行
            if (keyValue == null || string.IsNullOrEmpty(keyValue.ToString()))
            {
                Debug.LogError($"表 {table.TableName} 第{i + 1}行键值为空，已跳过");
                continue;
            }

            // 创建当前行的数据字典
            Dictionary<string, object> rowData = new Dictionary<string, object>();
            for (int j = 0; j < table.Columns.Count; j++)
            {
                // 跳过空字段名
                if (string.IsNullOrEmpty(rowName[j].ToString()))
                    continue;

                try
                {
                    // 获取字段名和类型
                    string fieldName = rowName[j].ToString();
                    string fieldType = rowType[j].ToString().ToLower();
                    object value = row[j];

                    // 转换值为正确的类型
                    value = ConvertValue(value, fieldType);
                    rowData[fieldName] = value;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"表 {table.TableName} 第{i + 1}行第{j + 1}列数据转换失败: {ex.Message}");
                }
            }

            dataDict[keyValue] = rowData;
            validRowCount++;
        }

        // 序列化并写入 JSON 文件
        try
        {
            string filePath = Path.Combine(JSON_OUTPUT_PATH, table.TableName + ".json");
            
            string jsonText;
            if (useNewtonsoft)
            {
                // 使用 Newtonsoft.Json 序列化，启用格式化输出
                jsonText = JsonConvert.SerializeObject(dataDict, Formatting.Indented);
                Debug.Log($"成功使用 Newtonsoft.Json 生成 JSON 文件: {filePath} (有效行数: {validRowCount})");
            }
            else
            {
                // 使用 LitJson 序列化，启用格式化输出
                LitJson.JsonWriter writer = new LitJson.JsonWriter();
                writer.PrettyPrint = true;  // 格式化 JSON 便于阅读
                JsonMapper.ToJson(dataDict, writer);
                jsonText = writer.ToString();
                Debug.Log($"成功使用 LitJson 生成 JSON 文件: {filePath} (有效行数: {validRowCount})");
            }
            
            File.WriteAllText(filePath, jsonText);
        }
        catch (Exception ex)
        {
            Debug.LogError($"生成 JSON 文件失败: {ex.Message}");
        }
    }

    // 新增：类型转换辅助方法（仿照 WriteFieldValue 实现）
    private static object ConvertValue(object value, string fieldType)
    {
        if (value == null) value = "";
        string strValue = value.ToString().Trim();

        switch (fieldType)
        {
            case "int":
            case "":  // 默认类型为 int
                if (int.TryParse(strValue, out int intVal))
                    return intVal;
                return 0;

            case "float":
                if (float.TryParse(strValue, out float floatVal))
                    return floatVal;
                return 0f;

            case "bool":
                return IsTrueValue(strValue);

            case "string":
                return strValue;

            default:
                throw new ArgumentException($"不支持的类型: {fieldType}");
        }
    }
}