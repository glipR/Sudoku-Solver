using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHandler : MonoBehaviour {

    public GameObject linePrefab;

    public GameObject canvas;
    public Vector2 canvasDims;
    LineController curLine;
    public bool drawing = false;

    private float lastUpdate = 0;
    public float pointInterval = 0.2f;

    private List<GameObject> lineList = new List<GameObject>();

    private void Start() {
        canvas = transform.parent.gameObject;
        canvasDims = canvas.GetComponent<RectTransform>().sizeDelta;
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
            }
            return;
        }
        drawing = true;
        var line = Instantiate(linePrefab, canvas.transform.parent);
        lineList.Add(line);
        curLine = line.GetComponent<LineController>();
        curLine.AddPoint(transform.position);
    }

    public void EnsureNotTrail() {
        if (!drawing) return;
        drawing = false;
        curLine = null;
    }

    public void Clear() {
        foreach (var g in lineList) Destroy(g);
        lineList.Clear();
    }

}
