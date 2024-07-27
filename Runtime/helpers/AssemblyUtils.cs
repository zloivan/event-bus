using System;
using System.Collections.Generic;

namespace IKhom.EventBusSystem.Runtime.helpers
{
    internal static class PredefinedAssemblyUtils
    {
        private enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpFirstPass,
            AssemblyCSharpFirstPassEditor
        }

        /// <summary>
        /// Retrieves the AssemblyType enum value corresponding to the given assembly name.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to identify its type.</param>
        /// <returns>The AssemblyType enum value corresponding to the given assembly name, or null if the assembly name does not match any known assembly.</returns>
        private static AssemblyType? GetAssemblyType(string assemblyName)
        {
            const string ASSEMBLY_CSHARP = "Assembly-CSharp";
            const string ASSEMBLY_CSHARP_EDITOR = "Assembly-CSharp-Editor";
            const string ASSEMBLY_CSHARP_FIRSTPASS = "Assembly-CSharp-firstpass";
            const string ASSEMBLY_CSHARP_FIRSTPASS_EDITOR = "Assembly-CSharp-Editor-firstpass";

            return assemblyName switch
            {
                ASSEMBLY_CSHARP => AssemblyType.AssemblyCSharp,
                ASSEMBLY_CSHARP_EDITOR => AssemblyType.AssemblyCSharpEditor,
                ASSEMBLY_CSHARP_FIRSTPASS => AssemblyType.AssemblyCSharpFirstPass,
                ASSEMBLY_CSHARP_FIRSTPASS_EDITOR => AssemblyType.AssemblyCSharpFirstPassEditor,
                _ => null
            };
        }

        /// <summary>
        /// Adds all types from the specified assembly that are assignable from the given interface type to the provided collection.
        /// </summary>
        /// <param name="assembly">The array of types representing the assembly to search for types.</param>
        /// <param name="interfaceType">The interface type to search for in the assembly.</param>
        /// <param name="types">The collection to add the found types to.</param>
        private static void AddTypesFromAssembly(Type[] assembly, Type interfaceType, ICollection<Type> types)
        {
            if (assembly == null)
                return;

            for (var i = 0; i < assembly.Length; i++)
            {
                var type = assembly[i];

                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                {
                    types.Add(type);
                }
            }
        }

        /// <summary>
        /// Retrieves all types from the specified assemblies that are assignable from the given interface type.
        /// </summary>
        /// <param name="interfaceType">The interface type to search for in the assemblies.</param>
        /// <returns>A list of types that are assignable from the given interface type and found in the specified assemblies.</returns>
        public static List<Type> GetType(Type interfaceType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var assemblyTypes = new Dictionary<AssemblyType, Type[]>();
            var types = new List<Type>();

            for (var i = 0; i < assemblies.Length; i++)
            {
                var assemblyName = assemblies[i].GetName().Name;
                var assemblyType = GetAssemblyType(assemblyName);
                if (assemblyType != null)
                {
                    assemblyTypes.Add((AssemblyType)assemblyType, assemblies[i].GetTypes());
                }
            }

            if (assemblyTypes.ContainsKey(AssemblyType.AssemblyCSharp))
            {
               AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharp], interfaceType, types); 
            }

            //Assembly will be created only if Standard Assets, Pro Standard Assets and Plugins folders exist in project
            //https://docs.unity3d.com/Manual/ScriptCompileOrderFolders.html
            if (assemblyTypes.ContainsKey(AssemblyType.AssemblyCSharpFirstPass))
            {
                AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharpFirstPass], interfaceType, types);  
            }

            return types;
        }
    }
}