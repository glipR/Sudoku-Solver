using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public static string SELECTION = "SelectionScene";
    public static string MENU = "MenuScene";
    public static string GAME = "GameScene";
    public static string EDIT = "EditScene";

    public static SceneController instance;

    List<string> sceneStack = new List<string>();

    void Start() {
        instance = this;
        sceneStack.Add(MENU);
    }

    public IEnumerator Load(string sceneName) {
        sceneStack.Add(sceneName);
        AsyncOperation loaded = SceneManager.LoadSceneAsync(sceneName);
        while (!loaded.isDone) {
            yield return null;
        }
        StartUp.instance.OnSceneChanged(sceneName);
    }

    public IEnumerator BackLoad() {
        if (sceneStack.Count > 0) {
            string sceneName = sceneStack[sceneStack.Count-1];
            AsyncOperation loaded = SceneManager.LoadSceneAsync(sceneName);
            sceneStack.RemoveAt(sceneStack.Count-1);
            while (!loaded.isDone) {
                yield return null;
            }
            StartUp.instance.OnSceneChanged(sceneName);
        }
    }

    public void LoadSelection() {
        StartCoroutine(Load(SELECTION));
    }

    public void LoadMenu() {
        StartCoroutine(Load(MENU));
    }

    public void LoadGame() {
        StartCoroutine(Load(GAME));
    }

    public void LoadEdit() {
        StartCoroutine(Load(EDIT));
    }

}
