﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Text uiPlayerName;
    public Text uiMovementsLeft;
    public Button uiRollDice;
    public Text uiRollResult;
    public Button uiSkipTurn;
    public Text uiWinText;
    public Text uiPlayer1Score;
    public Text uiPlayer2Score;
    public Text uiDebug;
    public GameObject uiTips;

    public int currentPlayer = 1;
    public Pawn currentlySelectedPawn = null;
    public int movesRemaining = 0; //will be updated when dice rolled
    private bool canRoll = false;
    private bool hasAlreadyRerolled = false;

    private PlayerManager pm;

    private bool animateRollResult = false;
    private float animateRollResultTimeStarted = 0f;
    private const float TIMETOWAIT_ROLLRESULT = 3f;

    private Texture2D[] cursors;
    private const int CURSOR_NORMAL = 0;
    private const int CURSOR_INVALID = 1;
    private const int CURSOR_ATTACK = 2;

    // Use this for initialization
    private IEnumerator Start() {
        //waits for the localization manager to be ready
        while (!LocalizationManager.instance.GetIsReady()) {
            yield return null;
        }
        currentPlayer = 1;
        uiRollDice.gameObject.SetActive(true);
        uiRollResult.gameObject.SetActive(false);
        uiSkipTurn.gameObject.SetActive(false);
        uiWinText.gameObject.SetActive(false);
        uiTips.SetActive(false);
        canRoll = true;
        hasAlreadyRerolled = false;

        pm = FindObjectOfType<PlayerManager>();

        uiPlayer1Score.color = pm.colorPlayer1;
        uiPlayer2Score.color = pm.colorPlayer2;

        cursors = new Texture2D[3];
        cursors[0] = (Texture2D)Resources.Load("Cursor_Normal");
        cursors[1] = (Texture2D)Resources.Load("Cursor_Invalid");
        cursors[2] = (Texture2D)Resources.Load("Cursor_Attack");
    }
	
	// Update is called once per frame
	void Update () {
        //uiPlayerName.text = "Player " + currentPlayer;
        uiPlayerName.text = LocalizationManager.instance.GetLocalizedValue("current_player", pm.GetPlayerName(currentPlayer));
        uiMovementsLeft.text = LocalizationManager.instance.GetLocalizedValue("moves_remaining", movesRemaining.ToString());
        if (currentPlayer == 1) {
            uiPlayerName.color = pm.colorPlayer1;
        } else {
            uiPlayerName.color = pm.colorPlayer2;
        }

        uiPlayer1Score.text = LocalizationManager.instance.GetLocalizedValue("score_player", pm.GetPlayerName(1), pm.scorePlayer1.ToString());
        uiPlayer2Score.text = LocalizationManager.instance.GetLocalizedValue("score_player", pm.GetPlayerName(2), pm.scorePlayer2.ToString());

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

        if ((canRoll) && (Input.GetKeyDown(KeyCode.Space))) {
            buttonRollDice();
        }


        uiTips.SetActive(currentlySelectedPawn != null);
    }

    private void LateUpdate() {
        string text;
        text = "";
        int cursorIndex = CURSOR_NORMAL;
        if ((currentlySelectedPawn != null) && (currentlySelectedPawn.currentSquare != null)){
            if (currentlySelectedPawn.currentSquare.isOccupied) {
                if (currentlySelectedPawn.currentSquare.currentPawn.playerOwner == currentPlayer) {
                    //can't move on my own pawns
                    text = "Can't move-Blocked";
                    cursorIndex = CURSOR_INVALID;
                } else if (currentlySelectedPawn.currentSquare.squareTerritory == 0) {
                    //can push out and move
                    if (currentlySelectedPawn.currentSquare.position == 8) {
                        text = "Can't attack-Safe space";
                        cursorIndex = CURSOR_INVALID;
                    } else if (currentlySelectedPawn.checkValidMovement()) {
                        text = "Can attack";
                        cursorIndex = CURSOR_ATTACK;
                    } else {
                        text = "Can't move-Move cost";
                        cursorIndex = CURSOR_INVALID;
                    }
                } else {
                    text = "Can't move-WrongPath";
                    cursorIndex = CURSOR_INVALID;
                }
            } else {
                if ((currentlySelectedPawn.currentSquare.squareTerritory == currentPlayer) || (currentlySelectedPawn.currentSquare.squareTerritory == 0)) {
                    if (currentlySelectedPawn.checkValidMovement()) {
                        //can move
                        text = "Can move";
                        cursorIndex = CURSOR_NORMAL;
                    } else {
                        //can't move
                        text = "Can't move-Move cost";
                        cursorIndex = CURSOR_INVALID;
                    }
                } else {
                    text = "Can't move-Wrong path";
                    cursorIndex = CURSOR_INVALID;
                }
            }
        }
        uiDebug.text = text;
        Cursor.SetCursor(cursors[cursorIndex], new Vector2(0,0), CursorMode.Auto);
    }

    public void togglePlayer() {
        if (currentPlayer == 1) { currentPlayer = 2; } else { currentPlayer = 1; }
        uiRollDice.gameObject.SetActive(true);
        uiRollResult.gameObject.SetActive(false);
        uiSkipTurn.gameObject.SetActive(false);
        canRoll = true;
        hasAlreadyRerolled = false;
        animateRollResult = false;
        Debug.Log("Current player is now " + currentPlayer);
    }

    public void consumeMoves(int moveCost, SquareType squareType) {
        movesRemaining -= moveCost;
        Debug.Log(moveCost + " move(s) used, " + movesRemaining + " remaining");

        if ((squareType == SquareType.Rosette) && (hasAlreadyRerolled == false)) {
            //Allow reroll
            Debug.Log("Landed on a rosette, allow re-roll");
            uiRollDice.gameObject.SetActive(true);
            uiSkipTurn.gameObject.SetActive(false);
            hasAlreadyRerolled = true;
            canRoll = true;
        } else {
            if (movesRemaining <= 0) {
                togglePlayer();
            } else {
                Debug.Log("Player " + currentPlayer + " still has " + movesRemaining + " moves left.");
            }
        }
    }

    public void scorePoint() {
        Debug.Log("Player " + currentPlayer + " has got 1 pawn home!");
        if (currentPlayer == 1) {
            pm.scorePlayer1++;
            if (pm.scorePlayer1 == 7) {
                Win();
            }
        } else {
            pm.scorePlayer2++;
            if (pm.scorePlayer2 == 7) {
                Win();
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

    public void buttonSkipTurn() {
        if (currentlySelectedPawn != null) {
            currentlySelectedPawn.cancelMovement();
            currentlySelectedPawn = null;
        }
        togglePlayer();
    }

    public void buttonRollDice() {
        movesRemaining = rollDice();
        uiRollDice.gameObject.SetActive(false);
        uiRollResult.gameObject.SetActive(true);
        canRoll = false;
        uiRollResult.text = LocalizationManager.instance.GetLocalizedValue("roll_result", movesRemaining.ToString());
        animateRollResult = true;
        animateRollResultTimeStarted = Time.time;
        if (movesRemaining == 0) {
            uiRollResult.text = uiRollResult.text + " :(";
            Invoke("togglePlayer", TIMETOWAIT_ROLLRESULT);
        } else {
            uiSkipTurn.gameObject.SetActive(true);
        }
    }

    public void Win() {
        uiWinText.text = LocalizationManager.instance.GetLocalizedValue("player_win", currentPlayer.ToString());
        uiWinText.gameObject.SetActive(true);
        uiRollDice.gameObject.SetActive(false);
        uiRollResult.gameObject.SetActive(false);
        uiSkipTurn.gameObject.SetActive(false);
        canRoll = false;
        currentPlayer = 0;
    }
}
