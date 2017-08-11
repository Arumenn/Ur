using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartupManager : MonoBehaviour {

    public Text uiPlayer1Name;
    public Text uiPlayer2Name;

    // Use this for initialization
    private IEnumerator Start() {
        //waits for the localization manager to be ready
        while (!LocalizationManager.instance.GetIsReady()) {
            yield return null;
        }
    }

    public void LoadFr() {
        LocalizationManager.instance.LoadLocalizedText("localizedText_fr.json");
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadEn() {
        LocalizationManager.instance.LoadLocalizedText("localizedText_en.json");
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame() {
        PlayerManager pm = FindObjectOfType<PlayerManager>();
        if (pm != null) {
            pm.player1Name = uiPlayer1Name.text;
            pm.player2Name = uiPlayer2Name.text;
            if (pm.player1Name == "") { pm.player1Name = LocalizationManager.instance.GetLocalizedValue("PlaceholderPlayer1"); }
            if (pm.player2Name == "") { pm.player2Name = LocalizationManager.instance.GetLocalizedValue("PlaceholderPlayer2"); }
            SceneManager.LoadScene("Egypt");
        } else {
            Debug.LogError("No PlayerManager found");
        }
    }

}