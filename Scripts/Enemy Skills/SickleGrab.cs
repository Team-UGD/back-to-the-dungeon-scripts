using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Skills/SickleGrab")]
public class SickleGrab : EnemySkill
{
    private Vector3 playerPos;
    private Vector3 creationPosition;
    public BossSmash bossSmash;
    private Vector3 playerInhook;
    private float startTime;
    private float curTime;


    //[SerializeField] private TrailRenderer effect;
    [SerializeField] private float heatingSpeed = 20f;
    [SerializeField] private float goSpeed = 35f;
    [SerializeField] private float grabSpeed = 60f;
    [SerializeField] private float heatingDist = 5f;
    [SerializeField] private float goDist = 10f;


    [System.NonSerialized]
    public float baseDamage = 15f;
    public AudioClip SmashSound;

    [Header("Creation")]
    public GrabbingSickle grabbingSickle;

    private Dictionary<Attacker, GrabbingSickle> createdBossSickleBuffer = new Dictionary<Attacker, GrabbingSickle>();
    public override void TriggerSkill(Attacker user, GameObject target)
    {
        startTime = Time.time;    //스킬이 Trigger가 된 시간을 startTime으로 잡는다.
        if (createdBossSickleBuffer.TryGetValue(user, out var grabSickle))
        {
            if (grabSickle != null)
            {
                Debug.Log($"[{typeof(GrabbingSickle)}] 아직 Hook이 존재합니다.");
                Destroy(grabSickle.gameObject);
                return;
            }
            createdBossSickleBuffer.Remove(user);
        }
        float targetPos_x = target.transform.position.x;
        CapsuleCollider2D playerCapsule = target.GetComponent<CapsuleCollider2D>();
        Bounds playerBounds = playerCapsule.bounds;
        if (targetPos_x <= user.transform.position.x)
        {
            playerPos = new Vector2(playerBounds.center.x, playerBounds.center.y);
            creationPosition = user.transform.position + new Vector3(-1f, 0f);
            var sickle = (GrabbingSickle)Instantiate(grabbingSickle, creationPosition, Quaternion.identity);
            sickle.transform.localScale = new Vector3(-1f, -1f, 1f);
            createdBossSickleBuffer.Add(user, sickle);

            sickle.StartCoroutine(sickleGrab(user, sickle, target, false));
            Debug.Log($"[{typeof(GrabbingSickle)}] 왼쪽 훅 스킬 활성화");
        }
        else
        {
            playerPos = new Vector2(playerBounds.center.x, playerBounds.center.y);
            creationPosition = user.transform.position + new Vector3(1f, 0f);
            var sickle = (GrabbingSickle)Instantiate(grabbingSickle, creationPosition, Quaternion.identity);
            sickle.transform.localScale = new Vector3(1f, -1f, 1f);
            createdBossSickleBuffer.Add(user, sickle);

            sickle.StartCoroutine(sickleGrab(user, sickle, target, true));
            Debug.Log($"[{typeof(GrabbingSickle)}] 오른쪽 훅 스킬 활성화");
        }
    }

    // 주어지는 값: heatingDist(예열 거리), goDist(플레이어에게 가는 거리), heatingSpeed(예열 속도), goSpeed(플레이어에게 가는 속도), grabSpeed(끄는 속도)

