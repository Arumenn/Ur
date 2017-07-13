using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Text uiPlayerName;
    public Text uiMovementsLeft;
    public Button uiRollDice;
    public Text uiRollResult;

    public int currentPlayer = 1;
    public Pawn currentlySelectedPawn = null;
    public int movesRemaining = 0; //will be updated when dice rolled

    private Color colorPlayer1 = new Color(255, 0, 90);
    private Color colorPlayer2 = new Color(0, 139, 255);

    private bool animateRollResult = false;
    private float animateRollResultTimeStarted = 0f;
    private const float TIMETOWAIT_ROLLRESULT = 3f;

	// Use this for initialization
	void Start () {
        currentPlayer = 1;
        uiRollDice.gameObject.SetActive(true);
        uiRollResult.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        uiPlayerName.text = "Player " + currentPlayer;
        uiMovementsLeft.text = "Moves left: " + movesRemaining;
        if (currentPlayer == 1) {
            uiPlayerName.color = colorPlayer1;
        } else {
            uiPlayerName.color = colorPlayer2;
        }

        if (animateRollResult) {
            if (Time.time - animateRollResultTimeStarted > TIMETOWAIT_ROLLRESULT) {
                animateRollResult = false;
                uiRollResult.gameObject.SetActive(false);
            }
        }


        if ((currentlySelectedPawn != null) && (currentlySelectedPawn.playerOwner == currentPlayer) && (Input.GetMouseButtonDown(1))) {
            Debug.Log("Right click");
            currentlySelectedPawn.cancelMovement();
        }
    }

    public void togglePlayer() {
        if (currentPlayer == 1) { currentPlayer = 2; } else { currentPlayer = 1; }
        uiRollDice.gameObject.SetActive(true);
        uiRollResult.gameObject.SetActive(false);
        animateRollResult = false;
        Debug.Log("Current player is now " + currentPlayer);
    }

    public void consumeMoves(int moveCost, SquareType squareType) {
        movesRemaining -= moveCost;
        Debug.Log(moveCost + " move(s) used, " + movesRemaining + " remaining");

        if (squareType == SquareType.Rosette) {
            //Allow reroll
            Debug.Log("Landed on a rosette, re-rolled");
            uiRollDice.gameObject.SetActive(true);
        } else {
            if (movesRemaining <= 0) {
                togglePlayer();
            } else {
                Debug.Log("Player " + currentPlayer + " still has " + movesRemaining + " moves left.");
            }
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

    public void buttonRollDice() {
        movesRemaining = rollDice();
        uiRollDice.gameObject.SetActive(false);
        uiRollResult.gameObject.SetActive(true);
        uiRollResult.text = "YOU ROLLED " + movesRemaining;
        animateRollResult = true;
        animateRollResultTimeStarted = Time.time;
    }
}
