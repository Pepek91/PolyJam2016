﻿using UnityEngine;
using System.Collections;

public class WalkerController : MonoBehaviour {
    
    public Transform target;
    public Transform[] players;
    public float newTargetPosMultiplier = 5;
    //public float switchPosTriggerRadius = 30;
    //public float increaseSpeedThreshold = 5;
    public float switchSensivity = 20;
    //public float dudeAcceleration = 10;
    //public float dudedeceleration = 2;
    public GameObject escort;
    public GameObject escortInstance;
    public float coulombsConstant;
    public Vector3 electricField;
    public float forceToSpeedIncDiv;

    private float minimumDistance = 2;
    private Vector3 targetVector;
    private float startSpeed;
    private NavMeshAgent navMeshAgent;
    private bool isCaptured;

    // Use this for initialization
    void Start () {
        navMeshAgent = GetComponent<NavMeshAgent> ();
        targetVector = new Vector3(1, 0, 0);

        startSpeed = navMeshAgent.speed;

        players[0] = GameObject.FindGameObjectWithTag("Player One").GetComponent<Transform>();
        players[1] = GameObject.FindGameObjectWithTag("Player Two").GetComponent<Transform>();
        players[2] = GameObject.FindGameObjectWithTag("Altar").GetComponent<Transform>();

        isCaptured = false;
    }
    
    // Update is called once per frame
    void Update () {
        Vector3 newTargetPosition = new Vector3();
        //updating dude NavMeshAgent component
        if (!isCaptured) {
            Vector3 force = new Vector3 (0, 0, 0);

            foreach (Transform playerTransform in players) {
                
                Vector3 dude_player_position_diff = transform.position - playerTransform.position;
                dude_player_position_diff.y = 0;
 
                // player to dude distance
                float distance = Mathf.Sqrt(Mathf.Pow(dude_player_position_diff.x, 2) + Mathf.Pow(dude_player_position_diff.z, 2)) - minimumDistance;
                if (distance < 0.1) {
                    distance = 0.1f;
                }

                force += dude_player_position_diff.normalized *
                coulombsConstant
                * gameObject.GetComponent<Electrostatic>().getMagOfCharge()
                * playerTransform.gameObject.GetComponent<Electrostatic>().getMagOfCharge()
                / Mathf.Pow(distance, 2); 
            }
            force += gameObject.GetComponent<Electrostatic>().getMagOfCharge() * electricField;

            targetVector += force * Time.deltaTime * switchSensivity;
            targetVector.Normalize();
            newTargetPosition = transform.position + newTargetPosMultiplier * targetVector;
            target.position = newTargetPosition;

            float newSpeed = force.magnitude / forceToSpeedIncDiv + startSpeed;

            navMeshAgent.speed = newSpeed;
        }
        else
        {
            newTargetPosition = target.position;

            ActivateMinigameIfAtPostion();
        }
        // if active update
        if (navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.SetDestination(newTargetPosition);
        }
    }

    private void ActivateMinigameIfAtPostion()
    {
        if (!navMeshAgent.pathPending 
            && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance
            && (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f))
        {
            // Run mini game and at the end kill dude.

            KillDudeAndEscort();
        }
    }

    private void KillDudeAndEscort()
    {
        Destroy(this.gameObject);
        Destroy(escortInstance);      
    }

    public void CaptureAndSetNewTarget(Transform newTarget)
    {
        target = newTarget;
        isCaptured = true;
        navMeshAgent.speed /= 3;
        escortInstance = Instantiate(escort);
        Transform escorts = escortInstance.transform;
        foreach(Transform escortDude in escorts)
        {
            if (escortDude.tag == "Escort One" || escortDude.tag == "Escort Two")
            {
                EscortNavScript escortScript = escortDude.gameObject.GetComponent<EscortNavScript>();
                escortScript.setCapturedDude(gameObject);
            }
        }
    }

    public bool IsCaptured()
    {
        return isCaptured;
    }

}
