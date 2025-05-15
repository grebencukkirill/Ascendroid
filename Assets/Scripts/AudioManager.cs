using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource editSourceA;
    public AudioSource editSourceB;
    public AudioSource playSourceA;
    public AudioSource playSourceB;

    [Header("Audio Clips")]
    public AudioClip editClip;
    public AudioClip playClip;

    [Header("Timing Settings")]
    public float bpm = 127f;
    public int fadeBeats = 24;

    private float fadeDuration;
    private float loopPointEdit;
    private float loopPointPlay;
    private float syncInterval;

    private AudioSource activeEdit, inactiveEdit;
    private AudioSource activePlay, inactivePlay;

    private bool isPlayMode = false;
    private Coroutine currentSwitchCoroutine;
    private Coroutine currentLoopCoroutine;

    public Action OnPlayModeReady;
    public Action OnEditModeReady;

    public static AudioManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (MenuMusicManager.Instance != null)
            MenuMusicManager.Instance.StopMusic();

        fadeDuration = 60f / bpm * fadeBeats;
        syncInterval = 60f / bpm * 4f;

        loopPointEdit = editClip.length - fadeDuration;
        loopPointPlay = playClip.length - fadeDuration;

        SetupSource(editSourceA, editClip);
        SetupSource(editSourceB, editClip);
        SetupSource(playSourceA, playClip);
        SetupSource(playSourceB, playClip);

        activeEdit = editSourceA;
        inactiveEdit = editSourceB;
        activePlay = playSourceA;
        inactivePlay = playSourceB;

        StartLoop(edit: true);
    }

    void SetupSource(AudioSource src, AudioClip clip)
    {
        src.clip = clip;
        src.loop = false;
        src.volume = 1f;
    }

    void StartLoop(bool edit)
    {
        StopCurrentLoop();

        if (edit)
        {
            activeEdit.time = 0f;
            activeEdit.Play();
            currentLoopCoroutine = StartCoroutine(LoopWithFade(editMode: true));
        }
        else
        {
            activePlay.time = 0f;
            activePlay.Play();
            currentLoopCoroutine = StartCoroutine(LoopWithFade(editMode: false));
        }
    }

    void StopCurrentLoop()
    {
        if (currentLoopCoroutine != null)
            StopCoroutine(currentLoopCoroutine);
    }

    IEnumerator LoopWithFade(bool editMode)
    {
        while (true)
        {
            AudioSource current = editMode ? activeEdit : activePlay;
            AudioSource next = editMode ? inactiveEdit : inactivePlay;
            float loopPoint = editMode ? loopPointEdit : loopPointPlay;
            AudioClip clip = editMode ? editClip : playClip;

            float timeToFade = loopPoint - current.time;
            if (timeToFade > 0)
                yield return new WaitForSecondsRealtime(timeToFade);

            next.time = 0f;
            next.volume = 1f;
            next.Play();

            StartCoroutine(FadeOut(current, fadeDuration));

            yield return new WaitForSecondsRealtime(fadeDuration);

            // Swap sources
            if (editMode)
            {
                var temp = activeEdit;
                activeEdit = inactiveEdit;
                inactiveEdit = temp;
            }
            else
            {
                var temp = activePlay;
                activePlay = inactivePlay;
                inactivePlay = temp;
            }
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

    IEnumerator SwitchToPlayMode()
    {
        float wait = syncInterval - (GetCurrentTime() % syncInterval);
        yield return new WaitForSecondsRealtime(wait);

        // --- Логика фрагмента (чтобы соблюсти структуру перехода)
        float currentTime = GetCurrentTime();
        int editFragmentIndex = Mathf.FloorToInt(currentTime / syncInterval);
        int localFragment = editFragmentIndex % 4;
        int nextFragment = (localFragment + 1) % 4;

        float targetTime = nextFragment * syncInterval;

        // Гарантируем, что не заедем в fade-зону
        if (targetTime >= loopPointPlay)
            targetTime = 0f;

        // Остановить старый цикл без fade
        StopCurrentLoop();

        activeEdit.Stop();
        inactiveEdit.Stop();

        // Запуск Play-трека на рассчитанной позиции
        activePlay.time = targetTime;
        activePlay.volume = 1f;
        activePlay.Play();

        // Перезапуск зацикливания с fade'ом внутри режима
        currentLoopCoroutine = StartCoroutine(LoopWithFade(editMode: false));
        isPlayMode = true;

        OnPlayModeReady?.Invoke();
    }

    IEnumerator SwitchToEditMode(bool force)
    {
        if (!force)
        {
            float wait = syncInterval - (GetCurrentTime() % syncInterval);
            yield return new WaitForSecondsRealtime(wait);
        }

        float currentTime = GetCurrentTime();
        float safeTime = Mathf.Min(currentTime, loopPointEdit);

        StopCurrentLoop();

        // Stop play mode source IMMEDIATELY
        activePlay.Stop();
        inactivePlay.Stop();

        activeEdit.time = force ? 0f : safeTime;
        activeEdit.volume = 1f;
        activeEdit.Play();

        currentLoopCoroutine = StartCoroutine(LoopWithFade(editMode: true));
        isPlayMode = false;

        OnEditModeReady?.Invoke();
    }

    public float GetCurrentTime()
    {
        return isPlayMode ? activePlay.time : activeEdit.time;
    }

    IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVol = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        source.Stop();
        source.volume = 1f;
    }
}
