using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.HighDefinition;



// This script you assign to your MainCamera and creates itslef the Thermal Vision Camera and the Environment Post-Processing


public class ThermalVisionCam : MonoBehaviour
{
    
    public int Thermal_Vision_Display = 1;
    public int Basic_Vision_Display = 2;
    
    private Camera mainCam;
    
    private GameObject cloneCam;
    private Camera TRUECam;
    
    private GameObject ThermalCam;
    private Camera TVcam;
    
    private GameObject EnvironmentPP;
    private Volume volume_env;
    private VolumeProfile profile_env; 
    
    private GameObject ThermalPP;
    private Volume volume_therm;
    private VolumeProfile profile_therm;
    
    private string m_Path;

    
    
    // Start is called before the first frame update
    void Start()
    {
        
        // Make sure that the Thermal Layer is created
        int layer_verif = LayerMask.NameToLayer("Thermal");
        if (layer_verif==-1){
            throw new UnassignedReferenceException("TATV : Layer 'Thermal' must be created in Layer Manager. Check ReadMe for further explanations.");
        }
        
        
        // Get the path to the script :
        m_Path = Application.dataPath + ("/TATV-TrueAutomatedThermalVision");
        

        
        // Clone Camera
        // Here "gameObject" = this
        
        cloneCam = new GameObject("tmp_MainCam");
        Vector3 pos = gameObject.transform.position;
        cloneCam.transform.parent = gameObject.transform;

        // Apply offset between the thermal cam and the EO
        // 
        pos.x = pos.x - 0.1f*Config.GetXOffset();
        pos.y = pos.y - 0.1f*Config.GetYOffset();
        cloneCam.transform.position =  pos;
        
        cloneCam.transform.rotation = gameObject.transform.rotation;
        cloneCam.transform.localScale = gameObject.transform.localScale;
        //cloneCam.AddComponent<MoveCamera>();
        TRUECam = cloneCam.AddComponent<Camera>();
        TRUECam.targetDisplay = Basic_Vision_Display - 1;
        TRUECam.cullingMask =  1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Water");
        TRUECam.farClipPlane = 10000;
        //Splitscreen
        if(Thermal_Vision_Display == Basic_Vision_Display){
            TRUECam.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
        }
    
        
        mainCam = gameObject.GetComponent<Camera>();
        var MainCameraData = mainCam.GetComponent<HDAdditionalCameraData>();
        // Field of view 60 Horizontal
        mainCam.fieldOfView = 60;
        mainCam.targetDisplay = Thermal_Vision_Display - 1;
        mainCam.cullingMask =  1 << LayerMask.NameToLayer("Thermal"); //  | 1 << LayerMask.NameToLayer("Default")
        MainCameraData.volumeLayerMask = LayerMask.NameToLayer("Nothing");
        MainCameraData.backgroundColorHDR = Color.black;
        MainCameraData.volumeLayerMask = 1 << LayerMask.NameToLayer("Thermal");
        if(Thermal_Vision_Display == Basic_Vision_Display){
            mainCam.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
        }
        

        
        
        
    }
}
