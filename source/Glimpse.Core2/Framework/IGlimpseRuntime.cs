using System.Collections.Generic;
using Glimpse.Core2.Extensibility;

namespace Glimpse.Core2.Framework
{
    //TODO: Clean this up. Mostly used for testability and mocking Glimpse
    public interface IGlimpseRuntime
    {
        string Version { get; }
        void BeginRequest();
        void EndRequest();
        void ExecuteDefaultResource();
        void ExecuteResource(string resourceName, IDictionary<string, string> parameters);
        void ExecuteResource(string resourceName, string[] parameters);
        void ExecuteTabs();
        void ExecuteTabs(LifeCycleSupport support);
        bool Initialize();
        bool IsInitialized { get; }
    }
}