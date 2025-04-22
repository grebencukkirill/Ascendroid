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

    public int laserLength = 3;
    public float phaseInterval = 0.2f;
    public float activeDuration = 0.5f;

    public Vector2Int direction = Vector2Int.up;

    private Coroutine cycleCoroutine;
    private List<GameObject> currentSegments = new List<GameObject>();

    private string[] deviceTags = { "LiftPad", "DashPad", "Redirect", "GravFlip", "AccelPad", "SlowPad" };

    void Start()
    {
        StartLaser();
    }

    public void StartLaser()
    {
        StopLaser();
        ReplaceSource(sourceEmpty);

        cycleCoroutine = StartCoroutine(LaserCycle());
    }

    public void StopLaser()
    {
        if (cycleCoroutine != null)
            StopCoroutine(cycleCoroutine);

        foreach (var seg in currentSegments)
            if (seg) Destroy(seg);

        currentSegments.Clear();
        ReplaceSource(sourceEmpty);
    }

    IEnumerator LaserCycle()
    {
        while (true)
        {
            if (laserLength == 1)
            {
                ReplaceSource(sourceHalf);
                yield return new WaitForSeconds(phaseInterval);

                ReplaceSource(sourceEmpty);
                yield return new WaitForSeconds(phaseInterval);
            }
            else
            {
                yield return StartCoroutine(ActivateLaser());
                yield return StartCoroutine(DeactivateLaser());
                yield return new WaitForSeconds(activeDuration);
            }
        }
    }

    IEnumerator ActivateLaser()
    {
        ReplaceSource(sourceHalf);
        yield return new WaitForSeconds(phaseInterval);

        ReplaceSource(sourceFull);
        yield return new WaitForSeconds(phaseInterval);

        for (int i = 1; i < laserLength; i++)
        {
            Vector3 pos = transform.position + new Vector3(direction.x, direction.y, 0) * i;

            GameObject half = Instantiate(segmentHalf, pos, GetRotation(), transform);
            currentSegments.Add(half);
            EraseDevicesAt(pos);
            yield return new WaitForSeconds(phaseInterval);

            if (i == laserLength - 1) break;

            GameObject full = Instantiate(segmentFull, pos, GetRotation(), transform);
            Destroy(half);
            currentSegments[currentSegments.Count - 1] = full;
            EraseDevicesAt(pos);
            yield return new WaitForSeconds(phaseInterval);
        }
    }

    IEnumerator DeactivateLaser()
    {
        for (int i = currentSegments.Count - 1; i >= 0; i--)
        {
            Destroy(currentSegments[i]);
            yield return new WaitForSeconds(phaseInterval);
        }
        currentSegments.Clear();

        ReplaceSource(sourceHalf);
        yield return new WaitForSeconds(phaseInterval);

        ReplaceSource(sourceEmpty);
        yield return new WaitForSeconds(phaseInterval);
    }

    void ReplaceSource(GameObject newSource)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Instantiate(newSource, transform.position, GetRotation(), transform);
    }

    Quaternion GetRotation()
    {
        if (direction == Vector2Int.up) return Quaternion.identity;
        if (direction == Vector2Int.right) return Quaternion.Euler(0, 0, -90);
        if (direction == Vector2Int.down) return Quaternion.Euler(0, 0, 180);
        if (direction == Vector2Int.left) return Quaternion.Euler(0, 0, 90);
        return Quaternion.identity;
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
