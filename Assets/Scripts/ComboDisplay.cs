using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboDisplay : MonoBehaviour
{
    public List<Transform> targets = new List<Transform>();
    public Sprite spriteArrowUp;
    public Sprite spriteArrowDown;
    public float? targetSize;
    public Image image;
    public Color color;
    public bool dieAtTarget;
    public Color upColor = Color.green;
    public Color downColor = Color.red;

    public bool isComboCombo = false;
    public float comboComboExpiration = -100;
    public GameObject comboComboDependencyA;
    public GameObject comboComboDependencyB;
    public Transform comboComboTarget;
    void OnEnable()
    {
        image = GetComponent<Image>();
    }
    public void InitAsComboCombo(Transform spot, Transform target, GameObject a, GameObject b, Sprite s, Color c) {
        isComboCombo = true;
        transform.position = spot.position;
        comboComboDependencyA = a;
        comboComboDependencyB = b;
        image.sprite = s;
        image.color = Color.clear;
        color = c;
        comboComboTarget = target;
        ((RectTransform)transform).sizeDelta = Vector2.one * 100;
    }
    public void SetArrowType(bool increasing) {
        color = increasing ? upColor : downColor;
        image.color = color;
        image.sprite = increasing ? spriteArrowUp : spriteArrowDown;
    }
    void Update()
    {
        if (isComboCombo) {
            if (comboComboTarget != null) {
                if (comboComboDependencyA == null && comboComboDependencyB == null) {
                    image.color = color;
                    var deltaToTarget = comboComboTarget.position - transform.position;
                    if (deltaToTarget.magnitude < 10) {
                        Destroy(gameObject);
                    } else {
                        transform.position += deltaToTarget * Mathf.Clamp01(10 * Time.deltaTime);
                    }
                }
            } else {
                if (comboComboExpiration > 0) {
                    transform.position += Vector3.up * 100 * Time.deltaTime;
                    if (Time.time > comboComboExpiration) {
                        Destroy(gameObject);
                    }
                } else {
                    if (comboComboDependencyA == null && comboComboDependencyB == null) {
                        comboComboExpiration = Time.time + .75f;
                        image.color = color;
                    }
                }
            }
            return;
        }
        

        // float ageModifier = Mathf.Clamp01(2f*(Time.time - startTime)/expireTime - 1.4f);
        // ageModifier = Mathf.Min(ageModifier, (Time.time - startTime) * 4f);
        // image.color = Color.Lerp(color, new Color(1, 1, 1, 0), ageModifier);
        image.color = color;

        if (targetSize.HasValue) {
            var sizeDelta = new Vector2(targetSize.Value, targetSize.Value) - ((RectTransform)transform).sizeDelta;
            ((RectTransform)transform).sizeDelta += sizeDelta * Mathf.Clamp01(Time.deltaTime * 3);
            if (sizeDelta.magnitude < 1) {
                targetSize = null;
            }
        }

        if (targets.Count == 0) {
            if (dieAtTarget) {
                Destroy(gameObject);
            }
            return;
        }

        var target = targets[0];
        var delta = target.position - transform.position;
        transform.position += delta * Mathf.Clamp01(Time.deltaTime * 10);
        if (delta.magnitude < 10f) {
            targets.RemoveAt(0);
        }
    }
    public void AddTarget(Transform t) {
        targets.Add(t);
    }
}
