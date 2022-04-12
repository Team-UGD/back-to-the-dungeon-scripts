using System.Collections;
using UnityEngine;

public partial class Laser : MonoBehaviour
{
    [SerializeField] private LineRenderer laserSignal;
    [SerializeField] private LineRenderer laserEffect;
    [SerializeField] private LaserData laserData;
    [SerializeField] private bool autoDestroy = true;

    public bool test;
    [MoveTool] public Vector2 start;
    [MoveTool] public Vector2 end;

    /// <summary>
    /// 레이저 발사 가능 여부
    /// </summary>
    public bool IsReady { get; private set; } = true;

    /// <summary>
    /// 레이저 발사 이후 자동으로 이 오브젝트를 파괴할지 여부. Default: true
    /// </summary>
    public bool AutoDestroy { get => autoDestroy; set => autoDestroy = value; }

    /// <summary>
    /// 레이저의 데미지
    /// </summary>
    public float Damage { get; set; } = 10f;

    /// <summary>
    /// 레이저 넉백 파워. Default: 1
    /// </summary>
    public float KnockbackPower { get; set; } = 1f;


    public readonly struct Route
    {
        public readonly Vector3 start;
        public readonly Vector3 end;

        public Route(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
        }
    }

    /// <summary>
    /// 레이저 발사 메서드
    /// </summary>
    /// <param name="route">경로</param>
    public void Fire(in Route route)
    {
        if (!IsReady)
            throw new System.InvalidOperationException($"You can only fire the laser when the state is ready.");

        // CleanUp
        //CleanUp();

        // Initialize
        SetLaserPosition(route);

        // Trigger Delaya
        this.currentRoutine = StartCoroutine(LaserFireRoutine(route));
    }
}

public partial class Laser
{
    private Coroutine currentRoutine;

    private void Update()
    {
        if (this.test && this.IsReady)
        {
            Fire(new Route(start, end));
        }
    }

    private bool DamageToTarget(in RaycastHit2D hitInfo)
    {
        if (hitInfo.collider == null)
            return false;

        var entity = hitInfo.collider.GetComponent<Entity>();
        if (entity == null)
            return false;

        entity.TakeDamage(Damage, hitInfo.point, hitInfo.normal, power: KnockbackPower);
        return true;
    }

    private void CleanUp()
    {
        this.laserEffect.gameObject.SetActive(false);
        //this.effect.gameObject.SetActive(false);
        //this.SetLaserColorAlphaValue(this.laserEffectDefaultAlphaValue);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
    }

    private void SetLaserPosition(in Route route)
    {
        laserEffect.SetPosition(0, route.start);
        laserEffect.SetPosition(1, route.start);
        laserSignal.SetPosition(0, route.start);
        laserSignal.SetPosition(1, route.end);
    }

    private void MoveLaserEndPosPerFrame(Vector3 velocity, float deltaTime)
    {
        this.laserEffect.SetPosition(1, laserEffect.GetPosition(1) + velocity * deltaTime);
    }

    private bool IsLaserArrived(Vector3 direction, Vector3 goalPos) => Vector3.Dot(direction, goalPos - laserEffect.GetPosition(1)) < 0;

