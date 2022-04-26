using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class QuickStartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int multiplayerSceneIndex;
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public override void OnJoinedRoom()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Room Joined");
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }
}
