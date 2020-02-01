using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxController : MonoBehaviour {

    public static int topBox = -1;
    public static int botBox = -2;

    public static float cornerRatio = 0.3f;
    public static Color givenColor = new Color(0f, 0f, 1f);

    public static GameObject layerPrefab;

    public (int x, int y) position;
    public Color currentColor = new Color(1, 1, 1, 1);
    public string currentFull = "";
    public List<int> cornerElements;
    public List<int> centreElements;

    public string currentVisibleFull { get { return centreMode ? currentFull : ""; } }

    public bool centreMode;
    public bool given = false;
    public bool editable = true;

    private Vector2[] anchors = {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(1, -1),
        new Vector2(0.5f, 0),
        new Vector2(0.5f, -1),
        new Vector2(0, 0.5f),
        new Vector2(1, 0.5f)
    };

    private void Awake() {
        centreMode = false;
    }

    public void SetUneditable() {
        editable = false;
    }

    public void SetColor(Color c) {
        currentColor = c;
        var img = GetComponent<Image>();
        img.color = c;
    }

    public void SetHighlight(Color c) {
        var img = GetComponent<Image>();
        Color newColor = (1-c.a) * currentColor + c * c.a;
        newColor.a = 1;
        img.color = newColor;
    }

    public void StartFlash(Color color, float lowAlpha, float highAlpha, int numFlashes, float flashTime) {
        StartCoroutine(GenerateFlash(color, lowAlpha, highAlpha, numFlashes, flashTime));
    }

    public void StartFlash(Color color, int numFlashes, float flashTime) {
        StartFlash(color, 0f, 0.3f, numFlashes, flashTime);
    }

    public void StartFlash(Color color) {
        StartFlash(color, 3, 2);
    }

    private IEnumerator GenerateFlash(Color color, float lowAlpha, float highAlpha, int numFlashes, float flashTime) {
        Color topHighlight = color;
        color.a = highAlpha;
        Color botHighlight = color;
        color.a = lowAlpha;
        for (int fNum=0; fNum<numFlashes; fNum++) {
            int numIterations = 20;
            for (int i=0; i<numIterations; i++) {
                SetHighlight(Color.Lerp(botHighlight, topHighlight, i < numIterations/2 ? i / (float)(numIterations/2) : (numIterations - i) / (float)(numIterations - numIterations/2)));
                yield return new WaitForSeconds(flashTime / (float)(numFlashes * numIterations));
            }
        }
        SetHighlight(new Color(0, 0, 0, 0));
    }

    public void SetFull(string s, bool fromUI) {
        if (given && fromUI) return;
        currentFull = s;
        var txt = transform.Find("FullNum").GetComponent<TextMeshProUGUI>();
        txt.text = s;
        if (given) txt.color = givenColor;
        // Clear centre text.
        var centre = transform.Find("CentreNum").GetComponent<TextMeshProUGUI>();
        centre.text = "";
        centreMode = s != "";
        // Clear corner text.
        for (int i=0; i<8; i++) {
            var t = transform.Find("Corner" + (i+1)).GetComponent<TextMeshProUGUI>();
            t.text = "";
        }
    }

    public void ToggleCentre(int e) {
        if (given) return;
        int index = centreElements.BinarySearch(e);
        if (index >= 0)
            centreElements.RemoveAt(index);
        else {
            // Insert in Order.
            centreElements.Insert(~index, e);
        }
        string x = "";
        foreach (var v in centreElements) x = x + v.ToString();
        if (!centreMode) {
            var text = transform.Find("CentreNum").GetComponent<TextMeshProUGUI>();
            text.text = x;
        }
    }

    public void ToggleCorner(int e) {
        if (given) return;
        int index = cornerElements.BinarySearch(e);
        if (index >= 0)
            cornerElements.RemoveAt(index);
        else {
            // Insert in Order.
            cornerElements.Insert(~index, e);
        }
        if (!centreMode)
        for (int i=0; i<8; i++) {
            var t = transform.Find("Corner" + (i+1)).GetComponent<TextMeshProUGUI>();
            t.text = i < cornerElements.Count ? cornerElements[i].ToString() : "";
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
        cn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x * 0.85f);
        cn.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y * 0.4f);
        for (int i=0; i<8; i++) {
            var t = transform.Find("Corner" + (i+1)).GetComponent<RectTransform>();
            t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x * cornerRatio);
            t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y * cornerRatio);
            t.anchoredPosition = anchors[i] + (
                anchors[i].x == 0 ? (
                    new Vector2(size.x * cornerRatio * 0.5f, 0)
                ) : (
                    anchors[i].x == 1 ? new Vector2(-size.x * cornerRatio * 0.5f, 0) : new Vector2(0, 0)
                )
            ) + (
                anchors[i].y == 0 ? (
                    new Vector2(0, -size.y * cornerRatio * 0.5f)
                ) : (
                    anchors[i].y == -1 ? new Vector2(0, size.y * cornerRatio * 0.5f) : new Vector2(0, 0)
                )
            );
        }
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>()) {
            text.fontSize *= Mathf.Min(size.x, size.y) / (70f);
        }
    }

    public void Clear() {
        if (given) return;
        this.currentColor = new Color(1, 1, 1, 1);
        this.centreElements.Clear();
        this.cornerElements.Clear();
        this.SetFull("", false);
        this.centreMode = false;
    }

    private void OnMouseDown() {
        if (!editable) return;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) BoxSelectionManager.instance.ToggleSelected(this);
        else BoxSelectionManager.instance.SetSelected(this);
    }

    private void OnMouseEnter() {
        if (!editable) return;
        if (Input.GetMouseButton(0)) BoxSelectionManager.instance.EnsureSelected(this);
    }

    // Overlays and Underlays
    public void AddUnderlay(Sprite sprite) {
        var obj = Instantiate(layerPrefab, transform);
        var rt = obj.GetComponent<RectTransform>();
        var img = obj.GetComponent<Image>();
        img.sprite = sprite;
    }

}
