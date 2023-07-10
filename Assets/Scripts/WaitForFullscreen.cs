using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForFullscreen : MonoBehaviour
{
    public GameObject toShowOnFullScreen;
    void Update()
    {
        if (Screen.width > 1000 && Screen.height > 1000) {
            toShowOnFullScreen.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
