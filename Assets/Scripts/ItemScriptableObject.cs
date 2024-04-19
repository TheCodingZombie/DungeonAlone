using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum ItemType
{
    Upgrade,
    Consumable,
    Equipment,
    Default
}

public class Item
{
    public int ID;
    public string Name;
    public string DisplayName;
    public ItemType type;
    public Sprite Icon;

    [TextArea(15,20)]
    public string ItemDescription;
    public Dictionary<string, int> stats = new Dictionary<string, int>();

    public Item (int ID, string Name, string DisplayName, ItemType type, Sprite Icon, string ItemDescription, Dictionary<string, int> stats){
        this.ID = ID;
        this.Name = Name;
        this.DisplayName = DisplayName;
        this.type = type;
        this.Icon = Icon;
        this.ItemDescription = ItemDescription;
        this.stats = stats;
    }
    public Item (Item item){
        this.ID = item.ID;
        this.Name = item.Name;
        this.DisplayName = item.DisplayName;
        this.type = item.type;
        this.Icon = item.Icon;
        this.ItemDescription = item.ItemDescription;
        this.stats = item.stats;
    }
}
