using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPRadius : MonoBehaviour
{
    private HPBar bar;

    // Start is called before the first frame update
    private void Awake()
    {
        bar = FindObjectOfType<HPBar>();
        bar.gameObject.SetActive(false);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            bar.gameObject.SetActive(true);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            bar.gameObject.SetActive(false);
        }
    }
}
