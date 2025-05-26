#if DebugMod
using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using HotFix.Manager.Net.Socket;
using HotFix.Module.Hall.RedPack.Interface;
using HotFix.Manager.Window;
using HotFix.Ctrl;
using HotFix.Module.GameBaloot.Interface;
using HotFix.Module.Hall.RankMatch.Interface;
using Best.HTTP.SecureProtocol.Org.BouncyCastle.Asn1.Mozilla;
using Runtime.Core.Event;
#if UNITY_EDITOR 

[CustomEditor(typeof(GmMono))]
public class GmMonoEditor : Editor
{




    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("添加宝箱"))
        {
            CSPlayerDebug debug = new CSPlayerDebug();
            debug.Params = $"addItem@13600001,{1}";
            MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
        }
        if(GUILayout.Button("设置代金券（水果机）"))
        {
            CSPlayerDebug debug = new CSPlayerDebug();
            debug.Params = $"addItem@14600001,{1}";
            MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
        }
        if (GUILayout.Button("清空代金券（水果机）"))
        {
            CSPlayerDebug debug = new CSPlayerDebug();
            debug.Params = $"addItem@14600001,{-99999999}";
            MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
        }
        if (GUILayout.Button("设置代金券（bc）"))
        {
            CSPlayerDebug debug = new CSPlayerDebug();
            debug.Params = $"addItem@14700001,{1}";
            MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
        }
        if (GUILayout.Button("清空代金券（bc）"))
        {
            CSPlayerDebug debug = new CSPlayerDebug();
            debug.Params = $"addItem@14700001,{-99999999}";
            MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
        }

        if (GUILayout.Button("设置长老1000"))
        {
            CSPlayerDebug debug = new CSPlayerDebug();
            debug.Params = $"setStar@45,1000";
            MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
        }

        if (GUILayout.Button("设置星耀5"))
        {
            CSPlayerDebug debug = new CSPlayerDebug();
            debug.Params = $"setStar@44,5";
            MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
        }

        if (GUILayout.Button("设置木2"))
        {
            CSPlayerDebug debug = new CSPlayerDebug();
            debug.Params = $"setStar@2,2";
            MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
        }



       
        if (GUILayout.Button("打开新赛季欢迎界面"))
        {
            UIWelcomeRankMatch.closeCallFunc = () =>
            {
                UIRankMatchMain.initShowOrHide = true;
                GameEvent.Send(GameEventId.OpenRankMatchMainUI);

            };

            if (!UIManager.Instance.IsWindowShow<UIWelcomeRankMatch>())
            {
                UIManager.Instance.ShowWindow<UIWelcomeRankMatch>();
            }
            UIRankMatchMain.initShowOrHide = false;
            if (!UIManager.Instance.IsWindowShow<UIRankMatchMain>())
            {
                UIManager.Instance.ShowWindow<UIRankMatchMain>();
            }
        }

        if(GUILayout.Button("显示排位赛主界面"))
        {
            UIManager.Instance.ShowWindow<UIWelcomeRankMatch>();
        }



#if HOT_UPDATE_TEST
        if(GUILayout.Button("测试分包逻辑-bc"))
        {
            
            var devivery = DlcHelper.Get(YooAssetsDefine.bcPackageName, 3);
            devivery.CheckDownLoad1();
        }
        if (GUILayout.Button("测试分包逻辑-fruit"))
        {
            var devivery = DlcHelper.Get(YooAssetsDefine.fruitPackageName, 3);
            devivery.CheckDownLoad1();
        }
#endif

