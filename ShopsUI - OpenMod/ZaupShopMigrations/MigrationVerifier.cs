using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using ShopsUI.API.Migrations;
using System;
using System.Collections.Concurrent;

namespace ShopsUI.ZaupShopMigrations
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class MigrationVerifier : IMigrationVerifier
    {
        private readonly struct UserIdentifier
        {
            private string UserId { get; }

            private string UserType { get; }

            public UserIdentifier(IPermissionActor actor)
            {
                UserId = actor.Id;
                UserType = actor.Type;
            }
        }

        private readonly ConcurrentDictionary<UserIdentifier, DateTime> _lastChecked = new();

        public const float MaxConfirmationTime = 30; // seconds

        public bool CheckShouldContinueMigration(ICommandActor commandActor)
        {
            var userId = new UserIdentifier(commandActor);

            if (_lastChecked.TryGetValue(userId, out var lastChecked))
            {
                if ((DateTime.Now - lastChecked).TotalSeconds < MaxConfirmationTime)
                {
                    return true;
                }
            }

            _lastChecked.AddOrUpdate(userId, DateTime.Now, (_, _) => DateTime.Now);

            return false;
        }
    }
}
