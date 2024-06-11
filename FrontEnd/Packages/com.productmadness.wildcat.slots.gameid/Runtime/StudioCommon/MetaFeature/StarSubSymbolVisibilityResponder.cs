using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using Slotsburg.MetaFeature;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Meant to be attached to be attached to symbols in order for them to be able to intelligently determine if they can allow star-subsymbols.
    /// The reason this was necessary was because of transitioning to new features could show the new symbols all with stars on them.
    /// </summary>
    public class StarSubSymbolVisibilityResponder : MonoBehaviour
    {
        [FieldRequiresModel] private SubSymbolEligibilityModel _subSymbolEligibilityModel = null;
        [FieldRequiresModel] private GameStateModel _gameStateModel;
        private RootReelView _rootReelView;
        private ISymbolCyclingReelView _symbolCyclingReelView;
        private Location _location; 
        private int _symbolCyclingReelViewIndex = -1; // -1 is an invalid entry in an array which this index is meant to cache

        public RootReelView RootReelView
        {
            get { return _rootReelView; }
            set { _rootReelView = value; }
        }

        public ISymbolCyclingReelView SymbolCyclingReelView
        {
            get { return _symbolCyclingReelView; }
            set { _symbolCyclingReelView = value; }
        }

        public Location Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public int SymbolCyclingReelViewIndex
        {
            get { return _symbolCyclingReelViewIndex; }
            set { _symbolCyclingReelViewIndex = value; }
        }

        private void Awake()
        {
            this.InitializeDependencies();
        }

        public bool IsAllowed()
        {
            // Stars are not allowed in any game modes other than the base game.
            if (_gameStateModel != null && (_gameStateModel.GameState != GameStateEnum.BaseSpin))
            {
                return false;
            }
            else
            {
                return _subSymbolEligibilityModel.IsPositionEligible(_rootReelView.GetInstanceID(), _location.rowIndex);
            }
        }
        
        public void ClearReferences()
        {
            _rootReelView = null;
            _location = null; 
            _symbolCyclingReelView = null;
            _symbolCyclingReelViewIndex = -1;
        }

    }
}
