#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Spine.Unity;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Tools
{
    public class TAAnimatedBoxChaser
    {
        [MenuItem("CONTEXT/Transform/TATools/Animated Box Maker")]

        private static void AddBoxChaser(MenuCommand command)
        {
            Transform t = (Transform)command.context;
            GameObject gameObject = t.gameObject;

            gameObject.AddComponent<AnimatedBoxMaker>();

        }

    }
}
#endif