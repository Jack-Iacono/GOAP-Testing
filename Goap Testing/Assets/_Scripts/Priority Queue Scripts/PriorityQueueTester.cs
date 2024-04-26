using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueueTester : MonoBehaviour
{
    PriorityQueue queue = new PriorityQueue(new List<PriorityQueue.Element>
        {
            new PriorityQueue.Element(6, 10),
            new PriorityQueue.Element(6, 10),
            new PriorityQueue.Element(6, 800),
            new PriorityQueue.Element(6, 72),
            new PriorityQueue.Element(5, 64),
            new PriorityQueue.Element(5, 52),
            new PriorityQueue.Element(5, 41),
            new PriorityQueue.Element(5, 344),
            new PriorityQueue.Element(5, 23),
            new PriorityQueue.Element(5, 0),
        }
        );

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            queue.Extract();
            queue.PrintHeap();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            queue.UpdateValue( 0, new PriorityQueue.Element(1,1000));
            queue.PrintHeap();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            queue.Insert(new PriorityQueue.Element(1, 115));
            queue.PrintHeap();
        }
    }
}
