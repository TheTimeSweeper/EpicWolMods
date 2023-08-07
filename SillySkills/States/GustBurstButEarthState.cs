using Chaos.AnimatorExtensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SillySkills.States
{
    public class GustBurstButEarthState : Player.SkillState
    {
        public new static string staticID = "GustBurstButEarth";

        private AudioSource holdAudioSource;
        private CastingCircle castCircle;

        public GustBurstButEarthState(FSM newFSM, Player newEnt) : base(staticID, newFSM, newEnt)
        {
            this.disableStartCooldown = true;
            this.isHoldSkill = true;
            this.SetAnimTimes(0.15f, 0.2f, 0.3f, 0.8f, 0.9f, 1f);
		}

		public override string OnEnterAnimStr
		{
			get
			{
				return this.parent.GSlamAnimStr;
			}
		}

        public override void OnEnter()
        {
            base.OnEnter();

            base.SetSkillLevel((!this.IsEmpowered) ? 1 : 2);

            float radius = IsEmpowered ? 6f : 4.7f;
            
            this.castCircle = CastingCirclePool.Spawn("Draw", 
                                                      parent.transform.position, 
                                                      false, 
                                                      null,
                                                      true,
                                                      false,
                                                      0.0f,
                                                      radius,
                                                      radius,
                                                      CastingCircle.defaultColor,
                                                      0.2f);
            ModifyCircle(true);
        }

        public void ModifyCircle(bool fuckery)
        {
            for (int i = 2; i <= 4; i++)
            {
                castCircle.castingCircleImages[i].gameObject.SetActive(!fuckery);
            }
        }

        public override void HandleHoldButton()
        {
            this.parent.anim.PlayDirectional(this.parent.GSlamAnimStr, -1, this.animHoldTime);
            this.holdReleased = (!this.parent.inputDevice.GetButton("Skill" + this.skillSlot));
            this.holdAudioSource = SoundManager.PlayIfNotPlaying("EarthLoop", this.holdAudioSource, this.parent.transform, false, -1f, -1f);
        }

        public override void OnExit()
        {
            if (!this.skillExecuted)
            {
                this.StartCooldownTimer(-1f, true);
            }
            base.OnExit();
            StopCastingCircle();
            this.StopAudio();
        }

        private void StopAudio()
        {
            if (this.holdAudioSource != null)
            {
                this.holdAudioSource.Stop();
                this.holdAudioSource = null;
            }
        }

        public override void ExecuteSkill()
        {
            base.ExecuteSkill();
            this.StartCooldownTimer(-1f, true);

            StopCastingCircle();

            this.parent.anim.PlayDirectional(this.parent.GSlamAnimStr, -1, this.animExecTime);

            this.StopAudio();
            SoundManager.PlayAudioWithDistance("EarthExplosion", new Vector2?(this.parent.transform.position), null, 24f, -1f, 0.9f, false);

            int radius = IsEmpowered ? 5 : 4;

            List<Vector3> points = Utils.DistributePointsEvenlyAroundCircle(16, radius, parent.transform.position);
            for (int i = 0; i < points.Count; i++)
            {
                //Vector2 point = Globals.GetSafeLinecastVector(parent.transform.position, points[i] - parent.transform.position, 1, ChaosCollisions.layerAllWallAndObst);
                //point += (point - (Vector2)parent.transform.position).normalized * TestValueManager.value1;
                //bool hitWall = Vector2.Distance(points[i], point) > 0.01f;
                bool hitWall = CheckCircleExceptSpikes(points[i], 0.1f, ChaosCollisions.layerAllWallAndObst);

                CreateFloorSpike(points[i], parent.transform.position, hitWall);
            }
        }

        private void StopCastingCircle()
        {
            if (this.castCircle != null)
            {
                ModifyCircle(true);
                this.castCircle.Reset(false);
                this.castCircle = null;
            }
        }

        private bool CheckCircleExceptSpikes(Vector3 location, float radius, LayerMask layerMask)
        {
            Collider2D[] coll = Physics2D.OverlapCircleAll(location, radius, layerMask);
            for (int i = 0; i < coll.Length; i++)
            {
                if (!coll[i].gameObject.GetComponentInParent<FloorSpike>())
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateFloorSpike(Vector3 point, Vector3 playerPosition, bool invisible)
        {
            FacingDirection direction = GetDifferenceDirection(point, playerPosition);

            FloorSpike spikeScript = ChaosInst<FloorSpike>(IsEmpowered ? Assets.FloorSpikeLarge : Assets.FloorSpikeSmall,new Vector2?(point));
            spikeScript.AttackBox.SetAttackInfo(parent.skillCategory, skillID, IsEmpowered? 2: 1, false);
            spikeScript.AttackBox.knockbackOverwriteVector = parent.transform.position - point;
            spikeScript.Init(playerPosition, direction, invisible);
        }

        private FacingDirection GetDifferenceDirection(Vector3 point, Vector3 position)
        {
            Vector3 difference = point - position;
            float angle = Vector3.Angle(difference, Vector3.up);
            if(angle< 45)
            {
                return FacingDirection.Down;
            }
            if (angle < 135)
            {
                return difference.x > 0 ? FacingDirection.Left : FacingDirection.Right;
            }
            return FacingDirection.Up;
        }
    }
}
