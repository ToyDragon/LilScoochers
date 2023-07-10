using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixCamOnStart : MonoBehaviour
{
    public int frame = 0;
    private GameObject camObj;
    void Update() {
        frame++;
        if (frame == 1) {
            camObj = Camera.main.gameObject;
            camObj.SetActive(false);
        }
        if (frame == 2) {
            camObj.SetActive(true);
            gameObject.SetActive(false);
            frame = 0;
        }
    }
}
