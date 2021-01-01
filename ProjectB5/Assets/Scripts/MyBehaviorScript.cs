using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using RootMotion.FinalIK;
using UnityEngine.UI;

public class MyBehaviorScript : MonoBehaviour
{
	//People in the scene
	public GameObject Daniel;
	public GameObject Jason;
	public GameObject Jake;
	public GameObject Bartender;

	//All the points
	public Transform[] doorButtonSpot;
	public Transform[] doorOutsideSpot;
	public Transform[] meetUpSpot;

	//All the objects
	public GameObject[] doorButton;
	public GameObject[] doors;

	//Interactive Behavior (IK)
	public InteractionObject[] roomButtonIK;
	public InteractionObject shakePoint;
	public InteractionObject shakePoint2;
	public FullBodyBipedEffector[] bodyIK;

	private BehaviorAgent behaviorAgent;
	private bool talkingtoeachother = false;
	// Use this for initialization
	void Start()
	{
		behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
		BehaviorManager.Instance.Register(behaviorAgent);
		behaviorAgent.StartBehavior();
	}

	// Update is called once per frame
	void Update()
	{

	}

	protected Node ST_ApproachAndWait(GameObject move, Transform target)
	{
		Val<Vector3> position = Val.V(() => target.position);
		return new Sequence(move.GetComponent<BehaviorMecanim>().Node_GoTo(position), new LeafWait(0));
	}

	protected Node BuildTreeRoot()
	{
		Node roaming = new DecoratorLoop(
						new Sequence(
							startUP(),
							this.Conversation(),
							this.Bar()
						)
		); ;
		return roaming;
	}
	#region StartUp

	protected Node startUP() {
		return new Sequence(
							this.SetDialogueText("God: Dan is on the left",500),
							
							this.SetDialogueText("God: Jason is in the center", 500),
							
							this.SetDialogueText("God: Jake is on the right", 500),
							
							this.SetDialogueText("God: Dan and Jake got an issue, and Jason is the middle person", 500)
							
							);
	}
	#endregion

	//Conversation(Non-Controllable)
	#region Conversation
	//This method with control the whole scene of conversation
	protected Node Conversation() {
		return new Sequence
			(
			//Dan-call-Jason
			Anim_Conversation_Over_Phone(Daniel, Jason, true),
			new LeafWait(2000),
			this.convoUIOverPhone_DanJason(),
			Anim_Conversation_Over_Phone(Daniel, Jason, false),

			//-------Implement Converstion Dialogue UI--------
			//
			//Jason-call-Jake
			Anim_Conversation_Over_Phone(Jason, Jake, true),
			new LeafWait(2000),
			this.convoUIOverPhoneJasonJae(),
			Anim_Conversation_Over_Phone(Jason, Jake, false),

			//As the Conversation finishes they approach outside the door and and close the door!
			new SequenceParallel
			(
			this.openTheDoor(Daniel, doorButtonSpot[0], doorOutsideSpot[0], doorButton[0], bodyIK[0], roomButtonIK[0], doors[0]),
			this.openTheDoor(Jason, doorButtonSpot[1], doorOutsideSpot[1], doorButton[1], bodyIK[1], roomButtonIK[1], doors[1]),
			this.openTheDoor(Jake, doorButtonSpot[2], doorOutsideSpot[2], doorButton[2], bodyIK[2], roomButtonIK[2], doors[2])
			)


			) ;
	}

	//This method will call and Animate the players calling
	protected Node Anim_Conversation_Over_Phone(GameObject player1Calling, GameObject player2Picking, bool talking)
	{
		return new Sequence
			(
			player1Calling.GetComponent<BehaviorMecanim>().Node_BodyAnimation("Talking On Phone", talking),
			player2Picking.GetComponent<BehaviorMecanim>().Node_BodyAnimation("Talking On Phone", talking),
			//new LeafWait(40000),
			//player1Calling.GetComponent<BehaviorMecanim>().Node_BodyAnimation("Talking On Phone", talking),
			//player2Picking.GetComponent<BehaviorMecanim>().Node_BodyAnimation("Talking On Phone", talking),
			new LeafWait(200)
			);
	}

	//This method will take them to door and open the door and move them outside
	protected Node openTheDoor(GameObject player, Transform moveSpot, Transform outsideDoorSpot, GameObject lookPosition, FullBodyBipedEffector hand, InteractionObject obj, GameObject door) {
		return new Sequence
		(
			//Move the player to Button Spot
			this.Move_GO_TO_Radius(player, moveSpot, 1.5f),
			new LeafWait(1500),

			//Press the Button
			player.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, obj),
			openAndCloseDoor(player, door, true, "Pointing", 1000),
			player.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand),


