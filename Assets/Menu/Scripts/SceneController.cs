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
    public GameObject loadingScreen;
    public Camera persistentCamera;

    List<string> sceneStack = new List<string>();

    public static bool ready = false;

    IEnumerator Start() {
        instance = this;
        yield return LoadMenu();
        ready = true;
    }

    public IEnumerator Load(string sceneName) {
        VisualBoardController.instance.Hide(VisualBoardController.instance.transform);
        loadingScreen.SetActive(true);
        persistentCamera.enabled = true;
        var obj = GameObject.FindGameObjectWithTag("MainCamera");
        if (obj != null)
            obj.GetComponent<Camera>().enabled = false;
        if (sceneStack.Count > 0) {
            AsyncOperation unloaded = SceneManager.UnloadSceneAsync(sceneStack[sceneStack.Count-1]);
            if (unloaded != null)
            while (!unloaded.isDone) {
                yield return null;
            }
        }
        sceneStack.Add(sceneName);
        AsyncOperation loaded = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loaded.isDone) {
            yield return null;
        }
        yield return StartUp.instance.OnSceneChanged(sceneName);
        persistentCamera.enabled = false;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;
        loadingScreen.SetActive(false);
        VisualBoardController.instance.Show(VisualBoardController.instance.transform);
        yield return null;
    }

    public IEnumerator BackLoad() {
        if (sceneStack.Count > 0) {
            string sceneName = sceneStack[sceneStack.Count-1];
            AsyncOperation loaded = SceneManager.LoadSceneAsync(sceneName);
            sceneStack.RemoveAt(sceneStack.Count-1);
            while (!loaded.isDone) {
                yield return null;
            }
            yield return StartUp.instance.OnSceneChanged(sceneName);
        }
    }

    public Coroutine LoadSelection() {
        return StartCoroutine(Load(SELECTION));
    }

    public Coroutine LoadMenu() {
        return StartCoroutine(Load(MENU));
    }

    public Coroutine LoadGame() {
        return StartCoroutine(Load(GAME));
    }

    public Coroutine LoadEdit() {
        return StartCoroutine(Load(EDIT));
    }

}
