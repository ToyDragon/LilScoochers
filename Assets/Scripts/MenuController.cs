using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;
    public bool open;
    public Rect windowRect;
    public bool isAPussy = false;
    public GameObject controlsDisplay;
    public int directionShowing = 0;
    public HandAnimator leftHand;
    public HandAnimator rightHand;
    public GameObject leftGreen;
    public GameObject leftRed;
    public GameObject rightGreen;
    public GameObject rightRed;

    public Transform leftComboStart;
    public Transform rightComboStart;
    public Transform comboCombinePos;

    public GameObject sampleFowardSprite;
    public GameObject sampleLeftSprite;
    public GameObject sampleRightSprite;
    public GameObject sampleStopSprite;
    public AudioSource musicSource;
    public bool showControlsWhenClosed = true;
    private List<AudioSource> sfxAudioSources;
    void OnEnable()
    {
        instance = this;
        windowRect = new Rect(Screen.width/2f - 300, Screen.height/2f - 200, 600, 400);
        leftGreen.SetActive(false);
        leftRed.SetActive(false);
        rightGreen.SetActive(false);
        rightRed.SetActive(false);
    }
    void Start() {
        sfxAudioSources = new List<AudioSource>();
        foreach (var sfx in FindObjectsOfType<SFXAudioSource>()) {
            sfxAudioSources.Add(sfx.GetComponent<AudioSource>());
        }
        foreach (var sfx in sfxAudioSources) {
            sfx.volume = musicSource.volume;
        }
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) {
            open = !open;
            if (open) {
                windowRect = new Rect(Screen.width/2f - 300, Screen.height/2f - 200, 600, 400);
                isAPussy = false;
            }
        }
        if ((!showControlsWhenClosed && !open) || LevelManager.instance.waitingForNext) {
            controlsDisplay.SetActive(false);
        } else {
            controlsDisplay.SetActive(true);
            UpdateControlDisplay();
            UpdateComboArrows();
        }
    }
    private Vector3 Lerp(Transform a, Transform b, float t) {
        return a.position + (b.position - a.position) * t;
    }
    private void UpdateComboArrows() {
        bool leftUp = directionShowing == 0 || directionShowing == 2;
        bool rightUp = directionShowing == 0 || directionShowing == 1;
        int state = leftHand.bstate;
        float t = leftHand.t;

        if (state < 5) {
            leftGreen.transform.position = leftComboStart.position;
            leftRed.transform.position = leftComboStart.position;
            rightGreen.transform.position = rightComboStart.position;
            rightRed.transform.position = rightComboStart.position;
            leftGreen.SetActive(false);
            leftRed.SetActive(false);
            rightGreen.SetActive(false);
            rightRed.SetActive(false);

            sampleFowardSprite.SetActive(false);
            sampleLeftSprite.SetActive(false);
            sampleRightSprite.SetActive(false);
            sampleStopSprite.SetActive(false);
        }

        if (state == 5 || state == 6 || state == 7) {
            leftGreen.SetActive(leftUp);
            leftRed.SetActive(!leftUp);
            rightGreen.SetActive(rightUp);
            rightRed.SetActive(!rightUp);

            sampleFowardSprite.SetActive(false);
            sampleLeftSprite.SetActive(false);
            sampleRightSprite.SetActive(false);
            sampleStopSprite.SetActive(false);

            leftGreen.transform.position = Lerp(leftComboStart, comboCombinePos, 0);
            leftRed.transform.position = Lerp(leftComboStart, comboCombinePos, 0);
            rightGreen.transform.position = Lerp(rightComboStart, comboCombinePos, 0);
            rightRed.transform.position = Lerp(rightComboStart, comboCombinePos, 0);
        }
        if (state == 8) {
            leftGreen.SetActive(leftUp);
            leftRed.SetActive(!leftUp);
            rightGreen.SetActive(rightUp);
            rightRed.SetActive(!rightUp);

            sampleFowardSprite.SetActive(false);
            sampleLeftSprite.SetActive(false);
            sampleRightSprite.SetActive(false);
            sampleStopSprite.SetActive(false);

            leftGreen.transform.position = Lerp(leftComboStart, comboCombinePos, t);
            leftRed.transform.position = Lerp(leftComboStart, comboCombinePos, t);
            rightGreen.transform.position = Lerp(rightComboStart, comboCombinePos, t);
            rightRed.transform.position = Lerp(rightComboStart, comboCombinePos, t);
        }
        if (state == 9 || state == 10 || state == 11 || state == 12 || state == 13) {
            leftGreen.SetActive(false);
            leftRed.SetActive(false);
            rightGreen.SetActive(false);
            rightRed.SetActive(false);

            sampleFowardSprite.SetActive(leftUp && rightUp);
            sampleLeftSprite.SetActive(!leftUp && rightUp);
            sampleRightSprite.SetActive(leftUp && !rightUp);
            sampleStopSprite.SetActive(!leftUp && !rightUp);
        }
        if (state >= 14) {
            leftGreen.SetActive(false);
            leftRed.SetActive(false);
            rightGreen.SetActive(false);
            rightRed.SetActive(false);

            sampleFowardSprite.SetActive(false);
            sampleLeftSprite.SetActive(false);
            sampleRightSprite.SetActive(false);
            sampleStopSprite.SetActive(false);
        }
    }
    void OnGUI() {
        if (!open) {
            return;
        }
        windowRect = GUILayout.Window(0, windowRect, MainWindow, "Lil' Scoochers");
    }
    private void UpdateControlDisplay() {
        bool leftUp = directionShowing == 0 || directionShowing == 2;
        leftHand.reversed = leftUp;
        // leftGreen.SetActive(leftUp);
        // leftRed.SetActive(!leftUp);
        bool rightUp = directionShowing == 0 || directionShowing == 1;
        rightHand.reversed = rightUp;
        // rightGreen.SetActive(rightUp);
        // rightRed.SetActive(!rightUp);
    }
    public void TryClose() {
        if (open) {
            open = false;
        }
    }
    private void MainWindow(int windowId) {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(300));

        GUILayout.Label("Controls:");
        GUILayout.BeginScrollView(Vector2.zero);
        GUILayout.Label("This game is about exploring this weird control scheme. Sorry if it takes a moment to understand :)");
        GUILayout.Label("Your left hand should be on the ASD keys, and your right hand on JKL.");
        GUILayout.Label("You'll roll your fingers either towards the center to add momentum to that side of the ball, or away from the center to slow that side of the ball.");
        GUILayout.Label("Rolling both hands towards the center will speed the ball up, and a differential will turn the ball.");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Show me how to scooch:");
        GUILayout.BeginVertical();
        if (GUILayout.Button((directionShowing == 0 ? "*" : "") + "forward")) {
            directionShowing = 0;
            leftHand.animStartTime = Time.time;
            rightHand.animStartTime = Time.time;
            UpdateControlDisplay();
        }
        if (GUILayout.Button((directionShowing == 1 ? "*" : "") + "left")) {
            directionShowing = 1;
            leftHand.animStartTime = Time.time;
            rightHand.animStartTime = Time.time;
            UpdateControlDisplay();
        }
        if (GUILayout.Button((directionShowing == 2 ? "*" : "") + "right")) {
            directionShowing = 2;
            leftHand.animStartTime = Time.time;
            rightHand.animStartTime = Time.time;
            UpdateControlDisplay();
        }
        if (GUILayout.Button((directionShowing == 3 ? "*" : "") + "stop")) {
            directionShowing = 3;
            leftHand.animStartTime = Time.time;
            rightHand.animStartTime = Time.time;
            UpdateControlDisplay();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUILayout.BeginVertical();

        GUILayout.Label("Settings:");
        GUILayout.BeginScrollView(Vector2.zero);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Show control tutorial:");
        if (GUILayout.Button(showControlsWhenClosed ? "always" : "in menu")) {
            showControlsWhenClosed = !showControlsWhenClosed;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("SFX Volume:");
        BallCar.instance.hitVolume = GUILayout.HorizontalSlider(BallCar.instance.hitVolume, 0, 1, GUILayout.Width(150));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Music Volume:");
        musicSource.volume = GUILayout.HorizontalSlider(musicSource.volume, 0, .5f, GUILayout.Width(150));
        foreach (var sfx in sfxAudioSources) {
            sfx.volume = musicSource.volume;
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Stuck/Glitch?:");
        if (GUILayout.Button("Reset Level")) {
            LevelManager.instance.ResetLevel();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Easy Input Mode:");
        if (!isAPussy) {
            isAPussy = GUILayout.Button("enable");
        } else {
            GUILayout.Button("haha you wish");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.Label("Press Tab to close.");
    }
}
