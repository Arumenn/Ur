using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {

    [SerializeField] public SquareType squareType = SquareType.Normal;
    [SerializeField] public int squareTerritory = 0;

    public Pawn currentPawn = null;
    public int position = 0;

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

    public bool switchWith(Pawn replacement) {
        if (currentPawn == null) {
            currentPawn = replacement;
            Debug.Log("Set current spawn to " + replacement.name);
            return true;
        } else if (currentPawn == replacement) {
            Debug.Log("Not switching anything, " + replacement.name + " is already on that square");
            currentPawn = replacement;
            replacement.pawnState = PawnState.inPlay;
            return false;
        } else { 
            Debug.Log("Switching " + currentPawn.name + " for " + replacement.name);
            currentPawn.pushOut();
            currentPawn = replacement;
            replacement.pawnState = PawnState.inPlay;
            return true;
        }
    }
    
}

public enum SquareType { Normal, Rosette, Enter, Exit }
