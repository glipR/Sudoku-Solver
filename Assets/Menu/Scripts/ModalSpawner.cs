using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalSpawner : MonoBehaviour {

    public static ModalSpawner instance;

    public enum ModalType {
        CREATE
    }

    public GameObject createObject;

    public void SpawnModal(ModalType t) {
        if (t == ModalType.CREATE) {
            var g = Instantiate(createObject, GameObject.Find("Canvas").transform);
            StartCoroutine(FlashUp(g));
        }
    }

    private IEnumerator FlashUp(GameObject g) {
        var rt = g.GetComponent<RectTransform>();
        Vector3 change = new Vector3(0, -GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y, 0);
        rt.localPosition += change;
        for (int i=0; i<30; i++) {
            rt.localPosition -= change / 30f;
            yield return new WaitForSeconds(0.005f);
        }
        yield return null;
    }

    private void Awake() {
        instance = this;
    }

}
