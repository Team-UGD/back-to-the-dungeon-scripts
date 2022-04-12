using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PhysicsUtility;
using static UnityEngine.Mathf;
using static UnityEngine.Vector2;


/// <summary>
/// 정의된 베지어 곡선과 속성에 따라 오브젝트를 움직여주는 컴포넌트
/// </summary>
public class BezierMoveTool : MonoBehaviour, IList<BezierPath2>
{
    #region === Serialized Fields ===

    [Header("< Path Settings >")]
    [Min(1), SerializeField] private int segmentCount = 20;

    //public Transform Target { get => target; set => target = value; }

    [Space]
    [MoveTool, SerializeField] private List<BezierPath2> bezierPath = new List<BezierPath2>();

    [Header("< Path Shift Settings >")]
    [SerializeField] private Vector2 shift;
    [SerializeField, Min(0)] private int shiftedPathIndex;

    [Header("< Run Settings >")]
    [SerializeField] private RunMode mode;
    [Space, SerializeField] private List<MoveProperty> runProperty = new List<MoveProperty>();

    #endregion

    #region === Public Members ===

    /// <summary>
    /// 경로 위에서 움직일 때마다 발생하는 이벤트
    /// </summary>
    public event BezierPath2Handler OnPathMove;

    /// <summary>
    /// 곡선의 분할(근사화) 정도
    /// </summary>
    public int SegmentCount { get => segmentCount; set => segmentCount = Max(1, value); }

    #endregion

    #region === Private Members ===

    private void Start()
    {
        this.Run(false);
    }

