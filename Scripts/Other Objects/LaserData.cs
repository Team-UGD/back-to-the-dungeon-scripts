using UnityEngine;

[CreateAssetMenu(menuName = "Custom Data/Laser")]
public class LaserData : ScriptableObject
{
    [Header("Fade In")]
    [SerializeField] private AnimationCurve fadeInAlphaValue;
    [SerializeField] private float fadeInTime;

    [Header("Laser Fire")]
    [SerializeField] private float attackDuration;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 hitAreaSize;
    [SerializeField] private LayerMask layerMask;

    [Header("Fade Out")]
    [SerializeField] private AnimationCurve fadeOutAlphaValue;
    [SerializeField] private AnimationCurve fadeOutWidthMultiplierValue;
    [SerializeField] private float fadeOutTime;

    public AnimationCurve FadeInAlphaValue { get => fadeInAlphaValue; }
    public AnimationCurve FadeOutAlphaValue { get => fadeOutAlphaValue; }
    public AnimationCurve FadeOutWidthMultiplierValue { get => fadeOutWidthMultiplierValue; }
    public float AttackDuration { get => attackDuration; }
    public float Speed { get => speed; }
    public float FadeInTime { get => fadeInTime; }
    public float FadeOutTime { get => fadeOutTime; }
    public LayerMask LayerMask { get => layerMask; }
    public Vector2 HitAreaSize { get => hitAreaSize; }
}
