using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {

    public int playerOwner = 0;
    public PawnState pawnState = PawnState.waiting;
    public Square currentSquare = null;
    public int lastPosition = 0;

    private GameManager gm;
    private GameCamera gc;
    private float heightOffset = 0.15f;
    private float groundOffset = -0.25f;
    private Vector3 originalPosition;

    //for undo
    private Vector3 lastKnownTransform;
    private Square lastSquare = null;
    private PawnState lastPawnState;
    private int lastKnownPosition;    

    // Use this for initialization
    void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
        gc = GameObject.FindObjectOfType<GameCamera>();
        originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (gm.currentlySelectedPawn == this) {
            //Isometric mouse to game
            Plane ground = new Plane(Vector3.up, groundOffset);
            Ray ray = gc.getCurrentCamera().ScreenPointToRay(Input.mousePosition);
            float dist;
            ground.Raycast(ray, out dist);
            Vector3 pos = ray.GetPoint(dist);
            transform.position = pos;

            //Raycasting to detect Squares
            var down = transform.TransformDirection(Vector3.down);
            RaycastHit squareHit;
            Debug.DrawRay(transform.position, down * 2, Color.green);
            if (Physics.Raycast(transform.position, down, out squareHit, 2)) {
                if (squareHit.collider.gameObject.GetComponent<Square>() != null) {
                    Debug.Log("Hit a square! " + squareHit.collider.gameObject.name);
                    currentSquare = squareHit.collider.gameObject.GetComponent<Square>();
                    Debug.Log(name + " is hovering over " + currentSquare.position);
                } else {
                    Debug.Log("Hit something else " + squareHit.collider.gameObject.name);
                    currentSquare = null;
                }
            } else {
                Debug.Log("Hit nothing");
                currentSquare = null;
            }
        }
	}

    private void OnMouseDown() {
        if ((gm.currentPlayer != playerOwner) && (pawnState != PawnState.finished)) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Clicked " + name + " (currentSquare=" + currentSquare + "; State=" + pawnState + ")");
            if (gm.movesRemaining <= 0) {
                Debug.Log("No moves remaining");
                return;
            }
            if (gm.currentlySelectedPawn == null) {
                //are we near the end?
                if (currentSquare != null) {
                    int distanceToOutside = 15 - currentSquare.position; //15 is outside the 14 squares, so past the finish line
                    if ((distanceToOutside <= 4) && (gm.movesRemaining > distanceToOutside)) {
                        //an exact throw is needed to get a pawn home
                        Debug.LogWarning("You need an exact throw of " + distanceToOutside + " to move this pawn home");
                        return;
                    }
                }
                Debug.Log("Picked up " + name);
                //saves last known infos
                lastKnownTransform = transform.position;
                lastPawnState = pawnState;
                lastSquare = currentSquare;
                lastKnownPosition = lastPosition;
                //sets to moving
                gm.currentlySelectedPawn = this;
                pawnState = PawnState.waiting;
            } else if (gm.currentlySelectedPawn == this) {
                if (checkValidMovement()) {
                    if (PlacePawn()) {
                        gm.currentlySelectedPawn = null;
                        gm.consumeMoves(gm.movesRemaining, currentSquare.squareType);
                    }
                } else {
                    pawnState = PawnState.waiting;
                    Debug.Log("Invalid placement clicked for " + name);
                }

                /*if ((currentSquare != null) && (currentSquare.position >= lastPosition)) {
                    int moveCost = currentSquare.position - lastPosition;
                    if (moveCost > gm.movesRemaining) {
                        Debug.LogWarning("Not enough moves remaining");
                    } else if (moveCost < gm.movesRemaining) {
                        Debug.LogWarning("You have to use all the moves");
                    } else {
                        Debug.Log("Valid square clicked for " + name);
                        
                    }
                } else {
                    pawnState = PawnState.waiting;
                    Debug.Log("Invalid placement clicked for " + name);
                }*/
            }
        }
    }

    public bool checkValidMovement() {
        if ((currentSquare != null) && (currentSquare.position >= lastPosition)) {
            int moveCost = currentSquare.position - lastPosition;
            if (moveCost > gm.movesRemaining) {
                Debug.LogWarning("Not enough moves remaining");
                return false;
            } else if (moveCost < gm.movesRemaining) {
                Debug.LogWarning("You have to use all the moves");
                return false;
            } else {
                Debug.Log("Valid square for " + name);
                return true;
            }
        }
        return false;
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
                        if (currentSquare.position == 8) {
                            Debug.Log("Can't attack on a safe space");
                            return false;
                        } else {
                            return Snap();
                        }
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
            //removing from previous square
            if (lastSquare != null) {
                Debug.Log("Removing from square " + lastSquare.position);
                lastSquare.currentPawn = null;
            }
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

    public void cancelMovement() {
        //restores last known infos
        if (lastSquare != null) {
            lastSquare.switchWith(this);
        }
        transform.position = lastKnownTransform;
        pawnState = lastPawnState;
        currentSquare = lastSquare;
        lastPosition = lastKnownPosition;
        gm.currentlySelectedPawn = null;
        Debug.Log("Cancelled movement");
    }

    /*private void OnTriggerEnter(Collider other) {
        Square square = other.gameObject.GetComponent<Square>();
        if (square) {
            Debug.Log(name + " entering " + square.position);
            if ((currentSquare != square)) {
                currentSquare = square;
                Debug.Log(name + " is hovering over " + square.position);
                //square.currentPawn = this;
                //Debug.Log(square.position + " is free so we set " + name + " as pawn");
            }
        }
    }*/

    /*private void OnTriggerExit(Collider other) {
        Square square = other.gameObject.GetComponent<Square>();
        if (square) {
            Debug.Log(name + " leaving " + square.position);
            if (currentSquare == square) {
                currentSquare = null;
                other.gameObject.GetComponent<Square>().currentPawn = null;
            }
        }
    }*/

    public void pushOut() {
        Debug.Log(name + " is dead");
        pawnState = PawnState.waiting;
        currentSquare.currentPawn = null;
        currentSquare = null;
        transform.position = originalPosition;
        lastPosition = 0;
    }

    public void gotHome() {
        Destroy(this.gameObject);
    }

}

public enum PawnState { waiting, inPlay, finished }
