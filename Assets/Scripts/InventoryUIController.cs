using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIController : MonoBehaviour
{
    public static List<InventorySlot> InventoryItems = new List<InventorySlot>();
    private VisualElement m_Root;
    private VisualElement m_SlotContainer;
    private VisualElement m_HotBarContainer;

    private bool isSwitching = false, inventoryShow = false; // prevents spamming the inventory button

    public Player player;

    //Global variable
    private static VisualElement m_GhostIcon;
    private static VisualElement m_Inventory;
    private static bool m_IsDragging;
    private static InventorySlot m_OriginalSlot;
  

    public static int hotbarSlot = 0; //determines which hotbar slot you currently are using

    private void Awake()
    {
        //Store the root from the UI Document component
        m_Root = GetComponent<UIDocument>().rootVisualElement;
        m_GhostIcon = m_Root.Query<VisualElement>("GhostIcon");

        m_GhostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        m_GhostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);

        //Search the root for the SlotContainer Visual Element
        m_SlotContainer = m_Root.Q<VisualElement>("SlotContainer");
        // Search the root for the Inventory
        m_Inventory = m_Root.Q<VisualElement>("Inventory");

        //Create InventorySlots and add them as children to the SlotContainer
        for (int i = 0; i < 16; i++)
        {
            InventorySlot item = new InventorySlot();
            item.player = this.player;

            InventoryItems.Add(item);

            m_SlotContainer.Add(item);
        }

        // find the HotBarContainer element
        m_HotBarContainer = m_Root.Q<VisualElement>("HotBarContainer");
        
        for (int i = 0; i < 4; i++)
        {
            InventorySlot item = InventoryItems[i]; // add references to the first 4 inventory slots

            m_HotBarContainer.Add(item); // stuff them into the hotbar so we could see them there
        }   

        m_Inventory.style.display = DisplayStyle.None;
        GameController.OnInventoryChanged += GameController_OnInventoryChanged;
        m_HotBarContainer[0].AddToClassList("highlighted");
        hotbarSlot = 0;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F)){
            if (!isSwitching){
                isSwitching = true;
                if (!inventoryShow) m_Root.Q("Inventory").style.display = DisplayStyle.Flex;
                else m_Root.Q("Inventory").style.display = DisplayStyle.None;
                inventoryShow = !inventoryShow;
            }
        }
        else{
            isSwitching = false;
        }
        if (Input.GetButtonDown("Jump")){
            if (!InventoryItems[hotbarSlot].empty) InventoryItems[hotbarSlot].UseItem();
            else{
                Debug.Log("Nothing was used!");
            }
        }
        if (Input.GetKey(KeyCode.Alpha1)){
            hotbarSlot = 0;
            m_HotBarContainer[0].AddToClassList("highlighted");
            m_HotBarContainer[1].RemoveFromClassList("highlighted");
            m_HotBarContainer[2].RemoveFromClassList("highlighted");
            m_HotBarContainer[3].RemoveFromClassList("highlighted");
        }
        else if (Input.GetKey(KeyCode.Alpha2)){
            hotbarSlot = 1;
            m_HotBarContainer[1].AddToClassList("highlighted");
            m_HotBarContainer[0].RemoveFromClassList("highlighted");
            m_HotBarContainer[2].RemoveFromClassList("highlighted");
            m_HotBarContainer[3].RemoveFromClassList("highlighted");
        }
        else if (Input.GetKey(KeyCode.Alpha3)){
            hotbarSlot = 2;
            m_HotBarContainer[2].AddToClassList("highlighted");
            m_HotBarContainer[1].RemoveFromClassList("highlighted");
            m_HotBarContainer[0].RemoveFromClassList("highlighted");
            m_HotBarContainer[3].RemoveFromClassList("highlighted");
        }
        else if (Input.GetKey(KeyCode.Alpha4)){
            hotbarSlot = 3;
            m_HotBarContainer[3].AddToClassList("highlighted");
            m_HotBarContainer[1].RemoveFromClassList("highlighted");
            m_HotBarContainer[2].RemoveFromClassList("highlighted");
            m_HotBarContainer[0].RemoveFromClassList("highlighted");
        }
    }

    private void GameController_OnInventoryChanged(int[] itemID, InventoryChangeType change)
    {
        //Loop through each item and if it has been picked up, add it to the next empty slot
        foreach (int item in itemID)
        {
            if (change == InventoryChangeType.Pickup)
            {
                var emptySlot = InventoryItems.FirstOrDefault(x => x.empty);
                            
                if (emptySlot != null)
                {
                    emptySlot.HoldItem(GameController.GetItemByID(item));
                }
            }
        }
    }
    public static void StartDrag(Vector2 position, InventorySlot originalSlot)
    {
        //Set tracking variables
        m_IsDragging = true;
        m_OriginalSlot = originalSlot;

        //Set the new position
        m_GhostIcon.style.top = position.y - m_GhostIcon.layout.height / 2;
        m_GhostIcon.style.left = position.x - m_GhostIcon.layout.width / 2;

        //Set the image
        m_GhostIcon.style.backgroundImage = GameController.GetItemByID(originalSlot.item.ID).Icon.texture;

        //Flip the visibility on
        m_GhostIcon.style.visibility = Visibility.Visible;
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!m_IsDragging)
        {
            return;
        }

        //Set the new position
        m_GhostIcon.style.top = evt.position.y - m_GhostIcon.layout.height / 2;
        m_GhostIcon.style.left = evt.position.x - m_GhostIcon.layout.width / 2;

    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!m_IsDragging)
        {
            return;
        }

        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<InventorySlot> slots = InventoryItems.Where(x => 
            x.worldBound.Overlaps(m_GhostIcon.worldBound));

        //Found at least one
        if (slots.Count() != 0)
        {
            InventorySlot closestSlot = slots.OrderBy(x => Vector2.Distance
            (x.worldBound.position, m_GhostIcon.worldBound.position)).First();
            
            int tempID = -1;
            bool tempEmpty = true;

            if (closestSlot.item != null){
                tempID = closestSlot.item.ID;
                tempEmpty = closestSlot.empty;
            }

            //Set the new inventory slot with the data
            closestSlot.HoldItem(GameController.GetItemByID(m_OriginalSlot.item.ID));

            // if there was an item there before, swap the item to the original slot
            if (!tempEmpty){
               m_OriginalSlot.HoldItem(GameController.GetItemByID(tempID));
            }
            else{
                //Clear the original slot
                m_OriginalSlot.DropItem();
            }
            

        }
        //Didn't find any (dragged off the window)
        else
        {
            //m_OriginalSlot.Icon.image = GameController.GetItemByGuid(m_OriginalSlot.ItemGuid).Icon.texture;
            //Clear the original slot
            m_OriginalSlot.DropItem();
        }

        //Clear dragging related visuals and data
        m_IsDragging = false;
        m_OriginalSlot = null;
        m_GhostIcon.style.visibility = Visibility.Hidden;

    }
}
