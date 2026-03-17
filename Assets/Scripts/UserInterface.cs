using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UserInterface : MonoBehaviour
{
    #region Variables
    private float sw, sh, bw, bh; // screen width/height, button width/height

    // Scroll
    private Vector2 scrollViewVector = Vector2.zero;
    private string scoresHeading = "Player Name\r\nWins / Losses / Draws\r\n\r\n";

    // Screen flags
    static bool MenuScreen    = true,  StartScreen  = false,
                ScoresScreen  = false, AboutScreen  = false,
                ReturningP1   = false, ReturningP2  = false,
                NewP1Screen   = false, NewP2Screen  = false,
                AIScreen      = false, IsThereAI    = false,
                DoesP1GoFirst = false, DoesAIGoFirst = false,
                EnterName     = false, IsPlayer1    = false,
                IsPlayer2     = false;

    static string[] returningPlayers;
    static string   stringToEdit, tempName,
                    player1Name, player2Name, playerHistory;

    static int selGridInt = -1, AIDifficultyLevel = 0;

    static GUIStyle scoreLabelText = new GUIStyle();
    private bool stylesReady = false;
    #endregion

    void Start()
    {
        sw = Screen.width;
        sh = Screen.height;
        bw = sw * 0.36f;
        bh = sh * 0.09f;
        InitializeStrings();
        InitializePlayerHistory();
        scoreLabelText.alignment = TextAnchor.UpperCenter;
        scoreLabelText.fontSize  = 16;
    }

    void OnGUI()
    {
        // Init styles once inside OnGUI (required by Unity legacy GUI)
        if (!stylesReady) { UIStyles.Initialize(); stylesReady = true; }

        UIStyles.DrawBackground();

        if      (MenuScreen)  mainMenu();
        else if (ReturningP1) { startMenu(); returningPlayer1Menu(); }
        else if (ReturningP2) { startMenu(); returningPlayer2Menu(); }
        else if (AIScreen)    { startMenu(); AIMenu(); }
        else if (StartScreen) startMenu();
        else if (ScoresScreen) scoresMenu();
        else if (AboutScreen)  aboutMenu();
        else if (NewP1Screen)  { startMenu(); newPlayer1Menu(); }
        else if (NewP2Screen)  { startMenu(); newPlayer2Menu(); }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MAIN MENU
    // ─────────────────────────────────────────────────────────────────────────
    void mainMenu()
    {
        float cx = sw * 0.5f;
        float panW = bw + 80f;
        float gap  = bh * 0.35f;
        float totalH = bh * 5 + gap * 4;
        float startY = (sh - totalH) * 0.5f + sh * 0.08f;

        // Title
        GUI.Label(new Rect(cx - 220, startY - sh * 0.18f, 440, 80), "🎮  TicTacToe", UIStyles.TitleStyle);
        GUI.Label(new Rect(cx - 200, startY - sh * 0.09f, 400, 36), "5 × 5 · Four in a row wins", UIStyles.SubtitleStyle);

        // Panel behind buttons
        GUI.Box(new Rect(cx - panW * 0.5f - 20, startY - 24, panW + 40, totalH + 48), "", UIStyles.PanelBox);

        float y = startY;
        if (UIStyles.CentreButton(cx, y, bw, bh, "Local Play",         UIStyles.PrimaryBtn))  { MenuScreen = false; StartScreen = true; SceneManager.LoadScene("Start"); }
        y += bh + gap;
        if (UIStyles.CentreButton(cx, y, bw, bh, "Online Multiplayer", UIStyles.BlueBtn))     { MenuScreen = false; SceneManager.LoadScene("OnlineLobby"); }
        y += bh + gap;
        if (UIStyles.CentreButton(cx, y, bw, bh, "Scores",             UIStyles.SecondaryBtn)){ MenuScreen = false; ScoresScreen = true; SceneManager.LoadScene("Scores"); }
        y += bh + gap;
        if (UIStyles.CentreButton(cx, y, bw, bh, "About",              UIStyles.AccentBtn))   { MenuScreen = false; AboutScreen = true; SceneManager.LoadScene("About"); }
        y += bh + gap;
        if (UIStyles.CentreButton(cx, y, bw, bh, "Quit",               UIStyles.DangerBtn))   { Application.Quit(); }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // START / PLAYER SELECTION SCREEN
    // ─────────────────────────────────────────────────────────────────────────
    void startMenu()
    {
        float panW = sw * 0.26f;
        float panH = sh * 0.52f;
        float panY = sh * 0.06f;
        float gap  = bh * 0.22f;
        float btnW = panW * 0.82f;
        float btnH = bh * 0.60f;

        // ── Player 1 Panel ──
        float p1x = sw * 0.08f;
        GUI.Box(new Rect(p1x, panY, panW, panH), "Player 1", UIStyles.PanelBox);
        float y = panY + sh * 0.09f;
        if (GUI.Button(new Rect(p1x + (panW - btnW) * 0.5f, y, btnW, btnH), "New User",       UIStyles.PrimaryBtn))   { NewP1Screen = true; StartScreen = false; }
        y += btnH + gap;
        if (GUI.Button(new Rect(p1x + (panW - btnW) * 0.5f, y, btnW, btnH), "Returning User", UIStyles.SecondaryBtn)) { ReturningP1 = true; }
        y += btnH + gap;
        if (GUI.Button(new Rect(p1x + (panW - btnW) * 0.5f, y, btnW, btnH), "Guest",          UIStyles.AccentBtn))    { player1Name = "Guest"; IsPlayer1 = true; ResetAIFlags(); }
        if (IsPlayer1)
            GUI.Label(new Rect(p1x + 8, panY + panH - sh * 0.09f, panW - 16, sh * 0.06f),
                      "✓  " + player1Name, UIStyles.BodyStyle);

        // ── Player 2 Panel ──
        float p2x = sw - p1x - panW;
        GUI.Box(new Rect(p2x, panY, panW, panH), "Player 2", UIStyles.PanelBox);
        y = panY + sh * 0.09f;
        if (GUI.Button(new Rect(p2x + (panW - btnW) * 0.5f, y, btnW, btnH), "New User",       UIStyles.PrimaryBtn))   { NewP2Screen = true; StartScreen = false; }
        y += btnH + gap;
        if (GUI.Button(new Rect(p2x + (panW - btnW) * 0.5f, y, btnW, btnH), "Returning User", UIStyles.SecondaryBtn)) { ReturningP2 = true; }
        y += btnH + gap;
        if (GUI.Button(new Rect(p2x + (panW - btnW) * 0.5f, y, btnW, btnH), "Guest",          UIStyles.AccentBtn))    { player2Name = "Guest"; IsPlayer2 = true; ResetAIFlags(); }
        y += btnH + gap;
        if (GUI.Button(new Rect(p2x + (panW - btnW) * 0.5f, y, btnW, btnH), "A.I.",           UIStyles.BlueBtn))      { IsThereAI = true; AIScreen = true; IsPlayer2 = true; }
        if (IsPlayer2)
            GUI.Label(new Rect(p2x + 8, panY + panH - sh * 0.09f, panW - 16, sh * 0.06f),
                      "✓  " + player2Name, UIStyles.BodyStyle);

        // ── Who Goes First ──
        float midPanW = sw * 0.22f;
        float midPanX = (sw - midPanW) * 0.5f;
        float midPanY = panY + panH + sh * 0.03f;
        GUI.Box(new Rect(midPanX, midPanY, midPanW, sh * 0.18f), "Who goes first?", UIStyles.PanelBox);
        DoesP1GoFirst  = GUI.Toggle(new Rect(midPanX + 18, midPanY + sh * 0.07f,  midPanW - 36, 28), DoesP1GoFirst,  "  Player 1");
        if (DoesP1GoFirst) DoesAIGoFirst = false;
        if (IsThereAI)
        {
            DoesAIGoFirst = GUI.Toggle(new Rect(midPanX + 18, midPanY + sh * 0.12f, midPanW - 36, 28), DoesAIGoFirst, "  A.I.");
            if (DoesAIGoFirst) DoesP1GoFirst = false;
        }

        // ── Bottom buttons ──
        float botY  = sh * 0.88f;
        float smBW  = bw * 0.55f;

        if (UIStyles.CentreButton(sw * 0.22f, botY, smBW, bh * 0.70f, "← Main Menu", UIStyles.SecondaryBtn))
        {
            StartScreen = false; MenuScreen = true; ResetAIFlags();
            SceneManager.LoadScene("Menu");
        }

        if (IsPlayer1 && IsPlayer2 && (DoesP1GoFirst || DoesAIGoFirst))
        {
            if (UIStyles.CentreButton(sw * 0.78f, botY, smBW, bh * 0.70f, "Play! →", UIStyles.PrimaryBtn))
            {
                new Player(player1Name, player2Name, IsThereAI, DoesAIGoFirst, AIDifficultyLevel);
                ResetMatchToMatchFlags();
                SceneManager.LoadScene("Gameplay");
                IsPlayer1 = false; IsPlayer2 = false;
            }
        }

        if (UIStyles.CentreButton(sw * 0.78f, botY + bh * 0.78f, smBW, bh * 0.55f, "Quit", UIStyles.DangerBtn))
            Application.Quit();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // NEW USER SCREENS
    // ─────────────────────────────────────────────────────────────────────────
    void newPlayer1Menu()
    {
        float panW = sw * 0.30f;
        float panH = sh * 0.40f;
        float panX = (sw - panW) * 0.5f;
        float panY = sh * 0.28f;
        GUI.Box(new Rect(panX, panY, panW, panH), "New Player 1", UIStyles.PanelBox);

        GUI.Label(new Rect(panX + 16, panY + sh * 0.06f, panW - 32, 30), "Enter your name:", UIStyles.BodyStyle);
        stringToEdit = GUI.TextField(new Rect(panX + 20, panY + sh * 0.13f, panW - 40, 42), stringToEdit, 25, UIStyles.InputField);

        // Remove commas
        for (int i = 0; i < stringToEdit.Length; i++)
            if (stringToEdit[i] == ',') stringToEdit = "";

        float btnW = panW * 0.44f;
        if (GUI.Button(new Rect(panX + 20,             panY + sh * 0.26f, btnW, bh * 0.65f), "Confirm", UIStyles.PrimaryBtn))
        {
            if (stringToEdit == player2Name || stringToEdit == "Master Shifu" ||
                stringToEdit == "Cunning Clive" || stringToEdit == "Easy Bob" ||
                stringToEdit == "Guest" || stringToEdit == "" || stringToEdit == " ")
                stringToEdit = "Invalid name";
            else { player1Name = stringToEdit; NewP1Screen = false; StartScreen = true; stringToEdit = ""; IsPlayer1 = true; ResetAIFlags(); }
        }
        if (GUI.Button(new Rect(panX + panW - 20 - btnW, panY + sh * 0.26f, btnW, bh * 0.65f), "Back", UIStyles.SecondaryBtn))
        { NewP1Screen = false; StartScreen = true; }
    }

    void newPlayer2Menu()
    {
        float panW = sw * 0.30f;
        float panH = sh * 0.40f;
        float panX = (sw - panW) * 0.5f;
        float panY = sh * 0.28f;
        GUI.Box(new Rect(panX, panY, panW, panH), "New Player 2", UIStyles.PanelBox);

        GUI.Label(new Rect(panX + 16, panY + sh * 0.06f, panW - 32, 30), "Enter your name:", UIStyles.BodyStyle);
        stringToEdit = GUI.TextField(new Rect(panX + 20, panY + sh * 0.13f, panW - 40, 42), stringToEdit, 25, UIStyles.InputField);

        for (int i = 0; i < stringToEdit.Length; i++)
            if (stringToEdit[i] == ',') stringToEdit = "";

        float btnW = panW * 0.44f;
        if (GUI.Button(new Rect(panX + 20,             panY + sh * 0.26f, btnW, bh * 0.65f), "Confirm", UIStyles.PrimaryBtn))
        {
            if (stringToEdit == player1Name || stringToEdit == "Master Shifu" ||
                stringToEdit == "Cunning Clive" || stringToEdit == "Easy Bob" ||
                stringToEdit == "Guest" || stringToEdit == "" || stringToEdit == " ")
                stringToEdit = "Invalid name";
            else { player2Name = stringToEdit; NewP2Screen = false; StartScreen = true; stringToEdit = ""; IsPlayer2 = true; ResetAIFlags(); }
        }
        if (GUI.Button(new Rect(panX + panW - 20 - btnW, panY + sh * 0.26f, btnW, bh * 0.65f), "Back", UIStyles.SecondaryBtn))
        { NewP2Screen = false; StartScreen = true; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // RETURNING USER SCREENS
    // ─────────────────────────────────────────────────────────────────────────
    void returningPlayer1Menu()
    {
        float panW = sw * 0.30f;
        float panH = sh * 0.50f;
        float panX = (sw - panW) * 0.5f;
        float panY = sh * 0.22f;
        GUI.Box(new Rect(panX, panY, panW, panH), "Select Player 1", UIStyles.PanelBox);

        scrollViewVector = GUI.BeginScrollView(
            new Rect(panX + 10, panY + sh * 0.06f, panW - 20, sh * 0.30f),
            scrollViewVector, new Rect(0, 0, panW - 40, sh * 1.5f));
        if ((selGridInt = GUI.SelectionGrid(new Rect(0, 0, panW - 40, sh * 0.75f), selGridInt, returningPlayers, 1)) > 0)
            if (returningPlayers[selGridInt] == player2Name) selGridInt = -1;
        GUI.EndScrollView();

        if (selGridInt != -1)
        { player1Name = returningPlayers[selGridInt]; ReturningP1 = false; selGridInt = -1; StartScreen = true; IsPlayer1 = true; ResetAIFlags(); }

        float btnW = panW * 0.44f;
        if (GUI.Button(new Rect(panX + (panW - btnW) * 0.5f, panY + panH - sh * 0.08f, btnW, bh * 0.60f), "Back", UIStyles.SecondaryBtn))
        { ReturningP1 = false; StartScreen = true; }
    }

    void returningPlayer2Menu()
    {
        float panW = sw * 0.30f;
        float panH = sh * 0.50f;
        float panX = (sw - panW) * 0.5f;
        float panY = sh * 0.22f;
        GUI.Box(new Rect(panX, panY, panW, panH), "Select Player 2", UIStyles.PanelBox);

        scrollViewVector = GUI.BeginScrollView(
            new Rect(panX + 10, panY + sh * 0.06f, panW - 20, sh * 0.30f),
            scrollViewVector, new Rect(0, 0, panW - 40, sh * 1.5f));
        if ((selGridInt = GUI.SelectionGrid(new Rect(0, 0, panW - 40, sh * 0.75f), selGridInt, returningPlayers, 1)) > 0)
            if (returningPlayers[selGridInt] == player1Name) selGridInt = -1;
        GUI.EndScrollView();

        if (selGridInt != -1)
        { player2Name = returningPlayers[selGridInt]; ReturningP2 = false; selGridInt = -1; StartScreen = true; IsPlayer2 = true; ResetAIFlags(); }

        float btnW = panW * 0.44f;
        if (GUI.Button(new Rect(panX + (panW - btnW) * 0.5f, panY + panH - sh * 0.08f, btnW, bh * 0.60f), "Back", UIStyles.SecondaryBtn))
        { ReturningP2 = false; StartScreen = true; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AI MENU
    // ─────────────────────────────────────────────────────────────────────────
    void AIMenu()
    {
        float panW = sw * 0.28f;
        float panH = sh * 0.38f;
        float panX = (sw - panW) * 0.5f;
        float panY = sh * 0.28f;
        float btnW = panW * 0.80f;
        float gap  = bh * 0.22f;

        GUI.Box(new Rect(panX, panY, panW, panH), "AI Difficulty", UIStyles.PanelBox);
        float y = panY + sh * 0.09f;

        if (GUI.Button(new Rect(panX + (panW - btnW) * 0.5f, y, btnW, bh * 0.65f), "Easy Bob",      UIStyles.PrimaryBtn))
        { AIDifficultyLevel = 0; player2Name = "Easy Bob";      StartScreen = true; AIScreen = false; }
        y += bh * 0.65f + gap;
        if (GUI.Button(new Rect(panX + (panW - btnW) * 0.5f, y, btnW, bh * 0.65f), "Cunning Clive", UIStyles.AccentBtn))
        { AIDifficultyLevel = 1; player2Name = "Cunning Clive"; StartScreen = true; AIScreen = false; }
        y += bh * 0.65f + gap;
        if (GUI.Button(new Rect(panX + (panW - btnW) * 0.5f, y, btnW, bh * 0.65f), "Master Shifu",  UIStyles.DangerBtn))
        { AIDifficultyLevel = 2; player2Name = "Master Shifu";  StartScreen = true; AIScreen = false; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // SCORES & ABOUT
    // ─────────────────────────────────────────────────────────────────────────
    void scoresMenu()
    {
        float cx = sw * 0.5f;
        GUI.Label(new Rect(cx - 180, sh * 0.06f, 360, 60), "Scores", UIStyles.TitleStyle);

        GUI.Box(new Rect(sw * 0.20f, sh * 0.18f, sw * 0.60f, sh * 0.62f), "", UIStyles.PanelBox);
        scrollViewVector = GUI.BeginScrollView(
            new Rect(sw * 0.22f, sh * 0.20f, sw * 0.56f, sh * 0.58f),
            scrollViewVector, new Rect(0, 0, sw * 0.50f, sh * 2.5f));
        GUI.Label(new Rect(0, 0, sw * 0.50f, sh * 2.5f), playerHistory, scoreLabelText);
        GUI.EndScrollView();

        if (UIStyles.CentreButton(cx, sh * 0.88f, bw * 0.70f, bh * 0.70f, "← Main Menu", UIStyles.SecondaryBtn))
        { ScoresScreen = false; MenuScreen = true; SceneManager.LoadScene("Menu"); }
    }

    void aboutMenu()
    {
        float cx = sw * 0.5f;
        GUI.Label(new Rect(cx - 180, sh * 0.06f, 360, 60), "About", UIStyles.TitleStyle);
        if (UIStyles.CentreButton(cx, sh * 0.88f, bw * 0.70f, bh * 0.70f, "← Main Menu", UIStyles.SecondaryBtn))
        { AboutScreen = false; MenuScreen = true; SceneManager.LoadScene("Menu"); }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────────────────────
    void InitializeStrings()
    {
        stringToEdit = ""; player1Name = ""; player2Name = ""; playerHistory = "";
    }

    void PopulateReturningPlayers()
    {
        int cr = History.GetCurrentRecords();
        returningPlayers = new string[cr];
        for (int i = 0; i < cr; i++)
            returningPlayers[i] = History.GetPlayerHistoryNames(i);
    }

    void InitializePlayerHistory()
    {
        History.PopulatePlayerHistory();
        PopulateReturningPlayers();
        playerHistory = History.GetPlayerHistoryEntry();
        playerHistory = scoresHeading + playerHistory;
    }

    void ResetAIFlags()
    {
        if (IsThereAI)    IsThereAI    = false;
        if (DoesAIGoFirst) DoesAIGoFirst = false;
    }

    void ResetMatchToMatchFlags()
    {
        MenuScreen = true; StartScreen = false; ScoresScreen = false;
        AboutScreen = false; NewP1Screen = false; NewP2Screen = false;
        AIScreen = false; IsThereAI = false; DoesP1GoFirst = false;
        DoesAIGoFirst = false; stringToEdit = "";
        player1Name = ""; player2Name = ""; playerHistory = "";
        AIDifficultyLevel = 0;
    }
}
