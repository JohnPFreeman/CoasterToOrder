using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSceneManager : MonoBehaviour
{
    public int currentLevel = 1;
    private OrderDataReader thisOrderReader;
    public GameObject orderPrefab;
    GameObject orderInstance;
    // Start is called before the first frame update
    void Start()
    {
        // Get the orders for the current level
        thisOrderReader = GetComponent<OrderDataReader>();
        for (int i = 0; i < thisOrderReader.orderData.Orders.Length; i++)
        {
            if (thisOrderReader.orderData.Orders[i].level == currentLevel)
            {
                // Add an order prefab to the scene
                orderInstance = Instantiate(orderPrefab);
                orderInstance.GetComponent<Order>().SetOrderFields(thisOrderReader.orderData.Orders[i]);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
