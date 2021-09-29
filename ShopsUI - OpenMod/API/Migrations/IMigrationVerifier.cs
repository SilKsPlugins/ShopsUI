using OpenMod.API.Commands;
using OpenMod.API.Ioc;

namespace ShopsUI.API.Migrations
{
    [Service]
    public interface IMigrationVerifier
    {
        bool CheckShouldContinueMigration(ICommandActor commandActor);
    }
}
