using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public Transform playerTransform;
    public float maxChaseDistance = 4f;
    
    public enum State
    {
        Patrol,
        Chase
    }

    [HideInInspector] public State currentState;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(playerTransform.position, transform.position) <= maxChaseDistance)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Patrol;
        }


    }
}
