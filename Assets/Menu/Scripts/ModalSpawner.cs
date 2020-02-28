using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class ModalSpawner : MonoBehaviour {

    public static ModalSpawner instance;

    public enum ModalType {
        CREATE
    }

    public GameObject createObject;

    public void SpawnModal(ModalType t) {
        if (t == ModalType.CREATE) {
            var g = Instantiate(createObject, GameObject.Find("Canvas").transform);
            FlashUp(g);
        }
    }

    private Vector3Tween FlashUp(GameObject g) {
        Vector3 change = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta;
        Vector3 startPos = new Vector3(0, -1f, 0);
        Vector3 endPos = Vector3.zero;
        Vector3 diff = endPos - startPos;
        var rt = g.GetComponent<RectTransform>();
        return g.gameObject.Tween("FlashModal", startPos, endPos, 0.5f, TweenScaleFunctions.CubicEaseOut, (t) => {
            Vector3 project = startPos + diff * t.CurrentProgress;
            rt.localPosition = new Vector3(change.x * project.x, change.y * project.y, change.z * project.z);
        });
    }

    private void Awake() {
        instance = this;
    }

}
