using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace RV.Systems.EventBus
{
    /// <summary>
    /// A utility class, PredefinedAssemblyUtil, provides methods to interact with predefined assemblies.
    /// It allows retrieving all types in the current AppDomain that implement a specific interface type.
    /// For more details, <see href="https://docs.unity3d.com/2023.3/Documentation/Manual/ScriptCompileOrderFolders.html">visit Unity Documentation</see>.
    /// </summary>
    public static class PredefinedAssemblyUtil 
    {
        /// <summary>
        /// Enum that defines the specific predefined types of assemblies for navigation.
        /// </summary>    
        private enum AssemblyType 
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharpFirstPass
        }

        /// <summary>
        /// Maps the assembly name to the corresponding AssemblyType.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>AssemblyType corresponding to the assembly name, or null if no match.</returns>
        private static AssemblyType? GetAssemblyType(string assemblyName) 
        {
            return assemblyName switch
            {
                "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
                "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
                _ => null
            };
        }

        /// <summary>
        /// Adds types from the given assembly that implement the specified interface to the results collection.
        /// </summary>
        /// <param name="assemblyTypes">Array of Type objects representing all the types in the assembly.</param>
        /// <param name="interfaceType">Type representing the interface to be checked against.</param>
        /// <param name="results">Collection of types where results should be added.</param>
        private static void AddTypesFromAssembly(Type[] assemblyTypes, Type interfaceType, ICollection<Type> results) 
        {
            if (assemblyTypes == null) return;

            foreach (var type in assemblyTypes)
            {
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                {
                    results.Add(type);
                }
            }
        }

        /// <summary>
        /// Gets all types from all assemblies in the current AppDomain that implement the provided interface type.
        /// </summary>
        /// <param name="interfaceType">Interface type to get all the types for.</param>
        /// <returns>List of types implementing the provided interface type.</returns>    
        public static List<Type> GetTypes(Type interfaceType) 
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
            var types = new HashSet<Type>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var assemblyType = GetAssemblyType(assembly.GetName().Name);
                    if (assemblyType != null && !assemblyTypes.ContainsKey((AssemblyType)assemblyType))
                    {
                        assemblyTypes[(AssemblyType)assemblyType] = assembly.GetTypes();
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.LogError($"Failed to load types from assembly {assembly.GetName().Name}: {ex}");
                }
            }

            if (assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes))
            {
                AddTypesFromAssembly(assemblyCSharpTypes, interfaceType, types);
            }

            if (assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes))
            {
                AddTypesFromAssembly(assemblyCSharpFirstPassTypes, interfaceType, types);
            }

            return new List<Type>(types);
        }
    }
}
