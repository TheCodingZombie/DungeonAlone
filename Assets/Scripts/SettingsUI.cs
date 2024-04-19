using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public MenuUI mainMenu;

    public void MainButton()
    {
        mainMenu.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
