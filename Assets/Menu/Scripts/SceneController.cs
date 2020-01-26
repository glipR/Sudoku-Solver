using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    static string SELECTION = "SelectionScene";
    static string MENU = "MenuScene";
    static string GAME = "SampleScene";

    public static SceneController instance;

    List<string> sceneStack = new List<string>();

    void Start() {
        instance = this;
        sceneStack.Add(MENU);
    }

    public void Load(string sceneName) {
        sceneStack.Add(sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void BackLoad() {
        if (sceneStack.Count > 0) {
            SceneManager.LoadScene(sceneStack[sceneStack.Count-1]);
            sceneStack.RemoveAt(sceneStack.Count-1);
        }
    }

    public void LoadSelection() {
        Load(SELECTION);
    }

    public void LoadMenu() {
        Load(MENU);
    }

}
