using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SubChain
{
    [SerializeField]
    Transform[] subJoints;
    [SerializeField]
    SubChain[] subChains;
}

[System.Serializable]
public struct RootChain
{
    [SerializeField]
    Transform[] rootJoints;
    [SerializeField]
    SubChain[] subChains;
}

public class FABRIK_MultiChain : MonoBehaviour
{
    [SerializeField]
    RootChain[] chains;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FABRIK_Multi()
    {

    }
}
