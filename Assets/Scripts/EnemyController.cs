using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public GameObject target;

    [System.Serializable]
    public class Properties
    {
        [Range(0.1f, 0.9f)]
        public float SNR = 0.8f;
        [Range(0.5f, 2.0f)]
        public float cycleTime = 1.0f;
        [Range(10.0f, 20.0f)]
        public float lifetime = 10.0f;
        public float speed = 1.0f;
    }
    [SerializeField]
    public Properties properties;

    private struct States
    {
        public bool destructing;
    }
    private States states;

    private bool chase = true;
    private float elapsed;
    private Animator animator;

	void Start ()
    {
        if (!target)
            target = GameObject.Find("Player");

        animator = GetComponentInChildren<Animator>();

        FuzzProperties();
        elapsed = 0;
        InitStates();
	}

    void FuzzProperties ()
    {
        float noise = 1.0f - properties.SNR;
        properties.cycleTime = FuzzProperty(properties.cycleTime);
        properties.lifetime = FuzzProperty(properties.lifetime);
        properties.speed = FuzzProperty(properties.speed);
    }

    float FuzzProperty (float property, float SNR=-1)
    {
        if (SNR == -1)
            SNR = properties.SNR;
        return Random.Range(property * SNR, property * (1 + (1 - SNR)));
    }

    void InitStates ()
    {
        states.destructing = false;
    }
	
	void Update ()
    {
        // Killswitch
        elapsed += Time.deltaTime;
        if (!states.destructing && elapsed >= properties.lifetime)
        {
            Debug.Log("destructing");
            StartCoroutine(SelfDestruct());
            states.destructing = true;
        }


	}

    IEnumerator SelfDestruct ()
    {
        animator.SetTrigger("self_destruct");
        yield return new WaitForSeconds(1.0f);
        Destroy(this.gameObject);
    }
}
