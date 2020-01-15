using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxController : MonoBehaviour {

    public (int x, int y) position;
    public Color currentColor = new Color(1, 1, 1, 1);
    public string currentCentre = "";

    public void SetColor(Color c, bool update) {
        if (update)
            currentColor = c;
        var img = GetComponent<Image>();
        img.color = c;
    }

    public void SetCentre(string s) {
        currentCentre = s;
        var txt = transform.Find("CentreNum").GetComponent<TextMeshProUGUI>();
        txt.text = s;
    }

    public void SetSize(Vector2 size) {
        var rt = GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        var bc = GetComponent<BoxCollider2D>();
        bc.size = size;
    }

    private void OnMouseDown() {
        BoxSelectionManager.instance.SetSelected(this);
    }

}