#if DEV_DEBUG_NET
        if (GUILayout.Button("测试置灰1"))
        {

            Selection.activeGameObject.SetGray(true);
        }
        if (GUILayout.Button("测试反置灰2"))
        {
            Selection.activeGameObject.SetGray(false);
        }

        if (GUILayout.Button("被剔出游戏"))
        {
            GameMgr.Instance.Logout();
        }
       
        if((HotFacade.monitorNetState && GUILayout.Button("模拟无网络")) ||
            (!HotFacade.monitorNetState && GUILayout.Button("模拟网络恢复")))
        {
            HotFacade.monitorNetState = !HotFacade.monitorNetState;
        }
        if(MessageMgr.forbidConnectSocket && GUILayout.Button("不禁止重连")  ||
            !MessageMgr.forbidConnectSocket && GUILayout.Button("禁止重连"))
        {
            MessageMgr.forbidConnectSocket = !MessageMgr.forbidConnectSocket;
        }
        if(GUILayout.Button("测试主动断开网络"))
        {
            
            WebSocketMgr.Instance.CloseSocket();
        }
        if(GUILayout.Button("测试无网络弹窗"))
        {
            UIDialog.ShowBoxTipWithSingleButton(LangHelper.GetValue(LanguageEnum.Language_40002785), () =>
            {
                Debug.Log("ok do !!!");
            }, ArabConstHelper.Sure);
        }
        if(!UIManager.forbidCloseUIWaiting  && GUILayout.Button("禁止关闭waiting") ||
            UIManager.forbidCloseUIWaiting && GUILayout.Button("不禁止关闭waiting"))
        {
            UIManager.forbidCloseUIWaiting = !UIManager.forbidCloseUIWaiting;
        }
#endif
#if FRUIT_DEBUG


        if (GUILayout.Button("弹窗"))
        {
            UIDialog.ShowBoxTip("التحقق", () =>
            {
                Debug.Log("ok do !!!");
            }, null, ArabConstHelper.Sure, ArabConstHelper.Cancel);
        }

        if (GUILayout.Button("弹窗2"))
        {
            UIDialog.ShowBoxTipWithSingleButton("التحقق", () =>
            {
                Debug.Log("ok do !!!");
            }, ArabConstHelper.Sure);
        }
        if (GUILayout.Button("111"))    
        {
            var xx = GameObject.FindFirstObjectByType<GameFruitUI>();
            xx.HandlerMyCouponBet();


        }

        //if(GUILayout.Button("播放转盘动画1"))
        //{
        //    var turnTable = GameObject.FindObjectOfType<GameFruitTurnTable>();
        //    turnTable.StartOrCloseTrunEffect(true,withCompleteEvent:false);
        //}
        //if (GUILayout.Button("播放转盘动画2"))
        //{
        //    var turnTable = GameObject.FindObjectOfType<GameFruitTurnTable>();
        //    turnTable.StartOrCloseTrunEffect(false, withCompleteEvent: false);
        //}
        //if (GUILayout.Button("测试飞金币特效"))
        //{
        //    List<RewardShow> list = new();
        //    var reShow = new RewardShow();
        //    reShow.ConfId = 11500101;
        //    reShow.Count = 1;
        //    list.Add(reShow);

        //    GameHelper.CheckRewardsShow(list);

        //    Timer.Register(1, () =>
        //    {
        //        var propUpWindow = UIManager.Instance.GetWindow<UIRewardProp>();
        //        var coin = propUpWindow.transform.FindComponent_BFS<Transform>("slot_coin");
        //        coin.SetActiveX(true);
        //        var control = coin.gameObject.AddComponent<ParticleHelper>();
        //        var target = propUpWindow.transform.FindComponent_BFS<Transform>("target");
        //        ParticleHelper.ParticelParam param = new ParticleHelper.ParticelParam();
        //        param.renderOrder = 2000;
        //        param.target = target;
        //        param.forceStrength = 30;
        //        param.gravityModifier = 143;
        //        param.speed = 0.3f;
        //        param.isFollow = false;
        //        param.localScale = 0.8f;
        //        control.SetParam(param);
        //    });

        //}
        if (GUILayout.Button("下注5"))
        {
            GameEvent.Send(GameEventId.FruitClickBet, 5);
        }
        if (GUILayout.Button("下注1"))
        {
            GameEvent.Send(GameEventId.FruitClickBet, 1);
        }
        if (GUILayout.Button("下注3"))
        {
            GameEvent.Send(GameEventId.FruitClickBet, 3);
        }

       

        if (GUILayout.Button("模拟上一轮下注"))
        {
            var luckyCard = GameObject.FindFirstObjectByType<GameFruitUI>();
            luckyCard.MonitorBet();

        }
#endif
    }
}
#endif
public class GmMono : MonoBehaviour
{
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
    }
}
#endif