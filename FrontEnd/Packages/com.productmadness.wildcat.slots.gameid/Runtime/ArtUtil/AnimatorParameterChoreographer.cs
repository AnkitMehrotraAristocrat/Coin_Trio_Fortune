#region Using

using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
using ILogger = Milan.FrontEnd.Bridge.Logging.ILogger;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ArtUtil
{
    /// <summary>
    /// A controller to handle playing a multitude of AnimatorParameterControllers, calling each by their string Tag.
    /// The assumption is that each AnimatorParameterController will be responsible for calling a single event.
    /// </summary>
    public class AnimatorParameterChoreographer : MonoBehaviour
    {
        #region Inspector

        /// <summary>
        /// List of animator parameter controllers that can be executed.
        /// </summary>
        public List<AnimatorParameterController> AnimatorParameterControllers;

        #endregion

        #region Animation Events

        /// <summary>
        /// Given the controller's name, will execute updating the chosen parameter.
        /// </summary>
        /// <param name="controllerName"></param>
        public void UpdateParameterOnController(string controllerName)
        {
            var controllerFound = false;

            if (AnimatorParameterControllers == null || AnimatorParameterControllers.Count == 0)
            {
                GameIdLogger.Logger.Error("AnimatorParameterChoreographer:: AnimatorParameterControllers is empty! Attach AnimatorParameterController objects to this list!");
            }

            // Iterate through all controllers in the controllers list.
            foreach (var controller in AnimatorParameterControllers)
            {
                // If we found the controller by name...
                if (controller.Tag == controllerName)
                {
                    // Execute UpdateChosenParameter to cause its configuration to fire and play the desired animation event.
                    controller.SetParamaterData();
                    controllerFound = true;
                }
            }

            if (!controllerFound)
            {
                GameIdLogger.Logger.Error("AnimatorParameterChoreographer:: Could not find controller with tag " + controllerName + " in AnimatorParameterControllers list.");
            }
        }

        #endregion
    }
}
