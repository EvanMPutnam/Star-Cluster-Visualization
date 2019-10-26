using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Control : MonoBehaviour
{
    
    //Public object that scales/moves to control star location.
    public GameObject particleContainer;

    //Private transform (Assigned once for performance)
    private Transform particleContainerTransform;
    


    //Constant for scaling object.
    public float SCALE_INC = 0.0005f;
    //Constant for base scale factor
    public float SCALE_BASE = 0.0001f;
    //Constant for value to increment x/y/z by on an input
    public float MOVE_INC = 0.0025f;
    //VR controller threshold (How far controller button needs to be pressed)
    public float VR_CONTROLLER_THRESHOLD = 0.5f;
    //Value for rotation speed.
    public float ROTATION_SPEED = 0.1f;



    // Start is called before the first frame update
    void Start()
    {
        particleContainerTransform = particleContainer.GetComponent<Transform>();
        particleContainerTransform.localScale = new Vector3(SCALE_BASE, SCALE_BASE, SCALE_BASE);
    }
    

    // Update is called once per frame
    void Update()
    {
        //Vector for inputs on right thumbstick.
        Vector2 vec = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

        //Movement z direction
        if(vec.y > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Translate(new Vector3(0, 0, MOVE_INC), Space.World);
        }
        else if(vec.y < -VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Translate(new Vector3(0, 0, -MOVE_INC), Space.World);
        }
        
        //Movement x direction
        if(vec.x > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Translate(new Vector3(MOVE_INC, 0, 0), Space.World);
        }
        else if(vec.x < -VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Translate(new Vector3(-MOVE_INC, 0, 0), Space.World);
        }
        
        //Movement y direction
        float rUnderTrigger = OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger);
        float lUnderTrigger = OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger);
        if(rUnderTrigger > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Translate(new Vector3(0, MOVE_INC, 0), Space.World);
            
        }
        else if(lUnderTrigger > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Translate(new Vector3(0, -MOVE_INC, 0), Space.World);
        }

        //Scaling of starfield.
        float rTopTrigger = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        float lTopTrigger = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
        if(rTopTrigger > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.localScale += new Vector3(SCALE_INC, SCALE_INC, SCALE_INC);
        }
        else if(lTopTrigger > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.localScale += new Vector3(-SCALE_INC, -SCALE_INC, -SCALE_INC);
        }


        //Rotation of starfield.
        vec = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        //Rotate around the y axis
        if(vec.y > VR_CONTROLLER_THRESHOLD)
        {
           particleContainerTransform.Rotate(ROTATION_SPEED, 0, 0, Space.Self);
        }
        else if(vec.y < -VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Rotate(-ROTATION_SPEED, 0, 0, Space.Self);   
        }

        //Rotate around the x axis.
        if(vec.x > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Rotate(0, ROTATION_SPEED, 0, Space.Self);
        }
        else if(vec.x < -VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Rotate(0,  -ROTATION_SPEED, 0, Space.Self);
        }


    }
}
