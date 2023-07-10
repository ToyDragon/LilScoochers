using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MomenumUI : MonoBehaviour
{
    public Image greenBar;
    public Image redBar;
    void Update()
    {
        float parentHeight = Mathf.Abs(((RectTransform)transform).rect.size.y - 30);
        greenBar.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 15, parentHeight * BallCar.instance.momentum / BallCar.instance.maxMomentum);
        redBar.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 15, parentHeight * BallCar.instance.recentMaxMomentum / BallCar.instance.maxMomentum);
    }
}
