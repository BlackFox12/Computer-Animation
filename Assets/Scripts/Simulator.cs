using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour
{

    public static Simulator _instance = null;
    public static Simulator GetInstance()
    {   
        if (_instance == null)
        {
            GameObject simulatorGameObject = new GameObject("Simulator");
            _instance = simulatorGameObject.AddComponent<Simulator>();
        }
        return _instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
