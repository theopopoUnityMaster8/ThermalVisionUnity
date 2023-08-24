using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainThermal : MonoBehaviour
{

    // This script is to be put on a terrain gameobject

    // Idea : Duplicate terrain, change terrain material, duplicate all prefabs adding "_thermal" ni the name and change material of them
    // Reassign the duplicated prefabs to the duplicated terrain


    private GameObject dupGO;

    public int temperature = 40;

    public int leaf_material_position = 1;

    private GameObject[] prefabs;
    private GameObject[] dup_prefabs;

    private Material thermal_mat;
    private Material[] mat_instance;

    private TerrainData terrain_data;
    private TerrainData dup_terrain_data;

    private TreePrototype[] treePrototype; 
    private TreePrototype[] dup_treePrototype;



    void Start(){

        /*
        dup = Instantiate(gameObject, gameObject.transform);
        dup.name = "thermal_" + gameObject.name;
        Destroy(dup.GetComponent<TerrainThermal>());
        Destroy(dup.GetComponent<ObjectSelector>());
        Destroy(dup.GetComponent<Crest.OceanRenderer>());
        Destroy(dup.GetComponent<Follower>());
        */


        
        terrain_data = gameObject.GetComponent<Terrain>().terrainData;

        if(terrain_data == null){
            Debug.Log("Terrain Data is null");
        }
        if(!gameObject.GetComponent<Terrain>()){
            Debug.Log("Component Terrain is NULL");
        }


        dup_terrain_data = Clone(terrain_data);

        if(dup_terrain_data == null){
            Debug.Log("Duplicated Terrain Data failed");
        }

        dupGO = Terrain.CreateTerrainGameObject(dup_terrain_data);
        dupGO.name = "thermal_" + gameObject.name;
        dupGO.layer = LayerMask.NameToLayer("Thermal");



        Material thermal_mat_tmp = Resources.Load<Material>("HDRP_Thermal_Mat");
        
        // Verify if the Resources are put in the Resources folder
        if (thermal_mat_tmp == null){
            throw new UnassignedReferenceException("TATV : Resources files must be copy/paste to the Assets > Resources > folder of your project. Check ReadMe for further explanations.");
        }
        
        thermal_mat = Instantiate(thermal_mat_tmp);
        thermal_mat.SetFloat("temperature", temperature);


        prefabs = new GameObject[terrain_data.treePrototypes.Length];
        dup_prefabs = new GameObject[terrain_data.treePrototypes.Length];

        for(int i = 0; i < terrain_data.treePrototypes.Length; i++)
        {
            prefabs[i] = terrain_data.treePrototypes[i].prefab;
        }

        // Duplicate all prefabs used in the terrain
        for(int i = 0; i < prefabs.Length; i++)
        {
                dup_prefabs[i] = Instantiate(prefabs[i], transform);
                dup_prefabs[i].name = prefabs[i].name + "_thermal";

                // modify tree prefab used by the duplicated terrain
                Renderer rend = dup_prefabs[i].GetComponentInChildren(typeof(Renderer)) as Renderer;

                if( rend != null ){
                // foreach materials
                mat_instance = rend.materials;
                Material[] tmp_materials = mat_instance;
                for(int k = 0; k < mat_instance.Length; k++){
                    if( k == leaf_material_position ){
                        // We set the thermal leaf material
                        tmp_materials[k].color = new Color(temperature/100,temperature/100,temperature/100,1);
                        tmp_materials[k].SetColor("_SpecularColor",new Color(temperature/100,temperature/100,temperature/100,1));
                        tmp_materials[k].SetColor("_EmissiveColor",new Color(temperature/100,temperature/100,temperature/100,1));
                        tmp_materials[k].SetInt("_UseEmissiveIntensity", 1);
                        tmp_materials[k].SetInt("_MaterialID", 4);
                        tmp_materials[k].SetFloat("_UseShadowThreshold", 0.5f);
                        tmp_materials[k].SetFloat("_Smoothness", 0.8f);

                    }
                    else{
                        // We set the common material
                        tmp_materials[k] = thermal_mat;
                    }

                    // And we apply for all the children of prefab
                    
                    int children = dup_prefabs[i].transform.childCount;
                    
                    for (int l = 0; l < children; ++l){
                    //print("For loop: " + transform.GetChild(i));
                        dup_prefabs[i].transform.GetChild(l).gameObject.AddComponent<ObjectThermal>();
                    }
                    
                    
                    
                    
                }
                
                rend.materials = tmp_materials;

                // We set the light layer to everything but the natural light
                rend.renderingLayerMask = 0 << 0;
                rend.renderingLayerMask = 1 << 8;

            }

                //dup_prefabs[i] = PrefabUtility.SaveAsPrefabAsset(dup_prefabs[i],""); 
        }

        dup_treePrototype = dup_terrain_data.treePrototypes;
         
        for(int i = 0; i < dup_treePrototype.Length; i++)
        {
            dup_treePrototype[i].prefab = dup_prefabs[i];
        }


        dup_terrain_data.treePrototypes = dup_treePrototype;
        dupGO.GetComponent<Terrain>().terrainData = dup_terrain_data;



        // PrefabUtility.PrefabInstanceUpdated

        dup_terrain_data.RefreshPrototypes(); 


        // Finally, change terrain parameters
        dupGO.GetComponent<Terrain>().materialTemplate = thermal_mat;
        dupGO.GetComponent<Terrain>().treeBillboardDistance = 500;
        dupGO.GetComponent<Terrain>().treeCrossFadeLength = 500;


    }


    // CLone Code found on : https://gist.github.com/zsoi/c965ff38938cd126f00d

    public static TerrainData Clone(TerrainData original)
		{
			TerrainData dup = new TerrainData();

			dup.alphamapResolution = original.alphamapResolution;
			dup.baseMapResolution = original.baseMapResolution;

			dup.detailPrototypes = CloneDetailPrototypes(original.detailPrototypes);
            //Debug.Log("Details cloned");

			// The resolutionPerPatch is not publicly accessible so
			// it can not be cloned properly, thus the recommendet default
			// number of 16
			dup.SetDetailResolution(original.detailResolution, 16);

			dup.heightmapResolution = original.heightmapResolution;
			dup.size = original.size;

			dup.splatPrototypes = CloneSplatPrototypes(original.splatPrototypes);
            //Debug.Log("SplatProto cloned");

			dup.thickness = original.thickness;
			dup.wavingGrassAmount = original.wavingGrassAmount;
			dup.wavingGrassSpeed = original.wavingGrassSpeed;
			dup.wavingGrassStrength = original.wavingGrassStrength;
			dup.wavingGrassTint = original.wavingGrassTint;
            //Debug.Log("WavingGrass param cloned");

			dup.SetAlphamaps(0, 0, original.GetAlphamaps(0, 0, original.alphamapWidth, original.alphamapHeight));
			dup.SetHeights(0, 0, original.GetHeights(0, 0, original.heightmapResolution, original.heightmapResolution));
            //Debug.Log("Alphamaps and Heights set");

			for (int n = 0; n < original.detailPrototypes.Length; n++)
			{
				dup.SetDetailLayer(0, 0, n, original.GetDetailLayer(0, 0, original.detailWidth, original.detailHeight, n));
			}
			dup.treePrototypes = CloneTreePrototypes(original.treePrototypes);

			dup.treeInstances = CloneTreeInstances(original.treeInstances);


			return dup;
		}

        static SplatPrototype[] CloneSplatPrototypes(SplatPrototype[] original)
		{
			SplatPrototype[] splatDup = new SplatPrototype[original.Length];

			for (int n = 0; n < splatDup.Length; n++)
			{
				splatDup[n] = new SplatPrototype
				{
					metallic = original[n].metallic,
					normalMap = original[n].normalMap,
					smoothness = original[n].smoothness,
					specular = original[n].specular,
					texture = original[n].texture,
					tileOffset = original[n].tileOffset,
					tileSize = original[n].tileSize
				};
			}

			return splatDup;
		}

        static DetailPrototype[] CloneDetailPrototypes(DetailPrototype[] original)
		{
			DetailPrototype[] protoDuplicate = new DetailPrototype[original.Length];

			for (int n = 0; n < original.Length; n++)
			{
				protoDuplicate[n] = new DetailPrototype
				{
					bendFactor = original[n].bendFactor,
					dryColor = original[n].dryColor,
					healthyColor = original[n].healthyColor,
					maxHeight = original[n].maxHeight,
					maxWidth = original[n].maxWidth,
					minHeight = original[n].minHeight,
					minWidth = original[n].minWidth,
					noiseSpread = original[n].noiseSpread,
					prototype = original[n].prototype,
					prototypeTexture = original[n].prototypeTexture,
					renderMode = original[n].renderMode,
					usePrototypeMesh = original[n].usePrototypeMesh,
				};
			}

			return protoDuplicate;
		}

		static TreePrototype[] CloneTreePrototypes(TreePrototype[] original)
		{
			TreePrototype[] protoDuplicate = new TreePrototype[original.Length];

			for (int n = 0; n < original.Length; n++)
			{
				protoDuplicate[n] = new TreePrototype
				{
					bendFactor = original[n].bendFactor,
					prefab = original[n].prefab,
				};
			}

			return protoDuplicate;
		}

		static TreeInstance[] CloneTreeInstances(TreeInstance[] original)
		{
			TreeInstance[] treeInst = new TreeInstance[original.Length];


			System.Array.Copy(original, treeInst, original.Length);

			return treeInst;
		}






    
}
