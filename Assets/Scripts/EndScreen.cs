using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndScreen : MonoBehaviour
{
    #region Variables
    private float sw, sh, bw, bh;

    static string[] winMsgArr = new string[7]
    {
        " is simply the best!",
        " is King of the World!",
        " crushed the opponent!",
        " played that perfectly!",
        " wins this round!",
        " is Tic Tac Toe-tally AWESOME!",
        " — Undefeated!"
    };

    static string winMsg = "";
    const  int    numMsgs = 7;

    private bool remoteWantsRematch = false;
    private bool localWantsRematch  = false;
    private bool stylesReady        = false;

    // Styles
    private GUIStyle winnerStyle;
    private GUIStyle drawStyle;
    #endregion

    void Start()
    {
        sw = Screen.width; sh = Screen.height;
        bw = sw * 0.34f;  bh = sh * 0.09f;
        SetWinMsg();

        if (Player.IsOnlineGame)
            NetworkGameSync.OnRemoteRetryRequested += OnRemoteRetryRequested;
    }

    void OnDestroy()
    {
        if (Player.IsOnlineGame)
            NetworkGameSync.OnRemoteRetryRequested -= OnRemoteRetryRequested;
    }

    void OnGUI()
    {
        if (!stylesReady)
        {
            UIStyles.Initialize();

            winnerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 38,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap  = true
            };
            winnerStyle.normal.textColor = new Color(0.35f, 0.20f, 0.60f, 1f);

            drawStyle = new GUIStyle(winnerStyle);
            drawStyle.normal.textColor = new Color(0.55f, 0.38f, 0.55f, 1f);

            stylesReady = true;
        }

        UIStyles.DrawBackground();

        float cx = sw * 0.5f;
        bool  isDraw = GameInfo.IsItADraw();

        // ── Winner / Draw panel ──────────────────────────────────────────────
        float panW = sw * 0.56f, panH = sh * 0.28f;
        float panX = (sw - panW) * 0.5f;
        float panY = sh * 0.12f;
        GUI.Box(new Rect(panX, panY, panW, panH), "", UIStyles.PanelBox);

        if (isDraw)
        {
            GUI.Label(new Rect(cx - 220, panY + 14, 440, 60),  "It's a Draw!", drawStyle);
            GUI.Label(new Rect(cx - 220, panY + 80, 440, 60),  "Nobody wins this time...", UIStyles.SubtitleStyle);
        }
        else
        {
            GUI.Label(new Rect(cx - 240, panY + 14,  480, 60), GameInfo.GetWinner(), winnerStyle);
            GUI.Label(new Rect(cx - 240, panY + 78,  480, 60), winMsg, UIStyles.SubtitleStyle);
        }

        // ── Buttons ──────────────────────────────────────────────────────────
        float gap = bh * 0.38f;
        float y   = sh * 0.53f;

        if (Player.IsOnlineGame)
        {
            string rematchLabel = localWantsRematch  ? "Waiting for opponent..."
                                : remoteWantsRematch ? "Accept Rematch!"
                                : "Rematch?";

            GUIStyle rematchStyle = remoteWantsRematch ? UIStyles.SuccessBtn : UIStyles.PrimaryBtn;
            if (UIStyles.CentreButton(cx, y, bw, bh, rematchLabel, rematchStyle))
            {
                localWantsRematch = true;
                NetworkGameSync.Instance?.SendRetryRequest();
                if (remoteWantsRematch && localWantsRematch)
                    SceneManager.LoadScene("Gameplay");
            }
            y += bh + gap;
            if (UIStyles.CentreButton(cx, y, bw, bh, "Leave Game", UIStyles.DangerBtn))
            { NetworkManager.Instance?.LeaveRoom(); SceneManager.LoadScene("Menu"); }
        }
        else
        {
            if (UIStyles.CentreButton(cx, y, bw, bh, "Play Again?", UIStyles.PrimaryBtn))
                SceneManager.LoadScene("Gameplay");
            y += bh + gap;
            if (UIStyles.CentreButton(cx, y, bw, bh, "Main Menu", UIStyles.SecondaryBtn))
                SceneManager.LoadScene("Menu");
        }
    }

    void OnRemoteRetryRequested()
    {
        remoteWantsRematch = true;
        if (localWantsRematch)
            SceneManager.LoadScene("Gameplay");
    }

    public static void ExecuteHistorySystem()
    {
        History.PopulatePlayerHistory();
        History.UpdatePlayerHistory(Player.GetPlayer1Name(), GameInfo.GetWinner());
        History.UpdatePlayerHistory(Player.GetPlayer2Name(), GameInfo.GetWinner());
        History.DisplayArray();
        History.WriteHistoryFile();
    }

    void SetWinMsg()
    {
        if (GameInfo.IsItADraw())
            winMsg = "Nobody wins this time...";
        else
            winMsg = winMsgArr[new System.Random().Next(numMsgs)];
    }
}
