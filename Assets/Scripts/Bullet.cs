//Bullet script for parabolic bullet: 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks
{
    private float speed;
    private float gravity;
    private Vector3 startPos;
    private Vector3 startForward;
    private bool isInitialized = false;
    private float StartTime = -1f;
    public GameObject BulletHolePrefab;
    public GameObject BloodSplatterPrefab;
    float Damage;
    public void Initialize(Transform startPoint, float speed, float gravity, float damage)
    {
        startPos = startPoint.position;
        startForward = startPoint.forward.normalized;
        this.speed = speed;
        this.gravity = gravity;
        isInitialized = true;
        Damage = damage;
    }

    private Vector3 FindPointOnBullet(float time){
        Vector3 point = startPos + (startForward * speed * time);
        // Debug.Log(gravity);
        Vector3 gravityVec = Vector3.down * gravity * time * time;
        // Debug.Log(gravityVec);
        return point + gravityVec;
    }
    private bool CastRayBetweenPoints(Vector3 startPoint, Vector3 endPoint, out RaycastHit hit){
        return Physics.Raycast(startPoint, endPoint - startPoint, out hit, (endPoint - startPoint).magnitude);
    }
    private void FixedUpdate()
    {
        if(!isInitialized)return;
        if(StartTime < 0)StartTime = Time.time;
        //Debug.Log(StartTime);
        RaycastHit hit;
        float currentTime = Time.time - StartTime;
        float nextTime = currentTime + Time.fixedDeltaTime;

        Vector3 currentPoint = FindPointOnBullet(currentTime);
        Vector3 nextPoint = FindPointOnBullet(nextTime);
        //.Log(nextTime);
        // Debug.Log(currentPoint);
        if(CastRayBetweenPoints(currentPoint, nextPoint, out hit)){
            Debug.Log(hit.collider.name);
            if(hit.collider.tag == "Enemy"){
                // hit.collider.GetComponent<Enemy>().TakeDamageEnemy(Damage);
                GameObject BloodSplatter = Instantiate(BloodSplatterPrefab, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
            }else{
                GameObject BulletHole = Instantiate(BulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
            }
            Destroy(gameObject);
        }
    }


    void Update()
    {
        if(!isInitialized || StartTime < 0)return;
        float currentTime = Time.time - StartTime;
        Vector3 currentPoint = FindPointOnBullet(currentTime);
        transform.position = currentPoint;
    }
}