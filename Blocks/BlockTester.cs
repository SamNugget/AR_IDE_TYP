using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTester : MonoBehaviour
{
    void Start()
    {
        GetComponent<Block>().initialise(-1, 0, new int[] { 3 });
    }
}
