using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupEffectController : MonoBehaviour
{
    public List<ParticleSystem> childParticleSystems;
    public float effectStartTime;
    public List<AudioClip> fireworkClips;
    public float nextFireworkEffect;
    private AudioSource audioSource;
    void OnEnable() {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        StopEffects();
    }
    public void StopEffects() {
        effectStartTime = 0;
        foreach (var system in childParticleSystems) {
            system.Stop();
        }
    }
    public void PlayEffects() {
        effectStartTime = Time.time;
        nextFireworkEffect = Time.time;

        foreach (var system in childParticleSystems) {
            system.Play();
        }
    }
    void Update()
    {
        if (effectStartTime > 0) {
            if (Time.time > nextFireworkEffect) {
                audioSource.PlayOneShot(fireworkClips[Random.Range(0, fireworkClips.Count)]);
                nextFireworkEffect = Time.time + Random.Range(.2f, .8f);
            }
        }
    }
}
