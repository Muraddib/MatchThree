using UnityEngine;
using System;
using System.Collections;

public interface IAnimationToken
{
    bool isPlaying { get; }

    bool isStopped { get; }

    void Cancel();
}

public static class AnimationTokenExtensionMethods
{
    public static bool IsValidAndPlaying(this IAnimationToken token)
    {
        return token != null && token.isPlaying;
    }

    public static bool IsNullOrStopped(this IAnimationToken token)
    {
        return token == null || token.isStopped;
    }

    public static void SafeCancel(this IAnimationToken token)
    {
        if (token != null)
        {
            token.Cancel();
        }
    }
}

public class AnimationHelper : MonoBehaviour
{
    private class AnimationToken : IAnimationToken
    {
        public Coroutine coroutine;

        public bool isPlaying { get { return coroutine != null; } }

        public bool isStopped { get { return coroutine == null; } }

        public void Cancel()
        {
            if (isPlaying)
            {
                inst.StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }

    private static AnimationHelper inst;

    void Awake()
    {
        EnforceSingleton();
    }

    void EnforceSingleton()
    {
        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public static IAnimationToken Animate(float startTime, float duration, Action<float> onUpdate, Action onFinish = null)
    {
        AnimationToken token = new AnimationToken();
        token.coroutine = inst.StartCoroutine(AnimateCoroutine(token, startTime, duration, onUpdate, onFinish));

        return token;
    }

    static IEnumerator AnimateCoroutine(AnimationToken token, float startTime, float duration, Action<float> onUpdate, Action onFinish)
    {
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            onUpdate(Mathf.Clamp01((Time.time - startTime) / duration));
            yield return null;
        }

        onUpdate(1f);

        yield return null;

        token.coroutine = null;

        if (onFinish != null)
        {
            onFinish();
        }

        yield return null;
    }
}