			//Go Outside the door and close the door!
			this.Move_GO_TO_Radius(player, outsideDoorSpot, 1.5f),
			openAndCloseDoor(player, door, false, "Clap", 500)
		);
	}

	//Conversation BETWEEN Dan&Jason using Dialoug UI
	protected Node convoUIOverPhone_DanJason()
	{
		return new Sequence(
			this.SetDialogueText("Dan: Hey Jason, how you been!",150),
			this.SetDialogueText("Jason: Hey Daniel, life's good how are you",100),
			this.SetDialogueText("Dan: Nothing good, Jake screwed up my chance with Jamie",100),
			this.SetDialogueText("Jason: What happened?",200),
			this.SetDialogueText("Dan: Jamie is the girl I like, and a huge fan of dart game, and "+
			"\n"+"Jake said I suck at DART,.",200),
			//this.SetDialogueText("Dan: Jake said I suck at ___,"),
			this.SetDialogueText("Jason: That's messed up! Let's go for a drink, and lets invite Jake",200),
			this.SetDialogueText("Dan: You do that, I am going to teach him who is the boss!",200)
			);
	}

	protected Node convoUIOverPhoneJasonJae() {
		return new Sequence(
			this.SetDialogueText("Jason: Hey Jake, how you been!",100),
			this.SetDialogueText("Jake: Hey Daniel, looks like Dan called",100),
			this.SetDialogueText("Jason: Yes, indeed. He said something about Jamie, and you screwed up.",200),
			this.SetDialogueText("Jake: From last six days, I have been working my butt off on the project",250),
			this.SetDialogueText("Jake: All Dan does is flirt with Jamie",200),
			this.SetDialogueText("Jason: Well if you want to resolve the issue then I will see you at the bar",250),
			this.SetDialogueText("Jake: See you in 5 min",250)
			);
	}


	//Helper method for openTheDoor()
	protected Node openAndCloseDoor(GameObject player, GameObject door, bool isOpen, String animation, long animRunTime) {


		return new Sequence(
			//Play Animation
			new LeafInvoke(() => player.GetComponent<CharacterMecanim>().HandAnimation(animation, true)),
			new LeafWait(animRunTime),

			//Open the door if true, if false close door
			new LeafInvoke(
				() => {
					if (isOpen)
					{
						door.SetActive(false);
					}
					else
					{
						door.SetActive(true);
					}
				}

				),
			//Stop Animation
			new LeafInvoke(() => player.GetComponent<CharacterMecanim>().HandAnimation(animation, false))

			);
	}

	#endregion

	//Bar(Controllable)[[[[[I wasn't able to complete the Bar interactive, fully]]]]]]
	#region AtBar

	//Main Bar Node that controls behavior[---------Wasn't able to finsih it-------------]
	protected Node Bar() {
		return new Sequence(
			new SequenceParallel(
			Move_GO_TO_Radius(Daniel, meetUpSpot[0], 1f),
			Move_GO_TO_Radius(Jason, meetUpSpot[1], 1f),
			Move_GO_TO_Radius(Jake, meetUpSpot[2], 1f)
			
			),

			new SequenceParallel(
				//new LeafInvoke(()=>print("yep")),
				new DecoratorForceStatus(RunStatus.Success, this.Look(Jake, meetUpSpot[3])),
				//this.Look(Jason, meetUpSpot[3]),
				new DecoratorForceStatus(RunStatus.Success, this.Look(Daniel, meetUpSpot[3])),
				new LeafInvoke(() => print("yep2"))
			),
			
			new Sequence(
				this.IK_HandShake(Daniel, Jason, shakePoint),
				//this.IK_HandShake(Daniel, Jason, shakePoint),
				this.SpeakUI(Daniel, "Dan: Hey Jason, thanks for inviting"),
				this.IK_HandShake(Jake,Jason,shakePoint2)),
				this.SpeakUI(Daniel, "Jake: Hey Jason, thanks for inviting"),
				this.SpeakUI(Jason, "Jason: You guys are welcome"),
				new LeafWait(1000),
				friendConversation()

			/////////////////////////////////////////
			//There was suppose to be user interaction after this part//
			//I WASN'T ABLE TO COMPLETE//
			/////////////////////////////////////////
			
		);
	}

	protected Node friendConversation() {
		return new Sequence(
			this.SpeakUI(Daniel, "Dan: Dude you messed  up long time"),
			this.SpeakUI(Jake, "Jake: So did you. You just forgot about the project"),
			this.SpeakUI(Jason, "Jason: Guys calmdown, we can solve this right here")
			);
	}

	#endregion

	//Methods that are used on frequently in the code
	#region General Method
	//This method is general and will take player to a Transform point on the field, at a radius!
	protected Node Move_GO_TO_Radius(GameObject player, Transform target, float howfar)
	{
		Val<Vector3> position = Val.V(() => target.position);

		return new Sequence(
							player.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(position, howfar),
							new LeafWait(1000)
							
						   );
	}

	//This method is general and will take player to a Transform point on the field!
	protected Node Move_GO_TO(GameObject player, Transform destination)
	{
		Val<Vector3> position = Val.V(() => destination.position);


		return new Sequence(
							new LeafInvoke(() => player.GetComponent<CharacterMecanim>().NavGoTo(position))
						   //new LeafInvoke(()=> DandestinationAchieved = true)
						   );
	}

	//This method makes a player face towards a given target
	protected Node Look(GameObject player1, Transform lookpoint)
	{
		//Val<Vector3> position = Val.V(() => lookpoint.position);
		return new Sequence(
			//Node_OrientTowards(Val.V(() => position.position))
			player1.GetComponent<BehaviorMecanim>().Node_OrientTowards(Val.V(() => lookpoint.position)),
			new LeafWait(1000)

			//player1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture(Val.V(()=>"WAVE"), Val.V(() => (long)1000))
			);
	}

	protected Node handMotion(GameObject player1, string motionCommand) {
		return new Sequence(
			player1.GetComponent<BehaviorMecanim>().Node_HandAnimation(motionCommand, true),
			new LeafWait(1500),
			player1.GetComponent<BehaviorMecanim>().Node_HandAnimation(motionCommand,false)
			) ;

			
	}

	//Random Talking Gestures that define if a person is talking or not.
	protected Node TalkingGestures(GameObject player1)
	{
		return new SelectorShuffle(
			player1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("Think", 2000),
			player1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("BeingCocky", 2000),
			player1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("Point", 2000),
			player1.GetComponent<BehaviorMecanim>().ST_PlayHandGesture("MouthWipe", 2000)
			);

	}
	#endregion

	//Implemented Affordance (2 new affordances, ONLY 1 IK affordances)
	//1 new affordance is inside Conversation() Region -> openDoor()
	#region Affordances
	//This controls the total conversation happening!
	private Node SetDialogueText(String text, long ms)
	{
		return (
			new Sequence(
				new LeafInvoke(() => {
					if (DialogueUI.Available())
					{
						if (DialogueUI.Finished(text))
						{
							return RunStatus.Success;
						}
						else
						{
							DialogueUI.SetText(text);
							return RunStatus.Running;
						}
					}
					else
					{
						return RunStatus.Running;
					}
				}),
				new LeafWait(ms)
			)
		);
	}

	//This controls the animation on when to start and when to end when the preson is talking!

	protected Node SpeakUI(GameObject player, string text) {
		return new Sequence(
			new SequenceParallel(
				this.TalkingGestures(player),
				//this.Look(player, look),
				new LeafInvoke(() =>
				{
					if (DialogueUI.Available())
					{
						if (DialogueUI.Finished(text))
						{
							return RunStatus.Success;
						}
						else
						{
							DialogueUI.SetText(text);
							return RunStatus.Running;
						}
					}
					else
					{
						return RunStatus.Running;
					}
				}

				),
				new LeafWait(1000)

			)

			);
	}

	private Node IK_HandShake(GameObject player1, GameObject player2, InteractionObject shakepoint)
	{
		return (new Sequence(
				
				new LeafInvoke(() => {
					var dir = player1.transform.position - player2.transform.position;
					dir.Normalize();
					var pos = (player1.transform.position + player2.transform.position) / 2;
					pos.y = 1;
					shakepoint.transform.position = pos - dir * 0.17f;
					shakepoint.transform.position = pos + dir * 0.17f;
					shakepoint.transform.rotation = Quaternion.LookRotation(player2.transform.position + player2.transform.right * -1f - player1.transform.position, Vector3.up);
					shakepoint.transform.rotation = Quaternion.LookRotation(player1.transform.position + player1.transform.right * -1f - player2.transform.position, Vector3.up);
				}),
				new SequenceParallel(
					player1.GetComponent<BehaviorMecanim>().Node_StartInteraction(bodyIK[0], shakepoint),
					player2.GetComponent<BehaviorMecanim>().Node_StartInteraction(bodyIK[1], shakepoint)
				),
				new LeafWait(1000),
				new SequenceParallel(
					player1.GetComponent<BehaviorMecanim>().Node_StopInteraction(bodyIK[0]),
					player2.GetComponent<BehaviorMecanim>().Node_StopInteraction(bodyIK[1])
				)
			)
		);
	}


	//Another Afforandance is inside of my CONVERSATION REGION! openTheDoor().It allows user
	#endregion
}
