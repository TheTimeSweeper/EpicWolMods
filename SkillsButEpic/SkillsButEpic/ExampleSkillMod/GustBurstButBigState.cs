using Chaos.AnimatorExtensions;
using UnityEngine;

namespace SkillsButEpic
{
    public class GustBurstButBigState : Player.SkillState
    {
        private WindBurst currentWB;
        private float burstScale = 5f;

        private ParticleSystemOverride implosionOverride => new ParticleSystemOverride
        {
            startSize = new float?(15.5f),
            startLifetime = new float?(0.7f)
        };

        private ParticleSystemOverride implosionOverrideLarge => new ParticleSystemOverride
        {
            startSize = new float?(16.5f),
            startLifetime = new float?(0.6f)
        };

        public GustBurstButBigState(FSM newFSM, Player newEnt) : base("GustBurstButBig", newFSM, newEnt)
        {
            this.SetAnimTimes(0.15f, 0.2f, 0.3f, 0.8f, 0.9f, 1f);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            this.parent.anim.PlayDirectional(this.parent.GSlamAnimStr, -1, this.animHoldTime);

            AirChannel_DashCreateImplosion(parent.attackOriginTrans.position);
        }


        //all I do is copy paste
        private void AirChannel_DashCreateImplosion(Vector3 spawnPosition)
        {
            //Log.Warning(skillID);
            this.currentWB = WindBurst.CreateBurst(spawnPosition, this.parent.skillCategory, this.skillID, 1, this.burstScale);
            this.currentWB.emitParticles = false;
            PoolManager.GetPoolItem<ParticleEffect>("WindBurstEffect").Emit(new int?(3), new Vector3?(spawnPosition), null, null, 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 33f))), 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(180f, 213f))), 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f))), 0f, null, null);
            PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(spawnPosition), this.implosionOverrideLarge, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f))), 0f, null, null);
            DustEmitter poolItem = PoolManager.GetPoolItem<DustEmitter>();
            int particleCount = 150;
            float scale = 2f;
            Vector3? emitPosition = new Vector3?(spawnPosition);
            poolItem.EmitCircle(particleCount, scale, -8f, -1f, emitPosition, null);
        }
    }
}
