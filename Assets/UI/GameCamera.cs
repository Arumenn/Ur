using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

    public Camera[] cameras;
    private int currentCameraIndex;
    private GameManager gm;

	// Use this for initialization
	void Start () {
        currentCameraIndex = 0;

        for (int i = 1; i < cameras.Length; i++) {
            cameras[i].gameObject.SetActive(false);
        }

        if (cameras.Length > 0) {
            cameras[0].gameObject.SetActive(true);
        }

        gm = GameObject.FindObjectOfType<GameManager>();
    }
	
	// Update is called once per frame
	void LateUpdate () {
		if (gm.currentPlayer - 1 != currentCameraIndex) {
            for (int i = 1; i < cameras.Length; i++) {
                cameras[i].gameObject.SetActive(false);
            }
            currentCameraIndex = gm.currentPlayer - 1;
            cameras[currentCameraIndex].gameObject.SetActive(true);
            Debug.Log("Camera " + currentCameraIndex + " is now active for player " + gm.currentPlayer);
        }
	}

    public Camera getCurrentCamera() {
        return cameras[currentCameraIndex];
    }
}
