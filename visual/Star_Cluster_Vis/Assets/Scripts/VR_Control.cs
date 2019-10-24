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
        float r_Trigger = OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger);
        float l_Trigger = OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger);
        if(r_Trigger > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Translate(new Vector3(0, MOVE_INC, 0), Space.World);
        }
        else if(l_Trigger > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.Translate(new Vector3(0, -MOVE_INC, 0), Space.World);
        }

        //Scaling up and down.
        vec = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        if(vec.y > VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.localScale += new Vector3(SCALE_INC, SCALE_INC, SCALE_INC);
        }
        else if(vec.y < -VR_CONTROLLER_THRESHOLD)
        {
            particleContainerTransform.localScale += new Vector3(-SCALE_INC, -SCALE_INC, -SCALE_INC);
        }

    }
}
