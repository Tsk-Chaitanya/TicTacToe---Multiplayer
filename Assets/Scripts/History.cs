using UnityEngine;
using System.Collections;
using System.IO;

public class History : MonoBehaviour {

	#region Variables
	const char		DELIM = ',';
	const string	gameHistory = "TTT_Game_History.txt",
					guestName 	= "Guest";
	const int 		numRecords 	= 100,
					numOfFields = 4,
					maxCount 	= 999,
					minCount 	= 0,
					nameIndex 	= 0,
					winIndex 	= 1,
					lossIndex 	= 2,
					drawIndex 	= 3;
	static int		playerIndex,
					currentRecords;
	static string 	readIn;
	static string[] inputLine;
	static bool 	playerFound;
	static string[,] PlayerHistory = new string[numRecords, numOfFields];
	#endregion

	#region Functions
	// Opens the history file and reads all content into PlayerHistory
	public static void PopulatePlayerHistory() {
		if(File.Exists(gameHistory)) {
			FileStream fileIO = new FileStream(gameHistory, FileMode.Open, FileAccess.Read);
			StreamReader readFile = new StreamReader(fileIO); // Initializes file reader object

			int count = 0;
			readIn = readFile.ReadLine();
			while(count < numRecords && readIn != null) {

				inputLine = readIn.Split(DELIM); // splits line into string array

				for(int j = 0; j < numOfFields; j++) {
					PlayerHistory[count, j] = inputLine[j];
				}
				count++;

				currentRecords = count;
				readIn = readFile.ReadLine();
			}
			readFile.Close();
			fileIO.Close();

			// Initializes all array spots after last record to empty string
			for(int k = count; k < numRecords; k++) {
				for(int i = 0; i < numOfFields; i++) {
					PlayerHistory[k, i] = "";
				}
			}
		}
	}

	public static void UpdatePlayerHistory(string pName, string win) {
		playerFound = false;
		string[] newPlayer = new string[numOfFields];

		if(pName != guestName) {
			int count = 0;

			while(count < currentRecords && !playerFound) {
				if(PlayerHistory[count,nameIndex] == pName) { // If name exists in the history file
					playerIndex = count;
					playerFound = true;
				}
				count++;
			}

			if(!playerFound) {
				playerIndex = currentRecords;
				currentRecords++;
				newPlayer[nameIndex] = pName;
			}

			if(pName == win) { // if player was the winner
				if(playerFound) {
					PlayerHistory[playerIndex, winIndex] = IncrementValue(PlayerHistory[playerIndex, winIndex]);
				}
				else {
					newPlayer[winIndex] = "1";
					newPlayer[lossIndex] = "0";
					newPlayer[drawIndex] = "0";
				}
			}
			else if (GameInfo.IsItADraw()){
				if(playerFound) {
					PlayerHistory[playerIndex, drawIndex] = IncrementValue(PlayerHistory[playerIndex, drawIndex]);
				}
				else {
					newPlayer[winIndex] = "0";
					newPlayer[lossIndex] = "0";
					newPlayer[drawIndex] = "1";
				}
			}
			else {
				if(playerFound) {
					PlayerHistory[playerIndex, lossIndex] = IncrementValue(PlayerHistory[playerIndex, lossIndex]);
				}
				else {
					newPlayer[winIndex] = "0";
					newPlayer[lossIndex] = "1";
					newPlayer[drawIndex] = "0";
				}
			}

			if(!playerFound) {
				for(int n = 0; n < numOfFields; n++) {
					PlayerHistory[playerIndex, n] = newPlayer[n];
				}
			}
		}
	}

	static string IncrementValue(string s) {
		string tempStr = s;
		int tempInt = System.Convert.ToInt32(s);

		if(tempInt >= minCount && tempInt <= maxCount) {
			tempInt++;
			tempStr = tempInt.ToString();
		}
		return tempStr;
	}


	static public void WriteHistoryFile() {
		/*FileMode.Create overwrites any existing file, which is what we want since the 
		  most updated player history data will always be stored in the PlayerHistory
		  array every time the game launches, or when a game is completed*/
		FileStream fileIO = new FileStream(gameHistory, FileMode.Create, FileAccess.Write);
		StreamWriter writeFile = new StreamWriter(fileIO); // Initializes file reader object
		string tempStr;

		for(int i = 0; i < currentRecords; i++) {
			tempStr = PlayerHistory[i, 0] + DELIM + PlayerHistory[i, 1] + DELIM + PlayerHistory[i, 2] + DELIM + PlayerHistory[i, 3];
			writeFile.WriteLine(tempStr);
		}
		writeFile.Close();
		fileIO.Close(); 
	}

	static public int GetCurrentRecords() {
		return currentRecords;
	}

	// Displays the non-empty contents of the PlayerHistory array line-by-line in the Console window
	static public bool DisplayArray() {
		string emptyString = "";
		string[] str = new string[numOfFields];
		for(int i = 0; i < currentRecords; i++) {
			for(int j = 0; j < numOfFields; j++ ) {
				if(PlayerHistory[i, j] == emptyString) {
					return true;
				}
				str[j] = PlayerHistory[i, j];
			}
			Debug.Log(str[0] + " , "  + str[1] + " , " + str[2] + " , " + str[3]);
			str[0] = str[1] = str[2] = str[3] = "";
		}
		return true;
	}

	// Returns each entry in the PlayerHistory array as a formatted string
	static public string GetPlayerHistoryEntry() {
		string emptyString = "", tempStr = "";
		string[] str = new string[numOfFields];
		Debug.Log("From GetPlayerHistoryEntry -- currentRecords = " + currentRecords);
		for(int j = 0; j < currentRecords; j++ ) {
			for(int k = 0; k < numOfFields; k++ ) {
				if(PlayerHistory[j, k] == emptyString) {
					return emptyString;
				}
				str[k] = PlayerHistory[j, k];
			}
			tempStr = tempStr + str[0] + "\r\n" + str[1] + " / " + str[2] + " / " + str[3] + "\r\n\r\n";
		}
		return tempStr;
	}

	static public string GetPlayerHistoryNames(int i) {
		if(PlayerHistory[i, 0] == "") {
			return "";
		}
		else {
			return PlayerHistory[i, 0];
		}
	}
	#endregion
}
