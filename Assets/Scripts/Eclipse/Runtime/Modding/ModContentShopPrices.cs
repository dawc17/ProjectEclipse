using System;

namespace Eclipse.Modding
{
    // Equipment shop prices are a shop surface, kept apart from the forge model.
    public sealed class ItemShopPriceDefinition
    {
        public ModId Owner { get; }
        public DefinitionId Item { get; }
        public ModPrice Price { get; }
        public ModPrice? SecondaryPrice { get; }
        internal ItemShopPriceDefinition(ModId owner, DefinitionId item, ModPrice price, ModPrice? secondaryPrice)
        { Owner = owner; Item = item; Price = price; SecondaryPrice = secondaryPrice; }
    }

    public sealed partial class ModRegistrationTransaction
    {
        public void SetItemShopPrice(DefinitionId item, ModPrice price, ModPrice? secondaryPrice = null)
        {
            ThrowIfCompleted();
            if (!CanReferenceNamespace(item.Namespace)) throw new ModContentException("Shop price item requires a declared dependency.");
            if (!TryGetPendingItem(item, out var target) && !_catalog.TryResolveItem(item, out target))
                throw new ModContentException("Unknown shop price item: " + item);
            if (!(target is WeaponDefinition || target is ArmorDefinition || target is HelmDefinition ||
                  target is RangedDefinition || target is MagicDefinition))
                throw new ModContentException("Shop price requires equipment.");
            if (!Enum.IsDefined(typeof(ModPriceCurrency), price.Currency) || price.Amount < 1 || price.Amount > int.MaxValue)
                throw new ModContentException("Shop price amount must be 1..2147483647 coins or gems.");
            if (secondaryPrice.HasValue && (!Enum.IsDefined(typeof(ModPriceCurrency), secondaryPrice.Value.Currency) ||
                secondaryPrice.Value.Amount < 1 || secondaryPrice.Value.Amount > int.MaxValue ||
                secondaryPrice.Value.Currency == price.Currency))
                throw new ModContentException("Secondary shop price must use the other currency and amount 1..2147483647.");
            if (_p1cShopPrices.ContainsKey(target.Id)) throw new ModContentException("Duplicate shop price: " + target.Id);
            EnsureCapacityForNewRegistration();
            _p1cShopPrices.Add(target.Id, new ItemShopPriceDefinition(Mod.Id, target.Id, price, secondaryPrice));
        }
    }
}
