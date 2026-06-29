using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterController : MonoBehaviour, IDamageable, IHasHP
{
    public enum State { MoveToBuilding, AttackBuilding, ChasePlayer, AttackPlayer, Dead }

    [SerializeField] private int monsterIndex = 0;

    private int       maxHP          = 50;
    private int       attackDamage   = 10;
    private float     attackRange    = 2.5f;
    private float     attackInterval = 1.5f;
    private int       dropGold       = 10;
    private string    monsterName;
    private DropTable dropTable;

    public int MonsterIndex => monsterIndex;

    [Header("감지")]
    [SerializeField]                  private float     sightRange      = 10f;
    [SerializeField, Range(0f, 360f)] private float     sightAngle      = 120f;
    [SerializeField]                  private LayerMask sightBlockLayer;

    public bool   IsDead        => currentHP <= 0;
    public int    CurrentHP    => currentHP;
    public int    MaxHP        => maxHP;
    public string MonsterName  => monsterName;
    public State  CurrentState => currentState;
    public bool      IsAttackReady  => attackCooldownTimer <= 0f;
    public Transform CurrentTarget  => targetPlayer != null ? targetPlayer.transform : null;
    public float     AttackRange    => attackRange;

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
    private Collider       buildingCollider;
    private Vector3        buildingNavPos;
    private CharacterStat  targetPlayer;
    private float          attackCooldownTimer;

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
        if (attackCooldownTimer > 0f) attackCooldownTimer -= Time.deltaTime;

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

        bool inRange     = GetBuildingEdgeDist() <= attackRange;
        bool pathBlocked = agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathPartial;

        if (inRange || pathBlocked)
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

        bool outOfRange  = GetBuildingEdgeDist() > attackRange;
        bool pathBlocked = agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathPartial;

        if (outOfRange && !pathBlocked) { TransitionTo(State.MoveToBuilding); return; }

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
        buildingCollider   = _building != null ? _building.GetComponent<Collider>() : null;
        navPosRefreshTimer = 0f;
        if (_building == null) return;
        RefreshBuildingNavPos();
    }

    // 건물 표면에서 attackRange만큼 떨어진 지점을 NavMesh 위에서 샘플링
    // 몬스터 위치 기준으로 계산하므로 방향이 달라도 각자 다른 접근 지점을 가짐
    private void RefreshBuildingNavPos()
    {
        navPosRefreshTimer = 0.5f;
        if (targetBuilding == null) return;

        Vector3 edgePoint  = GetBuildingClosestPoint();
        Vector3 dir        = (transform.position - edgePoint).normalized;
        Vector3 sampleFrom = edgePoint + dir * attackRange * 0.8f;

        if (NavMesh.SamplePosition(sampleFrom, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            buildingNavPos = hit.position;
        else if (NavMesh.SamplePosition(edgePoint, out hit, 10f, NavMesh.AllAreas))
            buildingNavPos = hit.position;
        else
            buildingNavPos = edgePoint;
    }

    private Vector3 GetBuildingClosestPoint()
    {
        if (targetBuilding == null) return Vector3.zero;
        return buildingCollider != null
            ? buildingCollider.ClosestPoint(transform.position)
            : targetBuilding.transform.position;
    }

    private float GetBuildingEdgeDist() =>
        Vector3.Distance(transform.position, GetBuildingClosestPoint());

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
        if (attackCooldownTimer > 0f) return;
        attackCooldownTimer = attackInterval;

        if (attackBehavior != null) { attackBehavior.Attack(attackDamage, gameObject); return; }
        MeleeAttack();
    }

    private void MeleeAttack()
    {
        if (currentState != State.AttackPlayer || targetPlayer == null) return;
        float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);
        if (dist > attackRange) return;
        targetPlayer.TakeDamage(new DamageInfo(gameObject, attackDamage));
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
        GameEvents.RaiseHUDShow(this);
        GameEvents.RaiseDamageNumber(_info.amount, transform.position + Vector3.up, _info.isCrit);

        if (IsDead) { Die(); return; }

        OnHit?.Invoke();
    }

    public void Init(MonsterData _data, DropTable _stageDropTable)
    {
        monsterName    = _data.monsterName;
        maxHP          = _data.maxHP;
        attackDamage   = _data.attackDamage;
        attackRange    = _data.attackRange;
        attackInterval = _data.attackInterval;
        dropGold       = _data.dropGold;
        dropTable      = _stageDropTable;
        currentHP      = maxHP;
        GameEvents.RaiseHUDRegister(this, transform, _data.monsterName);
    }

    private void Die()
    {
        currentState  = State.Dead;
        agent.ResetPath();
        agent.enabled = false;
        OnDied?.Invoke();
        ResourceSpawnManager.Instance?.SpawnGoldDrop(dropGold, transform.position);
        ResourceSpawnManager.Instance?.SpawnItemDrop(dropTable, transform.position);
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
