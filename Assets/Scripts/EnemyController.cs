using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _enemyAgent;
    private Transform _player;
    private EnemyState _currentState;
    [SerializeField] float _detectionRange;
    [SerializeField] float _attackRange;
    [SerializeField] Transform[] _patrolPoints;
    [SerializeField] private int _patrolIndex;
    private float _distanceToPlayer;
    private float timer;
    private float Cooldown = 1;

    void Awake()
    {
        _enemyAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }
    public enum EnemyState
    {
        Patrolling,

        Chasing,

        Attacking
    }
    void Update()
    {
        switch(_currentState)
        {
            case EnemyState.Patrolling:
                Patrolling();
            break;

            case EnemyState.Chasing:
                Chasing();
            break;

            case EnemyState.Attacking:
                Attacking();
            break;

            default:
                Patrolling();
            break;
        }
        Debug.Log(_currentState);
    }
    bool OnRange()
    {
        _distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if(_distanceToPlayer <= _detectionRange)
        {
            return true;
        }
        else if(_distanceToPlayer <= _attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void Patrolling()
    {
        if(OnRange())
        {
            _currentState = EnemyState.Chasing;
        }
        else if(_enemyAgent.remainingDistance <= 1)
        {
            PatrolInOrder();
        }
    }
    void PatrolInOrder()
    {
        _enemyAgent.SetDestination(_patrolPoints[_patrolIndex].position);
        _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
    }
    void Chasing()
    {
        if(OnRange())
        {
            _currentState = EnemyState.Attacking;
        }
        else if(!OnRange())
        {
            _currentState = EnemyState.Patrolling;
        }
        _enemyAgent.SetDestination(_player.position);
    }
    void Attacking()
    {
        if(!OnRange())
        {
            _currentState = EnemyState.Chasing;
        }
        timer += Time.deltaTime;
        if(timer >= Cooldown)
        {
            Debug.Log("Ataque");
            timer = 0;
        }
        _currentState = EnemyState.Chasing;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

}
