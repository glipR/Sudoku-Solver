using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour {

    public static StartUp instance;

    void Start() {
        instance = this;
        DontDestroyOnLoad(this);
        OnSceneChanged();
    }

    public void OnSceneChanged() {
        var canvas = transform.Find("Canvas").GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

}
