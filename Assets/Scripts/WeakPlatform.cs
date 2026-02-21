using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps; 

public class WeakPlatform : MonoBehaviour
{
    public float collapseTime = 1.5f;

    private bool isCollapsing = false;
    private CompositeCollider2D compositeCol; 
    private TilemapCollider2D tilemapCol; // Added to catch the raw collider
    private TilemapRenderer tmr;    

    void Start()
    {
        // Get ALL the components involved in the platform
        compositeCol = GetComponent<CompositeCollider2D>();
        tilemapCol = GetComponent<TilemapCollider2D>();
        tmr = GetComponent<TilemapRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isCollapsing)
        {
            StartCoroutine(Collapse());
        }
    }

    IEnumerator Collapse()
    {
        isCollapsing = true;

        // 1. Visual Warning (Red)
        if (tmr != null)
        {
            tmr.material.color = Color.red;
        }

        yield return new WaitForSeconds(collapseTime);

        // 2. Disable Visuals
        if (tmr != null)
        {
            tmr.enabled = false;
        }

        // 3. Disable BOTH Colliders to ensure nothing is blocking the player
        if (compositeCol != null) compositeCol.enabled = false;
        if (tilemapCol != null) tilemapCol.enabled = false;
    }
}