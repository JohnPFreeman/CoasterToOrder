using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;


[ExecuteAlways]
public class MovementPhysics : MonoBehaviour
{

    public GameObject cart;
    public float speed = 0;

    public float G = 9.8f;

    [Header("G-Force Variables")]
    public float currGForce = 0;
    public float maxGForce = 0;
    public float avgGForce = 0;
    public TMP_Text text;

    private List<float> allGForce = new List<float>();

    private Vector3 currV;
    private Vector3 prevV;


    // Start is called before the first frame update
    void Start()
    {
        speed = 0;

        currGForce = 0;
        maxGForce = 0;
        avgGForce = 0;
        allGForce = new List<float>();

        currV = cart.transform.right * speed;
    

    }

    // Update is called once per frame
    void Update()
    {
        
        SplineAnimate animation = cart.GetComponent<SplineAnimate>();


        float incline = 90 - Vector3.Angle(cart.transform.right, Vector3.up);
        incline *= 2 * Mathf.PI / 360;

        float gravityForce = Mathf.Sin(incline) * -G * Time.deltaTime;
        
        speed += gravityForce;

        speed = Mathf.Clamp(speed, 1f, 500f);


        prevV = currV;
        currV = cart.transform.right * speed;

        currGForce = (currV - prevV).magnitude / Time.deltaTime / G;

        if (currGForce > maxGForce)
        {
            maxGForce = currGForce;
        }

        allGForce.Add(currGForce);
        avgGForce = allGForce.Average();

        text.text = "Current GForce: " + currGForce.ToString("F3") + "\nMax GForce: " + maxGForce.ToString("F3") + "\nAverage GForce: " + avgGForce.ToString("F3");


        animation = UpdateSpeed(animation, speed);
      
    }

    private SplineAnimate UpdateSpeed(SplineAnimate animation, float newSpeed)
    {
        float prevProgress = animation.NormalizedTime;
        animation.MaxSpeed = newSpeed;
        animation.NormalizedTime = prevProgress;

        return animation;
    }
}
