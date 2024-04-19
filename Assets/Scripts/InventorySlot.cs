using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;


public class InventorySlot : VisualElement
{
    public Image Icon;
    public Item item; // pointer to an item that it contains.
    public bool empty = true; // checks if an inventory slot is empty, this is a band-aid fix because the original code has been edited so much it just doesn't work lol.
    private Tooltip newTip;
    public Player player;
    public InventorySlot()
    {
        //Create a new Image element and add it to the root
        Icon = new Image();
        Add(Icon);

        //Add USS style properties to the elements
        Icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");
        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        RegisterCallback<PointerLeaveEvent>(OnPointerLeave);

        newTip = GameObject.Find("Tooltip").GetComponent<Tooltip>();
    }

    public void HoldItem(Item item)
    {
        Icon.image = item.Icon.texture;
        this.item = item;
        empty = false;
    }

    public void DropItem()
    {
        item = null;
        empty = true;
        Icon.image = null;
    }

    public void UseItem()
    {
        if (item.type == ItemType.Consumable || item.type == ItemType.Upgrade){
            player.UseItem(item);
            item = null;
            Icon.image = null;
            empty = true;
        }
        else if (item.type == ItemType.Equipment){
            player.Attack();
            Debug.Log("Used a Wand!");
        }
        else{
            return;
        }
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        //Not the left mouse button
        if (evt.button != 0 || (item.ID < 0))
        {
            return;
        }

        //Clear the image
        Icon.image = null;

        //Start the drag
        InventoryUIController.StartDrag(evt.position, this);
    }

    private void OnPointerEnter(PointerEnterEvent evt){
        if(item != null){
            newTip.GenerateTooltip(this.item);
        }
    }
    
    private void OnPointerLeave(PointerLeaveEvent evt){
        newTip.gameObject.SetActive(false);
    }

    #region UXML
    [Preserve]
    public new class UxmlFactory : UxmlFactory<InventorySlot, UxmlTraits> { }

    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits { }
    #endregion
    
}

