using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NewEnemy : MonoBehaviour, IDamageable
{
    public Transform Player;
    public float UpdateRate = 0.1f;
    public NavMeshAgent Agent;
    public AttackRadius AttackRadius;
    private float turnSpeed = 3.0f;

    public static int bossHealth = 500;
    public static int bossMaxHP = 500;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        StartCoroutine(FollowTarget());
    }

    private void Update()
    {
        FaceTarget();
    }
    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);
        
        while(enabled)
        {
            Agent.SetDestination(new Vector3 (Player.transform.position.x, 2, Player.transform.position.z));
            yield return Wait;
        }
    }
    private void FaceTarget()
    {
        Vector3 direction = (Player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    public void TakeDamage(int Damage)
    {
        bossHealth -= Damage;
        if (bossHealth <= 0)
        {

        }
        else{
            
        }
    }

    public Transform GetTransform()
    {
        return GetComponent<Transform>();
    }
}