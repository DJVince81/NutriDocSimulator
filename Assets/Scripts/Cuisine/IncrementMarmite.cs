using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementMarmite : MonoBehaviour
{
    private int kcal = 0;
    private int prot = 0;
    private int carb = 0;
    private int fat = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        kcal += other.gameObject.GetComponent<Macro>().kcal;
        prot += other.gameObject.GetComponent<Macro>().prot;
        carb += other.gameObject.GetComponent<Macro>().carb;
        fat += other.gameObject.GetComponent<Macro>().fat;
        Destroy(other.gameObject);
        Debug.Log(kcal);
        Debug.Log(prot);
        Debug.Log(carb);
        Debug.Log(fat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
