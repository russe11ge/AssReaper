using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{

    public GameObject obj1;
    public GameObject obj2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Chair")
        {
            obj1.SetActive(true);   
        }
        if (other.tag == "Chair1")
        {
            obj2.SetActive(true);   
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Chair")
        {
            obj1.SetActive(false);
        }
         if (other.tag == "Chair1")
        {
            obj2.SetActive(false);
        }

    }
}
