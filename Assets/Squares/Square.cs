using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {

    [SerializeField] public SquareType squareType = SquareType.Normal;
    [SerializeField] public SquareTerritory squareTerritory = SquareTerritory.Neutral;

    public Pawn currentPawn = null;

    private GameManager gm;

    public bool isOccupied {
        get { return currentPawn != null; }
    }

    public bool isRosette {
        get { return squareType == SquareType.Rosette;  }
    }

	// Use this for initialization
	void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {

	}
    
    /*private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<Pawn>()) {
            if (!isOccupied) {
                currentPawn = other.gameObject.GetComponent<Pawn>();
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if ((gm.currentlySelectedPawn == currentPawn) && (currentPawn != null)) {
            if ((isOccupied) && (other.gameObject.GetComponent<Pawn>() == currentPawn)) {
                //snap it
                Vector3 snapPosition = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
                other.gameObject.transform.position = snapPosition;
                print("Snapping " + other.name);
            } else {
                print("Won't snap here");
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<Pawn>()) {
            if ((isOccupied) && (other.gameObject.GetComponent<Pawn>() == currentPawn)) {
                currentPawn = null;
            }
        }
    }*/
}

public enum SquareType { Normal, Rosette, Enter, Exit }
public enum SquareTerritory { Neutral, Player1, Player2 }
