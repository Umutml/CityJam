using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamebarUIController : MonoBehaviour
{
    [SerializeField] private RectTransform[] gamebarElements;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public RectTransform[] GetGamebarElements()
    {
        return gamebarElements;
    }
}
