using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AU_GameController : MonoBehaviour
{

    PhotonView myPv;
    int whichPlayerisImposter;
    private void Start()
    {
        myPv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            PickImposter();
        }
    }
    void PickImposter()
    {
        whichPlayerisImposter = Random.Range(0, PhotonNetwork.PlayerList.Length);
        myPv.RPC("RPC_PickImposter", RpcTarget.All, whichPlayerisImposter);
        Debug.Log(whichPlayerisImposter);

    }
    [PunRPC]
    void RPC_PickImposter(int imposterNo)
    {
        whichPlayerisImposter = imposterNo;
        AU_PlayerMovement.localPlayer.BecomeImposter(imposterNo);
    }


}
