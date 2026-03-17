using UnityEngine;

/// <summary>
/// Shared pastel UI styles and rounded texture helpers for all game screens.
/// Call UIStyles.Initialize() in each screen's Start() before using any styles.
/// </summary>
public static class UIStyles
{
    // ─── Pastel Color Palette ───────────────────────────────────────────────
    public static readonly Color BgTop       = new Color(0.93f, 0.91f, 0.98f, 1f); // soft lavender
    public static readonly Color BgBottom    = new Color(0.88f, 0.96f, 0.93f, 1f); // soft mint
    public static readonly Color MintGreen   = new Color(0.71f, 0.92f, 0.84f, 1f); // #B5EAD7
    public static readonly Color Lavender    = new Color(0.78f, 0.80f, 0.92f, 1f); // #C7CEEA
    public static readonly Color Peach       = new Color(1.00f, 0.85f, 0.73f, 1f); // #FFD9BA
    public static readonly Color SoftRose    = new Color(1.00f, 0.80f, 0.82f, 1f); // #FFCCD1
    public static readonly Color SoftYellow  = new Color(1.00f, 0.96f, 0.73f, 1f); // #FFF5BA
    public static readonly Color SoftBlue    = new Color(0.68f, 0.85f, 0.95f, 1f); // #ADD8F0
    public static readonly Color PanelBg     = new Color(1.00f, 1.00f, 1.00f, 0.65f);
    public static readonly Color DarkText    = new Color(0.28f, 0.22f, 0.38f, 1f); // #473862
    public static readonly Color MedText     = new Color(0.45f, 0.38f, 0.55f, 1f); // #73618C

    // ─── Private texture cache ───────────────────────────────────────────────
    private static Texture2D _mintTex, _lavenderTex, _peachTex,
                              _roseTex, _yellowTex, _blueTex,
                              _panelTex, _bgTex, _hoverTex;
    private static bool _ready = false;

    // ─── Cached GUIStyles ────────────────────────────────────────────────────
    public static GUIStyle TitleStyle     { get; private set; }
    public static GUIStyle SubtitleStyle  { get; private set; }
    public static GUIStyle BodyStyle      { get; private set; }
    public static GUIStyle CodeStyle      { get; private set; }
    public static GUIStyle PrimaryBtn     { get; private set; }  // mint
    public static GUIStyle SecondaryBtn   { get; private set; }  // lavender
    public static GUIStyle AccentBtn      { get; private set; }  // peach
    public static GUIStyle DangerBtn      { get; private set; }  // rose
    public static GUIStyle SuccessBtn     { get; private set; }  // yellow
    public static GUIStyle BlueBtn        { get; private set; }  // soft blue
    public static GUIStyle PanelBox       { get; private set; }
    public static GUIStyle InputField     { get; private set; }

    // ─── Initialize (call from Start()) ─────────────────────────────────────
    public static void Initialize()
    {
        if (_ready) return;

        // Build textures
        _mintTex     = MakeRoundedTex(256, 72, 18, MintGreen);
        _lavenderTex = MakeRoundedTex(256, 72, 18, Lavender);
        _peachTex    = MakeRoundedTex(256, 72, 18, Peach);
        _roseTex     = MakeRoundedTex(256, 72, 18, SoftRose);
        _yellowTex   = MakeRoundedTex(256, 72, 18, SoftYellow);
        _blueTex     = MakeRoundedTex(256, 72, 18, SoftBlue);
        _panelTex    = MakeRoundedTex(256, 256, 22, PanelBg);
        _hoverTex    = MakeRoundedTex(256, 72, 18, new Color(1f, 1f, 1f, 0.35f));

        // ── Title ──
        TitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 42,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            wordWrap  = false
        };
        TitleStyle.normal.textColor = DarkText;

