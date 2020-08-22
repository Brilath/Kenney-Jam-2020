using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System;

namespace BrilathTTV
{
    public class PlayfabConnector : MonoBehaviour
    {
        [SerializeField] private GameSession gameSession;
        [SerializeField] private string username;
        private bool isRegistering;

        private void Awake()
        {
            isRegistering = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            PlayerPrefs.DeleteKey("PLAYFABTOKEN");
            PlayerPrefs.DeleteKey("DESIREDPHOTONROOM");

            if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            {
                PlayFabSettings.TitleId = "10FB0";
            }

            if (!string.IsNullOrEmpty(gameSession.Username))
            {
                isRegistering = false;
                username = gameSession.Username;
                Connect(username);
            }
        }

        #region  Private Methods
        private void Connect(string username)
        {
            Debug.Log($"Login to Playfab as {username}");
            var request = new LoginWithCustomIDRequest { CustomId = username, CreateAccount = true };
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnFailure);
        }
        private void GetPlayfabAccountInfo(string id)
        {
            Debug.Log("Request Playfab Account Info");
            var request = new GetAccountInfoRequest { PlayFabId = id };
            PlayFabClientAPI.GetAccountInfo(request, OnAccountInfoSuccess, OnFailure);
        }

        private void UpdateTitleDisplayName(string displayName)
        {
            Debug.Log($"Updating Playfab account's Display Name to: {displayName}");
            var request = new UpdateUserTitleDisplayNameRequest { DisplayName = displayName };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameSuccess, OnFailure);
        }
        #endregion

        #region Public Method
        public void OnRegisterClicked()
        {
            if (string.IsNullOrEmpty(username)) return;

            PlayerPrefs.SetString("USERNAME", username);
            Connect(username);
        }
        public void UpdateUsername(string name)
        {
            username = name;
        }
        #endregion

        #region Playfab Callbacks
        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log($"Playfab login success");
            gameSession.PlayfabToken = result.AuthenticationContext.ClientSessionTicket;

            GetPlayfabAccountInfo(result.PlayFabId);
        }
        private void OnAccountInfoSuccess(GetAccountInfoResult result)
        {
            gameSession.Username = result.AccountInfo.TitleInfo.DisplayName;
            if (isRegistering)
            {
                UpdateTitleDisplayName(username);
            }
            else
            {
                SceneController.LoadScene("MainMenu");
            }
        }
        private void OnDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
        {
            Debug.Log($"Successfully updated Display Name to {result.DisplayName}");
            SceneController.LoadScene("MainMenu");
        }
        private void OnFailure(PlayFabError error)
        {
            Debug.Log($"Playfab errored: {error.GenerateErrorReport()}");
        }
        #endregion
    }
}