using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    static int Count;
    public float Life { get; private set; }
    public bool IsAlive { get => Life > 0; }

    [Header("Hit Marker")]
    public GameObject hitMarkerPrefab;

    protected Player player;
    protected CharacterController characterController;

    [Header("Living Parameters")]
    [SerializeField]
    protected float AttackDistance = 2;
    [SerializeField]
    private float maxLife = 5;

    [Header("Dying Parameters")]
    [SerializeField]
    private float yDelta = 2;
    [SerializeField]
    private float waitTime = 1;
    [SerializeField]
    private float timeOfDying = 1;


    abstract public void Attack();

    // Start is called before the first frame update
    virtual public void Awake()
    {
        Count++;
        Life = maxLife;
        player = FindObjectOfType<Player>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    virtual public void Update()
    {
        if(IsPlayerInDistance(Vector3.zero))
        {
            Attack();
        }
    }

    virtual public void TakeDamage(float damage, Vector3 position)
    {
        Life -= damage;
        Instantiate(hitMarkerPrefab, position, Quaternion.identity, transform);
        if (!IsAlive)
        {
            Die();
        }
    }

    virtual public void Die()
    {
        StartCoroutine(Dying());
    }


    public bool IsPlayerInDistance(Vector3 offset)
    {
        if(player == null || player.enabled == false)
        {
            return false;
        }
        return Vector3.Distance(player.transform.position, transform.position + offset) < AttackDistance;
    }

    private IEnumerator Dying()
    {
        GetComponent<CharacterController>().enabled = false;
        var timeRemain = timeOfDying;
        yield return new WaitForSeconds(waitTime);
        while((timeRemain -= Time.deltaTime) > 0)
        {
            transform.Translate(yDelta * Vector3.down * (Time.deltaTime / timeOfDying), Space.World);
            yield return null;
        }
        Destroy(gameObject);
        yield break;
    }
}
