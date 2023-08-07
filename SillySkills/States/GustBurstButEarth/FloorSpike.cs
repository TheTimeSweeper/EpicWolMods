using System;
using UnityEngine;

namespace SillySkills
{
    public class FloorSpike : MonoBehaviour
    {
        public Sprite TopSprite;
        public Sprite SideSprite;
        public Sprite BottomSprite;

        public SpriteRenderer spikeRendererRenderer;

        [SerializeField]
        private Transform spriteAnchor;

        [SerializeField]
        private Attack attackBox;
        public Attack AttackBox { get => attackBox; }

        [SerializeField]
        private GameObject floorContact;

        private Vector3 _summonerPosition;
        private bool _invisible;
        private AttackInfo _onHitInfo;
        private int _attackStopwatchID;

        private int _riseStopwatchID;
        private int _riseStage;
        private float[] _riseStageScales = new float[] { 0.20f, 1.00f };


        public void Init(Vector3 summonerPosition, FacingDirection direction, bool invisible)
        {
            _attackStopwatchID = ChaosStopwatch.Begin(0.2f, true, 3, 2, 0);
            _riseStopwatchID = ChaosStopwatch.Begin(0, true, 0.0417f, _riseStageScales.Length);

            _summonerPosition = summonerPosition;
            _invisible = invisible;

            _onHitInfo = attackBox.atkInfo;
            attackBox.entityCollisionEventHandlers += onAttackHit;

            toggleAttack(true);

            if (_invisible)
            {
                spikeRendererRenderer.enabled = false;
                return;
            }

            spriteAnchor.localScale = new Vector3(1, 0, 1);

            switch (direction)
            {
                case FacingDirection.Up:
                    spikeRendererRenderer.sprite = BottomSprite;
                    break;
                case FacingDirection.Right:
                    spikeRendererRenderer.sprite = SideSprite;
                    break;
                case FacingDirection.Down:
                    spikeRendererRenderer.sprite = TopSprite;
                    break;
                case FacingDirection.Left:
                    spikeRendererRenderer.sprite = SideSprite;
                    spikeRendererRenderer.flipX = true;
                    break;
            }

            PlayPlaySpawnParticles();
        }

        private void PlayPlaySpawnParticles()
        {
            PoolManager.GetPoolItem<RockMedDebrisEmitter>().EmitSingle(new int?(3), new Vector3?(transform.position + Vector3.up * 0.7f), null, null, 0f, null);
            PoolManager.GetPoolItem<RockDebrisEmitter>().EmitSingle(new int?(2), new Vector3?(transform.position), null, null, 0f, null);
            PoolManager.GetPoolItem<FloorCrackEmitter>()/*<ParticleEffect>("FloorCrater")*/.Emit(new int?(1), new Vector3?(transform.position), null, null, 0, null, null);
        }

        void OnDestroy()
        {
            attackBox.entityCollisionEventHandlers -= onAttackHit;
        }

        void Update()
        {
            switch (ChaosStopwatch.CheckInterval(_attackStopwatchID, true))
            {
                case StopwatchState.Running:
                    break;
                case StopwatchState.Ready:
                    toggleAttack(false);
                    break;
                case StopwatchState.Done:
                    BreakSelf();
                    break;
            }

            if(ChaosStopwatch.CheckInterval(_riseStopwatchID, true) == StopwatchState.Ready)
            {
                //bundling an animator crashes the game so I'm animating it manually :P
                spriteAnchor.localScale = new Vector3(1, _riseStageScales[_riseStage], 1);
                _riseStage++;
            }
        }

        private void onAttackHit(Entity givenEnt)
        {
            if (givenEnt == null || givenEnt.hurtBoxTransform == null)
            {
                return;
            }

            Vector3 otherPosition = givenEnt.hurtBoxTransform.position;
            float enemyDistanceFromSummoner = Vector2.Distance(_summonerPosition, otherPosition);
            //wacky formula that keeps them right within the rock area
            float multi = Mathf.Lerp(-25, 70, enemyDistanceFromSummoner/10);

            _onHitInfo.knockbackVector = attackBox.GetKnockbackVector(default(Vector2), default(Vector2), multi, true);
            givenEnt.ApplyKnockback(_onHitInfo);
        }

        private void toggleAttack(bool status)
        {
            AttackBox.gameObject.SetActive(status);
            floorContact.SetActive(!status && !_invisible);
        }

        private void BreakSelf()
        {
            SoundManager.PlayAudioWithDistance("RockExplode", new Vector2?(transform.position), null, 24f, -1f, SoundManager.StandardPitchRange, false);

            DustEmitter poolItem = PoolManager.GetPoolItem<DustEmitter>();
            int particleCount = 40;
            float num = 0.25f;
            Vector3? emitPosition = new Vector3?(transform.position + Vector3.down * 0.25f);
            poolItem.EmitCircle(particleCount, num, -1f, -1f, emitPosition, null);
            PoolManager.GetPoolItem<RockMedDebrisEmitter>().EmitSingle(RockDebrisType.Default, new int?(3), new Vector3?(base.transform.position + Vector3.up * 1.125f), null, null, 0f, null);
            PoolManager.GetPoolItem<RockMedDebrisEmitter>().EmitSingle(RockDebrisType.Default, new int?(3), new Vector3?(base.transform.position + Vector3.up * 1.75f), null, null, 0f, null);
            PoolManager.GetPoolItem<RockMedDebrisEmitter>().EmitSingle(RockDebrisType.Default, new int?(1), new Vector3?(base.transform.position + Vector3.up * 2.5f), null, null, 0f, null);
            PoolManager.GetPoolItem<RockMedDebrisEmitter>().EmitSingle(RockDebrisType.Default, new int?(3), new Vector3?(base.transform.position + Vector3.left * 0.33f), null, null, 0f, null);
            PoolManager.GetPoolItem<RockMedDebrisEmitter>().EmitSingle(RockDebrisType.Default, new int?(3), new Vector3?(base.transform.position + Vector3.right * 0.33f), null, null, 0f, null);

            if (gameObject != null)
                Destroy(gameObject);
        }
    }
}
