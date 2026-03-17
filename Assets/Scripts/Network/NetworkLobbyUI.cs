using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class NetworkLobbyUI : MonoBehaviour
{
    #region Variables
    private float sw, sh, bw, bh;

    private enum LobbyState
    { Connecting, MainLobby, WaitingForPlayer, EnteringCode, JoiningRoom, ReadyToStart, Starting }

    private LobbyState state = LobbyState.Connecting;
    private string roomCodeInput  = "";
    private string playerName     = "";
    private string statusMsg      = "";
    private bool   nameEntered    = false;
    private bool   stylesReady    = false;

    // Store delegates so OnDestroy can unsubscribe the exact same instances
    private NetworkManager.NetworkEvent _onStatusChanged;
    private NetworkManager.NetworkEvent _onRoomFull;
    private NetworkManager.NetworkEvent _onError;
    private NetworkManager.NetworkEvent _onPlayerLeft;
    #endregion

    #region Unity
    void Start()
    {
        sw = Screen.width; sh = Screen.height;
        bw = sw * 0.34f;  bh = sh * 0.09f;

        _onStatusChanged = s => statusMsg = s;
        _onRoomFull      = _ => state = LobbyState.ReadyToStart;
        _onError         = e => { statusMsg = e; if (state == LobbyState.JoiningRoom) state = LobbyState.EnteringCode; };
        _onPlayerLeft    = _ => { statusMsg = "Opponent disconnected!"; state = LobbyState.WaitingForPlayer; };

        NetworkManager.OnStatusChanged += _onStatusChanged;
        NetworkManager.OnRoomFull      += _onRoomFull;
        NetworkManager.OnError         += _onError;
        NetworkManager.OnPlayerLeft    += _onPlayerLeft;

        state = LobbyState.Connecting;
        if (NetworkManager.Instance != null) NetworkManager.Instance.ConnectToPhoton();
    }

    void OnDestroy()
    {
        NetworkManager.OnStatusChanged -= _onStatusChanged;
        NetworkManager.OnRoomFull      -= _onRoomFull;
        NetworkManager.OnError         -= _onError;
        NetworkManager.OnPlayerLeft    -= _onPlayerLeft;
    }

    void Update()
    {
        if (state == LobbyState.Connecting    && NetworkManager.IsConnectedToMaster) state = LobbyState.MainLobby;
        if (state == LobbyState.WaitingForPlayer && NetworkManager.IsRoomFull)        state = LobbyState.ReadyToStart;
        if (state == LobbyState.JoiningRoom      && NetworkManager.IsInRoom)          state = LobbyState.ReadyToStart;
    }
    #endregion

    #region GUI
    void OnGUI()
    {
        if (!stylesReady) { UIStyles.Initialize(); stylesReady = true; }
        UIStyles.DrawBackground();

        switch (state)
        {
            case LobbyState.Connecting:     DrawConnecting();     break;
            case LobbyState.MainLobby:
                if (!nameEntered)           DrawNameEntry();
                else                        DrawMainLobby();
                break;
            case LobbyState.EnteringCode:   DrawEnterCode();      break;
            case LobbyState.WaitingForPlayer: DrawWaiting();      break;
            case LobbyState.JoiningRoom:    DrawJoining();        break;
            case LobbyState.ReadyToStart:   DrawReady();          break;
            case LobbyState.Starting:       DrawStarting();       break;
        }

        // Back to menu (always visible except when starting)
        if (state != LobbyState.Starting)
        {
            float btnW = bw * 0.55f;
            if (UIStyles.CentreButton(sw * 0.5f, sh * 0.90f, btnW, bh * 0.60f, "← Main Menu", UIStyles.SecondaryBtn))
            {
                if (NetworkManager.Instance != null) NetworkManager.Instance.LeaveRoom();
                SceneManager.LoadScene("Menu");
            }
        }
    }

    // ── Shared panel helper ──────────────────────────────────────────────────
    void BeginPanel(float panW, float panH, out float px, out float py, out float cx)
    {
        px = (sw - panW) * 0.5f;
        py = (sh - panH) * 0.5f - sh * 0.03f;
        cx = sw * 0.5f;
        GUI.Box(new Rect(px, py, panW, panH), "", UIStyles.PanelBox);
    }

    // ── Screens ───────────────────────────────────────────────────────────────
    void DrawConnecting()
    {
        float cx = sw * 0.5f, cy = sh * 0.5f;
        GUI.Label(new Rect(cx - 220, cy - 80, 440, 60), "Online Multiplayer", UIStyles.TitleStyle);
        GUI.Label(new Rect(cx - 180, cy,      360, 40), "Connecting to server...", UIStyles.SubtitleStyle);
    }

    void DrawNameEntry()
    {
        float panW = sw * 0.38f, panH = sh * 0.42f;
        float px, py, cx; BeginPanel(panW, panH, out px, out py, out cx);

        GUI.Label(new Rect(cx - 160, py + sh * 0.04f, 320, 55), "Online Multiplayer", UIStyles.TitleStyle);
        GUI.Label(new Rect(cx - 130, py + sh * 0.14f, 260, 32), "Enter your display name", UIStyles.SubtitleStyle);

        playerName = GUI.TextField(new Rect(px + 28, py + sh * 0.22f, panW - 56, 48), playerName, 20, UIStyles.InputField);

        if (UIStyles.CentreButton(cx, py + sh * 0.32f, bw * 0.70f, bh * 0.72f, "Continue →", UIStyles.PrimaryBtn))
        {
            if (!string.IsNullOrWhiteSpace(playerName))
            { PhotonNetwork.NickName = playerName.Trim(); nameEntered = true; }
        }
    }

    void DrawMainLobby()
    {
        float panW = sw * 0.40f, panH = sh * 0.52f;
        float px, py, cx; BeginPanel(panW, panH, out px, out py, out cx);

        GUI.Label(new Rect(cx - 160, py + sh * 0.03f, 320, 55), "Online Multiplayer", UIStyles.TitleStyle);
        GUI.Label(new Rect(cx - 160, py + sh * 0.12f, 320, 32), "Welcome,  " + playerName + "!", UIStyles.SubtitleStyle);

        // Show region so both players can verify they're on the same server
        string region = PhotonNetwork.CloudRegion ?? "unknown";
        GUI.Label(new Rect(cx - 160, py + sh * 0.175f, 320, 26), "Server: " + region, UIStyles.BodyStyle);

        float gap = bh * 0.30f;
        float y = py + sh * 0.22f;
        if (UIStyles.CentreButton(cx, y, bw * 0.88f, bh, "Create Room", UIStyles.PrimaryBtn))
        { NetworkManager.Instance?.CreateRoom(); state = LobbyState.WaitingForPlayer; }
        y += bh + gap;
        if (UIStyles.CentreButton(cx, y, bw * 0.88f, bh, "Join Room",   UIStyles.BlueBtn))
        { state = LobbyState.EnteringCode; }

        GUI.Label(new Rect(cx - 155, y + bh + gap * 0.6f, 310, 44),
            "Create a room and share the code, or enter a friend's code to join.",
            UIStyles.BodyStyle);
    }

    void DrawEnterCode()
    {
        float panW = sw * 0.38f, panH = sh * 0.46f;
        float px, py, cx; BeginPanel(panW, panH, out px, out py, out cx);

        GUI.Label(new Rect(cx - 140, py + sh * 0.03f, 280, 55), "Join a Room", UIStyles.TitleStyle);
        GUI.Label(new Rect(cx - 130, py + sh * 0.13f, 260, 30), "Enter the 5-character room code:", UIStyles.BodyStyle);

        roomCodeInput = GUI.TextField(
            new Rect(cx - 80, py + sh * 0.21f, 160, 52),
            roomCodeInput.ToUpper(), 5, UIStyles.InputField).ToUpper();

        float gap = bh * 0.28f;
        float y = py + sh * 0.32f;
        if (UIStyles.CentreButton(cx, y, bw * 0.70f, bh * 0.78f, "Join →", UIStyles.PrimaryBtn))
        {
            if (roomCodeInput.Length == 5)
            { NetworkManager.Instance?.JoinRoom(roomCodeInput); state = LobbyState.JoiningRoom; }
            else statusMsg = "Code must be exactly 5 characters!";
        }
        y += bh * 0.78f + gap;
        if (UIStyles.CentreButton(cx, y, bw * 0.70f, bh * 0.65f, "Back", UIStyles.SecondaryBtn))
        { state = LobbyState.MainLobby; roomCodeInput = ""; statusMsg = ""; }

        if (!string.IsNullOrEmpty(statusMsg))
            GUI.Label(new Rect(cx - 150, y + bh, 300, 36), statusMsg, UIStyles.BodyStyle);
    }

    void DrawWaiting()
    {
        float panW = sw * 0.42f, panH = sh * 0.50f;
        float px, py, cx; BeginPanel(panW, panH, out px, out py, out cx);

        GUI.Label(new Rect(cx - 170, py + sh * 0.03f, 340, 50), "Waiting for Opponent", UIStyles.TitleStyle);
        GUI.Label(new Rect(cx - 155, py + sh * 0.12f, 310, 32), "Share this code with your friend:", UIStyles.SubtitleStyle);

        // Big room code box
        var codePanTex = UIStyles.MakeRoundedTex(320, 90, 18, new Color(0.78f, 0.68f, 0.95f, 0.55f));
        var codeBoxStyle = new GUIStyle(GUI.skin.box);
        codeBoxStyle.normal.background = codePanTex;
        GUI.Box(new Rect(cx - 130, py + sh * 0.20f, 260, 80), "", codeBoxStyle);
        GUI.Label(new Rect(cx - 120, py + sh * 0.20f, 240, 80), NetworkManager.RoomCode, UIStyles.CodeStyle);

        GUI.Label(new Rect(cx - 155, py + sh * 0.36f, 310, 36),
            "They need to enter this code in the Online Multiplayer lobby.", UIStyles.BodyStyle);

        if (UIStyles.CentreButton(cx, py + panH - sh * 0.10f, bw * 0.60f, bh * 0.65f, "Cancel", UIStyles.DangerBtn))
        { NetworkManager.Instance?.LeaveRoom(); state = LobbyState.MainLobby; }
    }

    void DrawJoining()
    {
        float cx = sw * 0.5f, cy = sh * 0.5f;
        GUI.Label(new Rect(cx - 160, cy - 70, 320, 55), "Joining Room...", UIStyles.TitleStyle);
        GUI.Label(new Rect(cx - 150, cy,      300, 40), statusMsg, UIStyles.SubtitleStyle);
    }

    void DrawReady()
    {
        float panW = sw * 0.42f, panH = sh * 0.50f;
        float px, py, cx; BeginPanel(panW, panH, out px, out py, out cx);

        GUI.Label(new Rect(cx - 160, py + sh * 0.03f, 320, 55), "Opponent Found!", UIStyles.TitleStyle);

        string roleText = NetworkManager.IsHost
            ? "You are Player 1  ✕  —  You go first!"
            : "You are Player 2  ○  —  Opponent goes first.";
        GUI.Label(new Rect(cx - 170, py + sh * 0.13f, 340, 36), roleText, UIStyles.SubtitleStyle);

        if (NetworkManager.IsHost)
        {
            if (UIStyles.CentreButton(cx, py + sh * 0.27f, bw * 0.88f, bh * 1.10f, "START GAME!", UIStyles.SuccessBtn))
            {
                state = LobbyState.Starting;
                new Player(playerName, "Opponent", true);
                NetworkGameSync.Instance?.SendPlayerName(playerName, true);
                NetworkGameSync.Instance?.SendStartGame();
            }
        }
        else
        {
            GUI.Label(new Rect(cx - 155, py + sh * 0.27f, 310, 40),
                "Waiting for host to start...", UIStyles.BodyStyle);
            NetworkGameSync.Instance?.SendPlayerName(playerName, false);
        }
    }

    void DrawStarting()
    {
        float cx = sw * 0.5f, cy = sh * 0.5f;
        GUI.Label(new Rect(cx - 160, cy - 40, 320, 55), "Starting Game...", UIStyles.TitleStyle);
    }
    #endregion
}
