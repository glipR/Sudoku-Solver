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

    public IEnumerator Load(string sceneName) {
        sceneStack.Add(sceneName);
        AsyncOperation loaded = SceneManager.LoadSceneAsync(sceneName);
        while (!loaded.isDone) {
            yield return null;
        }
        StartUp.instance.OnSceneChanged();
    }

    public IEnumerator BackLoad() {
        if (sceneStack.Count > 0) {
            AsyncOperation loaded = SceneManager.LoadSceneAsync(sceneStack[sceneStack.Count-1]);
            sceneStack.RemoveAt(sceneStack.Count-1);
            while (!loaded.isDone) {
                yield return null;
            }
            StartUp.instance.OnSceneChanged();
        }
    }

    public void LoadSelection() {
        StartCoroutine(Load(SELECTION));
    }

    public void LoadMenu() {
        StartCoroutine(Load(MENU));
    }

}
