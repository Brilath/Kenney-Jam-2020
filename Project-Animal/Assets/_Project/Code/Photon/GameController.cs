using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform spawnPoints;
    [SerializeField] private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        int point = PhotonNetwork.LocalPlayer.ActorNumber;
        point = Mathf.Clamp(point, 0, PhotonNetwork.CurrentRoom.PlayerCount);

        Vector3 spawnPosition = spawnPoints.GetChild(point).position;
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
