using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour {

    private LineRenderer renderer;

    private void Awake() {
        renderer = GetComponent<LineRenderer>();
    }

    public void AddPoint(Vector3 point) {
        var newP = new Vector3(point.x, point.y, 0f);
        renderer.positionCount++;
        renderer.SetPosition(renderer.positionCount-1, newP);
    }
}
