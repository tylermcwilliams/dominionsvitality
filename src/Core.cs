using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace dominions.vitality
{
    class Core : ModSystem
    {
        ICoreAPI api;
        ICoreClientAPI capi;

        public override void Start(ICoreAPI api)
        {
            this.api = api;

            RegisterEntityBehaviors();

            base.Start(api);
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            this.capi = api;
            RegisterThirstEvents();
            //RegisterClientEvents();

            base.StartClientSide(api);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
        }

        void RegisterEntityBehaviors()
        {
            this.api.RegisterEntityBehaviorClass("bodyheat", typeof(EntityBehaviorBodyheat));
            this.api.RegisterEntityBehaviorClass("thirst", typeof(EntityBehaviorThirst));

            this.api.RegisterBlockEntityBehaviorClass("warmentities", typeof(BehaviorWarmEntities));
        }

        bool isDrinking = false;
        void RegisterThirstEvents()
        {
            // VITALS HUD
            capi.Event.PlayerEntitySpawn += (IClientPlayer player) =>
            {
                if (player.PlayerUID == capi.World.Player.PlayerUID)
                {
                    HudElementVitals hudVitals = new HudElementVitals(capi);
                    hudVitals.ComposeBars();
                }
            };

            // MOUSE EVENT
            capi.Event.MouseDown += (MouseEvent e) =>
            {
                if (e.Button != EnumMouseButton.Right || isDrinking)
                {
                    return;
                }

                isDrinking = true;
                capi.World.ForceLiquidSelectable = true;

                long cbid = 0;
                cbid = capi.World.RegisterGameTickListener((float dt) =>
                {
                    DrinkWater(ref cbid);
                }, 1000);

            };
        }

        void DrinkWater(ref long callbackid)
        {
            if (capi.World.Player.CurrentBlockSelection != null
                &&
                capi.World.BlockAccessor.GetBlock(capi.World.Player.CurrentBlockSelection.Position).LiquidCode == "water")
            {
                capi.Network.SendEntityPacket(capi.World.Player.Entity.EntityId, 888, null);
            };

            isDrinking = false;
            capi.World.ForceLiquidSelectable = false;

            capi.World.UnregisterGameTickListener(callbackid);
        }

    }

}