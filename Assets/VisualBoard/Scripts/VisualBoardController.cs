using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBoardSettings {
    public int numHorizontalThicks = 4;
    public int numVerticalThicks = 4;
    public int numHorizontalThins = 2;
    public int numVerticalThins = 2;

    public VisualBoardSettings() {

    }
}

public class VisualBoardController : MonoBehaviour {

    public VisualBoardSettings settings = new VisualBoardSettings();

    [SerializeField]
    private GameObject ThickLine = null;
    [SerializeField]
    private GameObject ThinLine = null;
    private Vector2 backdropDimensions;

    private void Start() {
        backdropDimensions = transform.Find("WhiteBackdrop").GetComponent<RectTransform>().sizeDelta;
        GenerateBorders();
    }

    private void GenerateBorders() {
        var backdrop = transform.Find("WhiteBackdrop");

        // Create the thick lines, and then each thin line.
        for (int thickIndex=0; thickIndex<settings.numVerticalThicks; thickIndex++) {
            RectTransform top_line = Instantiate(ThickLine, backdrop.transform).GetComponent<RectTransform>();
            top_line.gameObject.name = "VerticalThick " + thickIndex;
            top_line.anchoredPosition = new Vector2(0, -top_line.sizeDelta.y / 2f - (backdropDimensions.y - top_line.sizeDelta.y) * thickIndex / (settings.numVerticalThicks - 1f));
            if (thickIndex != settings.numVerticalThicks-1)
            for (int thinIndex=1; thinIndex<=settings.numVerticalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, backdrop.transform).GetComponent<RectTransform>();
                thin_line.gameObject.name = "VerticalThin " + thickIndex + " " + thinIndex;
                thin_line.anchoredPosition = top_line.anchoredPosition - new Vector2(0, (backdropDimensions.y - top_line.sizeDelta.y) / (settings.numVerticalThicks - 1f) * (thinIndex / (float)(settings.numVerticalThins+1)));
            }
        }
        for (int thickIndex=0; thickIndex<settings.numHorizontalThicks; thickIndex++) {
            RectTransform left_line = Instantiate(ThickLine, backdrop.transform).GetComponent<RectTransform>();
            left_line.gameObject.name = "HorizontalThick " + thickIndex;
            left_line.anchoredPosition = new Vector2(-backdropDimensions.x / 2f + left_line.sizeDelta.y / 2f + (backdropDimensions.x - left_line.sizeDelta.y) * thickIndex / (settings.numHorizontalThicks - 1f), -backdropDimensions.y / 2f);
            left_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
            for (int thinIndex=1; thinIndex<=settings.numHorizontalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, backdrop.transform).GetComponent<RectTransform>();
                thin_line.gameObject.name = "HorizontalThin " + thickIndex + " " + thinIndex;
                thin_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
                thin_line.anchoredPosition = left_line.anchoredPosition + new Vector2((backdropDimensions.x - left_line.sizeDelta.y) / (settings.numHorizontalThicks - 1f) * (thinIndex / ((float)settings.numVerticalThins+1)), 0);
            }
        }

    }

}
