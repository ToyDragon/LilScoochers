using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputConfig : MonoBehaviour
{
    public static InputConfig instance;
    public class HandConfig {
        public KeyCode[] keys;
        public string[] chars;
    }
    public static HandConfig left = new HandConfig() {
        keys = new KeyCode[]{ KeyCode.A, KeyCode.S, KeyCode.D },
        chars = new string[]{ "A", "S", "D" },
    };
    public static HandConfig right = new HandConfig() {
        keys = new KeyCode[]{ KeyCode.L, KeyCode.K, KeyCode.J },
        chars = new string[]{ "J", "K", "L" },
    };
}
