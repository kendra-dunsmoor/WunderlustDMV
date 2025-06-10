   using UnityEngine;
   using System.Collections;

   public class UIShake : MonoBehaviour
   {
       public float shakeDuration = 0.2f;
       public float shakeAmount = 5f;

       private Vector3 originalPosition;

       public void StartShake()
       {
           originalPosition = transform.localPosition;
           StartCoroutine(Shake());
       }

       private IEnumerator Shake()
       {
           float elapsed = 0f;

           while (elapsed < shakeDuration)
           {
               float x = originalPosition.x + Random.Range(-1f, 1f) * shakeAmount;
               float y = originalPosition.y + Random.Range(-1f, 1f) * shakeAmount;

               transform.localPosition = new Vector3(x, y, originalPosition.z);

               elapsed += Time.deltaTime;
               yield return null;
           }

           transform.localPosition = originalPosition;
       }
   }