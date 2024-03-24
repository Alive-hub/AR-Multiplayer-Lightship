using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShowHideBlitImageUI : NetworkBehaviour
{
        [SerializeField] private GameObject BlitImageUI;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private GameObject retakePicture;
        
        private void Start()
        {
            BlitImageUI.SetActive(false);
            retakePicture.SetActive(false);
            StartGameAR.OnStartSharedSpace += StartNetworkingManagerSharedARImageOnOnSharedSpaceStarted;
            StartGameAR.OnStartGame += StartNetworkingManagerSharedARImageOnOnStartGame;
        }

        public override void OnNetworkDespawn()
        {
            StartGameAR.OnStartSharedSpace -= StartNetworkingManagerSharedARImageOnOnSharedSpaceStarted;
            StartGameAR.OnStartGame -= StartNetworkingManagerSharedARImageOnOnStartGame;
        }

        public override void OnDestroy()
        {
            StartGameAR.OnStartSharedSpace -= StartNetworkingManagerSharedARImageOnOnSharedSpaceStarted;
            StartGameAR.OnStartGame -= StartNetworkingManagerSharedARImageOnOnStartGame;
        }

        private void StartNetworkingManagerSharedARImageOnOnStartGame()
        {
            BlitImageUI.SetActive(false);
            retakePicture.SetActive(false);

        }

        private void StartNetworkingManagerSharedARImageOnOnSharedSpaceStarted()
        {
            if (_renderTexture != null)
            {
                _rectTransform.sizeDelta = new Vector2(_renderTexture.width / 3.25f, _renderTexture.height /3.25f); 
            }
            else
            {
                Debug.LogError("RenderTexture is not assigned or is null.");
            }
            
            BlitImageUI.SetActive(true);
            retakePicture.SetActive(true);

        }
        
        private void OnDisable()
        {
            StartGameAR.OnStartSharedSpace -= StartNetworkingManagerSharedARImageOnOnSharedSpaceStarted;
        }
}
