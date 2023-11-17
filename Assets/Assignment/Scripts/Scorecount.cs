using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scorecount : MonoBehaviour
{
    [SerializeField]private TMP_Text score;   
    
    private TankAgent tankAgent;
    // Start is called before the first frame update
    void Start()
    {
        Transform parentTransform = transform.parent;
        tankAgent = parentTransform.GetComponentInChildren<TankAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = tankAgent.Getscore().ToString();
    }
}
