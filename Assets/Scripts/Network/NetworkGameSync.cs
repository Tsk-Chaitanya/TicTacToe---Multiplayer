using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// Handles synchronization of game moves between networked players via Photon RPCs.
/// Attach this to the same persistent GameObject as NetworkManager, or to a
/// GameObject in the Gameplay scene that has a PhotonView component.
///
/// This script must have a PhotonView component on the same GameObject.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class NetworkGameSync : MonoBehaviourPun
{
    #region Singleton
    public static NetworkGameSync Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Variables
    // Event fired when a remote move is received
    public delegate void MoveReceivedHandler(int boardX, int boardZ, char playerShape);
    public static event MoveReceivedHandler OnRemoteMoveReceived;

    // Event fired when the remote player wants to start/restart
    public delegate void GameEventHandler();
    public static event GameEventHandler OnRemotePlayerReady;
    public static event GameEventHandler OnRemotePlayerLeft;
    public static event GameEventHandler OnRemoteRetryRequested;

    // Track if remote move has been received and needs processing
    public static bool HasPendingRemoteMove { get; private set; }
    public static int PendingMoveX { get; private set; }
    public static int PendingMoveZ { get; private set; }
    public static char PendingMoveShape { get; private set; }

    // Player names synced across network
    public static string HostPlayerName { get; private set; }
    public static string GuestPlayerName { get; private set; }
    #endregion

    #region Send Functions (called by local player)
    /// <summary>
    /// Send a move to the remote player. Called after local player places a piece.
    /// boardX and boardZ are the coordinates from the board space name "piece[X],[Z]"
    /// </summary>
    public void SendMove(int boardX, int boardZ, char playerShape)
    {
        Debug.Log("[NetworkSync] Sending move: (" + boardX + ", " + boardZ + ") shape: " + playerShape);
        photonView.RPC("RPC_ReceiveMove", RpcTarget.Others, boardX, boardZ, (int)playerShape);
    }

    /// <summary>
    /// Send player name to the other client.
    /// </summary>
    public void SendPlayerName(string playerName, bool isHost)
    {
        photonView.RPC("RPC_ReceivePlayerName", RpcTarget.Others, playerName, isHost);
    }

    /// <summary>
    /// Tell the other player we want to retry/rematch.
    /// </summary>
    public void SendRetryRequest()
    {
        photonView.RPC("RPC_RetryRequested", RpcTarget.Others);
    }

    /// <summary>
    /// Signal that this player is ready to start the game.
    /// </summary>
    public void SendPlayerReady()
    {
        photonView.RPC("RPC_PlayerReady", RpcTarget.Others);
    }

    /// <summary>
    /// Notify all clients to load the Gameplay scene (called by host only).
    /// </summary>
    public void SendStartGame()
    {
        if (NetworkManager.IsHost)
        {
            photonView.RPC("RPC_StartGame", RpcTarget.All);
        }
    }

    /// <summary>
    /// Notify remote player that the game ended.
    /// </summary>
    public void SendGameOver(string winnerName, bool isDraw)
    {
        photonView.RPC("RPC_GameOver", RpcTarget.Others, winnerName, isDraw);
    }
    #endregion

    #region RPCs (received from remote player)
    [PunRPC]
    void RPC_ReceiveMove(int boardX, int boardZ, int playerShapeInt)
    {
        char playerShape = (char)playerShapeInt;
        Debug.Log("[NetworkSync] Received move: (" + boardX + ", " + boardZ + ") shape: " + playerShape);

        // Store the pending move for the game to process
        HasPendingRemoteMove = true;
        PendingMoveX = boardX;
        PendingMoveZ = boardZ;
        PendingMoveShape = playerShape;

        // Fire event for listeners
        OnRemoteMoveReceived?.Invoke(boardX, boardZ, playerShape);
    }

    [PunRPC]
    void RPC_ReceivePlayerName(string playerName, bool isHost)
    {
        if (isHost)
        {
            HostPlayerName = playerName;
            Debug.Log("[NetworkSync] Host name received: " + playerName);
        }
        else
        {
            GuestPlayerName = playerName;
            Debug.Log("[NetworkSync] Guest name received: " + playerName);
        }
    }

    [PunRPC]
    void RPC_PlayerReady()
    {
        Debug.Log("[NetworkSync] Remote player is ready");
        OnRemotePlayerReady?.Invoke();
    }

    [PunRPC]
    void RPC_RetryRequested()
    {
        Debug.Log("[NetworkSync] Remote player wants a rematch");
        OnRemoteRetryRequested?.Invoke();
    }

    [PunRPC]
    void RPC_StartGame()
    {
        Debug.Log("[NetworkSync] Starting game!");
        SceneManager.LoadScene("Gameplay");
    }

    [PunRPC]
    void RPC_GameOver(string winnerName, bool isDraw)
    {
        Debug.Log("[NetworkSync] Game over! Winner: " + (isDraw ? "Draw" : winnerName));
    }
    #endregion

    #region Utility
    /// <summary>
    /// Clear the pending remote move after it has been processed.
    /// </summary>
    public static void ClearPendingMove()
    {
        HasPendingRemoteMove = false;
    }

    /// <summary>
    /// Apply a remote move to the board. Finds the board space and places the piece.
    /// Call this from the Player/Game controller when processing remote moves.
    /// </summary>
    public static void ApplyRemoteMove(int boardX, int boardZ, char shape, GameObject xPiece, GameObject oPiece)
    {
        // Find the board space by name
        string spaceName = "piece" + boardX + "," + boardZ;
        GameObject boardSpace = GameObject.Find(spaceName);

        if (boardSpace == null)
        {
            Debug.LogError("[NetworkSync] Could not find board space: " + spaceName);
            return;
        }

        if (boardSpace.tag != "Unoccupied")
        {
            Debug.LogWarning("[NetworkSync] Board space already occupied: " + spaceName);
            return;
        }

        // Determine which piece to place
        GameObject pieceToPlace = (shape == 'x') ? xPiece : oPiece;

        // Calculate position
        Vector3 boardLocation = new Vector3(
            boardSpace.transform.position.x,
            boardSpace.transform.position.y + Player.pieceHeightFromBoard,
            boardSpace.transform.position.z
        );

        // Instantiate the piece
        Object.Instantiate(pieceToPlace, boardLocation, Quaternion.identity);

        // Tag the space
        boardSpace.tag = (shape == 'x') ? "x_Occupied" : "o_Occupied";

        // Update win state
        BoardSpace thisSpace = boardSpace.GetComponent<BoardSpace>();
        if (thisSpace != null)
        {
            thisSpace.UpdateSpaceState(shape);
        }

        // Check for win
        Player.CheckWinState();

        // Update UI
        GameInfo.ChangeColor();
        GameInfo.IncrementTurnCount();

        Debug.Log("[NetworkSync] Remote move applied at " + spaceName);
    }
    #endregion
}
