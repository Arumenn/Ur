using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public string player1Name;
    public string player2Name;

    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;

    public Color colorPlayer1 = new Color(255, 0, 90);
    public Color colorPlayer2 = new Color(0, 139, 255);

    // Use this for initialization
    private IEnumerator Start() {
        //waits for the localization manager to be ready
        while (!LocalizationManager.instance.GetIsReady()) {
            yield return null;
        }
        DontDestroyOnLoad(gameObject);
    }

    public string GetPlayerName(int playerIndex) {
        if (playerIndex == 2) {
            return player2Name;
        } else {
            return player1Name;
        }
    }
}
