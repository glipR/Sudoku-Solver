using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUp : MonoBehaviour {

    public static StartUp instance;

    IEnumerator Start() {
        instance = this;
        while (!SceneController.ready) yield return null;
        while (!VisualBoardController.ready) yield return null;
    }

    public IEnumerator OnSceneChanged(string sceneName) {
        Debug.Log(sceneName);
        var canvas = transform.Find("Canvas").GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        yield return VisualBoardController.instance.SetView(sceneName);
        if (sceneName == SceneController.MENU) {
            GameObject.Find("SelectionButton").GetComponent<Button>().onClick.AddListener(() => {
                SceneController.instance.LoadSelection();
            });
            GameObject.Find("CreateButton").GetComponent<Button>().onClick.AddListener(() => {
                ModalSpawner.instance.SpawnModal(ModalSpawner.ModalType.CREATE);
            });
        }
        else if (sceneName == SceneController.SELECTION) {
            VisualBoardController.instance.interactionState = VisualBoardController.InteractionState.VIEWING;
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() => {
                if (ListController.instance.clicked)
                    SceneController.instance.LoadGame();
                else
                    Debug.Log("Select a board!");
            });
            GameObject.Find("EditButton").GetComponent<Button>().onClick.AddListener(() => {
                if (ListController.instance.clicked)
                    SceneController.instance.LoadEdit();
                else
                    Debug.Log("Select a board!");
            });
        } else if (sceneName == SceneController.GAME) {
            VisualBoardController.instance.interactionState = VisualBoardController.InteractionState.PLAYING;
            GameObject.Find("SolveButton").GetComponent<Button>().onClick.AddListener(VisualBoardController.instance.SolveBoard);
            GameObject.Find("HintButton").GetComponent<Button>().onClick.AddListener(VisualBoardController.instance.GetHint);
            GameObject.Find("CheckButton").GetComponent<Button>().onClick.AddListener(VisualBoardController.instance.GenerateErrors);
        } else if (sceneName == SceneController.EDIT) {
            VisualBoardController.instance.interactionState = VisualBoardController.InteractionState.EDITING;
            GameObject.Find("CheckButton").GetComponent<Button>().onClick.AddListener(VisualBoardController.instance.GenerateErrors);
            GameObject.Find("SolveButton").GetComponent<Button>().onClick.AddListener(VisualBoardController.instance.SolveBoard);
            GameObject.Find("SaveButton").GetComponent<Button>().onClick.AddListener(VisualBoardController.instance.SaveBoard);
        }
        yield return null;
    }

}
