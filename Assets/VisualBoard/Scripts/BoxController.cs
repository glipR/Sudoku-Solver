using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxController : MonoBehaviour {

    public (int x, int y) position;
    public Color currentColor = new Color(1, 1, 1, 1);
    public string currentFull = "";
    public List<int> cornerElements;
    public List<int> centreElements;

    public bool centreMode;

    private void Start() {
        centreMode = false;
    }

    public void SetColor(Color c, bool update) {
        if (update)
            currentColor = c;
        var img = GetComponent<Image>();
        img.color = c;
    }

    public void SetFull(string s) {
        currentFull = s;
        var txt = transform.Find("FullNum").GetComponent<TextMeshProUGUI>();
        txt.text = s;
        var centre = transform.Find("CentreNum").GetComponent<TextMeshProUGUI>();
        centre.text = "";
        centreMode = s != "";
    }

    public void ToggleCentre(int e) {
        int found = -1;
        for (int i=0; i<centreElements.Count; i++) if (centreElements[i] == e) found = i;
        if (found != -1)
            centreElements.RemoveAt(found);
        else centreElements.Add(e);
        string x = "";
        foreach (var v in centreElements) x = x + v.ToString();
        if (!centreMode) {
            var text = transform.Find("CentreNum").GetComponent<TextMeshProUGUI>();
            text.text = x;
        }
    }

    public void SetSize(Vector2 size) {
        // Set the collider and box size.
        var rt = GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        var bc = GetComponent<BoxCollider2D>();
        bc.size = size;
        // Set the inner text sizes.
        var fn = transform.Find("FullNum").GetComponent<RectTransform>();
        fn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        fn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        var cn = transform.Find("CentreNum").GetComponent<RectTransform>();
        cn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x * 0.7f);
        cn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y * 0.4f);
    }

    private void OnMouseDown() {
        BoxSelectionManager.instance.SetSelected(this);
    }

}
