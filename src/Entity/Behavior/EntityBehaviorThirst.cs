using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Datastructures;

namespace dominions.vitality
{
    class EntityBehaviorThirst : EntityBehavior
    {
        ITreeAttribute thirstTree;
        public float Hydration
        {
            get { return thirstTree.GetFloat("hydration"); }
            set
            {
                thirstTree.SetFloat("hydration", GameMath.Clamp(value, 30, 42));
                entity.WatchedAttributes.MarkPathDirty("thirst");
            }
        }

        public float MaxHydration
        {
            get { return thirstTree.GetFloat("maxhydration"); }
            set { thirstTree.SetFloat("maxhydration", value); entity.WatchedAttributes.MarkPathDirty("thirst"); }
        }

        public EntityBehaviorThirst(Entity entity) : base(entity)
        {

        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            thirstTree = entity.WatchedAttributes.GetTreeAttribute("thirst");

            if (thirstTree == null)
            {
                Hydration = 1500;
                MaxHydration = 1500;
            }

            base.Initialize(properties, attributes);
        }

        float secondsSinceLastUpdate;
        public override void OnGameTick(float deltaTime)
        {
            secondsSinceLastUpdate += deltaTime;

            if (secondsSinceLastUpdate >= 10)
            {
                this.secondsSinceLastUpdate = 0;

                if (this.Hydration <= 0)
                {
                    entity.ReceiveDamage(new DamageSource()
                    {
                        Source = EnumDamageSource.Weather,
                        Type = EnumDamageType.PiercingAttack
                    }, 1);
                }

                float temp = entity.GetBehavior<EntityBehaviorBodyheat>().Bodyheat - 29; // this gives range between 1 and 13

                Hydration -= temp > 9 ? temp * 1.6f : GameMath.Clamp(temp * 1.4f, 2.5f, 9);
            }
        }

        public void Drink(int quench)
        {
            this.Hydration += quench;
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
