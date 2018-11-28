using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FABRICK_Controller : MonoBehaviour
{
    [SerializeField]
    Transform goalPosition;

    [SerializeField]
    FABRIK_Joint[] Joints;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FABRIK(Transform[] jointPosition, Transform targetPosition, float tolerance)//joint positions; p
    {
        float[] jointSeperationDistances = new float[jointPosition.Length - 1];//d

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
            float[] JointSepOverTargDist = new float[jointPosition.Length];//lambda

            for (int i = 0; i < jointPosition.Length; i++)
            {
                JointTargetDistance[i] = Vector3.Distance(jointPosition[i].position, targetPosition.position);
                JointSepOverTargDist[i] = jointSeperationDistances[i] / JointTargetDistance[i];
            }
            //find new join position
            for (int i = 0; i < jointPosition.Length; i++)
            {
                jointPosition[i + 1].position = (1 - JointSepOverTargDist[i]) * jointPosition[i].position + JointSepOverTargDist[i] * targetPosition.position;
            }
        }
        else
        {
            //target is within reach
            Transform rootB = jointPosition[0];//b

            Transform endEffector = jointPosition[jointPosition.Length];

            while (tolerance > Vector3.Distance(jointPosition[jointPosition.Length].position, targetPosition.position))
            {
                //stage 1: forwards reaching

                //set end effector (jointPosition[jointPosition.Length].transform) as target
                //Pn = t

                jointPosition[jointPosition.Length] = targetPosition;


                float[] jointEndEffectorDistance = new float[jointPosition.Length];//r
                float[] JointSepOverTargDist = new float[jointPosition.Length];//lambda
                for (int i = 0; i < jointPosition.Length; i++)
                {
                    //find the distance (Ri) between the new joint position (Pi+1) and the joint (Pi)
                    jointEndEffectorDistance[i] = Vector3.Distance(jointPosition[i + 1].position, jointPosition[i].position);
                }

                for (int i = 0; i < jointPosition.Length; i++)
                {
                    JointSepOverTargDist[i] = jointSeperationDistances[i] / jointEndEffectorDistance[i];
                }

                //find the new joint positions
            }



        }




    }


}
