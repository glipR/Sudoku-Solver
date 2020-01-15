using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxController : MonoBehaviour {

    public (int x, int y) position;
    public Color currentColor = new Color(1, 1, 1, 1);
    public string currentCentre = "";

    public void SetColor(Color c) {
        currentColor = c;
        var img = GetComponent<Image>();
        img.color = c;
    }

    public void SetCentre(string s) {
        currentCentre = s;
        var txt = transform.Find("CentreNum").GetComponent<TextMeshProUGUI>();
        txt.text = s;
    }

}
