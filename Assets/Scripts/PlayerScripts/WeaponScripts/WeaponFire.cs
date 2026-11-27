using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WeaponFire : MonoBehaviour
{
    public GameObject Fists;
    public GameObject Revolver;
    public Animator fistsAnimator;
    public Animator revolverAnimator;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    float bulletVelocity = 30f; 
    float bulletLifetime = 3f;
    float FireDelay = 0.30f;
    float FireDelayAfter = 0.35f;
    float bulletSize; 
    private bool isPlaying = false;
    public GameObject Hand;
    private Vector3 originalHandPos;
    private bool isSwitching = false;
    public float switchDropAmount = 0.4f;
    public float reloadDropAmount = 0.2f;
    public float switchDuration = 0.15f;  
    public enum SelectedWeapon { Fists, Revolver }
    public SelectedWeapon currentWeapon = SelectedWeapon.Fists;
    public int RevolverBullets = 6;
    public int RevolverBulletsStored = 48;
    float revolverReloadDuration = 0.30f;
    private bool isReloading = false;
   void Start()
   {
        ApplyWeaponStats(currentWeapon);
        originalHandPos = Hand.transform.localPosition;
      
   }
 
    void Update()
    {   
        // Check animation playing based on current weapon
        if (currentWeapon == SelectedWeapon.Fists) {
           bool isPunchingState = fistsAnimator.GetCurrentAnimatorStateInfo(0).IsName("Punching");
           isPlaying = fistsAnimator.IsInTransition(0) || (isPunchingState && fistsAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < FireDelayAfter);
        }
        else if (currentWeapon == SelectedWeapon.Revolver) {
           bool isRevolverState = revolverAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shoot");
           isPlaying = revolverAnimator.IsInTransition(0) || (isRevolverState && revolverAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < FireDelayAfter);
        }

        // Weapon Switching 
        if (!isSwitching && !isReloading)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame && currentWeapon != SelectedWeapon.Fists)
            {
                StartCoroutine(SwitchWeaponRoutine(SelectedWeapon.Fists));
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame && currentWeapon != SelectedWeapon.Revolver)
            {
                StartCoroutine(SwitchWeaponRoutine(SelectedWeapon.Revolver));
            }
        }

        // Firing
        if (Mouse.current.leftButton.wasPressedThisFrame && !isPlaying && !isSwitching)
        {   
            if (currentWeapon == SelectedWeapon.Fists)
            {
                fistsAnimator.SetTrigger("Fire");
                StartCoroutine(FireWithDelay(FireDelay));
            }
            if (currentWeapon == SelectedWeapon.Revolver && RevolverBullets > 0)
            {
                revolverAnimator.SetTrigger("Fire");
                StartCoroutine(FireWithDelay(FireDelay));
            }else if (RevolverBulletsStored > 0)
            {
                StartCoroutine(ReloadRevolver());
            }
        }
        // Reloading
        if (Keyboard.current.rKey.wasPressedThisFrame && !isSwitching && !isReloading && !isPlaying)
        {
            if (currentWeapon == SelectedWeapon.Revolver)
            {
                StartCoroutine(ReloadRevolver());
            }
        }
    }

    private void ApplyWeaponStats(SelectedWeapon weapon)
    {
        if (currentWeapon == SelectedWeapon.Fists)
        {
            Fists.SetActive(true);
            Revolver.SetActive(false);
            fistsAnimator.SetBool("Fists", true);
            revolverAnimator.SetBool("Revolver", false);
            
            bulletVelocity = 4f; 
            bulletLifetime = 0.22f;
            FireDelay = 0.30f;
            FireDelayAfter = 0.35f;
        }
        else if (currentWeapon == SelectedWeapon.Revolver)
        {
            Fists.SetActive(false);
            Revolver.SetActive(true);
            fistsAnimator.SetBool("Fists", false);
            revolverAnimator.SetBool("Revolver", true);

            bulletVelocity = 20f; 
            bulletLifetime = 3f;
            FireDelay = 0.10f;
            FireDelayAfter = 0.75f;
        }
    }
    private IEnumerator ReloadRevolver(){
        if (RevolverBullets == 6) {
            yield break;
        }
        isReloading = true;
        Vector3 targetPos = originalHandPos - new Vector3(0, reloadDropAmount, 0);
        float elapsed = 0f;
        int bulletsToReload = 6 - RevolverBullets;
         while (elapsed < revolverReloadDuration * bulletsToReload)
        {
            

        // Move Hand Down
        

            Hand.transform.localPosition = Vector3.Lerp(originalHandPos, targetPos, elapsed / revolverReloadDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Hand.transform.localPosition = targetPos;
        //
        while (RevolverBullets < 6)
        {
            RevolverBulletsStored--;
            RevolverBullets++;
          //  Debug.Log(RevolverBullets);
        }
        //

         // Move Hand Back Up
        elapsed = 0f;
        while (elapsed < revolverReloadDuration * bulletsToReload)
        {
            Hand.transform.localPosition = Vector3.Lerp(targetPos, originalHandPos, elapsed / revolverReloadDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Hand.transform.localPosition = originalHandPos;

        isReloading= false;
    }

    private IEnumerator FireWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        FireWeapon();
        if (currentWeapon == SelectedWeapon.Revolver)
        {
            RevolverBullets--;
        }
        
    }

    void FireWeapon()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        // Get the Bullet script component
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        
            // Dictate damage based on what weapon is currently active
            if (currentWeapon == SelectedWeapon.Fists)
            {
                bulletScript.bulletDamage = 10; // Punch damage
            }
            else if (currentWeapon == SelectedWeapon.Revolver)
            {
                bulletScript.bulletDamage = 34; // Revolver shot damage
            }
        

        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifetime));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float timeUntilDelete)
    {
        yield return new WaitForSeconds(timeUntilDelete);
        Destroy(bullet);
    }

    private IEnumerator SwitchWeaponRoutine(SelectedWeapon targetWeapon)
    {
        // Straight up not mine, copied from the internet, tiny detail which i needed because im autistic
        isSwitching = true;

        // Move Hand Down
        Vector3 targetPos = originalHandPos - new Vector3(0, switchDropAmount, 0);
        float elapsed = 0f;

        while (elapsed < switchDuration)
        {
            Hand.transform.localPosition = Vector3.Lerp(originalHandPos, targetPos, elapsed / switchDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Hand.transform.localPosition = targetPos;

        // Swap the active weapon when hand is at the bottom
        if (targetWeapon == SelectedWeapon.Fists)
        {
            Fists.SetActive(true);
            Revolver.SetActive(false);
            currentWeapon = SelectedWeapon.Fists;
            bulletVelocity = 3f; 
            bulletLifetime = 0.1f;
            FireDelay = 0.30f;
            FireDelayAfter = 0.35f;
        }
        else if (targetWeapon == SelectedWeapon.Revolver)
        {
            Fists.SetActive(false);
            Revolver.SetActive(true);
            currentWeapon = SelectedWeapon.Revolver;
            bulletVelocity = 20f; 
            bulletLifetime = 1.2f;
            FireDelay = 0.10f;
            FireDelayAfter = 0.75f;
        }

        // Move Hand Back Up
        elapsed = 0f;
        while (elapsed < switchDuration)
        {
            Hand.transform.localPosition = Vector3.Lerp(targetPos, originalHandPos, elapsed / switchDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Hand.transform.localPosition = originalHandPos;

        isSwitching = false;
    }
}