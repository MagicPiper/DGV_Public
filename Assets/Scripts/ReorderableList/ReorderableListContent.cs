///// Credit Ziboo
///// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/

//using Assets.Scripts;
//using System.Collections;
//using System.Collections.Generic;

//namespace UnityEngine.UI.Extensions
//{
//    public class ReorderableListContent : MonoBehaviour
//    {
//        private List<Transform> _cachedChildren;
//        private List<ReorderableListElement> _cachedListElement;
//        private ReorderableListElement _ele;
//        private ReorderableList _extList;
//        private RectTransform _rect;

//        private void OnEnable()
//        {
//            if(_rect)StartCoroutine(RefreshChildren());

            
//        }


//        public void OnTransformChildrenChanged()
//        {
//            if(this.isActiveAndEnabled)StartCoroutine(RefreshChildren());
//        }

//        public void Init(ReorderableList extList)
//        {
//            _extList = extList;
//            _rect = GetComponent<RectTransform>();
//            _cachedChildren = new List<Transform>();
//            _cachedListElement = new List<ReorderableListElement>();

//            StartCoroutine(RefreshChildren());
//        }

//        private IEnumerator RefreshChildren()
//        {
//            //Handle new chilren

//            //HACK a little hack, if I don't wait one frame I don't have the right deleted children
//            yield return new WaitForEndOfFrame();

//            if (_cachedChildren.Count == 0)
//            {
//                Kids();
//            }

//            //Remove deleted child
//            for (int i = _cachedChildren.Count - 1; i >= 0; i--)
//            {
//                if (_cachedChildren[i] == null)
//                {
//                    Debug.Log("delete a child from " + this.name);
//                    _cachedChildren.RemoveAt(i);
//                    _cachedListElement.RemoveAt(i);
//                    Kids();
//                }
//            }
//        }

//        private void Kids()
//        {
//            for (int i = 0; i < _rect.childCount; i++)
//            {
//                //Get or Create ReorderableListElement
//                Debug.Log(this.name + " running init on ");
//                _ele = _rect.GetChild(i).gameObject.GetComponent<ReorderableListElement>() ??
//                       _rect.GetChild(i).gameObject.AddComponent<ReorderableListElement>();
//                _ele.Init(_extList);

//                if (!_cachedChildren.Contains(_rect.GetChild(i)))
//                {
//                    _cachedChildren.Add(_rect.GetChild(i));
//                    _cachedListElement.Add(_ele);
//                }
//            }
//        }
//    }
//}