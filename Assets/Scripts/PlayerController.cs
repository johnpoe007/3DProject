using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed;
    
    private Rigidbody rig;

    private void Awake()
    {
        //get the rigidbody component
        rig = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
    }

    void Move()
    {
        // getting our inputs
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");
        
        Vector3 dir = new Vector3(xInput, 0, zInput) * moveSpeed;
        dir.y = rig.velocity.y;
        
        rig.velocity = dir;
        
    }
}
