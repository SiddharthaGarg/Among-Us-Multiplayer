using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject buttonLoading;
    [SerializeField] private GameObject buttonJoinRoom;
    [SerializeField] private GameObject buttonCancel;
    [SerializeField] private int RoomSize;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        buttonLoading.SetActive(false);
        buttonJoinRoom.SetActive(true);
    }
    public void OnClickButtonJoinRoom()
    {

        buttonJoinRoom.SetActive(false);
        buttonCancel.SetActive(true);
        Debug.Log("Joining!");
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnClickButtonCancel()
    {

        buttonCancel.SetActive(false);
        PhotonNetwork.LeaveRoom();
        buttonJoinRoom.SetActive(true);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Room Joining failed");
        CreateRoom();
    }
    public void CreateRoom()
    {
        int roomCode = Random.Range(1, 10000);
        RoomOptions roomOptions = new RoomOptions() { IsOpen = true, IsVisible = true, MaxPlayers = (byte)RoomSize };
        PhotonNetwork.CreateRoom("Room" + roomCode, roomOptions);
        Debug.Log("Room" + roomCode + "created!");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room failed as room with same name already exists");
        CreateRoom();
    }

}
