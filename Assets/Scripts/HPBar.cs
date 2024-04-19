using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public UnityEngine.UI.Image hpBarBackground;
    public UnityEngine.UI.Image hpBarForeground;
    // Start is called before the first frame update
    void Start()
    {
        hpBarForeground.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        hpBarForeground.fillAmount =  NewEnemy.bossHealth / NewEnemy.bossMaxHP;
    }
}
