using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zinnia.Extension;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float conveyorSpeed = 12f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.gameObject.TryGetComponent<Rigidbody>(searchAncestors: true);
        if (rb != null ) {
            rb.freezeRotation = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.gameObject.TryGetComponent<Rigidbody>(searchAncestors: true);
        if (rb != null)
        {
            rb.AddForce(transform.forward * conveyorSpeed);
        }
    }
    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.gameObject.TryGetComponent<Rigidbody>(searchAncestors: true);
        if (rb != null)
        {
            rb.freezeRotation = false;
        }
    }
}
