using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CreatureController : MonoBehaviour
{
    public static List<CreatureController> creatures = new List<CreatureController>();
    public static event EventHandler<CreatureController> OnCreatureControllerAdded;

    public event EventHandler<CreatureController> OnCreatureDataChanged;

    public WorldState currentState;
    private List<Action> actions;

    private List<WorldState> goals = new List<WorldState>();

    public GameObject homeObject;
    public GameObject workObject;
    public List<GameObject> customers = new List<GameObject>();

    private List<Action> currentPlan = new List<Action>();

    private float actionTimer = 0.5f;
    private float actionTime = 0.5f;

    private NavMeshAgent navAgent;

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

        currentState = new WorldState( new Dictionary<Property.Key, Property.Value>()
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
                    { new Property.Key("has_money", gameObject), new Property.Value(100, Property.Value.CompareType.GREATER_EQUAL) }
                })
            );
        goals.Add(
            new WorldState(
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_pizza", gameObject), new Property.Value(0, Property.Value.CompareType.LESS_EQUAL) }
                })
            );

        navAgent = GetComponent<NavMeshAgent>();

        UIController.instance.SetState(currentState.ToString());

        OnCreatureDataChanged?.Invoke(this, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentPlan.Count == 0)
        {
            Invoke("MakePlan", startDelay);
        }
            

        if(currentPlan.Count > 0)
        {
            ExecutePlan();
        }
    }
    private void MakePlan()
    {
        currentState.Combine(WorldController.instance.currentState, WorldController.instance.gameObject);

        float startTime = Time.time;
        currentPlan = GOAP.Search(actions, currentState, goals[0]);

        string planString = "Plan is... ";
        foreach (Action action in currentPlan)
            planString += action.ToString() + " -> ";
        Debug.Log(planString + "\nPlan Finished in " + (Time.time - startTime).ToString());

        UIController.instance.SetGoal(goals[0].ToString());

        goals.RemoveAt(0);
    }

    public void ExecutePlan()
    {
        bool check = false;

        if (currentPlan[0].Doable(currentState))
        {
            check = currentPlan[0].DoAction();
            UIController.instance.SetAction(currentPlan[0].ToString());
        }
        else
        {
            Debug.Log("Plan Failed at " + currentPlan[0]);
            currentPlan.Clear();
        }

        if (check)
        {
            currentState.Apply(currentPlan[0]);
            
            currentPlan.RemoveAt(0);
            actionTimer = actionTime;

            OnCreatureDataChanged?.Invoke(this, this);
        }
    }

    private bool MakePizza()
    {
        actionTimer -= Time.deltaTime;
        if(actionTimer <= 0)
            return true;
        else return false;
    }
    private bool SellPizza()
    {
        actionTimer -= Time.deltaTime;
        if (actionTimer <= 0)
            return true;
        else return false;
    }
    private bool FindCustomer()
    {
        actionTimer -= Time.deltaTime;
        if (actionTimer <= 0)
            return true;
        else return false;
    }
    private bool GotoWork()
    {
        navAgent.destination = workObject.transform.position;
        if (Vector3.Distance(transform.position, workObject.transform.position) <= 0.5)
            return true;
        else return false;
    }
    private bool GotoHome()
    {
        navAgent.destination = homeObject.transform.position;
        if (Vector3.Distance(transform.position, homeObject.transform.position) <= 0.5)
            return true;
        else return false;
    }
    private bool GotoCustomer()
    {
        navAgent.destination = customers[0].transform.position;
        if (Vector3.Distance(transform.position, customers[0].transform.position) <= 0.5)
            return true;
        else return false;
    }


    
}
