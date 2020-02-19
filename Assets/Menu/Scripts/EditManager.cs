using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditManager : MonoBehaviour {

    public static EditManager instance;
    public List<(Variant v, IEditManager e)> editors;
    public GameObject editButtonPrefab;

    private IEnumerator Start() {
        while (!BoxSelectionManager.ready) yield return null;
        LoadEditorOptions();
        InitialiseEditor(-1);
        instance = this;
    }

    public void LoadEditorOptions() {
        editors = new List<(Variant v, IEditManager e)>();
        foreach (Variant v in VisualBoardController.instance.sudoku.variants) {
            foreach (var e in v.customEditors) {
                editors.Add((v, e));
            }
        }
        // Generate buttons on the screen, and add listeners.
        if (editors.Count > 0) {
            var box = GameObject.Find("EditButtons").transform;
            var baseButton = Instantiate(editButtonPrefab, box);
            baseButton.GetComponent<Button>().onClick.AddListener(() => InitialiseEditor(-1));
            baseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Default";
            for (int i=0; i<editors.Count; i++) {
                var newButton = Instantiate(editButtonPrefab, box);
                var x = i;
                newButton.GetComponent<Button>().onClick.AddListener(() => InitialiseEditor(x));
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = editors[i].v.type.ToString();
            }
        }
    }

    public void InitialiseEditor(int i) {
        BoxSelectionManager.instance.AddListeners();
        if (i >= 0) {
            editors[i].e.Initialise();
            editors[i].e.ModifyListeners();
        }
    }

}
