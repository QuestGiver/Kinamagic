using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FABRICK_Controller : MonoBehaviour
{
    [SerializeField]
    Transform goalPosition;

    [SerializeField]
    Transform[] Joints;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FABRIK(Joints, goalPosition, 0.01f);
    }


    public void FABRIKII(Transform[] jointPosition, Transform targetPosition, float tolerance)
    {

    }

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



            while (tolerance < endJointTargetDist)
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
}
