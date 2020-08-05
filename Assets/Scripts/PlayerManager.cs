using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    public GameObject panelCurrentHP;
    private RectTransform lifeBarRectTransform;
    public Text ammoCountText;
    public GameObject deathScreen;

    [SyncVar]
    public float currentHP;
    private float maxHP = 100;
    private int defaultAmmo = 200;

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

    // Update is called once per frame
    void Update()
    {
        // Update lifebar based on current health
        lifeBarRectTransform.sizeDelta = new Vector2(currentHP, lifeBarRectTransform.sizeDelta.y);
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

        SetDefaults();

        lifeBarRectTransform = (RectTransform)panelCurrentHP.transform;
        ammoCountText.text = characterStates.AmmoCount.ToString();
        //ammoCountText.text = characterStates.CmdGetAmmo(playerid).ToString();
    }



    private void Die()
    {
        isDead = true;
        deathScreen.SetActive(true);

        //disable components
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

        //call respawn
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        SetDefaults();

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

    }

    public void SetDefaults()
    {
        isDead = false;
        deathScreen.SetActive(false);

        currentHP = maxHP;
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

    /* ********** SERVER COMMANDS********** */


    /* ********** CLIENT COMMANDS ********** */
    [ClientRpc]
    public void RpcApplyDamage(float amount)
    {
        currentHP -= amount;

        Debug.Log(transform.name + " health: " + currentHP);

        if (currentHP <= 0.0)
        {
            Die();
        }
    }


    [ClientRpc]
    void RpcSelfTerminate()
    {
        Destroy(gameObject);
    }

    /* ********** ********** */


}