using UnityEngine;

public class TerrainSettingsManager : MonoBehaviour {
        
    [SerializeField] private Terrain template;

    //The higher the numbers, the more hills/mountains there are
    [SerializeField]
    private float HM = 0f;

    //The lower the numbers in the number range, the higher the hills/mountains will be...
    [SerializeField]
    private float divRange = 0f;

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        TerrainData oldTData = GetComponent<Terrain>().terrainData;
        Terrain oldT = GetComponent<Terrain>();

        TerrainData tData = template.terrainData;

        oldTData.splatPrototypes = tData.splatPrototypes;
        oldTData.treePrototypes = tData.treePrototypes;
        oldTData.detailPrototypes = tData.detailPrototypes;

        oldT.heightmapPixelError = template.heightmapPixelError;
        oldT.basemapDistance = template.basemapDistance;
        oldT.materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
      //  oldT.bakeLightProbesForTrees = false;
        oldT.detailObjectDistance = 50;
        oldT.detailObjectDensity = 1;
        oldT.treeDistance = 1000;
        oldT.treeBillboardDistance = 50;
        oldT.treeCrossFadeLength = 50;
        oldT.treeMaximumFullLODCount = 50;

        oldTData.wavingGrassAmount = 0.5f;
        oldTData.wavingGrassSpeed = 0.3f;
        oldTData.wavingGrassStrength = 0.5f;
        oldTData.wavingGrassTint = tData.wavingGrassTint;
    }
    
    [ContextMenu("Randomize Heightmap")]
    public void GenerateTerrain()
    {
        var t = GetComponent<Terrain>();
        var tileSize = HM;
        //Heights For Our Hills/Mountains
        float[,] hts = new float[t.terrainData.heightmapWidth, t.terrainData.heightmapHeight];
        for (int i = 0; i < t.terrainData.heightmapWidth; i++)
        {
            for (int k = 0; k < t.terrainData.heightmapHeight; k++)
            {
                var test = Mathf.PerlinNoise(((float)i / (float)t.terrainData.heightmapWidth) * tileSize, ((float)k / (float)t.terrainData.heightmapHeight) * tileSize) / divRange;

                hts[i, k] = test;
               // Debug.Log(test);
            }
            }
        Debug.LogWarning("DivRange: " + divRange + " , " + "HTiling: " + HM);
        t.terrainData.SetHeights(0, 0, hts);
    }
}