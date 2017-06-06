using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {

    [SerializeField] public SquareType squareType = SquareType.Normal;
    [SerializeField] public SquareTerritory squareTerritory = SquareTerritory.Neutral;

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

    public void switchWith(Pawn replacement) {
        if (currentPawn == null) {
            currentPawn = replacement;
            Debug.Log("Set current spawn to " + replacement.name);
        } else {
            Debug.Log("Switching " + currentPawn.name + " for " + replacement.name);
            currentPawn.pushOut();
            currentPawn = replacement;
            replacement.pawnState = PawnState.inPlay;
        }
    }
    
}

public enum SquareType { Normal, Rosette, Enter, Exit }
public enum SquareTerritory { Neutral, Player1, Player2 }
