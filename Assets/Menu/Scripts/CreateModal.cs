using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateModal : MonoBehaviour {

    public GameObject VariantSuggestionsPrefab;
    public List<GameObject> VariantSuggestions;

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
    }

    public void SetTagsDropdown(List<VariantList.VariantType> v) {
        foreach (GameObject g in VariantSuggestions) g.SetActive(false);
        int i=0;
        foreach (VariantList.VariantType x in v) {
            VariantSuggestions[i].transform.Find("Text").GetComponent<TextMeshProUGUI>().text = x.ToString();
            VariantSuggestions[i].transform.GetComponent<Button>().onClick.AddListener(() => ToggleVariant(x));
            VariantSuggestions[i].SetActive(true);
            i++;
        }
    }

    public void ToggleVariant(VariantList.VariantType v) {
        foreach (GameObject g in VariantSuggestions) g.SetActive(false);
        var tagInp = this.transform.Find("Tags").GetComponent<TMP_InputField>();
        tagInp.text = "";
    }

}
