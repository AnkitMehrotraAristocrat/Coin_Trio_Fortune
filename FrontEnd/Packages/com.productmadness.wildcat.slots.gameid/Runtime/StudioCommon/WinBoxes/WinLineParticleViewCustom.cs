using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Feature.v5_1_1.ProceduralGeo;
using Milan.FrontEnd.Slots.v5_1_1.WinLine;
using UnityEngine;


namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class WinLineParticleViewCustom : BasePayLineView, IPayLineView
    {
        [FieldRequiresSelf] private MeshFilter     _filter = null;
        //TODO: This name shouldn't be abbreviated
        [FieldRequiresSelf] private ParticleSystem _ps = null;

        [SerializeField] private Vector2 _topLeft     = new Vector2(-5, -5);
        [SerializeField] private Vector2 _bottomRight = new Vector2(5, 5);
        
        public bool instantlyClearParticles = false;
        public bool instantlyClearParticlesWhileWinCycling = false;
        private int particleCount;

        protected override void Awake()
        {
            base.Awake();    
            _filter.mesh  = null;
            particleCount = _ps.main.maxParticles;
        }

        public override void Build()
        {
            base.Build();
            
            //@TODO switch to an area based calcualtion
            var _psMain     = _ps.main;
            var _psEmision  = _ps.emission;
            
            var targetCount = particleCount * DrawnLines;
            _psMain.maxParticles    = targetCount;
            _psEmision.rateOverTime = targetCount;

            var sh = _ps.shape;
            sh.enabled       = true;
            sh.shapeType     = ParticleSystemShapeType.Mesh;
            sh.mesh          = _filter.sharedMesh;
            sh.meshSpawnMode = ParticleSystemShapeMultiModeValue.Random;
            sh.meshShapeType = ParticleSystemMeshShapeType.Triangle;

            if (instantlyClearParticlesWhileWinCycling)
                ClearWinLines();

            _ps.Play();
        }
        
        protected override bool InitShowLine()
        {
            if (_filter == null)
                return false;
            
            return base.InitShowLine();
        }

        public override void InitBuilder()
        {
            _builder = new ShapeMeshBuilder(_topLeft, _bottomRight);
        }       

        public override void ClearWinLines()
        {
            _filter.sharedMesh = null;
            
            if (instantlyClearParticles)
                _ps.Clear();
            
            _ps.Stop();
        }
    }
}
