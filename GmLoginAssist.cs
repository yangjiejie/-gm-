#if DebugMod && UNITY_STANDALONE_WIN
using System;

using System.Collections.Generic;
using System.IO;

using HotFix.Manager;
using HotFix.Module.Login.Interface;
using Launcher.Debug;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Runtime.Core.Utils;
using UnityEngine;
using static Launcher.Debug.GameDebugLauncher;

public class GmLoginAssist : SingletonMono<GmLoginAssist>
{

    public string jsonInput = "{\"username\": \"gm01\", \"password\": \"\", \"remember\": true, \"level\": 99}";
    private JObject parsedData;
    private Dictionary<string, object> fieldValues = new Dictionary<string, object>();
    private Vector2 scroll;
    public bool isShow { get; set; } = false;
    // 样式定义
    private GUIStyle labelStyle;
    private GUIStyle textFieldStyle;
    private GUIStyle buttonStyle;
    private GUIStyle toggleStyle;
    private GUIStyle boxStyle;
    private GUIStyle titleStyle;

    private bool inited = false;

    private int currentServerSelect = -1; // 当前服务器选择 
    public void Show()
    {
        AppConst.HotFixConfig.autoLogin = false;
        isShow = true;
    }
    public void Hide()
    {
        isShow = false;

    }
    void Init()
    {
        if (inited) return;
        inited = true;
        InitStyles();
        TryParseJson();
    }

    void InitStyles()
    {
        // 标题样式
        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24 * 2,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        // 普通 label
        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18 * 2
        };

