using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public GameObject sourceEmpty;
    public GameObject sourceHalf;
    public GameObject sourceFull;
    public GameObject segmentHalf;
    public GameObject segmentFull;

    public int laserLength;
    public float phaseInterval;
    public float activeDuration;
    public Vector2Int direction = Vector2Int.up;
    public bool lastSegmentIsHalf = true;

    private List<GameObject> currentSegments = new List<GameObject>();
    private bool isActive = false;
    private float totalCycleTime;
    private float segmentTime;

    private bool isEditorPreviewMode = false;

    private string[] deviceTags = { "LiftPad", "DashPad", "Redirect", "GravFlip", "AccelPad", "SlowPad" };

    void Start()
    {
        ResetLaser();
        totalCycleTime = 2 * laserLength * phaseInterval + activeDuration;
        segmentTime = phaseInterval;
    }

    public void StartLaser()
    {
        isActive = true;
        isEditorPreviewMode = false;
        ResetLaser();
    }

    public void StopLaser()
    {
        isActive = false;
        isEditorPreviewMode = false;
        ResetLaser();
    }

    public void ResetLaser()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var seg in currentSegments)
        {
            if (seg) Destroy(seg);
        }
        currentSegments.Clear();
        ReplaceSource(sourceEmpty);
    }

    public void ShowEditorPreview()
    {
        isEditorPreviewMode = true;
        ResetLaser();

        if (laserLength <= 0) return;

        ReplaceSource(laserLength == 1 && lastSegmentIsHalf ? sourceHalf : sourceFull, true);

        for (int i = 1; i < laserLength; i++)
        {
            Vector3 pos = transform.position + new Vector3(direction.x, direction.y, 0) * i;
            GameObject segment = Instantiate(
                (i == laserLength - 1 && lastSegmentIsHalf) ? segmentHalf : segmentFull,
                pos, GetRotation(), transform
            );
            MakePreviewVisual(segment);
            currentSegments.Add(segment);
        }
    }

    void MakePreviewVisual(GameObject obj)
    {
        var col = obj.GetComponent<Collider2D>();
        if (col) Destroy(col);

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr)
        {
            Color c = sr.color;
            sr.color = new Color(1f, 0f, 0f, 0.3f); // красный, прозрачный
        }
    }

    void ReplaceSource(GameObject newSource, bool isEditorPreview = false)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        var source = Instantiate(newSource, transform.position, GetRotation(), transform);
        if (isEditorPreview) MakePreviewVisual(source);
    }

    Quaternion GetRotation()
    {
        if (direction == Vector2Int.up) return Quaternion.identity;
        if (direction == Vector2Int.right) return Quaternion.Euler(0, 0, -90);
        if (direction == Vector2Int.down) return Quaternion.Euler(0, 0, 180);
        if (direction == Vector2Int.left) return Quaternion.Euler(0, 0, 90);
        return Quaternion.identity;
    }

    void Update()
    {
        if (!isActive || isEditorPreviewMode) return;

        float buildTime = laserLength * phaseInterval;
        float pauseBetween = phaseInterval;
        float teardownTime = laserLength * phaseInterval;
        float totalCycleTime = buildTime + pauseBetween + teardownTime + activeDuration;

        float t = LaserManager.Instance.GetCycleTime() % totalCycleTime;

        ResetLaser();

        if (t < buildTime)
        {
            // Появление лазера
            int segIndex = Mathf.FloorToInt(t / phaseInterval);
            if (segIndex == 0)
                ReplaceSource(sourceHalf);
            else
                ReplaceSource(sourceFull);

            for (int i = 1; i <= segIndex && i < laserLength; i++)
            {
                Vector3 pos = transform.position + new Vector3(direction.x, direction.y, 0) * i;
                GameObject prefab = (i == laserLength - 1 && lastSegmentIsHalf) ? segmentHalf : segmentFull;
                var seg = Instantiate(prefab, pos, GetRotation(), transform);
                currentSegments.Add(seg);
                EraseDevicesAt(pos);
            }
        }
        else if (t < buildTime + pauseBetween)
        {
            // Пауза между активацией и исчезновением
            ReplaceSource(sourceFull);
            for (int i = 1; i < laserLength; i++)
            {
                Vector3 pos = transform.position + new Vector3(direction.x, direction.y, 0) * i;
                GameObject prefab = (i == laserLength - 1 && lastSegmentIsHalf) ? segmentHalf : segmentFull;
                var seg = Instantiate(prefab, pos, GetRotation(), transform);
                currentSegments.Add(seg);
            }
        }
        else if (t < buildTime + pauseBetween + teardownTime)
        {
            // Исчезновение лазера
            float timeIntoTeardown = t - buildTime - pauseBetween;
            int remainingSegs = laserLength - Mathf.FloorToInt(timeIntoTeardown / phaseInterval);

            if (remainingSegs <= 0)
            {
                ReplaceSource(sourceEmpty);
            }
            else if (remainingSegs == 1)
            {
                ReplaceSource(sourceHalf);
            }
            else
            {
                ReplaceSource(sourceFull);
            }

            for (int i = 1; i < remainingSegs; i++)
            {
                Vector3 pos = transform.position + new Vector3(direction.x, direction.y, 0) * i;
                GameObject prefab = (i == laserLength - 1 && lastSegmentIsHalf) ? segmentHalf : segmentFull;
                var seg = Instantiate(prefab, pos, GetRotation(), transform);
                currentSegments.Add(seg);
            }
        }
        else
        {
            // Пауза после исчезновения 
            ReplaceSource(sourceEmpty);
        }
    }

    void EraseDevicesAt(Vector3 pos)
    {
        Vector2 cellSize = new Vector2(0.9f, 0.9f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, cellSize, 0f);

        foreach (var hit in hits)
        {
            if (IsDeviceTag(hit.tag))
            {
                int deviceIndex = System.Array.IndexOf(deviceTags, hit.tag);
                if (deviceIndex >= 0 && DevicePanel.Instance != null)
                {
                    DevicePanel.Instance.UpdateDeviceCount(deviceIndex, 1);
                }
                Destroy(hit.gameObject);
            }
        }
    }

    bool IsDeviceTag(string tag)
    {
        foreach (var t in deviceTags)
            if (tag == t) return true;
        return false;
    }
}
