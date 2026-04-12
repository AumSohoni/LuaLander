using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
public class Lander : MonoBehaviour
{

    private Rigidbody2D lanbderRb;
    [SerializeField] private float Force = 700f;
    [SerializeField] private float turnSpeed = 100f;
    private void Awake()
    {
        lanbderRb = GetComponent<Rigidbody2D>();   
    }
    
    private void FixedUpdate()
    {
        
        if(Keyboard.current.wKey.isPressed)
        {
           
            lanbderRb.AddForce(transform.up * Force * Time.deltaTime);
        }
         if(Keyboard.current.aKey.isPressed)
        {

            lanbderRb.AddTorque(turnSpeed * Time.deltaTime);
        }
         if(Keyboard.current.dKey.isPressed)
        {
            
            lanbderRb.AddTorque(-turnSpeed * Time.deltaTime);
        }

    }
}
