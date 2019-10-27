using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class DiscGallery : MonoBehaviour
{
    public DiscData discData;
    public DiscBehavior disc;

    [ContextMenu("Basic Colors")]
    public void BasicColors()
    {
        SpawnDiscs(discData.basicColors);
    }

    [ContextMenu("Burst Colors")]
    public void BurstColors()
    {
        SpawnDiscs(discData.burstColors);
    }

    [ContextMenu("Recycled Colors")]
    public void RecycledColors()
    {
        SpawnDiscs(discData.recycledColors);
    }

    [ContextMenu("All colors")]
    public void AllColors()
    {
        var list = new List<DiscColor>();
        list.AddRange(discData.basicColors);
        list.AddRange(discData.burstColors);
        list.AddRange(discData.recycledColors);
        list.AddRange(discData.rareStamps);

        SpawnDiscs(list.ToArray());
    }

        public void SpawnDiscs(DiscColor[] colors)
    {
        float xPos = 0;
        float yPos = 0;
        int counter = 0;

        disc.discData = discData;

        foreach (DiscColor color in colors)
        {
            var d = new Disc
            {
                colorsName = color.name,
            };

            disc.mould = discData.GetMould((DiscMould.MouldName)Random.Range(0, 12));

            disc.PopulateAppearance(d);

            disc.discModel.transform.position = new Vector3(xPos, yPos, 0f);
            disc.discModel.transform.SetParent(null);
            disc.discModel.transform.Rotate(new Vector3(90, 0, 0));
            disc.discModel.transform.Rotate(new Vector3(0, 180, 0));
            ////var editor = disc.discModel.AddComponent<DiscColorEditor>();
            //editor.colorName = color.name;
            //editor.discMat = disc.discModel.GetComponent<Renderer>().material;

            xPos += 0.6f;
            counter++;
            if (counter > 6)
            {
                yPos -= 0.6f;
                xPos = 0;
                counter = 0;
            }            
        }
    }
}