    private void Run(bool errorCheck = true)
    {
        StopAllPath();

        if (errorCheck)
        {
            if (mode == RunMode.None)
            {
                throw new ArgumentException($"Run Settings의 Mode가 None입니다.");
            }
            else if (runProperty.Count < 1)
            {
                throw new ArgumentException($"Run Settings의 Run Property에 실행 속성을 정의하세요.");
            }
        }

        try
        {
            for (int i = 0; i < runProperty.Count; i++)
            {
                switch (mode)
                {
                    case RunMode.Once:
                        MovePathOnce(runProperty[i].index, runProperty[i].direction);
                        break;
                    case RunMode.Iterative:
                        MovePathIteratively(runProperty[i].index, runProperty[i].direction);
                        break;
                }
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.LogError("Run Property의 인덱스가 Bezier Path의 인덱스 범위를 벗어났습니다.", this);
            StopAllPath();
        }
    }


    private Queue<(BezierPath2 path, BezierMoveDirection direction)> pathQueue = new Queue<(BezierPath2 path, BezierMoveDirection direction)>();
    private RunMode runMode = RunMode.None;

    [Serializable]
    private struct MoveProperty
    {
        [Min(0)] public int index;
        public BezierMoveDirection direction;

        public MoveProperty(int index, BezierMoveDirection direction)
        {
            this.index = index;
            this.direction = direction;
        }
    }

    public enum RunMode
    {
        None,
        Once,
        Iterative
    }

    #endregion

    #region === Public Methods ===

    /// <summary>
    /// 베지어 곡선 경로를 한번 움직인다.
    /// </summary>
    public void MovePathOnce(BezierPath2 path, BezierMoveDirection direction = BezierMoveDirection.Forward)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path), "경로가 null입니다.");

        if (runMode != RunMode.Once)
        {
            StopAllPath();
            pathQueue.Enqueue((path, direction));
            StartCoroutine(RunOnce());
        }
        else
        {
            pathQueue.Enqueue((path, direction));
        }
    }

    /// <summary>
    /// BezierMoveTool 내부에 정의된 경로들 중 인덱스에 해당하는 베지어 곡선 경로를 한번 움직인다.
    /// </summary>
    public void MovePathOnce(int index, BezierMoveDirection direction = BezierMoveDirection.Forward)
    {
        if (index < 0 || index >= bezierPath.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Bezier Path의 인덱스를 벗어났습니다.");

        if (runMode != RunMode.Once)
        {
            StopAllPath();
            pathQueue.Enqueue((bezierPath[index], direction));
            StartCoroutine(RunOnce());
        }
        else
        {
            pathQueue.Enqueue((bezierPath[index], direction));
        }
    }

    /// <summary>
    /// 베지어 곡선 경로를 반복적으로 움직인다.
    /// </summary>
    public void MovePathIteratively(BezierPath2 path, BezierMoveDirection direction = BezierMoveDirection.Forward)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path), "경로가 null입니다.");

        if (runMode != RunMode.Iterative)
        {
            StopAllPath();
            pathQueue.Enqueue((path, direction));
            StartCoroutine(RunIteratively());
        }
        else
        {
            pathQueue.Enqueue((path, direction));
        }
    }

    /// <summary>
    /// BezierMoveTool 내부에 정의된 경로들 중 인덱스에 해당하는 베지어 곡선 경로를 번복적으로 움직인다.
    /// </summary>
    public void MovePathIteratively(int index, BezierMoveDirection direction = BezierMoveDirection.Forward)
    {
        if (index < 0 || index >= bezierPath.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Bezier Path의 인덱스를 벗어났습니다.");

        if (runMode != RunMode.Iterative)
        {
            StopAllPath();
            pathQueue.Enqueue((bezierPath[index], direction));
            StartCoroutine(RunIteratively());
        }
        else
        {
            pathQueue.Enqueue((bezierPath[index], direction));
        }
    }

    /// <summary>
    /// 모든 경로와 움직임을 즉시 중단한다.
    /// </summary>
    public void StopAllPath()
    {
        StopAllCoroutines();
        pathQueue.Clear();
        runMode = RunMode.None;
    }

    #endregion

    #region === Private Methods ===

    private IEnumerator RunOnce()
    {
        this.runMode = RunMode.Once;
        while (pathQueue.Count != 0)
        {
            var item = pathQueue.Dequeue();
            yield return BezierMove(item.path, item.direction);
        }
        this.runMode = RunMode.None;
    }

    private IEnumerator RunIteratively()
    {
        this.runMode = RunMode.Iterative;
        while (true)
        {
            if (pathQueue.Count == 0)
                break;

            var item = pathQueue.Dequeue();
            yield return BezierMove(item.path, item.direction);
            pathQueue.Enqueue(item);
        }
        this.runMode = RunMode.None;
    }

    private IEnumerator BezierMove(BezierPath2 path, BezierMoveDirection direction = BezierMoveDirection.Forward)
    {
        var prop = path.property;

        if (prop.Count < 1)
        {
            Debug.LogError($"베지어 경로를 이동시키기 위해서는 반드시 경로에 대한 1개 이상의 Property를 정의해야 합니다.", this);
            yield break;
        }

        int segmentCount = this.segmentCount;
        int currentSegment = 0; // 현재 segment
        float sectionScale = 1f / segmentCount;

        bool zero2one = direction == BezierMoveDirection.Forward;
        float t = zero2one ? 0f : 1f; // weight
        float dir = (float)direction;

        prop = prop.OrderBy(p => dir * p.T).ToList(); // 프로퍼티 정렬
        int propIdx = 0; // 프로퍼티 인덱스
        var initialProp = prop[0];
        var goalProp = prop[0];

        var f = BezierInterpolation(path.point); // 보간된 베지어 함수
        Vector3 position = f(t);
        position.z = transform.position.z;
        transform.position = position;

        float speed = 0f;
        while ((zero2one && t < 1f) || (!zero2one && t > 0f))
        {
            // 프로퍼티 변경
            if (dir * t >= dir * prop[propIdx].T && propIdx + 1 < prop.Count)
            {
                initialProp = goalProp;
                goalProp = prop[++propIdx];
            }

            // 각 segment에서는 linear한 동선을 가짐
            float startWeight = (float)currentSegment / segmentCount; // segment의 시작 t
            float endWeight = startWeight + sectionScale; // segment의 끝 t

            if (!zero2one)
            {
                startWeight = 1 - startWeight;
                endWeight = 1 - endWeight;
            }

            // segment에서의 속도
            speed = initialProp.T != goalProp.T ?
                Lerp(initialProp.Speed, goalProp.Speed, (t - initialProp.T) / (goalProp.T - initialProp.T)) : goalProp.Speed;

            Vector2 start = f(t); // 현재 위치
            Vector2 end = f(endWeight); // segment의 끝 위치
            Vector2 s = end - start; // 변위
            Vector2 v = speed * s.normalized; // 속도
            float time = Dot(v, s) / Pow(speed, 2); // 걸리는 시간
            float segT = 0f;
            while (dir * t < dir * endWeight)
            {
                yield return null;
                t = Clamp01(t + dir * Time.deltaTime / (segmentCount * time));
                segT = Clamp01(segT + Time.deltaTime / time);
                //start = f(t);
                Vector3 next = Lerp(start, end, segT);
                next.z = transform.position.z;
                transform.position = next;
                OnPathMove?.Invoke(path, new PathProperty(t, speed), false);
            }

            currentSegment++;
        }

        OnPathMove?.Invoke(path, new PathProperty(t, speed), true);
    }

    #endregion

    #region === IList interface memebers ===

    public int Count => bezierPath.Count;

    public bool IsReadOnly => false;

    public BezierPath2 this[int index] { get => bezierPath[index]; set => bezierPath[index] = value; }

    public int IndexOf(BezierPath2 item) => this.bezierPath.IndexOf(item);

    public void Insert(int index, BezierPath2 item) => this.bezierPath.Insert(index, item);

    public void RemoveAt(int index) => this.bezierPath.RemoveAt(index);

    public void Add(BezierPath2 item) => this.bezierPath.Add(item);

    public void Clear() => this.bezierPath.Clear();

    public bool Contains(BezierPath2 item) => this.bezierPath.Contains(item);

    public void CopyTo(BezierPath2[] array, int arrayIndex) => this.bezierPath.CopyTo(array, arrayIndex);

    public bool Remove(BezierPath2 item) => this.bezierPath.Remove(item);

    public IEnumerator<BezierPath2> GetEnumerator() => this.bezierPath.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.bezierPath.GetEnumerator();

    #endregion
}

