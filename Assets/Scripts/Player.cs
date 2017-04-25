using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class Player : NetworkBehaviour {

    [SerializeField]
    GameObject playerDeadPrefab;
    private GameObject playerDeadInstance;

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    public float GetHealthAmount()
    {
        return currentHealth;
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    void Update()
    {
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(99999);
    //    }
         if (transform.position.y< -5f)
         {
            if (!isDead)
            {
                Die();
                // Added by me
                if (playerDeadInstance != null)
                    Destroy(playerDeadInstance);
                playerDeadInstance = Instantiate(playerDeadPrefab);
                playerDeadInstance.GetComponent<Text>().text = "\n\n" + transform.name + " fell off the map!";
                Destroy(playerDeadInstance, 2f);
            }
         }
    }

[ClientRpc]
    public void RpcTakeDamage(int _amount, string _shooter)
    {
        if (isDead)
            return;

        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if (currentHealth <= 0)
        {
            Die();
            // Added by me
            if (playerDeadInstance != null)
                Destroy(playerDeadInstance);
            playerDeadInstance = Instantiate(playerDeadPrefab);
            playerDeadInstance.gameObject.GetComponent<Text>().text = "\n\n" + transform.name + " was shot and killed by " + _shooter + "!";
            Destroy(playerDeadInstance, 2f);
        }
    }

    private void Die()
    {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        Debug.Log(transform.name + " is DEAD!");

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn ()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Destroy(playerDeadInstance);
        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(transform.name + " respawned.");
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;
    }

}
