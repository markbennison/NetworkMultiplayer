using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
public class HPManager : NetworkBehaviour
{
    public GameObject panelCurrentHP;
    private RectTransform lifeBarRectTransform;
    public float currentHP;
    public float maxHP;
    // Start is called before the first frame update
    void Start()
    {
        maxHP = 100;
        currentHP = 100;
        //if (lifeBarRectTransform != null)
        //{
            lifeBarRectTransform = (RectTransform)panelCurrentHP.transform;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //if (lifeBarRectTransform != null)
        //{
            lifeBarRectTransform.sizeDelta = new Vector2(currentHP, lifeBarRectTransform.sizeDelta.y);
        //} 
    }
}
