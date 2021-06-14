## Have you ever wanted your shops to be accessible through a UI?

Using this plugin, players can buy and sell items and vehicles in your server's shop easily!

*This plugin is made for [OpenMod](https://openmod.github.io/openmod-docs/), the successor of RocketMod. OpenMod supports all RocketMod plugins. [Migrate to OpenMod now](https://openmod.github.io/openmod-docs/user-guide/installation/unturned/). Feel free to ask questions in the [OpenMod Discord](https://discord.gg/6yy7gxk).*

### Usage

Players can simply use /shop (or /vshop to directly access the vehicle shop).
See the media on this page to view an example of the UI.

At the moment, commands must be used to configure the shop. More info on configuring the shops can be found in the **Commands** section.

Shop whitelists/blacklists are supported. The permission you specify in the commands for setup are not exact however. If you put `eaglefire` as the whitelist permission, the actual permission would be `ShopsUI:groups.eaglefire`. The permission you specify has `ShopsUI:groups.` added to the front.

### Commands

**Commands for UI:**

- /shop - Opens the shop UI.
- /vshop - Opens the shop UI directly to the vehicle tab.

**Commands for shop management:**

Basic commands:

- `/shop add <buy | sell> <item> <price>` - Adds the item to the shop to be bought or sold.
- `/shop remove <buy | sell> <item>` - Removes the buyable/sellable item from the shop.
- `/vshop add <vehicle> <price>` - Adds the vehicle to the shop to be bought.
- `/vshop remove <vehicle>` - Removes the buyable vehicle from the shop.
- `/shop reload` - Reloads the shops from the database.

UI-related management commands:

- `/shop order <item> <order>` - Sets the order of items in the shop UI.
- `/vshop order <vehicles> <order>` - Sets the order of vehicles in the shop UI.

Whitelist/blacklist commands:

- `/shop whitelist <add | rem> <item> <permission>` - Manage item shop whitelists.
- `/shop blacklist <add | rem> <item> <permission>` - Manage item shop blacklists.
- `/vshop whitelist <add | rem> <vehicle> <permission>` - Manage vehicle shop whitelists.
- `/vshop blacklist <add | rem> <vehicle> <permission>` - Manage vehicle shop blacklists.

The permission you specify has `ShopsUI:groups.` added to the front of it. `abc` turns into `ShopsUI:groups.abc`.

**Alternative buy/sell commands:**

- `/buy <item> [amount]` - Buys the item from the shop.
- `/sell <item> [amount]` - Sells the item to the shop.
- `/vbuy <vehicle>` - Buys the vehicle from the shop.

### Configuration
```
database:
  ConnectionStrings:
    default: "Server=127.0.0.1; Database=openmod; Port=3306; User=unturned; Password=password"

shops:
  # Blacklists allow all players to access a given shop
  # unless they have a configured blacklisted permission.
  # When this setting is set to true, shop blacklists will not be ignored.
  blacklistEnabled: false

  # Whitelists allow only the players who have a configured
  # whitelisted permission to access a given shop.
  # When this setting is set to true, shop whitelists will not be ignored.
  whitelistEnabled: false

ui:
  logoUrl: "https://i.imgur.com/t6HbFTN.png"
  mainEffect: 29150
```

### Translations
Available at https://pastebin.com/raw/1PaiSqtT

### Installation

1. Run the following command to install necessary libraries:

- `openmod install SilK.OpenMod.EntityFrameworkCore`
- `openmod install SilK.Unturned.Extras`

2. Specify in your Imperial Plugins config the openmod branch.

3. Add the following workshop id to your WorkshopDownloadConfig.json file - 2412328215
