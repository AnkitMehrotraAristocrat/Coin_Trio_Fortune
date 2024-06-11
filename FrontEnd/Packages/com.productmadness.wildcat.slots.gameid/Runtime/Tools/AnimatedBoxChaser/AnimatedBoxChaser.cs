using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
using Unity.Mathematics;
using ILogger = Milan.FrontEnd.Bridge.Logging.ILogger;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Tools
{
    public class AnimatedBoxChaser : MonoBehaviour
    {
        public bool particlesActive = true;
        public float speed = 0.1f;//Speed can be 1 to 5
        public GameObject topLeft;
        public GameObject botRight;

        private List<Vector3> startPositions = new List<Vector3>();
        private List<int> directions = new List<int>();
        public List<GameObject> chasers = new List<GameObject>();
        private int[] directionChangeIndexes = { 0, 0, 0, 0 };

        private bool active = true;
        private bool completedCircle = false;
        
        // Start is called before the first frame update
        void Start()
        {
            MapStartPositions();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateChasers();
        }

        private void Awake()
        {
            MapStartPositions();
        }

        public void killChasers()
        {
            GameIdLogger.Logger.Debug("Destroy Chasers");
            for (int i = 0; i < chasers.Count; i++)
            {
                GameObject go = chasers[i];
                DestroyImmediate(go);
            }
        }

        public void MapStartPositions()
        {

            startPositions.Clear();
            directions.Clear();

            if (chasers.Count != 0)
            {


                GameIdLogger.Logger.Debug("MapStartPositions()");
                Vector3 pos = topLeft.transform.localPosition;
                int idx = 0;
                startPositions.Add(pos);

                ///MEASURE TOP EDGE
                while (pos.x < botRight.transform.localPosition.x)
                {
                    pos.x += math.abs(speed);
                    startPositions.Add(pos);
                    idx++;
                }
                pos.x = botRight.transform.localPosition.x;
                startPositions.Add(pos);
                directionChangeIndexes[1] = idx;
                idx++;
                ///MEASURE RIGHT EDGE
                while (pos.y > botRight.transform.localPosition.y)
                {
                    pos.y -= math.abs(speed);
                    startPositions.Add(pos);
                    idx++;
                }
                pos.y = botRight.transform.localPosition.y;
                startPositions.Add(pos);
                directionChangeIndexes[2] = idx;
                idx++;
                //MEASURE BOTTOM EDGE
                while (pos.x > topLeft.transform.localPosition.x)
                {
                    pos.x -= math.abs(speed);
                    startPositions.Add(pos);
                    idx++;
                }
                pos.x = topLeft.transform.localPosition.x;
                startPositions.Add(pos);
                directionChangeIndexes[3] = idx;
                idx++;
                ///MEASURE LEFT EDGE
                while (pos.y < topLeft.transform.localPosition.y)
                {
                    pos.y += math.abs(speed);
                    startPositions.Add(pos);
                    idx++;
                }
                ResetPositions();
            }
            else
            {
                startPositions.Clear();
            }
        }

        private void ResetPositions()
        {
            int skip = (int)math.floor(startPositions.Count / chasers.Count);
            GameIdLogger.Logger.Debug("ResetPositions() startPositions.Count: " + startPositions.Count.ToString() + ", chasers.Count:" + chasers.Count.ToString() + ", skip length: " + skip.ToString());
            int idx = 0;
            if (speed >= 0)
            {
                for (int i = 0; i < chasers.Count; i++)
                {
                    directions.Add(0);
                    GameObject chaser = chasers[i];
                    chaser.transform.localPosition = startPositions[idx];
                    setDirectionForChaser(i, idx);
                    idx += skip;
                }
            }
            else
            {
                idx = startPositions.Count - 1;
                for (int i = 0; i < chasers.Count; i++)
                {
                    directions.Add(0);
                    GameObject chaser = chasers[i];
                    chaser.transform.localPosition = startPositions[idx];
                    setDirectionForChaser(i, idx);
                    idx -= skip;
                    if (idx <= 0)
                        idx = 0;
                }
            }
        }

        private void setDirectionForChaser(int chaserIdx, int positionIdx)
        {
            if (positionIdx >= directionChangeIndexes[3])
            {
                directions[chaserIdx] = 3;//UP
            }
            else if (positionIdx >= directionChangeIndexes[2])
            {
                directions[chaserIdx] = 2;//LEFT
            }
            else if (positionIdx >= directionChangeIndexes[1])
            {
                directions[chaserIdx] = 1;//DOWN
            }
            else
            {
                directions[chaserIdx] = 0;//RIGHT
            }
        }

        private void setParticlesEmit(bool emit)
        {
            ParticleSystem[] allChildren = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem child in allChildren)
            {
                child.enableEmission = emit;
            }
        }

        public void UpdateChasers()
        {
            if (!particlesActive)
            {
                setParticlesEmit(false);
                return;
            }
            else
            {
                setParticlesEmit(true);
            }
            for (int i = 0; i < chasers.Count; i++)
            {
                GameObject chaser = chasers[i];
                Vector3 pos = chaser.transform.localPosition;
                int dir = directions[i];
                switch (dir)
                {
                    case 0://TOP EDGE
                        pos.x += speed;
                        if (speed > 0)
                        {
                            if (pos.x >= botRight.transform.localPosition.x)
                            {
                                chaser.transform.localPosition = botRight.transform.localPosition;
                                directions[i] = 1;
                            }
                        }
                        else
                        {
                            if (pos.x <= topLeft.transform.localPosition.x)
                            {
                                chaser.transform.localPosition = topLeft.transform.localPosition;
                                directions[i] = 3;
                                if (chaser == chasers[0])
                                {
                                    if (!completedCircle)
                                    {
                                        //WE MADE A COMPLETE REVOLUTION
                                        //RESET POSITIONS TO MAINTAIN SYNC
                                        completedCircle = true;
                                    }
                                }
                            }
                        }
                        break;
                    case 1://RIGHT EDGE
                        pos.y -= speed;
                        if (speed > 0)
                        {
                            if (pos.y <= botRight.transform.localPosition.y)
                            {
                                chaser.transform.localPosition = botRight.transform.localPosition;
                                directions[i] = 2;
                            }
                        }
                        else
                        {
                            if (pos.y >= topLeft.transform.localPosition.y)
                            {
                                chaser.transform.localPosition = topLeft.transform.localPosition;
                                directions[i] = 0;
                            }
                        }
                        break;
                    case 2://BOTTOM EDGE
                        pos.x -= speed;
                        if (speed > 0)
                        {
                            if (pos.x <= topLeft.transform.localPosition.x)
                            {
                                chaser.transform.localPosition = topLeft.transform.localPosition;
                                directions[i] = 3;
                            }
                        }
                        else
                        {
                            if (pos.x >= botRight.transform.localPosition.x)
                            {
                                chaser.transform.localPosition = botRight.transform.localPosition;
                                directions[i] = 1;
                            }
                        }
                        break;
                    case 3://LEFT EDGE
                        pos.y += speed;
                        if (speed > 0)
                        {
                            if (pos.y >= topLeft.transform.localPosition.y)
                            {
                                chaser.transform.localPosition = topLeft.transform.localPosition;
                                directions[i] = 0;

                                if (chaser == chasers[0])
                                {
                                    if (!completedCircle)
                                    {
                                        //WE MADE A COMPLETE REVOLUTION
                                        //RESET POSITIONS TO MAINTAIN SYNC
                                        completedCircle = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (pos.y <= botRight.transform.localPosition.y)
                            {
                                chaser.transform.localPosition = botRight.transform.localPosition;
                                directions[i] = 2;

                            }
                        }
                        break;
                }
                chaser.transform.localPosition = pos;
                if (completedCircle)
                {
                    ResetPositions();
                    completedCircle = false;
                }
            }
        }
    }
}
