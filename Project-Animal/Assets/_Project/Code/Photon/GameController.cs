using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BrilathTTV
{


    public class GameController : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoints;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject gameMenu;

        void Start()
        {
            int point = PhotonNetwork.LocalPlayer.ActorNumber;
            point = Mathf.Clamp(point, 0, PhotonNetwork.CurrentRoom.PlayerCount);

            Vector3 spawnPosition = spawnPoints.GetChild(point).position;
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleGameMenu(!gameMenu.activeSelf);
            }
        }

        public void ToggleGameMenu(bool isActive)
        {
            gameMenu.SetActive(isActive);
        }
        public void ExitGame()
        {
            SceneManager.LoadScene("MainMenu");
        }

    }
}