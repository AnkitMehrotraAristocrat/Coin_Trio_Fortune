using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class ChildAnimatorParameterAttribute : PropertyAttribute
    {
        private AnimatorControllerParameterType _parameterType;
        public AnimatorControllerParameterType ParameterType => _parameterType;

        public ChildAnimatorParameterAttribute(AnimatorControllerParameterType type)
        {
            _parameterType = type;
        }
    }
}