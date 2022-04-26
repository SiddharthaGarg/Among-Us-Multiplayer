using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class MyPhotonPlayer : MonoBehaviour
{
    PhotonView myPV;
    GameObject myPlayerAvatar;
    Player[] allPlayers;
    int myPlayerNumber;

    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        allPlayers = PhotonNetwork.PlayerList;

        foreach (Player player in allPlayers)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                myPlayerNumber++;
            }

        }
        if (myPV.IsMine)
        {
            myPlayerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AU_Player"),
             AU_SpawnPoints.instance.spawnPoints[myPlayerNumber].position,
             Quaternion.identity);
            Debug.Log(myPlayerNumber);
        }
    }
}
