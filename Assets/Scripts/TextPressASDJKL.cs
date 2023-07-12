using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPressASDJKL : MonoBehaviour
{
    private TMPro.TMP_Text textElement;
    public bool[] presses = new bool[6];
    public string[] chars = new string[] {"A", "S", "D", "J", "K", "L"};
    public KeyCode[] keyCodes = new KeyCode[] { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.J, KeyCode.K, KeyCode.L };
    public string originalText;
    public Color pressedColor = Color.green;
    void OnEnable() {
        Reset();
        textElement = GetComponent<TMPro.TMP_Text>();
        if (originalText == "") {
            originalText = textElement.text;
        }
        textElement.richText = true;
        UpdateDisplay(); 
    }
    void OnDisable() {
        textElement.SetText(originalText);
    }
    public void Reset() {
        for (int i = 0; i < 6; i++) {
            presses[i] = false;
        }
    }
    void Update() {
        bool anyPress = false;
        bool allPressed = true;
        for (int i = 0; i < 6; i++) {
            if (Input.GetKeyDown(keyCodes[i])) {
                anyPress = true;
                presses[i] = true;
            }
            if (!presses[i]) {
                allPressed = false;
            }
        }
        if (allPressed) {
            LevelManager.instance.StartNextLevel();
            Reset();
        }
        if (anyPress) {
            UpdateDisplay();
        }
    }
    private void UpdateDisplay() {
        string richReplacement = "";
        for (int i = 0; i < 6; i++) {
            if (presses[i]) {
                richReplacement += $"<color=#{ColorUtility.ToHtmlStringRGB(pressedColor)}>";
            } else {
                richReplacement += $"<color=#FFFFFF>";
            }
            richReplacement += chars[i];
        }
        richReplacement += $"<color=#FFFFFF>";

        textElement.SetText(originalText.Replace("ASDJKL", richReplacement));
    }
}
