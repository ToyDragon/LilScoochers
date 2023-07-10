using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smack : MonoBehaviour
{
    public static Smack instance;
    private Rigidbody rb;
    public float strength = 4f;
    public float curveStrength = 2f;
    private float smackTime = -100;
    public AnimationCurve curveCurve;
    public float curveTime;
    public AudioClip hitClip;
    private AudioSource audioSource;
    public Transform ballStart;
    public int hitState;
    public float timeSlownessStarted;
    public float boostableTime = 5f;
    public float boostStrengthModifier = 3f;
    public GameObject velocityIndicator;
    void OnEnable()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;

        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        instance = this;
    }

    public Vector3 boostDir;
    public void Boost(Vector3 direction) {
        if (hitState == 1) {
            boostDir = direction;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!enabled) { return; }
        var reflectedVelocity = rb.velocity;
        foreach (ContactPoint contact in collision.contacts)
        {
            var dot = Vector3.Dot(rb.velocity, contact.normal);
            if (dot < 0) {
                // Sometimes the physics engine doesnt bounce right >:(
                reflectedVelocity = rb.velocity - 2*dot*contact.normal;
            }
            // else {
            //     Debug.Log("Skipping bound with dot " + dot);
            //     Debug.DrawRay(contact.point, contact.normal, Color.white, 15);
            //     Debug.DrawRay(contact.point, rb.velocity - 2*dot*contact.normal, Color.red, 15);
            //     Debug.DrawRay(contact.point, rb.velocity, Color.blue, 15);
            // }
        }
        rb.AddForce(reflectedVelocity - rb.velocity, ForceMode.VelocityChange);
    }

    public bool pressedSpace = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            pressedSpace = true;
        }
    }
    void FixedUpdate() {
        float velValue = rb.velocity.magnitude;
        rb.drag = Mathf.Clamp01(1 - velValue)*.9f + .1f;
        if (hitState == 0) {
            // Waiting for hit
            if (pressedSpace) {
                // var hitForce = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward * strength;
                var hitForce = Vector3.left * strength;
                rb.AddForceAtPosition(hitForce, transform.position + Vector3.up*.1f, ForceMode.Impulse);
                smackTime = Time.time;
                audioSource.PlayOneShot(hitClip);
                hitState = 1;
                boostDir = Vector3.zero;
                timeSlownessStarted = 0;
            }
        } else if (hitState == 1) {

            velocityIndicator.SetActive(true);

            velocityIndicator.transform.position = transform.position + Vector3.up * .07f;
            velocityIndicator.transform.localScale = new Vector3(.01f * (velValue*5 + 2f), .01f, .01f);
            velocityIndicator.transform.eulerAngles = new Vector3(0, 180 - Vector2.Angle(new Vector2(rb.velocity.x, rb.velocity.z), Vector2.right), 0);

            if (velValue > .02) {
                CameraFollow.instance.rotation = velocityIndicator.transform.eulerAngles.y - 90;
            }

            // Rolling
            if (velValue < .01) {
                if (timeSlownessStarted == 0) {
                    timeSlownessStarted = Time.time;
                }
                
                if (Time.time - timeSlownessStarted > 1f) {
                    float modifier = Mathf.Sqrt(1 - Time.time*2);
                    Vector3 newVel = rb.velocity * modifier;
                    if (!float.IsNaN(newVel.x)) {
                        rb.velocity = newVel;
                    }
                }
            }

            if (velValue < .005) {
                if (Time.time - timeSlownessStarted > 1f) {
                    rb.velocity = Vector3.zero;
                    Debug.Log("Moving to state 2 after " + (Time.time - timeSlownessStarted) + " seconds of slowness");
                    hitState = 2;
                }
            } else {
                timeSlownessStarted = 0;
                if (boostDir != Vector3.zero) {
                    float yrot = -Vector2.Angle(
                        new Vector2(rb.velocity.x, rb.velocity.z).normalized,
                        Vector2.up
                    );
                    Quaternion rot = Quaternion.Euler(0, yrot, 0);
                    float timeModifier = Mathf.Clamp01((Time.time - smackTime) / boostableTime);
                    float speedModifier = (Mathf.Clamp01(velValue)*.95f + .05f);
                    Vector3 smoothMaxClampedBoost = Mathf.Clamp01(boostDir.magnitude * .5f) * boostDir.normalized * 2f;
                    rb.AddForceAtPosition(rot * smoothMaxClampedBoost * timeModifier * speedModifier * Time.deltaTime * boostStrengthModifier, transform.position + Vector3.up*.1f, ForceMode.Impulse);
                    boostDir = Mathf.Clamp01(boostDir.magnitude - .01f) * boostDir.normalized;
                }
            }
            boostDir = Vector3.zero;
        } else if (hitState == 2) {
            // Paused at the end
            rb.velocity = Vector3.zero;
            if (pressedSpace) {
                transform.position = ballStart.position;
                hitState = 0;
                CameraFollow.instance.Reset(-90f);
            }
        }

        // Angular drag to prevent the ball from just spinning forever in weird directions
        // float angularYDrag = (1 - Time.deltaTime*.5f);
        // rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y * angularYDrag, rb.angularVelocity.z);
        // if (rb.velocity.magnitude < .01f) {
        //     float angularDrag = (1 - Time.deltaTime*.65f);
        //     rb.angularVelocity *= angularDrag;
        // }

        pressedSpace = false;
    }
}
