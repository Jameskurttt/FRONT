using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
   
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;  
    public float sightRange = 15f;
    public float attackRange = 10f;

    public float walkPointRange = 10f;

    private Vector3 walkPoint;
    private bool walkPointSet;

    private bool playerInSightRange;
    private bool playerInAttackRange;

    private EnemyShoot shooter;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        shooter = GetComponent<EnemyShoot>();
    }

    private void Update()
    {
        // check ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patroling();
        else if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        else if (playerInSightRange && playerInAttackRange)
            AttackPlayer();
    }

    private void Patroling()
    {
        shooter.StopShooting(); // stop shooting while patrolling

        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        if (Vector3.Distance(transform.position, walkPoint) < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Pick random X/Z in a square around the enemy
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        // Create candidate point
        Vector3 candidatePoint = new Vector3(randomX, transform.position.y, randomZ);

        // Make sure it's on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidatePoint, out hit, 2f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
        else
        {
            walkPointSet = false; // try again next frame
        }
    }   

    private void ChasePlayer()
    {
        shooter.StopShooting(); // stop shooting while chasing
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // optionally stop moving to shoot
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange * 0.9f)
            agent.SetDestination(player.position);
        else
            agent.SetDestination(transform.position);

        // call shooting script
        shooter.ShootAt(player);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}