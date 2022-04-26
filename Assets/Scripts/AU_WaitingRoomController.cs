using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class AU_WaitingRoomController : MonoBehaviour
{
    PhotonView myPv;
    [SerializeField] float timeToStart;
    [SerializeField] Text countdownDisplay;
    [SerializeField] int gameSceneIndex;
    [SerializeField] GameObject StartButton;
    float timerToStart;
    bool readyToStart;
    void Start()
    {
        myPv = GetComponent<PhotonView>();
        readyToStart = false;
        timerToStart = timeToStart;

    }

    void Update()
    {
        StartButton.SetActive(PhotonNetwork.IsMasterClient);

        if (readyToStart)
        {
            timerToStart -= Time.deltaTime;
            countdownDisplay.text = " " + timerToStart.ToString("0");

        }
        else
        {
            timerToStart = timeToStart;
            countdownDisplay.text = "";
        }
        if (PhotonNetwork.IsMasterClient)
        {

            if (timerToStart <= 0)
            {
                timerToStart = 100;
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.LoadLevel(gameSceneIndex);
            }
        }
    }
    public void Play()
    {
        myPv.RPC("RPC_StartGame", RpcTarget.All);
    }
    [PunRPC]
    void RPC_StartGame()
    {
        readyToStart = !readyToStart;
    }
}
