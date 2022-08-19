using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CommunistWall : Entity
{
    public Transform player;
    public bool canKillYou = true;
    private NavMeshAgent _navMeshAgent;
    
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to" + gameObject.name);
        }
    
    }

    private void Update()
    {
        _navMeshAgent.SetDestination(player.position);
        _navMeshAgent.speed = baseSpeed;

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!canKillYou)
                canKillYou = true;
            else
                canKillYou = false;
        }
    }


    public override void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerCol = other.GetComponent<Player>();

        if (playerCol != null && canKillYou)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
