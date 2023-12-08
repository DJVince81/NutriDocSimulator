using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class UpdateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI kcalText;
    [SerializeField] private TextMeshProUGUI protText;
    [SerializeField] private TextMeshProUGUI carbText;
    [SerializeField] private TextMeshProUGUI fatText;
    [SerializeField] private IncrementMarmite marmite;
    private static GameObject grabbedObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (grabbedObject == null)
        {
            name.text = "Marmite";
            kcalText.text = "kcal : " + marmite.kcal.ToString();
            protText.text = "prot : " + marmite.prot.ToString();
            carbText.text = "carb : " + marmite.carb.ToString();
            fatText.text = "fat : " + marmite.fat.ToString();
        }
        else
        {
            name.text = grabbedObject.name;
            Macro grabbedObjectMacro = grabbedObject.GetComponent<Macro>();
            kcalText.text = "kcal : " + grabbedObjectMacro.kcal.ToString();
            protText.text = "prot : " + grabbedObjectMacro.prot.ToString();
            carbText.text = "carb : " + grabbedObjectMacro.carb.ToString();
            fatText.text = "fat : " + grabbedObjectMacro.fat.ToString();
        }
    }

    public static void SetGrab(GameObject gameObject = null)
    {
        grabbedObject = gameObject;
    }
}
