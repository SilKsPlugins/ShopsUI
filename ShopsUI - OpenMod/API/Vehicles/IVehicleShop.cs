﻿using OpenMod.API.Permissions;
using OpenMod.Unturned.Users;
using System.Threading.Tasks;

namespace ShopsUI.API.Vehicles
{
    public interface IVehicleShop
    {
        IVehicleShopData ShopData { get; }
        
        Task<decimal> Buy(UnturnedUser user);

        Task<bool> IsPermitted(IPermissionActor user);
    }
}
