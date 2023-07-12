using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASDButtonishControl : MonoBehaviour
{
    public List<ImageBouncer> left = new List<ImageBouncer>();
    public List<ImageBouncer> right = new List<ImageBouncer>();
    public ImageBouncer leftCombo;
    public ImageBouncer rightCombo;
    public bool[] leftPresses = new bool[3];
    public bool[] rightPresses = new bool[3];
    public bool? leftComboUp = null;
    public bool? rightComboUp = null;
    public Color colorUp = Color.green;
    public Color colorDown = Color.red;
    public ImageBouncer displayPutt;
    public ImageBouncer displayLeft;
    public ImageBouncer displayRight;
    public ImageBouncer displayStop;
    public void Reset() {
        for (int i = 0; i < 3; i++) {
            left[i].HideIfShowing();
            right[i].HideIfShowing();
            leftPresses[i] = rightPresses[i] = false;
        }
    }
    void Update()
    {
        if (MenuController.instance.AnyWindowOpen()) {
            return;
        }

        foreach (bool isLeft in new []{true, false}) {
            var keys = isLeft ? InputConfig.left.keys : InputConfig.right.keys;
            var presses = isLeft ? leftPresses : rightPresses;
            var imgs = isLeft ? left : right;
            var comboIndicator = isLeft ? leftCombo : rightCombo;
            ref bool? comboUp = ref leftComboUp;
            if (!isLeft) {
                comboUp = ref rightComboUp;
            }
            if (Input.GetKeyDown(keys[0])) {
                if (presses[1] && presses[2]) {
                    if (!comboUp.HasValue || comboUp.Value != false) {
                        comboIndicator.Show(colorDown);
                    }
                    comboUp = false;
                    presses[0] = presses[1] = presses[2] = false;
                    imgs[0].Show(colorDown);
                    imgs[0].Hide();
                    imgs[1].Hide();
                    imgs[2].Hide();
                } else {
                    if (presses[1]) { imgs[1].Hide(); }
                    if (presses[2]) { imgs[2].Hide(); }
                    presses[1] = presses[2] = false;
                    presses[0] = true;
                    imgs[0].Show(colorUp);
                }
            }
            if (Input.GetKeyDown(keys[1])) {
                if (presses[0]) {
                    presses[1] = true;
                    imgs[1].Show(colorUp);
                } else if (presses[2]) {
                    presses[1] = true;
                    imgs[1].Show(colorDown);
                }
            }
            if (Input.GetKeyDown(keys[2])) {
                if (presses[0] && presses[1]) {
                    if (!comboUp.HasValue || comboUp.Value != true) {
                        comboIndicator.Show(colorUp);
                    }
                    comboUp = true;
                    presses[0] = presses[1] = presses[2] = false;
                    imgs[2].Show(colorUp);
                    imgs[0].Hide();
                    imgs[1].Hide();
                    imgs[2].Hide();
                } else {
                    if (presses[0]) { imgs[0].Hide(); }
                    if (presses[1]) { imgs[1].Hide(); }
                    presses[0] = presses[1] = false;
                    presses[2] = true;
                    imgs[2].Show(colorDown);
                }
            }
        }

        if (leftComboUp.HasValue && rightComboUp.HasValue) {
            if (leftComboUp.Value && rightComboUp.Value) {
                BallCar.instance.Boost(Vector3.forward);
                displayStop.HideIfShowing();
                displayRight.HideIfShowing();
                displayLeft.HideIfShowing();
                displayPutt.Show(Color.white);
            }
            if (!leftComboUp.Value && !rightComboUp.Value) {
                BallCar.instance.Boost(Vector3.back);
                displayPutt.HideIfShowing();
                displayRight.HideIfShowing();
                displayLeft.HideIfShowing();
                displayStop.Show(Color.white);
            }
            if (leftComboUp.Value && !rightComboUp.Value) {
                BallCar.instance.Boost(Vector3.right);
                displayPutt.HideIfShowing();
                displayStop.HideIfShowing();
                displayLeft.HideIfShowing();
                displayRight.Show(Color.white);
            }
            if (!leftComboUp.Value && rightComboUp.Value) {
                BallCar.instance.Boost(Vector3.left);
                displayPutt.HideIfShowing();
                displayStop.HideIfShowing();
                displayRight.HideIfShowing();
                displayLeft.Show(Color.white);
            }
            leftComboUp = null;
            rightComboUp = null;
            leftCombo.Hide();
            rightCombo.Hide();
        }
    }
}
