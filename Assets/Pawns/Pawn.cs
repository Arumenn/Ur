using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {

    public int playerOwner = 0;
    public PawnState pawnState = PawnState.waiting;

    private GameManager gm;
    public Square currentSquare = null;

    // Use this for initialization
    void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (gm.currentlySelectedPawn == this) {
            //Isometric mouse to game
            Plane ground = new Plane(Vector3.up, -0.15f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float dist;
            ground.Raycast(ray, out dist);
            Vector3 pos = ray.GetPoint(dist);
            transform.position = pos;
        }
	}

    private void OnMouseDown() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (gm.currentlySelectedPawn == null) {
                gm.currentlySelectedPawn = this;
                pawnState = PawnState.waiting;
            } else if (gm.currentlySelectedPawn == this) {
                gm.currentlySelectedPawn = null;
                if (currentSquare != null) {
                    PlacePawn();
                }else {
                    pawnState = PawnState.waiting;
                }
            }
        }
    }

    private void PlacePawn() {
        if (currentSquare != null) {
            if ((currentSquare.isOccupied)) {
                if (currentSquare.currentPawn == this) {
                    Snap();
                    pawnState = PawnState.inPlay;
                } else {
                    currentSquare.currentPawn.pawnState = PawnState.finished;
                    GameObject.Destroy(currentSquare.currentPawn);
                    pawnState = PawnState.inPlay;
                    Snap();
                }
            }else {
                print("Won't snap here");
                pawnState = PawnState.waiting;
            }
        }
    }

    private void Snap() {
        if (currentSquare != null) {
            //snap it
            Vector3 snapPosition = new Vector3(currentSquare.transform.position.x, currentSquare.transform.position.y + 0.15f, currentSquare.transform.position.z);
            transform.position = snapPosition;
            print("Snapping " + name);
        }
    }

    private void OnTriggerEnter(Collider other) {
        Square square = other.gameObject.GetComponent<Square>();
        if (square) {
            currentSquare = square;
            if (!square.isOccupied) {
                square.currentPawn = this;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Square square = other.gameObject.GetComponent<Square>();
        if (square) {
            if (currentSquare == square) {
                currentSquare = null;
                square.currentPawn = null;
            }
        }
    }

}

public enum PawnState { waiting, inPlay, finished }
