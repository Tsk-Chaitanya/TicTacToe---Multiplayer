using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Manages Photon connection, room creation/joining with room codes.
/// Attach this to a persistent GameObject (e.g., "NetworkManager") in the Menu scene.
/// Mark it DontDestroyOnLoad so it persists across scene loads.
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static NetworkManager Instance { get; private set; }

    private const string TARGET_REGION = "asia";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Lock game version so both players are always on the same Photon virtual app.
        // PUN2 segregates players by GameVersion — mismatched versions can't see each other's rooms.
        PhotonNetwork.GameVersion = "1.0";
    }
    #endregion

    #region Variables
    // Room code for the current game
    public static string RoomCode { get; private set; }

    // Whether this client is the host (Player 1 / X)
    public static bool IsHost { get; private set; }

    // Connection state tracking
    public static bool IsConnectedToMaster { get; private set; }
    public static bool IsInRoom { get; private set; }
    public static bool IsRoomFull { get; private set; }

    // Status message for UI
    public static string StatusMessage { get; private set; }

    // Events for UI updates
    public delegate void NetworkEvent(string message);
    public static event NetworkEvent OnStatusChanged;
    public static event NetworkEvent OnRoomJoined;
    public static event NetworkEvent OnRoomFull;
    public static event NetworkEvent OnError;
    public static event NetworkEvent OnPlayerLeft;

    private const string ROOM_CODE_CHARS = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // No I,O,0,1 to avoid confusion
    private const int ROOM_CODE_LENGTH = 5;
    #endregion

    #region Photon Connection
    void Start()
    {
        StatusMessage = "Not connected";
        IsConnectedToMaster = false;
        IsInRoom = false;
        IsRoomFull = false;
    }

    /// <summary>
    /// Connect to Photon servers. Call this when entering the online lobby.
    /// Uses ConnectToRegion directly instead of ConnectUsingSettings to guarantee
    /// both players always land on the exact same Photon master server.
    /// </summary>
    public void ConnectToPhoton()
    {
        // If already connected, verify we're on the correct region
        if (PhotonNetwork.IsConnected)
        {
            string currentRegion = PhotonNetwork.CloudRegion ?? "";
            if (currentRegion.StartsWith(TARGET_REGION))
            {
                IsConnectedToMaster = true;
                SetStatus("Already connected [Region: " + currentRegion + "]");
                return;
            }
            // Wrong region — disconnect and reconnect to the right one
            Debug.LogWarning("[Network] Connected to wrong region: " + currentRegion + ", reconnecting to " + TARGET_REGION);
            _reconnecting = true;
            PhotonNetwork.Disconnect();
        }

        if (!_reconnecting)
        {
            SetStatus("Connecting to " + TARGET_REGION + " server...");
            PhotonNetwork.ConnectToRegion(TARGET_REGION);
        }
    }

    public override void OnConnectedToMaster()
    {
        IsConnectedToMaster = true;
        string region = PhotonNetwork.CloudRegion ?? "unknown";
        SetStatus("Connected! [Region: " + region + "]  Ready to create or join a room.");
        Debug.Log("[Network] Connected to Photon Master Server — Region: " + region);
    }

    private bool _reconnecting = false;

    public override void OnDisconnected(DisconnectCause cause)
    {
        IsConnectedToMaster = false;
        IsInRoom = false;
        IsRoomFull = false;

        // If we disconnected to switch regions, reconnect automatically
        if (_reconnecting)
        {
            _reconnecting = false;
            SetStatus("Reconnecting to " + TARGET_REGION + " server...");
            PhotonNetwork.ConnectToRegion(TARGET_REGION);
            return;
        }

        SetStatus("Disconnected: " + cause.ToString());
        Debug.LogWarning("Disconnected from Photon: " + cause);
    }
    #endregion

    #region Room Management
    /// <summary>
    /// Create a new room with a generated room code. The creator becomes Host (Player 1 / X).
    /// </summary>
    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SetError("Not connected to server. Please wait...");
            return;
        }

        RoomCode = GenerateRoomCode();
        IsHost = true;

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = false,  // Room is private, only joinable via code
            IsOpen = true
        };

        SetStatus("Creating room: " + RoomCode + "...");
        PhotonNetwork.CreateRoom(RoomCode, options);
    }

    /// <summary>
    /// Join an existing room using a room code. The joiner becomes Guest (Player 2 / O).
    /// </summary>
    public void JoinRoom(string code)
    {
        if (!PhotonNetwork.IsConnected)
        {
            SetError("Not connected to server. Please wait...");
            return;
        }

        if (string.IsNullOrEmpty(code) || code.Length != ROOM_CODE_LENGTH)
        {
            SetError("Invalid room code. Must be " + ROOM_CODE_LENGTH + " characters.");
            return;
        }

        RoomCode = code.ToUpper();
        IsHost = false;

        SetStatus("Joining room: " + RoomCode + "...");
        PhotonNetwork.JoinRoom(RoomCode);
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        IsInRoom = false;
        IsRoomFull = false;
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    #endregion

    #region Photon Room Callbacks
    public override void OnCreatedRoom()
    {
        Debug.Log("Room created: " + RoomCode);
        SetStatus("Room created! Code: " + RoomCode + "\nWaiting for opponent...");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
        // If room code collision, try again with new code
        if (returnCode == ErrorCode.GameIdAlreadyExists)
        {
            CreateRoom(); // Retry with new code
        }
        else
        {
            SetError("Failed to create room: " + message);
        }
    }

    public override void OnJoinedRoom()
    {
        IsInRoom = true;
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);

        if (IsHost)
        {
            SetStatus("Room: " + RoomCode + " - Waiting for opponent...");
            OnRoomJoined?.Invoke(RoomCode);
        }
        else
        {
            SetStatus("Joined room: " + RoomCode + " - Connected to host!");
            OnRoomJoined?.Invoke(RoomCode);
        }

        // Check if room is now full (2 players)
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            IsRoomFull = true;
            OnRoomFull?.Invoke("Room is full, starting game!");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        string region = PhotonNetwork.CloudRegion ?? "unknown";
        Debug.LogError("Join room failed (region: " + region + ", code: " + returnCode + "): " + message);
        if (returnCode == ErrorCode.GameDoesNotExist)
        {
            SetError("Room not found [" + region + "]. Check code & ask host to stay in lobby.");
        }
        else if (returnCode == ErrorCode.GameFull)
        {
            SetError("Room is full. This game already has 2 players.");
        }
        else
        {
            SetError("Failed to join room: " + message);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player joined: " + newPlayer.NickName);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            IsRoomFull = true;
            SetStatus("Opponent connected! Starting game...");
            OnRoomFull?.Invoke("Room is full, starting game!");
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player left: " + otherPlayer.NickName);
        IsRoomFull = false;
        SetStatus("Opponent disconnected!");
        OnPlayerLeft?.Invoke("Opponent disconnected!");
    }
    #endregion

    #region Utility
    private string GenerateRoomCode()
    {
        string code = "";
        for (int i = 0; i < ROOM_CODE_LENGTH; i++)
        {
            code += ROOM_CODE_CHARS[Random.Range(0, ROOM_CODE_CHARS.Length)];
        }
        return code;
    }

    private void SetStatus(string msg)
    {
        StatusMessage = msg;
        OnStatusChanged?.Invoke(msg);
        Debug.Log("[Network] " + msg);
    }

    private void SetError(string msg)
    {
        StatusMessage = "ERROR: " + msg;
        OnError?.Invoke(msg);
        Debug.LogError("[Network] " + msg);
    }

    /// <summary>
    /// Returns whether it's this client's turn based on host/guest status.
    /// Host is always Player 1 (X), Guest is always Player 2 (O).
    /// </summary>
    public static bool IsMyTurn(bool altTurns)
    {
        // altTurns true = Player 1's turn, false = Player 2's turn
        // Host = Player 1, Guest = Player 2
        return (IsHost && altTurns) || (!IsHost && !altTurns);
    }
    #endregion
}
