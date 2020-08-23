using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrilathTTV
{
    public class GameSessionController : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        public GameSession CurrentSession
        {
            get { return gameSession; }
            private set { gameSession = value; }
        }

        public void UpdatePlayfabToken(string token)
        {
            gameSession.PlayfabToken = token;
        }
        public string GetPlayfabToken()
        {
            return gameSession.PlayfabToken;
        }

        public void UpdateUsername(string username)
        {
            gameSession.Username = username;
        }
        public string GetUsername()
        {
            return gameSession.Username;
        }

        public void UpdateDesiredPhotonRoom(string roomName)
        {
            gameSession.DesiredPhotonRoom = roomName;
        }
        public string GetDesiredPhotonRoom()
        {
            return gameSession.DesiredPhotonRoom;
        }

        public void UpdateSelectedCharacter(int selection)
        {
            gameSession.SelectedCharacter = selection;
        }
        public int GetSelectedCharacter()
        {
            return gameSession.SelectedCharacter;
        }
    }
}