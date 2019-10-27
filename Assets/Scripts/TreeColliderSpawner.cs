using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TreeColliderSpawner : MonoBehaviour
    {
        public Terrain terrain;
        public Transform holder;
        public BezierCurve line;
        public float distanceFromLine;
        public float min;
        public float falloff;

        [ContextMenu("SpawnColliders")]
        public void SpawnColliders()
        {
            var trees = terrain.terrainData.treeInstances;

            foreach (TreeInstance tree in trees)
            {
                if (CloseToLine(tree.position))
                {
                    // Debug.Log(terrain.terrainData.treePrototypes[tree.prototypeIndex].prefab.name);
                    var prefab = terrain.terrainData.treePrototypes[tree.prototypeIndex].prefab;

                    var collider = prefab.GetComponentsInChildren<TreeCollider>(true);

                    if (collider.Length > 0)
                    {
                        Debug.Log("found a tree collider");
                        var obj = Instantiate(collider[0], holder);
                        obj.gameObject.SetActive(true);
                        obj.transform.position = new Vector3(tree.position.x * terrain.terrainData.size.x, tree.position.y * terrain.terrainData.size.y, tree.position.z * terrain.terrainData.size.z);
                        obj.transform.localScale = new Vector3(obj.transform.localScale.x * tree.widthScale, obj.transform.localScale.y * tree.heightScale, obj.transform.localScale.z * tree.widthScale);
                        obj.transform.localScale = obj.transform.localScale * prefab.transform.localScale.x;
                        //obj.transform.rotation = Quaternion.AngleAxis(tree.rotation * Mathf.Rad2Deg, Vector3.up);
                    }
                }
            }
        }

        private bool CloseToLine(Vector3 position)
        {
            var pos = new Vector3(position.x * terrain.terrainData.size.x, position.y * terrain.terrainData.size.y, position.z * terrain.terrainData.size.z);

            for (float i = 0.01f;  i < 1f; i+=0.1f)
            {
                var point = line.GetPoint(i);
                var distance = distanceFromLine * (i+falloff);
                distance = distance < min ? min : distance;
                distance = distance > distanceFromLine ? distanceFromLine : distance;
                Debug.Log("distance point " + i + " = " + Vector3.Distance(pos, point));
                if(Vector3.Distance(pos, point) < distance)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
