using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPTesting : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestCase1();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestCase2();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestCase3();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestCase4();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TestCase5();
        }
    }

    private void TestCase1()
    {
        WorldState initialState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>() 
                {
                    { new Property.Key("at_job", gameObject), new Property.Value(false) },
                    { new Property.Key("at_mine", gameObject), new Property.Value(false) },
                    { new Property.Key("at_refinery", gameObject), new Property.Value(false) },
                    { new Property.Key("at_shop", gameObject), new Property.Value(false) },
                    { new Property.Key("at_workbench", gameObject), new Property.Value(false) },
                    { new Property.Key("has_tool", gameObject), new Property.Value(false) },
                    { new Property.Key("has_money", gameObject), new Property.Value(false) },
                    { new Property.Key("has_raw_material", gameObject), new Property.Value(false) },
                    { new Property.Key("has_refined_material", gameObject), new Property.Value(false) }
                }
            );

        WorldState goalState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_tool", gameObject), new Property.Value(true) }
                }
            );

        List<Action> actions = new List<Action>
        {
            new Action
                ("Goto Mine", 1,
                    new WorldState(),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_mine", gameObject), new Property.Value(true) },
                            { new Property.Key("at_job", gameObject), new Property.Value(false) },
                            { new Property.Key("at_workbench", gameObject), new Property.Value(false) },
                            { new Property.Key("at_refinery", gameObject), new Property.Value(false) },
                            { new Property.Key("at_shop", gameObject), new Property.Value(false) }
                        }
                    )
                ),
            new Action
                ("Goto Workbench", 1,
                    new WorldState(),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_mine", gameObject), new Property.Value(false) },
                            { new Property.Key("at_job", gameObject), new Property.Value(false) },
                            { new Property.Key("at_workbench", gameObject), new Property.Value(true) },
                            { new Property.Key("at_refinery", gameObject), new Property.Value(false) },
                            { new Property.Key("at_shop", gameObject), new Property.Value(false) }
                        }
                    )
                ),
            new Action
                ("Goto Refinery", 1,
                    new WorldState(),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_mine", gameObject), new Property.Value(false) },
                            { new Property.Key("at_job", gameObject), new Property.Value(false) },
                            { new Property.Key("at_workbench", gameObject), new Property.Value(false) },
                            { new Property.Key("at_refinery", gameObject), new Property.Value(true) },
                            { new Property.Key("at_shop", gameObject), new Property.Value(false) }
                        }
                    )
                ),
            new Action
                ("Goto Job", 1,
                    new WorldState(),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_mine", gameObject), new Property.Value(false) },
                            { new Property.Key("at_job", gameObject), new Property.Value(true) },
                            { new Property.Key("at_workbench", gameObject), new Property.Value(false) },
                            { new Property.Key("at_refinery", gameObject), new Property.Value(false) },
                            { new Property.Key("at_shop", gameObject), new Property.Value(false) }
                        }
                    )
                ),
            new Action
                ("Goto Shop", 1,
                    new WorldState(),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_mine", gameObject), new Property.Value(false) },
                            { new Property.Key("at_job", gameObject), new Property.Value(false) },
                            { new Property.Key("at_workbench", gameObject), new Property.Value(false) },
                            { new Property.Key("at_refinery", gameObject), new Property.Value(false) },
                            { new Property.Key("at_shop", gameObject), new Property.Value(true) }
                        }
                    )
                ),
            new Action
                ("Shop", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(true) },
                            { new Property.Key("at_shop", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(false) },
                            { new Property.Key("has_tool", gameObject), new Property.Value(true) }
                        }
                    )
                ),
            new Action
                ("Work", 8,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_job", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(true) },
                        }
                    )
                ),
            new Action
                ("Refine", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_refinery", gameObject), new Property.Value(true) },
                            { new Property.Key("has_raw_material", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_refined_material", gameObject), new Property.Value(true) },
                            { new Property.Key("has_raw_material", gameObject), new Property.Value(true) }
                        }
                    )
                ),
            new Action
                ("Craft", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_refined_material", gameObject), new Property.Value(true) },
                            { new Property.Key("at_workbench", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_refined_material", gameObject), new Property.Value(false) },
                            { new Property.Key("has_tool", gameObject), new Property.Value(true) }
                        }
                    )
                ),
            new Action
                ("Gather", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_mine", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_raw_material", gameObject), new Property.Value(true) }
                        }
                    )
                ),
            new Action
                ("Drop Materials", 0,
                    new WorldState(),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_raw_material", gameObject), new Property.Value(false) },
                            { new Property.Key("has_refined_material", gameObject), new Property.Value(false) }
                        }
                    )
                )
        };

        float startTime = Time.time;
        List<Action> plan = GOAP.Search(actions, initialState, goalState);
        string planString = "Plan is... ";
        foreach( Action action in plan )
            planString += action.ToString() + " -> ";
        Debug.Log(planString + "\nPlan Finished in " + (Time.time - startTime).ToString());
    }
    private void TestCase2()
    {
        WorldState initialState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_pizza", gameObject), new Property.Value(0, Property.Value.CompareType.LESS_EQUAL) },
                    { new Property.Key("has_money", gameObject), new Property.Value(10, Property.Value.CompareType.LESS_EQUAL) },
                    { new Property.Key("at_work", gameObject), new Property.Value(false) },
                    { new Property.Key("at_home", gameObject), new Property.Value(true) }
                }
            );

        WorldState goalState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_pizza", gameObject), new Property.Value(2, Property.Value.CompareType.GREATER_EQUAL) }
                }
            );

        List<Action> actions = new List<Action>
        {
            new Action
                ("Buy Pizza", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(7, Property.Value.CompareType.GREATER_EQUAL) },
                            { new Property.Key("at_home", gameObject), new Property.Value(true) },
                            { new Property.Key("at_work", gameObject), new Property.Value(false) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_pizza", gameObject), new Property.Value(1, Property.Value.MergeType.ADD) },
                            { new Property.Key("has_money", gameObject), new Property.Value(-7, Property.Value.MergeType.ADD) }
                        }
                    )
                ),
            new Action
                ("Work", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_work", gameObject), new Property.Value(true) },
                            { new Property.Key("at_home", gameObject), new Property.Value(false) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(5, Property.Value.MergeType.ADD) }
                        }
                    )
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
                            { new Property.Key("at_home", gameObject), new Property.Value(false) }
                        }
                    )
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
                            { new Property.Key("at_home", gameObject), new Property.Value(true) }
                        }
                    )
                )
        };

        float startTime = Time.time;
        List<Action> plan = GOAP.Search(actions, initialState, goalState);
        string planString = "Plan is... ";
        foreach (Action action in plan)
            planString += action.ToString() + " -> ";
        Debug.Log(planString + "\nPlan Finished in " + (Time.time - startTime).ToString());
    }
    private void TestCase3()
    {
        WorldState initialState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("lights_on", gameObject), new Property.Value(5) }
                }
            );

        WorldState goalState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("lights_on", gameObject), new Property.Value(0, Property.Value.CompareType.LESS_EQUAL) }
                }
            );

        List<Action> actions = new List<Action>
        {

            new Action
                ("Turn_Off_Light", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("is_hungry", gameObject), new Property.Value(false) },
                            { new Property.Key("near_light", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("lights_on", gameObject), new Property.Value(-1, Property.Value.MergeType.ADD) },
                            { new Property.Key("is_hungry", gameObject), new Property.Value(true) },
                            { new Property.Key("near_light", gameObject), new Property.Value(false) }
                        }
                    )
                ),
            new Action
                ("Eat", 1,
                    new WorldState(),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("is_hungry", gameObject), new Property.Value(false) }
                        }
                    )
                ),
            new Action
                ("Goto Light", 1,
                    new WorldState(),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("near_light", gameObject), new Property.Value(true) }
                        }
                    )
                )
        };

        float startTime = Time.time;
        List<Action> plan = GOAP.Search(actions, initialState, goalState);
        string planString = "Plan is... ";
        foreach (Action action in plan)
            planString += action.ToString() + " -> ";
        Debug.Log(planString + "\nPlan Finished in " + (Time.time - startTime).ToString());
    }
    private void TestCase4()
    {
        WorldState initialState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_pizza", gameObject), new Property.Value(3) },
                    { new Property.Key("has_money", gameObject), new Property.Value(0) },
                    { new Property.Key("at_home", gameObject), new Property.Value(true) },
                    { new Property.Key("at_work", gameObject), new Property.Value(false) }
                }
            );

        WorldState goalState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_money", gameObject), new Property.Value(28, Property.Value.CompareType.GREATER_EQUAL) }
                }
            );

        List<Action> actions = new List<Action>
        {
            new Action
                ("Make Pizza", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("at_home", gameObject), new Property.Value(false) },
                            { new Property.Key("at_work", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_pizza", gameObject), new Property.Value(1, Property.Value.MergeType.ADD) },
                        }
                    )
                ),
            new Action
                ("Sell Pizza", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_pizza", gameObject), new Property.Value(1, Property.Value.CompareType.GREATER_EQUAL) },
                            { new Property.Key("at_home", gameObject), new Property.Value(true) },
                            { new Property.Key("at_work", gameObject), new Property.Value(false) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("has_money", gameObject), new Property.Value(7, Property.Value.MergeType.ADD) },
                            { new Property.Key("has_pizza", gameObject), new Property.Value(-1, Property.Value.MergeType.ADD) }
                        }
                    )
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
                            { new Property.Key("at_home", gameObject), new Property.Value(false) }
                        }
                    )
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
                            { new Property.Key("at_home", gameObject), new Property.Value(true) }
                        }
                    )
                )
        };

        Debug.Log("Started");

        float startTime = Time.time;
        List<Action> plan = GOAP.Search(actions, initialState, goalState);
        string planString = "Plan is... ";
        foreach (Action action in plan)
            planString += action.ToString() + " -> ";
        Debug.Log(planString + "\nPlan Finished in " + (Time.time - startTime).ToString());
    }
    private void TestCase5()
    {
        WorldState initialState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("lights_on", gameObject), new Property.Value(5) },
                    { new Property.Key("near_light", gameObject), new Property.Value(false) },
                    { new Property.Key("light_found", gameObject), new Property.Value(false) }
                }
            );

        WorldState goalState = new WorldState
            (
                new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("lights_on", gameObject), new Property.Value(0, Property.Value.CompareType.LESS_EQUAL) }
                }
            );

        List<Action> actions = new List<Action>
        {

            new Action
                ("Turn_Off_Light", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("near_light", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("lights_on", gameObject), new Property.Value(-1, Property.Value.MergeType.ADD) },
                            { new Property.Key("near_light", gameObject), new Property.Value(false) },
                            { new Property.Key("light_found", gameObject), new Property.Value(false) }
                        }
                    )
                ),
            new Action
                ("Goto Light", 1,
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("light_found", gameObject), new Property.Value(true) }
                        }
                    ),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("near_light", gameObject), new Property.Value(true) }
                        }
                    )
                ),
            new Action
                ("Find Light", 1,
                    new WorldState(),
                    new WorldState
                    (
                        new Dictionary<Property.Key, Property.Value>()
                        {
                            { new Property.Key("light_found", gameObject), new Property.Value(true) }
                        }
                    )
                )
        };

        float startTime = Time.time;
        List<Action> plan = GOAP.Search(actions, initialState, goalState);
        string planString = "Plan is... ";
        foreach (Action action in plan)
            planString += action.ToString() + " -> ";
        Debug.Log(planString + "\nPlan Finished in " + (Time.time - startTime).ToString());
    }
}
