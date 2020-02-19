using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateModal : MonoBehaviour {

    public GameObject VariantSuggestionsPrefab;
    public GameObject VariantTagPrefab;
    public List<GameObject> VariantSuggestions;
    public List<GameObject> VariantTagPool = new List<GameObject>();

    private List<VariantList.VariantType> selectedVariants;

    private void Awake() {
        var tagInp = transform.Find("Tags").GetComponent<TMP_InputField>();
        tagInp.onValueChanged.AddListener((s) => {
            SetTagsDropdown(VariantList.SuggestedVariants(s));
        });
        tagInp.onSubmit.AddListener((s) => {
            var res = VariantList.SuggestedVariants(s);
            if (res.Count == 0) return;
            ToggleVariant(res[0]);
            EventSystem.current.SetSelectedGameObject(transform.Find("Tags").gameObject, null);
            transform.Find("Tags").GetComponent<TMP_InputField>().ActivateInputField();
        });
        VariantSuggestions = new List<GameObject>();
        for (int i=0; i<5; i++) {
            var g = Instantiate(VariantSuggestionsPrefab, transform);
            g.transform.localPosition += new Vector3(0, -i * g.GetComponent<RectTransform>().sizeDelta.y, 0);
            g.SetActive(false);
            VariantSuggestions.Add(g);
        }
        selectedVariants = new List<VariantList.VariantType>();
        selectedVariants.Add(VariantList.VariantType.Base);
    }

    public void SetTagsDropdown(List<VariantList.VariantType> v) {
        foreach (GameObject g in VariantSuggestions) g.SetActive(false);
        int i=0;
        foreach (VariantList.VariantType x in v) {
            VariantSuggestions[i].transform.Find("Text").GetComponent<TextMeshProUGUI>().text = x.ToString();
            VariantSuggestions[i].transform.GetComponent<Button>().onClick.RemoveAllListeners();
            VariantSuggestions[i].transform.GetComponent<Button>().onClick.AddListener(() => ToggleVariant(x));
            VariantSuggestions[i].SetActive(true);
            i++;
        }
    }

    public void ToggleVariant(VariantList.VariantType v) {
        foreach (GameObject g in VariantSuggestions) g.SetActive(false);
        var tagInp = this.transform.Find("Tags").GetComponent<TMP_InputField>();
        tagInp.text = "";
        if (selectedVariants.Contains(v)) selectedVariants.Remove(v);
        else selectedVariants.Add(v);
        UpdateVisualTags();
    }

    public void UpdateVisualTags() {
        int j=0;
        foreach (var v in selectedVariants) {
            // Don't show base variant.
            if (v == VariantList.VariantType.Base) continue;
            if (VariantTagPool.Count <= j) {
                var t = Instantiate(VariantTagPrefab, transform);
                // This positions two tags per line.
                var rt = t.GetComponent<RectTransform>();
                if (j % 2 == 1) {
                    // Translate halfway
                    rt.localPosition += new Vector3(rt.sizeDelta.x * 1.14f, 0, 0);
                }
                rt.localPosition -= new Vector3(0, (j / 2) * rt.sizeDelta.y * 1.2f, 0);
                t.GetComponentInChildren<TextMeshProUGUI>().text = v.ToString();
                VariantTagPool.Add(t);
            } else {
                VariantTagPool[j].GetComponentInChildren<TextMeshProUGUI>().text = v.ToString();
                VariantTagPool[j].SetActive(true);
            }
            j++;
        }
        for (; j<VariantTagPool.Count; j++) VariantTagPool[j].SetActive(false);
    }

    public void Submit() {
        string title = transform.Find("Filename").GetComponent<TMP_InputField>().text;
        var variant_strings = new string[selectedVariants.Count];
        for (int i=0; i<selectedVariants.Count; i++) variant_strings[i] = selectedVariants[i].ToString();
        Sudoku s = new Sudoku(variant_strings);
        var obj = new BoardSerializer.SerializedBoard(s);
        string filename = "Testing/" + title + ".json";
        StreamWriter sr = File.CreateText(filename);
        sr.WriteLine(JsonUtility.ToJson(obj));
        sr.Close();
        VisualBoardController.instance.sudoku = obj.Deserialized();
        VisualBoardController.instance.ResetView();
        VisualBoardController.instance.boardName = title;
        SceneController.instance.LoadEdit();
    }

}
