
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CreativeSpore.SpriteSorting
{

    [ExecuteInEditMode]
    public class IsoSpriteSorting : MonoBehaviour
    {
        //NOTE: set to -32768 instead if you are displaying more than 32767 sprites to allow sorting properly another 32768 extra sprites.
        // Or set to 0 to see the order number better (starting with 0) as so many sprites at the same time would be crazy
        public const int k_BaseSortingValue = 0;
        public const string k_IntParam_ExecuteInEditMode = "IsoSpriteSorting_ExecuteInEditMode";

        interface IRenderer
        {
            int sortingOrder{get; set;}
            bool isVisible { get; }
            bool isAlive { get; }
            Component context { get; }
        }

        class RenderWrapper : IRenderer
        {
            public RenderWrapper(Renderer renderer){m_renderer = renderer;}
            public int sortingOrder 
            { 
                get { return m_renderer.sortingOrder; }
                set { m_renderer.sortingOrder = value; }
            }
            public bool isVisible
            {
                get { return m_renderer.isVisible; }
            }
            public bool isAlive { get { return m_renderer != null; } }
            public Component context { get { return m_renderer; } }
            private Renderer m_renderer;
        }

        class CanvasWrapper : IRenderer
        {
            public CanvasWrapper(Canvas canvas) { m_canvas = canvas; }
            public int sortingOrder
            {
                get { return m_canvas.sortingOrder; }
                set { m_canvas.sortingOrder = value; }
            }
            public bool isVisible
            {
                get { return m_canvas.isActiveAndEnabled; }
            }
            public bool isAlive { get { return m_canvas != null; } }
            public Component context { get { return m_canvas; } }
            private Canvas m_canvas;
        }

        /// <summary>
        /// Data for each separated axis
        /// </summary>
        class AxisData
        {
            public List<IsoSpriteSorting> ListSortedIsoSprs = new List<IsoSpriteSorting>();
            public int OrderCnt = k_BaseSortingValue;
            public UnityEngine.Object Master = null;
        };

        /// <summary>
        /// Sorting axis
        /// </summary>
        public enum eSortingAxis
        {
            X,
            Y,
            Z,
            XY,
            YZ,
            ZX,
            CameraForward
        }

        public bool IsSortEnabled { get { return Application.isPlaying || !Application.isPlaying && PlayerPrefs.GetInt(IsoSpriteSorting.k_IntParam_ExecuteInEditMode, 1) != 0; } }

        static Dictionary<IsoSpriteSorting, IRenderer[]> s_dicInstanceSpriteList = new Dictionary<IsoSpriteSorting, IRenderer[]>();


        /// <summary>
        /// Separation of management by sorting axis
        /// </summary>
        static AxisData[] s_axisData = null;

        /// <summary>
        /// Sorting axis
        /// </summary>
        public eSortingAxis SortingAxis = eSortingAxis.CameraForward;
        public Vector3 SorterPositionOffset = new Vector3();

        /// <summary>
        /// If invalidated and IncludeInactiveRenderer is true, inactive renderers will be taking into account
        /// </summary>
        public bool IncludeInactiveRenderer = true;

        private bool m_invalidated = true;

        [ContextMenu("Invalidate")]
        /// <summary>
        /// Invalidate when adding or removing any renderer
        /// </summary>
        public void Invalidate()
        {
            m_invalidated = true;
        }

        static bool s_invalidateAll = false;

        /// <summary>
        /// Invalidate all objects
        /// </summary>
        public static void InvalidateAll()
        {
            s_invalidateAll = true;            
        }

        public static void Invalidate(GameObject gameObject)
        {
            IsoSpriteSorting sortingComp = s_dicInstanceSpriteList.Keys.FirstOrDefault(x => x.gameObject == gameObject);
            if (sortingComp)
                sortingComp.Invalidate();
        }

        public string GetStatistics()
        {
            string sStats = "";
            
            int nbOfVisibleObjs = 0;
            int nbOfRenderers = 0;

            sStats = "<b>Stats By Sorting Axis:</b>\n";
            foreach( eSortingAxis sortingAxis in System.Enum.GetValues(typeof(eSortingAxis)) )
            {
                sStats += "  <b>- " + sortingAxis + ":</b>\n";
                if (s_axisData[(int)sortingAxis].ListSortedIsoSprs.Count > 0)
                {
                    int orderCntRelToBase = s_axisData[(int)sortingAxis].OrderCnt - k_BaseSortingValue;
                    nbOfVisibleObjs += s_axisData[(int)sortingAxis].ListSortedIsoSprs.Count;
                    nbOfRenderers += orderCntRelToBase;
                    sStats += "\tTotal Visible Objects: " + s_axisData[(int)sortingAxis].ListSortedIsoSprs.Count + "\n";
                    sStats += "\tTotal Visible Renderers: " + orderCntRelToBase + "\n";
                }
            }

            sStats += "\n<b>Global Stats:</b>\n";
            sStats += "\tTotal Registered Objects: " + s_dicInstanceSpriteList.Keys.Count + "\n";
            sStats += "\tTotal Visible Objects: " + nbOfVisibleObjs + "\n";
            sStats += "\tTotal Visible Renderers: " + nbOfRenderers + "\n";

            return sStats;
        }

        private void OnValidate()
        {
            Invalidate();
        }

        void Start()
        {
            Invalidate();
        }

        private void OnEnable()
        {
            Camera.onPreRender += _OnPreRender;            
        }

        private void OnDisable()
        {
            Camera.onPreRender -= _OnPreRender;
        }

        private void _OnPreRender(Camera cam)
        {
            //Debug.Log(Time.frameCount + ":" + name + ":_OnPreRender " + cam.name);
            if (IsSortEnabled)
            {
                UpdateSorting();
            }
        }

        void Update()
        {            

            int iSortingAxis = (int)SortingAxis;
            if (s_axisData == null || s_axisData.Length != s_eSortingAxisCount)
            {
                s_axisData = new AxisData[System.Enum.GetValues(typeof(eSortingAxis)).Length];
                for (int i = 0; i < s_axisData.Length; ++i)
                {
                    s_axisData[i] = new AxisData();
                }
            }

            AxisData axisData = s_axisData[iSortingAxis];
            if (!s_axisData[iSortingAxis].Master)
                s_axisData[iSortingAxis].Master = this;
            if (s_axisData[iSortingAxis].Master == this)
            {
                axisData.ListSortedIsoSprs.Clear();
                axisData.OrderCnt = k_BaseSortingValue;

                if (s_invalidateAll)
                {
                    s_invalidateAll = false;
                    foreach (IsoSpriteSorting obj in s_dicInstanceSpriteList.Keys)
                    {
                        obj.Invalidate();
                    }
                }
            }

            if (m_invalidated)
            {
                m_invalidated = false;
                List<IRenderer> outList;
                outList = Enumerable.Select(GetComponentsInChildren<Renderer>(IncludeInactiveRenderer), x => new RenderWrapper(x)).Cast<IRenderer>().ToList();
                outList.AddRange(Enumerable.Select(GetComponentsInChildren<Canvas>(IncludeInactiveRenderer), x => new CanvasWrapper(x)).Cast<IRenderer>().ToList());
                IRenderer[] outArray = outList.ToArray();
                System.Array.Sort(outArray, (a, b) => a.sortingOrder.CompareTo(b.sortingOrder));
                s_dicInstanceSpriteList[this] = outArray;
            }

            if (IsSortEnabled)
            {
                // Add to sorting render list if visible
                {
                    IRenderer[] aSprRenderer = null;
                    s_dicInstanceSpriteList.TryGetValue(this, out aSprRenderer);
                    if (aSprRenderer != null)
                    {
                        for (int i = 0; i < aSprRenderer.Length; ++i)
                        {
                            if (aSprRenderer[i].isVisible)
                            {
                                axisData.ListSortedIsoSprs.Add(this);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (IsSortEnabled)
            {
                int iSortingAxis = (int)SortingAxis;
                AxisData axisData = s_axisData[iSortingAxis];
                if (axisData.Master == this)
                {
                    axisData.Master = null;
                    axisData.OrderCnt = k_BaseSortingValue;
                    List<IsoSpriteSorting> listSortedIsoSpr = axisData.ListSortedIsoSprs;
                    //Sort sprites
                    switch (SortingAxis)
                    {//NOTE: !a || !b? 0 : avoids null exceptions when using the Profiler after stopping the game
                        case eSortingAxis.X: listSortedIsoSpr.Sort((a, b) => !a || !b ? 0 : (b.SorterPositionOffset.x + b.transform.position.x).CompareTo(a.SorterPositionOffset.x + a.transform.position.x)); break;
                        case eSortingAxis.Y: listSortedIsoSpr.Sort((a, b) => !a || !b ? 0 : (b.SorterPositionOffset.y + b.transform.position.y).CompareTo(a.SorterPositionOffset.y + a.transform.position.y)); break;
                        case eSortingAxis.Z: listSortedIsoSpr.Sort((a, b) => !a || !b ? 0 : (b.SorterPositionOffset.z + b.transform.position.z).CompareTo(a.SorterPositionOffset.z + a.transform.position.z)); break;
                        case eSortingAxis.XY: listSortedIsoSpr.Sort((a, b) => !a || !b ? 0 : Vector3.Dot(s_vXY, (b.SorterPositionOffset + b.transform.position) - (a.SorterPositionOffset + a.transform.position)).CompareTo(0f)); break;
                        case eSortingAxis.YZ: listSortedIsoSpr.Sort((a, b) => !a || !b ? 0 : Vector3.Dot(s_vYZ, (b.SorterPositionOffset + b.transform.position) - (a.SorterPositionOffset + a.transform.position)).CompareTo(0f)); break;
                        case eSortingAxis.ZX: listSortedIsoSpr.Sort((a, b) => !a || !b ? 0 : Vector3.Dot(s_vZX, (b.SorterPositionOffset + b.transform.position) - (a.SorterPositionOffset + a.transform.position)).CompareTo(0f)); break;
                        case eSortingAxis.CameraForward: listSortedIsoSpr.Sort((a, b) => Vector3.Dot(Camera.main.transform.forward, (b.SorterPositionOffset + b.transform.position) - (a.SorterPositionOffset + a.transform.position)).CompareTo(0f)); break;
                    }
                }
            }
        }

        static Vector3 s_vXY = new Vector3(1f, 1f, 0f);
        static Vector3 s_vYZ = new Vector3(0f, 1f, 1f);
        static Vector3 s_vZX = new Vector3(1f, 0f, 1f);
        static int s_eSortingAxisCount = System.Enum.GetValues(typeof(eSortingAxis)).Length;
        void UpdateSorting()
        {
            int iSortingAxis = (int)SortingAxis;
            AxisData axisData = s_axisData[iSortingAxis];
            List<IsoSpriteSorting> listSortedIsoSpr = axisData.ListSortedIsoSprs;
            for (int i = 0; i < listSortedIsoSpr.Count; ++i)
            {
                IRenderer[] aSprRenderer = null;
                listSortedIsoSpr[i].m_invalidated |= !s_dicInstanceSpriteList.TryGetValue(listSortedIsoSpr[i], out aSprRenderer);
                if (aSprRenderer != null)
                {
                    for (int j = 0; j < aSprRenderer.Length; ++j)
                    {
                        if (aSprRenderer[j].isAlive)
                            aSprRenderer[j].sortingOrder = s_axisData[iSortingAxis].OrderCnt++;
                        else // a renderer was destroyed
                            listSortedIsoSpr[i].m_invalidated = true;
                    }
                }
            }            
        }

        void OnDestroy()
        {
            s_dicInstanceSpriteList.Remove(this);
            if (s_axisData != null)
            {
                s_axisData[(int)SortingAxis].ListSortedIsoSprs.Remove(this);
            }
        }
    }
}