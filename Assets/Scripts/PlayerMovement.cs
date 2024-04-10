using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//System reflection
using System.Reflection;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    //PhotonEngine
    public bool IsLocalPlayerOwner;

    //Camera
    [Header("Camera Movement")]
    public Camera MyCamera;
    public float Sensitivity;
    float camX;

    [Header("Rigidbody Movement")]
    public Rigidbody rb;
    public float speed;
    public bool Looks;
    public bool Sliding;
    public bool SlidAble;

    [Header("Jumping")]
    public GameObject JumpObject;
    public float jumprad;
    public float JumpPower;
    bool IsGrounded;
    public bool Jumpable = true;

    [Header("Sliding")]
    public bool hasAppliedForce;

    [Header("PlayerComponents")]
    public GameObject HandObject;
    public float Health = 100;
    public bool Walking;
    public bool Running;
    public float CapsuleHeight;
    public float WalkSpeed;
    public float RunningSpeed;
    public GameObject HipPosition;
    public GameObject AimPosition;
    public bool Aiming;
    public float HandLerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        IsLocalPlayerOwner = photonView.IsMine;
        MyCamera = GetComponentInChildren<Camera>();
        Aiming = false;
        SlidAble = true;
        Jumpable = true;
        Health = 100;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Looks = true;
        speed = 50;
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine){
            IsLocalPlayerOwner = photonView.IsMine;
            MyCamera.enabled = true;
            MyCamera.GetComponent<AudioListener>().enabled = true;
            MyCamera = GetComponentInChildren<Camera>();
            Look();
        }else{
            MyCamera.enabled = false;
            MyCamera.GetComponent<AudioListener>().enabled = false;
        }
    }
    void FixedUpdate()
    {
        if(photonView.IsMine){
            if(IsGrounded && !Sliding){
                HandleInput();
            }
            ApplySlideForce();
            UpdateHandPosition();
            SlidingMechanics();
            Jumping();
        }
    }
    void Look(){
        if (Looks)
        {
            float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.timeScale;
            float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.timeScale;
            transform.Rotate(transform.up * mouseX);

            camX -= mouseY;
            camX = Mathf.Clamp(camX, -70, 70);
            GetComponentInChildren<Camera>().transform.localRotation = Quaternion.Euler(camX, 0, 0);
        }
    }
    void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = (moveX * transform.right + moveZ * transform.forward).normalized;
        Vector3 move = moveDirection * speed * Time.deltaTime;
        rb.AddForce(move, ForceMode.VelocityChange);

        if (Input.GetKey(KeyCode.LeftShift) && IsGrounded)
        {
            Running = true;
            speed = RunningSpeed;
        }
        else if (IsGrounded)
        {
            speed = WalkSpeed;
            Running = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Aiming = !Aiming;
        }

    }
        void Jumping(){
        IsGrounded = false;
        foreach(Collider i in Physics.OverlapSphere(JumpObject.transform.position, jumprad)){
            if(i.transform.tag != "Player"){
                IsGrounded = true;
                break;
            }
        }
        if(IsGrounded){
            if(Input.GetKeyDown(KeyCode.Space) && !Sliding && Jumpable == true){
                StartCoroutine(JumpDelay());
                rb.AddForce(transform.up * JumpPower, ForceMode.VelocityChange);
            }
        }
        if(!Sliding){
            rb.drag = IsGrounded ? 15 : 0.1f;
        }
    }
    IEnumerator JumpDelay(){
        Jumpable = false;
        yield return new WaitForSeconds(0.1f);
        Jumpable = true;
    }

    void SlidingMechanics()
    {
        if (Input.GetKey(KeyCode.C) && IsGrounded)
        {
            if (Input.GetKeyDown(KeyCode.C) && !Sliding)
            {
                Sliding = true;
            }

            Sliding = true;
            GetComponentInChildren<CapsuleCollider>().material.dynamicFriction = 0;
            GetComponentInChildren<CapsuleCollider>().material.staticFriction = 0f;
            CapsuleHeight = Mathf.Lerp(CapsuleHeight, 1, 0.05f);
            rb.drag = 0.2f;
            rb.mass = 10;
        }
        else
        {
            Sliding = false;
            GetComponentInChildren<CapsuleCollider>().material.dynamicFriction = 0.6f;
            GetComponentInChildren<CapsuleCollider>().material.staticFriction = 0.6f;
            GetComponentInChildren<CapsuleCollider>().height = CapsuleHeight;
            rb.mass = 1;
            CapsuleHeight = Mathf.Lerp(CapsuleHeight, 2, 0.05f);
            hasAppliedForce = false;
        }

        GetComponentInChildren<CapsuleCollider>().height = CapsuleHeight;
    }

    void ApplySlideForce()
    {
        if (Sliding && !hasAppliedForce && SlidAble && rb.velocity.magnitude > 1)
        {
            Debug.Log("Force");
            StartCoroutine(SlideForceDelay());
            rb.AddForce(transform.forward * 20 * Time.deltaTime, ForceMode.VelocityChange);
            hasAppliedForce = true;
        }
    }

    void UpdateHandPosition()
    {
        if (Aiming)
        {
            Vector3 aimPositionWorld = AimPosition.transform.position;
            Vector3 hipPositionWorld = HipPosition.transform.position;
            HandObject.transform.position = Vector3.Lerp(hipPositionWorld, aimPositionWorld, HandLerpSpeed);
        }
        else if (!Aiming)
        {
            Vector3 aimPositionWorld = AimPosition.transform.position;
            Vector3 hipPositionWorld = HipPosition.transform.position;
            HandObject.transform.position = Vector3.Lerp(aimPositionWorld, hipPositionWorld, HandLerpSpeed);
        }
    }

    IEnumerator SlideForceDelay()
    {
        SlidAble = false;
        yield return new WaitForSeconds(0.4f);
        SlidAble = true;
    }
}
