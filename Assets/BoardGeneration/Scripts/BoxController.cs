using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxController : MonoBehaviour {

    public enum ActionType {
        FULL,
        CORNER,
        CENTRE,
        COLOR
    }
    public class BoxState {
        public Color color;
        public bool centreMode;
        public string full;
        public List<string> corners;
        public List<string> centres;

        public BoxState(BoxController bc) {
            color = bc.currentColor;
            centreMode = bc.centreMode;
            full = bc.currentFull;
            corners = bc.cornerElements;
            centres = bc.centreElements;
        }

        public void Apply(BoxController bc) {
            bc.Clear(false);
            bc.SetColor(color, false);
            if (centreMode) bc.SetFull(full, false);
            else {
                foreach (var x in centres)
                    bc.ToggleCentre(x, false);
                foreach (var x in corners)
                    bc.ToggleCorner(x, false);
            }
        }
    }
    public class Action {
        public BoxState prevState;
        public ActionType type;
    }

    public static int topBox = -1;
    public static int botBox = -2;

    public static float cornerRatio = 0.3f;
    public static Color givenColor = new Color(0, 19/255f, 87/255f);

    [SerializeField]
    public GameObject layerPrefab;
    public List<GameObject> layers = new List<GameObject>();

    public bool visible;
    public (int x, int y) position;
    public Color currentColor = new Color(1, 1, 1, 1);
    public string currentFull = "";
    public List<string> cornerElements = new List<string>();
    public List<string> centreElements = new List<string>();

    public string currentVisibleFull { get { return centreMode ? currentFull : ""; } }

    public bool centreMode;
    public bool given = false;

    public List<Action> prevActions = new List<Action>();

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
        var canvas = transform.Find("UnderlayCanvas").GetComponent<Canvas>();
        canvas.sortingLayerName = "TopCanvas";
        canvas.sortingOrder = 0;
        canvas = transform.Find("ColorCanvas").GetComponent<Canvas>();
        canvas.sortingLayerName = "TopCanvas";
        canvas.sortingOrder = -1;
        visible = true;
    }

    public void SetColor(Color c, bool actionAble) {
        if (actionAble) {
            Action a = new Action();
            a.type = ActionType.COLOR;
            a.prevState = new BoxState(this);
            prevActions.Add(a);
        }
        currentColor = c;
        var img = transform.Find("ColorCanvas/Image").GetComponent<Image>();
        img.color = c;
    }

    public void SetHighlight(Color c) {
        var img = transform.Find("ColorCanvas/Image").GetComponent<Image>();
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

    public void SetFull(string s, bool actionAble) {
        if (actionAble) {
            Action a = new Action();
            a.type = ActionType.COLOR;
            a.prevState = new BoxState(this);
            prevActions.Add(a);
        }
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

    public void ToggleCentre(string s, bool actionAble) {
        if (actionAble) {
            Action a = new Action();
            a.type = ActionType.COLOR;
            a.prevState = new BoxState(this);
            prevActions.Add(a);
        }
        if (given) return;
        int index = centreElements.BinarySearch(s);
        if (index >= 0)
            centreElements.RemoveAt(index);
        else {
            // Insert in Order.
            centreElements.Insert(~index, s);
        }
        string x = "";
        foreach (var v in centreElements) x = x + v.ToString();
        if (!centreMode) {
            var text = transform.Find("CentreNum").GetComponent<TextMeshProUGUI>();
            text.text = x;
        }
    }

    public void ToggleCorner(string s, bool actionAble) {
        if (actionAble) {
            Action a = new Action();
            a.type = ActionType.COLOR;
            a.prevState = new BoxState(this);
            prevActions.Add(a);
        }
        if (given) return;
        int index = cornerElements.BinarySearch(s);
        if (index >= 0)
            cornerElements.RemoveAt(index);
        else {
            // Insert in Order.
            cornerElements.Insert(~index, s);
        }
        string[] ordering = {
            "1",
            "12",
            "123",
            "1234",
            "15234",
            "152364",
            "1527364",
            "15278364",
            "152798364"
        };
        if (!centreMode)
        for (int i=0; i<9; i++) {
            if (i < cornerElements.Count) {
                var t = transform.Find("Corner" + (ordering[cornerElements.Count-1][i])).GetComponent<TextMeshProUGUI>();
                t.text = cornerElements[i].ToString();
            } else {
                var t = transform.Find("Corner" + (i+1)).GetComponent<TextMeshProUGUI>();
                t.text = "";
            }
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

    public void Clear(bool actionAble) {
        if (actionAble) {
            Action a = new Action();
            a.type = ActionType.COLOR;
            a.prevState = new BoxState(this);
            prevActions.Add(a);
        }
        this.given = false;
        this.currentColor = new Color(1, 1, 1, 1);
        this.centreElements.Clear();
        this.cornerElements.Clear();
        this.SetFull("", false);
        this.centreMode = false;
    }

    private void OnMouseDown() {
        if (!visible) return;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) BoxSelectionManager.instance.ToggleSelected.Invoke(this);
        else BoxSelectionManager.instance.SetSelected.Invoke(this);
    }

    private void OnMouseEnter() {
        if (!visible) return;
        if (Input.GetMouseButton(0)) BoxSelectionManager.instance.EnsureSelected.Invoke(this);
    }

    // Overlays and Underlays
    public void AddUnderlay(Sprite sprite, int rotation) {
        var obj = Instantiate(layerPrefab, transform.Find("UnderlayCanvas"));
        obj.transform.Rotate(Vector3.forward, rotation);
        var img = obj.GetComponent<Image>();
        img.sprite = sprite;
        layers.Add(obj);
    }

    public void RemoveUnderlays() {
        foreach (var g in layers) Destroy(g);
        layers.Clear();
    }

    public void Revert() {
        if (prevActions.Count == 0) return;
        prevActions[prevActions.Count-1].prevState.Apply(this);
        prevActions.RemoveAt(prevActions.Count-1);
    }

}
