using UnityEngine;
using System.Collections;

public class BoardArray : MonoBehaviour {

	#region Variables
	public const int 	nAcross		= 5,  // number of game board spaces across
					 	nDown		= 5,  // number of game board spaces down
					 	nWinStates	= 28; // number of posible win states
	// winCheckBoard keeps track of which win conditions apply to which coordinates
	static bool[,,]		winCheckBoard;
	// winCheckSumO/X keep track of each player's win conditions
	static int[]		winCheckSumX;
	static int[]		winCheckSumO;
	#endregion

	#region Functions
	void Start () {
		winCheckSumX	= new int[nWinStates];
		winCheckSumO	= new int[nWinStates];
		winCheckBoard	= new bool[nAcross, nDown, nWinStates];

		for(int x = 0; x < nAcross; x++) {
			for(int y = 0; y < nDown; y++) {
				for(int z = 0; z < nWinStates; z++)
					winCheckBoard[x, y, z] = false; } }

		winCheckBoard[0, 0, 0] 	= true; // 1,1
		winCheckBoard[0, 0, 10] = true;
		winCheckBoard[0, 0, 22] = true;
		winCheckBoard[0, 1, 1] 	= true; // 1,2
		winCheckBoard[0, 1, 10] = true;
		winCheckBoard[0, 1, 15] = true;
		winCheckBoard[0, 1, 21] = true;
		winCheckBoard[0, 2, 2] 	= true; // 1,3
		winCheckBoard[0, 2, 10] = true;
		winCheckBoard[0, 2, 15] = true;
		winCheckBoard[0, 3, 3] 	= true; // 1,4
		winCheckBoard[0, 3, 10] = true;
		winCheckBoard[0, 3, 15] = true;
		winCheckBoard[0, 3, 25] = true;
		winCheckBoard[0, 4, 4] 	= true; // 1,5
		winCheckBoard[0, 4, 15] = true;
		winCheckBoard[0, 4, 27] = true;
		winCheckBoard[1, 0, 0] 	= true; // 2,1
		winCheckBoard[1, 0, 5] 	= true;
		winCheckBoard[1, 0, 11] = true;
		winCheckBoard[1, 0, 20] = true;
		winCheckBoard[1, 1, 1] 	= true; // 2,2
		winCheckBoard[1, 1, 6] 	= true;
		winCheckBoard[1, 1, 11] = true;
		winCheckBoard[1, 1, 16] = true;
		winCheckBoard[1, 1, 22] = true;
		winCheckBoard[1, 1, 23] = true;
		winCheckBoard[1, 2, 2] 	= true; // 2,3
		winCheckBoard[1, 2, 7] 	= true;
		winCheckBoard[1, 2, 11] = true;
		winCheckBoard[1, 2, 16] = true;
		winCheckBoard[1, 2, 21] = true;
		winCheckBoard[1, 2, 25] = true;
		winCheckBoard[1, 3, 3] 	= true; // 2,4
		winCheckBoard[1, 3, 8] 	= true;
		winCheckBoard[1, 3, 11] = true;
		winCheckBoard[1, 3, 16] = true;
		winCheckBoard[1, 3, 26] = true;
		winCheckBoard[1, 3, 27] = true;
		winCheckBoard[1, 4, 4] 	= true; // 2,5
		winCheckBoard[1, 4, 9] 	= true;
		winCheckBoard[1, 4, 16] = true;
		winCheckBoard[1, 4, 24] = true;
		winCheckBoard[2, 0, 0] 	= true; // 3,1
		winCheckBoard[2, 0, 5] 	= true;
		winCheckBoard[2, 0, 12] = true;
		winCheckBoard[2, 1, 1] 	= true; // 3,2
		winCheckBoard[2, 1, 6] 	= true;
		winCheckBoard[2, 1, 12] = true;
		winCheckBoard[2, 1, 17] = true;
		winCheckBoard[2, 1, 20] = true;
		winCheckBoard[2, 1, 25] = true;
		winCheckBoard[2, 2, 2] 	= true; // 3,3
		winCheckBoard[2, 2, 7] 	= true;
		winCheckBoard[2, 2, 12] = true;
		winCheckBoard[2, 2, 17] = true;
		winCheckBoard[2, 2, 22] = true;
		winCheckBoard[2, 2, 23] = true;
		winCheckBoard[2, 2, 26] = true;
		winCheckBoard[2, 2, 27] = true;
		winCheckBoard[2, 3, 3] 	= true; // 3,4
		winCheckBoard[2, 3, 8] 	= true;
		winCheckBoard[2, 3, 12] = true;
		winCheckBoard[2, 3, 17] = true;
		winCheckBoard[2, 3, 21] = true;
		winCheckBoard[2, 3, 24] = true;
		winCheckBoard[2, 4, 4] 	= true; // 3,5
		winCheckBoard[2, 4, 9] 	= true;
		winCheckBoard[2, 4, 17] = true;
		winCheckBoard[3, 0, 0] 	= true; // 4,1
		winCheckBoard[3, 0, 5] 	= true;
		winCheckBoard[3, 0, 13] = true;
		winCheckBoard[3, 0, 25] = true;
		winCheckBoard[3, 1, 1] 	= true; // 4,2
		winCheckBoard[3, 1, 6] 	= true;
		winCheckBoard[3, 1, 13] = true;
		winCheckBoard[3, 1, 18] = true;
		winCheckBoard[3, 1, 26] = true;
		winCheckBoard[3, 1, 27] = true;
		winCheckBoard[3, 2, 2] 	= true; // 4,3
		winCheckBoard[3, 2, 7] 	= true;
		winCheckBoard[3, 2, 13] = true;
		winCheckBoard[3, 2, 18] = true;
		winCheckBoard[3, 2, 20] = true;
		winCheckBoard[3, 2, 24] = true;
		winCheckBoard[3, 3, 3] 	= true; // 4,4
		winCheckBoard[3, 3, 8] 	= true;
		winCheckBoard[3, 3, 13] = true;
		winCheckBoard[3, 3, 18] = true;
		winCheckBoard[3, 3, 22] = true;
		winCheckBoard[3, 3, 23] = true;
		winCheckBoard[3, 4, 4] 	= true; // 4,5
		winCheckBoard[3, 4, 9] 	= true;
		winCheckBoard[3, 4, 18] = true;
		winCheckBoard[3, 4, 21] = true;
		winCheckBoard[4, 0, 5] 	= true; // 5,1
		winCheckBoard[4, 0, 14] = true;
		winCheckBoard[4, 0, 26] = true;
		winCheckBoard[4, 1, 6] 	= true; // 5,2
		winCheckBoard[4, 1, 14] = true;
		winCheckBoard[4, 1, 19] = true;
		winCheckBoard[4, 1, 24] = true;
		winCheckBoard[4, 2, 7] 	= true; // 5,3
		winCheckBoard[4, 2, 14] = true;
		winCheckBoard[4, 2, 19] = true;
		winCheckBoard[4, 3, 8] 	= true; // 5,4
		winCheckBoard[4, 3, 14] = true;
		winCheckBoard[4, 3, 19] = true;
		winCheckBoard[4, 3, 20] = true;
		winCheckBoard[4, 4, 9] 	= true; // 5,5
		winCheckBoard[4, 4, 19] = true;
		winCheckBoard[4, 4, 23] = true;

		for(int j = 0; j < nWinStates; j++) {
			winCheckSumX[j] = 0;
			winCheckSumO[j] = 0; };

		Debug.Log("Board Initialized");
	}

	public static void IncrementWCSX(int x) {
		winCheckSumX[x]++;
	}

	public static void IncrementWCSO(int x) {
		winCheckSumO[x]++;
	}

	public static int GetWCSX(int x) {
		return winCheckSumX[x];
	}

	public static int GetWCSO(int x) {
		return winCheckSumO[x];
	}

	public static bool GetWCB(int x, int y, int z) {
		return winCheckBoard[x, y, z];
	}
	#endregion
}