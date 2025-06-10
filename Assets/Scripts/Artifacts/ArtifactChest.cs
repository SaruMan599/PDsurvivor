// ArtifactChest.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ArtifactChest : MonoBehaviour
{
    private bool isOpened = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpened) return;
        if (other.CompareTag("Player"))
        {
            isOpened = true;
            ArtifactManager mgr = FindObjectOfType<ArtifactManager>();
            if (mgr != null) mgr.OnChestOpened();
            Destroy(gameObject);
        }
    }
}
