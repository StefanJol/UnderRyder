using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private EnemyBaseClass enemyBase;
    public bool isHeadshot = false;

    void Start()
    {
        // Automatically find the EnemyBaseClass on the parent object
        enemyBase = GetComponentInParent<EnemyBaseClass>();
    }

    // from bullet to this to the base class
    public void OnHitByBullet(int damageAmount)
    {
            if (isHeadshot){
                Debug.Log("Headshot");
            }else{
                Debug.Log("Bodyshot");
            }
            enemyBase.TakeDamage(damageAmount, isHeadshot);
        
    }
}