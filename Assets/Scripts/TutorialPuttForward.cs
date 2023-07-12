using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPuttForward : MonoBehaviour
{
    public GameObject checkA;
    public GameObject checkB;
    public GameObject checkC;
    public Transform arrowSpotA;
    public Transform arrowSpotS;
    public Transform arrowSpotD;
    public Transform arrowSpotJ;
    public Transform arrowSpotK;
    public Transform arrowSpotL;
    public TMPro.TMP_Text textA;
    public TMPro.TMP_Text textS;
    public TMPro.TMP_Text textD;
    public TMPro.TMP_Text textJ;
    public TMPro.TMP_Text textK;
    public TMPro.TMP_Text textL;
    public TutorialMovingArrow leftArrow;
    public TutorialMovingArrow rightArrow;
    public int leftProgress = 0;
    public int rightProgress = 0;
    public int repetitions = 0;
    public Color pressedColor = Color.green;
    public float closeStart = 0;
    private AudioSource audioSource;
    void OnEnable() {
        audioSource = GetComponent<AudioSource>();
        leftArrow.gameObject.SetActive(true);
        rightArrow.gameObject.SetActive(true);
        checkA.SetActive(false);
        checkB.SetActive(false);
        checkC.SetActive(false);
        textA.color = Color.white;
        textS.color = Color.white;
        textD.color = Color.white;
        textJ.color = Color.white;
        textK.color = Color.white;
        textL.color = Color.white;
        leftArrow.Reset(arrowSpotA);
        rightArrow.Reset(arrowSpotL);
        leftProgress = 0;
        rightProgress = 0;
        repetitions = 0;
    }
    void Update()
    {
        if (closeStart > 0) {
            if (Time.time > closeStart + 1.3f) {
                closeStart = 0;
                gameObject.SetActive(false);
            }
            return;
        }
        if (leftProgress == 0 && Input.GetKeyDown(KeyCode.A)) {
            leftProgress++;
            leftArrow.target = arrowSpotS;
            textA.color = pressedColor;
        }
        if (leftProgress == 1 && Input.GetKeyDown(KeyCode.S)) {
            leftProgress++;
            leftArrow.target = arrowSpotD;
            textS.color = pressedColor;
        }
        if (leftProgress == 2 && Input.GetKeyDown(KeyCode.D)) {
            leftProgress++;
            textD.color = pressedColor;
        }

        if (rightProgress == 0 && Input.GetKeyDown(KeyCode.L)) {
            rightProgress++;
            rightArrow.target = arrowSpotK;
            textL.color = pressedColor;
        }
        if (rightProgress == 1 && Input.GetKeyDown(KeyCode.K)) {
            rightProgress++;
            rightArrow.target = arrowSpotJ;
            textK.color = pressedColor;
        }
        if (rightProgress == 2 && Input.GetKeyDown(KeyCode.J)) {
            rightProgress++;
            textJ.color = pressedColor;
        }

        if (leftProgress >= 3 && rightProgress >= 3) {
            leftProgress = 0;
            rightProgress = 0;
            repetitions++;
            if (repetitions == 1) { checkA.SetActive(true); }
            if (repetitions == 2) { checkB.SetActive(true); }
            if (repetitions == 3) { checkC.SetActive(true); }
            leftArrow.Reset(arrowSpotA);
            rightArrow.Reset(arrowSpotL);
            textA.color = Color.white;
            textS.color = Color.white;
            textD.color = Color.white;
            textJ.color = Color.white;
            textK.color = Color.white;
            textL.color = Color.white;

            BallCar.instance.PlayHitClip();

            if (repetitions == 3) {
                audioSource.volume = MenuController.instance.sfxVolume;
                audioSource.Play();
                closeStart = Time.time;
                leftArrow.gameObject.SetActive(false);
                rightArrow.gameObject.SetActive(false);
            }
        }
    }
}
