using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] Transform playerBody;
    [SerializeField] GameObject middleRay;

    // Target position, updated through movement input, be careful when editing this as most of the code works based upon it
    private Vector3 target;

    // Adjustable roation and movement variables
    [SerializeField] float rotationSpeed = 6.0f;
    [SerializeField] float movementSpeed = 2.0f;
    [SerializeField] float gridSize = 8.0f;

    // Collision detection variables
    private float detectWallDistance;

    public bool isMoving;
    public bool isRotating;

    public UIBehavior chestUI;
    private bool chest = false;
    private bool inDialogue = false; //prevents further updates
    private bool open = false;

    // health and damage
    public static int health = 2;
    public static int maxHP = 2;
    public int dmg = 1;
    public float atkSpd = 2.0f;
    public static int Mana = 10;

    public int baseMMP = 10;
    public static int MMP = 10;
    public List<int> spellsList = new List<int>();
    public Projectiles projectiles;
    public IDamageable targeted;
    public GameObject spawnPoint;
    private Item hotbar;
    private int bonusSpeed = 0;
    private int bonusAtk = 0;
    private float bonusAtkSpd = 0;
    private TMP_Text popup;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the directional raycast distance to detect walls with small error correction amount
        detectWallDistance = gridSize + 0.2f;
        Physics.queriesHitTriggers = false;
        spellsList.Add(0);
    }

    private void Awake()
    {
        popup = GameObject.Find("LearnedSpell").GetComponent<TMP_Text>();
        popup.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hotbar = InventoryUIController.InventoryItems[InventoryUIController.hotbarSlot].item;
        if (hotbar != null){
            if (hotbar.type == ItemType.Equipment){
                MMP = baseMMP + hotbar.stats["MaxMana"];
            }
        }
        
        // This state allows the player to input movement
        if (isMoving == false && isRotating == false && !chest)
        {
            // Sets a movement target within a raycast collision detection system
            CollisionCheckedMovement();

            // Sets a rotation tartget
            SetRotationTarget();

            if (Input.GetKey(KeyCode.M)){
                if (Mana < MMP){
                    Mana += 1;
                }
            }
        }

        else if (chest && !inDialogue){
            chestUI.ToggleDialogue(true);
            inDialogue = true;
        }
        else if (inDialogue){
            if (Input.GetKey(KeyCode.R)){
                chestUI.ToggleDialogue(false);
                open = true;
                inDialogue = false;
            }
            if (Input.GetKey(KeyCode.Escape)){
                chestUI.ToggleDialogue(false);
                chest = false;
                inDialogue = false;
            }
        }
        
        targeted = RoomBehaviour.newAgent.GetComponent<IDamageable>();
    }

    private void Update()
    {
        // Carries out movement to targets whilst adjusting the y axis height, to allow climbing and descending of stairs and falling due to gravity
        if (isMoving == true)
        {
            HeightCheckedMovementAnimation();
        }

        // Carries out rotation to targets whilst adjusting the final rotation vector to 90o to remove rounding errors
        if (isRotating == true)
        {
            ErrorCorrectionRotation();
        }
       
    }

    void CollisionCheckedMovement()
    {
        RaycastHit wallHit;

        // Remember to delete the "wall" tag check from the Physics.Raycast code or set your walls to have the tag "Walls"

        if (Input.GetKey(KeyCode.W))
        {
            Ray forwardRay = new Ray(middleRay.transform.position, transform.forward);

            
            if (Physics.Raycast(forwardRay, out wallHit, detectWallDistance) && wallHit.distance <= detectWallDistance)
            {
                isMoving = false;
            }

            else
            {
                SetTargetForward();
            }

        }

        if (Input.GetKey(KeyCode.S))
        {
            Ray backwardRay = new Ray(middleRay.transform.position, -transform.forward);

            if (Physics.Raycast(backwardRay, out wallHit, detectWallDistance) && wallHit.distance <= detectWallDistance)
            {
                 isMoving = false;
            }

            else
            {
                SetTargetBackward();
            }

        }

        if (Input.GetKey(KeyCode.A))
        {
            Ray leftRay = new Ray(middleRay.transform.position, -transform.right);
            if (Physics.Raycast(leftRay, out wallHit, detectWallDistance) && wallHit.distance <= detectWallDistance)
            {
                isMoving = false;
            }

            else
            {
                SetTargetLeft();
            }

        }

        if (Input.GetKey(KeyCode.D))
        {
            Ray rightRay = new Ray(middleRay.transform.position, transform.right);
            if (Physics.Raycast(rightRay, out wallHit, detectWallDistance) && wallHit.distance <= detectWallDistance)
            {
                isMoving = false;
            }

            else
            {
                SetTargetRight();
            }

        }
    }

    void SetTargetForward()
    { 
        Vector3 forward = playerBody.position + transform.forward * gridSize;
        target = forward;
        isMoving = true;
    }

    void SetTargetBackward()
    {
       Vector3 backward = playerBody.position + transform.forward * -gridSize;
        target = backward;
        isMoving = true;
    }

    void SetTargetLeft()
    {
        Vector3 left = playerBody.position + transform.right * -gridSize;
        target = left;
        isMoving = true;
    }

    void SetTargetRight()
    {
        Vector3 right = playerBody.transform.position + transform.right * gridSize;
        target = right;
        isMoving = true;
    }

    void SetRotationTarget()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            SetRotateTargetLeft();
        }

        if (Input.GetKey(KeyCode.E))
        {
            SetRotateTargetRight();
        }
    }

    void SetRotateTargetLeft()
    {
        isRotating = true;
        target = transform.right * -gridSize;
    }

    void SetRotateTargetRight()
    {
        isRotating = true;
        target = transform.right * gridSize;
    }

    void HeightCheckedMovementAnimation()
    {

            RaycastHit floorHit;

            Ray downRay = new Ray(middleRay.transform.position, -transform.up);

            if (Physics.Raycast(downRay, out floorHit))
            {
                SetTargetHeightToCurrentHeight(floorHit);
            }

            AnimatedMovement();

            HeightCorrectedMovementCheck(floorHit);

    }

    void SetTargetHeightToCurrentHeight(RaycastHit floorDistance)
    {
        target.y = playerBody.transform.position.y;

        if (floorDistance.distance < 2)
        {
            playerBody.transform.position += transform.up * gridSize * Time.deltaTime;
        }

        if (floorDistance.distance > 2)
        {
            playerBody.transform.position += -transform.up * gridSize * Time.deltaTime;
        }
    }

    void AnimatedMovement()
    {
        
        
        if (hotbar != null){
            if (hotbar.type == ItemType.Equipment){
                bonusSpeed = InventoryUIController.InventoryItems[InventoryUIController.hotbarSlot].item.stats["Speed"];
            }
        }
        else{
            bonusSpeed = 0;
        }
        
        var step = Time.deltaTime * gridSize * (movementSpeed + bonusSpeed);

        playerBody.position = Vector3.MoveTowards(playerBody.position, target, step);
    }

    void HeightCorrectedMovementCheck(RaycastHit floorDistance)
    {
        float movementDistance = Vector3.Distance(target, playerBody.transform.position);

        if (movementDistance == 0)
        {
            if (floorDistance.distance < 2.0)
            {
                playerBody.transform.position += transform.up * (2 - floorDistance.distance);
            }

            if (floorDistance.distance > 2.0)
            {
                playerBody.transform.position += -transform.up * (floorDistance.distance - 2);
            }
            isMoving = false;
        }
    }

    void ErrorCorrectionRotation()
    {
        var smoothRotation = Time.deltaTime * rotationSpeed;

        Quaternion quaternionTarget = Quaternion.LookRotation(target);
        transform.rotation = Quaternion.Lerp(transform.rotation, quaternionTarget, smoothRotation);
        float rotationAngle = Quaternion.Angle(transform.rotation, quaternionTarget);
        // corrects the inaccuracy of the rotation
        if (rotationAngle == 0f)
        {
            isRotating = false;
            transform.rotation = quaternionTarget;
        }
    }

    // detects if the trigger is touching a chest
    private void OnTriggerEnter (Collider other)
    {
        var elc = other.gameObject.GetComponent<ChestBehavior>();
        if (other.tag == "Chest" && !elc.chestOpened){
            chest = true;
            elc.ui = chestUI;
        }
        
    }

    private void OnTriggerStay (Collider other)
    {
        var elc = other.gameObject.GetComponent<ChestBehavior>();
        if (other.tag == "Chest" && !elc.chestOpened){
            if (open){
                elc.ChestOpening();
                if (elc.chestOpened){
                    chest = false;
                    open = false;
                    inDialogue = false;
                }
            }
        }
    }

    private void OnTriggerExit (Collider other)
    {
        chest = false;
        open = false;
        inDialogue = false;
    }


    // Trouble shooting:
    // If the player is passing through walls occasionally:-
    // Set the wall detect distance amount higher than 0.1f;
    // or
    // Make sure that you set wall tags to "Walls" or remove that section of the code that checks for tags

    public void TakeDamage(int Damage)
    {
        health -= Damage;
        if (health <= 0){
        }

    }

    public Transform GetTransform()
    {
        return GetComponent<Transform>();
    }

    public void Attack()
    {
        if (Mana > 0){
            StartCoroutine(Swipe());
        }
    }

    public IEnumerator Swipe()
    {
        Debug.Log("Made bullet!");

        Projectiles bullet = Instantiate(projectiles, spawnPoint.transform.position, transform.rotation);

        if (hotbar != null){
            if (hotbar.type == ItemType.Equipment){
                bonusAtk = hotbar.stats["Attack"];
                bonusAtkSpd = hotbar.stats["AtkSpd"];
            }
        }

        bullet.Damage = dmg + bonusAtk;

        Mana -= 1;

        WaitForSeconds Wait = new WaitForSeconds(atkSpd - bonusAtkSpd);

        yield return Wait;
    }

    public void UseItem(Item item)
    {
        if (item.type == ItemType.Consumable)
        {
            health += item.stats["HP"];
        }
        else if (item.type == ItemType.Upgrade)
        {
            if (!spellsList.Contains(item.stats["Spell"])){
                spellsList.Add(item.stats["Spell"]);
                StartCoroutine(Popup(item));
            }
            else{
                StartCoroutine(Popup());
            }
        }
    }
    
    private IEnumerator Popup(Item item)
    {
        popup.gameObject.SetActive(true);

        popup.text = "You Learned " + GameController.m_SpellDatabase[item.stats["Spell"]] + " Spell!";

        WaitForSeconds Wait = new WaitForSeconds(3f);

        yield return Wait;

        popup.gameObject.SetActive(false);
    }

        private IEnumerator Popup()
    {
        popup.gameObject.SetActive(true);

        popup.text = "You already know that Spell!";

        WaitForSeconds Wait = new WaitForSeconds(3f);

        yield return Wait;

        popup.gameObject.SetActive(false);
    }
}