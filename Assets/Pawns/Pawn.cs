﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {

    public int playerOwner = 0;
    public PawnState pawnState = PawnState.waiting;
    public Square currentSquare = null;

    private GameManager gm;
    public int lastPosition = 0;

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
        if (Input.GetKeyDown(KeyCode.Mouse0) && (gm.currentPlayer == playerOwner)) {
            Debug.Log("Clicked " + name + " (currentSquare=" + currentSquare + "; State=" + pawnState + ")");
            if (gm.currentlySelectedPawn == null) {
                gm.currentlySelectedPawn = this;
                pawnState = PawnState.waiting;
                Debug.Log("Picked up " + name);
            } else if (gm.currentlySelectedPawn == this) {
                if ((currentSquare != null) && (currentSquare.position >= lastPosition)) {
                    Debug.Log("Valid square clicked for " + name);
                    gm.currentlySelectedPawn = null;
                    PlacePawn();
                    gm.togglePlayer();
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

    private void PlacePawn() {
        Debug.Log("Request to place pawn " + name + "...");
        if (currentSquare != null) {
            if ((currentSquare.isOccupied)) {
                Debug.Log("Square " + currentSquare.position + " is occupied...");
                if (currentSquare.currentPawn == this) {
                    Debug.Log("by me.");
                    Snap();
                    pawnState = PawnState.inPlay;
                } else if (currentSquare.currentPawn.playerOwner != playerOwner) {
                    Debug.Log("by an other player's pawn");
                    Snap();
                }
            }else {
                Debug.Log("Square " + currentSquare.position + " is free...");
                Snap();
                pawnState = PawnState.waiting;
            }
        } else {
            Debug.Log("No square to place on.");
        }
    }

    private void Snap() {
        if (currentSquare != null) {
            //snap it
            Vector3 snapPosition = new Vector3(currentSquare.transform.position.x, currentSquare.transform.position.y + 0.15f, currentSquare.transform.position.z);
            transform.position = snapPosition;
            lastPosition = currentSquare.position;
            currentSquare.switchWith(this);
            print("Snapping " + name + " to position " + lastPosition);
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