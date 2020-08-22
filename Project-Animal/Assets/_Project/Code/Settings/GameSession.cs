using UnityEngine;
using PlayFab.ClientModels;

namespace BrilathTTV
{
    [CreateAssetMenu(fileName = "GameSession", menuName = "BrilathTTV/GameData", order = 1)]
    public class GameSession : ScriptableObject
    {
        [SerializeField]
        private string playfabToken;
        public string PlayfabToken
        {
            get
            {
                playfabToken = PlayerPrefs.GetString("PLAYFABTOKEN");
                return username;
            }
            set
            {
                playfabToken = value;
                PlayerPrefs.SetString("PLAYFABTOKEN", value);
            }
        }
        [SerializeField]
        private string username;
        public string Username
        {
            get
            {
                username = PlayerPrefs.GetString("USERNAME");
                return username;
            }
            set
            {
                username = value;
                PlayerPrefs.SetString("USERNAME", value);
            }
        }
        [SerializeField]
        private string desiredPhotonRoom;
        public string DesiredPhotonRoom
        {
            get
            {
                desiredPhotonRoom = PlayerPrefs.GetString("DESIREDPHOTONROOM");
                return desiredPhotonRoom;
            }
            set
            {
                desiredPhotonRoom = value;
                PlayerPrefs.SetString("DESIREDPHOTONROOM", value);
            }
        }
        [SerializeField]
        private int selectedCharacter;
        public int SelectedCharacter
        {
            get
            {
                selectedCharacter = PlayerPrefs.GetInt("SELECTEDCHARACTER");
                return selectedCharacter;
            }
            set
            {
                selectedCharacter = value;
                PlayerPrefs.SetInt("DESIREDPHOTONROOM", value);
            }
        }
    }

}