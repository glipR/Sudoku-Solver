using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class LoadingImage : MonoBehaviour {

    public float spinWait;
    public float spinDuration;

    private float time;

    private void Start() {
        time = 0;
    }

    private void Update() {
        float newTime = time + Time.deltaTime;
        if (Mathf.Floor(time / spinWait) < Mathf.Floor(newTime / spinWait)) {
            gameObject.Tween("RotateLoad", 0f, 360f, spinDuration, TweenScaleFunctions.QuadraticEaseInOut, (t)=> {
                transform.localRotation = Quaternion.Euler(0f, 0f, t.CurrentValue);
            });
        }
        time = newTime;
    }

}
