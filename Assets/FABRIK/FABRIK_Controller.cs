﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FABRIK_Joint
{
    [HideInInspector]
    public Transform joint;

    public float orientationLimmit;
    //public float xRotationLimmit;
    //public float yRotationLimmit;
    //public float zRotationLimmit;

}

public class FABRIK_Controller : MonoBehaviour
{
    [SerializeField]
    Transform goalPosition;

    [SerializeField]
    float orientationLimmitOverride;

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
            if (orientationLimmitOverride != 0)
            {
                JointInfo[i].orientationLimmit = orientationLimmitOverride;
            }

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


        for (int i = 0; i < JointInfo.Length-1; i++)
        {
            JointInfo[i].joint.LookAt(JointInfo[i + 1].joint, Vector3.up);
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




                        float angle = Vector3.SignedAngle(jointPosition[(jointPosition.Length - 1) - i].joint.position, jointPosition[(jointPosition.Length - 1) - (i + 1)].joint.position, -jointPosition[(jointPosition.Length - 1) - i].joint.forward);

                        if (angle > jointPosition[(jointPosition.Length - 1) - i].orientationLimmit)
                        {
                            Vector3 shaftPoint = jointPosition[(jointPosition.Length - 1) - i].joint.position - (jointPosition[(jointPosition.Length - 1) - i].joint.forward * jointSeperationDistances[(jointSeperationDistances.Length - 1) - i]);
                            Vector3 shaftPointJoinDist = jointPosition[(jointPosition.Length - 1) - (i + 1)].joint.position - shaftPoint;

                            jointPosition[(jointPosition.Length - 1) - (i + 1)].joint.position = shaftPoint + (shaftPointJoinDist / shaftPointJoinDist.magnitude) * jointPosition[(jointPosition.Length - 1) - i].orientationLimmit;
                        }

                    }
                    else
                    {
                        jointPosition[(jointPosition.Length - 1) - i].joint.position// the joint just before the current forwardmost joint being proccessed, otherwise known as joint "A"
                            = jointPosition[(jointPosition.Length) - i].joint.position//current forwardmost joint being proccessed, otherwise known as joint "B"
                            + ((jointPosition[(jointPosition.Length - 1) - (i + 1)].joint.position) - jointPosition[(jointPosition.Length) - i].joint.position).normalized//the normalized direction between joint A and joint B
                            * jointSeperationDistances[(jointSeperationDistances.Length - 1) - i];//the recorded distance between joint A and joint B, being used as a limb/distance restraint via scaling the given direction  

                        float angle = Vector3.SignedAngle(jointPosition[(jointPosition.Length) - i].joint.position, jointPosition[(jointPosition.Length - 1) - (i + 1)].joint.position, -jointPosition[(jointPosition.Length - 1) - i].joint.forward);

                        if (angle > jointPosition[(jointPosition.Length) - i].orientationLimmit)
                        {
                            Vector3 shaftPoint = jointPosition[(jointPosition.Length) - i].joint.position - (jointPosition[(jointPosition.Length) - i].joint.forward * jointSeperationDistances[(jointSeperationDistances.Length - 1) - i]);
                            Vector3 shaftPointJoinDist = jointPosition[(jointPosition.Length - 1) - i].joint.position - shaftPoint;

                            jointPosition[(jointPosition.Length - 1) - i].joint.position = shaftPoint + (shaftPointJoinDist / shaftPointJoinDist.magnitude) * jointPosition[(jointPosition.Length) - i].orientationLimmit;
                        }

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



                    float angle = Vector3.SignedAngle(jointPosition[i].joint.position, jointPosition[i + 1].joint.position, -jointPosition[i].joint.position);

                    if (angle > jointPosition[i].orientationLimmit)
                    {
                        Vector3 shaftPoint = jointPosition[i].joint.position - (jointPosition[i].joint.forward * jointSeperationDistances[i]);
                        Vector3 shaftPointJoinDist = jointPosition[i + 1].joint.position - shaftPoint;

                        jointPosition[i + 1].joint.position = shaftPoint + (shaftPointJoinDist / shaftPointJoinDist.magnitude) * jointPosition[i].orientationLimmit;
                    }

                }
                //---------------------------------------------------------------------------------------------------
            }
        }






    }


    Vector3 Orientation(Vector3 forwardJoint, Vector3 currentJoint)
    {


        Vector2 xy_f = new Vector2(forwardJoint.x, forwardJoint.y);
        Vector2 xy_c = new Vector2(currentJoint.x, currentJoint.y);
        float zAxisAngle = Vector2.SignedAngle(xy_f, xy_c);

        Vector2 zy_f = new Vector2(forwardJoint.z, forwardJoint.y);
        Vector2 zy_c = new Vector3(currentJoint.z, currentJoint.y);
        float yAxisAngle = Vector2.SignedAngle(zy_f, zy_c);

        Vector2 zx_f = new Vector2(forwardJoint.z, forwardJoint.x);
        Vector2 zx_c = new Vector2(currentJoint.z, currentJoint.x);
        float xAxisAngle = Vector2.SignedAngle(zx_f, zx_c);

        return new Vector3(xAxisAngle, yAxisAngle, zAxisAngle);


    }

}
