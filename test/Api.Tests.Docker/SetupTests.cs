using Api.Tests.Docker;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using LightBDD.XUnit2;

/*
 * This is a way to enable LightBDD - XUnit integration.
 * It is required to do it in all assemblies with LightBDD scenarios.
 * It is possible to either use [assembly:LightBddScope] directly to use LightBDD with default configuration, 
 * or customize it in a way that is shown below.
 */
[assembly: SetupTests]

namespace Api.Tests.Docker;

public class SetupTests : LightBddScopeAttribute
{
    private ICompositeService dockerCompose;

    protected override void OnSetUp()
    {
        var composeLocal = Path.Combine(Directory.GetCurrentDirectory(), "docker-compose.tests.yml");
        var composeOverride = Path.Combine(Directory.GetCurrentDirectory(), "docker-compose.override.yml");

        dockerCompose = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(composeLocal, composeOverride)
            .RemoveOrphans()
            //.WaitForHttp("AWS", "localStackURL")
            .ForceBuild()
            .Build().Start();
        
        Thread.Sleep(10000); // waiting for AWS services to start. May be replaces by WaitForHttp later?
        
        base.OnSetUp();
    }
    
    protected override void OnTearDown()
    {
        dockerCompose.Stop();
        dockerCompose.Remove();
        
        base.OnTearDown();
    }
}