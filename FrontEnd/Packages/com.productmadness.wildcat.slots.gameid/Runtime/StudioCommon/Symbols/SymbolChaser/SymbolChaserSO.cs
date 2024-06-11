using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Pooling;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolChaser
{
    /// <summary>
    /// A scriptable object that defines a symbol chaser and the supporting properties.
    /// Is used by the SymbolChaserView.
    /// </summary>
    [CreateAssetMenu(fileName = "SymbolChaser", menuName = "NMG/Symbol Chasers/Symbol Chaser")]
    public class SymbolChaserSO : ScriptableObject
    {
        [SerializeField] private int _targetSymbolId;
        [SerializeField] private string _animationTriggerBase = "chase";
        [SerializeField] private string _idleStateTag = "idle";
        [SerializeField] private GameObject _chaserPrefab;
        [SerializeField] private int _initialPoolCount = 1;
        [SerializeField][Tooltip("Optional")] private BaseSymbolChaserTriggerModifierSO _triggerModifier;

        private GameObjectPool _chaserPool; // Pool of chaser prefabs

        private SymbolId _targetSymbol;
        public SymbolId TargetSymbol => _targetSymbol;
        public string IdleStateTag => _idleStateTag;

        /// <summary>
        /// Initializes the target symbol, chaser pool and if applicable the trigger modifier.
        /// </summary>
        /// <param name="parent"></param>
        public void Initialize(Transform parent)
        {
            _targetSymbol = new SymbolId(_targetSymbolId);
            _chaserPool = new GameObjectPool(_chaserPrefab, parent, _initialPoolCount);
            _triggerModifier?.Initialize(GlobalObjectExtensions.GetGlobalComponent<ServiceLocator>());
        }

        /// <summary>
        /// Provides a prefab from the chaser game object pool.
        /// </summary>
        /// <returns></returns>
        public GameObject SpawnPrefab()
        {
            return _chaserPool.Spawn();
        }

        /// <summary>
        /// Returns the supplied prefab back to the chaser game object pool.
        /// </summary>
        /// <param name="prefab"></param>
        public void DespawnPrefab(GameObject prefab)
        {
            _chaserPool.Despawn(prefab);
        }

        /// <summary>
        /// Provides the animation trigger to be used.
        /// The expectation is that the trigger name is either constant or if amendable, is amended
        /// by the trigger modifier.
        /// </summary>
        /// <returns></returns>
        public string GetAnimTrigger()
        {
            // Set a local variable equal to the serialize field _animationTrigger
            string trigger = _animationTriggerBase;

            // Check if we have a _triggerModifier, if we do, invoke it's ModifyTrigger method
            // thereby updating the trigger name (local variable we generated in the step above)
            _triggerModifier?.ModifyTrigger(ref trigger);

            return trigger;
        }
    }

    [Preserve]
    [Serializable]
    public class SymbolChaserSOs : ReorderableArray<SymbolChaserSO> { }
}
