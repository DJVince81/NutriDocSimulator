using System;
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
            if (rb.name == "Interactions.Interactable_Marmite")
            {

                int kcalResult = rb.GetComponentInChildren<IncrementMarmite>().kcal;
                int protResult = rb.GetComponentInChildren<IncrementMarmite>().prot;
                int carbResult = rb.GetComponentInChildren<IncrementMarmite>().carb;
                int fatResult = rb.GetComponentInChildren<IncrementMarmite>().fat;

                float kcalNeeded = 2000;
                float protNeeded = 100;
                float carbNeeded = 300;
                float fatNeeded = 50;

                float diff = (Math.Abs(kcalNeeded - kcalResult)/kcalNeeded + Math.Abs(protNeeded - protResult)/protNeeded + Math.Abs(carbNeeded - carbResult) / carbNeeded + Math.Abs(fatNeeded - fatResult) / fatNeeded) / 4;
                int note;
                if (diff <= 0.10) {
                    note = 3;
                } else if (diff <= 0.25)
                {
                    note = 2;
                } else if (diff <= 0.5)
                {
                    note = 1;
                } else
                {
                    note = 0;
                }
                Debug.Log(note);
            }
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
