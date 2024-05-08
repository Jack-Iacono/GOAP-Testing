using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CreatureController : MonoBehaviour
{
    public static List<CreatureController> creatures = new List<CreatureController>();
    public static event EventHandler<CreatureController> OnCreatureControllerAdded;

    public event EventHandler<CreatureController> OnCreatureDataChanged;

    public State<CreatureController> currentActionState;

    public WorldState currentWorldState;
    private List<Action> actions;

    private List<WorldState> goals = new List<WorldState>();

    public GameObject homeObject;
    public GameObject workObject;
    public List<GameObject> customers = new List<GameObject>();

    private List<Action> currentPlan = new List<Action>();

    public NavMeshAgent navAgent;

    private bool hasPlan;

    public float startDelay = 0;

    private void Awake()
    {
        creatures.Add(this);
    }
    private void OnDestroy()
    {
        creatures.Remove(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        OnCreatureControllerAdded?.Invoke(this, this);

        // Initialize the Actions
        actions = new List<Action>
        {
            new Action
                ("Make Pizza", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_home", gameObject), new Property.Value(false) },
                            { new Property.Key("at_work", gameObject), new Property.Value(true) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(false) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_pizza", gameObject), new Property.Value(1, Property.Value.MergeType.ADD) },
                        }
                    )
                    , MakePizza
                ),
            new Action
                ("Make Pizza Bulk", 4,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_home", gameObject), new Property.Value(false) },
                            { new Property.Key("at_work", gameObject), new Property.Value(true) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(false) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_pizza", gameObject), new Property.Value(5, Property.Value.MergeType.ADD) },
                        }
                    )
                    , MakePizza
                ),
            new Action
                ("Sell Pizza", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_pizza", gameObject), new Property.Value(1, Property.Value.CompareType.GREATER_EQUAL) },
                            { new Property.Key("at_home", gameObject), new Property.Value(false) },
                            { new Property.Key("at_work", gameObject), new Property.Value(false) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(3, Property.Value.MergeType.ADD) },
                            { new Property.Key("has_money"), new Property.Value(12, Property.Value.MergeType.ADD) },
                            { new Property.Key("has_pizza", gameObject), new Property.Value(-1, Property.Value.MergeType.ADD) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(false) }
                        }
                    )
                    , SellPizza
                ),
            new Action
                ("Sell Pizza Bulk", 2,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_pizza", gameObject), new Property.Value(3, Property.Value.CompareType.GREATER_EQUAL) },
                            { new Property.Key("at_home", gameObject), new Property.Value(false) },
                            { new Property.Key("at_work", gameObject), new Property.Value(false) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(9, Property.Value.MergeType.ADD) },
                            { new Property.Key("has_money"), new Property.Value(36, Property.Value.MergeType.ADD) },
                            { new Property.Key("has_pizza", gameObject), new Property.Value(-3, Property.Value.MergeType.ADD) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(false) }
                        }
                    )
                    , SellPizza
                ),
            new Action
                ("GoTo Work", 6,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_work", gameObject), new Property.Value(false) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_work", gameObject), new Property.Value(true) },
                            { new Property.Key("at_home", gameObject), new Property.Value(false) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(false) }
                        }
                    )
                    , GotoWork
                ),
            new Action
                ("GoTo Home", 6,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_home", gameObject), new Property.Value(false) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_work", gameObject), new Property.Value(false) },
                            { new Property.Key("at_home", gameObject), new Property.Value(true) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(false) }
                        }
                    )
                    , GotoHome
                ),
            new Action
                ("GoTo Customer", 6,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_customer", gameObject), new Property.Value(false) },
                            { new Property.Key("found_customer", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_work", gameObject), new Property.Value(false) },
                            { new Property.Key("at_home", gameObject), new Property.Value(false) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(true) },
                            { new Property.Key("found_customer", gameObject), new Property.Value(false) }
                        }
                    )
                    , GotoCustomer
                ),
            new Action
                ("Find Customer", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("found_customer", gameObject), new Property.Value(false) },
                            { new Property.Key("at_home", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("found_customer", gameObject), new Property.Value(true) }
                        }
                    )
                    , FindCustomer
                ),
            new Action
                ("Deposit Money 1", 5,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(1, Property.Value.CompareType.GREATER_EQUAL) },
                            { new Property.Key("at_home", gameObject), new Property.Value(false) },
                            { new Property.Key("at_work", gameObject), new Property.Value(true) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(false) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(-1, Property.Value.MergeType.ADD) },
                            { new Property.Key("has_money"), new Property.Value(1, Property.Value.MergeType.ADD) }
                        }
                    )
                    , SellPizza
                ),
            new Action
                ("Deposit Money 5", 5,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(5, Property.Value.CompareType.GREATER_EQUAL) },
                            { new Property.Key("at_home", gameObject), new Property.Value(false) },
                            { new Property.Key("at_work", gameObject), new Property.Value(true) },
                            { new Property.Key("at_customer", gameObject), new Property.Value(false) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(-5, Property.Value.MergeType.ADD) },
                            { new Property.Key("has_money"), new Property.Value(5, Property.Value.MergeType.ADD) }
                        }
                    )
                    , SellPizza
                )
        };

        currentWorldState = new WorldState( new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_pizza", gameObject), new Property.Value(0) },
                    { new Property.Key("has_money", gameObject), new Property.Value(0) },
                    { new Property.Key("at_home", gameObject), new Property.Value(false) },
                    { new Property.Key("at_work", gameObject), new Property.Value(false) },
                    { new Property.Key("at_customer", gameObject), new Property.Value(false) },
                    { new Property.Key("found_customer", gameObject), new Property.Value(false) }
                });

        goals.Add(
            new WorldState(
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_pizza", gameObject), new Property.Value(5, Property.Value.CompareType.GREATER_EQUAL) }
                })
            );
        goals.Add(
            new WorldState(
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_pizza", gameObject), new Property.Value(0, Property.Value.CompareType.EQUAL) }
                })
            );
        

        navAgent = GetComponent<NavMeshAgent>();

        UIController.instance.SetState(currentWorldState.ToString());

        OnCreatureDataChanged?.Invoke(this, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasPlan)
            MakePlan();

        if(hasPlan)
        {
            ExecutePlan();
        }
    }
    private void MakePlan()
    {
        currentWorldState.Combine(WorldController.instance.currentState, WorldController.instance.gameObject);

        float startTime = Time.time;
        currentPlan = GOAP.Search(actions, currentWorldState, goals[0]);

        string planString = "Plan is... ";
        foreach (Action action in currentPlan)
            planString += action.ToString() + " -> ";
        Debug.Log(planString + "\nPlan Finished in " + (Time.time - startTime).ToString());

        UIController.instance.SetGoal(goals[0].ToString());
        UIController.instance.SetState(currentWorldState.ToString());

        WorldState goal = goals[0];
        goals.RemoveAt(0);
        goals.Add(goal);


        if (currentPlan.Count > 0)
        {
            hasPlan = true;
        }
    }

    public void ExecutePlan()
    {
        if (currentActionState == null)
        {
            if(currentPlan[0].Doable(currentWorldState))
            {
                currentPlan[0].DoAction();
            }
            else
            {
                Debug.Log("Plan Failed at " + currentPlan[0]);
                currentPlan.Clear();
            }
        }
        else
        {
            State<CreatureController>.Status status = currentActionState.Check(Time.deltaTime);

            UIController.instance.SetAction(currentPlan[0].ToString());

            if (status == State<CreatureController>.Status.SUCCESS)
            {
                currentWorldState.Apply(currentPlan[0]);
                UIController.instance.SetState(currentWorldState.ToString());

                currentPlan.RemoveAt(0);

                OnCreatureDataChanged?.Invoke(this, this);

                if(currentPlan.Count > 0)
                {
                    currentPlan[0].DoAction();
                }
                else
                {
                    currentActionState = null;
                    hasPlan = false;
                }
            }
        }
    }

    private void ChangeState(State<CreatureController> newState)
    {
        if (currentActionState != null)
            currentActionState.Exit();
        currentActionState = newState;
        currentActionState.Enter();
    }

    private void MakePizza()
    {
        ChangeState(new MakePizzaState(this));
    }
    private void SellPizza()
    {
        ChangeState(new SellPizzaState(this));
    }
    private void FindCustomer()
    {
        ChangeState(new FindCustomerState(this));
    }
    private void GotoWork()
    {
        ChangeState(new GoToState(this, workObject.transform));
    }
    private void GotoHome()
    {
        ChangeState(new GoToState(this, homeObject.transform));
    }
    private void GotoCustomer()
    {
        ChangeState(new GoToState(this, customers[UnityEngine.Random.Range(0,customers.Count)].transform));
    }

    class MakePizzaState : State<CreatureController>
    {
        private float actionTime = 1;
        private float actionTimer = 0;

        public MakePizzaState(CreatureController owner) : base(owner) { }

        public override Status Check(float deltaTime)
        {
            actionTimer -= Time.deltaTime;
            if (actionTimer <= 0)
                return Status.SUCCESS;
            else return Status.RUNNING;
        }

        public override void Enter()
        {
            actionTimer = actionTime;
        }
        public override void Exit() { }
    }
    class SellPizzaState : State<CreatureController>
    {
        private float actionTime = 1;
        private float actionTimer = 0;

        public SellPizzaState(CreatureController owner) : base(owner) { }

        public override Status Check(float deltaTime)
        {
            actionTimer -= Time.deltaTime;
            if (actionTimer <= 0)
                return Status.SUCCESS;
            else return Status.RUNNING;
        }

        public override void Enter()
        {
            actionTimer = actionTime;
        }
        public override void Exit() { }
    }
    class FindCustomerState : State<CreatureController>
    {
        private float actionTime = 2;
        private float actionTimer = 0;

        public FindCustomerState(CreatureController owner) : base(owner) { }

        public override Status Check(float deltaTime)
        {
            actionTimer -= Time.deltaTime;
            if (actionTimer <= 0)
                return Status.SUCCESS;
            else return Status.RUNNING;
        }

        public override void Enter()
        {
            actionTimer = actionTime;
        }
        public override void Exit() { }
    }
    class GoToState : State<CreatureController>
    {
        private Transform dst;

        public GoToState(CreatureController owner, Transform destination) : base(owner) { dst = destination; }

        public override Status Check(float deltaTime)
        {
            owner.navAgent.destination = dst.position;
            if (Vector3.Distance(owner.transform.position, dst.position) <= 0.5)
                return Status.SUCCESS;
            return Status.RUNNING;
        }

        public override void Enter() { }
        public override void Exit() { }
    }
}
