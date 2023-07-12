using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMovingArrow : MonoBehaviour
{
    public Transform target;
    public Vector3 rootPos;
    public float bounceHeight = 12f;
    public float bouncePeriod = 1f;
    public float speed = 10;
    public float rotationSpeed = 5;
    void OnEnable() {
        rootPos = transform.position;
    }
    public void Reset(Transform newTarget) {
        target = newTarget;
        transform.position = rootPos = target.position;
        transform.rotation = newTarget.rotation;
    }
    void Update()
    {
        var delta = target.position - rootPos;
        rootPos += Mathf.Min(Time.deltaTime * speed, delta.magnitude) * delta.normalized;
        transform.position = rootPos + Mathf.Abs(Mathf.Sin(Time.time * Mathf.PI / bouncePeriod)) * bounceHeight * Vector3.up;
        float zDelta = target.rotation.eulerAngles.z - transform.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z + Mathf.Min(Mathf.Abs(zDelta), rotationSpeed) * Mathf.Sign(zDelta)
        );
    }
}
