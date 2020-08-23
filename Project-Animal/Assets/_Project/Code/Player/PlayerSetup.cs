using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace BrilathTTV
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] PlayerController controller;
        [SerializeField] PhotonView playerPhotonView;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] sprites;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            playerPhotonView = GetComponent<PhotonView>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        // Start is called before the first frame update
        void Start()
        {
            if (!playerPhotonView.IsMine)
            {
                controller.enabled = false;
            }
            else
            {

                playerPhotonView.RPC("SetPlayerModel", RpcTarget.AllBuffered, GetCharacterSelection());
            }
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
        [PunRPC]
        private void SetPlayerModel(int modelId)
        {
            spriteRenderer.sprite = sprites[modelId];
        }
    }
}