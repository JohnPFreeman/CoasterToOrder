using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script should be attached to the OrderSceneManager object
// It reads the JSON order data and turns it into a Unity object
public class OrderDataReader : MonoBehaviour
{
    public TextAsset OrderDataJSON;

    [System.Serializable]
    public class OrderData
    {
        public int level; // Which level this order will be presented in 
        public int xPosition; // To set the order as left, center, or right
        public string customerName;
        public int budget;
        public int[] dimensions; // [x, y]
        //public string font;
        public string description;
    }

    [System.Serializable]
    public class OrdersList
    {
        public OrderData[] Orders;
    }

    public OrdersList orderData = new OrdersList();

    private void Awake()
    {
        orderData = JsonUtility.FromJson<OrdersList>(OrderDataJSON.text);
    }


}
