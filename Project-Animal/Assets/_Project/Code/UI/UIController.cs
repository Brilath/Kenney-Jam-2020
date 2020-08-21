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

        private void Awake()
        {
            PhotonController.OnDisplayFriends += HandleDisplayFriends;
        }
        private void OnDestroy()
        {
            PhotonController.OnDisplayFriends -= HandleDisplayFriends;
        }
        private void HandleDisplayFriends(List<FriendInfo> friends)
        {
            //playerName.text = PhotonNetwork.CurrentRoom.Name;
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
    }
}