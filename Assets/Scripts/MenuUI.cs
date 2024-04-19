using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public ShopUI shop;
    public SettingsUI settings;
    
    public void PlayButton (){
        SceneManager.LoadScene("GameScene");
    }

    public void QuitButton (){
        Application.Quit();
    }

    public void ShopButton (){
        shop.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void SettingsButton (){
        settings.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
