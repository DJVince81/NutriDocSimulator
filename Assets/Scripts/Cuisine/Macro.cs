using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Macro : MonoBehaviour
{
    public int kcal;
    public int prot;
    public int carb;
    public int fat;

    private Vector3 initPos;
    private Quaternion initRot;
    
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        initRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Reset()
    {
        transform.position = initPos;
        transform.rotation = initRot;
    }

    public void sendUpdate()
    {
        UpdateUI.SetGrab(gameObject);
    }

    public void resetUpdate()
    {
        UpdateUI.SetGrab();
    }
}
