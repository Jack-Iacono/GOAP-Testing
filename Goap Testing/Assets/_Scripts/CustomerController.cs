using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    public static List<CustomerController> customers = new List<CustomerController>();

    private float hungerLevel;

    private WorldState currentState;

    private Property.Key hungerKey;

    public event EventHandler<bool> OnHungerStatusChanged;

    private void Awake()
    {
        customers.Add(this);
    }
    private void OnDestroy()
    {
        customers.Remove(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = new WorldState(new Dictionary<Property.Key, Property.Value>()
                {
                    { new Property.Key("is_hungry", gameObject), new Property.Value(false) }
                });

        hungerKey = new Property.Key("is_hungry", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(hungerLevel <= 40)
        {
            currentState.ChangeProperty(hungerKey, new Property.Value(true));
        }

        if(hungerLevel > 0)
        {
            hungerLevel -= Time.deltaTime;
        }
    }
    public void Eat()
    {
        hungerLevel = 10;
        currentState.ChangeProperty(hungerKey, new Property.Value(false));
    }
}
