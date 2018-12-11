using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FABRIK_Joint
{
    [HideInInspector]
    public Transform joint;
    public float xRotationLimmit;
    public float yRotationLimmit;
    public float zRotationLimmit;

}

public class FABRIK_Controller : MonoBehaviour
{
    [SerializeField]
    Transform goalPosition;

    [SerializeField]
    Transform[] jointTransforms;


    [SerializeField]
    FABRIK_Joint[] JointInfo;

    public float slack;

    public bool run = true;

    public bool reverseOrder = false;

    private void OnValidate()
    {
        if (reverseOrder)
        {
            

            for (int i = 0; i < jointTransforms.Length/2; i++)
            {
                Transform temp = jointTransforms[i];
                jointTransforms[i] = jointTransforms[(jointTransforms.Length - 1) - i];
                jointTransforms[(jointTransforms.Length - 1) - i] = temp;
            }
            
        }

        JointInfo = new FABRIK_Joint[jointTransforms.Length];

        for (int i = 0; i <jointTransforms.Length; i++)
        {
            JointInfo[i].joint = jointTransforms[i];
        }
        reverseOrder = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            FABRIK(JointInfo, goalPosition, 0.01f);
        }
    }

    private void OnDrawGizmos()
    {

        for (int i = 0; i < JointInfo.Length-1; i++)
        {

            Debug.DrawLine(JointInfo[i].joint.position, JointInfo[i+1].joint.position, Color.red);

        }
    }



    public void FABRIK(FABRIK_Joint[] jointPosition, Transform targetPosition, float tolerance)
    {

        //Functionwide variables=======================================================================================
        float[] jointSeperationDistances = new float[jointPosition.Length - 1];//distance between each joint
        float rootTargetDistance = Vector3.Distance(jointPosition[0].joint.position, targetPosition.position);//distance between the target and the root of the model, this needs to be a double in order to preserve distances
        float reach = 0;//assuming the link chain starts at "zero position", this is the total sum of distances between each joint
        //end functionwide variables===================================================================================

        //Initializing IK function------------------------------------------------------------------------------------------------------------------------------------------------------


        for (int i = 0; i < jointPosition.Length - 1; i++)//calculates the starting distance between each point within the jointArray
        {
            jointSeperationDistances[i] = Vector3.Distance(jointPosition[i + 1].joint.position, jointPosition[i].joint.position);
        }

        for (int i = 0; i < jointSeperationDistances.Length; i++)
        {
            reach += jointSeperationDistances[i] + slack;
        }


        //end initialization------------------------------------------------------------------------------------------------------------------------------------------------------------

        //Determining IK behavior(attempt get as close to the target as possible because it is out of range v.s attempt to touch the target because it is within range)

        if (reach < rootTargetDistance /*is the reach is greater than the distance from the target?*/)
        {
            //if the target is not within reach: have the model 'point' at the target

            //the variable containing the direction between the the root and the target;
            Vector3 targetRootDirection = (targetPosition.position - jointPosition[0].joint.position).normalized;


            //sets new positions for joints in the chain-----------
            for (int i = 0; i < jointPosition.Length - 1; i++)
            {
                float dist = 0;
                for (int j = 0; j < i + 1; j++)
                {
                    dist += jointSeperationDistances[j];
                }

                jointPosition[i + 1].joint.position = jointPosition[0].joint.position + (targetRootDirection * dist);

            }
            //----------------------------------------------------


        }
        else
        {
            //if the target is within reach: Have the model begin forwards and backwards reaching iterations

            Vector3 savedRootPosition = jointPosition[0].joint.position;//saves the root position
            float endJointTargetDist = Vector3.Distance(jointPosition[jointPosition.Length - 1].joint.position, targetPosition.position);//the distance between the end joint and the target
            endJointTargetDist = Vector3.Distance(jointPosition[jointPosition.Length - 1].joint.position, targetPosition.position);

            if (tolerance < endJointTargetDist)
            {
                //Forwards Reaching Phase+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                jointPosition[jointPosition.Length - 1].joint.position = targetPosition.position;//the ending joint is teleported to the target


                //the ending joint was moved to the target, this forloop simulates the end joint 'dragging' the other point along one at a time
                for (int i = 0; i < jointPosition.Length - 1; i++)
                {
                    if (i <= jointPosition.Length - 2)
                    {
                        jointPosition[(jointPosition.Length - 1) - (i + 1)].joint.position// the joint just before the current forwardmost joint being proccessed, otherwise known as joint "A"
                           = jointPosition[(jointPosition.Length - 1) - i].joint.position//current forwardmost joint being proccessed, otherwise known as joint "B"
                           + ((jointPosition[(jointPosition.Length - 1) - (i + 1)].joint.position) - jointPosition[(jointPosition.Length - 1) - i].joint.position).normalized//the normalized direction between joint A and joint B
                           * jointSeperationDistances[(jointSeperationDistances.Length - 1) - i];//the recorded distance between joint A and joint B, being used as a limb/distance restraint via scaling the given direction               

                    }
                    else
                    {
                        jointPosition[(jointPosition.Length - 1) - i].joint.position// the joint just before the current forwardmost joint being proccessed, otherwise known as joint "A"
                            = jointPosition[(jointPosition.Length) - i].joint.position//current forwardmost joint being proccessed, otherwise known as joint "B"
                            + ((jointPosition[(jointPosition.Length - 1) - (i + 1)].joint.position) - jointPosition[(jointPosition.Length) - i].joint.position).normalized//the normalized direction between joint A and joint B
                            * jointSeperationDistances[(jointSeperationDistances.Length - 1) - i];//the recorded distance between joint A and joint B, being used as a limb/distance restraint via scaling the given direction  
                    }

                }
                //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                //Backwards Reaching Phase---------------------------------------------------------------------------

                jointPosition[0].joint.position = savedRootPosition;//sets the root position back to its original position

                //the root was moved to it's original position, this forloop simulates the root joint 'dragging' the other joints along one at a time
                for (int i = 0; i < jointPosition.Length - 1; i++)
                {
                    jointPosition[i + 1].joint.position//the point just before the backwards most joint being processed, aka Joint "A"
                        = jointPosition[i].joint.position //the current backwards most joint being processed, aka Joint "B"
                        + (jointPosition[i + 1].joint.position - jointPosition[i].joint.position).normalized //the normalized direction between Joint A and Joint B
                        * jointSeperationDistances[i];// the recorded distance between the joint A and Joint B, being used as a limb/distance restraint via scaling the given direction
                }
                //---------------------------------------------------------------------------------------------------
            }
        }




    }
















































































































    /*
    public void FABRIK(Transform[] jointPosition, Transform targetPosition, float tolerance)//joint positions; p
    {
        float[] jointSeperationDistances = new float[jointPosition.Length - 1];//d
        float endJointTargetDist = Vector3.Distance(jointPosition[jointPosition.Length-1].position, targetPosition.position);
        //distance between root and target
        float rootDist = Vector3.Distance(jointPosition[0].position, targetPosition.position);

        for (int i = 0; i < jointPosition.Length - 1; i++)
        {

            jointSeperationDistances[i] = Vector3.Distance(jointPosition[i + 1].position, jointPosition[i].position);

        }

        //check if target is within reach
        float reach = 0;//assuming the link chain starts at "zero position"
        for (int i = 0; i < jointSeperationDistances.Length; i++)
        {
            reach += jointSeperationDistances[i];
        }
        if (rootDist > reach)
        {

            //target is not within reach

            //find the distance (joint target distance [i]) between targetPosition and the current joint
            float[] JointTargetDistance = new float[jointPosition.Length];//r
            float[] jointSepOverTargDist = new float[jointPosition.Length];//lambda

            for (int i = 0; i < jointPosition.Length-1 ; i++)
            {
                JointTargetDistance[i] = Vector3.Distance(jointPosition[i].position, targetPosition.position);
                jointSepOverTargDist[i] = jointSeperationDistances[i] / JointTargetDistance[i];
            }
            //find new join position
            for (int i = 0; i < jointPosition.Length-1; i++)
            {
                jointPosition[i + 1].position = (1 - jointSepOverTargDist[i]) * jointPosition[i].position + jointSepOverTargDist[i] * targetPosition.position;
            }

        }
        else
        {
            //target is within reach
            Transform rootB = jointPosition[0];//b

            Transform endEffector = jointPosition[jointPosition.Length-1];
            endJointTargetDist = Vector3.Distance(jointPosition[jointPosition.Length-1].position, targetPosition.position);



            while (tolerance > endJointTargetDist)
            {

                //stage 1: forwards reaching

                //set end effector (jointPosition[jointPosition.Length].transform) as target
                //Pn = t

                jointPosition[jointPosition.Length-1] = targetPosition;


                float[] jointEndEffectorDistance = new float[jointPosition.Length];//r
                float[] jointSepOverTargDist = new float[jointPosition.Length];//lambda
                for (int i = 0; i < jointPosition.Length-1; i++)
                {
                    //find the distance (Ri) between the new joint position (Pi+1) and the joint (Pi)
                    jointEndEffectorDistance[i] = Vector3.Distance(jointPosition[i + 1].position, jointPosition[i].position);
                }

                for (int i = 0; i < jointPosition.Length -1 ; i++)
                {
                    jointSepOverTargDist[i] = jointSeperationDistances[i] / jointEndEffectorDistance[i];
                }

                //find the new joint positions P[i]

                for (int i = 0; i < jointPosition.Length - 1; i++)
                {
                    jointPosition[i].position = ((1 - jointSepOverTargDist[i]) * jointPosition[i + 1].position) + jointSepOverTargDist[i] * jointPosition[i].position;
                }


                //Stage 2: Backwards reaching
                //set the root p[0] back to its initial value
                jointPosition[0] = rootB;
                float[] newJointPosDistance = new float[jointPosition.Length - 1];//r[i]
                float[] newjointSepOverTargDist = new float[jointPosition.Length];//lambda
                for (int i = 0; i < jointPosition.Length-1; i++)
                {
                    //find the distance (r[i]) between the new joint position (p[i])
                    newJointPosDistance[i] = Vector3.Distance(jointPosition[i + 1].position, jointPosition[i].position);
                    newjointSepOverTargDist[i] = jointSeperationDistances[i] / newJointPosDistance[i];
                    //find new joint positions p[i]
                    jointPosition[i].position = ((1 - newjointSepOverTargDist[i]) * jointPosition[i].position) + newjointSepOverTargDist[i] * jointPosition[i + 1].position;
                }

                // Check whether the distance between the end effector p[n] and the target t is greater than a tolerance.
                endJointTargetDist = Vector3.Distance(jointPosition[jointPosition.Length-1].position, targetPosition.position);

            }
        }
    }
    */

}
