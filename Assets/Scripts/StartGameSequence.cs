using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameSequence : MonoBehaviour
{
    public Color typedColor = Color.green;
    public TMPro.TMP_Text imgA;
    public bool typedA;
    public TMPro.TMP_Text imgS;
    public bool typedS;
    public TMPro.TMP_Text imgD;
    public bool typedD;
    public TMPro.TMP_Text imgJ;
    public bool typedJ;
    public TMPro.TMP_Text imgK;
    public bool typedK;
    public TMPro.TMP_Text imgL;
    public bool typedL;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            imgA.color = typedColor;
            typedA = true;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            imgS.color = typedColor;
            typedS = true;
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            imgD.color = typedColor;
            typedD = true;
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            imgJ.color = typedColor;
            typedJ = true;
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            imgK.color = typedColor;
            typedK = true;
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            imgL.color = typedColor;
            typedL = true;
        }

        if (typedA && typedS && typedD && typedJ && typedK && typedL) {
            // Debug.Log("Loading scene " + sceneToLaunch.name);
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
            enabled = false;
        }
    }
}
