using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static OrderDataReader;

public class Order : MonoBehaviour
{
    private Vector3 initialPos;
    private Boolean isClicked = false;

    private OrderData orderData;
    public TextMeshPro NameText;
    public TextMeshPro BudgetText;
    public TextMeshPro DescriptionText;
    public GameObject approvedObject;

    public void SetOrderFields(OrderData order)
    {
        // Populate order object w/ the given order data and save the data to the var orderData
        orderData = order;
        initialPos = new Vector3(order.xPosition, 0, -1);
        transform.position = initialPos;
        NameText.text = order.customerName;
        BudgetText.text = "$" + order.budget.ToString();
        DescriptionText.text = order.description;
        approvedObject.SetActive(false);
    }

    public void approveOrder()
    {
        approvedObject.SetActive(true);
    }

    private void OnMouseEnter()
    {
        if (isClicked) return;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, -10), 1);
    }
    private void OnMouseExit()
    {
        if (isClicked) return;
        transform.position = Vector3.Lerp(transform.position, new Vector3(initialPos.x, initialPos.y, -1), 1);
    }
    private void OnMouseDown()
    {
        if (!isClicked)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0, -30), 1);
            isClicked = true;
            approveOrder();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(initialPos.x, initialPos.y, -1), 1);
            isClicked = false;
        }
    }
}
