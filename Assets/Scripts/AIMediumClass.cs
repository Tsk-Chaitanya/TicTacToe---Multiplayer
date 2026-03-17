// AIClass written by Sean Cary, 2014
using System;
using UnityEngine;

public class AIMediumClass : AIClass {

	public AIMediumClass () {}

	public AIMediumClass (GameObject piece, char pShape) {
		gamePiece = piece;
		shape = pShape;
		if (shape == 'o')
			opponentShape = 'x';
		else if (shape == 'x')
			opponentShape = 'o';
	}

	public override bool GetMove() {
		int tempX, tempZ;
		float tempY;
		Vector3 boardLocation;
		Vector3 boardLocation2;
		Vector3 nullVector = new Vector3 (-1, 0.1f, -1);

		convertBoardToArray ();

		boardLocation = canIWin ();
		boardLocation2 = canIBlock ();

		System.Random ran = new System.Random ();
		if (boardLocation == nullVector && boardLocation2 == nullVector) {
			do {
				tempX = ran.Next (5);
				tempY = 0.1f;
				tempZ = ran.Next (5);

				// Gets the location on the board that the piece 
				gameBoardSpace = GameObject.Find ("piece" + tempX + "," + tempZ);
				boardLocation = new Vector3 (tempX, tempY, tempZ);
				Debug.Log ("Placing piece at (" + tempX + ", " + tempZ + ")");
	
				// Check if object is vacant. If vacant, place piece of that player
				if (gameBoardSpace.tag != "x_Occupied" && gameBoardSpace.tag != "o_Occupied" && !GameInfo.IsItADraw()) {
					Instantiate (gamePiece, boardLocation, Quaternion.identity);
					gameBoardSpace.tag = shape + "_Occupied";
					BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent ("BoardSpace");
					thisSpace.UpdateSpaceState (shape);
					GameInfo.IncrementTurnCount();
					Player.CheckWinState();
					GameInfo.ChangeColor();
					return true;
				}
			} while(gameBoardSpace.tag == "x_Occupied" || gameBoardSpace.tag == "o_Occupied");
		}
		else if (boardLocation != nullVector) {
			Instantiate (gamePiece, boardLocation, Quaternion.identity);
			gameBoardSpace = GameObject.Find ("piece" + (int)boardLocation.x + "," + (int)boardLocation.z);
			gameBoardSpace.tag = shape + "_Occupied";
			BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent ("BoardSpace");
			thisSpace.UpdateSpaceState (shape);
			GameInfo.IncrementTurnCount();
			Player.CheckWinState();
			GameInfo.ChangeColor();
			return true;
		}
		else if (boardLocation2 != nullVector) {
			Instantiate (gamePiece, boardLocation2, Quaternion.identity);
			gameBoardSpace = GameObject.Find ("piece" + (int)boardLocation2.x + "," + (int)boardLocation2.z);
			gameBoardSpace.tag = shape + "_Occupied";
			BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent ("BoardSpace");
			thisSpace.UpdateSpaceState (shape);
			GameInfo.IncrementTurnCount();
			Player.CheckWinState();
			GameInfo.ChangeColor();
			return true;
		}
		return false;
	}

	public virtual int convertBoardToArray() {
		for(int n = 0; n < 5; n++) {
			for(int b = 0; b < 5; b++) {
				gameBoardSpace = GameObject.Find("piece" + n + "," + b);
				if(gameBoardSpace.tag == "x_Occupied")
					theBoard[n,b] = 'x';
				else if(gameBoardSpace.tag == "o_Occupied")
					theBoard[n,b] = 'o';
				else
					theBoard[n,b] = '-';
			}
		}
		return 0;
	}

