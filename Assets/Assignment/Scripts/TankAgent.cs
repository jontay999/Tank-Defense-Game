using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Diagnostics;

public class TankAgent : Agent
{
    Rigidbody rBody;
    float moveSpeed = 20f;
    float fireRange = 30f;
    bool fellOffPlatform = false;
    int enemiesHit = 0;
    int alliesHit = 0;
    int enemiesPassed = 0;
    float maxScore = 20f;
    public int totalScore = 0;
    public RayPerceptionSensorComponent3D rayPerceptionSensor;
    static int episodeCount = 0;
    static int goodEpisodes = 0;
    static int badEpisodes = 0;
    Stopwatch stopwatch;
    
    HashSet<int> trackedGameIds = new HashSet<int>();
    

    void Start()
    {
        stopwatch = new Stopwatch();
        rBody = GetComponent<Rigidbody>();
        rBody.transform.rotation = Quaternion.identity;
        rBody.constraints = RigidbodyConstraints.FreezeRotation;
        rayPerceptionSensor = GetComponent<RayPerceptionSensorComponent3D>();
    }


    // called when agent receives an action, common to assign reward here
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        AddReward(-0.000005f);
        float totalReward = GetCumulativeReward();
        if(totalScore >= maxScore) {
            goodEpisodes += 1;
            EndEpisode();
        }

        if(this.fellOffPlatform){
            AddReward(-1f);
            badEpisodes += 1;
            EndEpisode();
            return;
        }
        
        // [moveLeft, moveRight, don't move]
        int movementAction = actionBuffers.DiscreteActions[0];
        float moveX = 0f;
        if(movementAction == 0) moveX = -1f;
        if(movementAction == 1) moveX = 1f;
        this.transform.localPosition += new Vector3(moveX, 0, 0) * Time.deltaTime * this.moveSpeed;

        // [fire, don't fire]
        int firingAction = actionBuffers.DiscreteActions[1];
        if (firingAction == 0) Fire();
    }
    
    // called at beginning of simulation
    public override void OnEpisodeBegin() {
        // reset the tank back to initial state
        episodeCount+= 1;
        double good_percentage = ((double)goodEpisodes / episodeCount) * 100;
        UnityEngine.Debug.Log("Episode: " + episodeCount + ", Good: " + goodEpisodes + ", Bad: " + badEpisodes + ", Percentage: " + good_percentage + "%" );
        this.transform.localPosition = new Vector3(0,0,-35);
        this.enemiesHit = 0;
        this.alliesHit = 0;
        this.enemiesPassed = 0;
        this.totalScore = 0;
        this.fellOffPlatform = false;
        this.trackedGameIds.Clear();
        stopwatch.Restart();
    }

    

    // called every step that agent requests a decision
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(this.transform.localPosition);

        var castInput = rayPerceptionSensor.GetRayPerceptionInput();
        var castOutput = RayPerceptionSensor.Perceive(castInput);
        
        var output1 = castOutput.RayOutputs[castOutput.RayOutputs.Length-2];
        var output2 = castOutput.RayOutputs[castOutput.RayOutputs.Length-1];
        
        // check if enemyAI crossed at -90deg
        if(output1.HitTagIndex == 0){
            int objectId = output1.HitGameObject.GetInstanceID();
            if(!trackedGameIds.Contains(objectId)){
                AddReward(-0.5f);
                this.totalScore -= 1;
                enemiesPassed += 1;
                trackedGameIds.Add(objectId);
            }
        }
        
        // check if something crossed at 90deg
        if(output2.HitTagIndex == 0){
            int objectId = output2.HitGameObject.GetInstanceID();
            if(!trackedGameIds.Contains(objectId)){
                AddReward(-0.5f);
                this.totalScore -= 1;
                enemiesPassed += 1;
                trackedGameIds.Add(objectId);
            }
        }

        // if the tank falls off the platform
        if (Mathf.Abs(this.transform.localPosition.x) > 35f){
            this.fellOffPlatform = true;
        }
    }

    public int Getscore(){
        return this.totalScore;
    }

    private void OnCollisionEnter(Collision collision){
        string name = collision.gameObject.name.ToString();
        if(name == "FriendlyTankNew(Clone)"){
            AddReward(1f);
            this.totalScore += 2;
        }

        if(name == "EnemyTankNew(Clone)"){
            AddReward(-1f);
            badEpisodes += 1;
            EndEpisode();
        }
    }

    
    private void Fire()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;

        UnityEngine.Debug.DrawRay(this.transform.position, this.transform.forward * fireRange, Color.red, 0.5f);
        if (Physics.Raycast(ray, out hit, fireRange))
        {
            if (hit.collider.CompareTag("EnemyAi"))
            {
                EnemyTankNew enemy = hit.collider.GetComponent<EnemyTankNew>();
                if(enemy != null){
                    AddReward(1f);
                    this.totalScore += 2;
                    enemiesHit += 1;
                    enemy.Hit();
                }
            }
            else if(hit.collider.CompareTag("Friendly"))
            {
                FriendlyTankNew ally = hit.collider.GetComponent<FriendlyTankNew>();
                if(ally != null){
                    AddReward(-0.5f);
                    this.totalScore -= 1;
                    alliesHit += 1;
                    ally.Hit();
                }
            }
        }
    }
}
