using UnityEngine;
using System.Collections;

// All in-game GUI related articles live here
public class GameInfo : MonoBehaviour {

	#region Variables
	static int		turnCount;
	static float	screenHeight,
					screenWidth,
					labelWidth,
					labelHeight;
	static string 	winner,
					p1name,
					p2name;
	static GUIStyle p1Text		= new GUIStyle();
	static GUIStyle p2Text		= new GUIStyle();
	static GUIStyle turnText	= new GUIStyle();
	#endregion

	#region Functions
	void Start () {
		turnCount		= 0; // tracks # of turns
		screenHeight	= Screen.height;
		screenWidth 	= Screen.width;
		labelWidth		= screenWidth/10;
		labelHeight		= screenHeight/10;
		p1Text.fontSize = 20;
		p2Text.fontSize = 20;
		SetInitialTextColors();
		winner			= "";
	}

	// Alternates player name colors on the ui every turn
	public static void ChangeColor() {
		if(!Player.WhoseTurn()) {
			p1Text.normal.textColor = Color.green; // Green when it's that player's turn
			p2Text.normal.textColor = Color.gray; // Gray when it's the other player's turn
		}
		else {
			p1Text.normal.textColor = Color.gray;
			p2Text.normal.textColor = Color.green;
		};
	}

	//Label Template -->  GUI.Label (new Rect(loc x, loc y, size x, size y), "contents");
	public void OnGUI() {
		GUI.Label (new Rect((Screen.width/2) - (labelWidth/2) + (labelWidth/10), // loc x
		                    (Screen.height/11) - (labelHeight/2) - (labelHeight/4), // loc y
		                    labelWidth, labelHeight), "Turn Count: " + turnCount.ToString()); // size x, size y, text
		
		GUI.Label (new Rect((Screen.width/8) - (labelWidth/2),
		                    (Screen.height/2) - (labelHeight/2),
		                    labelWidth, labelHeight), Player.GetPlayer1Name(), p1Text);
		
		GUI.Label (new Rect((Screen.width) - ((Screen.width/5) - (labelWidth/2)),
		                    (Screen.height/2) - (labelHeight/2),
		                    labelWidth, labelHeight), Player.GetPlayer2Name(), p2Text);
	}
	

	// Assigns initial colors of player names and turn count text
	void SetInitialTextColors() {
		turnText.normal.textColor = Color.blue;
		if(!Player.IsAIFirst()) { // Sets starting player name colors
			p1Text.normal.textColor = Color.green;
			p2Text.normal.textColor = Color.gray;
		}
		else {
			p1Text.normal.textColor = Color.gray;
			p2Text.normal.textColor = Color.green;
		}
	}

	public static bool IsItADraw() {
		if(turnCount >= 25) {
			return true;
		}
		return false;	
	}

	public static void IncrementTurnCount() {
		turnCount++;
	}

	public static void SetWinner(string w) {
		winner = w;
	}

	public static string GetWinner() {
		return winner;
	}
	#endregion
}