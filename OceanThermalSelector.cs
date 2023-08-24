using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AnimationModule;

public class OceanThermalSelector : MonoBehaviour
{
    // This script must be applied on the Ocean object of your scene.
    // It is not possible to affect a thermal material on the ocean, so we have to disable the original ocean and create a simulation of it
    // This script will disable the current Ocean and create a plane at the same position, with a realistic Thermal Sea Material
    
    // Start is called before the first frame update
    void Start()
    {
        
        Material thermal_water_mat = Resources.Load<Material>("Thermal_Water_Mat");
        
        
        Plane plane = new Plane(Vector3.up,transform.position);
        GameObject planeGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planeGO.transform.localScale = new Vector3(1000, 1, 1000);
 
        Renderer rend = planeGO.GetComponent<Renderer>();
        
        rend.material = thermal_water_mat;
        
        planeGO.layer = LayerMask.NameToLayer("Thermal");
        
        
        // Now we add the animation
        // RuntimeAnimatorController water_ctrl = Resources.Load("Water_ctrl"); //Only in EDITOR
        Animator plane_anim = planeGO.AddComponent<Animator>();
        plane_anim.runtimeAnimatorController = Resources.Load("Water_ctrl") as RuntimeAnimatorController;
        
        
    }

}
