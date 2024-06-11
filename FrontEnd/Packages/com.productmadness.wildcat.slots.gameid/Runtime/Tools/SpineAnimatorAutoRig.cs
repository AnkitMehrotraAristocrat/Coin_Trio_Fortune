#if UNITY_EDITOR
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
using UnityEditor;
using Spine.Unity;
using Spine;
using Slotsburg.Slots.SharedFeatures;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Tools
{
    public class SpineAnimatorAutoRig
    {

        [MenuItem("CONTEXT/Transform/TATools/Spine Animator Auto-Rig")]

        private static void GenerateUnityAnimsForSpine(MenuCommand command)
        {
            //QUICK CHECK TO SEE IF A GAMEOBJECT CONTAINS A GIVEN COMPONENT
            bool ContainsComponent(GameObject go, string component)
            {
                var c = go.GetComponent(component);
                if (c)
                    return true;
                return false;
            }


            //Construct gameObjectRelativePath to apply in Unity AnimationClip so it can find child object.
            Transform t = (Transform)command.context;
            GameObject gameObject = t.gameObject;

            if (ContainsComponent(gameObject, "Animator"))
            {
                GameIdLogger.Logger.Error("ERROR: Can't automate process if gameObject already contains Animator component.");
                return;
            }

            string newFolderPath = "";
            string gameObjectRelativePath = "";

            Component[] skeletonAnimations = gameObject.GetComponentsInChildren<SkeletonAnimation>();

            Vector3 pos = new Vector3(50.0f, 180.0f, 0.0f);//To control where our animation state nodes appear in the stateMachine

            foreach (SkeletonAnimation skeletonAnimation in skeletonAnimations)
            {
                Transform transform = skeletonAnimation.gameObject.transform;
                GameObject g = transform.gameObject;

                //Work our way up to parent object
                while (transform != t)
                {
                    gameObjectRelativePath = transform.gameObject.name + '/' + gameObjectRelativePath;
                    transform = transform.parent;
                }
                // Drop the trailing '/':
                if (gameObjectRelativePath.Length >= 1)
                    gameObjectRelativePath = gameObjectRelativePath.Remove(gameObjectRelativePath.Length - 1);

                GameIdLogger.Logger.Debug("gameObjectRelativePath: " + gameObjectRelativePath);//This is the path to the animated Spine Object relative to our controller


                //ADD ANIMATION INDEXER IF ONE IS MISSING
                SpineAnimationIndexer spineAnimationIndexer = g.GetComponent<SpineAnimationIndexer>();
                if (!spineAnimationIndexer)
                {
                    spineAnimationIndexer = g.AddComponent<SpineAnimationIndexer>();
                    spineAnimationIndexer.GenerateAnimationIndices();
                    spineAnimationIndexer.GenerateSkinIndices();
                }

                if (spineAnimationIndexer._animations.Length <= 0)
                {
                    spineAnimationIndexer.GenerateAnimationIndices();
                    spineAnimationIndexer.GenerateSkinIndices();
                }

                //ADD ANIMATION CONTROLLER IF ONE IS MISSING
                SpineAnimationController spineAnimationController = g.GetComponent<SpineAnimationController>();
                if (!spineAnimationController)
                {
                    spineAnimationController = g.AddComponent<SpineAnimationController>();
                }

                spineAnimationController._animationIndex = 0;
                spineAnimationController.SetShouldRefreshMesh(true);

                //CREATE THE ANIMATION FOLDER IF DOESN'T EXIST
                if (!AssetDatabase.IsValidFolder("Assets/" + "Animations"))
                {
                    // StaticLogForwarder.Logger.Log("Folder Does not exist:" + "Assets/" + "Animations");
                    AssetDatabase.CreateFolder("Assets", "Animations");
                }

                //CREATE THE GAMEOBJECT SUBDIRECTORY IF DOESN'T EXIST
                if (!AssetDatabase.IsValidFolder("Assets/Animations/" + gameObject.name))
                {
                    // StaticLogForwarder.Logger.Log("Folder Does not exist:" + "Assets/Animations/" + gameObject.name);
                    string guid = AssetDatabase.CreateFolder("Assets/Animations", gameObject.name);
                    newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
                }

                Animator animator;

                //ADD THE ANIMATOR AND ITS ANIMATOR CONTROLLER TO THE GAMEOBJECT ONLY ONCE
                if (!ContainsComponent(gameObject, "Animator"))
                {
                    gameObject.AddComponent<Animator>();
                }

                animator = gameObject.GetComponent<Animator>();

                UnityEditor.Animations.AnimatorController controller = (UnityEditor.Animations.AnimatorController)animator.runtimeAnimatorController;
                if (!controller)
                {
                    controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Animations/" + gameObject.name + "/" + gameObject.name + "AnimatorController.controller");
                    animator.runtimeAnimatorController = controller;
                }

                UnityEditor.Animations.AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;

                //GET LIST OF SPINE ANIMATIONS
                SkeletonDataAsset m_skeletonDataAssets = skeletonAnimation.SkeletonDataAsset;

                if (m_skeletonDataAssets == null)
                {
                    GameIdLogger.Logger.Error("ERROR: This Spine Object does not have Spine animations");
                    return;
                }

                SkeletonData m_skeletonData = m_skeletonDataAssets.GetSkeletonData(false);

                int animIndex = 0;

                //CREATE UNITY ANIMATION CLIP FOR EVERY SPINE ANIMATION IN SKELETON
                foreach (Spine.Animation animation in m_skeletonData.Animations)
                {
                    string name = animation.Name;
                    float duration = animation.Duration;

                    //Create empty animation clip
                    AnimationClip clip = new AnimationClip();

                    //create curve for track time
                    AnimationCurve curve1 = new AnimationCurve(new Keyframe(0, 0), new Keyframe(duration, 1));
                    clip.SetCurve(gameObjectRelativePath, typeof(SpineAnimationController), "_trackTime", curve1);

                    //create curve for animation index
                    AnimationCurve curve2 = new AnimationCurve(new Keyframe(0, animIndex));
                    clip.SetCurve(gameObjectRelativePath, typeof(SpineAnimationController), "_animationIndex", curve2);

                    //turn clip into an asset file
                    AssetDatabase.CreateAsset(clip, newFolderPath + "/" + g.name + "_" + name + ".anim");

                    //Create a state of the same name as the clip
                    UnityEditor.Animations.AnimatorState state = new UnityEditor.Animations.AnimatorState();
                    state.name = g.name + "_" + name;//Assigns the name to the state.
                    state.motion = clip;//Assigns asset clip to state.

                    //Add the state to the state machine
                    stateMachine.AddState(state, pos);//add ACTUAL STATE, not string name.

                    //Adjusts the position of the next node to be created in the state machine
                    pos.y += 50.0f;
                    animIndex++;

                }
                gameObjectRelativePath = "";
                continue;
            }

            return;
        }
    }
}
#endif