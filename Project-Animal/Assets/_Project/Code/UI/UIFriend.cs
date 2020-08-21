using System;
using UnityEngine;
using TMPro;
using Photon.Realtime;
namespace BrilathTTV
{
    public class UIFriend : MonoBehaviour
    {
        [SerializeField] private TMP_Text friendName;
        [SerializeField] private FriendInfo friend;

        public static Action<string> OnSwitchPhotonRoom = delegate { };
        public void Initialize(FriendInfo friend)
        {
            this.friend = friend;
            friendName.SetText(this.friend.UserId);
        }
        public void OnJoinFriend()
        {
            OnSwitchPhotonRoom?.Invoke(friend.UserId);
            // Leave room
            // rejoin lobby
            // join new room
        }
    }
}