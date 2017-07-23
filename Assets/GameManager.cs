using System.Collections;
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

    public int currentPlayer = 1;
    public Pawn currentlySelectedPawn = null;
    public int movesRemaining = 0; //will be updated when dice rolled
    private bool canRoll = false;

    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;

    private Color colorPlayer1 = new Color(255, 0, 90);
    private Color colorPlayer2 = new Color(0, 139, 255);

    private bool animateRollResult = false;
    private float animateRollResultTimeStarted = 0f;
    private const float TIMETOWAIT_ROLLRESULT = 3f;

    private Texture2D[] cursors;
    private const int CURSOR_NORMAL = 0;
    private const int CURSOR_INVALID = 1;
    private const int CURSOR_ATTACK = 2;

    // Use this for initialization
    void Start () {
        currentPlayer = 1;
        uiRollDice.gameObject.SetActive(true);
        uiRollResult.gameObject.SetActive(false);
        uiSkipTurn.gameObject.SetActive(false);
        uiWinText.gameObject.SetActive(false);
        canRoll = true;

        uiPlayer1Score.color = colorPlayer1;
        uiPlayer2Score.color = colorPlayer2;

        cursors = new Texture2D[3];
        cursors[0] = (Texture2D)Resources.Load("Cursor_Normal");
        cursors[1] = (Texture2D)Resources.Load("Cursor_Invalid");
        cursors[2] = (Texture2D)Resources.Load("Cursor_Attack");
    }
	
	// Update is called once per frame
	void Update () {
        //uiPlayerName.text = "Player " + currentPlayer;
        uiPlayerName.text = LocalizationManager.instance.GetLocalizedValue("current_player", currentPlayer.ToString());
        uiMovementsLeft.text = LocalizationManager.instance.GetLocalizedValue("moves_remaining", movesRemaining.ToString());
        if (currentPlayer == 1) {
            uiPlayerName.color = colorPlayer1;
        } else {
            uiPlayerName.color = colorPlayer2;
        }

        uiPlayer1Score.text = LocalizationManager.instance.GetLocalizedValue("score_player", "1", scorePlayer1.ToString());
        uiPlayer2Score.text = LocalizationManager.instance.GetLocalizedValue("score_player", "2", scorePlayer2.ToString());

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
                    if (currentlySelectedPawn.checkValidMovement()) {
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
            uiSkipTurn.gameObject.SetActive(false);
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
            scorePlayer1++;
            if (scorePlayer1 == 7) {
                Win();
            }
        } else {
            scorePlayer2++;
            if (scorePlayer2 == 7) {
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
