using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitGame : NetworkBehaviour
{
    [SerializeField] private Button QuitGameButton;
    
    // Start is called before the first frame update
    void Start()
    {
        QuitGameButton.onClick.AddListener(() =>
        {
            RequestServerToQuitGameServerRpc();
        });
    }
    
    [ServerRpc(RequireOwnership = false)]
    void RequestServerToQuitGameServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("LoadScene", LoadSceneMode.Single);
    }

}