	public Vector3 canIWin() {
		Vector3 myVector = new Vector3 (-1, 0.1f, -1);
		int tempX, tempZ;

		#region VertsAndHoriz
		for(int lc = 0; lc < 5; lc++) {
			for(int inn = 0; inn < 2; inn++) {
				if(theBoard[lc, 0 + inn] == shape && theBoard[lc, 1 + inn] == shape && theBoard[lc, 2 + inn] == shape && theBoard[lc, 3 + inn] == '-')
				{
					tempX = lc;
					tempZ = 3 + inn;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[lc, 0 + inn] == shape && theBoard[lc, 1 + inn] == shape && theBoard[lc, 3 + inn] == shape && theBoard[lc, 2 + inn] == '-')
				{
					tempX = lc;
					tempZ = 2 + inn;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[lc, 0 + inn] == shape && theBoard[lc, 2 + inn] == shape && theBoard[lc, 3 + inn] == shape && theBoard[lc, 1 + inn] == '-')
				{
					tempX = lc;
					tempZ = 1 + inn;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[lc, 1 + inn] == shape && theBoard[lc, 2 + inn] == shape && theBoard[lc, 3 + inn] == shape && theBoard[lc, 0 + inn] == '-')
				{
					tempX = lc;
					tempZ = 0 + inn;
					return new Vector3(tempX, 0.1f, tempZ);
				}

				////////////////////////////////////////////////////////////////////////////////////////////////////
				if(theBoard[0 + inn, lc] == shape && theBoard[1 + inn, lc] == shape && theBoard[2 + inn, lc] == shape && theBoard[3 + inn, lc] == '-')
				{
					tempX = 3 + inn;
					tempZ = lc;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[0 + inn, lc] == shape && theBoard[1 + inn, lc] == shape && theBoard[3 + inn, lc] == shape && theBoard[2 + inn, lc] == '-')
				{
					tempX = 2 + inn;
					tempZ = lc;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[0 + inn, lc] == shape && theBoard[2 + inn, lc] == shape && theBoard[3 + inn, lc] == shape && theBoard[1 + inn, lc] == '-')
				{
					tempX = 1 + inn;
					tempZ = lc;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[1 + inn, lc] == shape && theBoard[2 + inn, lc] == shape && theBoard[3 + inn, lc] == shape && theBoard[0 + inn, lc] == '-')
				{
					tempX = 0 + inn;
					tempZ = lc;
					return new Vector3(tempX, 0.1f, tempZ);
				}
			}
		}
		#endregion

		#region diagonals
		//Main Diagonal
		for(int inn = 0; inn < 2; inn++)
		{
			//Bottom left to top right
			if(theBoard[0 + inn, 0 + inn] == shape && theBoard[1 + inn, 1 + inn] == shape && theBoard[2 + inn, 2 + inn] == shape && theBoard[3 + inn, 3 + inn] == '-')
			{
				tempX = 3 + inn;
				tempZ = 3 + inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[0 + inn, 0 + inn] == shape && theBoard[1 + inn, 1 + inn] == shape && theBoard[3 + inn, 3 + inn] == shape && theBoard[2 + inn, 2 + inn] == '-')
			{
				tempX = 2 + inn;
				tempZ = 2 + inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[0 + inn, 0 + inn] == shape && theBoard[2 + inn, 2 + inn] == shape && theBoard[3 + inn, 3 + inn] == shape && theBoard[1 + inn, 1 + inn] == '-')
			{
				tempX = 1 + inn;
				tempZ = 1 + inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[1 + inn, 1 + inn] == shape && theBoard[2 + inn, 2 + inn] == shape && theBoard[3 + inn, 3 + inn] == shape && theBoard[0 + inn, 0 + inn] == '-')
			{
				tempX = 0 + inn;
				tempZ = 0 + inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			//Top left to bottom right
			if(theBoard[0 + inn, 4 - inn] == shape && theBoard[1 + inn, 3 - inn] == shape && theBoard[2 + inn, 2 - inn] == shape && theBoard[3 + inn, 1 - inn] == '-')
			{
				tempX = 3 + inn;
				tempZ = 1 - inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[0 + inn, 4 - inn] == shape && theBoard[1 + inn, 3 - inn] == shape && theBoard[3 + inn, 1 - inn] == shape && theBoard[2 + inn, 2 - inn] == '-')
			{
				tempX = 2 + inn;
				tempZ = 2 - inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[0 + inn, 4 - inn] == shape && theBoard[2 + inn, 2 - inn] == shape && theBoard[3 + inn, 1 - inn] == shape && theBoard[1 + inn, 3 - inn] == '-')
			{
				tempX = 1 + inn;
				tempZ = 3 - inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[1 + inn, 3 - inn] == shape && theBoard[2 + inn, 2 - inn] == shape && theBoard[3 + inn, 1 - inn] == shape && theBoard[0 + inn, 4 - inn] == '-')
			{
				tempX = 0 + inn;
				tempZ = 4 - inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
		}
		//Diag 1
		if(theBoard[0,1] == shape && theBoard[1,2] == shape && theBoard[2,3] == shape && theBoard[3,4] == '-')
		{
			tempX = 3;
			tempZ = 4;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[0,1] == shape && theBoard[1,2] == shape && theBoard[3,4] == shape && theBoard[2,3] == '-')
		{
			tempX = 2;
			tempZ = 3;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[0,1] == shape && theBoard[2,3] == shape && theBoard[3,4] == shape && theBoard[1,2] == '-')
		{
			tempX = 1;
			tempZ = 2;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[1,2] == shape && theBoard[2,3] == shape && theBoard[3,4] == shape && theBoard[0,1] == '-')
		{
			tempX = 0;
			tempZ = 1;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		// Diag 2
		if(theBoard[1,0] == shape && theBoard[2,1] == shape && theBoard[3,2] == shape && theBoard[4,3] == '-')
		{
			tempX = 4;
			tempZ = 3;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[1,0] == shape && theBoard[2,1] == shape && theBoard[4,3] == shape && theBoard[3,2] == '-')
		{
			tempX = 3;
			tempZ = 2;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[1,0] == shape && theBoard[3,2] == shape && theBoard[4,3] == shape && theBoard[2,1] == '-')
		{
			tempX = 2;
			tempZ = 1;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[2,1] == shape && theBoard[3,2] == shape && theBoard[4,3] == shape && theBoard[1,0] == '-')
		{
			tempX = 1;
			tempZ = 0;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		//Diag 3
		
		if(theBoard[4,1] == shape && theBoard[3,2] == shape && theBoard[2,3] == shape && theBoard[1,4] == '-')
		{
			tempX = 1;
			tempZ = 4;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[4,1] == shape && theBoard[3,2] == shape && theBoard[1,4] == shape && theBoard[2,3] == '-')
		{
			tempX = 2;
			tempZ = 3;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[4,1] == shape && theBoard[2,3] == shape && theBoard[1,4] == shape && theBoard[3,2] == '-')
		{
			tempX = 3;
			tempZ = 2;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[3,2] == shape && theBoard[2,3] == shape && theBoard[1,4] == shape && theBoard[4,1] == '-')
		{
			tempX = 4;
			tempZ = 1;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		
		// Diag 4
		if(theBoard[3,0] == shape && theBoard[2,1] == shape && theBoard[1,2] == shape && theBoard[0,3] == '-')
		{
			tempX = 0;
			tempZ = 3;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[3,0] == shape && theBoard[2,1] == shape && theBoard[0,3] == shape && theBoard[1,2] == '-')
		{
			tempX = 1;
			tempZ = 2;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[3,0] == shape && theBoard[1,2] == shape && theBoard[0,3] == shape && theBoard[2,1] == '-')
		{
			tempX = 2;
			tempZ = 1;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[2,1] == shape && theBoard[1,2] == shape && theBoard[0,3] == shape && theBoard[3,0] == '-')
		{
			tempX = 3;
			tempZ = 0;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		#endregion

		return myVector;
	}

	public Vector3 canIBlock()
	{
		Vector3 myVector = new Vector3 (-1, 0.1f, -1);
		int tempX, tempZ;
		
		#region VertsAndHoriz
		for(int lc = 0; lc < 5; lc++)
		{
			for(int inn = 0; inn < 2; inn++)
			{
				if(theBoard[lc, 0 + inn] == opponentShape && theBoard[lc, 1 + inn] == opponentShape && theBoard[lc, 2 + inn] == opponentShape && theBoard[lc, 3 + inn] == '-')
				{
					tempX = lc;
					tempZ = 3 + inn;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[lc, 0 + inn] == opponentShape && theBoard[lc, 1 + inn] == opponentShape && theBoard[lc, 3 + inn] == opponentShape && theBoard[lc, 2 + inn] == '-')
				{
					tempX = lc;
					tempZ = 2 + inn;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[lc, 0 + inn] == opponentShape && theBoard[lc, 2 + inn] == opponentShape && theBoard[lc, 3 + inn] == opponentShape && theBoard[lc, 1 + inn] == '-')
				{
					tempX = lc;
					tempZ = 1 + inn;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[lc, 1 + inn] == opponentShape && theBoard[lc, 2 + inn] == opponentShape && theBoard[lc, 3 + inn] == opponentShape && theBoard[lc, 0 + inn] == '-')
				{
					tempX = lc;
					tempZ = 0 + inn;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				
				////////////////////////////////////////////////////////////////////////////////////////////////////
				if(theBoard[0 + inn, lc] == opponentShape && theBoard[1 + inn, lc] == opponentShape && theBoard[2 + inn, lc] == opponentShape && theBoard[3 + inn, lc] == '-')
				{
					tempX = 3 + inn;
					tempZ = lc;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[0 + inn, lc] == opponentShape && theBoard[1 + inn, lc] == opponentShape && theBoard[3 + inn, lc] == opponentShape && theBoard[2 + inn, lc] == '-')
				{
					tempX = 2 + inn;
					tempZ = lc;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[0 + inn, lc] == opponentShape && theBoard[2 + inn, lc] == opponentShape && theBoard[3 + inn, lc] == opponentShape && theBoard[1 + inn, lc] == '-')
				{
					tempX = 1 + inn;
					tempZ = lc;
					return new Vector3(tempX, 0.1f, tempZ);
				}
				if(theBoard[1 + inn, lc] == opponentShape && theBoard[2 + inn, lc] == opponentShape && theBoard[3 + inn, lc] == opponentShape && theBoard[0 + inn, lc] == '-')
				{
					tempX = 0 + inn;
					tempZ = lc;
					return new Vector3(tempX, 0.1f, tempZ);
				}
			}
		}
		#endregion
		
		#region diagonals
		//Loop through the main diagonal
		for(int inn = 0; inn < 2; inn++)
		{
			//Bottom left to top right
			if(theBoard[0 + inn, 0 + inn] == opponentShape && theBoard[1 + inn, 1 + inn] == opponentShape && theBoard[2 + inn, 2 + inn] == opponentShape && theBoard[3 + inn, 3 + inn] == '-')
			{
				tempX = 3 + inn;
				tempZ = 3 + inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[0 + inn, 0 + inn] == opponentShape && theBoard[1 + inn, 1 + inn] == opponentShape && theBoard[3 + inn, 3 + inn] == opponentShape && theBoard[2 + inn, 2 + inn] == '-')
			{
				tempX = 2 + inn;
				tempZ = 2 + inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[0 + inn, 0 + inn] == opponentShape && theBoard[2 + inn, 2 + inn] == opponentShape && theBoard[3 + inn, 3 + inn] == opponentShape && theBoard[1 + inn, 1 + inn] == '-')
			{
				tempX = 1 + inn;
				tempZ = 1 + inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[1 + inn, 1 + inn] == opponentShape && theBoard[2 + inn, 2 + inn] == opponentShape && theBoard[3 + inn, 3 + inn] == opponentShape && theBoard[0 + inn, 0 + inn] == '-')
			{
				tempX = 0 + inn;
				tempZ = 0 + inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			//Top left to bottom right
			if(theBoard[0 + inn, 4 - inn] == opponentShape && theBoard[1 + inn, 3 - inn] == opponentShape && theBoard[2 + inn, 2 - inn] == opponentShape && theBoard[3 + inn, 1 - inn] == '-')
			{
				tempX = 3 + inn;
				tempZ = 1 - inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[0 + inn, 4 - inn] == opponentShape && theBoard[1 + inn, 3 - inn] == opponentShape && theBoard[3 + inn, 1 - inn] == opponentShape && theBoard[2 + inn, 2 - inn] == '-')
			{
				tempX = 2 + inn;
				tempZ = 2 - inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[0 + inn, 4 - inn] == opponentShape && theBoard[2 + inn, 2 - inn] == opponentShape && theBoard[3 + inn, 1 - inn] == opponentShape && theBoard[1 + inn, 3 - inn] == '-')
			{
				tempX = 1 + inn;
				tempZ = 3 - inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
			if(theBoard[1 + inn, 3 - inn] == opponentShape && theBoard[2 + inn, 2 - inn] == opponentShape && theBoard[3 + inn, 1 - inn] == opponentShape && theBoard[0 + inn, 4 - inn] == '-')
			{
				tempX = 0 + inn;
				tempZ = 4 - inn;
				return new Vector3(tempX, 0.1f, tempZ);
			}
		}
		//Check through short diagonals
		//Diag 1
		if(theBoard[0,1] == opponentShape && theBoard[1,2] == opponentShape && theBoard[2,3] == opponentShape && theBoard[3,4] == '-')
		{
			tempX = 3;
			tempZ = 4;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[0,1] == opponentShape && theBoard[1,2] == opponentShape && theBoard[3,4] == opponentShape && theBoard[2,3] == '-')
		{
			tempX = 2;
			tempZ = 3;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[0,1] == opponentShape && theBoard[2,3] == opponentShape && theBoard[3,4] == opponentShape && theBoard[1,2] == '-')
		{
			tempX = 1;
			tempZ = 2;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[1,2] == opponentShape && theBoard[2,3] == opponentShape && theBoard[3,4] == opponentShape && theBoard[0,1] == '-')
		{
			tempX = 0;
			tempZ = 1;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		//Diag 2
		if(theBoard[1,0] == opponentShape && theBoard[2,1] == opponentShape && theBoard[3,2] == opponentShape && theBoard[4,3] == '-')
		{
			tempX = 4;
			tempZ = 3;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[1,0] == opponentShape && theBoard[2,1] == opponentShape && theBoard[4,3] == opponentShape && theBoard[3,2] == '-')
		{
			tempX = 3;
			tempZ = 2;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[1,0] == opponentShape && theBoard[3,2] == opponentShape && theBoard[4,3] == opponentShape && theBoard[2,1] == '-')
		{
			tempX = 2;
			tempZ = 1;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[2,1] == opponentShape && theBoard[3,2] == opponentShape && theBoard[4,3] == opponentShape && theBoard[1,0] == '-')
		{
			tempX = 1;
			tempZ = 0;
			return new Vector3(tempX, 0.1f, tempZ);
		}

		//Diag 3

		if(theBoard[4,1] == opponentShape && theBoard[3,2] == opponentShape && theBoard[2,3] == opponentShape && theBoard[1,4] == '-')
		{
			tempX = 1;
			tempZ = 4;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[4,1] == opponentShape && theBoard[3,2] == opponentShape && theBoard[1,4] == opponentShape && theBoard[2,3] == '-')
		{
			tempX = 2;
			tempZ = 3;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[4,1] == opponentShape && theBoard[2,3] == opponentShape && theBoard[1,4] == opponentShape && theBoard[3,2] == '-')
		{
			tempX = 3;
			tempZ = 2;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[3,2] == opponentShape && theBoard[2,3] == opponentShape && theBoard[1,4] == opponentShape && theBoard[4,1] == '-')
		{
			tempX = 4;
			tempZ = 1;
			return new Vector3(tempX, 0.1f, tempZ);
		}

		// Diag 4
		if(theBoard[3,0] == opponentShape && theBoard[2,1] == opponentShape && theBoard[1,2] == opponentShape && theBoard[0,3] == '-')
		{
			tempX = 0;
			tempZ = 3;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[3,0] == opponentShape && theBoard[2,1] == opponentShape && theBoard[0,3] == opponentShape && theBoard[1,2] == '-')
		{
			tempX = 1;
			tempZ = 2;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[3,0] == opponentShape && theBoard[1,2] == opponentShape && theBoard[0,3] == opponentShape && theBoard[2,1] == '-')
		{
			tempX = 2;
			tempZ = 1;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		if(theBoard[2,1] == opponentShape && theBoard[1,2] == opponentShape && theBoard[0,3] == opponentShape && theBoard[3,0] == '-')
		{
			tempX = 3;
			tempZ = 0;
			return new Vector3(tempX, 0.1f, tempZ);
		}
		#endregion
		
		return myVector;
	}
}

