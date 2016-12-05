using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour {
	public TerrainData myTerrainData;
	public Vector3 worldSize;
	public int resolution = 1000;
    //public GameObject tree;

	public Texture2D myTexture;
	// Use this for initialization
	void Start () {
		TerrainCollider data = gameObject.GetComponent<TerrainCollider>();
		if (data != null) 
		{
			myTerrainData = data.terrainData;
			myTerrainData.size = worldSize;
			myTerrainData.heightmapResolution = resolution;
		}	
		SplatPrototype[] terrainTexture = new SplatPrototype[1];
		terrainTexture[0] = new SplatPrototype ();
		terrainTexture [0].texture = myTexture;
		myTerrainData.splatPrototypes = terrainTexture;
       /* for(int i = 0; i < worldSize.x; i++)
        {
            for(int j = 0; j < worldSize.z; j++)
            {
                if(Random.Range(0f,1f) < .004f)
                {
                    GameObject newTree = Instantiate(tree);
                    GameObject.Find("GameManager").GetComponent<GameManager>().trees.Add(newTree);
                    newTree.transform.position = new Vector3(i + .5f, 3.4f, j + .5f);
                }
            }
        }*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
