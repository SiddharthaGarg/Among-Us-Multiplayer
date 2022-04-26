using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class GameSetupController : MonoBehaviour
{

    void Start()
    {
        Debug.Log("Creating Player");
        CreatePlayer();
    }
    void CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
    }


}
