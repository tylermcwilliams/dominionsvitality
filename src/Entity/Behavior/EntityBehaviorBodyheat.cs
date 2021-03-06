﻿using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using Vintagestory.API.MathTools;
using Vintagestory.API.Datastructures;

namespace dominions.vitality
{
    class EntityBehaviorBodyheat : EntityBehavior
    {
        ITreeAttribute bodyheatTree;

        public EntityBehaviorBodyheat(Entity entity) : base(entity)
        {
        }

        public float Bodyheat
        {
            get { return bodyheatTree.GetFloat("currentbodyheat"); }
            set
            {
                bodyheatTree.SetFloat("currentbodyheat", GameMath.Clamp(value, 30, 42));
                entity.WatchedAttributes.MarkPathDirty("bodyheat");
            }
        }

        public float MaxBodyheat
        {
            get { return bodyheatTree.GetFloat("maxbodyheat"); }
            set { bodyheatTree.SetFloat("maxbodyheat", value); entity.WatchedAttributes.MarkPathDirty("bodyheat"); }
        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            bodyheatTree = entity.WatchedAttributes.GetTreeAttribute("bodyheat");

            if (bodyheatTree == null)
            {
                entity.WatchedAttributes.SetAttribute("bodyheat", bodyheatTree = new TreeAttribute());

                Bodyheat = 36;
                MaxBodyheat = 42;

            }

            base.Initialize(properties, attributes);
        }

        float secondsSinceLastUpdate;
        public override void OnGameTick(float deltaTime)
        {


            secondsSinceLastUpdate += deltaTime;
            if (secondsSinceLastUpdate >= 10)
            {
                if (this.Bodyheat < 32)
                {
                    entity.ReceiveDamage(new DamageSource()
                    {
                        Source = EnumDamageSource.Weather,
                        Type = EnumDamageType.PiercingAttack
                    }, (36 - Bodyheat) / 3);
                }

                secondsSinceLastUpdate = 0;
                IWorldAccessor world = entity.World;

                // Grab temp at block, subtract it to current temp to get initial magnitude
                float tempChange = this.Bodyheat - world.BlockAccessor.GetClimateAt(entity.ServerPos.AsBlockPos).Temperature;

                // daylight makes it warmer
                float sunStrength = 10 - (world.Calendar.DayLightStrength * 20);
                tempChange += sunStrength;

                // if player is outside, make temp more impactful
                //
                if (world.BlockAccessor.GetRainMapHeightAt(entity.ServerPos.AsBlockPos) <= entity.ServerPos.Y)
                {
                    tempChange *= 1.2f;
                }

                // distance to sea level /5

                // Make the change warmer based on saturation
                // 15 --> 0
                EntityBehaviorHunger ebh = entity.GetBehavior<EntityBehaviorHunger>();
                if (ebh != null)
                {
                    tempChange -= ebh.Saturation * 0.01f;
                }

                // if player is in liquid make colder by 10, or 20 if outright swimming
                // -10 --> -20
                if (entity.FeetInLiquid)
                {
                    tempChange += entity.Swimming ? 20 : 10;
                }

                // clothing:

                // change temperature
                this.Bodyheat -= tempChange * 0.02f;
            }
        }

        public void HeatUp(float amount)
        {
            this.Bodyheat += amount;
        }

        public override void OnEntityDeath(DamageSource damageSourceForDeath)
        {
            Bodyheat = 36;
        }

        public override string PropertyName()
        {
            return "bodyheat";
        }
    }
}
