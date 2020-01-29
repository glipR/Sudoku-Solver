using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHandler : MonoBehaviour {

    public GameObject linePrefab;

    public GameObject canvas;
    public GameObject digitCanvas;
    public Vector2 canvasDims;
    public Vector2 digitCanvasDims;
    LineController curLine;
    LineController curLine2;
    public bool drawing = false;

    private float lastUpdate = 0;
    public float pointInterval = 0.2f;

    private List<GameObject> lineList = new List<GameObject>();

    private void Start() {
        canvas = transform.parent.gameObject;
        canvasDims = canvas.GetComponent<RectTransform>().sizeDelta;
        digitCanvas = GameObject.Find("DigitCanvas");
        digitCanvasDims = digitCanvas.GetComponent<RectTransform>().sizeDelta;
    }

    private void Update() {
        if (
            (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            ||  Input.GetMouseButton(0)
        ) {
            Plane objPlane = new Plane(Camera.main.transform.forward*-1, this.transform.position);

            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (objPlane.Raycast(mRay, out rayDistance)) {
                Vector3 newPos = canvas.transform.InverseTransformPoint(mRay.GetPoint(rayDistance));
                if (
                    Mathf.Abs(newPos.x) < .5f*canvasDims.x
                    && Mathf.Abs(newPos.y) < .5f*canvasDims.y
                ) {
                    this.transform.localPosition = newPos;
                    EnsureTrail();
                }
                else {
                    EnsureNotTrail();
                }
            }
        } else EnsureNotTrail();
    }

    public void EnsureTrail() {
        if (drawing) {
            lastUpdate += Time.deltaTime;
            if (lastUpdate > pointInterval) {
                lastUpdate -= pointInterval;
                curLine.AddPoint(transform.position);
                Vector3 newPos = transform.localPosition;
                newPos = new Vector3(newPos.x/canvasDims.x, newPos.y/canvasDims.y + 20, transform.position.z);
                curLine2.AddPoint(newPos);
            }
            return;
        }
        drawing = true;
        var line = Instantiate(linePrefab, canvas.transform.parent);
        var line2 = Instantiate(linePrefab, digitCanvas.transform);
        lineList.Add(line);
        lineList.Add(line2);
        curLine = line.GetComponent<LineController>();
        curLine.SetReal();
        curLine2 = line2.GetComponent<LineController>();
        curLine2.SetFake();
        curLine.AddPoint(transform.position);
        Vector3 p2 = transform.localPosition;
        p2 = new Vector3(p2.x/canvasDims.x, p2.y/canvasDims.y + 20, transform.position.z);
        curLine2.AddPoint(p2);
    }

    public void EnsureNotTrail() {
        if (!drawing) return;
        drawing = false;
        curLine = null;
        curLine2 = null;
    }

    public void Clear() {
        foreach (var g in lineList) Destroy(g);
        lineList.Clear();
    }

}
