using UnityEngine;
using System.Collections;

public class WeakPlatform : MonoBehaviour
{
    public float collapseTime = 1.5f;

    private bool isCollapsing = false;
    private BoxCollider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
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

        // Optional: visual warning
        if (sr != null)
            sr.color = Color.red;

        yield return new WaitForSeconds(collapseTime);

        // Disable platform permanently
        col.enabled = false;

        if (sr != null)
            sr.enabled = false;

        // If you want it to fully disappear:
        // gameObject.SetActive(false);
    }
}
