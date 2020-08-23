using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace BrilathTTV
{
    public class UIPlayerSelection : MonoBehaviour
    {
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private Image animalImage;
        [SerializeField] private GameObject selectButtons;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private int currentSelection;
        [SerializeField] private Player player;
        [SerializeField] private PhotonView selectionPhotonView;

        private void Awake()
        {
            selectionPhotonView = GetComponent<PhotonView>();
            currentSelection = 0;
        }

        public void Initialize(Player player, string parentTransform)
        {
            this.player = player;

            UpdateCharacterSelection(currentSelection);

            selectionPhotonView.RPC("RPCSetSelectionButtons", RpcTarget.AllBuffered);
            selectionPhotonView.RPC("RPCSetParentTransform", RpcTarget.AllBuffered, parentTransform);
            SetupPlayerSelection();
        }

        private void SetupPlayerSelection()
        {
            currentSelection = GetCharacterSelection();

            ShowSelected(currentSelection);
            selectionPhotonView.RPC("RPCSetUsernameText", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.UserId);
        }

        private void ShowSelected(int selection)
        {
            selectionPhotonView.RPC("RPCShowSelected", RpcTarget.AllBuffered, selection);
        }

        private void UpdateCharacterSelection(int selection)
        {
            Debug.Log($"Updating Photon Custom Property {NetworkCustomSettings.CHARACTER_SELECTION_NUMBER} for {PhotonNetwork.LocalPlayer.UserId} to {selection}");

            ExitGames.Client.Photon.Hashtable playerSelectionProperty = new ExitGames.Client.Photon.Hashtable()
            {
                {NetworkCustomSettings.CHARACTER_SELECTION_NUMBER, selection}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperty);
        }

        private int GetCharacterSelection()
        {
            int selection = 0;
            object playerSelectionObj;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(NetworkCustomSettings.CHARACTER_SELECTION_NUMBER, out playerSelectionObj))
            {
                selection = (int)playerSelectionObj;
            }
            return selection;
        }

        public void PreviousSelection()
        {
            currentSelection--;
            if (currentSelection < 0)
            {
                currentSelection = sprites.Length - 1;
            }
            UpdateCharacterSelection(currentSelection);
            ShowSelected(currentSelection);
        }
        public void NextSelection()
        {
            currentSelection++;
            if (currentSelection > sprites.Length - 1)
            {
                currentSelection = 0;
            }
            UpdateCharacterSelection(currentSelection);
            ShowSelected(currentSelection);
        }
        [PunRPC]
        private void RPCSetParentTransform(string transformName)
        {
            transform.SetParent(GameObject.Find(transformName).transform);
        }
        [PunRPC]
        private void RPCSetSelectionButtons()
        {
            PhotonView PV = GetComponent<PhotonView>();
            if (!PV.IsMine)
            {
                selectButtons.SetActive(false);
            }
        }
        [PunRPC]
        private void RPCShowSelected(int selected)
        {
            Debug.Log($"RPCShowSelected: {selected}");
            animalImage.sprite = sprites[selected];
        }
        [PunRPC]
        private void RPCSetUsernameText(string username)
        {
            Debug.Log($"RPCSetUsernameText: {username}");
            usernameText.SetText(username);
        }
    }
}