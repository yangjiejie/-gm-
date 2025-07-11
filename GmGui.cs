
#if DebugMod
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using HotFix.Manager;
using HotFix.Module.Hall.Dialog.Interface;
using HotFix.Module.Hall.Personal.Mgr;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GmGui : SingletonMono<GmGui>
{

    public enum ShowModel
    {
        Normal,
        Detail,
    }

    ShowModel showMode = ShowModel.Normal;
    private List<UnityEngine.Object> recordObjs;
    static float initY = 0;
    static float initX = 20;
    bool lastGmCommandToggle { get; set; } = false;


    public Rect rect = new Rect(20, initY, 125    , 125);

    public Rect rectButton = new Rect(20, initY + 200, 125, 125);
    public Rect gmRect = new Rect(0,0,225,225);
    public Rect left_arrow = new Rect();
    public Rect right_arrow = new Rect();
    public Rect func_param = new Rect();
  
    public Rect panel_text_area = new Rect();

    public Action<string> saveLogFunc;

    public const int MaxColume = 5;


    GUIStyle arrawButtonStyle;

    public int gap = 20;
    public Vector2 buttonSize = new Vector2(200, 200);
    public bool isHideInHierarchy = false;

    public GUIStyle style;

    public GUIStyle tipStyle;

    GUIStyle titleLable;

    GUIStyle lableStyle;

    GUIStyle inputStyle;

    GUIStyle lableExeButton;

    GUIStyle lastGmToggleStyle;

  

    MethodInfoEx currentDrawfuncEx;
    string currentButtonName;
    string[] currentPs;
    string currentTag = "";


    public string gameGmButton = "gm系统";
    private int isExpandGmUI = 0; // 是否展开 
    
    private int IsExpandGmUI
    {
        set
        {
            if(IsExpandGmUI != value)
            {
                isExpandGmUI = value;
                OnFlagChange();
            }
        }
        get
        {
            return isExpandGmUI;
        }
    }

    public class MethodInfoEx
    {
        
        public MethodInfo methodInfo;
        public string desc;
        public bool isLua;
    }

    public Dictionary<int, List<MethodInfoEx>> methodMap = new();



    public List<int> pageKeys = new List<int>();

    public Type gmType;

    public string inputText;
    public int panelIndex = 0;
    public string panelIndexStr;

    public int maxPanelCount = 0;
    
    public string Content = string.Empty;

    public bool isShow { get; set; } = false;

    
    private void OnFlagChange()
    {
        if(isExpandGmUI == 1)
        {
            HideEvent(true);
        }
        else
        {
            HideEvent(false);
            
        }
    }
   
    public void InitMethodInfo(int idGroup , MethodInfo me, bool isLua,string desc)
    {
        string tdesc = isLua ? desc : me.GetCustomAttribute<GmAttribute>().desc;
        if (!methodMap.ContainsKey(idGroup))
        {
            var tmpMap = new List<MethodInfoEx>();
            methodMap[idGroup] = tmpMap;
            tmpMap.Add(new MethodInfoEx() { methodInfo = me, isLua = isLua, desc = tdesc });
        }
        else
        {
            methodMap[idGroup].Add(new MethodInfoEx() { methodInfo = me, isLua = isLua, desc = tdesc });
        }
        if (!pageKeys.Contains(idGroup))
        {
            pageKeys.Add(idGroup);
        }
    }
    public bool isInited { get; set; } = false;

    private GmCommandManager gmManager { get; set; }

    public void HideUI()
    {
        this.isShow = false;
        gmManager.SaveCommand();
        HideEvent(false);
    }
    public void ShowUI()
    {
        isShow = true;
    }
    public override void Init()
    {
        if (isInited) return;
        isInited = true;
        gmManager = new GmCommandManager();
        gmManager.Init();
        base.Init();
       
        if (isHideInHierarchy)
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
        else
        {
            gameObject.hideFlags = HideFlags.None;
        }
        
        GameObject.DontDestroyOnLoad(gameObject);
        
        string path = "";
        path = path.Replace("\\", "/");
        gmType = typeof(GMFunc);
        var methods = gmType.GetMethods(System.Reflection.BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(o=>(Attribute.IsDefined(o,typeof(GmAttribute))));
        foreach(var me in methods)
        {
            foreach (var item in me.GetCustomAttributes(false))
            {
                var idGroup = (item as GmAttribute).idGroup;
                
                
                InitMethodInfo(idGroup , me,  false,"");
            }
        }
        maxPanelCount = methodMap.Keys.Count;

        pageKeys.Sort();

        panelIndexStr = pageKeys[panelIndex].ToString();
    }


    private void DrawLeftAndRightExchangePanel()
    {
        left_arrow.width = rect.width / 1.2f;
        left_arrow.height = rect.height / 1.2f;
        left_arrow.x = left_arrow.width + gap;
        left_arrow.y = rect.y;
        

        right_arrow.x = left_arrow.x + left_arrow.width + gap;
        right_arrow.y = rect.y;
        right_arrow.width = rect.width / 1.2f;
        right_arrow.height = rect.height / 1.2f;



        panel_text_area.width = 75;
        panel_text_area.height = 75;
        panel_text_area.x = Screen.width - panel_text_area.width* 2;
        panel_text_area.y = 0;
        

       

        func_param.x = right_arrow.x + right_arrow.width + gap;
        func_param.y = rect.y;
        func_param.width = rect.width / 1.2f;
        func_param.height = rect.height / 1.2f;

        if (GUI.Button(left_arrow, "<-", arrawButtonStyle))
        {
            if (--panelIndex < 0)
                panelIndex = maxPanelCount - 1;
            panelIndexStr = pageKeys[panelIndex].ToString();
        }

        GUI.changed = false;
         GUI.TextArea(panel_text_area, $"页面：\n{panelIndexStr}", this.style);
        if (GUI.changed)
        {
            int.TryParse(panelIndexStr, out panelIndex);
        }

        if (GUI.Button(right_arrow, "->", arrawButtonStyle))
        {
            if (++panelIndex >= maxPanelCount)
                panelIndex = 0;
            panelIndexStr = pageKeys[panelIndex].ToString();
        }



    }

    private void HideEvent(bool isTrue)
    {
        
        if (recordObjs == null) recordObjs = UnityEngine.Pool.ListPool<UnityEngine.Object>.Get();
        recordObjs.Clear();

        var xx = GameObject.FindObjectsByType(typeof(GraphicRaycaster), FindObjectsSortMode.None);
        recordObjs.AddRange(xx);
        foreach (var item in recordObjs)
        {
            if (item)
            {
                (item as GraphicRaycaster).enabled = !isTrue;
            }
        }
    }
    void InitStyle()
    {
        if (lableStyle == null)
        {
            tipStyle = new GUIStyle(GUI.skin.label);
            tipStyle.fontSize = 36;
            tipStyle.alignment = TextAnchor.MiddleCenter;
            titleLable = new GUIStyle(GUI.skin.label);
            titleLable.fontSize = 42;
            titleLable.alignment = TextAnchor.MiddleCenter;
            titleLable.wordWrap = true;

            inputStyle = new GUIStyle(GUI.skin.textField);
            inputStyle.fontSize = 28;
            inputStyle.alignment = TextAnchor.MiddleCenter;

            lableStyle = new GUIStyle(GUI.skin.label);
            lableStyle.fontSize = 36;
            lableStyle.fixedHeight = 80;
            lableStyle.fixedWidth = 400;

            lableExeButton = new GUIStyle(GUI.skin.button);
            lableExeButton.fontSize = 36;
            lableExeButton.fixedHeight = 100;
            lableExeButton.fixedWidth = Screen.width * 0.75f - 10;
            lableExeButton.alignment = TextAnchor.MiddleCenter;

            lastGmToggleStyle = new GUIStyle(GUI.skin.toggle);
            lastGmToggleStyle.fontSize = 36;
            lastGmToggleStyle.fixedHeight = 100;
            lastGmToggleStyle.fixedWidth = 300;

            Texture2D toggleOffTex = GUI.skin.toggle.normal.background;
            Texture2D toggleOnTex = GUI.skin.toggle.onNormal.background;

            lastGmToggleStyle.normal.background = toggleOffTex;
            lastGmToggleStyle.onNormal.background = toggleOnTex;

            lastGmToggleStyle.imagePosition = ImagePosition.ImageLeft; // 图标在左，文字在右
            lastGmToggleStyle.padding = new RectOffset(50, 0, 0, 0); // 让文字和勾选分开

            style = new GUIStyle(GUI.skin.button);
            style.fontSize = 42;
            style.alignment = TextAnchor.MiddleCenter;
            style.wordWrap = true;
            style.fixedWidth = 200;
            style.fixedHeight = 200;

            style.alignment = TextAnchor.MiddleCenter;
            style.wordWrap = true;




            arrawButtonStyle = new GUIStyle(GUI.skin.button);
            arrawButtonStyle.fontSize = 42;
            arrawButtonStyle.alignment = TextAnchor.MiddleCenter;

        }
    }
    private Rect timePos = new Rect(50, 30, 90, 70);
    private void OnGUI() // 绘制GUI界面的坐标系以屏幕的左上角为（0，0）点”
    {
        if (!isShow) return;

        long serverTimeMs = (long)(PlayerMgr.Instance.ServerTime / 1000);

        // 1. 转换为 DateTimeOffset（UTC）
        DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeSeconds(serverTimeMs);

        // 2. 转换为本地时间
        DateTime localTime = utcTime.LocalDateTime;

        // 3. 格式化为 yyyy:MM:dd
        string formattedDate = localTime.ToString("yyyy:MM:dd");


        GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);

        // 2. 修改字体大小
        textFieldStyle.fontSize = 30; // 改成你需要的字号

        // 3. 使用这个样式来绘制 TextField
         GUI.TextArea(new Rect(10, 10, 300, 30), formattedDate, textFieldStyle);
       

            InitStyle();
        

       
        gmRect.x = Screen.width/ 2 - gmRect.width/ 2;
        gmRect.y = 100;
        //绘制gm按钮  
        if (GUI.Button(gmRect, gameGmButton, style))
        {
            IsExpandGmUI = isExpandGmUI == 0 ? 1 : 0;
        }


        if (isExpandGmUI == 1) // 展开 
        {
            if (showMode == ShowModel.Normal)
            {
                var realIndex = pageKeys[panelIndex];
                methodMap.TryGetValue(realIndex, out var subMap);
                var subMapCount = subMap?.Count ?? 1;
                DrawLeftAndRightExchangePanel();
                
                var tmpRect = new Rect( left_arrow.x,left_arrow.y + left_arrow.height, lastGmToggleStyle.fixedWidth, lastGmToggleStyle.fixedHeight);
                //绘制最近gm指令集 开关 


                Rect toggleRect = tmpRect;

                if (GUI.Button(new Rect(toggleRect.x - 125, toggleRect.y+gap, 100, 100), "")) // 左侧绘制大勾
                {
                    lastGmCommandToggle = !lastGmCommandToggle;
                }

                Texture2D tex = lastGmCommandToggle ? GUI.skin.toggle.onNormal.background : GUI.skin.toggle.normal.background;
                GUI.DrawTexture(new Rect(toggleRect.x- 125, toggleRect.y+ gap, 100, 100), tex, ScaleMode.StretchToFill);

                GUIStyle bigFont = new GUIStyle(GUI.skin.label);
                bigFont.fontSize = 36;
                bigFont.wordWrap = true;
                GUI.Label(new Rect(toggleRect.x -20, toggleRect.y+ gap, 175, 100), "最近常用gm指令集", bigFont);


                GUIStyle gmArchiveButton = new GUIStyle(GUI.skin.button);
                gmArchiveButton.fontSize = 36;
                gmArchiveButton.alignment = TextAnchor.MiddleCenter;
                gmArchiveButton.fixedHeight = 150;
                gmArchiveButton.fixedWidth = 150;
                gmArchiveButton.wordWrap = true;  // 支持换行 
                //绘制清理gm存档
                if (GUI.Button(new Rect(toggleRect.x - 100 , toggleRect.y+150, gmArchiveButton.fixedWidth, gmArchiveButton.fixedHeight), "跳转到存档目录", gmArchiveButton))
                {

#if UNITY_EDITOR
                    EditorUtility.RevealInFinder(Application.persistentDataPath);
#endif

                }
                //绘制清理gm存档
                if (GUI.Button(new Rect(toggleRect.x - 100 + 30+ gmArchiveButton.fixedWidth, toggleRect.y+150 , gmArchiveButton.fixedHeight , gmArchiveButton.fixedHeight), "清理gm存档", gmArchiveButton))
                {
                    gmManager.Clear();
                }



                //绘制提示语 
                tmpRect.Set(right_arrow.x + right_arrow.width, left_arrow.y, 600,100);

                GUI.Label(tmpRect, "按g关闭gm系统，当前所有游戏按钮事件已经关闭,关闭gm系统后恢复", tipStyle);




                if (GUI.Button(new Rect(panel_text_area.x, panel_text_area.y + panel_text_area.height + 125, 75, 75), "↑", arrawButtonStyle))
                {
                    rect.y = Mathf.Clamp(rect.y - buttonSize.y, -subMapCount * gap - initY, Screen.height - initY);
                    rectButton.y = Mathf.Clamp(rectButton.y - buttonSize.y, -subMapCount * gap - initY-100, Screen.height - initY - 100);
                }
                if (GUI.Button(new Rect(panel_text_area.x, panel_text_area.y + panel_text_area.height + 200, 75, 75), "↓", arrawButtonStyle))
                {
                    rect.y = Mathf.Clamp(rect.y + buttonSize.y, -subMapCount * gap - initY, Screen.height - initY );
                    rectButton.y = Mathf.Clamp(rectButton.y + buttonSize.y, -subMapCount * gap - initY - 100, Screen.height - initY - 100);
                }

                if (lastGmCommandToggle) // 绘制常用gm指令  
                {
                    int lineNumber = 0;
                    int column = 0;
                    int tmpIndex = 0;
                    for(int i = gmManager.commandList.Count -1; i>=0 ; --i)
                    {
                        
                        lineNumber = tmpIndex / MaxColume;
                        column = tmpIndex % MaxColume;
                        DrawButtonWithTag(lineNumber, column, gmManager.commandList[i].Method, gmManager.commandList[i].method_params,
                            gmManager.commandList[i].tag);
                        tmpIndex++;
                    }
                    
                }
                else // 绘制全部gm指令 
                {
                    
                    foreach (var bigGroup in methodMap) // 大类暂时不用 未来拓展 
                    {
                        if (bigGroup.Key == realIndex)
                        {
                            int nRaw = 0, nColume = 0;
                            for(int i = 0; i < bigGroup.Value.Count;i++)
                            {
                                var item = bigGroup.Value[i];
                                nRaw = i / MaxColume;
                                nColume = i % MaxColume;
                                DrawButtonSimple(nRaw, nColume, item);
                                
                            }
                            
                            break;
                        }
                    }
                }
                
            }
            else if (showMode == ShowModel.Detail)
            {
                DrawButton(currentDrawfuncEx, currentButtonName, currentPs, ref currentTag);
            }

        }
    }
    private Dictionary<MethodInfo, string[]> paramInputDict = new();
    
    string inputWidth = "400";
    string inputheight = "50";


    void DrawButtonWithTag(int row, int column, MethodInfoEx funcEx, string[] ps ,  string tag = "")
    {
        MethodInfo func = null;
        if (!funcEx.isLua)
        {
            func = funcEx.methodInfo;
        }
        string buttonName = funcEx.desc;
        var templeteRect = rect;
        templeteRect.Set(initX + column * (gap + buttonSize.x), rectButton.y + (row + 1) * (gap + buttonSize.y), rectButton.width, rectButton.height);

        var showButtonName = string.IsNullOrEmpty(tag) ? buttonName : tag;

        if (GUI.Button(templeteRect, showButtonName, style))
        {
            if (!funcEx.isLua)
            {
                showMode = ShowModel.Detail;
                currentDrawfuncEx = funcEx;
                currentButtonName = buttonName;
                currentPs = ps;
                currentTag = tag;
            }
        }
    }

    void DrawButtonSimple(int row, int column, MethodInfoEx funcEx,string[] ps = null)
    {
        
        MethodInfo func = null;
        if (!funcEx.isLua)
        {
            func = funcEx.methodInfo;
        }
        string buttonName =  funcEx.desc;
        var templeteRect = rect;
        templeteRect.Set(initX + column * (gap + buttonSize.x), rectButton.y + (row + 1) * (gap + buttonSize.y), rectButton.width, rectButton.height);

        var showButtonName = buttonName;

        if (GUI.Button(templeteRect, showButtonName, style))
        {
            if (!funcEx.isLua)
            {
                showMode = ShowModel.Detail;
                currentDrawfuncEx = funcEx;
                currentButtonName = buttonName;
                currentPs = ps;
                currentTag = "";
            }
        }
    }

    void DrawButton(MethodInfoEx funcEx,string buttonName, string[] ps ,ref string alias )
    {
        MethodInfo func = funcEx.isLua ? null : funcEx.methodInfo;
        if (func == null) return;

        // 获取参数信息
        var parameters = func.GetParameters();
        int paramCount = parameters.Length;

        
       
       
        float groupHeight = Screen.height * 0.5f;
        float groupWidth = Screen.width * 0.75f;


        Rect groupRect = new Rect(
            (Screen.width - groupWidth) /  2,
            (Screen.height - groupHeight) / 2,
            groupWidth, 
            groupHeight
        );


        GUILayout.BeginArea(groupRect, GUI.skin.box);

        // 绘制gm接口描述信息 
        GUILayout.Label(buttonName, titleLable);

        if (paramCount > 0)
        {
            // 准备参数输入容器
            if(ps != null)
            {
                paramInputDict[func] = ps;
            }
            else
            {
                if (!paramInputDict.ContainsKey(func))
                    paramInputDict[func] = new string[paramCount];
            }
            

            string[] arr = paramInputDict[func];
            for (int i = 0; i < paramCount; i++)
            {
                ParameterInfo pi = parameters[i];
                GUILayout.BeginHorizontal();
                
                GUILayout.Label($"参数{i + 1}:{pi.Name} ({pi.ParameterType.Name})", lableStyle);
                inputStyle.fixedHeight = float.Parse(inputheight);
                inputStyle.fixedWidth = float.Parse(inputWidth);

                arr[i] = GUILayout.TextField(arr[i] ?? "", inputStyle);
    
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label($"自定义功能别名：", lableStyle);
            alias = GUILayout.TextField(alias, inputStyle);
            GUILayout.EndHorizontal();

            // 执行按钮
            if (GUILayout.Button("执行", lableExeButton))
            {
                object[] paramArr = new object[paramCount];
                bool legal = true;
                for (int i = 0; i < paramCount; i++)
                {
                    try
                    {
                        paramArr[i] = Convert.ChangeType(arr[i], parameters[i].ParameterType);
                    }
                    catch
                    {
                        legal = false;
                        var tips = $"参数{i + 1} 转换失败({arr[i]})→{parameters[i].ParameterType.Name}";
                        UIDialog.ShowTextTip(tips);
                        UnityEngine.Debug.LogError(tips);
                        break;
                    }
                }
                if (legal)
                {

                    gmManager.CreateCommand(funcEx, buttonName, arr, alias);
                    func.Invoke(gmType, paramArr);
                }
            }
        }
        else
        {
            // 无参数直接按钮（同原来）
            if (GUILayout.Button("执行", lableExeButton))
            {
                gmManager.CreateCommand(funcEx, buttonName, null, alias);
                func.Invoke(gmType, null);
            }
        }

        if (GUILayout.Button("退出", lableExeButton))
        {
            showMode = ShowModel.Normal;
        }

        GUILayout.EndArea();
    }

  
}
#endif