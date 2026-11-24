using UnityEngine;
using System.Collections;
public class Bullet : MonoBehaviour
{
   private void OnCollisionEnter(Collision collision) {
        Debug.Log("working collision");
        if (collision.gameObject.CompareTag("Enemy")){
             Debug.Log("Enemy hit");
            EnemyBaseClass flash = collision.gameObject.GetComponent<EnemyBaseClass>();
            flash.TriggerFlash();
            
            // Destroy the bullet upon impact
            Destroy(gameObject);
           
        }
   }





}
