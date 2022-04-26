using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class AU_PlayerMovement : MonoBehaviour, IPunObservable
{
    public static AU_PlayerMovement localPlayer;
    List<AU_PlayerMovement> targets;

    // Components
    Transform myAvatar;
    Animator myAnim;
    Rigidbody myRb;
    SpriteRenderer myAvatarSprite;
    static Color myColor;
    [SerializeField] Collider myCollider;
    [SerializeField] bool hasControl;
    [SerializeField] GameObject bodySprite;
    bool isDead;

    // Player Movement
    [SerializeField] InputAction WASD;

    [SerializeField] float movementSpeed;
    Vector2 movementInput;
    float direction = 1;

    // Player Role
    [SerializeField] InputAction KILL;
    [SerializeField] bool isImposter;

    // Report Body
    [SerializeField] InputAction REPORT;
    [SerializeField] LayerMask ignoreforBody;
    public static List<Transform> allBodies;
    List<Transform> bodiesFound;
    PhotonView myPv;
    [SerializeField] GameObject lightmask;
    Camera myCamera;
    //Interaction
    [SerializeField] InputAction MOUSE;
    [SerializeField] InputAction INTERACTION;
    [SerializeField] LayerMask interactableLayer;
    Vector2 mouseInput;
    private void Awake()
    {
        KILL.performed += KillTarget;
        REPORT.performed += ReportBody;
        INTERACTION.performed += Interaction;
        isImposter = false;
    }
    private void OnEnable()
    {
        WASD.Enable();
        KILL.Enable();
        REPORT.Enable();
        INTERACTION.Enable();
        MOUSE.Enable();
    }
    private void OnDisable()
    {
        WASD.Disable();
        KILL.Disable();
        REPORT.Disable();
        INTERACTION.Disable();
        MOUSE.Disable();
    }
    private void Start()
    {
        myPv = GetComponent<PhotonView>();
        if (myPv.IsMine)
        {
            localPlayer = this;
        }
        myRb = GetComponent<Rigidbody>();
        myAvatar = GetComponent<Transform>().GetChild(0);
        myAnim = GetComponent<Animator>();
        myAvatarSprite = myAvatar.GetComponent<SpriteRenderer>();
        targets = new List<AU_PlayerMovement>();
        allBodies = new List<Transform>();
        bodiesFound = new List<Transform>();
        myCamera = GetComponentInChildren<Camera>();
        //gameObject.layer = 3;

        if (myColor == Color.clear)
        {
            myColor = Color.white;
        }
        if (!myPv.IsMine)
        {
            myCamera.gameObject.SetActive(false);
            lightmask.SetActive(false);
            return;
        }
        myAvatarSprite.color = myColor;

    }
    private void Update()
    {
        myAvatar.localScale = new Vector2(direction, 1);
        if (!myPv.IsMine)
            return;
        movementInput = WASD.ReadValue<Vector2>();
        if (movementInput.x != 0)
        {
            direction = Mathf.Sign(movementInput.x);
            // myAvatar.localScale = new Vector2(direction, 1);
        }
        myAnim.SetFloat("Speed", movementInput.magnitude);
        if (allBodies.Count > 0)
        {
            BodySearch();
        }
        mouseInput = MOUSE.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        myRb.velocity = movementInput * movementSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            AU_PlayerMovement tempTarget = other.GetComponent<AU_PlayerMovement>();
            if (isImposter)
            {
                if (tempTarget.isImposter)
                    return;
                else
                {
                    targets.Add(tempTarget);
                    // Debug.Log( target.name);
                }
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            AU_PlayerMovement tempTarget = other.GetComponent<AU_PlayerMovement>();

            if (targets.Contains(tempTarget))
            {
                targets.Remove(tempTarget);
            }

        }
    }
    public void SetRole(bool newRole)
    {
        isImposter = newRole;
    }
    public void SetColor(Color newColor)
    {
        myColor = newColor;
        if (myAvatarSprite != null)
        {
            myAvatarSprite.color = myColor;
        }
    }
    void BodySearch()
    {
        foreach (Transform body in allBodies)
        {
            Ray ray = new Ray(transform.position, body.position - transform.position);
            Debug.DrawRay(transform.position, body.position - transform.position, Color.green);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~ignoreforBody))
            {
                if (hit.transform == body)
                {

                    Debug.Log(hit.transform.name);
                    Debug.Log(bodiesFound.Count);
                    if (bodiesFound.Contains(body))
                    {
                        return;
                    }
                    bodiesFound.Add(body.transform);

                }
                else
                {
                    bodiesFound.Remove(body.transform);
                }
            }

        }
    }
    void KillTarget(InputAction.CallbackContext context)
    {

        if (!myPv.IsMine)
        {
            return;
        }
        if (!isImposter)
        {
            return;
        }
        if (context.phase == InputActionPhase.Performed)
        {
            if (targets.Count != 0)
            {
                if (targets[targets.Count - 1].isDead)
                    return;
                transform.position = targets[targets.Count - 1].transform.position;
                targets[targets.Count - 1].myPv.RPC("RPC_KillTarget", RpcTarget.All);
                // targets[targets.Count - 1].Die();
                targets.RemoveAt(targets.Count - 1);


            }
        }
    }
    void ReportBody(InputAction.CallbackContext obj)
    {
        if (bodiesFound == null)
            return;
        if (bodiesFound.Count == 0)
            return;
        Transform tempBody = bodiesFound[bodiesFound.Count - 1];
        allBodies.Remove(tempBody);
        bodiesFound.Remove(tempBody);
        tempBody.GetComponent<AU_Body>().Report();
    }

    [PunRPC]
    void RPC_KillTarget()
    {
        Die();
    }
    void Die()
    {
        if (!myPv.IsMine)
        {
            return;
        }
        isDead = true;
        myAnim.SetBool("isDead", isDead);
        myCollider.enabled = false;
        gameObject.layer = 3;
        AU_Body myBody = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AU_Body"),
         transform.position,
         transform.rotation).GetComponent<AU_Body>();
        //AU_Body bodyPrefab = Instantiate(bodySprite, transform.position, transform.rotation).GetComponent<AU_Body>();
        myBody.SetColor(myAvatarSprite.color);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(direction);
            stream.SendNext(isImposter);
        }
        else
        {
            direction = (float)stream.ReceiveNext();
            isImposter = (bool)stream.ReceiveNext();
        }
    }
    public void BecomeImposter(int imposterNo)
    {
        if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[imposterNo])
        {
            isImposter = true;
            Debug.Log("I am the imposter");
        }
    }
    void Interaction(InputAction.CallbackContext context)
    {

        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("Here!");
            RaycastHit hit;
            Ray ray = myCamera.ScreenPointToRay(mouseInput);
            if (Physics.Raycast(ray, out hit, interactableLayer))
            {
                Debug.Log("Interactable object was hit!");
                if (hit.transform.tag == "Interactable")
                {

                    if (!hit.transform.GetChild(0).gameObject.activeInHierarchy)
                        return;
                    Debug.Log("PlayingGame!");
                    AU_Interactable temp = hit.collider.GetComponent<AU_Interactable>();
                    temp.PlayMiniGame();
                }
            }
            /* int bitMask = 1 << 6; // bcoz otherside is layer 6
             Debug.Log(Convert.ToString(bitMask, 2).PadLeft(32, '0'));
             Debug.Log(bitMask);
             if (Physics.Raycast(ray, out hit, bitMask))
             {
                 Debug.Log("Interactable object was hit!");
                 if (hit.transform.tag == "Interactable")
                 {

                     if (!hit.transform.GetChild(0).gameObject.activeInHierarchy)
                         return;
                     Debug.Log("PlayingGame!");
                     AU_Interactable temp = hit.collider.GetComponent<AU_Interactable>();
                     temp.PlayMiniGame();
                 }
             }*/


        }
    }
}
