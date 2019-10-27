using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using static UnityEngine.UI.Extensions.ReorderableList;

namespace Assets.Scripts.Menu
{
    public class DiscCollection : MenuPane
    {
        public UIDisc UIDisc;
        public Transform discBagHolder;
        public ReorderableList discBagReorderableList;
        public ReorderableList collectionReorderableList;
        public Transform discCollectionHolder;
        private List<Disc> discBag;
        private List<Disc> collection;
        public PlayerSave playerSave;
        public Transform threeeedeeeeDiscHolder;
        public DiscStatsPane fullStatsPane;
        public MenuDisc discPrefab;
        public MenuDisc selectedDisc;
        private int discBagMax;
       
        private CanvasGroup goBackTo;
        public Image[] filterButtons;
        public GameObject[] emptySlots;
        public StartMenu start;
        public List<Disc> testDiscs = new List<Disc>();
        private List<UIDisc> discCOllectionObjects;
        private List<UIDisc> discBagObjects;
        public CanvasGroup confirmDeletePane;
        public Button confirmDelete;
        public Button cancelDelete;

        public void Show(MenuPane goBackTo)
        {
            currentPane = this;
            FadeInPane(GetComponent<CanvasGroup>());
            discBagMax = playerSave.playerStats.statsData.devSupport == true ? 9 : 6;

            Debug.Log("discbag size = " + discBagMax);

            Populate();
            fullStatsPane.Clear();
            backPane = goBackTo;            
        }

        public override void Hide()
        {
            fullStatsPane.Clear();

            if (selectedDisc != null)
            {
                Destroy(selectedDisc.gameObject);
            }

            playerSave.SaveProfile();
            FadeOutPane(GetComponent<CanvasGroup>());
        }

        private void Update()
        {
            threeeedeeeeDiscHolder.Rotate(Vector3.up, 20f*Time.deltaTime);
        }

        private void Populate()
        {
            if (discCOllectionObjects != null)
            {
                foreach (UIDisc disc in discCOllectionObjects)
                {
                    Destroy(disc.gameObject);
                }
            }

            if (discBagObjects != null)
            {
                foreach (UIDisc disc in discBagObjects)
                {
                    Destroy(disc.gameObject);
                }
            }
            discCOllectionObjects = new List<UIDisc>();
            discBagObjects = new List<UIDisc>();

            discBag = playerSave.GetDiscBagContent();
            collection = playerSave.GetDiscCollection();
            foreach (Disc disc in discBag)
            {
                var uidisc = Instantiate(UIDisc, discBagHolder);
                uidisc.Populate(disc, UIDisc.Context.Collection);
                uidisc.discCollection = this;
                var r = uidisc.gameObject.AddComponent<ReorderableListElement>();
                r.Init(discBagReorderableList);
                discBagObjects.Add(uidisc);

            }
            PopulateCollection();

            UpdateDiscBagMinMax();           

        }

        private void PopulateCollection()
        {
            foreach (Disc disc in collection)
            {
                var uidisc = Instantiate(UIDisc, discCollectionHolder);
                uidisc.Populate(disc, UIDisc.Context.Collection);
                uidisc.discCollection = this;
                var r = uidisc.gameObject.AddComponent<ReorderableListElement>();
                r.Init(collectionReorderableList);
                discCOllectionObjects.Add(uidisc);
            }
            FilterCollection(1);

        }

        public void FilterCollection(int type)
        {
            foreach (UIDisc disc in discCOllectionObjects)
            {
                if ((int)disc.mould.discType == type)
                {
                    disc.gameObject.SetActive(true);
                }
                else
                {
                    disc.gameObject.SetActive(false);
                }
            }
            foreach (var butt in filterButtons)
            {
                butt.color = new Color(butt.color.r, butt.color.g, butt.color.b, 0.1f);
            }
            filterButtons[type - 1].color = new Color(filterButtons[type - 1].color.r, filterButtons[type - 1].color.g, filterButtons[type - 1].color.b, 1f);

        }

        private void PopulateTest()
        {
            discBag = testDiscs;
            collection = new List<Disc>();
            foreach (Disc disc in discBag)
            {
                var uidisc = Instantiate(UIDisc, discBagHolder);
                uidisc.Populate(disc, UIDisc.Context.Collection);
                uidisc.discCollection = this;
                var r = uidisc.gameObject.AddComponent<ReorderableListElement>();
                r.Init(discBagReorderableList);
            }
            foreach (Disc disc in collection)
            {
                var uidisc = Instantiate(UIDisc, discCollectionHolder);
                uidisc.Populate(disc, UIDisc.Context.Collection);
            }

            UpdateDiscBagMinMax();
        }

