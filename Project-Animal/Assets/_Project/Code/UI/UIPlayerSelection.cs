using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

namespace BrilathTTV
{
    public class UIPlayerSelection : MonoBehaviour
    {
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private Image animalImage;
        [SerializeField] private GameSession gameSession;
        [SerializeField] private GameObject selectButtons;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private int currentSelection;
        [SerializeField] private Player player;
        [SerializeField] private PhotonView selectionPhotonView;

        private void Awake()
        {
            selectionPhotonView = GetComponent<PhotonView>();
        }

        public void Initialize(Player player, string parentTransform)
        {
            this.player = player;

            // if (!selectionPhotonView.IsMine)
            // {
            //     selectButtons.SetActive(false);
            // }
            selectionPhotonView.RPC("RPCSetSelectionButtons", RpcTarget.AllBuffered);
            selectionPhotonView.RPC("RPCSetParentTransform", RpcTarget.AllBuffered, parentTransform);
            SetupPlayerSelection();
        }

        private void SetupPlayerSelection()
        {
            currentSelection = gameSession.SelectedCharacter;

            ShowSelected();
            selectionPhotonView.RPC("RPCSetUsernameText", RpcTarget.AllBuffered, gameSession.Username);
        }

        private void ShowSelected()
        {
            selectionPhotonView.RPC("RPCShowSelected", RpcTarget.AllBuffered, currentSelection);
        }

        public void PreviousSelection()
        {
            currentSelection--;
            if (currentSelection < 0)
            {
                currentSelection = sprites.Length - 1;
            }
            ShowSelected();
        }
        public void NextSelection()
        {
            currentSelection++;
            if (currentSelection > sprites.Length - 1)
            {
                currentSelection = 0;
            }
            ShowSelected();
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