    // <sickleGrabSkill은 세 가지의 행동으로 진행된다.>
    // Step 1. 예열하는 행동(또는 준비 시간)  -> 속도를 느리게 할 계획
    // Step 2. 플레이어에게 슝 하고 튀어가는 행동 -> 급진적으로 상향되는 속도
    // Step 3. User쪽으로 당기는 행동 -> 이 과정에서 플레이어가 감지가 된다면 플레이어가 끌리게 되고 -> 바로 BossSmash 실행
    // 이 스킬은 각 특정 (주어진 속도, 거리로 도출해낸)시간동안 각 Step들이 순서대로 진행된다.
    private IEnumerator sickleGrab(Attacker user, GrabbingSickle hook, GameObject target, bool isRigh)
    {
        bool touch = false;
        // S = V * T 공식을 사용하여 Inspector에서 조정된 값에 따른 시간을 적용할 수 있도록 
        // T = S / V 로 주어진 속도로 주어진 거리를 가려면 얼마의 시간이 걸릴지 계산한다.
        float grabDist = heatingDist + goDist;
        float heatingTime = heatingDist / heatingSpeed;
        float goTime = goDist / goSpeed;
        float grabTime = grabDist / grabSpeed;

        // 각 시간에 맞는 baseCoolDown 시간과, 1초를 더해주어 조금 안정화된 castingTime을 조정해줄 수 있다.
        baseCoolDown = heatingTime + goTime + grabTime;
        castingTime = heatingTime + goTime + grabTime + 1f;

        // 프리팹의 피봇값이 각도가 맞지 않아 부모오브젝트에 각도를 맞췄다.
        // 부모 오브젝트를 이용하여 Sickle이 플레이어를 향해 바라볼 수 있도록 조절해주었다.
        // 그러나 프리팹을 자식 오브젝트에 넣어서 플레이어가 닿았는지 검사해주는 건(isTouch) 자식 오브젝트에서 정보를 얻어와야한다.
        // childSickle에 자식 오브젝트를 부른다.
        // 코드가 길어질 것과, isTouch를 가져오기 위해 childSickle.GetComponent<GrabbingSickle>()을 매번 해주면 효율성이 떨어질것으로 예상.
        // grabbingChild라는 곳에 미리 childSickle.GetComponent<GrabbingSickle>()를 해준다.

        GameObject childSickle = hook.transform.GetChild(0).gameObject;
        GrabbingSickle grabbingChild = childSickle.GetComponent<GrabbingSickle>();
        grabbingChild.effect.enabled = true;
        if (target != null)
        {
            Vector2 dir = new Vector2(creationPosition.x - playerPos.x, creationPosition.y - playerPos.y);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            hook.transform.rotation = Quaternion.Slerp(hook.transform.rotation, angleAxis, 1);

        }

        //훅의 y축을 이용하여 이동해줄 것이다. 
        //아까 설명해준 Step들을 이행할 부분
        while (true)
        {
            //Debug.Log($"<{nameof(SickleGrab)}> isTouch = {grabbingChild.isTouch}", this);
            curTime = Time.time;

            if ((curTime - startTime >= 0) && (curTime - startTime < heatingTime))                          //Step 1
            {
                hook.transform.localPosition -= hook.transform.up * heatingSpeed * Time.fixedDeltaTime;
            }
            else if ((curTime - startTime >= heatingTime) && (curTime - startTime < goTime + heatingTime))  //Step 2   
            {
                hook.transform.localPosition -= hook.transform.up * goSpeed * Time.fixedDeltaTime;
            }
            else if ((curTime - startTime >= goTime + heatingTime) && (curTime - startTime < goTime + heatingTime + grabTime))  //Step 3
            {
                //grabTime 중에 플레이어가 닿았다면, 
                if (grabbingChild.isTouch)
                {
                    touch = true;
                    CapsuleCollider2D playerCapsule = target.GetComponent<CapsuleCollider2D>();
                    Bounds playerBounds = playerCapsule.bounds;
                    playerInhook = new Vector2(playerBounds.center.x, playerBounds.center.y);
                    if (target != null)
                    {
                        Vector2 dir1 = new Vector2(playerInhook.x - user.transform.position.x, playerInhook.y - user.transform.position.y);
                        float angle1 = Mathf.Atan2(dir1.y, dir1.x) * Mathf.Rad2Deg;
                        Quaternion angleAxis1 = Quaternion.AngleAxis(angle1 + 90f, Vector3.forward);
                        hook.transform.rotation = Quaternion.Slerp(hook.transform.rotation, angleAxis1, 1);
                    }
                    hook.transform.position += hook.transform.up * grabSpeed * Time.fixedDeltaTime;
                    target.transform.position = hook.BladePosition;
                }
                else                                          //grabTime 중 플레이어가 닿지 않았다면,
                {
                    if (touch)
                    {
                        CapsuleCollider2D playerCapsule = target.GetComponent<CapsuleCollider2D>();
                        Bounds playerBounds = playerCapsule.bounds;
                        playerInhook = new Vector2(playerBounds.center.x, playerBounds.center.y);
                        if (target != null)
                        {
                            Vector2 dir1 = new Vector2(playerInhook.x - user.transform.position.x, playerInhook.y - user.transform.position.y);
                            float angle1 = Mathf.Atan2(dir1.y, dir1.x) * Mathf.Rad2Deg;
                            Quaternion angleAxis1 = Quaternion.AngleAxis(angle1 + 90f, Vector3.forward);
                            hook.transform.rotation = Quaternion.Slerp(hook.transform.rotation, angleAxis1, 1);
                        }
                        hook.transform.position += hook.transform.up * grabSpeed * Time.fixedDeltaTime;
                        target.transform.position = hook.BladePosition;
                    }
                    else
                    {
                        hook.transform.localPosition += hook.transform.up * grabSpeed * Time.fixedDeltaTime;
                    }
                }
            }
            else
            {
                if (touch)
                {
                    yield return new WaitForSeconds(0.5f);
                    Debug.Log("BossSkill 부르기");
                    bossSmash.TriggerSkill(user, target);
                    touch = false;
                }
                yield return new WaitForSeconds(grabbingChild.effect.time);
                grabbingChild.effect.enabled = true;
                Destroy(hook.gameObject);
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        //3개의 Step이 마무리 된다면 hook을 Destroy해준다.
    }
}
