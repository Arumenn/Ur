﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour {

    public static LocalizationManager instance;

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    // Use this for initialization
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        //loads default language based on system
        if (Application.systemLanguage == SystemLanguage.French) {
            LoadLocalizedText("localizedText_fr.json");
        } else {
            LoadLocalizedText("localizedText_en.json");
        }
    }

    public void LoadLocalizedText(string fileName) {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath)) {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++) {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
        } else {
            Debug.LogError("Cannot find file!");
        }

        isReady = true;
    }

    public string GetLocalizedValue(string key, params string[] replace) {
        string result = missingTextString;
        if (localizedText.ContainsKey(key)) {
            result = localizedText[key];
            string old;
            for (int i = 0; i< replace.Length; i++) {
                old = "%" + (i + 1);
                result = result.Replace(old, replace[i]);
            }
        }

        return result;

    }

    public bool GetIsReady() {
        return isReady;
    }

}