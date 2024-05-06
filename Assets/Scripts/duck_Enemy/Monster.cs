using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private Vector3 _startPosition;
    
    [SerializeField] 
    private float findDistance = 5f;
    [SerializeField] 
    private LayerMask m_layerMask = 0;


    [SerializeField] 
    private float monsterHp = 20f;

    private bool _isDamaged = false;

    [SerializeField] 
    private float attackDistance = 2f;
    [SerializeField]
    private float moveSpeed = 8f;

    private float _currentTime = 0f;
    [SerializeField] 
    private float attackDelay = 2f;
    [SerializeField] 
    private float attackPower = 5f;
    private Transform _player;

    [SerializeField] 
    private Animator anim;

    public bool arrive = true;
    private Vector3 _randomPosition;
    private Vector3 distan;
    private NavMeshAgent _navMeshA;

    MonsterState m_State;

    [SerializeField] private bool isMovingMonster = true;
    [SerializeField] private List<Vector3> moveDirectionList;
    private int _moveDirectionIndex = 0;


    enum MonsterState
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }

    // Start is called before the first frame update
    void Start()
    {
        _navMeshA = GetComponent<NavMeshAgent>();
        _startPosition = transform.position;
        _navMeshA.SetDestination(moveDirectionList[_moveDirectionIndex++]);
        m_State = MonsterState.Idle;
        _player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case MonsterState.Damaged:
                return;
            case MonsterState.Idle:
                Idle();
                break;
            case MonsterState.Move:
                Move();
                break;
            case MonsterState.Attack:
                Attack();
                break;
            case MonsterState.Die:
                Die();
                break;
        }
    }
  
    void Idle()
    {
        if (isMovingMonster && _navMeshA.remainingDistance <= 0.2f)
        {
            _navMeshA.SetDestination(moveDirectionList[_moveDirectionIndex]);
            _moveDirectionIndex = (_moveDirectionIndex + 1) % moveDirectionList.Count;
            Debug.Log(_moveDirectionIndex);
        }
        
        //시야 적용 방식
        float fov = 90f; // 시야 각
        
        for (float angle = -fov / 2; angle <= fov / 2; angle += 5f) // 5도 간격으로 레이캐스트 발사함
        {
            
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            Debug.DrawRay(transform.position, direction * findDistance, Color.red);
        
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, findDistance))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    m_State = MonsterState.Move;
                    Debug.Log("추적");
                }
            }
        }
    }

    void Move()
    {
        float SearchDistance = _isDamaged ? findDistance * 3 : findDistance * 2;
        if (Vector3.Distance(_player.position, transform.position) > SearchDistance)
        {
            Debug.Log("d?" + SearchDistance);
            m_State = MonsterState.Idle;
            _navMeshA.SetDestination(_startPosition);
            _isDamaged = false;
            arrive = true;
        }
        else if (Vector3.Distance(_player.position, transform.position) > attackDistance)
        {
            //Vector3 dir = (_player.position - transform.position);
            // dir = new Vector3(dir.x, 0, dir.z).normalized;
            // transform.position += dir * moveSpeed * Time.deltaTime;
            //transform.forward = dir;
            _navMeshA.isStopped = true;
            _navMeshA.ResetPath();
            _navMeshA.stoppingDistance = attackDistance;
            // 내비게이션의 목적지를 플레이어의 위치로 설정한다.
            _navMeshA.destination = _player.position;
        }
        else
        {
            m_State = MonsterState.Attack;
            _currentTime = attackDelay;
        }
    }

    void Attack()
    {
        if (Vector3.Distance(_player.position, transform.position) < attackDistance)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > attackDelay)
            {
                Debug.Log("공격");
                _currentTime = 0;
            }
        }
        else
        {
            m_State = MonsterState.Move;
            _currentTime = 0;
        }
    }

    public void HitMonster(int hitPower)
    {
        if (m_State == MonsterState.Damaged || m_State == MonsterState.Die) return;
        if (monsterHp > 0)
        {
            monsterHp -= hitPower;
            m_State = MonsterState.Damaged;
            _isDamaged = true;
            StartCoroutine(WaitDamage());
        }
    }

    IEnumerator WaitDamage()
    {
        yield return new WaitForSeconds(1f);
        m_State = MonsterState.Move;
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}