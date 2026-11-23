using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponFire : MonoBehaviour
{
    public Animator weaponAnimator;

    void Update()
    {
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
                weaponAnimator.SetTrigger("Fire");
                Debug.Log("Firing Works");
        }
    }
}