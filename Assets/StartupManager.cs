using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour {

    // Use this for initialization
    private IEnumerator Start() {
        //waits for the localization manager to be ready
        while (!LocalizationManager.instance.GetIsReady()) {
            yield return null;
        }
    }

    public void LoadFr() {
        LocalizationManager.instance.LoadLocalizedText("localizedText_fr.json");
        SceneManager.LoadScene("Sandbox");
    }

    public void LoadEn() {
        LocalizationManager.instance.LoadLocalizedText("localizedText_en.json");
        SceneManager.LoadScene("Sandbox");
    }

}