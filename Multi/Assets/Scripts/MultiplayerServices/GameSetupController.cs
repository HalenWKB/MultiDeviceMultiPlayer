using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

namespace MultiplayerServices
{
    public class GameSetupController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            CreatePlayer();
        }

        void CreatePlayer()
        {
            Debug.Log("Create Player");
            Vector3 spawnPos = new Vector3(Random.value, Random.value, 0) * 4;
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), spawnPos, Quaternion.identity);
        }
    }

}
