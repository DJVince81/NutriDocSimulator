using UnityEngine;

public class IncrementMarmite : MonoBehaviour
{
    public int kcal = 0;
    public int prot = 0;
    public int carb = 0;
    public int fat = 0;

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
        //Debug.Log(kcal + " " + prot + " " + carb + " " + fat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
