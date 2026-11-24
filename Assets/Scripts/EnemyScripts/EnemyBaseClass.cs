using System.Collections;
using UnityEngine;

public class EnemyBaseClass : MonoBehaviour
{
    private Renderer targetRenderer;
    private Material mat;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        
            mat = targetRenderer.material;
            originalColor = mat.color;
        
    }

    public void TriggerFlash()
    {
        if (targetRenderer == null) return; // if enemy dead, dont do anything

        // If it's already flashing, restart the timer smoothly
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        mat.color = originalColor;
        flashRoutine = null;
    }
}