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
        Macro macro = other.gameObject.GetComponent<Macro>();
        if (macro != null)
        {
            kcal += macro.kcal;
            prot += macro.prot;
            carb += macro.carb;
            fat += macro.fat;
            macro.Reset();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
