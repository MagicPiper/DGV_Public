using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TerrainManager : MonoBehaviour
    {
        private TreeInstance[] original;

        public Terrain terrain;
        public Material transparent;
        private List<GameObject> transparentTrees = new List<GameObject>();
        private Bounds terrainBounds;
        private bool terrainBoundsPopulated;

        // Use this for initialization
        void Start()
        {
            original = DeepCopyTreeInstances(terrain.terrainData.treeInstances);
        }

        public Bounds TerrainBounds()
        {
            if (!terrainBoundsPopulated)
            {
                terrainBoundsPopulated = true;

                var size = new Vector3(terrain.terrainData.bounds.size.x, terrain.terrainData.bounds.size.y + 100, terrain.terrainData.bounds.size.z);
                terrainBounds = new Bounds(terrain.terrainData.bounds.center, size);
                terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, size);

                Debug.Log(terrainBounds.ToString());
                Debug.Log(terrain.terrainData.bounds.ToString());
            }

            return terrainBounds;
        }

        private void OnApplicationQuit()
        {
            if (Application.isEditor)
            {
                terrain.terrainData.treeInstances = original;
            }
        }

        private void OnDestroy()
        {
            if (Application.isEditor)
            {
                terrain.terrainData.treeInstances = original;
            }
        }

        public void HideTrees(Vector3 pos)
        {
            RemoveTransparentTrees();

            terrain.terrainData.treeInstances = original;
            List<TreeInstance> newTreeInstances = new List<TreeInstance>();
            List<TreeInstance> removedTrees = new List<TreeInstance>();

            for (int i = 0; i < terrain.terrainData.treeInstances.Length; i++)
            {
                var tree = terrain.terrainData.treeInstances[i];
                float distance = Vector3.Distance(Vector3.Scale(tree.position, terrain.terrainData.size) + Terrain.activeTerrain.transform.position, pos);

                if (distance > 5)
                {
                    newTreeInstances.Add(tree);
                }
                else
                {
                    removedTrees.Add(tree);
                }
            }

            //Point the terrain data to the new list of trees:
            terrain.terrainData.treeInstances = new TreeInstance[newTreeInstances.Count];
            terrain.terrainData.treeInstances = newTreeInstances.ToArray();

            TransparentTrees(removedTrees);

        }

        private void TransparentTrees(List<TreeInstance> removedTrees)
        {
            foreach (TreeInstance tree in removedTrees)
            {
                Vector3 pos = Vector3.Scale(tree.position, terrain.terrainData.size) + terrain.transform.position;
                TreePrototype treeProt = terrain.terrainData.treePrototypes[tree.prototypeIndex];
                GameObject prefab = treeProt.prefab;
                GameObject obj = GameObject.Instantiate(prefab, pos, Quaternion.AngleAxis(tree.rotation * Mathf.Rad2Deg, Vector3.up)) as GameObject;
                MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();

                renderer.material = transparent;

                if (renderer.materials.Length > 1)
                {
                    Material[] matArray = renderer.materials;
                    matArray[1] = transparent;
                    renderer.materials = matArray;
                }

                obj.layer = 2;

                Transform t = obj.transform;
                t.localScale = new Vector3(t.localScale.x * tree.widthScale, t.localScale.y * tree.heightScale, t.localScale.z * tree.widthScale);
                t.rotation = Quaternion.AngleAxis(tree.rotation * Mathf.Rad2Deg, Vector3.up);

                transparentTrees.Add(obj);
            }
        }

        private void RemoveTransparentTrees()
        {
            foreach (GameObject tree in transparentTrees)
            {
                Destroy(tree);
            }
        }

        TreeInstance[] DeepCopyTreeInstances(TreeInstance[] source)
        {
            //A TreeInstance array for all the trees on one of the terrains.
            TreeInstance[] treeInstance = new TreeInstance[source.Length];

            //Iterate over each treeInstance in the source (the original trees in the scene) and copy them into the new treeInstance array:
            for (int i = 0; i < source.Length; i++)
            {
                treeInstance[i].color.a = source[i].color.a;
                treeInstance[i].color.r = source[i].color.r;
                treeInstance[i].color.g = source[i].color.g;
                treeInstance[i].color.b = source[i].color.b;

                treeInstance[i].heightScale = source[i].heightScale;

                treeInstance[i].lightmapColor.a = source[i].lightmapColor.a;
                treeInstance[i].lightmapColor.r = source[i].lightmapColor.r;
                treeInstance[i].lightmapColor.g = source[i].lightmapColor.g;
                treeInstance[i].lightmapColor.b = source[i].lightmapColor.b;

                treeInstance[i].position.x = source[i].position.x;
                treeInstance[i].position.y = source[i].position.y;
                treeInstance[i].position.z = source[i].position.z;

                treeInstance[i].prototypeIndex = source[i].prototypeIndex;

                treeInstance[i].rotation = source[i].rotation;
                treeInstance[i].widthScale = source[i].widthScale;
            }

            return treeInstance;
        }

    }
}
