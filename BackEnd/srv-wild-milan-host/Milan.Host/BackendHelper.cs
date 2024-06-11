using Milan.Common.Interfaces.Entities;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace Wildcat.Milan.Host
{
    public class BackendHelper
    {
        /// <summary>
        /// Looking for concrete implementation of Milan.Common.Interfaces.Entities.IBackend
        /// </summary>
        public static IEnumerable<Type> GetBackendImplementations()
        {
            var type = typeof(IBackend);

            foreach (var assemblyName in typeof(Startup).Assembly.GetReferencedAssemblies())
            {
                Assembly.Load(assemblyName);
            }

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

            return types;
        }

        public static string GetBackendGameId()
        {
            var type = typeof(IBackend);

            foreach (var assemblyName in typeof(Startup).Assembly.GetReferencedAssemblies())
            {
                Assembly.Load(assemblyName);
            }

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

            var backendImplementation = types.FirstOrDefault();

            if (backendImplementation == null || types.Count() > 1) { throw new Exception("Host doesn't support having multiple implementation of IBackend."); }

            var backend = (IBackend)Activator.CreateInstance(backendImplementation);

            return backend.Metadata.BackendId;
        }
    }
}
