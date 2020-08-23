using System;
using System.Collections.Generic;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace BrilathTTV
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [Header("Friend UI")]
        [SerializeField] private UIFriend uiFriendPrefab;
        [SerializeField] private Transform friendContainer;
        [SerializeField] private GameObject friendPanel;
        [SerializeField] private GameObject recentPlayersPanel;
        [Header("Room UI")]
        [SerializeField] private UIPlayerSelection uiPlayerSelectionPrefab;
        [SerializeField] private Transform playerSelectionContainer;
        [SerializeField] private Dictionary<Player, UIPlayerSelection> roomPlayerSelection;
        [SerializeField] private Button playButton;
        [Header("Game UI")]
        [SerializeField] private string gameSceneName;

        public static Action<string> OnLoadNetworkScene = delegate { };

        private void Awake()
        {
            roomPlayerSelection = new Dictionary<Player, UIPlayerSelection>();
            PhotonController.OnDisplayFriends += HandleDisplayFriends;
            PhotonController.OnDisplayRoomPlayer += HandleDisplayRoomPlayer;
            PhotonController.OnRemovePlayerInRoom += HandleRemovePlayerInRoom;
            PhotonController.OnRemovePlayersInRoom += HandleRemovePlayersInRoom;
            PhotonController.OnMasterClient += HandleMasterClientSwitch;
        }

        private void OnDestroy()
        {
            PhotonController.OnDisplayFriends -= HandleDisplayFriends;
            PhotonController.OnDisplayRoomPlayer -= HandleDisplayRoomPlayer;
            PhotonController.OnRemovePlayerInRoom -= HandleRemovePlayerInRoom;
            PhotonController.OnRemovePlayersInRoom -= HandleRemovePlayersInRoom;
            PhotonController.OnMasterClient -= HandleMasterClientSwitch;
        }

        public void ToggleFriendUIPanels()
        {
            friendPanel.SetActive(!friendPanel.activeSelf);
            recentPlayersPanel.SetActive(!recentPlayersPanel.activeSelf);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void StartGame()
        {
            OnLoadNetworkScene?.Invoke(gameSceneName);
        }

        private void HandleDisplayFriends(List<FriendInfo> friends)
        {
            for (int i = 0; i < friendContainer.childCount; i++)
            {
                Destroy(friendContainer.GetChild(i).gameObject);
            }

            RectTransform rectTransform = friendContainer.GetComponent<RectTransform>();
            foreach (FriendInfo friend in friends)
            {
                UIFriend uiFriend = Instantiate(uiFriendPrefab, friendContainer);
                uiFriend.Initialize(friend);
                rectTransform.sizeDelta += new Vector2(0, 100);
            }
        }
        private void HandleDisplayRoomPlayer(Player player)
        {
            Debug.Log($"Handling Diplay Room Player {PhotonNetwork.LocalPlayer.NickName}");
            if (player.IsLocal)
            {
                GameObject uiPlayerSelectionGO = PhotonNetwork.Instantiate(uiPlayerSelectionPrefab.name, Vector3.zero, Quaternion.identity);
                UIPlayerSelection uiPlayerSelection = uiPlayerSelectionGO.GetComponent<UIPlayerSelection>();
                // UIPlayerSelection uiPlayerSelection = Instantiate(uiPlayerSelectionPrefab, playerSelectionContainer);
                uiPlayerSelection.Initialize(player, playerSelectionContainer.name);
                roomPlayerSelection.Add(player, uiPlayerSelection);
            }
        }
        private void HandleRemovePlayersInRoom()
        {
            for (int i = 0; i < playerSelectionContainer.childCount; i++)
            {
                Destroy(playerSelectionContainer.GetChild(i).gameObject);
            }
            playButton.gameObject.SetActive(false);
            roomPlayerSelection.Clear();
        }
        private void HandleRemovePlayerInRoom(Player player)
        {
            if (roomPlayerSelection.ContainsKey(player))
            {
                UIPlayerSelection removeUIPlayerSelection = roomPlayerSelection[player];
                Destroy(removeUIPlayerSelection);
            }
        }
        private void HandleMasterClientSwitch(Player masterClient)
        {
            if (PhotonNetwork.LocalPlayer.Equals(masterClient))
            {
                playButton.gameObject.SetActive(true);
            }
            else
            {
                playButton.gameObject.SetActive(false);
            }
        }
    }
}