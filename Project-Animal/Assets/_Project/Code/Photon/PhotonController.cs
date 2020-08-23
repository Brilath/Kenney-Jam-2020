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
        public static Action<Player> OnDisplayRoomPlayer = delegate { };
        public static Action<Player> OnRemovePlayerInRoom = delegate { };
        public static Action OnRemovePlayersInRoom = delegate { };
        public static Action<Player> OnMasterClient = delegate { };

        #region Unity Methods
        private void Awake()
        {
            playfabFriends = new List<PlayfabFriendInfo>();
            PlayfabController.OnFriendsFound += HandleFriendsFound;
            UIFriend.OnSwitchPhotonRoom += HandleSwitchPhotonRoom;
            UIAddFriend.OnNewFriend += HandleNewFriend;
            UIController.OnLoadNetworkScene += HandleLoadNetworkScene;
        }

        private void OnDestroy()
        {
            PlayfabController.OnFriendsFound -= HandleFriendsFound;
            UIFriend.OnSwitchPhotonRoom -= HandleSwitchPhotonRoom;
            UIAddFriend.OnNewFriend -= HandleNewFriend;
            UIController.OnLoadNetworkScene -= HandleLoadNetworkScene;
        }
        void Start()
        {
            string nickName = gameSession.Username;
            ConnectToPhoton(nickName);
        }

        #endregion

        #region Private Methods
        private void ConnectToPhoton(string nickName)
        {
            Debug.Log($"Connect to Photon as {nickName}");
            PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
            PhotonNetwork.SendRate = 20;
            PhotonNetwork.SerializationRate = 5;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = nickName;
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.ConnectUsingSettings();
        }
        private void CreatePhotonRoom(string roomName)
        {
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 4;
            ro.IsOpen = true;
            ro.IsVisible = true;
            PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
        }
        private void JoinPhotonRoom(string roomName)
        {
            Debug.Log($"Join Photon room: {roomName}");
            gameSession.DesiredPhotonRoom = "";
            PhotonNetwork.JoinRoom(roomName);
        }
        private void HandleFriendsFound(List<PlayfabFriendInfo> friends)
        {
            Debug.Log($"Search for Photon Friends: {friends.Count}");
            playfabFriends = friends;
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }
        private void HandleSwitchPhotonRoom(string roomName)
        {
            gameSession.DesiredPhotonRoom = roomName;
            PhotonNetwork.LeaveRoom();
        }
        private void HandleNewFriend()
        {
            Debug.Log("Photon handle new friend");
            gameSession.DesiredPhotonRoom = PhotonNetwork.CurrentRoom.Name;
        }
        private void HandleLoadNetworkScene(string sceneName)
        {
            PhotonNetwork.LoadLevel(sceneName);
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
            FindPhotonFriends();

            if (string.IsNullOrEmpty(gameSession.DesiredPhotonRoom))
            {
                CreatePhotonRoom(PhotonNetwork.LocalPlayer.UserId);
            }
            else
            {
                Debug.Log($"Joining desired Photon room {gameSession.DesiredPhotonRoom}");
                JoinPhotonRoom(gameSession.DesiredPhotonRoom);
            }
        }

        private void FindPhotonFriends()
        {
            Debug.Log("Finding Photon Friends");
            if (playfabFriends.Count != 0)
            {
                string[] friendDisplayNames = playfabFriends.Select(f => f.TitleDisplayName).ToArray();
                PhotonNetwork.FindFriends(friendDisplayNames);
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
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                OnMasterClient?.Invoke(PhotonNetwork.LocalPlayer);
            }
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                Debug.Log($"{player.Value.UserId} in the Photon Room {PhotonNetwork.CurrentRoom.Name} with you!");
                OnDisplayRoomPlayer?.Invoke(player.Value);
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"{newPlayer.UserId} has entered the room");
            OnDisplayRoomPlayer?.Invoke(newPlayer);
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Error joining Photon Room: {returnCode} {message}");
            CreatePhotonRoom(PhotonNetwork.LocalPlayer.UserId);
        }
        public override void OnLeftRoom()
        {
            Debug.Log("Left Photon room");
            OnRemovePlayersInRoom?.Invoke();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"{otherPlayer.UserId} has left Photon room {PhotonNetwork.CurrentRoom.Name}");
            OnRemovePlayerInRoom?.Invoke(otherPlayer);
        }
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log($"Switching Master Client to {newMasterClient.NickName}");
            OnMasterClient?.Invoke(newMasterClient);
        }
        #endregion
    }
}