using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// You have to apply this script on every parent/prefab that you want to make appear bright in your thermal vision

public class ObjectThermal : MonoBehaviour
{
    
    public int temperature = 80;
    
    private Renderer rend = null;
    
    private Material thermal_mat;
    
    private Material[] mat_instance;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        // if have any Collider, change material to Thermal Mat of temperature 
        //thermal_mat = Resources.Load<Material>("MatThermalVis");
        Material thermal_mat_tmp = Resources.Load<Material>("HDRP_Thermal_Mat");
        
        
        // Verify if the Resources are put in the Resources folder
        if (thermal_mat_tmp == null){
            throw new UnassignedReferenceException("TATV : Resources files must be copy/paste to the Assets > Resources > folder of your project. Check ReadMe for further explanations.");
        }
        
        thermal_mat = Instantiate(thermal_mat_tmp);
        
        
        gameObject.layer = LayerMask.NameToLayer("Thermal");
        
        rend = GetComponent<Renderer>();
        
        if( rend != null ){
            // foreach materials
            mat_instance = rend.materials;
            Material[] tmp_materials = mat_instance;
            for(int i = 0; i < mat_instance.Length; i++){
                tmp_materials[i] = thermal_mat;
            }
            
            rend.materials = tmp_materials;
        }
        
        // and for every children, do the same
        
        int children = transform.childCount;
        for (int i = 0; i < children; ++i){
            //print("For loop: " + transform.GetChild(i));
            transform.GetChild(i).gameObject.AddComponent<ObjectThermal>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Temperature parameter is hidden with : Vector1_f14b5e1c8cef440ba1da9a4dd0c8889c
        if( rend != null ){
            for(int i = 0; i < mat_instance.Length; i++){
                    mat_instance[i].SetFloat("temperature", temperature);
                    //mat_instance[i].temperature = temperature;
                    rend.materials = mat_instance;
                }
            }
            
        else{
            int children = transform.childCount;
            for (int i = 0; i < children; ++i){
                transform.GetChild(i).gameObject.GetComponent<ObjectThermal>().temperature = temperature;
            }
        }
    }
}
