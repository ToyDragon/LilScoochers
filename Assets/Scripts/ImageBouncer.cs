using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBouncer : MonoBehaviour
{
    public Vector3 initialSize;
    public float springTime = 6;
    public float springMax = 1.4f;
    public bool shown = false;
    public float stateChangeTime = 0;
    public float shrinkTime = .3f;
    private Image image;
    public bool instantHide = false;
    void OnEnable() {
        image = GetComponent<Image>();
        shown = false;
        initialSize = transform.localScale;
        transform.localScale = Vector3.zero;
    }
    void OnDisable() {
        transform.localScale = initialSize;
    }
    void Update() {
        if (shown) {
            float t = (Time.time - stateChangeTime) * 12f / springTime;
            if (t >= springTime) {
                transform.localScale = initialSize;
                if (instantHide) {
                    shown = false;
                    stateChangeTime = Time.time;
                }
            } else {
                float strength = Mathf.Pow(.3f, t);
                float inverseStrength = 1 - strength;
                float springy = Mathf.Sin(t * Mathf.PI * 1.8f - Mathf.PI*.5f)*springMax*.5f + springMax*.5f;
                transform.localScale = initialSize * (springy*strength + 1*inverseStrength);
            }
        } else {
            float t = Mathf.Clamp01((Time.time - stateChangeTime) / shrinkTime);
            transform.localScale = (1 - t) * initialSize;
        }
    }
    public void Show(Color c) {
        stateChangeTime = Time.time;
        shown = true;
        image.color = c;
        transform.localScale = Vector3.zero;
    }
    public void Hide() {
        stateChangeTime = Time.time;
        shown = false;
    }
    public void HideIfShowing() {
        if (!shown) {
            return;
        }
        stateChangeTime = Time.time;
        shown = false;
    }
}
