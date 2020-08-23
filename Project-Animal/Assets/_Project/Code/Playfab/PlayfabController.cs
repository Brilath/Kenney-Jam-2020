using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace BrilathTTV
{
    public class PlayfabController : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        public static Action<List<FriendInfo>> OnFriendsFound = delegate { };

        private void Awake()
        {
            UIAddFriend.OnAddFriend += HandleAddPlayfabFriend;
        }

        private void OnDestroy()
        {
            UIAddFriend.OnAddFriend -= HandleAddPlayfabFriend;
        }

        void Start()
        {
            GetPlayfabFriends();
        }

        private void HandleAddPlayfabFriend(string displayName)
        {
            Debug.Log($"Add Playfab friend {displayName}");
            var request = new AddFriendRequest { FriendTitleDisplayName = displayName };
            PlayFabClientAPI.AddFriend(request, OnFriendAddedSuccess, OnFailure);
        }

        private void GetPlayfabFriends()
        {
            Debug.Log($"Get Playfab Friends for {gameSession.Username}");
            var request = new GetFriendsListRequest { IncludeSteamFriends = false, IncludeFacebookFriends = false, XboxToken = null };
            PlayFabClientAPI.GetFriendsList(request, OnFriendsListSuccess, OnFailure);
        }

        private void OnFriendsListSuccess(GetFriendsListResult result)
        {
            Debug.Log("Retrieved Playfab friends");
            foreach (FriendInfo friend in result.Friends)
            {
                Debug.Log($"Friend Found: {friend.TitleDisplayName}");
            }
            OnFriendsFound?.Invoke(result.Friends);
        }

        private void OnFriendAddedSuccess(AddFriendResult result)
        {
            Debug.Log("You have added friend");
            GetPlayfabFriends();
        }

        private void OnFailure(PlayFabError error)
        {
            Debug.Log($"Playfab errored: {error.GenerateErrorReport()}");
        }
    }
}