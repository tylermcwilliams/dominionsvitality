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

            RegisterClientEvents();

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
        }

        void RegisterClientEvents()
        {
            // VITALS HUD
            capi.Event.PlayerEntitySpawn += (IClientPlayer player) =>
            {
                if (player.PlayerUID == capi.World.Player.PlayerUID)
                {
                    HudElementVitals hudVitals = new HudElementVitals(capi);
                    hudVitals.ComposeBars();

                    if (capi.World.Player.InventoryManager.ActiveHotbarSlot.Empty && capi.World.Player.Entity.WatchedAttributes.GetFloat("thirst") < 1500)
                    {
                        capi.World.ForceLiquidSelectable = true;
                    }
                }
            };

            // ON CHANGE SLOT
            capi.Event.AfterActiveSlotChanged += (ActiveSlotChangeEventArgs args) =>
            {
                if (capi.World.Player.InventoryManager.ActiveHotbarSlot.Empty && capi.World.Player.Entity.WatchedAttributes.GetFloat("thirst") < 1500)
                {
                    capi.World.ForceLiquidSelectable = true;
                }
                else
                {
                    capi.World.ForceLiquidSelectable = false;
                }
            };

            // DRINKING FROM BLOCKS
            capi.Event.MouseDown += (MouseEvent e) =>
            {
                if (e.Button != EnumMouseButton.Right
                ||
                capi.World.ForceLiquidSelectable == false)
                {
                    return;
                }

                if (capi.World.Player.CurrentBlockSelection != null && capi.World.BlockAccessor.GetBlock(capi.World.Player.CurrentBlockSelection.Position).LiquidCode == "water")
                {
                    capi.Network.SendEntityPacket(capi.World.Player.Entity.EntityId, 888, null);
                }

            };
        }
    }
}