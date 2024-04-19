using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class UIBehavior : MonoBehaviour
{
    // 0 for chest-pop-up, 1 for inventory screen, 2 for hotbar, 3 for full inventory pop up
    [System.Serializable]
    public class UIOB
    {
        public GameObject UIObject;
        public bool activeStart;
        public int UIType;
        public void active (bool x) {
            UIObject.SetActive(x);
        }
    }
    public UIOB[] list;
    private List<bool> status = new List<bool>();
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < list.Length; i++){
            list[i].active(list[i].activeStart);
            status.Add(list[i].activeStart);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F)){ // bring up inventory screen
            for (int i = 0; i < list.Length; i++){
                if (list[i].UIType == 2){
                    status[i] = !status[i];
                    list[i].active(status[i]);
                }
            }
        }
    }

    public void ToggleDialogue (bool x){
        for (int i = 0; i < list.Length; i++){
            if (list[i].UIType == 0){
                status[i] = x;
                list[i].active(x);
            }
        }
    }

    public void FullInventory (){
        StartCoroutine(TempDialogue());
    }

    private IEnumerator TempDialogue()
    {
        WaitForSeconds Wait = new WaitForSeconds(6.0f);
        
        for (int i = 0; i < list.Length; i++){
            if (list[i].UIType == 3){
                status[i] = !status[i];
                list[i].active(status[i]);
            }
        }
        yield return Wait;
        for (int i = 0; i < list.Length; i++){
            if (list[i].UIType == 3){
                status[i] = !status[i];
                list[i].active(status[i]);
            }
        }
    }
}
