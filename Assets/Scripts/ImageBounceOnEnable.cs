using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageBounceOnEnable : MonoBehaviour
{
    public Vector3 initialSize;
    public float activateTime;
    public float springTime = 6;
    public float springMax = 1.4f;
    void OnEnable() {
        initialSize = transform.localScale;
        activateTime = Time.time;
        transform.localScale = Vector3.zero;
    }
    void OnDisable() {
        transform.localScale = initialSize;
    }
    void Update() {
        float t = (Time.time - activateTime) * 12f / springTime;
        if (t >= springTime) {
            transform.localScale = initialSize;
        } else {
            float strength = Mathf.Pow(.3f, t);
            float inverseStrength = 1 - strength;
            float springy = Mathf.Sin(t * Mathf.PI * 1.8f - Mathf.PI*.5f)*springMax*.5f + springMax*.5f;
            transform.localScale = initialSize * (springy*strength + 1*inverseStrength);
        }
    }
}
