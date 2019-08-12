using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour {
    
    //Enemies Array to hold all enemies
    public Enemy[] enemies;
    //Enum for state of enemy 
    public enum TriggerState
    {
        Patrol,
        Hide,
        Attack,
        Stop
    }
    //Variable for each state
    public TriggerState enter;
    public TriggerState exit;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //On Collider trigger, check if it is the player, change state accordingly
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (enter)
            {
                case TriggerState.Patrol:
                    ChangeEnemyStates(0);
                    break;
                case TriggerState.Hide:
                    ChangeEnemyStates(1);
                    break;
                case TriggerState.Attack:
                    ChangeEnemyStates(2);
                    break;
                case TriggerState.Stop:
                    ChangeEnemyStates(3);
                    break;
            }

        }

    }

    //On Collider exit, check if it is the player, change state accordingly
    void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player")
        {
            switch (exit)
            {
                case TriggerState.Patrol:
                    ChangeEnemyStates(0);
                    break;
                case TriggerState.Hide:
                    ChangeEnemyStates(1);
                    break;
                case TriggerState.Attack:
                    ChangeEnemyStates(2);
                    break;
            }
        }
    }


    private void ChangeEnemyStates(int state)
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.newState = state;
        }
    }

}
