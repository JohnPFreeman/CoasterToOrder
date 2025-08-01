using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;


[ExecuteAlways]
public class MovementPhysics : MonoBehaviour
{

    public GameObject cart;
    public float speed = 0;


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
        SplineAnimate animation = cart.GetComponent<SplineAnimate>();


        float incline = 90 - Vector3.Angle(cart.transform.right, Vector3.up);
        incline *= 2 * Mathf.PI / 360;

        float gravityForce = Mathf.Sin(incline) * -9.8f * Time.deltaTime;
        
        speed += gravityForce;
        Debug.Log(gravityForce);

        speed = Mathf.Clamp(speed, 1f, 50f);


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
