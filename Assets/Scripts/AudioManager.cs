using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource editSource;
    public AudioSource playSource;

    public AudioClip editClip;
    public AudioClip playClip;

    private float syncInterval = 1.889f;
    private float fadeDuration = 11.334f;

    private float loopPointEdit;
    private float loopPointPlay;

    private Coroutine currentSwitchCoroutine;
    private bool isPlayMode = false;

    public Action OnPlayModeReady;
    public Action OnEditModeReady;

    void Start()
    {
        if (MenuMusicManager.Instance != null)
        {
            MenuMusicManager.Instance.StopMusic();
        }

        loopPointEdit = editClip.length - fadeDuration;
        loopPointPlay = playClip.length - fadeDuration;

        editSource.clip = editClip;
        playSource.clip = playClip;

        editSource.loop = false;
        playSource.loop = false;

        editSource.Play();
    }

    void Update()
    {
        float time = GetCurrentTime();

        if (isPlayMode && time >= loopPointPlay)
        {
            playSource.time = 0f;
            playSource.Play();
        }
        else if (!isPlayMode && time >= loopPointEdit)
        {
            editSource.time = 0f;
            editSource.Play();
        }
    }

    public void RequestPlayMode()
    {
        if (isPlayMode) return;

        if (currentSwitchCoroutine != null)
            StopCoroutine(currentSwitchCoroutine);

        currentSwitchCoroutine = StartCoroutine(SwitchToPlayMode());
    }

    public void RequestEditMode(bool force = false)
    {
        if (!isPlayMode && !force) return;

        if (currentSwitchCoroutine != null)
            StopCoroutine(currentSwitchCoroutine);

        currentSwitchCoroutine = StartCoroutine(SwitchToEditMode(force));
    }

    private IEnumerator SwitchToEditMode(bool force)
    {
        if (!force)
        {
            float waitTime = syncInterval - (GetCurrentTime() % syncInterval);
            yield return new WaitForSecondsRealtime(waitTime);
        }

        playSource.Stop();
        editSource.time = force ? 0f : GetCurrentTime();
        editSource.Play();
        isPlayMode = false;

        OnEditModeReady?.Invoke();
    }

    private IEnumerator SwitchToPlayMode()
    {
        float waitTime = syncInterval - (GetCurrentTime() % syncInterval);
        yield return new WaitForSecondsRealtime(waitTime);

        float currentTime = GetCurrentTime();
        int editFragmentIndex = Mathf.FloorToInt(currentTime / syncInterval);
        int localFragment = editFragmentIndex % 4;
        int nextFragment = (localFragment + 1) % 4;

        float targetTime = nextFragment * syncInterval;
        if (targetTime >= playClip.length - fadeDuration)
            targetTime = 0f;

        playSource.time = targetTime;
        playSource.Play();
        editSource.Stop();
        isPlayMode = true;

        OnPlayModeReady?.Invoke();
    }

    float GetCurrentTime()
    {
        return isPlayMode ? playSource.time : editSource.time;
    }
}
