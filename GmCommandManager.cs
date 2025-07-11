
#if DebugMod
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HotFix.Module.Hall.Dialog.Interface;
using HotFix.UtilTool;
using Newtonsoft.Json;
using UnityEngine;



public class GmCommandManager
{
    public List<GmCommand> commandList;
    public const int maxCommand = 25; // 最大缓存命令
    public GmCommandManager()
    {
        commandList = new();
    }
    public void Init()
    {

        var gmPath = CommonUtils.GetLinuxPath(Application.persistentDataPath + "/gmfunc.json");
        if (System.IO.File.Exists(gmPath))
        {
            var json = System.IO.File.ReadAllText(gmPath);
            commandList = JsonConvert.DeserializeObject<List<GmCommand>>(json);
        }

    }

    bool ParamsEquals(string[] a, string[] b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null ^ b == null) return false; //如果只有一个是空 另外一个必然不为空 那么返回false
        return a.SequenceEqual(b); // 只有a、b都不为null时才会执行到这里
    }

    bool StringEquires(string a, string b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null ^ b == null) return false; //如果只有一个是空 另外一个必然不为空 那么返回false
        return a == b; // 只有a、b都不为null时才会执行到这里
    }

    bool Contains(GmGui.MethodInfoEx method, string buttonName, String[] ps, string tag)
    {
        return commandList.Any(c =>
       c.methodName == method.methodInfo.Name &&
       c.buttonName == buttonName &&
       StringEquires(c.tag,tag) &&
       ParamsEquals(c.method_params, ps));
    }
    public void CreateCommand(GmGui.MethodInfoEx method, string buttonName, String[] ps, string tag)
    {

        if (!Contains(method, buttonName, ps, tag))
        {
            var command = new GmCommand();
            command.Method = method;
            command.methodName = method.methodInfo.Name;
            command.buttonName = buttonName;
            command.method_params = ps;
            command.tag = tag ?? "";
            if (commandList.Count > maxCommand)
            {
                commandList.RemoveAt(0);
            }
            commandList.Add(command);
        }

    }

    public void CreateDir(string path)
    {
        if (File.Exists(path))
        {
            return;
        }
        if (Directory.Exists(path))
        {
            return;
        }

        var father = Directory.GetParent(path);

        while (!father.Exists)
        {
            Directory.CreateDirectory(father.FullName);
            father = Directory.GetParent(father.FullName);
        }
    }
    public void Clear()
    {
        commandList?.Clear();
        SaveCommand();
    }
    /// <summary>
    /// 存档命令 
    /// </summary>
    public void SaveCommand()
    {
        var gmPath = CommonUtils.GetLinuxPath(Application.persistentDataPath + "/gmfunc.json");
        CreateDir(gmPath);
        if (commandList == null || commandList.Count == 0)
        {
            File.Delete(gmPath);
            UIDialog.ShowTextTip("clear Archive sucess!");
            return;
        }

        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented, // 格式化输出，使生成的 JSON 有缩进

        };
        var json = JsonConvert.SerializeObject(commandList, settings);

        File.WriteAllText(gmPath, json);
    }

}
public class GmCommand : IEquatable<GmCommand>
{
    [JsonIgnore]
    private GmGui.MethodInfoEx method;
    [JsonIgnore]
    public GmGui.MethodInfoEx Method
    {
        get
        {
            if (method == null)
            {
                method = new GmGui.MethodInfoEx();
                method.isLua = false;
                method.desc = buttonName;
                method.methodInfo = GetGmMethodInfo(methodName);
            }
            return method;
        }
        set
        {
            method = value;
        }

    }
    public string methodName;
    public string buttonName;
    public string[] method_params;
    public string tag = "";


    public MethodInfo GetGmMethodInfo(string funcName)
    {
        var methodInfo = typeof(GMFunc).GetMethod(funcName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return methodInfo;
    }

    public override bool Equals(object obj)
    {

        typeof(GMFunc).GetMethod("", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        return Equals(obj as GmCommand);
    }

    public bool Equals(GmCommand other)
    {
        if (other == null)
            return false;



        // 比较buttonName

        if (!string.Equals(this.methodName, other.methodName, StringComparison.Ordinal))
            return false;

        if (!string.Equals(this.buttonName, other.buttonName, StringComparison.Ordinal))
            return false;

        if (!string.Equals(this.tag, other.tag, StringComparison.Ordinal))
            return false;

        // 比较参数数组
        if ((this.method_params == null) != (other.method_params == null))
            return false;
        if (this.method_params != null && other.method_params != null)
        {
            if (this.method_params.Length != other.method_params.Length)
                return false;
            for (int i = 0; i < this.method_params.Length; i++)
            {
                if (!string.Equals(this.method_params[i], other.method_params[i], StringComparison.Ordinal))
                    return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + (methodName != null ? methodName.GetHashCode() : 0);
        hash = hash * 23 + (buttonName != null ? buttonName.GetHashCode() : 0);
        if (method_params != null)
        {
            foreach (var s in method_params)
            {
                hash = hash * 23 + (s != null ? s.GetHashCode() : 0);
            }
        }
        return hash;
    }
    public static bool operator ==(GmCommand a, GmCommand b)
    {
        if (object.ReferenceEquals(a, b))
            return true;
        if (a is null || b is null)
            return false;
        return a.Equals(b);
    }

    public static bool operator !=(GmCommand a, GmCommand b)
    {
        return !(a == b);
    }

    public GmCommand()
    {

    }
}
#endif