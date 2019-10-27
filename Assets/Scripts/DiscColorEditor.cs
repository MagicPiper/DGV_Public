//using Assets.Scripts;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public class DiscColorEditor : MonoBehaviour
//{
//    public Material discMat;
//    public string colorName;

//    [ContextMenu("Save Color")]
//    public void SaveBasicColor()
//    {
//        DiscColor asset = ScriptableObject.CreateInstance<DiscColor>();
//        asset.baseColor = discMat.GetColor("_Color");
//        asset.stampColor = discMat.GetColor("_StampColor");

//        AssetDatabase.CreateAsset(asset, "Assets/Prefabs/Discs/Colors/Basic/" + colorName + ".asset");

//        AssetDatabase.SaveAssets();
//        EditorUtility.FocusProjectWindow();
//        Selection.activeObject = asset;
//    }

//    [ContextMenu("Save Recycled Color")]
//    public void SaveRecycledColor()
//    {
//        DiscColorPattern asset = ScriptableObject.CreateInstance<DiscColorPattern>();
//        asset.baseColor = discMat.GetColor("_Color");
//        asset.stampColor = discMat.GetColor("_StampColor");
//        asset.patternColor = discMat.GetColor("_PatternColor");        

//        AssetDatabase.CreateAsset(asset, "Assets/Prefabs/Discs/Colors/Recycled/" + colorName + ".asset");

//        AssetDatabase.SaveAssets();
//        EditorUtility.FocusProjectWindow();
//        Selection.activeObject = asset;
//    }
//    [ContextMenu("Save Burst Color")]
//    public void SaveBurstColor()
//    {
//        DiscColorPattern asset = ScriptableObject.CreateInstance<DiscColorPattern>();
//        asset.baseColor = discMat.GetColor("_Color");
//        asset.stampColor = discMat.GetColor("_StampColor");
//        asset.patternColor = discMat.GetColor("_PatternColor");        

//        AssetDatabase.CreateAsset(asset, "Assets/Prefabs/Discs/Colors/Burst/" + colorName + ".asset");

//        AssetDatabase.SaveAssets();
//        EditorUtility.FocusProjectWindow();
//        Selection.activeObject = asset;
//    }
//}
