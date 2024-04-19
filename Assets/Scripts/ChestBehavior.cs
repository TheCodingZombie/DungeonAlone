using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : MonoBehaviour
{
    public bool chestOpened = false;
    public UIBehavior ui;

    public void ChestOpening (){
        chestOpened = true;
        int randInd = Random.Range(0, GameController.length);
        Debug.Log("Random Item" + randInd);
        for (int i = 0; i <= 16; i++)
        {
            if (i == 16){
                ui.ToggleDialogue(false);
                ui.FullInventory();
                return;
            }
            if (InventoryUIController.InventoryItems[i].empty){
                Item newItem = GameController.GetItemByID(randInd);
                Item randedItem = GameController.randomizeItem(newItem);
                InventoryUIController.InventoryItems[i].HoldItem(randedItem);
                ui.ToggleDialogue(false);
                gameObject.SetActive(false);
                return;
            }
        }
    }
}
