using Chaos.AnimatorExtensions;
using UnityEngine;

namespace SillySkills.States
{
    public class GustBurstButBigState : Player.SkillState
    {
        public new static string staticID = "GustBurstButBig";
        private WindBurst currentWB;
        private float burstScale = 5f;

        private int stopwatchID;
        private bool switchDirection;

        private static float scaleEmpowered = 1.1f;
        private static float scaleNormal = 0.85f;
        private float scaleMultiplier => IsEmpowered ? scaleEmpowered : scaleNormal;

        private int currentCast;

        private ParticleSystemOverride implosionOverrideNormal = new ParticleSystemOverride
        {
            startSize = new float?(6 * scaleNormal),
            startLifetime = new float?(0.7f)
        };
        private ParticleSystemOverride implosionOverrideNormalLarge = new ParticleSystemOverride
        {
            startSize = new float?(12 * scaleNormal),
            startLifetime = new float?(0.6f)
        };

        private ParticleSystemOverride implosionOverrideEmpowered = new ParticleSystemOverride
        {
            startSize = new float?(6 * scaleEmpowered),
            startLifetime = new float?(0.7f)
        };
        private ParticleSystemOverride implosionOverrideEmpoweredLarge = new ParticleSystemOverride
        {
            startSize = new float?(13 * scaleEmpowered),
            startLifetime = new float?(0.6f)
        };

        private ParticleSystemOverride implosionOverride => IsEmpowered ? implosionOverrideEmpowered : implosionOverrideNormal;
        private ParticleSystemOverride implosionOverrideLarge => IsEmpowered ? implosionOverrideEmpoweredLarge : implosionOverrideNormalLarge;

        //private ParticleSystemOverride twirlOverride = new ParticleSystemOverride
        //{
        //    startSize = new float?(13),
        //    startLifetime = new float?(0.6f)
        //};

        public GustBurstButBigState(FSM newFSM, Player newEnt) : base(staticID, newFSM, newEnt)
        {
            applyStopElementStatus = true;
            this.SetAnimTimes(0.15f, 0.2f, 0.1f, 0.5f, 0.6f, 0.7f);
        }
        
        public override void OnEnter()
        {
            base.OnEnter();

            currentCast = 0;
            stopwatchID = ChaosStopwatch.Begin(0f, true, IsEmpowered ? 0.15f : 0.25f, IsEmpowered ? 3 : 2 , 0);

            parent.ToggleEnemyFloorCollisions(false);
        }

        public override void ExecuteSkill()
        {
            if (base.CancelToDash(false))
                return;

            switch (ChaosStopwatch.CheckInterval(stopwatchID, true))
            {
                case StopwatchState.Done:
                    base.ExecuteSkill();
                    break;
                case StopwatchState.Running:
                    break;
                case StopwatchState.Ready:
                    PlayAnim(animExecTime);
                    AirChannelDash_CreateImplosion();
                    currentCast++;
                    break;
            }
        }

        //all I do is copy paste
        private void AirChannelDash_CreateImplosion()
        {
            Vector3 spawnPosition = parent.attackOriginTrans.position;
            
            this.currentWB = WindBurst.CreateBurst(spawnPosition, parent.skillCategory, this.skillID, IsEmpowered ? 2 : 1, this.burstScale * scaleMultiplier);
            this.currentWB.emitParticles = false;
            //PoolManager.GetPoolItem<ParticleEffect>("WindBurstEffect").Emit(new int?(3), new Vector3?(spawnPosition), null, null, 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 33f))), 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(180f, 213f))), 0f, null, null);
            //PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f))), 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(spawnPosition), this.implosionOverrideLarge, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f))), 0f, null, null);
            
            DustEmitter poolItem = PoolManager.GetPoolItem<DustEmitter>();
            int particleCount = 150;
            float scale = 2f;
            Vector3? emitPosition = new Vector3?(spawnPosition);
            poolItem.EmitCircle(particleCount, scale, -8f, -1f, emitPosition, null);

            SoundManager.PlayAudioWithDistance("StandardHeavySwing", new Vector2?(this.parent.transform.position), null, 24f, -1f, 0.9f, false);

            //PoolManager.GetPoolItem<ParticleEffect>("WindTwirlEffect").Emit(new int?(2), new Vector3?(spawnPosition), null);
            ////ParticleEffect particleEffect2 = twirlEffect0;
            ////emissionRate = new float?(2f);
            ////position = new Vector3?(this.parent.attackOriginTrans.position);
            ////attackOriginTrans = this.parent.attackOriginTrans;
            ////Vector3? localEulerAngles = new Vector3?(new Vector3(0f, 0f, 25f));
            ////particleEffect2.Play(emissionRate, position, ParticleEffectOverrides.StartLifeTime0p25, null, attackOriginTrans, localEulerAngles, 0f, true);
        }

        private void PlayAnim(float givenTime)
        {
            parent.FaceTarget(parent.transform.position + ((!switchDirection) ? Vector3.left : Vector3.right));
            parent.anim.PlayDirectional(parent.GSlamAnimStr, -1, givenTime);
            switchDirection = !switchDirection;
        }

        public override void OnExit()
        {
            base.OnExit();
            parent.ToggleEnemyFloorCollisions(true);
        }
    }
}
