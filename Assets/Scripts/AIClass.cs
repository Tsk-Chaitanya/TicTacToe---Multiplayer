// AIClass written by Sean Cary, 2014
using UnityEngine;
using System.Collections;

public class AIClass : PlayerClass {
	#region Functions
	public AIClass() {}
	
	public AIClass (GameObject piece, char pShape) {
		gamePiece	= piece;
		shape		= pShape;
	}
	
	public override bool GetMove() {
		int		AIPieceCoordX, AIPieceCoordZ;
		float	AIPieceCoordY;
		System.Random ran = new System.Random();
		
		do {
			AIPieceCoordX = ran.Next (BoardArray.nAcross);
			AIPieceCoordY = Player.pieceHeightFromBoard;
			AIPieceCoordZ = ran.Next (BoardArray.nAcross);
			
			//StartCoroutine(WaitForAI());

			// Check if object is occupied by a game piece. If vacant, place piece of that player 
			gameBoardSpace = GameObject.Find("piece" + AIPieceCoordX + "," + AIPieceCoordZ);

			Vector3 boardLocation = new Vector3(AIPieceCoordX, AIPieceCoordY, AIPieceCoordZ);
			Debug.Log("Placing piece at (" + AIPieceCoordX + ", " + AIPieceCoordZ + ")");
			
			// Check if object is vacant. If vacant, place piece of that player
			if(gameBoardSpace.tag != "o_Occupied" && gameBoardSpace.tag != "x_Occupied" && !GameInfo.IsItADraw()) {

				// Create player's game piece in game space
				Instantiate(gamePiece, boardLocation, Quaternion.identity);

				// Tag player-clicked space as occupied
				if(shape == 'x') {
					gameBoardSpace.tag = "x_Occupied";
				}
				else if(shape == 'o') {
					gameBoardSpace.tag = "o_Occupied";
				}
				
				// Get a reference to the GameObject that was clicked
				BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent("BoardSpace");

				// Call the function of the clicked piece to update win state information
				thisSpace.UpdateSpaceState(shape);

				// Increment turn counter
				GameInfo.IncrementTurnCount();

				Player.CheckWinState();

				// Alternate player name UI colors to denote whose turn it is
				GameInfo.ChangeColor();
				return true;
			}
		} while(gameBoardSpace.tag == "x_Occupied" || gameBoardSpace.tag == "o_Occupied");
		return false;
	}
	#endregion
}
