using TMPro;
using UnityEngine;

public class TextStealth : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float fadeTime;
    [SerializeField] private FadeMode fadeModeOnEnter = FadeMode.FadeIn;

    private FadeMode currentMode = FadeMode.None;

    private float t = 0f;

    public enum FadeMode
    {
        None = 0,
        FadeIn = 1,
        FadeOut = -1
    }

    private void Awake()
    {
        Color color = text.color;
        color.a = 0f;
        text.color = color;
    }

    private void OnEnable()
    {
        currentMode = (FadeMode)(-1 * (int)fadeModeOnEnter);
    }

    private void Update()
    {
        switch (currentMode)
        {
            case FadeMode.FadeIn:
                if (text.color.a < 1f)
                {
                    t = Mathf.Clamp01(t + Time.deltaTime / fadeTime);
                    Color color = text.color;
                    color.a = Mathf.Lerp(0f, 1f, t);
                    text.color = color;
                }
                else
                {
                    currentMode = FadeMode.None;
                    t = 1f;
                }
                break;
            case FadeMode.FadeOut:
                if (text.color.a > 0f)
                {
                    t = Mathf.Clamp01(t - Time.deltaTime / fadeTime);
                    Color color = text.color;
                    color.a = Mathf.Lerp(0f, 1f, t);
                    text.color = color;
                }
                else
                {
                    currentMode = FadeMode.None;
                    t = 0f;
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            currentMode = fadeModeOnEnter;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            currentMode = (FadeMode)(-1 * (int)fadeModeOnEnter);
    }
}