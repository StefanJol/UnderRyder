using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class WeaponFire : MonoBehaviour
{
    public Animator weaponAnimator;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f; // for melee weapons with a delay/windup il just change this around
    public float bulletLifetime = 3f;
    public float FireDelay = 0.30f;
    public float FireDelayAfter = 0.35f;
    public float bulletSize; // for Melee Wider angle of sweep for sword for later

    void Update()
    {
        bool isPlaying = weaponAnimator.IsInTransition(0) || weaponAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < FireDelayAfter;
        if (Mouse.current.leftButton.wasPressedThisFrame && isPlaying == false)
        {   
            // Animation Play
                weaponAnimator.SetTrigger("Fire");
                StartCoroutine(FireWithDelay(FireDelay));
                //Debug.Log("Firing Works");
        }
    }

    private IEnumerator FireWithDelay(float delay)
        {
        yield return new WaitForSeconds(delay);
        FireWeapon();
    }

    void FireWeapon(){
        
        //instantiating the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position , Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        // Destroy bullet after X time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifetime));

    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float timeUntilDelete){
        yield return new WaitForSeconds(timeUntilDelete);
        Destroy(bullet);

    }

}