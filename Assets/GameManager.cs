using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public int currentPlayer = 1;
    public Pawn currentlySelectedPawn = null;
    public int movesRemaining = 0; //will be updated when dice rolled

	// Use this for initialization
	void Start () {
        currentPlayer = 1;
        movesRemaining = 3;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void togglePlayer() {
        if (currentPlayer == 1) { currentPlayer = 2; } else { currentPlayer = 1; }
        movesRemaining = 3; //TODO temp
        Debug.Log("Current player is now " + currentPlayer);
    }

    public void consumeMoves(int moveCost) {
        movesRemaining -= moveCost;
        Debug.Log(moveCost + " move(s) used, " + movesRemaining + " remaining");
        if (movesRemaining <= 0) {
            togglePlayer();
        } else {
            Debug.Log("Player " + currentPlayer + " still has " + movesRemaining + " moves left.");
        }
    }
}
