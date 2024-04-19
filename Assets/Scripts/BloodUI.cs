using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class BloodUI : MonoBehaviour
{
    public UnityEngine.UI.Image image;
    public float newAlpha;
    // Start is called before the first frame update
    void Start()
    {  

    }

    // Update is called once per frame
    void Update()
    {
        newAlpha = (float)(1f - (Player.health / Player.maxHP));
        var oldColor = image.color;

        var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);
        image.color = newColor;
    }
}
