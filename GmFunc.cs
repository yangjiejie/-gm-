
#if DebugMod
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using HotFix.Ctrl;

using HotFix.Manager.Net.Socket;
using HotFix.Manager.Window;

using HotFix.Module.GameBaloot.Interface;
using HotFix.Module.Hall.GoldFinger.Interface;
using HotFix.Module.Hall.Personal.Mgr;

public static partial class GMFunc
{
    
    [Gm(1, "添加道具")]
    public static void AddItem(int goodId,int number)
    {
        CSPlayerDebug debug = new CSPlayerDebug();
        debug.Params = $"addItem@{goodId},{number}";
        MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
    }
    [Gm(1, "ClearLoginData")]
    public static void ClearLoginData()
    {
        UIGoldFinger.ClearAllPlayerPrefs();
    }
    [Gm(1,"safe user")]
    public static void SafeUser()
    {
        long PlayerId = PlayerMgr.Instance.PlayerId;
        CSPlayerDebug debug = new CSPlayerDebug();
        debug.Params = $"safeUser@{PlayerId},123";
        MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
    }
    [Gm(1,"add  gold")]
    public static void AddGold(int gold)
    {
        
        CSPlayerDebug debug = new CSPlayerDebug();
        debug.Params = $"addItem@11500101,{gold}";
        MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
    }
    [Gm(1,"add exp")]
    public static void AddExp(int exp)
    {
        long PlayerId = PlayerMgr.Instance.PlayerId;
        CSPlayerDebug debug = new CSPlayerDebug();
        debug.Params = $"addExp@{PlayerId},{exp}";
        MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
    }

    [Gm(1,"add vip")]
    public static void AddVip(int vip)
    {
        CSPlayerDebug debug = new CSPlayerDebug();
        debug.Params = $"addVip@{vip},";
        MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
    }
    [Gm(1,"send cmd")]
    public static void SendCmd(string cmd)
    {
        CSPlayerDebug debug = new CSPlayerDebug();
        debug.Params = cmd;
        MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);
    }


    [Gm(1, "B Score")]
    public static void BScore(int score)
    {
        CSPlayerDebug debug = new CSPlayerDebug();
        debug.Params = $"balootScore@{score}";
        MessageMgr.Instance.SendMessage(MsgIds.CSPlayerDebug, debug);

    }

    [Gm(1, "一键成号")]
    public static void AddPlayerAllProp()
    {
        int maxValue = 5000000;
        SafeUser();
        AddGold(maxValue);
        AddExp(maxValue);
        maxValue = 9999999;
        AddVip(maxValue);

    }
}
#endif