#if DebugMod
using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif

public class GmMono : MonoBehaviour
{

    public void Start()
    {
        HotFix.Helper.LogHelper.LogHelper.Instance.Init();
        
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            
            GmGui.Instance.Init();
            if(!GmGui.Instance.isShow)
            {
                GmGui.Instance.ShowUI();

            }
            else
            {
                GmGui.Instance.HideUI();

            }
        }
#if DebugMod && UNITY_STANDALONE_WIN
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!GmLoginAssist.Instance.isShow)
            {
                GmLoginAssist.Instance.Show();

            }
            else
            {
                GmLoginAssist.Instance.Hide();

            }
        }
#endif
#if UNITY_EDITOR
        else if(Input.GetKey(KeyCode.RightShift) &&  Input.GetKeyDown(KeyCode.P))
        {
            EditorApplication.ExecuteMenuItem("Edit/Pause");

        }
#endif
    }
}
#endif