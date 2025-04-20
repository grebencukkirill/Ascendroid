using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyCapsuleManager : MonoBehaviour
{
    public static EnergyCapsuleManager Instance;

    [System.Serializable]
    public class CapsuleSlot
    {
        public Image background;
        public Image icon;
    }

    public CapsuleSlot[] slots;
    private List<EnergyCapsule> allCapsules;

    public Sprite emptyBackground;
    public Sprite collectedBackground;

    public AudioSource audioSource;

    private int collectedCount = 0;

    private void Awake()
    {
        Instance = this;
        allCapsules = new List<EnergyCapsule>(FindObjectsOfType<EnergyCapsule>());
    }

    public void OnCapsuleCollected()
    {
        if (collectedCount >= slots.Length)
        {
            Debug.LogWarning("All capsules already collected.");
            return;
        }

        CapsuleSlot slot = slots[collectedCount];
        slot.background.sprite = collectedBackground;
        slot.icon.enabled = true;

        collectedCount++;
        
        audioSource.Play();

        Debug.Log($"Capsule collected! Total: {collectedCount}");
    }

    public int GetCollectedCount()
    {
        return collectedCount;
    }

    public void ResetCapsules()
    {
        collectedCount = 0;

        foreach (var slot in slots)
        {
            slot.background.sprite = emptyBackground;
            slot.icon.enabled = false;
        }

        foreach (var capsule in allCapsules)
        {
            capsule.ResetCapsule();
        }

        Debug.Log("Capsules reset.");
    }
}
