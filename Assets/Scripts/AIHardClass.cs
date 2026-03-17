// AIHardClass written by Sean Cary, 2014
using System;
using UnityEngine;

public class AIHardClass : AIMediumClass
{
	int[,] weightedMap = new int[,]{ 	{3, 4, 3 ,4, 3},
										{4, 6, 6, 6, 4},
										{3, 6, 8, 6, 3},
										{4, 6, 6, 6, 4},
										{3, 4, 3, 4, 3}};
	public AIHardClass () {}

	public AIHardClass (GameObject piece, char pShape) {
		gamePiece = piece;
		shape = pShape;
		if (shape == 'o')
			opponentShape = 'x';
		else if (shape == 'x')
			opponentShape = 'o';
	}

	public override bool GetMove()
	{
		int tempX, tempZ;
		float tempY;
		Vector3 boardLocation;
		Vector3 boardLocation2;
		Vector3 boardLocation3;
		Vector3 boardLocation4;
		Vector3 boardLocation5;
		Vector3 boardLocation6;
		Vector3 boardLocation7;

		Vector3 nullVector = new Vector3 (-1, 0.1f, -1);
		
		convertBoardToArray ();
		
		boardLocation = canIWin ();
		boardLocation2 = canIBlock ();
		boardLocation3 = block2InARow ();
		boardLocation4 = block2InARowEdges ();
		boardLocation5 = blockSean();
		boardLocation6 = takeEdges ();
		boardLocation7 = findHighestWeight ();

		System.Random ran = new System.Random ();
		if (boardLocation == nullVector && boardLocation2 == nullVector &&
		    boardLocation3 == nullVector && boardLocation4 == nullVector &&
		    boardLocation5 == nullVector && boardLocation6 == nullVector &&
		    boardLocation7 == nullVector) {
			do {
				tempX = ran.Next (5);
				tempY = 0.1f;
				tempZ = ran.Next (5);
				
				// Gets the location on the board that the piece 
				
				gameBoardSpace = GameObject.Find ("piece" + tempX + "," + tempZ);
				boardLocation = new Vector3 (tempX, tempY, tempZ);
				Debug.Log ("Placing piece at (" + tempX + ", " + tempZ + ")");
				
				// Check if object is vacant. If vacant, place piece of that player
				if (gameBoardSpace.tag != "x_Occupied" && gameBoardSpace.tag != "o_Occupied") {
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
				} else if (boardLocation != nullVector) { //If the ai can win...
						Instantiate (gamePiece, boardLocation, Quaternion.identity);
						gameBoardSpace = GameObject.Find ("piece" + (int)boardLocation.x + "," + (int)boardLocation.z);
						gameBoardSpace.tag = shape + "_Occupied";
						BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent ("BoardSpace");
						thisSpace.UpdateSpaceState (shape);
						GameInfo.IncrementTurnCount();
						Player.CheckWinState();
						GameInfo.ChangeColor();
						return true;
				} else if (boardLocation2 != nullVector) { //If the ai can block a win
						Instantiate (gamePiece, boardLocation2, Quaternion.identity);
						gameBoardSpace = GameObject.Find ("piece" + (int)boardLocation2.x + "," + (int)boardLocation2.z);
						gameBoardSpace.tag = shape + "_Occupied";
						BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent ("BoardSpace");
						thisSpace.UpdateSpaceState (shape);
						GameInfo.IncrementTurnCount();
						Player.CheckWinState();
						GameInfo.ChangeColor();
						return true;
				} else if (boardLocation3 != nullVector) { ///If the ai can block 2 in a row in the inner 9
						Instantiate (gamePiece, boardLocation3, Quaternion.identity);
						gameBoardSpace = GameObject.Find ("piece" + (int)boardLocation3.x + "," + (int)boardLocation3.z);
						gameBoardSpace.tag = shape + "_Occupied";
						BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent ("BoardSpace");
						thisSpace.UpdateSpaceState (shape);
						GameInfo.IncrementTurnCount();
						Player.CheckWinState();
						GameInfo.ChangeColor();
						return true;
				} else if (boardLocation4 != nullVector) { ///If the ai can block someone from setting up on edges
						Instantiate (gamePiece, boardLocation4, Quaternion.identity);
						gameBoardSpace = GameObject.Find ("piece" + (int)boardLocation4.x + "," + (int)boardLocation4.z);
						gameBoardSpace.tag = shape + "_Occupied";
						BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent ("BoardSpace");
						thisSpace.UpdateSpaceState (shape);
						GameInfo.IncrementTurnCount();
						Player.CheckWinState();
						GameInfo.ChangeColor();
						return true;
				} else if (boardLocation5 != nullVector) { ///If the ai can take a high value point (this should always happen when the above dont)
						Instantiate (gamePiece, boardLocation5, Quaternion.identity);
						gameBoardSpace = GameObject.Find ("piece" + (int)boardLocation5.x + "," + (int)boardLocation5.z);
						gameBoardSpace.tag = shape + "_Occupied";
						BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent ("BoardSpace");
						thisSpace.UpdateSpaceState (shape);
						GameInfo.IncrementTurnCount();
						Player.CheckWinState();
						GameInfo.ChangeColor();
						return true;
				} else if (boardLocation6 != nullVector) { ///If the ai can take a high value point (this should always happen when the above dont)
					Instantiate (gamePiece, boardLocation6, Quaternion.identity);
					gameBoardSpace = GameObject.Find ("piece" + (int)boardLocation6.x + "," + (int)boardLocation6.z);
					gameBoardSpace.tag = shape + "_Occupied";
					BoardSpace thisSpace = (BoardSpace)gameBoardSpace.GetComponent ("BoardSpace");
					thisSpace.UpdateSpaceState (shape);
					GameInfo.IncrementTurnCount();
					Player.CheckWinState();
					GameInfo.ChangeColor();
					return true;
				} else if (boardLocation7 != nullVector) { ///If the ai can take a high value point (this should always happen when the above dont)
					Instantiate (gamePiece, boardLocation7, Quaternion.identity);
					gameBoardSpace = GameObject.Find ("piece" + (int)boardLocation7.x + "," + (int)boardLocation7.z);
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
	
	public override int convertBoardToArray()
	{
		for(int n = 0; n < 5; n++)
		{
			for(int b = 0; b < 5; b++)
			{
				gameBoardSpace = GameObject.Find("piece" + n + "," + b);
				if(gameBoardSpace.tag == "x_Occupied")
				{
					theBoard[n,b] = 'x';
					weightedMap[n,b] = 0;
				}
				else if(gameBoardSpace.tag == "o_Occupied")
				{
					theBoard[n,b] = 'o';
					weightedMap[n,b] = 0;
				}
				else
					theBoard[n,b] = '-';
			}
		}
		return 0;
	}

	public Vector3 findHighestWeight()
	{
		int highest = 0;
		int highestX = -1;
		int highestZ = -1;
		Vector3 highestVector;

		for(int n = 0; n < 5; n++)
		{
			for(int b = 0; b < 5; b++)
			{
				if(weightedMap[n,b] > highest)
				{
					highest = weightedMap[n,b];
					highestX = n;
					highestZ = b;
				}
			}
		}

		highestVector = new Vector3 (highestX, 0.1f, highestZ);
		return highestVector;
	}

	public Vector3 block2InARow()
	{
		int x = -1;
		int z = -1;
		Vector3 blockingVector;

		for (int n = 1; n < 4; n++)
		{

			if (theBoard[n, 1] == opponentShape && theBoard[n, 2] == opponentShape && theBoard[n, 3] == '-')
			{
				x = n;
				z = 3;
			}
			else if (theBoard[n, 2] == opponentShape && theBoard[n, 3] == opponentShape && theBoard[n, 1] == '-')
			{
				x = n;
				z = 1;
			}
			else if(theBoard[n, 1] == opponentShape && theBoard[n, 3] == opponentShape && theBoard[n, 2] == '-')
			{
				x = n;
				z = 2;
			}
			
			else if (theBoard[1, n] == opponentShape && theBoard[2, n] == opponentShape && theBoard[3, n] == '-')
			{
				x = 3;
				z = n;
			}
			else if (theBoard[2, n] == opponentShape && theBoard[3, n] == opponentShape && theBoard[1, n] == '-')
			{
				x = 1;
				z = n;
			}
			else if(theBoard[1, n] == opponentShape && theBoard[3, n] == opponentShape && theBoard[2, n] == '-')
			{
				x = 2;
				z = n;
			}
		}

		if (theBoard[1, 1] == opponentShape && theBoard[2, 2] == opponentShape && theBoard[3, 3] == '-')
		{
			x = 3;
			z = 3;
		}
		else if (theBoard[2, 2] == opponentShape && theBoard[3, 3] == opponentShape && theBoard[1, 1] == '-')
		{
			x = 1;
			z = 1;
		}
		else if (theBoard[1, 1] == opponentShape && theBoard[3, 3] == opponentShape && theBoard[2, 2] == '-')
		{
			x = 2;
			z = 2;
		}

		else if (theBoard[1, 3] == opponentShape && theBoard[2, 2] == opponentShape && theBoard[3, 1] == '-')
		{
			x = 3;
			z = 1;
		}
		else if (theBoard[2, 2] == opponentShape && theBoard[3, 1] == opponentShape && theBoard[1, 3] == '-')
		{
			x = 1;
			z = 3;
		}
		else if (theBoard[1, 3] == opponentShape && theBoard[3, 1] == opponentShape && theBoard[2, 2] == '-')
		{
			x = 2;
			z = 2;
		}

		blockingVector = new Vector3 (x, 0.1f, z);

		return blockingVector;
	}

	public Vector3 block2InARowEdges()
	{
		int x = -1;
		int z = -1;
		Vector3 blockingVector;
		
		for (int n = 0; n < 5; n += 4)
		{
			
			if (theBoard[n, 1] == opponentShape && theBoard[n, 2] == opponentShape && theBoard[n, 3] == '-')
			{
				x = n;
				z = 3;
			}
			else if (theBoard[n, 2] == opponentShape && theBoard[n, 3] == opponentShape && theBoard[n, 1] == '-')
			{
				x = n;
				z = 1;
			}
			else if(theBoard[n, 1] == opponentShape && theBoard[n, 3] == opponentShape && theBoard[n, 2] == '-')
			{
				x = n;
				z = 2;
			}
			
			else if (theBoard[1, n] == opponentShape && theBoard[2, n] == opponentShape && theBoard[3, n] == '-')
			{
				x = 3;
				z = n;
			}
			else if (theBoard[2, n] == opponentShape && theBoard[3, n] == opponentShape && theBoard[1, n] == '-')
			{
				x = 1;
				z = n;
			}
			else if(theBoard[1, n] == opponentShape && theBoard[3, n] == opponentShape && theBoard[2, n] == '-')
			{
				x = 2;
				z = n;
			}
		}
		
		blockingVector = new Vector3 (x, 0.1f, z);
		
		return blockingVector;
	}

	public Vector3 takeEdges()
	{
		int x = -1;
		int z = -1;
		Vector3 winningVector;
		
		for (int n = 0; n < 5; n += 4)
		{
			
			if (theBoard[n, 1] == shape && theBoard[n, 2] == shape && theBoard[n, 3] == '-' && theBoard[n, 0] == '-' && theBoard[n, 4] == '-')
			{
				x = n;
				z = 3;
			}
			else if (theBoard[n, 2] == shape && theBoard[n, 3] == shape && theBoard[n, 1] == '-' && theBoard[n, 0] == '-' && theBoard[n, 4] == '-')
			{
				x = n;
				z = 1;
			}
			else if(theBoard[n, 1] == shape && theBoard[n, 3] == shape && theBoard[n, 2] == '-' && theBoard[n, 0] == '-' && theBoard[n, 4] == '-')
			{
				x = n;
				z = 2;
			}
			/////////////////////////////////////
			else if (theBoard[1, n] == shape && theBoard[2, n] == shape && theBoard[3, n] == '-' && theBoard[0, n] == '-' && theBoard[4, n] == '-')
			{
				x = 3;
				z = n;
			}
			else if (theBoard[2, n] == shape && theBoard[3, n] == shape && theBoard[1, n] == '-' && theBoard[0, n] == '-' && theBoard[4, n] == '-')
			{
				x = 1;
				z = n;
			}
			else if(theBoard[1, n] == shape && theBoard[3, n] == shape && theBoard[2, n] == '-' && theBoard[0, n] == '-' && theBoard[4, n] == '-')
			{
				x = 2;
				z = n;
			}
		}
		
		winningVector = new Vector3 (x, 0.1f, z);
		
		return winningVector;
	}

	public Vector3 blockSean()
	{
		Vector3 blocker;
		int x = -1;
		int z = -1;

		if(theBoard[1,2] == opponentShape)
		{
			if(theBoard[0,1] == '-')
			{
				x = 0;
				z = 1;
			}
			else if(theBoard[0,3] == '-')
			{
				x = 0;
				z = 3;
			}
		}
		if(theBoard[3,2] == opponentShape)
		{
			if(theBoard[4,1] == '-')
			{
				x = 4;
				z = 1;
			}
			else if(theBoard[4,3] == '-')
			{
				x = 4;
				z = 3;
			}
		}
		if(theBoard[2,1] == opponentShape)
		{
			if(theBoard[1,0] == '-')
			{
				x = 1;
				z = 0;
			}
			else if(theBoard[3,0] == '-')
			{
				x = 3;
				z = 0;
			}
		}
		if(theBoard[2,3] == opponentShape)
		{
			if(theBoard[1,4] == '-')
			{
				x = 1;
				z = 4;
			}
			else if(theBoard[3,4] == '-')
			{
				x = 3;
				z = 4;
			}
		}

		blocker = new Vector3 (x, 0.1f, z);
		Debug.Log ("blockSean playing at (" + x + "," + z + ")");
		return blocker;
	}
}	
