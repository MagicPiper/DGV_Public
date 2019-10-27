/// Credit Ziboo, Andrew Quesenberry 
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/
/// Last Child Fix - https://bitbucket.org/ddreaper/unity-ui-extensions/issues/70/all-re-orderable-lists-cause-a-transform

using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{

    [RequireComponent(typeof(RectTransform))]
    public class ReorderableListElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Tooltip("Can this element be dragged?")]
        public bool IsGrabbable = true;
        [Tooltip("Can this element be transfered to another list")]
        public bool IsTransferable = true;
        [Tooltip("Can this element be dropped in space?")]
        public bool isDroppableInSpace = false;


        private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
        public ReorderableList ToList;
        private RectTransform _draggingObject;
        private LayoutElement _draggingObjectLE;
        private Vector2 _draggingObjectOriginalSize;
        private RectTransform _fakeElement;
        private LayoutElement _fakeElementLE;
        private int _fromIndex;
        private bool _isDragging;
        private RectTransform _rect;
        public ReorderableList FromList;
        internal bool isValid;

        #region IBeginDragHandler Members

        public void OnBeginDrag(PointerEventData eventData)
        {
            isValid = true;
            if (FromList == null)
                return;

            //Can't drag, return...
            if (!FromList.IsDraggable || !this.IsGrabbable)
            {
                _draggingObject = null;
                return;
            }

            //If CloneDraggedObject  just set draggingObject to this gameobject
            if (FromList.CloneDraggedObject == false)
            {
                _draggingObject = _rect;
                _fromIndex = _rect.GetSiblingIndex();
                //Send OnElementRemoved Event
                if (FromList.OnElementRemoved != null)
                {
                    FromList.OnElementRemoved.Invoke(new ReorderableList.ReorderableListEventStruct
                    {
                        DroppedObject = _draggingObject.gameObject,
                        IsAClone = FromList.CloneDraggedObject,
                        SourceObject = FromList.CloneDraggedObject ? gameObject : _draggingObject.gameObject,
                        FromList = FromList,
                        FromIndex = _fromIndex,
                    });
                }
                if (isValid == false)
                {
                    _draggingObject = null;
                    return;
                }
            }
            //Else Duplicate
            else
            {
                GameObject clone = (GameObject)Instantiate(gameObject);
                _draggingObject = clone.GetComponent<RectTransform>();
            }

            //Put _dragging object into the dragging area
            _draggingObjectOriginalSize = gameObject.GetComponent<RectTransform>().rect.size;
            _draggingObjectLE = _draggingObject.GetComponent<LayoutElement>();
            _draggingObject.SetParent(FromList.DraggableArea, true);
            _draggingObject.SetAsLastSibling();

            //Create a fake element for previewing placement
            _fakeElement = new GameObject("Fake").AddComponent<RectTransform>();
            _fakeElementLE = _fakeElement.gameObject.AddComponent<LayoutElement>();


            RefreshSizes();

            //Send OnElementGrabbed Event
            if (FromList.OnElementGrabbed != null)
            {
                FromList.OnElementGrabbed.Invoke(new ReorderableList.ReorderableListEventStruct
                {
                    DroppedObject = _draggingObject.gameObject,
                    IsAClone = FromList.CloneDraggedObject,
                    SourceObject = FromList.CloneDraggedObject ? gameObject : _draggingObject.gameObject,
                    FromList = FromList,
                    FromIndex = _fromIndex,
                });

                if (!isValid)
                {
                    CancelDrag();
                    return;
                }
            }

            _isDragging = true;
        }

        #endregion

        #region IDragHandler Members

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging)
                return;
            if (!isValid)
            {
                CancelDrag();
                return;
            }
            //Set dragging object on cursor
            var canvas = _draggingObject.GetComponentInParent<Canvas>();
            Vector3 worldPoint;            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position,
                null, out worldPoint);

            _draggingObject.position = worldPoint;

            //Check everything under the cursor to find a ReorderableList
            EventSystem.current.RaycastAll(eventData, _raycastResults);
            for (int i = 0; i < _raycastResults.Count; i++)
            {
                ToList = _raycastResults[i].gameObject.GetComponent<ReorderableList>();
                if (ToList != null)
                {
                    break;
                }
            }

            //If nothing found or the list is not dropable, put the fake element outsite
            if (ToList == null || ToList.IsDropable == false)
            {
                RefreshSizes();
                _fakeElement.transform.SetParent(FromList.DraggableArea, false);

            }
            //Else find the best position on the list and put fake element on the right index  
            else
            {
                if (_fakeElement.parent != ToList)
                    _fakeElement.SetParent(ToList.Content, false);

                float minDistance = float.PositiveInfinity;
                int targetIndex = 0;
                float dist = 0;
                for (int j = 0; j < ToList.Content.childCount; j++)
                {
                    var c = ToList.Content.GetChild(j).GetComponent<RectTransform>();

                    if (ToList.ContentLayout is VerticalLayoutGroup)
                        dist = Mathf.Abs(c.position.y - worldPoint.y);
                    else if (ToList.ContentLayout is HorizontalLayoutGroup)
                        dist = Mathf.Abs(c.position.x - worldPoint.x);
                    else if (ToList.ContentLayout is GridLayoutGroup)
                        dist = (Mathf.Abs(c.position.x - worldPoint.x) + Mathf.Abs(c.position.y - worldPoint.y));

                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        targetIndex = j;
                    }
                }

                RefreshSizes();
                _fakeElement.SetSiblingIndex(targetIndex);
                _fakeElement.gameObject.SetActive(true);

            }
        }

        #endregion

        #region IEndDragHandler Members

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            if (_draggingObject != null)
            {
                //If we have a, ReorderableList that is dropable
                //Put the dragged object into the content and at the right index
                if (ToList != null && ToList.IsDropable
                    && (IsTransferable || ToList == FromList ))
                {
                    var args = new ReorderableList.ReorderableListEventStruct
                    {
                        DroppedObject = _draggingObject.gameObject,
                        IsAClone = FromList.CloneDraggedObject,
                        SourceObject = FromList.CloneDraggedObject ? gameObject : _draggingObject.gameObject,
                        FromList = FromList,
                        FromIndex = _fromIndex,
                        ToList = ToList,
                        ToIndex = _fakeElement.GetSiblingIndex()
                    };
                    //Send OnelementDropped Event
                    if (FromList && FromList.OnElementDropped != null)
                    {
                        FromList.OnElementDropped.Invoke(args);
                    }
                    if (!isValid)
                    {
                        CancelDrag();
                        return;
                    }
                    RefreshSizes();
                    _draggingObject.SetParent(ToList.Content, false);
                    _draggingObject.rotation = ToList.transform.rotation;
                    _draggingObject.SetSiblingIndex(_fakeElement.GetSiblingIndex());
                    FromList = ToList;

                    FromList.OnElementAdded.Invoke(args);

                    if(!isValid) throw new Exception("It's too late to cancel the Transfer! Do so in OnElementDropped!");

                }
                //We don't have an ReorderableList
                else
                {
                    if (this.isDroppableInSpace)
                    {
                        FromList.OnElementDropped.Invoke(new ReorderableList.ReorderableListEventStruct
                        {
                            DroppedObject = _draggingObject.gameObject,
                            IsAClone = FromList.CloneDraggedObject,
                            SourceObject =
                                FromList.CloneDraggedObject ? gameObject : _draggingObject.gameObject,
                            FromList = FromList,
                            FromIndex = _fromIndex
                        });
                    }
                    else
                    {
                        CancelDrag();
                    }
                }
            }

            //Delete fake element
            if (_fakeElement != null)
                Destroy(_fakeElement.gameObject);
        }

        #endregion

        void CancelDrag()
        {
            _isDragging = false;
            //If it's a clone, delete it
            if (FromList.CloneDraggedObject)
            {
                Destroy(_draggingObject.gameObject);
            }
            //Else replace the draggedObject to his first place
            else
            {
                RefreshSizes();
                _draggingObject.SetParent(FromList.Content, false);
                _draggingObject.rotation = FromList.Content.transform.rotation;
                _draggingObject.SetSiblingIndex(_fromIndex);


                var args = new ReorderableList.ReorderableListEventStruct
                {
                    DroppedObject = _draggingObject.gameObject,
                    IsAClone = FromList.CloneDraggedObject,
                    SourceObject = FromList.CloneDraggedObject ? gameObject : _draggingObject.gameObject,
                    FromList = FromList,
                    FromIndex = _fromIndex,
                    ToList = FromList,
                    ToIndex = _fromIndex
                };


                FromList.OnElementAdded.Invoke(args);

                if (!isValid) throw new Exception("Transfer is already Cancelled.");

            }

            //Delete fake element
            if (_fakeElement != null)
                Destroy(_fakeElement.gameObject);
        }

        private void RefreshSizes()
        {
            //Vector2 size = _draggingObjectOriginalSize;

            //if (ToList != null && ToList.IsDropable && ToList.Content.childCount > 0)
            //{
            //    var firstChild = ToList.Content.GetChild(0);
            //    if (firstChild != null)
            //    {
            //        size = firstChild.GetComponent<RectTransform>().rect.size;
            //    }
            //}

            //_draggingObject.sizeDelta = size;
            ////_fakeElementLE.preferredHeight = _draggingObjectLE.preferredHeight = size.y;
            ////_fakeElementLE.preferredWidth = _draggingObjectLE.preferredWidth = size.x;

            GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }

        public void Init(ReorderableList reorderableList)
        { 
            FromList = reorderableList;
            _rect = GetComponent<RectTransform>();
        }
    }
}