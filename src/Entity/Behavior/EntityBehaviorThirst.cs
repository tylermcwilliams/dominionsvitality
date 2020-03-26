﻿using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace dominions.vitality
{
    class EntityBehaviorThirst : EntityBehavior
    {
        public float Thirst
        {
            get
            {
                return entity.WatchedAttributes.GetFloat("thirst", 0);
            }
            set
            {
                entity.WatchedAttributes.SetFloat("thirst", GameMath.Clamp(value, 0, 1500));
            }
        }

        public EntityBehaviorThirst(Entity entity) : base(entity)
        {

        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            base.Initialize(properties, attributes);
        }

        float secondsSinceLastUpdate;
        public override void OnGameTick(float deltaTime)
        {
            secondsSinceLastUpdate += deltaTime;

            if (secondsSinceLastUpdate >= 10)
            {
                this.secondsSinceLastUpdate = 0;

                if (this.Thirst <= 0)
                {
                    entity.ReceiveDamage(new DamageSource()
                    {
                        Source = EnumDamageSource.Weather,
                        Type = EnumDamageType.PiercingAttack
                    }, 1);
                }

                float temp = entity.GetBehavior<EntityBehaviorBodyheat>().Temperature - 29; // this gives range between 1 and 13

                Thirst -= temp > 9 ? temp * 1.6f : GameMath.Clamp(temp * 1.4f, 2.5f, 9);
            }
        }

        public void Drink(int quench)
        {
            this.Thirst += quench;
        }

        public override string PropertyName()
        {
            return "thirst";
        }

        public override void OnReceivedClientPacket(IServerPlayer player, int packetid, byte[] data, ref EnumHandling handled)
        {
            if (packetid == 888)
            {
                Drink(25);
            }
            base.OnReceivedClientPacket(player, packetid, data, ref handled);
        }
    }
}
