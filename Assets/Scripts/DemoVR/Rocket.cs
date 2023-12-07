using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    [SerializeField] private Rigidbody myRigidBody;
    [SerializeField] private Vector3 forceVector;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            myRigidBody.AddForce(forceVector);
        }
    }

    public void Launch() {
        myRigidBody.AddForce(forceVector);
    }
}
