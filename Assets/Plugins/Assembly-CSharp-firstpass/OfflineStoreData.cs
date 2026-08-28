// Local item/UI data, not a purchasing SDK. No store or payment transport exists.
namespace SF2.Offline
{
    public enum ProductType { Consumable, NonConsumable, Subscription }
    public enum InitializationFailureReason { PurchasingUnavailable, NoProductsAvailable, AppNotKnown }
    public enum PurchaseFailureReason
    {
        PurchasingUnavailable, ExistingPurchasePending, ProductUnavailable,
        SignatureInvalid, UserCancelled, PaymentDeclined, DuplicateTransaction, Unknown
    }
    public sealed class ProductDefinition
    {
        public string id { get; private set; }
        public string storeSpecificId { get; private set; }
        public ProductType type { get; private set; }
        public ProductDefinition(string id, ProductType type)
        {
            this.id = id;
            storeSpecificId = id;
            this.type = type;
        }
    }
    public sealed class ProductMetadata
    {
        public string localizedPriceString = string.Empty;
        public decimal localizedPrice;
        public string isoCurrencyCode = string.Empty;
    }
    public sealed class Product
    {
        public ProductDefinition definition;
        public ProductMetadata metadata = new ProductMetadata();
        public string receipt = string.Empty;
        public string transactionID = string.Empty;
        public bool availableToPurchase { get { return false; } }
        public bool hasReceipt { get { return false; } }
    }
}
