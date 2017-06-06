using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public int currentPlayer = 1;
    public Pawn currentlySelectedPawn = null;

	// Use this for initialization
	void Start () {
        currentPlayer = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void togglePlayer() {
        if (currentPlayer == 1) { currentPlayer = 2; } else { currentPlayer = 1; }
        Debug.Log("Current player is now " + currentPlayer);
    }
}
