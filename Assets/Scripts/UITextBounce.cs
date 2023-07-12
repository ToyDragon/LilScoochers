using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITextBounce : MonoBehaviour
{
    public Vector3 startPos;
    public float bounceHeight = 25f;
    public float bouncePeriod = 1f;
    void OnEnable() {
        startPos = transform.localPosition;
    }
    void OnDisable() {
        transform.localPosition = startPos;
    }
    void Update()
    {
        transform.localPosition = startPos + Mathf.Abs(Mathf.Sin(Time.time * Mathf.PI / bouncePeriod)) * bounceHeight * Vector3.up;
    }
}
