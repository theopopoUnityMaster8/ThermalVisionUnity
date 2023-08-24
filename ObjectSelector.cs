using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    // Duplicate the current object, put it in a thermal layer, and process ObjectThermal on it
    
    private GameObject dup;
    
    public int temperature = 80;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        dup = Instantiate(gameObject, gameObject.transform);
        dup.name = "thermal_" + gameObject.name;
        Destroy(dup.GetComponent<ObjectSelector>());
        Destroy(dup.GetComponent<Crest.OceanRenderer>());
        Destroy(dup.GetComponent<Follower>());
        
        dup.AddComponent<ObjectThermal>();
        dup.GetComponent<ObjectThermal>().temperature = temperature;
        
        
        dup.transform.position = gameObject.transform.position;
        dup.transform.rotation = gameObject.transform.rotation;
        dup.transform.localScale = new Vector3(1,1,1);

    }
    
    
    void Update(){
        dup.GetComponent<ObjectThermal>().temperature = temperature;
    }

}
