using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JointIK : MonoBehaviour
{
    //this script re implaments object parenting
    public Vector3 axis;
    public float distanceThreshold;
    public Vector3 startOffset;

    public float learningRate = 2;

    public GameObject[] Joints;

    void Awake()
    {
        startOffset = transform.localPosition;
    }

    //returns the postion sof the next point in the hierarchy 
    public Vector3 ForwardKinematics(float[] angles)
    {

        Vector3 prevPoint = Joints[0].transform.position;
        JointIK[] jointScript = new JointIK[Joints.Length];

        for (int i = 0; i < Joints.Length; i++)
        {
            jointScript[i] = Joints[i].GetComponent<JointIK>();
        }

        Quaternion rotation = Quaternion.identity;

        for (int i = 1; i < Joints.Length; i++)
        {
            // Rotates around a new axis
            rotation *= Quaternion.AngleAxis(angles[i - 1], jointScript[i - 1].axis);
            Vector3 nextPoint = prevPoint + rotation * jointScript[i].startOffset;
            prevPoint = nextPoint;
        }

        return prevPoint;
    }

    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }

    public float PartialGradient(Vector3 target, float[] angles, int i, float samplingDistance)
    {
        //saves the angle
        //will be restored later
        float angle = angles[i];

        //Gradient : [F(X + SamplingDistance) - F(X)]/h
        float f_x = DistanceFromTarget(target, angles);

        angles[i] += samplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);
        float gradient = (f_x_plus_d - f_x) / samplingDistance;
        //restores
        angles[i] = angle;
        return gradient;
    }

    void GradientDecentKinematics(Vector3 target, float[] angles, float samplingDistance)
    {
        if (DistanceFromTarget(target, angles) < distanceThreshold)
        {
            return;
        }

        for (int i = Joints.Length - 1; i >= 0; i--)
        {
            //Gradient decent
            //Update: Solution -= learningRate * Gradient
            float gradient = PartialGradient(target, angles, i, samplingDistance);
            angles[i] -= learningRate * gradient;
            if (DistanceFromTarget(target, angles) < distanceThreshold)
            {
                return;
            }

        }
    }
}
