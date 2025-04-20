using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCapsule : MonoBehaviour
{
    private bool collected = false;

    public void Collect()
    {
        if (collected) return;

        collected = true;
        EnergyCapsuleManager.Instance.OnCapsuleCollected();
        gameObject.SetActive(false);
    }

    public void ResetCapsule()
    {
        collected = false;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Robot"))
        {
            Collect();
        }
    }
}
