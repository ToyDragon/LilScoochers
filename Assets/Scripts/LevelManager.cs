using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public GameObject minimapCamera;
    public int currentLevel = -1;
    public List<LevelData> levelDatas = new List<LevelData>();
    public bool waitingForNext;
    public GameObject courseDoneDisplayRoot;
    public float timeLevelStarted;
    public List<float> fastestCompletions = new List<float>();
    public List<int> fewestPutts = new List<int>();
    public TMPro.TMP_Text endTimeText;
    public TMPro.TMP_Text endPuttsText;
    public TMPro.TMP_Text levelNameText;
    public TMPro.TMP_Text minimapText;
    public int puttCount;
    public int totalPuttCount;
    public float totalTime;

    public TMPro.TMP_Text gameDoneTotalTime;
    public TMPro.TMP_Text gameDoneTotatPutts;
    public GameObject gamedoneScreen;
    public TMPro.TMP_Text againPressAText;
    public bool againPressedA;
    public TMPro.TMP_Text againPressSText;
    public bool againPressedS;
    public TMPro.TMP_Text againPressDText;
    public bool againPressedD;
    public TMPro.TMP_Text againPressJText;
    public bool againPressedJ;
    public TMPro.TMP_Text againPressKText;
    public bool againPressedK;
    public TMPro.TMP_Text againPressLText;
    public bool againPressedL;
    public Color btnPressedColor = Color.green;
    void OnEnable() {
        instance = this;
        fastestCompletions.Clear();
        fewestPutts.Clear();
        againPressedA = againPressedS = againPressedD = againPressedJ = againPressedK = againPressedL = false;
        for (int i = 0; i < levelDatas.Count; i++) {
            fastestCompletions.Add(0f);
            fewestPutts.Add(0);
        }
    }
    void Start() {
        ResetLevel();
    }
    void Update() {
        if (waitingForNext) {
            if (currentLevel == levelDatas.Count - 1) {
                if (!gamedoneScreen.activeSelf) {
                    if (Input.GetKeyDown(KeyCode.Space)) {
                        levelDatas[currentLevel].cupEffects.StopEffects();
                        courseDoneDisplayRoot.SetActive(false);
                        gamedoneScreen.SetActive(true);
                        againPressedA = againPressedS = againPressedD = againPressedJ = againPressedK = againPressedL = false;
                        againPressAText.color = Color.white;
                        againPressSText.color = Color.white;
                        againPressDText.color = Color.white;
                        againPressJText.color = Color.white;
                        againPressKText.color = Color.white;
                        againPressLText.color = Color.white;
                        gameDoneTotalTime.SetText(TimeToString(totalTime));
                        gameDoneTotatPutts.SetText(totalPuttCount + "");
                    }
                } else {
                    if (Input.GetKeyDown(KeyCode.A)) {
                        againPressedA = true;
                        againPressAText.color = btnPressedColor;
                    }
                    if (Input.GetKeyDown(KeyCode.S)) {
                        againPressedS = true;
                        againPressSText.color = btnPressedColor;
                    }
                    if (Input.GetKeyDown(KeyCode.D)) {
                        againPressedD = true;
                        againPressDText.color = btnPressedColor;
                    }
                    if (Input.GetKeyDown(KeyCode.J)) {
                        againPressedJ = true;
                        againPressJText.color = btnPressedColor;
                    }
                    if (Input.GetKeyDown(KeyCode.K)) {
                        againPressedK = true;
                        againPressKText.color = btnPressedColor;
                    }
                    if (Input.GetKeyDown(KeyCode.L)) {
                        againPressedL = true;
                        againPressLText.color = btnPressedColor;
                    }
                    if (againPressedA && againPressedS && againPressedD && againPressedJ && againPressedK && againPressedL) {
                        currentLevel = 0;
                        ResetLevel();
                        gamedoneScreen.SetActive(false);
                    }
                }
            } else {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    StartNextLevel();
                }
            }
        }
    }
    public void ResetLevel() {
        waitingForNext = false;
        courseDoneDisplayRoot.SetActive(false);
        gamedoneScreen.SetActive(false);
        levelDatas[currentLevel].cupEffects.StopEffects();
        minimapCamera.transform.position = levelDatas[currentLevel].minimapCameraPos.position;
        minimapCamera.transform.rotation = levelDatas[currentLevel].minimapCameraPos.rotation;

        BallCar.instance.Show();
        BallCar.instance.transform.position = levelDatas[currentLevel].ballStartPos.position;
        BallCar.instance.lastDirection = levelDatas[currentLevel].ballStartPos.eulerAngles.y;
        BallCar.instance.Reset();
        if (timeLevelStarted >= 0) {
            totalTime += Time.time - timeLevelStarted;
        }
        timeLevelStarted = Time.time;
        totalPuttCount += puttCount;
        puttCount = 0;
        minimapText.SetText("Course " + (currentLevel + 1));
        Debug.Log("Reset course " + (currentLevel + 1));
    }
    private string TimeToString(float totalTime) {
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.CeilToInt(totalTime - 60*minutes);
        string timeText = minutes + "m ";
        if (seconds == 0) {
            timeText += "00s";
        } else if (seconds < 10) {
            timeText += "0" + seconds + "s";
        } else {
            timeText += seconds + "s";
        }
        return timeText;
    }
    public void LevelFinished() {
        waitingForNext = true;
        BallCar.instance.Hide();
        levelDatas[currentLevel].cupEffects.PlayEffects();
        courseDoneDisplayRoot.SetActive(true);
        float completionTime = Time.time - timeLevelStarted;
        totalTime += completionTime;
        timeLevelStarted = -1;
        bool newFastestTime = false;
        if (fastestCompletions[currentLevel] <= 0) {
            fastestCompletions[currentLevel] = completionTime;
        } else {
            newFastestTime = completionTime < fastestCompletions[currentLevel];
            fastestCompletions[currentLevel] = Mathf.Min(completionTime, fastestCompletions[currentLevel]);
        }
        bool newFewestPutts = false;
        if (fewestPutts[currentLevel] <= 0) {
            fewestPutts[currentLevel] = puttCount;
        } else {
            newFewestPutts = puttCount < fewestPutts[currentLevel];
            fewestPutts[currentLevel] = Mathf.Min(puttCount, fewestPutts[currentLevel]);
        }
        endTimeText.SetText(TimeToString(completionTime));
        endPuttsText.SetText(puttCount + "");
        levelNameText.SetText("Course " + (currentLevel + 1));
        totalPuttCount += puttCount;
    }
    public void StartNextLevel() {
        if (currentLevel >= 0) {
            levelDatas[currentLevel].cupEffects.StopEffects();
        }
        currentLevel++;
        if (currentLevel >= levelDatas.Count) {
            currentLevel = 0;
            return;
        }
        ResetLevel();
    }
}
