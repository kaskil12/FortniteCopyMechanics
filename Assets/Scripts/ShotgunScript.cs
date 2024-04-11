using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ShotgunScript : MonoBehaviourPunCallbacks
{
    public bool enabled;
    [Header("Shotgun Attributes")]
    public float damage;
    public float range;
    public float fireRate;
    public float reloadTime;
    public float knockbackForce;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Equip()
    {
        enabled = true;
        GetComponentInChildren<Collider>().enabled = false;
        GetComponentInChildren<Rigidbody>().isKinematic = true;
    }
    public void Unequip()
    {
        enabled = false;
        GetComponentInChildren<Collider>().enabled = true;
        GetComponentInChildren<Rigidbody>().isKinematic = false;
    }
}
