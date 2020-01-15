﻿using System.Collections;
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
    [SerializeField]
    private GameObject BoxObject = null;
    private Vector2 backdropDimensions;

    // Cached display properties
    private Vector2 beginningVerticalPos;
    private Vector2 beginningHorizontalPos;
    private Vector2 largeLineLengths;
    private Vector2 smallLineLengths;
    private float thickWidth;
    private float thinWidth;
    private GameObject backdrop;

    // Publically accessible boxes.
    public BoxController[,] boxes;

    private void Start() {
        GenerateDisplayConstants();
        GenerateBorders();
        GenerateBoxes();
    }

    private void GenerateDisplayConstants() {
        backdrop = transform.Find("WhiteBackdrop").gameObject;
        backdropDimensions = backdrop.GetComponent<RectTransform>().sizeDelta;
        thickWidth = ThickLine.GetComponent<RectTransform>().sizeDelta.y;
        thinWidth = ThinLine.GetComponent<RectTransform>().sizeDelta.y;
        beginningVerticalPos = new Vector2(0, -thickWidth / 2f);
        beginningHorizontalPos = new Vector2(-backdropDimensions.x / 2f + thickWidth / 2f, -backdropDimensions.y / 2f);
        largeLineLengths = new Vector2((backdropDimensions.x - thickWidth) / (settings.numHorizontalThicks - 1f), (backdropDimensions.y - thickWidth) / (settings.numVerticalThicks - 1f));
        smallLineLengths = new Vector2((largeLineLengths.x - thickWidth + thinWidth) / (settings.numHorizontalThins + 1f), (largeLineLengths.y - thickWidth + thinWidth) / (settings.numVerticalThins + 1f));
    }

    private void GenerateBorders() {

        // Create the thick lines, and then each thin line.
        for (int thickIndex=0; thickIndex<settings.numVerticalThicks; thickIndex++) {
            RectTransform top_line = Instantiate(ThickLine, backdrop.transform).GetComponent<RectTransform>();
            top_line.gameObject.name = "VerticalThick " + thickIndex;
            top_line.anchoredPosition = beginningVerticalPos + new Vector2(0, -largeLineLengths.y * thickIndex);
            if (thickIndex != settings.numVerticalThicks-1)
            for (int thinIndex=1; thinIndex<=settings.numVerticalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, backdrop.transform).GetComponent<RectTransform>();
                thin_line.gameObject.name = "VerticalThin " + thickIndex + " " + thinIndex;
                thin_line.anchoredPosition = top_line.anchoredPosition - new Vector2(0, (thickWidth - thinWidth) / 2f + smallLineLengths.y * thinIndex);
            }
        }
        for (int thickIndex=0; thickIndex<settings.numHorizontalThicks; thickIndex++) {
            RectTransform left_line = Instantiate(ThickLine, backdrop.transform).GetComponent<RectTransform>();
            left_line.gameObject.name = "HorizontalThick " + thickIndex;
            left_line.anchoredPosition = beginningHorizontalPos + new Vector2(largeLineLengths.x * thickIndex , 0f);
            left_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
            if (thickIndex != settings.numHorizontalThicks-1)
            for (int thinIndex=1; thinIndex<=settings.numHorizontalThins; thinIndex++) {
                RectTransform thin_line = Instantiate(ThinLine, backdrop.transform).GetComponent<RectTransform>();
                thin_line.gameObject.name = "HorizontalThin " + thickIndex + " " + thinIndex;
                thin_line.rotation = Quaternion.AngleAxis(90f, Vector3.forward);
                thin_line.anchoredPosition = left_line.anchoredPosition + new Vector2((thickWidth - thinWidth) / 2f + smallLineLengths.x * thinIndex, 0);
            }
        }

    }

    private void GenerateBoxes() {
        boxes = new BoxController[(settings.numHorizontalThicks - 1) * (settings.numHorizontalThins + 1), (settings.numVerticalThicks - 1) * (settings.numVerticalThins + 1)];
        for (int thickH=0; thickH < settings.numHorizontalThicks-1; thickH++) {
            for (int thickV=0; thickV < settings.numVerticalThicks-1; thickV++) {
                for (int thinH=0; thinH < settings.numHorizontalThins+1; thinH++) {
                    for (int thinV=0; thinV < settings.numVerticalThins+1; thinV++) {
                        RectTransform box = Instantiate(BoxObject, backdrop.transform).GetComponent<RectTransform>();
                        box.anchoredPosition = new Vector2(
                            thickWidth + (smallLineLengths.x - thinWidth) / 2f + largeLineLengths.x * thickH + smallLineLengths.x * thinH,
                            -thickWidth - (smallLineLengths.y - thinWidth) / 2f - largeLineLengths.y * thickV - smallLineLengths.y * thinV
                        );
                        box.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, smallLineLengths.y - thinWidth);
                        box.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, smallLineLengths.x - thinWidth);
                        int inx = thickH * (settings.numHorizontalThins + 1) + thinH;
                        int iny = thickV * (settings.numVerticalThins + 1) + thinV;
                        box.gameObject.name = "Box (" + (inx+1) + ", " + (iny+1) + ")";
                        boxes[inx, iny] = box.GetComponent<BoxController>();
                        boxes[inx, iny].position = (inx, iny);
                    }
                }
            }
        }
    }

}
