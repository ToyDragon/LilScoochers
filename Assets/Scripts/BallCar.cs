using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCar : MonoBehaviour
{
    public static BallCar instance;
    private Rigidbody rb;
    public float maxMomentum = 10f;
    public float momentum = 10;
    public float recentMaxMomentum;
    public float momentLossPerS = 1;
    public float maxMomentumPotential = 2;
    public float targetSpeed;
    public float curveStrength;
    public float dragMomentum = 5;
    public float maxDrag = 3;
    public float targetSpeedStrength = .5f;
    public float overspeedDragLimit = 4f;
    public float maxSpeedDrag = 8f;
    public float speedModifier = 1;
    public float lastDirection;
    public int state = 0;
    public float hitVolume = .5f;
    public bool spacePressed = false;
    public AudioClip ballHitClip;
    public AudioClip enterCupClip;
    private AudioSource audioSource;
    public float recentFallSpeed;
    private Vector3 velLastUpdate;
    private Vector3? bounceForce;
    public Renderer mainRenderer;
    public List<AudioClip> bounceClips;
    public float lastBounceClipTime;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TheCUP>()) {
            audioSource.PlayOneShot(enterCupClip, hitVolume);
            LevelManager.instance.LevelFinished();
        }
        if (other.GetComponent<ThePIT>()) {
            audioSource.PlayOneShot(enterCupClip, hitVolume);
            LevelManager.instance.ResetLevel();
        }
    }
    public void PlayHitClip() {
        audioSource.PlayOneShot(ballHitClip, hitVolume);
    }
    void OnEnable()
    {
        recentMaxMomentum = momentum;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        instance = this;
    }
    public void Hide() {
        mainRenderer.enabled = false;
        state = 2;
    }
    public void Show() {
        mainRenderer.enabled = true;
        state = 0;
    }
    void Update()
    {
        if (state == 2) {
            return;
        }
        if (transform.position.y < -20) {
            LevelManager.instance.ResetLevel();
        }
        if (state == 0) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                spacePressed = true;
                momentum = recentMaxMomentum = maxMomentum;
            }
        } else if (state == 1) {
            // recentMaxMomentum = Mathf.Min(recentMaxMomentum, momentum + maxMomentumPotential);
            // if (momentum == 0) {
            //     recentMaxMomentum = Mathf.Max(recentMaxMomentum - momentLossPerS * Time.deltaTime, 0);
            // }
            momentum = Mathf.Max(momentum - momentLossPerS * Time.deltaTime, 0);
            recentMaxMomentum = momentum;
            recentFallSpeed = Mathf.Max(Mathf.Min(recentFallSpeed, 4) - Time.deltaTime, 0);
        }
        velLastUpdate = rb.velocity;
    }
    public void Reset() {
        momentum = recentMaxMomentum = 0;
        state = 0;
        curveStrength = 0;
    }
    public void Boost(Vector3 direction) {
        if (state == 2) {
            return;
        }
        if (direction == Vector3.forward) {
            MenuController.instance.TryClose();
        }
        if (state == 1) {
            if (direction == Vector3.forward) {
                // float gainedMomentum = recentMaxMomentum - momentum;
                // if (gainedMomentum > 0) {
                //     momentum = recentMaxMomentum;
                //     recentMaxMomentum -= gainedMomentum * .5f;
                // }
                if (momentum == 0 && rb.velocity.magnitude < .1f) {
                    rb.AddForce(Quaternion.Euler(0, lastDirection, 0) * Vector3.forward * targetSpeed * 1, ForceMode.Force);
                    momentum = recentMaxMomentum = maxMomentum;
                    speedModifier = Mathf.Max(1f, speedModifier + 1f);
                } else {
                    momentum = recentMaxMomentum = maxMomentum;
                    speedModifier = Mathf.Max(1f, speedModifier + 1f);
                }
                audioSource.PlayOneShot(ballHitClip, hitVolume);
                LevelManager.instance.puttCount++;
            }
            if (direction == Vector3.back) {
                speedModifier = Mathf.Max(speedModifier - .4f, 0f);
                momentum = recentMaxMomentum = Mathf.Max(momentum - 1, 0);
            }
            if (direction == Vector3.left) {
                curveStrength -= .2f;
            }
            if (direction == Vector3.right) {
                curveStrength += .2f;
            }
        }
        if (state == 0) {
            if (direction == Vector3.forward) {
                rb.AddForce(Quaternion.Euler(0, lastDirection, 0) * Vector3.forward * targetSpeed * 1, ForceMode.Force);
                rb.drag = 0;
                state = 1;
                momentum = recentMaxMomentum = maxMomentum;
                audioSource.PlayOneShot(ballHitClip, hitVolume);
                LevelManager.instance.puttCount++;
            }
            if (direction == Vector3.back) {
                // Nothing
            }
            if (direction == Vector3.left) {
                lastDirection -= 30;
            }
            if (direction == Vector3.right) {
                lastDirection += 30;
            }
        }
    }
    void FixedUpdate() {
        if (state == 2) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        if (bounceForce.HasValue) {
            rb.AddForce(bounceForce.Value, ForceMode.Force);
            rb.angularVelocity = Vector3.zero;
            bounceForce = null;
        }

        if (state == 0) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        } else if (state == 1) {
            Vector3 planarVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            float vel = rb.velocity.magnitude;
            recentFallSpeed = Mathf.Max(recentFallSpeed, Mathf.Abs(rb.velocity.y));
            float targetSpeedWithModifier = targetSpeed * speedModifier;
            float velRatioToTarget = targetSpeedWithModifier / Mathf.Max(vel, 0.01f);
            speedModifier += (1 - speedModifier) * Time.deltaTime;

            // Speed the ball up if it's below the target speed
            if (velRatioToTarget > 1 && momentum > dragMomentum * .25f) {
                float upHillModifier = Mathf.Clamp01(1 - rb.velocity.y*2)*.5f + .5f;
                rb.AddForce(planarVel * velRatioToTarget * targetSpeedStrength * upHillModifier, ForceMode.Force);
            }

            // Slow the ball down if out of momentum, or if going too fast
            float dragFromMomentum = (1 - Mathf.Clamp01(momentum / dragMomentum)) * maxDrag;
            float dragFromSpeed = Mathf.Clamp01((vel - targetSpeed) / overspeedDragLimit) * maxSpeedDrag;
            // Reduce drag when falling
            rb.drag = Mathf.Max(dragFromSpeed, dragFromMomentum) * Mathf.Clamp01(1 - recentFallSpeed*5f);

            // Apply horizontal curve
            rb.AddForce(Quaternion.Euler(0, 90, 0) * rb.velocity * curveStrength, ForceMode.Force);
            curveStrength *= 1 - Time.deltaTime;

            if (vel < .01f && recentMaxMomentum == 0f) {
                state = 0;
                rb.velocity = Vector3.zero;
                curveStrength = 0;
            }
            if (vel > .02) {
                lastDirection = -Vector2.SignedAngle(Vector2.up, new Vector2(rb.velocity.x, rb.velocity.z));
            }
        }
        CameraFollow.instance.rotation = lastDirection;
        spacePressed = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!enabled) {
            Debug.Log("Collision entered but not enabled?");
            return;
        }
        var reflectedVelocity = rb.velocity;
        if (collision.contacts.Length != 1) {
            Debug.Log("Collision with weird length? " + collision.contacts.Length);
        }
        bool bounced = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            var dot = Vector3.Dot(velLastUpdate, contact.normal);
            Debug.DrawRay(contact.point, contact.normal, Color.red, 15);
            Debug.Log($"Collision at {contact.point} normal {contact.normal} with dot {dot}");
            if (dot < 0) {
                // Sometimes the physics engine doesnt bounce right >:(
                reflectedVelocity = velLastUpdate - 2*dot*contact.normal;
                bounceForce = reflectedVelocity - velLastUpdate;

                // Only play the bounce sound on wall collisions
                if (Mathf.Abs(Vector3.Dot(contact.normal, Vector3.up)) < .3f) {
                    bounced = true;
                }
            }
        }

        if (bounced && Time.time - lastBounceClipTime > .25f) {
            audioSource.PlayOneShot(bounceClips[Random.Range(0, bounceClips.Count)], hitVolume);
            lastBounceClipTime = Time.time;
        }
    }
}
