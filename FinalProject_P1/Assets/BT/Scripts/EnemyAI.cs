using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private int startingHealth;
    [SerializeField] private float lowHealthThreshold;
    [SerializeField] private float healthRestoreRate;

    [SerializeField] private float chasingRange;
    [SerializeField] private float shootingRange;

  
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Cover[] avaliableCovers;

    private Vector3 lastPos;


    private Animator anim;
    private Rigidbody[] bodyParts;

    private Material material;
    private Transform bestCoverSpot;
    private NavMeshAgent agent;

    private Node topNode;

    [SerializeField]private int _currentHealth;
	public int currentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = Mathf.Clamp(value, 0, startingHealth); }
    }

    [SerializeField] private int armor;
    public int Armor { get => armor; set => armor = value; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        material = GetComponentInChildren<MeshRenderer>().material;

        //-----------------//
        weaponsHolster = transform.Find("Weapons");
        weapons = weaponsHolster.GetComponentsInChildren<Shooter>();
        if (weapons.Length > 0)
            equip(activeWeaponIndex);
        //-----------------//
    }

    private void Start()
    {
        armor = startingHealth;
        bodyParts = transform.GetComponentsInChildren<Rigidbody>();
        EnableRagdoll(false);
        anim = GetComponentInChildren<Animator>();
        _currentHealth = startingHealth;
        
        ConstructBehahaviourTree();
    }

    private void ConstructBehahaviourTree()
    {
        IsCovereAvaliableNode coverAvaliableNode = new IsCovereAvaliableNode(avaliableCovers, playerTransform, this);
        GoToCoverNode goToCoverNode = new GoToCoverNode(agent, this);
        HealthNode healthNode = new HealthNode(this, lowHealthThreshold);
        IsCoveredNode isCoveredNode = new IsCoveredNode(playerTransform, transform);
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, this);
        RangeNode chasingRangeNode = new RangeNode(chasingRange, playerTransform, transform);
        RangeNode shootingRangeNode = new RangeNode(shootingRange, playerTransform, transform);
        ShootNode shootNode = new ShootNode(agent, this, playerTransform, activeWeapon);

        Sequence chaseSequence = new Sequence(new List<Node> { chasingRangeNode, chaseNode });
        Sequence shootSequence = new Sequence(new List<Node> { shootingRangeNode, shootNode });

        Sequence goToCoverSequence = new Sequence(new List<Node> { coverAvaliableNode, goToCoverNode });
        Selector findCoverSelector = new Selector(new List<Node> { goToCoverSequence, chaseSequence });
        Selector tryToTakeCoverSelector = new Selector(new List<Node> { isCoveredNode, findCoverSelector });
        Sequence mainCoverSequence = new Sequence(new List<Node> { healthNode, tryToTakeCoverSelector });

        topNode = new Selector(new List<Node> { mainCoverSequence, shootSequence, chaseSequence });


    }
    public Animator playerAnim() {
        return this.anim;
    }

    private void Update()
    {
        if (currentHealth < 1)
        {
            EnableRagdoll(true);
            anim.enabled = false;
            topNode = null;
            agent.enabled = false;
        }
        else
        {
            if (agent.isStopped)
            {
                //anim.applyRootMotion = true;
                playerAnim().SetBool("isRunning", false);
                playerAnim().SetFloat("Vertical", 0.0f);
            }
            else
            {
                //anim.applyRootMotion = false;
                float velocity = (transform.position - lastPos).magnitude / Time.deltaTime;
                lastPos = transform.position;
                playerAnim().SetBool("isRunning", true);
                playerAnim().SetFloat("Vertical", velocity / agent.speed);
            }

            topNode.Evaluate();
            if (topNode.nodeState == NodeState.FAILURE)
            {
                SetColor(Color.red);
                agent.isStopped = true;
            }
            //currentHealth += Time.deltaTime * healthRestoreRate;
        }
    }


    public void TakeDamage(int damage)
    {
        if (armor > 0)
        {
            if (armor < 10)
            {
                armor = 0;
            }
            else
                armor -= damage;
        }
        else
        {
            if (currentHealth < 10)
            {
                currentHealth = 0;
            }
            else
                currentHealth -= damage;
        }
    }

    public void SetColor(Color color)
    {
        material.color = color;
    }

    public void SetBestCoverSpot(Transform bestCoverSpot)
    {
        this.bestCoverSpot = bestCoverSpot;
    }

    public Transform GetBestCoverSpot()
    {
        return bestCoverSpot;
    }

    //----------------------------------------------------------//
    [SerializeField] float timeToSwichWeapon;
    Shooter[] weapons;
    Shooter activeWeapon;
    int activeWeaponIndex = 0;
    bool canFire;
    Transform weaponsHolster;
    public event System.Action<Shooter> OnWeaponSwich;

    public Shooter ActiveWeapon
    {
        get
        {
            return activeWeapon;
        }
    }

    /*void Awake()
    {
        weaponsHolster = transform.Find("Weapons");
        weapons = weaponsHolster.GetComponentsInChildren<Shooter>();
        if (weapons.Length > 0)
            equip(activeWeaponIndex);
    }*/

    void SwichWeapon(int direction)
    {
        canFire = false;
        activeWeaponIndex += direction;
        if (activeWeaponIndex > weapons.Length - 1)
            activeWeaponIndex = 0;
        else if (activeWeaponIndex < 0)
            activeWeaponIndex = weapons.Length - 1;
        GameManager.GetInstance().GetTimer().add(() => {
            equip(activeWeaponIndex);
        }, timeToSwichWeapon);
    }

    void deactivateWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].transform.SetParent(weaponsHolster);
            weapons[i].gameObject.SetActive(false);
        }
    }

    void equip(int index)
    {
        canFire = true;
        activeWeapon = weapons[index];
        //deactivateWeapons();
        activeWeapon.gameObject.SetActive(true);
        activeWeapon.equip();
        if (OnWeaponSwich != null)
            OnWeaponSwich(activeWeapon);
    }
    //----------------------------------------------------------//
    void EnableRagdoll(bool value)
    {
        print("EnableRagDoll");
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].isKinematic = !value;
        }
    }
}
