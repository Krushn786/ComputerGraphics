using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(MoveController))]
[RequireComponent(typeof(PlayerState))]
public class Player : MonoBehaviour {
    // This was Test [E #3] InputController inputcontroller;
    [System.Serializable]
    //Add some Sensitivity
    public class MouseInput {
        public Vector2 Damping;
        public Vector2 Sensitivity;        
    }
    private Animator anim;
    private NavMeshAgent agent;

    public MoveController m_MoveController;
    public MoveController MoveController
    {
        get
        {
            if (m_MoveController == null)
            {
                m_MoveController = new MoveController();
                m_MoveController.GetComponent<MoveController>();
            }
            return m_MoveController;
        }
    }
    [SerializeField] private Text armorHealthText;
    [SerializeField] private Text currentHealthText;
    private Rigidbody[] bodyParts;
    private Crosshair m_Crosshair;
    private Crosshair Crosshair {
        get {
            if (m_Crosshair == null) 
                m_Crosshair = GetComponentInChildren<Crosshair>();
            
            return m_Crosshair;
        }
    }

    private PlayerState playerState;
    public PlayerState PlayerState
    {
        get
        {
            if (playerState == null)
                playerState = GetComponentInChildren<PlayerState>();
            return playerState;
        }
    }
    [SerializeField] private int armor;

    [SerializeField]private int playerHealth;

    public int PlayerHealth { get => playerHealth; set => playerHealth = value; }
    public int Armor { get => armor; set => armor = value; }

    [SerializeField] GameObject bodyArmor;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] MouseInput MouseControl;
    [SerializeField] AudioController footSteps;
    [SerializeField] float minimumMovementToPlayFootstep;
    [SerializeField] float walkingDelayBetweenClips;
    [SerializeField] float RunningDelayBetweenClips;

	public PlayerAim playerAim;

    InputController playerInput;
    Vector2 mouseInput;
    Vector3 prevPosition; //to implement movement sound
	// Use this for initialization
	void Awake () {
        // This was Test [E #3] inputcontroller = GameManager.Instance.InputController;
        playerInput = GameManager.GetInstance().GetInputController();
        GameManager.GetInstance().LocalPlayer = this;
        prevPosition = transform.position;
    }


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        bodyParts = transform.GetComponentsInChildren<Rigidbody>();
        EnableRagdoll(false);
    }
    // Update is called once per frame
    void Update() {

        armorHealthText.text = "ARMOR: " + armor;
        currentHealthText.text = "HEALTH: " + playerHealth;
        if (playerHealth < 1)
        {
            EnableRagdoll(true);
            anim.enabled = false;
            agent.enabled = false;
        }
        else
        {
            if (Armor <= 0)
            {
                bodyArmor.SetActive(false);
            }
            else
            {
                bodyArmor.SetActive(true);
            }


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            /*
             * Tests [E #3]
             * print("Horizontal: " + inputcontroller.
             * print("Mouse: " + inputcontroller.MouseInput);
             */
            move();


            mouseInput.x = Mathf.Lerp(mouseInput.x, playerInput.MouseInput.x, 1f / MouseControl.Damping.x);
            mouseInput.y = Mathf.Lerp(mouseInput.y, playerInput.MouseInput.y, 1f / MouseControl.Damping.y);

            transform.Rotate(Vector3.up * mouseInput.x * MouseControl.Sensitivity.x);

            //Crosshair.LookHeight(mouseInput.x * MouseControl.Sensitivity.x);
            //Crosshair.LookHeight (mouseInput.y * MouseControl.Sensitivity.y);
            //playerAim.SetRotation(mouseInput.x * MouseControl.Sensitivity.x);
            playerAim.SetRotation(mouseInput.y * MouseControl.Sensitivity.y);
            //print(Crosshair.transform);
        }
    }
    public void TakeDamage(int damage)
    {
        if (armor > 0)
        {
            if (armor < 10) {
                armor = 0;
            }
            else
            armor -= damage;
        }
        else
        {
            if (playerHealth < 10) {
                playerHealth = 0;
            }
            else
            playerHealth -= damage;
        }
    }

    void move()
    {
        float moveSpeed = walkSpeed;
        float DelayBetweenClips = walkingDelayBetweenClips;
        if (GameManager.GetInstance().GetInputController().Run)
        {
            moveSpeed = runSpeed;
            DelayBetweenClips = RunningDelayBetweenClips;
        }
        Vector2 direction = new Vector2(playerInput.Vertical * moveSpeed, playerInput.Horizontal * moveSpeed);
        m_MoveController.Move(direction);
        if (Vector3.Distance(transform.position, prevPosition) > minimumMovementToPlayFootstep)
            footSteps.Play(DelayBetweenClips);
        prevPosition = transform.position;
    }
    void EnableRagdoll(bool value)
    {
        print("EnableRagDoll");
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].isKinematic = !value;
        }
    }

}
