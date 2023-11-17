using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyTankNew : MonoBehaviour
{
    public float speed;
    private Rigidbody rbody;
    private GameObject Enemy;
    public GameObject AI;    
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        Enemy = GetComponent<GameObject>();      


    }

    void Update()
    {      
        transform.localPosition += transform.forward * Time.deltaTime * speed;

        if (this.transform.localPosition.z < -37)
        {
            Destroy(gameObject);
        }
    }
    public void Hit()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Tank")
        {
            Destroy(gameObject);
        }

    }
}
