using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

namespace BrilathTTV
{
    [CreateAssetMenu(fileName = "GameSession", menuName = "BrilathTTV/GameData", order = 1)]
    public class GameSession : ScriptableObject
    {
        [SerializeField]
        private LoginResult playfabSession;
        public LoginResult PlayfabSession { get { return playfabSession; } set { playfabSession = value; } }
        [SerializeField]
        private GetAccountInfoResult playfabAccountInfo;
        public GetAccountInfoResult PlayfabAccountInfo { get { return playfabAccountInfo; } set { playfabAccountInfo = value; } }
        [SerializeField]
        private string desiredPhotonRoom;
        public string DesiredPhotonRoom { get { return desiredPhotonRoom; } set { desiredPhotonRoom = value; } }
    }
}