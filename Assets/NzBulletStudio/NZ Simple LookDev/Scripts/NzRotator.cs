using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace NzBulletLookDev
{
    public class NzRotator : MonoBehaviour
    {
        

        public AudioClip Explode_Sound;
        public AudioClip Recall_Sound;
        public AudioSource Audio;
    
        public float ExplodeSpeed;
        public bool IsExplode = false;
        public bool IsAuto = false;

        Quaternion pointA;
        public Quaternion pointB;
        public KeyCode ExplodeButton = KeyCode.Space;

        public UnityEvent StarAction;
        public UnityEvent EndAction;

    

        private void Start() 
        {
            pointA = transform.rotation;

            if(IsAuto)
            {
                StartCoroutine(MoveObject(transform, pointA, pointB, ExplodeSpeed));
            }


            if (StarAction == null)
            StarAction = new UnityEvent();

            
            
        }
        void Update()
         {
            
            if (Input.GetKeyDown(ExplodeButton) && !IsExplode)
            {
                Audio.PlayOneShot(Explode_Sound); 
                StartCoroutine(MoveObject(transform, pointA, pointB, ExplodeSpeed));
                IsExplode = true;
                if (StarAction != null)
                {
                    StarAction.Invoke();
                }
            }
            else if (Input.GetKeyDown(ExplodeButton) && IsExplode)
            {
                Audio.PlayOneShot(Recall_Sound); 
                StartCoroutine(MoveObject(transform, pointB, pointA, ExplodeSpeed));
                IsExplode = false;
                if (EndAction != null)
                {
                    EndAction.Invoke();
                }
            

            }


            
         }
      
         IEnumerator MoveObject(Transform thisTransform, Quaternion startPos, Quaternion endPos, float time)
         {
             var i= 0.0f;
             var rate= 1.0f/time;
             while (i < 1.0f) {
                 i += Time.deltaTime * rate;
                 thisTransform.rotation = Quaternion.Lerp(startPos, endPos, i);
                 yield return null; 
             }
         }
     
    }
}
