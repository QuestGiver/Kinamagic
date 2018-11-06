using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KinaJoint : MonoBehaviour
{
    public Vector3 Axis;
    public Vector3 StartOffset;


    void Awake()
    {
        StartOffset = transform.localPosition;
    }

    public Vector3 ForwardKinematics(float[] angles, Transform[] Joints)
    {
        Vector3 prevPoint = Joints[0].position;
        Quaternion rotation = Quaternion.identity;

        for (int i = 1; i < Joints.Length; i++)
        {
            // *= Quaternion.AngleAxis(Joints[i].)
        }

        return prevPoint;
    }
}
//Had to do this whole thing. (assuming[code] makes code blocks)

//[code]
//public Vector3 ForwardKinematics (float[]
//angles, GameObject []
//Joints) {

//    Vector3 prevPoint = Joints[0].transform.position;
//    JointIK[] JointScript = new JointIK[Joints.Length];
//    for (int i = 0; i < Joints.Length; i++)
//    {

//        JointScript[i] = Joints[i].GetComponent("JointIK") as JointIK;

//    }
//    Quaternion rotation = Quaternion.identity;
//    for (int i = 1; i < Joints.Length; i++)
//    {
//        // Rotates around a new axis
//        rotation *= Quaternion.AngleAxis(angles[i – 1], JointScript[i - 1].Axis);
//        Vector3 nextPoint = prevPoint + rotation * JointScript[i].StartOffset;

//        prevPoint = nextPoint;
//    }
//    return prevPoint;

//}
//[/code]
