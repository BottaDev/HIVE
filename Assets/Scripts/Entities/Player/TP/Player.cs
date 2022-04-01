using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [Header("Assignables")]
    [SerializeField] private PlayerInput input;
    public bool restart { get { return input.restart; } }

    [SerializeField] private HealthBar _healthBar;
    
    protected override void Awake()
    {
        base.Awake();
        
        EventManager.Instance?.Subscribe("OnPlayerDamaged", OnPlayerDamaged);
    }

    private void Start()
    {
        _healthBar.SetMaxHealt(maxHealth);
    }

    private void Update()
    {
        if (restart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnPlayerDamaged(params object[] parameters)
    {
        TakeDamage((float)parameters[0]);
    }

    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        EventManager.Instance.Trigger("OnLifeUpdated", CurrentHealth);

        if (CurrentHealth <= 0)
        {
            EventManager.Instance.Trigger("OnPlayerDead");
            EventManager.Instance.Unsubscribe("OnPlayerDamaged", OnPlayerDamaged);
            gameObject.SetActive(false);
        }
    }
}