        // 输入框
        textFieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 18 * 2,
            fixedHeight = 40 * 2
        };

        // 按钮
        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 18 * 2,
            fixedHeight = 40 * 2,
            alignment = TextAnchor.MiddleCenter
        };

        // Toggle
        toggleStyle = new GUIStyle(GUI.skin.toggle)
        {
            fontSize = 18 * 2
        };

        // Box
        boxStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 20 * 2,
            alignment = TextAnchor.UpperCenter
        };
    }
    TestConfigDecorate testConfigDecorate;
    void TryParseJson()
    {
        try
        {
            var path = Environment.CurrentDirectory + "/PCTest.json";
            if (File.Exists(path))
            {
                testConfigDecorate = FileUtils.loadObjectFromJsonFile<TestConfigDecorate>(path);
                if(currentServerSelect == -1)
                {
                    currentServerSelect = testConfigDecorate.index;
                }
                else
                {
                    testConfigDecorate.index = currentServerSelect;
                }
                
                var config = testConfigDecorate.serverList[testConfigDecorate.index];
                jsonInput = JsonConvert.SerializeObject(config);
                
            }
            
            parsedData = JObject.Parse(jsonInput);
            fieldValues.Clear();
            foreach (var pair in parsedData)
            {
                fieldValues[pair.Key] = pair.Value.Type switch
                {
                    JTokenType.Integer => pair.Value.ToObject<int>(),
                    JTokenType.Float => pair.Value.ToObject<float>(),
                    JTokenType.Boolean => pair.Value.ToObject<bool>(),
                    JTokenType.String => pair.Value.ToObject<string>(),
                    _ => null
                };
            }
            fieldValues["ServerType"] = currentServerSelect;
        }
        catch
        {
            Debug.LogError("JSON 解析失败！");
        }
    }

    void OnGUI()
    {
        if (!isShow) return;
        Init();
        float width = Screen.width * 0.8f;
        float height = Screen.height * 0.6f;
        float x = (Screen.width - width) / 2f;
        float y = (Screen.height - height) / 2f;

        // 窗口框
        GUI.Box(new Rect(x, y, width, height), "", boxStyle);

        // 标题
        GUI.Label(new Rect(x, y, width, 60), "登录账号", titleStyle);

        GUILayout.BeginArea(new Rect(x + 20, y + 90, width - 40, height - 100));
        scroll = GUILayout.BeginScrollView(scroll);
       

        GUILayout.Space(10);

        if (parsedData != null)
        {
            List<string> keys = new List<string>(fieldValues.Keys);
            foreach (string key in keys)
            {
                object val = fieldValues[key];

                GUILayout.BeginHorizontal();
                GUILayout.Label(key, labelStyle, GUILayout.Width(150));

                if (val is string)
                    fieldValues[key] = GUILayout.TextField((string)val, textFieldStyle);
                else if (val is int)
                {

                    
                    var oldValue = fieldValues[key];
                    fieldValues[key] = int.TryParse(GUILayout.TextField(val.ToString(), textFieldStyle), out var i) ? i : val;


                    
                    if (key == "ServerType" && (int)oldValue != (int)fieldValues[key] ) //如果是服务器类型 
                    {
                        currentServerSelect = (int)fieldValues[key];
                        testConfigDecorate.index = currentServerSelect;
                        var config = testConfigDecorate.serverList[currentServerSelect];
                        jsonInput = JsonConvert.SerializeObject(config);
                        TryParseJson();
                        
                    }
                }
                    
                else if (val is float)
                    fieldValues[key] = float.TryParse(GUILayout.TextField(val.ToString(), textFieldStyle), out var f) ? f : val;
                else if (val is bool)
                {
                    fieldValues[key] = DrawCircleToggle((bool)val, 40f); // 第二个参数控制圆圈大小
                }
                    
                else
                    GUILayout.Label("(不支持)", labelStyle);

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("创建新账号", buttonStyle, GUILayout.Height(80), GUILayout.Width(Screen.width * 0.6f)))
            {
                fieldValues["use_token"] =  DateTime.Now.Ticks.ToString();
            }

            if (GUILayout.Button("进入游戏", buttonStyle, GUILayout.Height(80), GUILayout.Width(Screen.width * 0.6f)))
            {
                var refreshConfig = typeof(GameDebugLauncher).GetMethod("RefreshTestConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Static);

                var loadJsonConfig = typeof(GameDebugLauncher).GetMethod("LoadTestConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Static);

                var onClickLogin = typeof(HotFix.Module.Login.Interface.UILogin).GetMethod("OnClickGuestBtn",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance|
                    System.Reflection.BindingFlags.Static);

                var uiLogin = GameObject.FindAnyObjectByType<UILogin>();
                var gameDebugLauncher = GameObject.FindAnyObjectByType<GameDebugLauncher>();
                //刷新本地配置 
                testConfigDecorate.serverList[currentServerSelect].use_token = (string  )fieldValues["use_token"];
                testConfigDecorate.serverList[currentServerSelect].ConfVer = (string)fieldValues["ConfVer"];
                
                testConfigDecorate.serverList[currentServerSelect].LogLevel = (GameLogLevel)fieldValues["LogLevel"];
                
                testConfigDecorate.serverList[currentServerSelect].ShowFPS = (bool)fieldValues["ShowFPS"];
                testConfigDecorate.serverList[currentServerSelect].hotUpdate = (bool)fieldValues["hotUpdate"];

                
                refreshConfig?.Invoke(gameDebugLauncher, new object[] { testConfigDecorate });
                //读取配置
                loadJsonConfig?.Invoke(gameDebugLauncher, null);
                //模拟用户点击
                onClickLogin?.Invoke(uiLogin, null);

                Hide();
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private bool DrawCircleToggle(bool value, float size = 40f)
    {
        // 计算正方形区域
        Rect rect = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));

        // 鼠标点击检测（圆心+半径判断）
        Vector2 center = new Vector2(rect.x + size / 2, rect.y + size / 2);
        float radius = size / 2;

        Event e = Event.current;
        if (e.type == EventType.MouseDown && Vector2.Distance(e.mousePosition, center) <= radius)
        {
            value = !value;
            e.Use();
        }

        // 保存 GUI color
        Color prevColor = GUI.color;

        // 画圆形边框（空心圆）
        GUI.color = Color.white;
        GUI.DrawTexture(rect, GetCachedCircleTexture((int)size, Color.white, borderThickness: 3));

        // 画中间实心圆（选中时）
        if (value)
        {
            float innerSize = size * 0.6f;
            float offset = (size - innerSize) / 2f;
            Rect innerRect = new Rect(rect.x + offset, rect.y + offset, innerSize, innerSize);

            GUI.color = Color.green;
            GUI.DrawTexture(innerRect, GetCachedCircleTexture((int)innerSize, Color.green, borderThickness: 0));
        }

        GUI.color = prevColor;
        return value;
    }


    private Dictionary<string, Texture2D> circleCache = new();

    private Texture2D GetCachedCircleTexture(int size, Color color, int borderThickness = 0)
    {
        string key = $"{size}_{color}_{borderThickness}";
        if (circleCache.TryGetValue(key, out var tex)) return tex;

        tex = MakeCircleTexture(size, color, borderThickness);
        circleCache[key] = tex;
        return tex;
    }

    private Texture2D MakeCircleTexture(int size, Color color, int borderThickness = 0)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.hideFlags = HideFlags.DontSave;
        tex.filterMode = FilterMode.Bilinear;

        Color clear = new Color(0, 0, 0, 0);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);

                if (borderThickness > 0)
                {
                    //正方形 排除内结缘的其他范围设置为透明色 ，另外就是半径以内的区域也是设置为透明色
                    if (dist > radius || dist < radius - borderThickness)
                    {
                         tex.SetPixel(x, y, clear);
                    }
                    else
                    {
                        tex.SetPixel(x, y, color);
                    }
                        
                }
                else
                {
                    if (dist > radius)
                        tex.SetPixel(x, y, clear);
                    else
                        tex.SetPixel(x, y, color);
                }
            }
        }

        tex.Apply();
        return tex; 
    }


}
#endif