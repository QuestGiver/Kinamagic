using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FABRICK_Library
{



    static void FABRICK()
    {
        //while (abs(endEffectorPosition — goalPosition) > EPS)
        //{
        //    FinalToRoot(); // PartOne
        //    RootToFinal(); // PartTwo
        //}
    }


    static void FinalToRoot(Transform goal, FABRIK_Joint[] Limbs)
    {
        FABRIK_Joint currentLimb;
        Transform targetposition = goal;
        currentLimb = Limbs[Limbs.Length];//(last limb in array)
        currentLimb.incommingPosition = currentLimb.transform.position;


        while (currentLimb != null)
        {
            currentLimb.transform.rotation = Vector3.Angle(currentLimb.transform.rotation, targetposition.position - currentLimb.incommingPosition);
            currentLimb.outboardPosition = target position;
            target position = currentLimb.incommingPosition;
            currentLimb = next limb furthur within the treebranch;
        }
    }



    static void RootToFinal(Transform goal, FABRIK_Joint[] Limbs)
    {
        //currentIncommingPosition = rootLimb.incommingPosition;(first limb in array)
        //currentLimb = rootLimb;


        //while (currentLimb != NULL)
        //{
        //    currentLimb.incommingPosition = currentincommingPosition;
        //    currentincommingPosition = currentLimb.outGoingPosition;
        //    currentLimb = next limb furthur away the treebranch root;
        //}
    }



}
