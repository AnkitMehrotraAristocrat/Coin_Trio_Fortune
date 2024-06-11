using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Pooling;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	public abstract class TensionType : BaseScriptableObjectUnsubscribable //ScriptableObject
	{
		protected ServiceLocator _serviceLocator;
		protected SymbolId _symbolId;

		public int SymbolId;
		public int Priority;
		public GameObject Prefab;

		public BaseTensionAnimProviderSO AnimProviderSO;
		public BaseTensionAnimProvider AnimProvider;
		public BaseTensionSoundProviderSO SoundProviderSO;
		public BaseTensionSoundProvider SoundProvider;

		private GameObjectPool _prefabPool;

		public virtual void Initialize(ServiceLocator serviceLocator, Transform prefabParent, SymbolLocator symbolLocator)
		{
			AnimProvider = AnimProviderSO.GetAnimProvider(this, serviceLocator);
			SoundProvider = SoundProviderSO.GetSoundProvider(this, serviceLocator, symbolLocator);
			_prefabPool = new GameObjectPool(Prefab, prefabParent, 2);
			_serviceLocator = serviceLocator;
			_symbolId = new SymbolId(SymbolId);
			Validate();
		}

		public virtual void Validate()
		{
			AnimProvider?.Validate();
			SoundProvider?.Validate();
		}

		public GameObject SpawnTensionPrefab()
		{
			return _prefabPool.Spawn();
		}

		public void DespawnTensionPrefab(GameObject prefab)
		{
			prefab.GetComponentInChildren<Animator>().SetTrigger(AnimProvider.GetIdleAnimTrigger());
			_prefabPool.Despawn(prefab);
		}

		public abstract bool IsEligible(int reelIndex);

		public virtual void ViewEnabled()
		{
			AnimProvider?.ViewEnabled();
			SoundProvider?.ViewEnabled();
		}

		public virtual void ViewDisabled()
		{
			AnimProvider?.ViewDisabled();
			SoundProvider?.ViewDisabled();
		}

		public virtual void SetSpinSubscriptions()
		{
			AnimProvider?.SetSpinSubscriptions();
			SoundProvider?.SetSpinSubcriptions();
		}

		public virtual void SpinStarted()
		{
			AnimProvider?.SpinStarted();
			SoundProvider?.SpinStarted();
		}

		public virtual void SpinCompleted()
		{
			AnimProvider?.SpinCompleted();
			SoundProvider?.SpinCompleted();
		}

		public virtual void OnReelSpin(int reelIndex)
		{
			AnimProvider?.OnReelSpin(reelIndex);
			SoundProvider?.OnReelSpin(reelIndex);
		}

		public virtual void OnReelStop(int reelIndex)
		{
			AnimProvider?.OnReelStop(reelIndex);
			SoundProvider?.OnReelStop(reelIndex);
		}
	}
}
