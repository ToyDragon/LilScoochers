using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextColorLooper : MonoBehaviour
{
    private Color originalColor;
    public Color colorA = Color.white;
    public Color colorB = Color.cyan;
    public float period = .57f;
    private TMPro.TMP_Text textElement;
    void OnEnable() {
        textElement = GetComponent<TMPro.TMP_Text>();
        originalColor = textElement.color;
    }
    void OnDisable() {
        textElement.color = originalColor;
    }
    void Update()
    {
        textElement.color = Color.Lerp(colorA, colorB, Mathf.Sin(Time.time * Mathf.PI * 2f / period)*.5f + .5f);
    }
}
