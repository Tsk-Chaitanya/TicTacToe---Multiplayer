using UnityEngine;
using System.Collections;

public class BoardSpace : MonoBehaviour {
	
	#region Variables
	int		h_val,
			v_val;
	string 	thisSpaceName;
	#endregion

	#region Functions
	void Start () {
		thisSpaceName	= this.name;
		h_val			= thisSpaceName[5] - 48;
		v_val			= thisSpaceName[7] - 48;
	}

	public void UpdateSpaceState (char player) {
		for(int i = 0; i < 28; i++) {
			if(BoardArray.GetWCB(h_val, v_val, i)) {
				if(player == 'o') {
					BoardArray.IncrementWCSO(i);
					/*Debug.Log("winCheckSumO[" + i + "] = " + BoardArray.GetWCSO(i).ToString()
					          + ",  h_val:" + h_val + " & v_val:" + v_val);*/
				};
				if(player == 'x') {
					BoardArray.IncrementWCSX(i);
					/*Debug.Log("winCheckSumX[" + i + "] = " + BoardArray.GetWCSX(i).ToString()
					          + ",  h: " + h_val + " & v: " + v_val);*/
				};
			}
		}
	}
	#endregion
}