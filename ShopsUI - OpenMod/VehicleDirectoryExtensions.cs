﻿using OpenMod.Core.Helpers;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI
{
    // todo: remove this in OpenMod 3.0.19
    public static class VehicleDirectoryExtensions
    {
        /// <summary>
        /// Searches for vehicles by the vehicle asset name.
        /// </summary>
        /// <param name="directory">The item directory service.</param>
        /// <param name="vehicleName">The name of the vehicle asset.</param>
        /// <param name="exact">If true, only exact name matches will be used.</param>
        /// <returns><b>The <see cref="IVehicleAsset"/></b> if found; otherwise, <b>null</b>.</returns>
        public static async Task<IVehicleAsset?> FindByNameAsync(this IVehicleDirectory directory, string vehicleName, bool exact = true)
        {
            if (exact)
                return (await directory.GetVehicleAssetsAsync()).FirstOrDefault(d =>
                    d.VehicleName.Equals(vehicleName, StringComparison.OrdinalIgnoreCase));

            var matches = (await directory.GetVehicleAssetsAsync())
                .Where(x => x.VehicleName.IndexOf(vehicleName, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();

            var minDist = int.MaxValue;
            IVehicleAsset? match = null;

            foreach (var asset in matches)
            {
                var distance = StringHelper.LevenshteinDistance(vehicleName, asset.VehicleName);

                // There's no lower distance
                if (distance == 0)
                    return asset;

                if (match == null || distance < minDist)
                {
                    match = asset;
                    minDist = distance;
                }
            }

            return match;
        }
    }
}
