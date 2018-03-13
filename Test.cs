// <copyright file="Test.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>
namespace MedusaSharpBasic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Enums;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.SDK.Inventory;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Orbwalker;
    using Ensage.SDK.TargetSelector;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("MedusaSharpBasic", HeroId.npc_dota_hero_medusa)]
    public class Test : Plugin, IOrbwalkingMode
    {


        private readonly Lazy<IOrbwalkerManager> orbwalkerManager;

        private readonly Lazy<ITargetSelectorManager> targetManager;

        [ImportingConstructor]
        public Test([Import] Lazy<IOrbwalkerManager> orbManager, [Import] Lazy<ITargetSelectorManager> targetManager, [Import] IInventoryManager inventory) // import IOrbwalker, ITargetSelectorManager and IInventoryManager
        {

            this.orbwalkerManager = orbManager;
            this.targetManager = targetManager;

            // this.BloodThorn = this.Owner.FindItem("item_bloodthorn");
            // this.Satanic = this.Owner.FindItem("item_satanic");
            // this.SolarCrest = this.Owner.FindItem("item_solar_crest");
            // this.Halberd = this.Owner.FindItem("item_heavens_halberd");


            // inventory setter
            this.Inventory = inventory;
        }


        public MyHeroConfig Config { get; private set; }

        private IOrbwalker Orbwalker => this.orbwalkerManager.Value.Active;

        private ITargetSelector TargetSelector => this.targetManager.Value.Active;

        public IInventoryManager Inventory { get; }

        private Item BloodThorn { get; set; }

        private Item Butterfly { get; set; }

        private Item Halberd { get; set; }

        private Ability MysticSnake { get; set; }

        private Item Satanic { get; set; }

        private Item MaskOfMadness { get; set; }

        private Item HurricanePike { get; set; }

        private Ability ManaShield { get; set; }

        private Ability StoneGaze { get; set; }

        public bool CanExecute => this.Config.Enabled && this.Config.Key;

        public void Execute()
        {
            var me = ObjectManager.LocalHero;
            var target = this.TargetSelector.GetTargets().FirstOrDefault(x => x.Distance2D(me) <= me.AttackRange * 2);

            StoneGaze = me.Spellbook.SpellR;
            MysticSnake = me.Spellbook.SpellW;
            ManaShield = me.Spellbook.SpellE;

            if (!me.IsSilenced())
            {
                if (target != null && this.Config.AbilityValue.IsEnabled(this.StoneGaze.Name) && this.StoneGaze.CanBeCasted(target) && Utils.SleepCheck("MedusaSharpBasic.StoneGaze"))
                {
                    this.StoneGaze.UseAbility(target);
                    Utils.Sleep(100, "MedusaSharpBasic.StoneGaze");
                }

                // MysticSnake.
                if (this.StoneGaze.IsInAbilityPhase && this.Config.AbilityValue.IsEnabled(this.MysticSnake.Name) && this.MysticSnake.CanBeCasted() && Utils.SleepCheck("MedusaSharpBasic.MysticSnake"))
                {
                    this.MysticSnake.UseAbility(target);
                    Utils.Sleep(100, "MedusaSharpBasic.MysticSnake");
                }
                else if (!Config.AbilityValue.IsEnabled(this.StoneGaze.Name) && Config.AbilityValue.IsEnabled(this.MysticSnake.Name) && this.MysticSnake.CanBeCasted() && Utils.SleepCheck("MedusaSharpBasic.MysticSnake"))
                {
                    this.MysticSnake.UseAbility(target);
                    Utils.Sleep(100, "MedusaSharpBasic.MysticSnake");
                }
            }
            // Toggle on if comboing and target is not null
            if (this.CanExecute && this.Config.AbilityValue.IsEnabled(this.ManaShield.Name) && this.ManaShield.CanBeCasted(target) && !this.ManaShield.IsAutoCastEnabled && Utils.SleepCheck("MedusaSharpBasic.ManaShield"))
            {
                this.ManaShield.ToggleAutocastAbility();
                Utils.Sleep(150, "MedusaSharpBasic.ManaShield");
            }
            // Toggle off if target is null
            else if (this.Config.AbilityValue.IsEnabled(this.ManaShield.Name) && target == null && this.ManaShield.IsAutoCastEnabled && Utils.SleepCheck("MedusaSharpBasic.ManaShield"))
            {
                this.ManaShield.ToggleAutocastAbility();
                Utils.Sleep(150, "MedusaSharpBasic.ManaShield2");
            }

            if (this.BloodThorn != null && this.BloodThorn.IsValid && target != null && this.BloodThorn.CanBeCasted(target) && Utils.SleepCheck("MedusaSharpBasic.BT")
                && this.Config.ItemValue.IsEnabled(this.BloodThorn.Name))
            {
                this.BloodThorn.UseAbility(target);
                Utils.Sleep(150, "MedusaSharpBasic.BT");
            }

            if (this.Satanic != null && this.Satanic.IsValid && target != null && me.Health / me.MaximumHealth <= 0.2 && this.Satanic.CanBeCasted()
                && Utils.SleepCheck("MedusaSharpBasic.Satanic") && this.Config.ItemValue.IsEnabled(this.Satanic.Name))
            {
                this.Satanic.UseAbility();
                Utils.Sleep(150, "MedusaSharpBasic.Satanic");
            }

            if (this.MaskOfMadness != null && this.MaskOfMadness.IsValid && target != null && this.MaskOfMadness.CanBeCasted(target) && Utils.SleepCheck("MedusaSharpBasic.SolarCrest")
                && this.Config.ItemValue.IsEnabled(this.MaskOfMadness.Name))
            {
                this.MaskOfMadness.UseAbility(target);
                Utils.Sleep(150, "MedusaSharpBasic.SolarCrest");
            }

            if (this.Halberd != null && this.Halberd.IsValid && target != null && this.Halberd.CanBeCasted(target) && Utils.SleepCheck("MedusaSharpBasic.Halberd")
                && this.Config.ItemValue.IsEnabled(this.Halberd.Name))
            {
                this.Halberd.UseAbility(target);
                Utils.Sleep(150, "MedusaSharpBasic.Halberd");
            }

            if (this.Butterfly != null && this.Butterfly.IsValid && target != null && this.Butterfly.CanBeCasted(target) && Utils.SleepCheck("MedusaSharpBasic.Butterfly")
                && this.Config.ItemValue.IsEnabled(this.Butterfly.Name))
            {
                this.Butterfly.UseAbility(target);
                Utils.Sleep(150, "MedusaSharpBasic.Butterfly");
            }

            if (this.HurricanePike != null && this.HurricanePike.IsValid && target == null && this.HurricanePike.CanBeCasted() &&
                         this.HurricanePike.IsToggled && this.Config.ItemValue.IsEnabled(this.HurricanePike.Name) &&
                         Utils.SleepCheck("MedusaSharpBasic.HurricanePike"))
            {
                this.HurricanePike.ToggleAbility();
                Utils.Sleep(1000, "MedusaSharpBasic.HurricanePike2");
            }

        }

        protected override void OnActivate()
        {
            this.Config = new MyHeroConfig(); // create menus

            if (!this.Config.TogglerSet)
            {
                this.Config.ItemValue = this.Config.Toggler.Value;
                this.Config.AbilityValue = this.Config.AbilityToggler.Value;
                this.Config.TogglerSet = true;
            }

            this.orbwalkerManager.Value.RegisterMode(this);
            this.Inventory.CollectionChanged += this.OnInventoryChanged; // sub to inventory changed
        }

        protected override void OnDeactivate()
        {
            this.orbwalkerManager.Value.UnregisterMode(this);
            this.Config.Dispose();
            this.Inventory.CollectionChanged -= this.OnInventoryChanged;
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in args.NewItems.OfType<InventoryItem>())
                {
                    // new items
                    switch (item.Id)
                    {
                        case Ensage.AbilityId.item_bloodthorn:
                            this.BloodThorn = item.Item;
                            break;

                        case Ensage.AbilityId.item_satanic:
                            this.Satanic = item.Item;
                            break;

                        case Ensage.AbilityId.item_mask_of_madness:
                            this.MaskOfMadness = item.Item;
                            break;

                        case Ensage.AbilityId.item_heavens_halberd:
                            this.Halberd = item.Item;
                            break;

                        case Ensage.AbilityId.item_butterfly:
                            this.Butterfly = item.Item;
                            break;

                        case Ensage.AbilityId.item_hurricane_pike:
                            this.HurricanePike = item.Item;
                            break;
                    }
                }
            }

            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in args.OldItems.OfType<InventoryItem>())
                {
                    // removed items
                    switch (item.Id)
                    {
                        case Ensage.AbilityId.item_bloodthorn:
                            this.BloodThorn = null;
                            break;

                        case Ensage.AbilityId.item_satanic:
                            this.Satanic = null;
                            break;

                        case Ensage.AbilityId.item_mask_of_madness:
                            this.MaskOfMadness = null;
                            break;

                        case Ensage.AbilityId.item_heavens_halberd:
                            this.Halberd = null;
                            break;

                        case Ensage.AbilityId.item_butterfly:
                            this.Butterfly = item.Item;
                            break;

                        case Ensage.AbilityId.item_hurricane_pike:
                            this.HurricanePike = null;
                            break;
                    }
                }
            }
        }
    }

    public class MyHeroConfig : IDisposable
    {
        public AbilityToggler ItemValue, AbilityValue;

        public bool TogglerSet = false;

        public MyHeroConfig()
        {
            var itemDict = new Dictionary<string, bool>
                           {
                               { "item_bloodthorn", true },
                               { "item_satanic", true },
                               { "item_mask_of_madness", true },
                               { "item_heavens_halberd", true },
                               { "item_hurricane_pike", true },
                               { "item_butterfly", true },
                           };

            var spellDict = new Dictionary<string, bool>
                           {
                               { "medusa_mystic_snake", true },
                               { "medusa_mana_shield", true },
                               { "medusa_stone_gaze", true }
                           };

            this.Menu = MenuFactory.Create("MedusaSharpBasic");
            this.Enabled = this.Menu.Item("Enabled?", true);
            this.Key = this.Menu.Item("Combo Key", new KeyBind(32, KeyBindType.Press));
            this.AbilityToggler = this.Menu.Item("Ability Toggler", new AbilityToggler(spellDict));
            this.Toggler = this.Menu.Item("Ability Toggler", new AbilityToggler(itemDict));
        }

        public void Dispose()
        {
            this.Menu?.Dispose();
        }

        public MenuFactory Menu { get; }

        public MenuItem<bool> Enabled { get; }

        public MenuItem<AbilityToggler> AbilityToggler { get; }

        public MenuItem<KeyBind> Key { get; }

        public MenuItem<AbilityToggler> Toggler { get; }
    }
}