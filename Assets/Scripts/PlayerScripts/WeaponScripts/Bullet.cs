using UnityEngine;
using System.Collections;





public class Bullet : MonoBehaviour
{
     public int bulletDamage = 25;


   void OnCollisionEnter(Collision collision)
     {
          //Debug.Log("Bullet collided with: " + collision.gameObject.name);
    EnemyHitbox hitbox = collision.gameObject.GetComponent<EnemyHitbox>();
    if (collision.gameObject.CompareTag("Enemy"))
    {
        hitbox.OnHitByBullet(bulletDamage);
        
        
    }
    
          
          
    

    Destroy(gameObject); // Destroy the bullet
     }





}
