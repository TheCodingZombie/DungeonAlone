using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Rigidbody))]
public class Projectiles : MonoBehaviour
{
    // public variables for changing
    public float spd = 2f;
    public int Damage = 1;
    protected Transform Target = null;
    protected float flightTime;
    protected float maxFlightTime = 8f;

    [SerializeField]
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        flightTime = 0f;
    }

    protected virtual void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Walls"){
            Destroy(gameObject);
        }
        
        IDamageable dmgable;

        if (other.TryGetComponent<IDamageable>(out dmgable))
        {
            dmgable.TakeDamage(Damage);
            Debug.Log("Hit" + other.gameObject);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Target == null)
        {
            FindTarget();
        }
        else
        {
            // Move towards the Target
            UnityEngine.Vector3 direction = Target.position - transform.position;
            transform.Translate(direction.normalized * spd * Time.deltaTime, Space.World);

            // Rotate towards the Target
            UnityEngine.Quaternion lookRotation = UnityEngine.Quaternion.LookRotation(direction);
            transform.rotation = UnityEngine.Quaternion.RotateTowards(transform.rotation, lookRotation, spd * Time.deltaTime);

            // Update Target's position for dynamic tracking
            FindTarget();
        }

        flightTime += Time.deltaTime;
        if (flightTime >= maxFlightTime)
        {
            Destroy(gameObject); // Destroy the projectile if it exceeds the maximum flight time
        }
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = UnityEngine.Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null){
            Target = closestEnemy.transform;
        }
        else{
            Target = null;
        }
    }
}