    private IEnumerator LaserFireRoutine(Route route)
    {
        this.IsReady = false;

        // Fade signal in
        laserSignal.gameObject.SetActive(true);

        float currentTime = 0f;

        float defaultSignalAlpha = laserSignal.colorGradient.alphaKeys[0].alpha;
        float defaultEffectAlpha = laserEffect.colorGradient.alphaKeys[0].alpha;
        //this.laserEffectDefaultAlphaValue = defaultSignalAlpha;

        while (currentTime <= laserData.FadeInTime)
        {
            currentTime += Time.deltaTime;
            SetLaserColorAlphaValue(laserSignal, defaultSignalAlpha * laserData.FadeInAlphaValue.Evaluate(Mathf.InverseLerp(0f, laserData.FadeInTime, currentTime)));
            yield return null;
        }

        // Fire Laser
        laserEffect.gameObject.SetActive(true);
        //SetLaserColorAlphaValue(defaultAlphaValue);
        Vector2 displacement = route.end - route.start;
        //float angle = Vector2.SignedAngle(Vector2.right, displacement);
        //var hitInfo = Physics2D.BoxCast(route.start, laserData.HitAreaSize, angle, displacement.normalized, displacement.magnitude, laserData.LayerMask);
        //this.DamageToTarget(hitInfo);

        //currentTime = 0f;

        StartCoroutine(AttackTarget(route));
        Vector2 velocity = laserData.Speed * displacement.normalized;
        while (true)
        {
            this.MoveLaserEndPosPerFrame(velocity, Time.deltaTime);
            if (this.IsLaserArrived(velocity, route.end))
            {
                laserEffect.SetPosition(1, route.end);
                break;
            }

            yield return null;
        }

        //Debug.Log("종료");

        //yield return new WaitForSeconds(laserData.LaserFireDuration);

        // Fade laser, signal out
        currentTime = 0f;
        //SetLaserColorAlphaValue(laserSignal, defaultSignalAlpha);
        //laserSignal.gameObject.SetActive(false);

        float defaultEffectWidth = laserEffect.widthMultiplier;
        float defaultSignalWidth = laserSignal.widthMultiplier;
        while (currentTime <= laserData.FadeOutTime)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.InverseLerp(0f, laserData.FadeOutTime, currentTime);

            // Alpha value set
            float t_alpha = laserData.FadeOutAlphaValue.Evaluate(t);
            SetLaserColorAlphaValue(laserEffect, defaultEffectAlpha * t_alpha);
            SetLaserColorAlphaValue(laserSignal, defaultSignalAlpha * t_alpha);

            // Width value set
            float t_width = laserData.FadeOutWidthMultiplierValue.Evaluate(t);
            SetLaserWidth(laserEffect, defaultEffectWidth * t_width);
            SetLaserWidth(laserSignal, defaultSignalWidth * t_width);
            yield return null;
        }

        SetLaserWidth(laserSignal, defaultSignalWidth);
        SetLaserWidth(laserEffect, defaultEffectWidth);
        SetLaserColorAlphaValue(laserSignal, defaultSignalAlpha);
        SetLaserColorAlphaValue(laserEffect, defaultEffectAlpha);
        laserSignal.gameObject.SetActive(false);
        laserEffect.gameObject.SetActive(false);
        //laserSignal.gameObject.SetActive(false);

        this.IsReady = true;

        if (AutoDestroy)
            Destroy(this.gameObject);
    }

    private IEnumerator AttackTarget(Route route)
    {
        float t = 0f;
        Vector2 displacement = route.end - route.start;
        float angle = Vector2.SignedAngle(Vector2.right, displacement);

        while (t < laserData.AttackDuration)
        {
            t += Time.deltaTime;
            var hitInfo = Physics2D.BoxCast(route.start, laserData.HitAreaSize, angle, displacement.normalized, displacement.magnitude, laserData.LayerMask);
            if (this.DamageToTarget(hitInfo))
                yield break;

            yield return null;
        }
    }

    private void SetLaserColorAlphaValue(LineRenderer renderer, float alpha)
    {
        var keys = renderer.colorGradient.alphaKeys;
        for (int i = 0; i < keys.Length; i++)
        {
            var key = keys[i];
            key.alpha = alpha;
            keys[i] = key;
        }
        var gradient = new Gradient();
        gradient.SetKeys(renderer.colorGradient.colorKeys, keys);
        renderer.colorGradient = gradient;
        //Debug.Log($"Alpha: {keys[0].alpha}");
    }

    private void SetLaserWidth(LineRenderer laser, float width)
    {
        laser.widthMultiplier = width;
    }
}