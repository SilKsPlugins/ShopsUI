namespace ShopsUI.UI
{
    public static class UIElements
    {
        public const string HeaderImage = "HeaderImage";
        public const string HeaderText = "HeaderText";

        public const string ShopSearchPlaceholderText = "ShopSearchPlaceholder";
        public const string ShopSearchInputField = "ShopSearch";

        public const string CloseButton = "CloseButton";

        public const int MaxCategories = 20;

        public static readonly UIListDetails ShopCategoryButtons = new("ShopCategory ({0})");
        public static readonly UIListDetails ShopCategoryTexts = new("ShopCategoryText ({0})");

        public const int MaxShopItems = 100;

        public static readonly UIListDetails ShopItems = new("ShopItem ({0})");

        public const string ShopItemsClearer = "ShopItemsClearer";

        public static readonly UIListDetails ShopItemNames = new("ItemName ({0})");
        public static readonly UIListDetails ShopItemResetters = new("ShopItemResetter ({0})");
        public static readonly UIListDetails ShopItemImages = new("ItemImage ({0})");

        public static readonly UIListDetails ShopItemBuyButtons = new("ItemBuy ({0})");
        public static readonly UIListDetails ShopItemBuyPriceTexts = new("ItemBuyPrice ({0})");

        public static readonly UIListDetails ShopItemSellButtons = new("ItemSell ({0})");
        public static readonly UIListDetails ShopItemSellPriceTexts = new("ItemSellPrice ({0})");

        public static readonly UIListDetails ShopItemBackgroundCommon = new("ItemBackground_0 ({0})");
        public static readonly UIListDetails ShopItemBackgroundUncommon = new("ItemBackground_1 ({0})");
        public static readonly UIListDetails ShopItemBackgroundRare = new("ItemBackground_2 ({0})");
        public static readonly UIListDetails ShopItemBackgroundEpic = new("ItemBackground_3 ({0})");
        public static readonly UIListDetails ShopItemBackgroundLegendary = new("ItemBackground_4 ({0})");
        public static readonly UIListDetails ShopItemBackgroundMythical = new("ItemBackground_5 ({0})");

        public const int ShopNotificationDuration = 1100; //ms
        public const int MaxNotifications = 8;

        public static readonly UIListDetails ShopSuccessNotifications = new("Notification ({0})");
        public static readonly UIListDetails ShopErrorNotifications = new("NotificationError ({0})");

        public static readonly UIListDetails ShopSuccessNotificationsText = new("NotificationText ({0})");
        public static readonly UIListDetails ShopErrorNotificationsText = new("NotificationErrorText ({0})");
    }
}
