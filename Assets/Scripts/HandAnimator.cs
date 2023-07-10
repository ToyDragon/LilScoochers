using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandAnimator : MonoBehaviour
{
    public Transform AJoint;
    public Vector3 AJointInitialRot;
    public Transform BJoint;
    public Vector3 BJointInitialRot;
    public Transform CJoint;
    public Vector3 CJointInitialRot;
    public Image AKey;
    public Image BKey;
    public Image CKey;
    public float actuationRange = 30f;
    public float timePerFinger;
    public bool reversed = false;
    public Color pressedColor = Color.red;
    public Color originalColor;
    public float t;
    public int bstate;
    public float animStartTime = 0;
    void OnEnable() {
        AJointInitialRot = AJoint.transform.eulerAngles;
        BJointInitialRot = BJoint.transform.eulerAngles;
        CJointInitialRot = CJoint.transform.eulerAngles;
        originalColor = AKey.color;
    }
    Vector3 OffsetAngle(Vector3 original, float xOffset) {
        return new Vector3(
            original.x + xOffset,
            original.y,
            original.z
        );
    }
    void Update()
    {
        float animTime = Time.time - animStartTime;
        int state = Mathf.FloorToInt(animTime / timePerFinger);
        float t = Mathf.Clamp01((animTime - state*timePerFinger) / timePerFinger);
        state = (state + 18) % 20;
        bstate = state;
        float range = actuationRange;
        this.t = t;
        if (reversed) {
            if (state < 6) {
                state = 5 - state;
                t = 1 - t;
            }
        }
        if(state == 0) {
            AJoint.transform.eulerAngles = OffsetAngle(AJointInitialRot, t * actuationRange);
            AKey.color = pressedColor;
        } else if (state == 1) {
            AJoint.transform.eulerAngles = OffsetAngle(AJointInitialRot, (1 - t) * actuationRange);
            AKey.color = pressedColor;
        } else {
            AJoint.transform.eulerAngles = AJointInitialRot;
            AKey.color = originalColor;

        }

        if(state == 2) {
            BJoint.transform.eulerAngles = OffsetAngle(BJointInitialRot, t * actuationRange);
            BKey.color = pressedColor;
        } else if (state == 3) {
            BJoint.transform.eulerAngles = OffsetAngle(BJointInitialRot, (1 - t) * actuationRange);
            BKey.color = pressedColor;
        } else {
            BJoint.transform.eulerAngles = BJointInitialRot;
            BKey.color = originalColor;
        }

        if(state == 4) {
            CJoint.transform.eulerAngles = OffsetAngle(CJointInitialRot, t * actuationRange);
            CKey.color = pressedColor;
        } else if (state == 5) {
            CJoint.transform.eulerAngles = OffsetAngle(CJointInitialRot, (1 - t) * actuationRange);
            CKey.color = pressedColor;
        } else {
            CJoint.transform.eulerAngles = CJointInitialRot;
            CKey.color = originalColor;
        }
    }
}
