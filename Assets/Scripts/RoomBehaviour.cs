using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls; // 0 - Up 1 -Down 2 - Right 3- Left
    public GameObject[] doors;
    public Transform Player;
    public NewEnemy agent; // boss
    public bool isEnemyRoom; // false = normal room, true = Order Room
    public static NewEnemy newAgent; // reference for global use

    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
        if (isEnemyRoom){
            NewEnemy newEnemy = Instantiate(agent, Vector3.zero, Quaternion.identity, transform);
            newEnemy.Agent.Warp(new Vector3(transform.position.x, 2, transform.position.z));
            newEnemy.Player = Player;
            newAgent = newEnemy;
        }
        
    }
}
