using UnityEngine;

[System.Serializable]
public struct DeviceInfo
{
    public string deviceName;
    public int deviceCount;
    public Sprite deviceIcon;
    public GameObject devicePrefab;
    public GameObject deviceSilhouette;
}