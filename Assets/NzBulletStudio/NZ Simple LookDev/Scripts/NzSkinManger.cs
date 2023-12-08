using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NzBulletLookDev
{

    public class NzSkinManger : MonoBehaviour
    {
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        private int currentModel;
        public int storedModel;
    
        public GameObject SkinController;
        public GameObject ScopeController;
        public GameObject ScopeHolder;
    
    
    
    
        private void Awake()
        {
            SelectSkin(0);
        }
    
        private void OnEnable() 
        {
            SkinController.SetActive(true);  
            ScopeController.SetActive(true); 
            ScopeHolder.SetActive(true);
           
        }
    
        private void OnDisable() 
        {
            //IconContainer.SetActive(false); 
    
            if (SkinController != null)
            {     
                SkinController.SetActive(false); 
            }  
    
            if (ScopeController != null)
            {     
                ScopeController.SetActive(false); 
            } 
    
            if (ScopeHolder != null)
            {     
                ScopeHolder.SetActive(false); 
            } 
    
        }
    
    
    
        private void SelectSkin(int _index)
            {
                previousButton.interactable = (_index != 0);
                nextButton.interactable = (_index != transform.childCount-1);
        
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(i == _index);
                }
            }
        
            public void ChangeSkin(int _change)
            {
                currentModel += _change;
                SelectSkin(currentModel);
                //storedModel += _change;
            }
    
    }
}
