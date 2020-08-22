using System;
using System.Collections;
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
        [Header("Room UI")]
        [SerializeField] private UIPlayerSelection uiPlayerSelectionPrefab;
        [SerializeField] private Transform playerSelectionContainer;
        [SerializeField] private Dictionary<Player, UIPlayerSelection> roomPlayerSelection;

        private void Awake()
        {
            roomPlayerSelection = new Dictionary<Player, UIPlayerSelection>();
            PhotonController.OnDisplayFriends += HandleDisplayFriends;
            PhotonController.OnDisplayRoomPlayer += HandleDisplayRoomPlayer;
            PhotonController.OnRemovePlayerInRoom += HandleRemovePlayerInRoom;
            PhotonController.OnRemovePlayersInRoom += HandleRemovePlayersInRoom;
        }

        private void OnDestroy()
        {
            PhotonController.OnDisplayFriends -= HandleDisplayFriends;
            PhotonController.OnDisplayRoomPlayer -= HandleDisplayRoomPlayer;
            PhotonController.OnRemovePlayerInRoom -= HandleRemovePlayerInRoom;
            PhotonController.OnRemovePlayersInRoom -= HandleRemovePlayersInRoom;
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

            // foreach (UIPlayerSelection playerSelection in FindObjectsOfType<UIPlayerSelection>())
            // {
            //     playerSelection.transform.SetParent(playerSelectionContainer);
            // }
        }
        private void HandleRemovePlayersInRoom()
        {
            for (int i = 0; i < playerSelectionContainer.childCount; i++)
            {
                Destroy(playerSelectionContainer.GetChild(i).gameObject);
            }
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
    }
}