        // ── Subtitle ──
        SubtitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 20,
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleCenter,
            wordWrap  = true
        };
        SubtitleStyle.normal.textColor = MedText;

        // ── Body ──
        BodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 16,
            alignment = TextAnchor.MiddleCenter,
            wordWrap  = true
        };
        BodyStyle.normal.textColor = DarkText;

        // ── Code (big room code display) ──
        CodeStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize  = 52,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        CodeStyle.normal.textColor = new Color(0.40f, 0.25f, 0.65f, 1f);

        // ── Shared button base ──
        var btnBase = new GUIStyle(GUI.skin.button);
        btnBase.fontSize  = 18;
        btnBase.fontStyle = FontStyle.Bold;
        btnBase.alignment = TextAnchor.MiddleCenter;
        btnBase.border    = new RectOffset(12, 12, 12, 12);
        btnBase.padding   = new RectOffset(16, 16, 10, 10);
        btnBase.normal.textColor   = DarkText;
        btnBase.hover.textColor    = DarkText;
        btnBase.active.textColor   = DarkText;
        btnBase.focused.textColor  = DarkText;

        PrimaryBtn   = StyledBtn(btnBase, _mintTex);
        SecondaryBtn = StyledBtn(btnBase, _lavenderTex);
        AccentBtn    = StyledBtn(btnBase, _peachTex);
        DangerBtn    = StyledBtn(btnBase, _roseTex);
        SuccessBtn   = StyledBtn(btnBase, _yellowTex);
        BlueBtn      = StyledBtn(btnBase, _blueTex);

        // ── Panel box ──
        PanelBox = new GUIStyle(GUI.skin.box);
        PanelBox.normal.background = _panelTex;
        PanelBox.border = new RectOffset(22, 22, 22, 22);
        PanelBox.padding = new RectOffset(20, 20, 20, 20);
        PanelBox.normal.textColor = DarkText;
        PanelBox.fontSize = 16;
        PanelBox.fontStyle = FontStyle.Bold;

        // ── Input field ──
        InputField = new GUIStyle(GUI.skin.textField);
        InputField.fontSize = 20;
        InputField.alignment = TextAnchor.MiddleCenter;
        InputField.normal.textColor = DarkText;
        InputField.normal.background = MakeRoundedTex(256, 56, 12, new Color(1f, 1f, 1f, 0.88f));
        InputField.padding = new RectOffset(12, 12, 8, 8);

        _ready = true;
    }

    // ─── Full-screen soft gradient background ────────────────────────────────
    public static void DrawBackground()
    {
        // Top band
        GUI.color = BgTop;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height * 0.5f), Texture2D.whiteTexture);
        // Bottom band
        GUI.color = BgBottom;
        GUI.DrawTexture(new Rect(0, Screen.height * 0.5f, Screen.width, Screen.height * 0.5f), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    // ─── Helper: centred button shortcut ─────────────────────────────────────
    public static bool CentreButton(float centerX, float y, float w, float h,
                                    string label, GUIStyle style)
    {
        return GUI.Button(new Rect(centerX - w * 0.5f, y, w, h), label, style);
    }

    // ─── Texture factories ────────────────────────────────────────────────────
    private static GUIStyle StyledBtn(GUIStyle baseStyle, Texture2D tex)
    {
        var s = new GUIStyle(baseStyle);
        s.normal.background  = tex;
        s.hover.background   = tex;
        s.active.background  = tex;
        s.focused.background = tex;
        return s;
    }

    public static Texture2D MakeRoundedTex(int w, int h, int r, Color col)
    {
        var tex    = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var pixels = new Color[w * h];
        r = Mathf.Min(r, w / 2, h / 2);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                pixels[y * w + x] = InsideRounded(x, y, w, h, r) ? col : Color.clear;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }

    private static bool InsideRounded(int x, int y, int w, int h, int r)
    {
        // Corners
        if (x < r     && y < r)     return Dist(x, y, r,     r)     <= r;
        if (x >= w-r  && y < r)     return Dist(x, y, w-r-1, r)     <= r;
        if (x < r     && y >= h-r)  return Dist(x, y, r,     h-r-1) <= r;
        if (x >= w-r  && y >= h-r)  return Dist(x, y, w-r-1, h-r-1) <= r;
        return true;
    }

    private static float Dist(int x, int y, int cx, int cy)
        => Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
}
