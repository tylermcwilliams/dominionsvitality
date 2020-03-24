using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace dominions.vitality
{
    class HudElementVitals : HudElement
    {
        GuiElementStatbar bodyheatBar;
        GuiElementStatbar thirstBar;

        public HudElementVitals(ICoreClientAPI capi) : base(capi)
        {
            capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener("bodyheat", () =>
            {
                this.UpdateBodyheat();
            });

            capi.World.Player.Entity.WatchedAttributes.RegisterModifiedListener("thirst", () =>
            {
                this.UpdateThirst();
            });
        }

        void UpdateBodyheat()
        {
            if (capi.World.Player == null) return;

            bodyheatBar.SetValue(capi.World.Player.Entity.WatchedAttributes.GetFloat("bodyheat") - 30);

            if (bodyheatBar.GetValue() < 2 || bodyheatBar.GetValue() > 10)
            {
                bodyheatBar.ShouldFlash = true;
            }
            else
            {
                bodyheatBar.ShouldFlash = false;
            }
        }

        void UpdateThirst()
        {
            if (capi.World.Player == null) return;

            thirstBar.SetValue(capi.World.Player.Entity.WatchedAttributes.GetFloat("thirst", 0));

            if (thirstBar.GetValue() < 300)
            {
                thirstBar.ShouldFlash = true;
            }
            else
            {
                thirstBar.ShouldFlash = false;
            }
        }

        public void ComposeBars()
        {
            double elemToDlgPad = GuiStyle.ElementToDialogPadding;
            float width = 850;

            ElementBounds bounds = new ElementBounds()
            {
                Alignment = EnumDialogArea.CenterBottom,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = width,
                fixedHeight = 100
            }.WithFixedAlignmentOffset(0, -1);

            ElementBounds bodyheatBounds = StatbarBounds(EnumDialogArea.LeftTop, width * 0.41).WithFixedAlignmentOffset(-2, 7);
            ElementBounds thirstBarBounds = StatbarBounds(EnumDialogArea.RightTop, width * 0.41).WithFixedAlignmentOffset(-2, 7);

            ElementBounds horBarBounds = ElementStdBounds.SlotGrid(EnumDialogArea.CenterFixed, 0, 38, 10, 1);
            Composers["hudvitals"] =
                capi.Gui
                .CreateCompo("inventory-hudvitals", bounds.FlatCopy().FixedGrow(0, 10))
                .BeginChildElements(bounds)
                .AddStatbar(bodyheatBounds, GuiStyle.XPBarColor, "bodyheatstatbar")
                .AddInvStatbar(thirstBarBounds, GuiStyle.DialogBlueBgColor, "thirststatbar")
                .EndChildElements()
                .Compose();

            bodyheatBar = Composers["hudvitals"].GetStatbar("bodyheatstatbar");
            thirstBar = Composers["hudvitals"].GetStatbar("thirststatbar");

            bodyheatBar.SetLineInterval((float)2);
            bodyheatBar.FlashTime = 2;
            bodyheatBar.SetMinMax((float)0, (float)12);
            bodyheatBar.ShowValueOnHover = false;


            thirstBar.SetLineInterval((float)100);
            thirstBar.FlashTime = 2;
            thirstBar.SetMinMax((float)0, (float)1500);

            this.UpdateBodyheat();
            this.UpdateThirst();

            TryOpen();
        }

        public override bool ShouldReceiveMouseEvents()
        {
            return false;
        }

        public override bool TryClose()
        {
            return false;
        }

        public override bool ShouldReceiveKeyboardEvents()
        {
            return false;
        }

        public override void OnRenderGUI(float deltaTime)
        {
            if (capi.World.Player.WorldData.CurrentGameMode != EnumGameMode.Spectator)
            {
                base.OnRenderGUI(deltaTime);
            }
        }

        public static ElementBounds StatbarBounds(EnumDialogArea alignment, double width)
        {
            return new ElementBounds()
            {
                Alignment = alignment,
                fixedWidth = width,
                fixedHeight = GuiElementStatbar.DefaultHeight,
                BothSizing = ElementSizing.Fixed
            };
        }
    }
}