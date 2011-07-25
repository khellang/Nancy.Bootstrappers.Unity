﻿namespace Nancy.BootStrappers.Unity.Tests
{
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Bootstrapper;
    using Routing;
    using Nancy.Tests;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class UnityNancyBootstrapperFixture
    {
        private readonly FakeUnityNancyBootstrapper bootstrapper;

        public UnityNancyBootstrapperFixture()
        {
            this.bootstrapper = new FakeUnityNancyBootstrapper();
            this.bootstrapper.Initialise();
        }

        [Fact]
        public void GetEngine_ReturnsEngine()
        {
            // Given, When
            var result = this.bootstrapper.GetEngine();

            // Then
            result.ShouldNotBeNull();
            result.ShouldBeOfType<INancyEngine>();
        }

        [Fact]
        public void GetAllModules_Returns_As_MultiInstance()
        {
            // Given
            this.bootstrapper.GetEngine();
            var context = new NancyContext();

            // When
            var output1 = this.bootstrapper.GetAllModules(context).Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();
            var output2 = this.bootstrapper.GetAllModules(context).Where(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath)).FirstOrDefault();

            // Then
            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetModuleByKey_Returns_As_MultiInstance()
        {
            // Given
            this.bootstrapper.GetEngine();
            var context = new NancyContext();

            // When
            var output1 = this.bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName, context);
            var output2 = this.bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName, context);

            // Then
            output1.ShouldNotBeNull();
            output2.ShouldNotBeNull();
            output1.ShouldNotBeSameAs(output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            // Given
            this.bootstrapper.GetEngine();
            this.bootstrapper.RequestContainerConfigured = false;

            // When
            this.bootstrapper.GetAllModules(new NancyContext());

            // Then
            this.bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetModuleByKey_Configures_Child_Container()
        {
            // Given
            this.bootstrapper.GetEngine();
            this.bootstrapper.RequestContainerConfigured = false;

            // When
            this.bootstrapper.GetModuleByKey(typeof(FakeNancyModuleWithBasePath).FullName, new NancyContext());

            this.bootstrapper.RequestContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_ConfigureApplicationContainer_Should_Be_Called()
        {
            // Given, When
            this.bootstrapper.GetEngine();

            // Then
            this.bootstrapper.ApplicationContainerConfigured.ShouldBeTrue();
        }

        [Fact]
        public void GetEngine_Defaults_Registered_In_Container()
        {
            // Given
            this.bootstrapper.GetEngine();
            
            // When, Then
            this.bootstrapper.Container.Resolve<INancyModuleCatalog>();
            this.bootstrapper.Container.Resolve<IRouteResolver>();
            this.bootstrapper.Container.Resolve<INancyEngine>();
            this.bootstrapper.Container.Resolve<IModuleKeyGenerator>();
            this.bootstrapper.Container.Resolve<IRouteCache>();
            this.bootstrapper.Container.Resolve<IRouteCacheProvider>();
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Same_Request_Lifetime_Instance_To_Each_Dependency()
        {
            // Given
            this.bootstrapper.GetEngine();

            // When
            var result = this.bootstrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency)), new NancyContext()) as FakeNancyModuleWithDependency;

            // Then
            result.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldBeSameAs(result.Dependency.FooDependency);
        }

        [Fact]
        public void Get_Module_By_Key_Gives_Different_Request_Lifetime_Instance_To_Each_Call()
        {
            // Given
            this.bootstrapper.GetEngine();
            var context = new NancyContext();

            // When
            var result = this.bootstrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency)), context) as FakeNancyModuleWithDependency;
            var result2 = this.bootstrapper.GetModuleByKey(new DefaultModuleKeyGenerator().GetKeyForModuleType(typeof(FakeNancyModuleWithDependency)), context) as FakeNancyModuleWithDependency;

            // Then
            result.FooDependency.ShouldNotBeNull();
            result2.FooDependency.ShouldNotBeNull();
            result.FooDependency.ShouldNotBeSameAs(result2.FooDependency);
        }
    }
}
