#if DebugMod
using HotFix.Ctrl;

using HotFix.Manager.Net.Socket;
using HotFix.Manager.Window;

using HotFix.Module.GameBaloot.Interface;
using HotFix.Module.Hall.Activity.Interface;
using HotFix.Module.Hall.GoldFinger.Interface;
using HotFix.Module.Hall.Personal.Mgr;
using HotFix.Module.Hall.RankMatch.Interface;

public static partial class GMFunc
{
    [Gm(2, "打开新赛季欢迎")]
    public static void OpenWelcomeRankMatch()
    {
        UIManager.Instance.ShowWindow<UIWelcomeRankMatch>();
    }

    [Gm(2, "打开破产礼包")]
    public static void OpenBankRuptPack()
    {
        UIManager.Instance.ShowWindow<UIBankruptPack>();
    }
        [Gm(2, "打开结算窗口")]
    public static void OpenRankMatchResultUI()
    {
        SCBalootGameSettle data = new SCBalootGameSettle();

        data = new SCBalootGameSettle();
        data.SurrenderColor = BalootTeamColor.ColorBlue;
        data.FreeBalootCoins = 99;
        data.ChairCoins.Add(1, 1);
        data.ChairCoins.Add(2, 2);
        data.ChairCoins.Add(3, 3);
        data.ChairCoins.Add(4, 4);

        if (data.RankedMatchSettle == null)
        {
            data.RankedMatchSettle = new();
        }
        if (data.RankedMatchSettle.CurTire == null)
        {
            data.RankedMatchSettle.CurTire = new();
            data.RankedMatchSettle.CurTire.TotalStar = 11001;
            data.RankedMatchSettle.CurTire.Season = 1;
        }

        if (data.RankedMatchSettle.BeforeTire == null)
        {
            data.RankedMatchSettle.BeforeTire = new();
            data.RankedMatchSettle.BeforeTire.TotalStar = 11002;
            data.RankedMatchSettle.BeforeTire.Season = 1;
        }


        data.RankedMatchSettle = new SCBalootGameSettle.Types.RankedMatchSettle();

        data.RankedMatchSettle.CurTire = new RankTier();
        data.RankedMatchSettle.CurTire.TotalStar = 110001;

        data.RankedMatchSettle.BeforeTire = new RankTier();
        data.RankedMatchSettle.BeforeTire.TotalStar = 110002;


        data.RankedMatchSettle.BeforeVp = 0;
        var roundList = data.Rounds;
        roundList.Add(new SCBalootGameSettle.Types.Round());
        roundList[roundList.Count - 1].BlueTeamScore = 100;
        roundList[roundList.Count - 1].RedTeamScore = 100;
        roundList[roundList.Count - 1].Multiple = 1;

        UIManager.Instance.ShowWindow<GameBalootRankMatchResult>(data, true);
    }
}
#endif