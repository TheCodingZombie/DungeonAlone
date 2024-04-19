using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

public enum InventoryChangeType
{
    Pickup,
    Drop
}
public delegate void OnInventoryChangedDelegate(int[] itemID, InventoryChangeType change);

/// <summary>
/// Generates and controls access to the Item Database and Inventory Data
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> IconSprites;
    private static Dictionary<int, Item> m_ItemDatabase = new Dictionary<int, Item>();
    public static int length;
    public List<Item> m_PlayerInventory = new List<Item>();
    public static event OnInventoryChangedDelegate OnInventoryChanged = delegate { };
    public static Dictionary<int, string> m_SpellDatabase = new Dictionary<int, string>();

    private void Awake()
    {
        PopulateDatabase();
    }

    private void Start()
    {
        //Add the ItemDatabase to the players inventory and let the UI know that some items have been picked up
        m_PlayerInventory.Add(m_ItemDatabase[2]);
        OnInventoryChanged.Invoke(m_PlayerInventory.Select(x=> x.ID).ToArray(), InventoryChangeType.Pickup);
    }

    /// <summary>
    /// Populate the database
    /// </summary>
    public void PopulateDatabase()
    {
        m_ItemDatabase = new Dictionary<int, Item>(){
            {
                0, new Item (
                    0,
                    "book",
                    "Spell Book",
                    ItemType.Upgrade,
                    IconSprites.FirstOrDefault(x => x.name.Equals("book")),
                    "A spell book used to learn spells",
                    new Dictionary<string, int>(){
                        {"Spell", 0}
                    }
                )
            },
            {
                1, new Item (
                    1,
                    "potion",
                    "Health Potion",
                    ItemType.Consumable,
                    IconSprites.FirstOrDefault(x => x.name.Equals("potion")),
                    "A health potion that heals 1 HP",
                    new Dictionary<string, int>(){
                        {"HP", 1}
                    }
                )
            },
            {
                2, new Item (
                    2,
                    "wand",
                    "Wand",
                    ItemType.Equipment,
                    IconSprites.FirstOrDefault(x => x.name.Equals("wand")),
                    "A wand used to cast spells",
                    new Dictionary<string, int>(){
                        {"Attack", 0},
                        {"AtkSpd", 0},
                        {"Speed", 0},
                        {"MaxMana", 0}
                    }
                )
            }
        };

        length = m_ItemDatabase.Count;

        m_SpellDatabase = new Dictionary<int, string>(){
            {0, "Lightning"},
            {1, "Explosion"},
            {2, "Poison"},
            {3, ""},
            {4, ""},
            {5, ""}
        };
    }

    /// <summary>
    /// Retrieve item details based on the GUID
    /// </summary>
    /// <param name="id">ID to look up</param>
    /// <returns>Item details</returns>
    public static Item GetItemByID(int id)
    {
        if (m_ItemDatabase.ContainsKey(id))
        {
            //Item tempItem = new Item();
            return m_ItemDatabase[id];
        }

        return null;
    }

    public static Item randomizeItem(Item item)
    {
        switch (item.ID){
            case 0:
                int spellType = UnityEngine.Random.Range(0, 5);
                item.stats = new Dictionary<string, int>(){
                            {"Spell", spellType}
                        };
                return item;
            case 1:
                int hp = UnityEngine.Random.Range(1, 3);
                item.stats = new Dictionary<string, int>(){
                            {"HP", hp}
                        };
                item.ItemDescription = "A health potion that heals " + hp + " HP";
                return item;
            case 2:
                int atk = UnityEngine.Random.Range(0, 5), asp = UnityEngine.Random.Range(0, 3), spd = UnityEngine.Random.Range(0, 5), mmp = UnityEngine.Random.Range(0, 10);
                item.stats = new Dictionary<string, int>(){
                        {"Attack", atk},
                        {"AtkSpd", asp},
                        {"Speed", spd},
                        {"MaxMana", mmp}
                };
                return item;
            default:
                return null;
        }
    }
}