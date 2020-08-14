using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    public GameObject panelCurrentHP;
    private RectTransform lifeBarRectTransform;
    public Text ammoCountText;
    public GameObject deathScreen;

    private float maxHP = 100;
    private int defaultAmmo = 200;

    //[SyncVar]
    //public float currentHP;

    [SyncVar]
    SecureFloat currentHP;

    [SyncVar]
    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        protected set { isDead = value; }
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    CharacterStates characterStates;
    string playerid;

    void Start()
    {
        // Setup(); is called by the PlayerSetup so not requried here.

    }

    void Update()
    {
        // Update UI lifebar based on current health
        // ANTI-HACK SecureFloat implementation for health
        //lifeBarRectTransform.sizeDelta = new Vector2(currentHP, lifeBarRectTransform.sizeDelta.y);
        lifeBarRectTransform.sizeDelta = new Vector2(currentHP.Get(), lifeBarRectTransform.sizeDelta.y);

        // Update UI ammo count text
        ammoCountText.text = characterStates.AmmoCount.ToString();
    }

    public void Setup()
    {
        Collider collider = GetComponent<Collider>();
        playerid = collider.name;
        characterStates = gameObject.GetComponent<CharacterStates>();

        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        currentHP = new SecureFloat(maxHP);
        SetDefaults();

        lifeBarRectTransform = (RectTransform)panelCurrentHP.transform;
        ammoCountText.text = characterStates.AmmoCount.ToString();
    }

    private void Die()
    {
        isDead = true;
        deathScreen.SetActive(true);

        // Disable Components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        characterStates.CmdTriggerDeath(playerid);

        Debug.Log(transform.name + " is dead");

        // Call respawn as coroutine
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        SetDefaults();

        // Disable CharacterController or remote players may not transport correctly.
        // (The CharacterController collides with obstacles and the player will appear stuck).
        transform.gameObject.GetComponent<CharacterController>().enabled = false;

        // Reset player's position to the next spawn point.
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // Re-enable CharacterController and render visible.
        transform.gameObject.GetComponent<CharacterController>().enabled = true;
    }

    public void SetDefaults()
    {
        isDead = false;
        deathScreen.SetActive(false);

        // ANTI-HACK SecureFloat implementation for health
        //currentHP = maxHP;
        currentHP.Set(maxHP);

        characterStates.CmdSetAmmo(playerid, defaultAmmo);

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    /* ********** *************** ********** */
    /* ***** SERVER-TO-CLIENT COMMANDS ***** */
    /* ********** *************** ********** */

    // Called from PlayerController's SERVER COMMAND - 'CmdPlayerHit'
    [ClientRpc]
    public void RpcApplyDamage(float amount)
    {
        // ANTI-HACK SecureFloat implementation for health
        //currentHP -= amount;
        currentHP.SubtractFloat(amount);

        Debug.Log(transform.name + " health: " + currentHP.ToString());

        if (currentHP.Get() <= 0.0f)
        {
            Die();
        }
    }

    // Not currently used, but may be required for removal of player.
    [ClientRpc]
    void RpcSelfTerminate()
    {
        Destroy(gameObject);
    }
}