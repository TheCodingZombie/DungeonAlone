using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public TMP_Text tooltip;

    void Start () {
        gameObject.SetActive(false);
    }

    public void GenerateTooltip (Item item){
        string statText = "";
        if (item.stats.Count > 0){
            foreach (var stat in item.stats){
                statText += stat.Key.ToString() + ": " + stat.Value.ToString() + "\n";
            }
        }

        string tooltipText = string.Format("<b>{0}</b>\n{1}\n\n<b>{2}</b>", item.DisplayName, item.ItemDescription, statText);
        tooltip.text = tooltipText;
        gameObject.SetActive(true);
    }
}
