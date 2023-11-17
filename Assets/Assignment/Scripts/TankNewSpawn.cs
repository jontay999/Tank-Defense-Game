using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankNewSpawn : MonoBehaviour
{
    public GameObject EnemyTank;
    public float delay;
    public GameObject FriendlyTank;
    public float delay2;
    public Transform Ai;
    private float TimeStart;
    private float TimeStart2;
    private GameObject Enemy;
    private GameObject Friendly;   
    void Start()
    {
        TimeStart = 0f;
    }

    void Update()
    {
        TimeStart += Time.deltaTime;
        TimeStart2+= Time.deltaTime;
        if (TimeStart > delay)
        {
            SpawnEnemy(delay, true);
            TimeStart -= delay;
            
        }
        if (TimeStart2 > delay2)
        {
            SpawnEnemy(delay, false);
            TimeStart2 -= delay2;
            
        }
       

    }
    public void SpawnEnemy(float delay, bool isEnemy)
    {
        Vector3 SpawnPosition;

        SpawnPosition = new Vector3(Random.Range(-30, 30) + this.transform.position.x, this.transform.position.y, 30 + this.transform.position.z);
        

        if (isEnemy)
        {
            Enemy = Instantiate(EnemyTank, SpawnPosition, Quaternion.AngleAxis(180, Vector3.up), this.transform);
        }
        else
        {
            Friendly = Instantiate(FriendlyTank, SpawnPosition, Quaternion.AngleAxis(180, Vector3.up), this.transform);
           
        }

    }

    public void DestroyObject()
    {
        for (var i = this.transform.childCount - 1; i >= 0; i--)
        {
            // only destroy tagged object
            if (this.transform.GetChild(i).gameObject.tag == "EnemyAi")
                Destroy(this.transform.GetChild(i).gameObject);
            if (this.transform.GetChild(i).gameObject.tag == "Friendly")
                Destroy(this.transform.GetChild(i).gameObject);
        }
        TimeStart = 0;
    }

}
