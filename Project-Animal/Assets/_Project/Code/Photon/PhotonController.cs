using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using PlayfabFriendInfo = PlayFab.ClientModels.FriendInfo;
using PhotonFriendInfo = Photon.Realtime.FriendInfo;
using System;
using System.Linq;

namespace BrilathTTV
{
    public class PhotonController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private List<PlayfabFriendInfo> playfabFriends;
        public static Action<List<PhotonFriendInfo>> OnDisplayFriends = delegate { };
        #region Unity Methods
        private void Awake()
        {
            PlayfabController.OnFriendsFound += HandleFriendsFound;
            UIFriend.OnSwitchPhotonRoom += HandleSwitchPhotonRoom;
        }

        void Start()
        {
            string nickName = gameSession.PlayfabAccountInfo.AccountInfo.TitleInfo.DisplayName;
            ConnectToPhoton(nickName);
        }
        private void OnDestroy()
        {
            PlayfabController.OnFriendsFound -= HandleFriendsFound;
            UIFriend.OnSwitchPhotonRoom -= HandleSwitchPhotonRoom;
        }
        #endregion

        #region Private Methods
        private void ConnectToPhoton(string nickName)
        {
            Debug.Log($"Connect to Photon as {nickName}");
            PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
        private void CreatePhotonRoom(string roomName)
        {
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 4;
            PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
        }
        private void JoinPhotonRoom(string roomName)
        {
            gameSession.DesiredPhotonRoom = "";
            PhotonNetwork.JoinRoom(roomName);
        }
        private void HandleFriendsFound(List<PlayfabFriendInfo> friends)
        {
            Debug.Log($"Search for Photon Friends: {friends.Count}");
            playfabFriends = friends;
        }
        private void HandleSwitchPhotonRoom(string roomName)
        {
            gameSession.DesiredPhotonRoom = roomName;
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region  Photon Callbacks
        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon Master server");
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }
        public override void OnJoinedLobby()
        {
            Debug.Log($"Connected to Photon Lobby: {PhotonNetwork.CurrentLobby}");
            if (playfabFriends.Count == 0) return;

            string[] friendDisplayNames = playfabFriends.Select(f => f.TitleDisplayName).ToArray();
            PhotonNetwork.FindFriends(friendDisplayNames);
            if (string.IsNullOrEmpty(gameSession.DesiredPhotonRoom))
            {
                CreatePhotonRoom(PhotonNetwork.LocalPlayer.UserId);
            }
            else
            {
                JoinPhotonRoom(gameSession.DesiredPhotonRoom);
            }

        }

        public override void OnFriendListUpdate(List<PhotonFriendInfo> friendList)
        {
            Debug.Log("Photon Friend List Updated");
            foreach (PhotonFriendInfo friend in friendList)
            {
                Debug.Log($"Photon Friend Found: {friend.UserId}");
            }
            OnDisplayFriends?.Invoke(friendList);
        }
        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined Photon Room: {PhotonNetwork.CurrentRoom.Name}");
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                Debug.Log($"{player.Value.UserId} in the Photon Room {PhotonNetwork.CurrentRoom.Name} with you!");
            }
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Error joining Photon Room: {message}");
            CreatePhotonRoom(PhotonNetwork.LocalPlayer.UserId);
        }
        public override void OnLeftRoom()
        {
            Debug.Log("Left Photon room");
        }
        #endregion
    }
}