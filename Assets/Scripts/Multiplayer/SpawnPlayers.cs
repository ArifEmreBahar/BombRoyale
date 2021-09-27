using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPreFab;

    private void Start()
    {
        Vector3 randomPositon = new Vector3(0.5f, 3f, 0.5f);
        PhotonNetwork.Instantiate(playerPreFab.name, randomPositon, Quaternion.identity);
    }
}