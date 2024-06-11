#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Tools
{
    [ExecuteAlways]

    public class AnimatedBoxMaker : MonoBehaviour
    {
        private float pixelsPerUnit = 0.01f;

        private int chaserCountLast = 0;
        public int chaserCount = 0;

        private int oldWidth = 0;
        private int oldHeight = 0;

        public GameObject chaserPrefab;

        public int width = 256;
        public int height = 256;

        public float chaserSpeed = 0.1f;

        private AnimatedBoxChaser script;
        private List<GameObject> chasers = new List<GameObject>();

        public enum BoxChaserAlignment
        {
            Center,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public BoxChaserAlignment boxAlignment;
        private BoxChaserAlignment lastAlignment;


        // private BoxChaserAlignment BoxAlignment { get => boxAlignment; set => boxAlignment = value; }

        void Start()
        {
            //REMOVE SELF FROM OBJECT
            // var scriptSelf = GetComponent<AnimatedBoxMaker>();
            // scriptSelf.enabled = false;
        }

        private void Update()
        {

            script = GetComponent<AnimatedBoxChaser>();

            if (!script)
            {
                script = gameObject.AddComponent<AnimatedBoxChaser>();
                script.topLeft = new GameObject();
                script.topLeft.name = "topLeft";
                script.topLeft.transform.parent = gameObject.transform;
                script.botRight = new GameObject();
                script.botRight.name = "botRight";
                script.botRight.transform.parent = gameObject.transform;
            }

            if (chaserCountLast != chaserCount || lastAlignment != boxAlignment && chaserPrefab != null)
            {

                script.killChasers();
                chasers.Clear();
                if (chaserCount == 0)
                    return;

                for (int i = 0; i < chaserCount; i++)
                {
                    GameObject thisChaser = Instantiate(chaserPrefab, gameObject.transform);
                    thisChaser.transform.localPosition = Vector3.zero;
                    thisChaser.name = "chaser_" + i.ToString();
                    chasers.Add(thisChaser);
                }

                script.chasers = chasers;
                updateCorners();
                script.MapStartPositions();
                chaserCountLast = chaserCount;

            }
            else
            {

                SyncSpeed();

                if (width != oldWidth)
                {
                    updateCorners();
                    script.MapStartPositions();
                }
                else if (height != oldHeight)
                {
                    updateCorners();
                    script.MapStartPositions();
                }

                script.UpdateChasers();
            }
        }

        private void SyncSpeed()
        {
            script = GetComponent<AnimatedBoxChaser>();

            if (!script)
                return;

            script.speed = chaserSpeed;
        }

        private void updateCorners()
        {
            script = GetComponent<AnimatedBoxChaser>();

            if (!script)
                return;

            Vector3 pos = Vector3.zero;

            Vector3 posTL = pos;
            Vector3 posBR = pos;

            float offsetTL_X = 0.0f;
            float offsetTL_Y = 0.0f;
            float offsetBR_X = 0.0f;
            float offsetBR_Y = 0.0f;

            switch (boxAlignment)
            {
                case BoxChaserAlignment.Center:

                    offsetTL_X = -((width * pixelsPerUnit) * .5f);
                    offsetTL_Y = (height * pixelsPerUnit) * .5f;
                    offsetBR_X = (width * pixelsPerUnit) * .5f;
                    offsetBR_Y = -((height * pixelsPerUnit) * .5f);

                    break;

                case BoxChaserAlignment.TopLeft:

                    offsetBR_X = width * pixelsPerUnit;
                    offsetBR_Y = -(height * pixelsPerUnit);

                    break;

                case BoxChaserAlignment.TopRight:
                    offsetTL_X = -(width * pixelsPerUnit);
                    offsetBR_Y = -(height * pixelsPerUnit);
                    break;

                case BoxChaserAlignment.BottomLeft:
                    offsetTL_Y = height * pixelsPerUnit;
                    offsetBR_X = width * pixelsPerUnit;
                    break;

                case BoxChaserAlignment.BottomRight:
                    offsetTL_X = -(width * pixelsPerUnit);
                    offsetTL_Y = height * pixelsPerUnit;
                    break;
            }

            posTL.x += offsetTL_X;
            posTL.y += offsetTL_Y;

            posBR.x += offsetBR_X;
            posBR.y += offsetBR_Y;

            script.topLeft.transform.localPosition = posTL;
            script.botRight.transform.localPosition = posBR;

            oldWidth = width;
            oldHeight = height;

            lastAlignment = boxAlignment;

        }

        private void OnGUI()
        {
            //StaticLogForwarder.Logger.Log("chasers.Count:" + chasers.Count);
            boxAlignment = (BoxChaserAlignment)EditorGUILayout.EnumPopup("Box Alignment:", boxAlignment);
        }


    }
}
#endif