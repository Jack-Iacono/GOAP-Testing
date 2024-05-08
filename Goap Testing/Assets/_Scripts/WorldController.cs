using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public static WorldController instance;

    public WorldState currentState;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this);

        // Register to the static event
        CreatureController.OnCreatureControllerAdded += OnCreatureControllerAdded;

        currentState = new WorldState(new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("has_money"), new Property.Value(0) },
                });
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCreatureControllerAdded(object sender, CreatureController e)
    {
        e.OnCreatureDataChanged += OnCreatureDataChanged;
    }
    private void OnCreatureDataChanged(object sender, CreatureController e)
    {
        currentState.Combine(e.currentWorldState, e.gameObject);
        //UIController.instance.SetState(currentState.ToString());
    }
}
