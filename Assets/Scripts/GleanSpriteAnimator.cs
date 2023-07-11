using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GleanSpriteAnimator : MonoBehaviour
{
    private Vector3 originalScale;
    private Quaternion originalRotation;
    public float timeOffset;
    void OnEnable() {
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }
    void OnDisable() {
        transform.localScale = originalScale;
        transform.rotation = originalRotation;
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, Time.time * 90f);
        float newScale = Mathf.Clamp01(Mathf.Sin((Time.time + timeOffset) * Mathf.PI * 2f / 4f) * 3 - 1.5f);
        transform.localScale = newScale * Vector3.one;
        
    }
}
