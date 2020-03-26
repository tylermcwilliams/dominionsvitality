using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace dominions.vitality
{
    class BehaviorWarmEntities : BlockEntityBehavior
    {
        BlockEntity block;
        ICoreAPI api;

        int heatRange;
        float heatTemp;

        public BehaviorWarmEntities(BlockEntity block) : base(block)
        {
            this.block = block;
        }

        public override void Initialize(ICoreAPI api, JsonObject properties)
        {
            if (api.Side.IsClient())
            {
                return;
            }

            this.api = api;

            heatRange = properties["heatRange"].AsInt(12);
            heatTemp = properties["heatTemp"].AsFloat(0.5f);

            block.RegisterGameTickListener(HeatEntities, 10000);

            base.Initialize(api, properties);
        }

        void HeatEntities(float dt)
        {
            IPlayer[] players = api.World.AllOnlinePlayers;

            foreach (IPlayer curPlayer in players)
            {
                EntityPlayer playerEntity = curPlayer.Entity;
                float distance = block.Pos.DistanceTo(playerEntity.Pos.AsBlockPos);

                if (distance > heatRange || !playerEntity.HasBehavior("bodyheat"))
                {
                    continue;
                }

                float distanceModifier = (heatRange - distance) / heatRange;

                switch (block.GetType().Name)
                {
                    case "BlockEntityForge":
                        if (((BlockEntityForge)block).IsBurning)
                        {
                            break;
                        }
                        continue;
                    case "BlockEntityFirepit":
                        if (((BlockEntityFirepit)block).IsBurning)
                        {
                            break;
                        }
                        continue;
                    case "BlockEntityCharcoalPit":
                        if (((BlockEntityCharcoalPit)block).Lit)
                        {
                            break;
                        }
                        continue;
                }

                playerEntity.GetBehavior<EntityBehaviorBodyheat>()?.HeatUp(heatTemp * distanceModifier);

            }
        }
    }
}