/// <summary>
/// 베지어 곡선 이동 방향
/// </summary>
public enum BezierMoveDirection
{
    /// <summary>
    /// t: 0 -> 1
    /// </summary>
    Forward = 1,

    /// <summary>
    /// t: 1 -> 0
    /// </summary>
    Backward = -1
}

/// <summary>
/// 베지어 곡선 이벤트 핸들러
/// </summary>
/// <param name="path">현재 이동중인 경로</param>
/// <param name="current">현재 속성</param>
/// <param name="isTermiated">곡선의 끝에 도달했는지 여부</param>
public delegate void BezierPath2Handler(BezierPath2 path, PathProperty current, bool isTermiated);

/// <summary>
/// 베지어 곡선을 정의
/// </summary>
[Serializable, MoveToolAvailable]
public class BezierPath2
{
    /// <summary>
    /// 베지어 곡선 보간을 위한 점들
    /// </summary>
    public List<Vector2> point;

    /// <summary>
    /// 베지어 곡선의 보간된 지점에서의 속성
    /// </summary>
    public List<PathProperty> property;

    public BezierPath2()
    {
        this.point = new List<Vector2>();
        this.property = new List<PathProperty>();
    }

    public BezierPath2(IEnumerable<Vector2> points, IEnumerable<PathProperty> properties)
    {
        this.point = points.ToList();
        this.property = properties.ToList();
    }
}

/// <summary>
/// 경로 속성
/// </summary>
[Serializable]
public struct PathProperty
{
    [Range(0f, 1f), SerializeField] private float t;

    [Min(0f), SerializeField]
    private float speed;

    /// <summary>
    /// 보간 비중(0 ~ 1)
    /// </summary>
    public float T { get => t; set => Clamp01(value); }

    /// <summary>
    /// t에서의 속도
    /// </summary>
    public float Speed { get => speed; set => Max(0f, value); }

    public PathProperty(float t, float speed)
    {
        this.t = Clamp01(t);
        this.speed = Max(0f, speed);
    }
}