using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public GameObject target;

    [System.Serializable]
    public class Properties
    {
        public float xMin, xMax, yMin, yMax;
        [Range(0.1f, 0.9f)]
        public float SNR = 0.8f;
        [Range(0.5f, 2.0f)]
        public float cycleTime = 1.0f;
        public float speed = 1.0f;
    }
    [SerializeField]
    public Properties properties;

    public struct Triggers
    {
        public bool destruct, freeze;
    }
    public Triggers triggers;

    private enum States { chasing, wandering, destructing, frozen };
    private States state;
    private Vector3 deltaPosition;
    private float cycleCounter;
    private Animator animator;

    // = chase or roam
    private Statics.Void Behaviour;

	void Start ()
    {

        animator = GetComponentInChildren<Animator>();
        int roll = Random.Range(0, 2);
        GetComponentInChildren<SpriteRenderer>().flipX = (roll == 0) ? false : true;

        FuzzProperties();
        cycleCounter = 0;
        state = States.wandering;
        ResetTriggers();
	}

    void ResetTriggers ()
    {
        triggers.destruct = false;
    }

    void FuzzProperties ()
    {
        properties.cycleTime = FuzzProperty(properties.cycleTime);
        properties.speed = properties.speed * ((float)(GameManager.Level + 1) / 5) + 1;
        properties.speed = Mathf.Clamp(properties.speed, 0.2f, 4.0f);
    }

    float FuzzProperty (float property, float SNR=-1)
    {
        if (SNR == -1)
            SNR = properties.SNR;
        return Random.Range(property * SNR, property * (1 + (1 - SNR)));
    }
	
	void Update ()
    {
        if (!target)
            target = GameObject.Find("Player");

        // Timers
        cycleCounter += Time.deltaTime;
        
        if (triggers.destruct)
        { 
            StartCoroutine(SelfDestruct());
            state = States.destructing;
            ResetTriggers();
        }

        if (triggers.freeze)
        {
            state = States.frozen;
            return;
        }

        // State switcher
        if (cycleCounter >= properties.cycleTime)
        {
            if (state == States.wandering)
                state = States.chasing;
            else if (state == States.chasing)
            {

                state = States.wandering;
            }
            cycleCounter = 0;
        }

        // State executor
        switch (state)
        {
            case States.chasing:
                Behaviour = Chase;
                break;
            default:
                Behaviour = Wander;
                break;
        }

        if (Behaviour != null && target != null)
            Behaviour();
	}

    void ClampPosition ()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, properties.xMin, properties.xMax),
            Mathf.Clamp(transform.position.y, properties.yMin, properties.yMax),
            0.0f
            );
    }

    void Chase()
    {
        Vector3 toPlayer = (target.transform.position - transform.position).normalized;
        Vector3 previousPosition = transform.position;
        transform.position += toPlayer * properties.speed * Time.deltaTime * 5;
        ClampPosition();
        deltaPosition = transform.position - previousPosition;
    }

    void Wander()
    {
        //transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        transform.position = transform.position + deltaPosition.normalized * properties.speed * Time.deltaTime * 5;
        ClampPosition();
    }

    IEnumerator SelfDestruct ()
    {
        animator.SetTrigger("self_destruct");
        yield return new WaitForSeconds(1.0f);
        Destroy(this.gameObject);
    }
}
