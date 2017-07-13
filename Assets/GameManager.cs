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
        movesRemaining = rollDice();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void togglePlayer() {
        if (currentPlayer == 1) { currentPlayer = 2; } else { currentPlayer = 1; }
        movesRemaining = rollDice();
        Debug.Log("Current player is now " + currentPlayer);
    }

    public void consumeMoves(int moveCost, SquareType squareType) {
        movesRemaining -= moveCost;
        Debug.Log(moveCost + " move(s) used, " + movesRemaining + " remaining");

        if (squareType == SquareType.Rosette) {
            //Allow reroll
            Debug.Log("Landed on a rosette, re-rolled");
            movesRemaining = rollDice(); //TODO
        }

        if (movesRemaining <= 0) {
            togglePlayer();
        } else {
            Debug.Log("Player " + currentPlayer + " still has " + movesRemaining + " moves left.");
        }
    }

    public int rollDice() {
        int whiteCorners = 0;
        int dieResult = 0;

        //rolls 4 dice and check if white corner
        for (int i = 1; i < 5; i++) {
            //1 or 2 == white corner
            dieResult = Random.Range(0, 4);
            if (dieResult == 1 || dieResult == 2) {
                whiteCorners++;
            }
        }
        Debug.Log("Rolled " + whiteCorners + " white corners");
        return whiteCorners;
    }
}