        internal void SelectedDisc(Disc disc)
        {
            if (selectedDisc != null)
            {
                Destroy(selectedDisc.gameObject);
            }
            fullStatsPane.Clear();
            fullStatsPane.Populate(disc);
            selectedDisc = Instantiate(discPrefab, threeeedeeeeDiscHolder, false);
            selectedDisc.transform.localPosition = new Vector3(0, 0, 0);

            threeeedeeeeDiscHolder.transform.rotation = Quaternion.identity;

            selectedDisc.transform.Rotate(Vector3.up, -60);
            selectedDisc.transform.Rotate(Vector3.right, 60);

            selectedDisc.PopulateDisc(disc);
            //selectedDisc.rb.useGravity = false;
            //selectedDisc.blobShadow.gameObject.SetActive(false);
        }

        private void UpdateDiscBagMinMax()
        {
            if (discBag.Count >= discBagMax)
            {
                discBagReorderableList.IsDropable = false;
            }
            else
            {
                discBagReorderableList.IsDropable = true;
            }
            if (discBag.Count <= 1)
            {
                discBagReorderableList.IsDraggable = false;
            }
            else
            {
                discBagReorderableList.IsDraggable = true;
            }
            UpdateEmptySlots();
        }

        private void UpdateEmptySlots()
        {
            int i = 0;
            foreach(GameObject slot in emptySlots)
            {
                if(i < discBag.Count || i >= discBagMax)
                {
                    slot.SetActive(false);
                }
                else
                {
                    slot.SetActive(true);
                }
                slot.transform.SetAsLastSibling();
                i++;
            }
        }

        public void DiscDropped(ReorderableListEventStruct drop)
        {
            var disc = drop.DroppedObject.GetComponent<UIDisc>();
            var lay = drop.DroppedObject.GetComponent<LayoutElement>();
            lay.preferredHeight = 200;
            lay.preferredWidth = 200;

            RemoveDisc(drop.FromList.name, disc);
            AddDisc(drop.ToList.name, disc, drop.ToIndex);
            // playerSave.SaveProfile();


            UpdateDiscBagMinMax();
        }

        public void DiscDroppedTrash(ReorderableListEventStruct drop)
        {
            var disc = drop.DroppedObject.GetComponent<UIDisc>();
            FadeInPane(confirmDeletePane);
            confirmDelete.onClick.RemoveAllListeners();
            cancelDelete.onClick.RemoveAllListeners();
            confirmDelete.onClick.AddListener(delegate { RemoveDisc(drop.FromList.name, disc); Destroy(disc.gameObject); UpdateDiscBagMinMax(); fullStatsPane.Clear(); Destroy(selectedDisc.gameObject); });
            cancelDelete.onClick.AddListener(delegate { disc.transform.SetParent(drop.FromList.ContentLayout.transform); disc.GetComponent<ReorderableListElement>().Init(drop.FromList); });
            // playerSave.SaveProfile();
        }

        public void DiscBagGrabbed(ReorderableListEventStruct grab)
        {
            discBagReorderableList.IsDropable = true;
        }

        private void AddDisc(string toList, UIDisc UIdisc, int index)
        {
            Debug.Log("Add to " + toList);

            var disc = UIdisc.disc;
            if(index > discBag.Count)
            {
                index = discBag.Count;
            }

            if (toList == "DiscBag")
            {
                discBag.Insert(index, disc);
                discBagObjects.Add(UIdisc);
                
                Debug.Log("add to DB");
            }
            else if (toList == "Collection")
            {
                collection.Insert(index, disc);
                discCOllectionObjects.Add(UIdisc);
                FilterCollection((int)UIdisc.mould.discType);

                Debug.Log("add to Coll");
            }
        }

        private void RemoveDisc(string fromList, UIDisc UIdisc)
        {
            var disc = UIdisc.disc;

            Debug.Log("remove from " + fromList);

            if (fromList == "DiscBag")
            {
                discBag.Remove(disc);
                discBagObjects.Remove(UIdisc);

                Debug.Log("remove from DB");
            }
            else if (fromList == "Collection")
            {
                collection.Remove(disc);
                discCOllectionObjects.Remove(UIdisc);

                Debug.Log("remove from Coll");
            }
        }
    }
}
