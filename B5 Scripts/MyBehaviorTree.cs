using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using RootMotion.FinalIK;
using UnityEngine.UI;
using System.Collections.Generic;

public class MyBehaviorTree : MonoBehaviour
{
	public bool feedback = true;

	public Transform wander1;
	public Transform wander2;
	public Transform wander3;
	public GameObject participant1;
	public GameObject participant2;
	public GameObject participant3;

	public GameObject ball;
	public Text dialogue;
	public InteractionObject ikBall;
	public FullBodyBipedEffector righthand;
	public FullBodyBipedEffector lefthand;
	public InteractionObject shakePoint1;
	public InteractionObject shakePoint2;
	public InteractionObject risePoint;
	public InteractionObject redCard;
	public InteractionObject AttackPoint;
	public FullBodyBipedEffector rightFoot;
	public InteractionObject rightFootAttractor;
	public FullBodyBipedEffector leftFoot;
	public InteractionObject leftFootAttractor;

	private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}


	protected Node ST_ApproachAndWait(Transform target, GameObject p)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence( p.GetComponent<BehaviorMecanim>().Node_GoTo(target.transform.position), new LeafWait(0));
	}


	protected Node PickUp(GameObject p)
	{
		return new Sequence(p.GetComponent<BehaviorMecanim>().Node_StartInteraction(righthand, ikBall),
							new LeafWait(300),
							p.GetComponent<BehaviorMecanim>().Node_StopInteraction(righthand));
	}

	public Node Node_ThrowBall()
	{
		return new LeafInvoke(() => {
			//new LeafWait(1000);
			Rigidbody rb = ball.GetComponent<Rigidbody>();
			rb.velocity = 2 * (participant2.transform.position - ball.transform.position);
			rb.isKinematic = false;
			ball.transform.parent = null;
			return RunStatus.Success;
		});
	}
	/*
	protected Node Pass()
	{
		return new Sequence(
			new SequenceParallel (
				participant1.GetComponent<BehaviorMecanim>().Node_Throw(),
				this.Node_ThrowBall()
				),
			new LeafWait(300),
			participant2.GetComponent<BehaviorMecanim>().Node_Catch()
			);
	}
	*/

	protected Node Pass()
	{
		return
			new SequenceParallel(
				new SequenceParallel(
					participant1.GetComponent<BehaviorMecanim>().Node_Throw(),
					participant2.GetComponent<BehaviorMecanim>().Node_Catch()
					//this.Node_ThrowBall()
					),
				new Sequence(
					new LeafWait(1500),
					this.Node_ThrowBall()
				)
			);;
	}

	public Node shakeDoubleHands(GameObject p1, GameObject p2)
	{
		return new Sequence(
			p1.GetComponent<BehaviorMecanim>().Node_StartInteraction(righthand, shakePoint1),
			p2.GetComponent<BehaviorMecanim>().Node_StartInteraction(lefthand, shakePoint2),
			p1.GetComponent<BehaviorMecanim>().Node_StartInteraction(lefthand, shakePoint1),
			p2.GetComponent<BehaviorMecanim>().Node_StartInteraction(righthand, shakePoint2),
			p1.GetComponent<BehaviorMecanim>().say("You: You can do this, pass me by your feet next time!", dialogue),
			new LeafWait(2000),
			p1.GetComponent<BehaviorMecanim>().Node_StopInteraction(righthand),
			p2.GetComponent<BehaviorMecanim>().Node_StopInteraction(lefthand),
			p1.GetComponent<BehaviorMecanim>().Node_StopInteraction(lefthand),
			p2.GetComponent<BehaviorMecanim>().Node_StopInteraction(righthand)
			) ;
	}

	public Node touchPoints(GameObject p)
	{
		return new Sequence(
			p.GetComponent<BehaviorMecanim>().Node_StartInteraction(righthand, risePoint),
			new LeafWait(1000),
			p.GetComponent<BehaviorMecanim>().Node_StopInteraction(righthand)
			);
	}

	public Node part3(GameObject p)
	{
		return new Sequence(
			p.GetComponent<BehaviorMecanim>().say("Referee: Red Card Warning! It's soccer! Throwing the ball with your hand is a foul", dialogue),
			p.GetComponent<BehaviorMecanim>().Node_StartInteraction(righthand, redCard),
			new LeafWait(300),
			p.GetComponent<BehaviorMecanim>().Node_StopInteraction(righthand),
			this.touchPoints(p)
			);
	}

	public Node punch(GameObject p)
	{
		return new Sequence(
			p.GetComponent<BehaviorMecanim>().Node_StartInteraction(righthand, AttackPoint),
			new LeafWait(250),
			p.GetComponent<BehaviorMecanim>().Node_StopInteraction(righthand),
			p.GetComponent<BehaviorMecanim>().Node_StartInteraction(lefthand, AttackPoint),
			new LeafWait(250),
			p.GetComponent<BehaviorMecanim>().Node_StopInteraction(lefthand),
			p.GetComponent<BehaviorMecanim>().say("You: Fuck you!", dialogue)
			);
	}


	protected Node BuildTreeRoot()
	{
		Node roaming =
		new DecoratorLoop(
			//new Selector(	
				new Sequence(
					participant1.GetComponent<BehaviorMecanim>().say("", dialogue),
					this.ST_ApproachAndWait(this.wander1, participant1),
					participant1.GetComponent<BehaviorMecanim>().say("Your Teamate:Hey, let's play soccer!", dialogue),
					new Sequence(
						this.PickUp(participant1),
						participant1.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => participant2.transform.position)),
						participant2.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => ball.transform.position)),
						participant3.GetComponent<BehaviorMecanim>().ST_TurnToFace(Val.V(() => ball.transform.position)),
						new LeafWait(200),
						participant1.GetComponent<BehaviorMecanim>().say("You:Pass me!", dialogue),
						this.Pass(),
						new LeafWait(200),
						this.part3(participant3),
						//new LeafWait(50),
						//participant2.GetComponent<BehaviorMecanim>().Node_Catch(),
						this.ST_ApproachAndWait(this.wander3, participant2),
						new ControlNode(			
							new DecoratorLoop(
								this.punch(participant2)
							),
							new DecoratorLoop(
								this.shakeDoubleHands(participant1, participant2)
							)
						)
					//this.PutDown(participant)
					)
				)
			//this.ST_ApproachAndWait(this.wander2, participant1)
			//)
		);
		return roaming;
	}

	private class ControlNode : NodeGroup
	{

		public ControlNode(params Node[] children)
			: base(children)
		{
		}

		public override IEnumerable<RunStatus> Execute()
		{

			foreach (Node node in this.Children)
			{
				this.Selection = node;
				node.Start();

				if (GameObject.Find("Updater").GetComponent<MyBehaviorTree>().feedback == true)
				{
					GameObject.Find("Updater").GetComponent<MyBehaviorTree>().feedback = false;
					continue;
				}

				// If the current node is still running, report that. Don't 'break' the enumerator
				RunStatus result;
				while ((result = this.TickNode(node)) == RunStatus.Running)
					yield return RunStatus.Running;

				// Call Stop to allow the node to clean anything up.
				node.Stop();

				// Clear the selection
				this.Selection.ClearLastStatus();
				this.Selection = null;

				if (result == RunStatus.Failure)
				{
					yield return RunStatus.Failure;
					yield break;
				}

				yield return RunStatus.Running;
			}
			yield return RunStatus.Success;
			yield break;
		}

	}
}



