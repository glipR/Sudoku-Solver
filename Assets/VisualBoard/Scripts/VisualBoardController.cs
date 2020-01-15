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

    // Cached display properties
    private Vector2 beginningVerticalPos;
    private Vector2 beginningHorizontalPos;
    private Vector2 largeLineLengths;
    private Vector2 smallLineLengths;
    private float thickWidth;
    private GameObject backdrop;

    private void Start() {
        GenerateDisplayConstants();
        GenerateBorders();
    }

    private void GenerateDisplayConstants() {
        backdrop = transform.Find("WhiteBackdrop").gameObject;
        backdropDimensions = backdrop.GetComponent<RectTransform>().sizeDelta;
        thickWidth = ThickLine.GetComponent<RectTransform>().sizeDelta.y;
        beginningVerticalPos = new Vector2(0, -thickWidth / 2f);
        beginningHorizontalPos = new Vector2(-backdropDimensions.x / 2f + thickWidth / 2f, -backdropDimensions.y / 2f);
        largeLineLengths = new Vector2((backdropDimensions.y - thickWidth) / (settings.numVerticalThicks - 1f), (backdropDimensions.x - thickWidth) / (settings.numHorizontalThicks - 1f));
        smallLineLengths = new Vector2(largeLineLengths.x / (settings.numVerticalThins + 1f), largeLineLengths.y / (settings.numHorizontalThins + 1f));
    }

    private void GenerateBorders() {

        // Create the thick lines, and then each thin line.
        for (int thickIndex=0; thickIndex<settings.numVerticalThicks; thickIndex++) {
            RectTransform top_line = Instantiate(ThickLine, backdrop.transform).GetComponent<RectTransform>();
            top_line.gameObject.name = "VerticalThick " + thickIndex;
            top_line.anchoredPosition = beginningVerticalPos + new Vector2(0, -largeLineLengths.x * thickIndex);
            if (thickIndex != settings.numVerticalThicks-1)
            for (int thinIndex=1; thinIndex<=settings.numVerticalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, backdrop.transform).GetComponent<RectTransform>();
                thin_line.gameObject.name = "VerticalThin " + thickIndex + " " + thinIndex;
                thin_line.anchoredPosition = top_line.anchoredPosition - new Vector2(0, smallLineLengths.x * thinIndex);
            }
        }
        for (int thickIndex=0; thickIndex<settings.numHorizontalThicks; thickIndex++) {
            RectTransform left_line = Instantiate(ThickLine, backdrop.transform).GetComponent<RectTransform>();
            left_line.gameObject.name = "HorizontalThick " + thickIndex;
            left_line.anchoredPosition = beginningHorizontalPos + new Vector2(largeLineLengths.y * thickIndex , 0f);
            left_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
            for (int thinIndex=1; thinIndex<=settings.numHorizontalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, backdrop.transform).GetComponent<RectTransform>();
                thin_line.gameObject.name = "HorizontalThin " + thickIndex + " " + thinIndex;
                thin_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
                thin_line.anchoredPosition = left_line.anchoredPosition + new Vector2(smallLineLengths.y * thinIndex, 0);
            }
        }

    }

}
