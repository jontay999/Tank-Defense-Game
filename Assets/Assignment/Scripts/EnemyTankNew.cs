using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankNew : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
       
        Vector3 moveVect = transform.forward * speed * Time.deltaTime;
        rbody.MovePosition(rbody.position + moveVect);

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
