using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ASDJKLInputManager : MonoBehaviour
{
    public Image IndicatorA;
    public Image IndicatorS;
    public Image IndicatorD;
    public Image IndicatorJ;
    public Image IndicatorK;
    public Image IndicatorL;
    public float comboExpirationTime = 2f;
    public List<int> leftPresses = new List<int>();
    public List<int> rightPresses = new List<int>();
    public class KeyInputCombo {
        public bool increasing;
        public float time;
        public GameObject displayObject;
        public ComboDisplay comboDisplay;
    }
    public KeyInputCombo pendingLeftCombo;
    public KeyInputCombo pendingRightCombo;
    public KeyInputCombo progressLeftCombo;
    public KeyInputCombo progressRightCombo;
    public Transform comboIndicatorLeftPosStart;
    public Transform comboIndicatorLeftPos1;
    public Transform comboIndicatorLeftPosExpire;
    public Transform comboIndicatorRightPosStart;
    public Transform comboIndicatorRightPos1;
    public Transform comboIndicatorRightPosExpire;
    public Transform comboIndicatorComplete;
    public Transform comboLeftProgressUp;
    public Transform comboLeftProgressDown;
    public Transform comboRightProgressUp;
    public Transform comboRightProgressDown;
    public GameObject comboIndicatorPrefab;
    public Transform momentumBarPos;
    public Sprite spriteTurnLeft;
    public Sprite spriteTurnRight;
    public Sprite spriteGoForward;
    public Sprite spriteGoBack;
    public int keysSinceCombo;
    public int weirdKeysSinceCombo;
    public GameObject wrongPlacementHintObject;
    void OnEnable()
    {
        comboIndicatorLeftPosStart.GetComponent<Image>().color = Color.clear;
        comboIndicatorLeftPos1.GetComponent<Image>().color = Color.clear;
        // comboIndicatorLeftPos2.GetComponent<Image>().color = Color.clear;
        comboIndicatorLeftPosExpire.GetComponent<Image>().color = Color.clear;
        comboIndicatorRightPosStart.GetComponent<Image>().color = Color.clear;
        comboIndicatorRightPos1.GetComponent<Image>().color = Color.clear;
        // comboIndicatorRightPos2.GetComponent<Image>().color = Color.clear;
        comboIndicatorRightPosExpire.GetComponent<Image>().color = Color.clear;
        comboIndicatorComplete.GetComponent<Image>().color = Color.clear;

        comboLeftProgressUp.GetComponent<Image>().color = Color.clear;
        comboLeftProgressDown.GetComponent<Image>().color = Color.clear;
        comboRightProgressUp.GetComponent<Image>().color = Color.clear;
        comboRightProgressDown.GetComponent<Image>().color = Color.clear;
    }
    private void PushState(bool isLeft, int pressedValue1) {
        keysSinceCombo++;
        List<int> presses = isLeft ? leftPresses : rightPresses;
        ref KeyInputCombo progressCombo = ref progressLeftCombo;
        ref KeyInputCombo pendingCombo = ref pendingLeftCombo;
        if (!isLeft) {
            progressCombo = ref progressRightCombo;
            pendingCombo = ref pendingRightCombo;
        }

        int pressedValue2 = presses.Count >= 1 ? presses[presses.Count - 1] : -1;
        int pressedValue3 = presses.Count >= 2 ? presses[presses.Count - 2] : -1;
        presses.Add(pressedValue1);

        if (progressCombo == null) {
            progressCombo = new KeyInputCombo();
            progressCombo.displayObject = GameObject.Instantiate(comboIndicatorPrefab);
            progressCombo.comboDisplay = progressCombo.displayObject.GetComponent<ComboDisplay>();
            progressCombo.displayObject.SetActive(false);
        }

        bool setIncreasing = false;
        bool setDecreasing = false;
        float newLength = 0;
        bool submitCombo = false;

        if (pressedValue3 == 2 && pressedValue2 == 1 && pressedValue1 == 0) {
            setDecreasing = true;
            newLength = 225;
            submitCombo = true;
        } else if (pressedValue3 == 0 && pressedValue2 == 1 && pressedValue1 == 2) {
            setIncreasing = true;
            newLength = 225;
            submitCombo = true;
        } else if (pressedValue2 == 0 && pressedValue1 == 1) {
            setIncreasing = true;
            newLength = 150;
        } else if (pressedValue2 == 2 && pressedValue1 == 1) {
            setDecreasing = true;
            newLength = 150;
        } else if (pressedValue1 == 0) {
            setIncreasing = true;
            newLength = 75;
        } else if (pressedValue1 == 2) {
            setDecreasing = true;
            newLength = 75;
        }

        if (setIncreasing) {
            var parentObj = isLeft ? comboLeftProgressUp : comboRightProgressUp;
            progressCombo.displayObject.transform.SetParent(parentObj);
            progressCombo.displayObject.SetActive(true);
            progressCombo.comboDisplay.SetArrowType(true);
            progressCombo.increasing = true;
            ((RectTransform)progressCombo.displayObject.transform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, newLength);
            ((RectTransform)progressCombo.displayObject.transform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 60);
        } else if (setDecreasing) {
            var parentObj = isLeft ? comboLeftProgressDown : comboRightProgressDown;
            progressCombo.displayObject.transform.SetParent(parentObj);
            progressCombo.displayObject.SetActive(true);
            progressCombo.comboDisplay.SetArrowType(false);
            progressCombo.increasing = false;
            ((RectTransform)progressCombo.displayObject.transform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, newLength);
            ((RectTransform)progressCombo.displayObject.transform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 60);
        } else {
            progressCombo.displayObject.SetActive(false);
        }

        if (submitCombo) {
            if (pendingCombo != null) {
                pendingCombo.comboDisplay.AddTarget(isLeft ? comboIndicatorLeftPosExpire : comboIndicatorRightPosExpire);
                pendingCombo.comboDisplay.dieAtTarget = true;
            }
            pendingCombo = progressCombo;
            pendingCombo.comboDisplay.AddTarget(isLeft ? comboIndicatorLeftPos1 : comboIndicatorRightPos1);
            pendingCombo.comboDisplay.targetSize = 60;
            progressCombo = null;
            presses.Clear();
            CheckCombos();
        }
    }
    private void ComboDone(Vector3 direction) {
        if (Smack.instance != null) {
            Smack.instance.Boost(direction);
        }
        if (BallCar.instance != null) {
            BallCar.instance.Boost(direction);
        }
    }
    private void CheckCombos() {
        if (pendingLeftCombo == null || pendingRightCombo == null) {
            return;
        }
        keysSinceCombo = 0;
        weirdKeysSinceCombo = 0;

        Sprite s = spriteGoForward;
        Color c = pendingLeftCombo.comboDisplay.upColor;
        Transform target = null;
        if (pendingLeftCombo.increasing && pendingRightCombo.increasing) {
            ComboDone(Vector3.forward);
            target = momentumBarPos;
        }
        if (!pendingLeftCombo.increasing && !pendingRightCombo.increasing) {
            ComboDone(Vector3.back);
            s = spriteGoBack;
            c = pendingLeftCombo.comboDisplay.downColor;
            target = momentumBarPos;
        }
        if (pendingLeftCombo.increasing && !pendingRightCombo.increasing) {
            ComboDone(Vector3.right);
            s = spriteTurnRight;
            c = Color.white;
        }
        if (!pendingLeftCombo.increasing && pendingRightCombo.increasing) {
            ComboDone(Vector3.left);
            s = spriteTurnLeft;
            c = Color.white;
        }

        if (pendingLeftCombo.comboDisplay.targets.Count > 0) {
            pendingLeftCombo.displayObject.transform.position = pendingLeftCombo.comboDisplay.targets[pendingLeftCombo.comboDisplay.targets.Count - 1].position;
        }
        pendingLeftCombo.comboDisplay.targets.Clear();
        pendingLeftCombo.comboDisplay.AddTarget(comboIndicatorComplete);
        pendingLeftCombo.comboDisplay.dieAtTarget = true;

        if (pendingRightCombo.comboDisplay.targets.Count > 0) {
            pendingRightCombo.displayObject.transform.position = pendingRightCombo.comboDisplay.targets[pendingRightCombo.comboDisplay.targets.Count - 1].position;
        }
        pendingRightCombo.comboDisplay.targets.Clear();
        pendingRightCombo.comboDisplay.AddTarget(comboIndicatorComplete);
        pendingRightCombo.comboDisplay.dieAtTarget = true;

        var comboCombo = GameObject.Instantiate(comboIndicatorPrefab);
        comboCombo.transform.SetParent(comboIndicatorComplete);
        var comboComboDisplay = comboCombo.GetComponent<ComboDisplay>();
        comboComboDisplay.InitAsComboCombo(comboIndicatorComplete, target, pendingLeftCombo.displayObject, pendingRightCombo.displayObject, s, c);

        pendingLeftCombo = null;
        pendingRightCombo = null;
    }
    void Update()
    {
        if (MenuController.instance.AnyWindowOpen()) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            PushState(true, 0);
        } else if (Input.GetKeyDown(KeyCode.S)) {
            PushState(true, 1);
        } else if (Input.GetKeyDown(KeyCode.D)) {
            PushState(true, 2);
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E)) {
            weirdKeysSinceCombo++;
            keysSinceCombo += 2;
        }

        if (Input.GetKeyDown(KeyCode.J)) {
            PushState(false, 2);
        } else if (Input.GetKeyDown(KeyCode.K)) {
            PushState(false, 1);
        } else if (Input.GetKeyDown(KeyCode.L)) {
            PushState(false, 0);
        }

        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Semicolon) || Input.GetKeyDown(KeyCode.Colon) || Input.GetKeyDown(KeyCode.N)  || Input.GetKeyDown(KeyCode.M)) {
            weirdKeysSinceCombo++;
            keysSinceCombo += 2;
        }

        wrongPlacementHintObject.SetActive(keysSinceCombo > 12 && weirdKeysSinceCombo > 0);

        RemoveStaleCombos();
        UpdateUI();
    }
    private int GetPressAge(int value, List<int> presses) {

        for (int i = 0; i < Mathf.Min(presses.Count, 3); i++) {
            if (presses[presses.Count - 1 - i] == value) {
                return i;
            }
        }
        return 3;
    }
    public Color[] ageColors = new Color[]{Color.red, Color.blue, Color.green, Color.white};
    private void UpdateUI() {
        IndicatorA.color = ageColors[GetPressAge(0, leftPresses)];
        IndicatorS.color = ageColors[GetPressAge(1, leftPresses)];
        IndicatorD.color = ageColors[GetPressAge(2, leftPresses)];

        IndicatorJ.color = ageColors[GetPressAge(2, rightPresses)];
        IndicatorK.color = ageColors[GetPressAge(1, rightPresses)];
        IndicatorL.color = ageColors[GetPressAge(0, rightPresses)];
    }
    private void RemoveStaleCombos() {
        // if (bothCombos == null) {
        //     bothCombos = new []{ leftCombos, rightCombos };
        // }
        // foreach (var combos in bothCombos) {
        //     for (int i = combos.Count - 1; i >= 0; i--) {
        //         if (combos[i].time < Time.time - comboExpirationTime) {
        //             combos[i].comboDisplay.SetTarget(combos == leftCombos ? comboIndicatorLeftPosExpire : comboIndicatorRightPosExpire);
        //             combos[i].comboDisplay.dieAtTarget = true;
        //             combos.RemoveAt(i);
        //         }
        //     }
        //     while (combos.Count >= 3) {
        //         combos[0].comboDisplay.SetTarget(combos == leftCombos ? comboIndicatorLeftPosExpire : comboIndicatorRightPosExpire);
        //         combos[0].comboDisplay.dieAtTarget = true;
        //         combos[0].comboDisplay.expireTime = 1000;
        //         combos.RemoveAt(0);
        //     }
        // }
    }
}
