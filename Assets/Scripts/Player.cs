using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour {

	#region Variables
	public GameObject 	X_Piece, // must be public to be assigned in the editor
						O_Piece; // must be public to be assigned in the editor
	static char 		p1Shape,
						p2Shape;
	static bool			AIflag,
						AITurnFirst,
						FirstAIturn,
						altTurns,
						gameWon;
	static int			AIlevel;
	static PlayerClass	P1,
						P2;
	static string 		player1Name,
						player2Name;
	static string[] 	AInames;
	public const int 	maxTurns 	= 25,
						numAInames 	= 3;
	public const float 	pieceHeightFromBoard = 0.1f;

	// ===== NETWORK ADDITIONS =====
	// Flag indicating if this is an online multiplayer game
	public static bool IsOnlineGame { get; private set; }

	// References to piece prefabs for network move application
	private static GameObject staticXPiece;
	private static GameObject staticOPiece;
	#endregion

	#region Functions
	public Player() {}

	// Original constructor for local/AI games
	public Player (string p1name, string p2name, bool AI, bool AIfirst, int AIlvl) {
		player1Name = p1name;
		player2Name = p2name;
		AIflag 		= AI;
		AITurnFirst = AIfirst;
		AIlevel 	= AIlvl;
		IsOnlineGame = false;
	}

	// New constructor for online multiplayer games
	public Player (string p1name, string p2name, bool isOnline) {
		player1Name = p1name;
		player2Name = p2name;
		AIflag 		= false;
		AITurnFirst = false;
		AIlevel 	= 0;
		IsOnlineGame = isOnline;
	}

	void Start () {
		AInames		= new string[numAInames] {"Easy Bob", "Cunning Clive", "Master Shifu"};
		gameWon		= false;
		p1Shape		= 'x';
		p2Shape		= 'o';
		altTurns	= true;		// True = P1, false = P2/AI

		// Store static references to prefabs for network move application
		staticXPiece = X_Piece;
		staticOPiece = O_Piece;

		CreateObjects();

		// Subscribe to network move events if online
		if (IsOnlineGame) {
			NetworkGameSync.OnRemoteMoveReceived += HandleRemoteMove;
		}
	}

	void OnDestroy() {
		// Unsubscribe from network events
		if (IsOnlineGame) {
			NetworkGameSync.OnRemoteMoveReceived -= HandleRemoteMove;
		}
	}

	// Update checks for state change every frame
	void Update ()
	{
		if (IsOnlineGame) {
			UpdateOnline();
		} else {
			UpdateLocal();
		}
	}

	/// <summary>
	/// Original local game update logic (unchanged).
	/// </summary>
	void UpdateLocal()
	{
		// Detects release of the left mouse button
		if(Input.GetMouseButtonUp(0))
		{
			if(altTurns)
			{
				if(P1.GetMove())
					altTurns = false;
			}
			else if (!altTurns)
			{
				if(P2.GetMove())
					altTurns = true;
			}
		}

		CheckWinConditions();

		/* This is how the AI takes its first move. It only occurs
		 * once per game, and only if the AI has been chosen to go first*/
		if(AITurnFirst)
		{
			AITurnFirst = false;

			if(P2.GetMove())
				altTurns = true;
		}
	}

	/// <summary>
	/// Online multiplayer update logic.
	/// Only allows input when it's THIS client's turn.
	/// </summary>
	void UpdateOnline()
	{
		// Check if it's this client's turn
		bool isMyTurn = NetworkManager.IsMyTurn(altTurns);

		// Only process local input if it's our turn
		if (isMyTurn && Input.GetMouseButtonUp(0))
		{
			// Determine which local player object to use
			PlayerClass localPlayer = NetworkManager.IsHost ? P1 : P2;

			if (localPlayer.GetMove())
			{
				// Move was successful locally, now send it to remote player
				int moveX = localPlayer.GetLastMoveX();
				int moveZ = localPlayer.GetLastMoveZ();
				char moveShape = NetworkManager.IsHost ? p1Shape : p2Shape;

				if (NetworkGameSync.Instance != null)
				{
					NetworkGameSync.Instance.SendMove(moveX, moveZ, moveShape);
				}

				// Toggle turn
				altTurns = !altTurns;
			}
		}

		// Process any pending remote moves
		if (NetworkGameSync.HasPendingRemoteMove)
		{
			// The remote move is handled by the event handler (HandleRemoteMove)
			// We just need to clear it here if it was already processed
		}

		CheckWinConditions();
	}

	/// <summary>
	/// Handles a move received from the remote player via network.
	/// </summary>
	void HandleRemoteMove(int boardX, int boardZ, char playerShape)
	{
		Debug.Log("[Player] Processing remote move at (" + boardX + ", " + boardZ + ") shape: " + playerShape);

		// Apply the remote move to the local board
		NetworkGameSync.ApplyRemoteMove(boardX, boardZ, playerShape, staticXPiece, staticOPiece);

		// Toggle turn
		altTurns = !altTurns;

		// Clear the pending move
		NetworkGameSync.ClearPendingMove();
	}

	void CreateObjects()
	{
		P1 = new PlayerClass(X_Piece, p1Shape);

		if (IsOnlineGame) {
			// In online mode, P2 is always a regular PlayerClass
			// (the remote player controls P2's moves via network)
			P2 = new PlayerClass(O_Piece, p2Shape);
		}
		else if(AIflag)
		{
			if(AIlevel == 0) {
				P2 = new AIClass(O_Piece, p2Shape);
			}
			else if (AIlevel == 1) {
				P2 = new AIMediumClass(O_Piece, p2Shape);
			}
			else if (AIlevel == 2) {
				P2 = new AIHardClass(O_Piece, p2Shape);
			}
		}
		else
			P2 = new PlayerClass(O_Piece, p2Shape);
	}

	public static bool IsAIFirst() {
		return AITurnFirst;
	}

	public static bool WhoseTurn() {
		return altTurns;
	}

	public static string GetPlayer1Name() {
		return player1Name;
	}

	public static string GetPlayer2Name() {
		if(AIflag) {
			return AInames[AIlevel]; }
		else {
			return player2Name; }
	}

	// Allow setting player 2 name (for when remote player name is received)
	public static void SetPlayer2Name(string name) {
		player2Name = name;
	}

	public static void CheckWinConditions() {
		if(gameWon || GameInfo.IsItADraw()) {
			if(GameInfo.IsItADraw()) {
				Debug.Log("No one wins!");
			}
			EndScreen.ExecuteHistorySystem();
			SceneManager.LoadScene("End"); // Load game over screen
		}
	}

	public static void CheckWinState()
	{
		// Checks both win state arrays for a win condition
		for(int i = 0; i < 28; i++) {
			if(BoardArray.GetWCSX(i) == 4) {
				gameWon = true;
				GameInfo.SetWinner(GetPlayer1Name());
				Debug.Log(player1Name + " Wins!");
				break;
			};
			if(BoardArray.GetWCSO(i) == 4) {
				gameWon = true;
				GameInfo.SetWinner(GetPlayer2Name());
				Debug.Log(player2Name + " Wins!");
				break;
			};
		};
	}

	/// <summary>
	/// Returns true if the game has been won.
	/// </summary>
	public static bool IsGameWon() {
		return gameWon;
	}
	#endregion
}
