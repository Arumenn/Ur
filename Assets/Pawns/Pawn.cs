using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {

    public int playerOwner = 0;
    public PawnState pawnState = PawnState.waiting;
    public Square currentSquare = null;

    private GameManager gm;
    private float heightOffset = 0.15f;
    private float groundOffset = -0.25f;
    [SerializeField] private int lastPosition = 0;

    // Use this for initialization
    void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (gm.currentlySelectedPawn == this) {
            //Isometric mouse to game
            Plane ground = new Plane(Vector3.up, groundOffset);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            ground.Raycast(ray, out dist);
            Vector3 pos = ray.GetPoint(dist);
            transform.position = pos;
        }
	}

    private void OnMouseDown() {
        if (Input.GetKeyDown(KeyCode.Mouse0) && (gm.currentPlayer == playerOwner)) {
            Debug.Log("Clicked " + name + " (currentSquare=" + currentSquare + "; State=" + pawnState + ")");
            if (gm.currentlySelectedPawn == null) {
                gm.currentlySelectedPawn = this;
                pawnState = PawnState.waiting;
                Debug.Log("Picked up " + name);
            } else if (gm.currentlySelectedPawn == this) {
                if ((currentSquare != null) && (currentSquare.position >= lastPosition)) {
                    int moveCost = currentSquare.position - lastPosition;
                    if (moveCost > gm.movesRemaining) {
                        Debug.LogWarning("Not enough moves remaining");
                    } else {
                        Debug.Log("Valid square clicked for " + name);
                        if (PlacePawn()) {
                            gm.currentlySelectedPawn = null;
                            if (currentSquare.squareType == SquareType.Rosette) {
                                //Allow reroll
                                Debug.Log("Landed on a rosette, added 3 moves moves");
                                gm.movesRemaining += 3; //TODO
                            }
                            gm.consumeMoves(moveCost);
                        }
                    }
                } else {
                    pawnState = PawnState.waiting;
                    Debug.Log("Invalid placement clicked for " + name);
                }
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Mouse1) && (gm.currentPlayer == playerOwner) && (gm.currentlySelectedPawn == this)) {
                //cancel movement
                //TODO code for cancel movement
                Debug.Log("Cancel movement NOT DONE YET");
            }
        }
    }

    private bool PlacePawn() {
        Debug.Log("Request to place pawn " + name + "...");
        if (currentSquare != null) {
            if ((currentSquare.squareTerritory == 0) || (currentSquare.squareTerritory == playerOwner)) {
                if ((currentSquare.isOccupied)) {
                    Debug.Log("Square " + currentSquare.position + " is occupied...");
                    if (currentSquare.currentPawn == this) {
                        Debug.Log("by me.");
                        return Snap();
                    } else if (currentSquare.currentPawn.playerOwner != playerOwner) {
                        Debug.Log("by an other player's pawn");
                        return Snap();
                    }
                } else {
                    Debug.Log("Square " + currentSquare.position + " is free...");
                    pawnState = PawnState.waiting;
                    return Snap();
                }
            } else {
                Debug.Log("Not your territory.");
                return false;
            }
        } else {
            Debug.Log("No square to place on.");
        }
        return false;
    }

    private bool Snap() {
        if (currentSquare != null) {
            //snap it
            Vector3 snapPosition = new Vector3(currentSquare.transform.position.x, currentSquare.transform.position.y + heightOffset, currentSquare.transform.position.z);
            transform.position = snapPosition;
            lastPosition = currentSquare.position;
            print("Snapping " + name + " to position " + lastPosition);
            return currentSquare.switchWith(this);
        } else {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        Square square = other.gameObject.GetComponent<Square>();
        if (square) {
            Debug.Log(name + " entering " + square.position);
            if ((currentSquare != square)) {
                currentSquare = square;
                Debug.Log(name + " is hovering over " + square.position);
                /*square.currentPawn = this;
                Debug.Log(square.position + " is free so we set " + name + " as pawn");*/
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Square square = other.gameObject.GetComponent<Square>();
        if (square) {
            Debug.Log(name + " leaving " + square.position);
            if (currentSquare == square) {
                currentSquare = null;
                square.currentPawn = null;
            }
        }

    }

    public void pushOut() {
        Debug.Log(name + " is dead");
        pawnState = PawnState.finished;
        currentSquare = null;
        GameObject.Destroy(gameObject);
    }

}

public enum PawnState { waiting, inPlay, finished }
