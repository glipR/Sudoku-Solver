using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUp : MonoBehaviour {

    public static StartUp instance;

    void Start() {
        instance = this;
        DontDestroyOnLoad(this);
        OnSceneChanged(SceneController.MENU);
    }

    public void OnSceneChanged(string sceneName) {
        var canvas = transform.Find("Canvas").GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        VisualBoardController.instance.SetView(sceneName);
        if (sceneName == SceneController.SELECTION) {
            VisualBoardController.instance.interactionState = VisualBoardController.InteractionState.VIEWING;
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() => {
                if (ListController.instance.clicked)
                    SceneController.instance.LoadGame();
                else
                    Debug.Log("Select a board!");
            });
        } else if (sceneName == SceneController.GAME) {
            VisualBoardController.instance.interactionState = VisualBoardController.InteractionState.PLAYING;
            GameObject.Find("SolveButton").GetComponent<Button>().onClick.AddListener(VisualBoardController.instance.SolveBoard);
            GameObject.Find("HintButton").GetComponent<Button>().onClick.AddListener(VisualBoardController.instance.GetHint);
            GameObject.Find("CheckButton").GetComponent<Button>().onClick.AddListener(VisualBoardController.instance.GenerateErrors);
        }
    }

}
