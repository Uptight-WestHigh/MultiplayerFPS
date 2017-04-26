using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    private TextMesh nameTag;

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject[] playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            // Disable player graphics for local player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            // Configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("No PlayerUI component on PlayerUI prefab.");
            ui.SetController(GetComponent<PlayerController>());

            GetComponent<Player>().SetupPlayer();

            // Lock the cursor
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

        }
    }

    void SetLayerRecursively(GameObject[] obj, int newLayer)
    {
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i].layer = newLayer;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        nameTag.text = "Player " + _netID;

        GameManager.RegisterPlayer(_netID, _player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    void OnDisable()
    {
        Destroy(playerUIInstance);

        if (isLocalPlayer)
            GameManager.instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
