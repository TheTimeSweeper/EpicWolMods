using UnityEngine;

namespace MyBeloved {
    public class AirChannelDashGood : Player.BaseDashState {

		public AirChannelDashGood(FSM fsm, Player parentPlayer) : base("AirChannelDashGood0", fsm, parentPlayer) {
			//this.hasEmpowered = true;
			this.applyStopElementStatus = true;
			base.InitChargeSkillSettings(2, 0f, this.skillData, this);
		}

		public override void SetEmpowered(bool givenStatus, BoolVarStatMod givenMod) {
			base.SetEmpowered(givenStatus, givenMod);
			this.burstScale = ((!this.IsEmpowered) ? 1.75f : 2f);
		}

		public override void OnEnter() {
			base.OnEnter();
			if (this.cooldownReady) {
				this.spawnPosition = this.parent.attackOriginTrans.position;
				this.CreateImplosion();
				SoundManager.PlayAudioWithDistance("StandardHeavySwing", new Vector2?(this.parent.transform.position), null, 24f, -1f, 1.4f, false);
				PoolManager.GetPoolItem<SectionedTrailEmitter>("WindTrail").Emit(this.spawnPosition, this.spawnPosition + this.inputVector * 5f, -1, false, -1f, true, 0.3f, 0.15f, null, true, null, null);
				PoolManager.GetPoolItem<SectionedTrailEmitter>("WindTrail").Emit(this.spawnPosition, this.spawnPosition + this.inputVector * 5f, -1, false, -1f, true, 0.4f, 0.15f, null, true, null, null);
			}
		}

		public override void OnExit() {
			if (this.cooldownReady && !this.fsm.nextStateName.Contains("Hurt") && !this.fsm.nextStateName.Contains("Dead")) {
				this.CreateAirChannel();
				if (this.IsEmpowered) {
					this.spawnPosition = this.parent.attackOriginTrans.position;
					this.CreateImplosion();
				}
			}
			base.OnExit();
		}

		private void CreateImplosion() {
			//Log.Warning(skillID);
			this.currentWB = WindBurst.CreateBurst(this.spawnPosition, this.parent.skillCategory, this.skillID, 1, this.burstScale);
			this.currentWB.emitParticles = false;
			PoolManager.GetPoolItem<ParticleEffect>("WindBurstEffect").Emit(new int?(3), new Vector3?(this.spawnPosition), null, null, 0f, null, null);
			PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(this.spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 33f))), 0f, null, null);
			PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(this.spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(180f, 213f))), 0f, null, null);
			PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(this.spawnPosition), this.implosionOverride, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f))), 0f, null, null);
			PoolManager.GetPoolItem<ParticleEffect>("AirVortex").Emit(new int?(1), new Vector3?(this.spawnPosition), this.implosionOverrideLarge, new Vector3?(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f))), 0f, null, null);
			DustEmitter poolItem = PoolManager.GetPoolItem<DustEmitter>();
			int particleCount = 150;
			float scale = 2f;
			Vector3? emitPosition = new Vector3?(this.spawnPosition);
			poolItem.EmitCircle(particleCount, scale, -8f, -1f, emitPosition, null);
		}

		private void CreateAirChannel() {
			this.currentAC = base.ChaosInst<AirChannel>(AirChannel.Prefab, new Vector2?(this.spawnPosition), new Quaternion?(Globals.GetRotationQuaternion(this.inputVector)), null);
			this.currentAC.attack.SetAttackInfo(this.parent.skillCategory, this.skillID, 2, false);
			this.currentAC.attack.knockbackOverwriteVector = this.inputVector;
			this.currentAC.targetVector = this.inputVector;
		}

		public new static string staticID = "AirChannelDash";

        private WindBurst currentWB;

        private AirChannel currentAC;

        private Vector2 spawnPosition;

        private float burstScale = 1.75f;

        private ParticleSystemOverride implosionOverride = new ParticleSystemOverride {
            startSize = new float?(5.5f),
            startLifetime = new float?(0.7f)
        };

        private ParticleSystemOverride implosionOverrideLarge = new ParticleSystemOverride {
            startSize = new float?(6.5f),
            startLifetime = new float?(0.6f)
        };


    }
}
