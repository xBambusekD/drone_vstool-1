using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleColor : MonoBehaviour {

    public Color OnColor;
    public Color OffColor;
    public Image ToggleBackground;

    public void ChangeToggleColor(bool toggleOn) {
        ToggleBackground.color = toggleOn ? OnColor : OffColor;
    }
}
