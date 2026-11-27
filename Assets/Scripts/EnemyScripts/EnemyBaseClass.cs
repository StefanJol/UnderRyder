using System.Collections;
using UnityEngine;

public class EnemyBaseClass : MonoBehaviour
{   
    private Renderer[] targetRenderers;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    public int HeadShotModifier = 3;
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        // Find all renderers in this object and its children (Head, Body, etc.)
        targetRenderers = GetComponentsInChildren<Renderer>();
        
        // Store their original colors
        originalColors = new Color[targetRenderers.Length];
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            originalColors[i] = targetRenderers[i].material.color;
        }
    }

    public void TakeDamage(int damage, bool isHeadshot)
    {
        // Apply headshot multiplier
        int finalDamage = isHeadshot ? damage * HeadShotModifier : damage;
        currentHealth -= finalDamage;

        
        TriggerFlash();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TriggerFlash()
    {
        if (targetRenderers == null || targetRenderers.Length == 0) return;

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Turn all renderers red
        foreach (Renderer r in targetRenderers)
        {
            r.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.15f);

        // Restore original colors
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            targetRenderers[i].material.color = originalColors[i];
        }

        flashRoutine = null;
    }

    void Die()
    {
        //Debug.Log(gameObject.name + " died!");
        Destroy(gameObject); // Destroys the enemy body
    }
}