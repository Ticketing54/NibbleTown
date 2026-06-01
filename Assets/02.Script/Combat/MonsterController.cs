using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterController : MonoBehaviour, IDamageable, IHasHP
{
    public enum State { MoveToBuilding, AttackBuilding, ChasePlayer, AttackPlayer, Dead }

    [Header("Stats")]
    [SerializeField] private int   maxHP        = 50;
    [SerializeField] private int   attackDamage = 10;
    [SerializeField] private float attackRange  = 2.5f;
    [SerializeField] private int   dropGold     = 10;
    [SerializeField] private string monsterName;

    [Header("원거리 공격")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform  firePoint;

    [Header("감지")]
    [SerializeField]                  private float     sightRange      = 10f;
    [SerializeField, Range(0f, 360f)] private float     sightAngle      = 120f;
    [SerializeField]                  private LayerMask sightBlockLayer;

    public bool   IsDead       => currentHP <= 0;
    public int    CurrentHP    => currentHP;
    public int    MaxHP        => maxHP;
    public string MonsterName  => monsterName;
    public State  CurrentState => currentState;

    public event Action<State> OnStateChanged;
    public event Action        OnHit;
    public event Action        OnDied;

    private State          currentState;
    private int            currentHP;
    private float          sightDelay;
    private IAttackBehavior attackBehavior;
    private float          navPosRefreshTimer;
    private NavMeshAgent   agent;
    private BuildingHealth targetBuilding;
    private Vector3        buildingNavPos;
    private CharacterStat  targetPlayer;

    private void Awake()
    {
        agent                       = GetComponent<NavMeshAgent>();
        agent.autoBraking           = false;
        agent.stoppingDistance      = 0f;
        agent.acceleration          = 50f;
        agent.angularSpeed          = 9999f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        currentHP                   = maxHP;
        attackBehavior              = GetComponentInChildren<IAttackBehavior>();
    }

    private void Start()
    {
        sightDelay = 1f;
        TransitionTo(State.MoveToBuilding);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.MoveToBuilding: UpdateMoveToBuilding(); break;
            case State.AttackBuilding: UpdateAttackBuilding(); break;
            case State.ChasePlayer:    UpdateChasePlayer();    break;
            case State.AttackPlayer:   UpdateAttackPlayer();   break;
        }
    }

    // ── State Updates ────────────────────────────────────────────────────────

    private void UpdateMoveToBuilding()
    {
        CharacterStat spotted = FindVisiblePlayer();
        if (spotted != null) { TransitionToChase(spotted); return; }

        if (targetBuilding == null || targetBuilding.IsDead)
        {
            SetTargetBuilding(FindNearestBuilding());
            if (targetBuilding == null) { agent.ResetPath(); return; }
        }

        // 접근 방향이 바뀌므로 주기적으로 NavMesh 도달 지점 갱신
        navPosRefreshTimer -= Time.deltaTime;
        if (navPosRefreshTimer <= 0f)
            RefreshBuildingNavPos();

        float dist = Vector3.Distance(transform.position, buildingNavPos);
        if (dist <= attackRange ||
            (agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathPartial && dist < attackRange * 3f))
        {
            TransitionTo(State.AttackBuilding);
            return;
        }

        agent.SetDestination(buildingNavPos);
    }

    private void UpdateAttackBuilding()
    {
        CharacterStat spotted = FindVisiblePlayer();
        if (spotted != null) { TransitionToChase(spotted); return; }

        if (targetBuilding == null || targetBuilding.IsDead)
        {
            TransitionTo(State.MoveToBuilding);
            return;
        }

        float dist = Vector3.Distance(transform.position, buildingNavPos);
        if (dist > attackRange) { TransitionTo(State.MoveToBuilding); return; }

        FaceTarget(targetBuilding.transform);
    }

    private void UpdateChasePlayer()
    {
        if (IsTargetPlayerGone()) { TransitionTo(State.MoveToBuilding); return; }

        float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);
        if (dist <= attackRange) { TransitionTo(State.AttackPlayer); return; }

        agent.SetDestination(targetPlayer.transform.position);
    }

    private void UpdateAttackPlayer()
    {
        if (IsTargetPlayerGone()) { TransitionTo(State.MoveToBuilding); return; }

        float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);
        if (dist > attackRange) { TransitionTo(State.ChasePlayer); return; }

        FaceTarget(targetPlayer.transform);
    }

    // ── Transitions ──────────────────────────────────────────────────────────

    private void TransitionTo(State next)
    {
        currentState   = next;
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        if (next == State.MoveToBuilding)
        {
            targetPlayer = null;
            SetTargetBuilding(FindNearestBuilding());
        }

        OnStateChanged?.Invoke(next);
    }

    private void TransitionToChase(CharacterStat _player)
    {
        targetPlayer   = _player;
        currentState   = State.ChasePlayer;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        OnStateChanged?.Invoke(State.ChasePlayer);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void SetTargetBuilding(BuildingHealth _building)
    {
        targetBuilding     = _building;
        navPosRefreshTimer = 0f;
        if (_building == null) return;
        RefreshBuildingNavPos();
    }

    // 몬스터의 현재 위치 기준으로 건물 가장자리 NavMesh 지점을 계산
    // 건물 내부엔 NavMesh가 없으므로, 몬스터 쪽 방향에서 샘플링해 가장 가까운 경로 확보
    private void RefreshBuildingNavPos()
    {
        navPosRefreshTimer = 0.5f;
        if (targetBuilding == null) return;

        Vector3 center     = targetBuilding.transform.position;
        Vector3 dir        = (center - transform.position).normalized;
        Vector3 sampleFrom = center - dir * 8f;

        if (NavMesh.SamplePosition(sampleFrom, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            buildingNavPos = hit.position;
        else if (NavMesh.SamplePosition(center, out hit, 20f, NavMesh.AllAreas))
            buildingNavPos = hit.position;
        else
            buildingNavPos = center;
    }

    private bool IsTargetPlayerGone() =>
        targetPlayer == null || targetPlayer.IsDead;

    private CharacterStat FindVisiblePlayer()
    {
        if (sightDelay > 0f) { sightDelay -= Time.deltaTime; return null; }

        foreach (CharacterStat p in PlayerRegistry.All)
        {
            if (p == null || p.IsDead) continue;
            if (CanSee(p.transform)) return p;
        }
        return null;
    }

    private bool CanSee(Transform _target)
    {
        Vector3 dir  = _target.position - transform.position;
        float   dist = dir.magnitude;

        if (dist > sightRange) return false;
        if (Vector3.Angle(transform.forward, dir) > sightAngle * 0.5f) return false;
        if (sightBlockLayer != 0 &&
            Physics.Raycast(transform.position + Vector3.up, dir.normalized, dist, sightBlockLayer))
            return false;

        return true;
    }

    // MonsterAnimatorDriver가 애니메이션 타이밍에 맞춰 호출
    public void LaunchAttack()
    {
        if (attackBehavior != null) { attackBehavior.Attack(attackDamage, gameObject); return; }
        FireProjectile();
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;
        Vector3    spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up;
        GameObject go       = Instantiate(projectilePrefab, spawnPos, transform.rotation);
        if (go.TryGetComponent(out Projectile proj))
            proj.Init(attackDamage, gameObject, transform.forward);
    }

    private BuildingHealth FindNearestBuilding()
    {
        BuildingHealth nearest = null;
        float          minDist = float.MaxValue;

        foreach (BuildingHealth b in BuildingHealth.All)
        {
            if (b == null || b.IsDead) continue;
            float d = Vector3.Distance(transform.position, b.transform.position);
            if (d < minDist) { minDist = d; nearest = b; }
        }

        return nearest;
    }

    public void TakeDamage(DamageInfo _info)
    {
        if (IsDead) return;

        if (PlayerRegistry.IsPlayer(_info.dealer) &&
            currentState != State.ChasePlayer && currentState != State.AttackPlayer)
        {
            CharacterStat attacker = PlayerRegistry.GetNearest(transform.position);
            if (attacker != null) TransitionToChase(attacker);
        }

        currentHP = Mathf.Max(0, currentHP - _info.amount);
        GameHUDManager.Instance?.SpawnDamageNumber(_info.amount, transform.position + Vector3.up, _info.isCrit);

        if (IsDead) { Die(); return; }

        OnHit?.Invoke();
    }

    public void Init(MonsterData _data)
    {
        monsterName = _data.monsterName;
        dropGold    = _data.dropGold;
        GameHUDManager.Instance?.Register(this, transform, _data.monsterName);
    }

    private void Die()
    {
        currentState  = State.Dead;
        agent.ResetPath();
        agent.enabled = false;
        OnDied?.Invoke();
        GameEvents.RaiseMonsterDied(dropGold);
        Destroy(gameObject, 1f);
    }

    private void FaceTarget(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(dir), Time.deltaTime * 10f);
    }

    private void OnDrawGizmosSelected()
    {
        bool isAggro = currentState == State.ChasePlayer || currentState == State.AttackPlayer;
        Gizmos.color = isAggro ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 leftDir  = Quaternion.Euler(0, -sightAngle * 0.5f, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0,  sightAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftDir  * sightRange);
        Gizmos.DrawRay(transform.position, rightDir * sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
