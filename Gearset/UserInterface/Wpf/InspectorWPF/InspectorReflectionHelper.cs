using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Gearset.Helpers;

namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Delegate used for methods that return the value of a variable.
    /// </summary>
    public delegate Object Getter(params Object[] o);

    /// <summary>
    /// Delegate used for methods that sets the value of a variable.
    /// </summary>
    public delegate void Setter(params Object[] o);

    internal static class InspectorReflectionHelper
    {
        /// <summary>
        /// Used to store the setters and getters for types we've
        /// already generated.
        /// </summary>
        private static Dictionary<String, Dictionary<MemberInfo, SetterGetterPair>> setterGetterCache;
        
        /// <summary>
        /// Helper class to store a setter and a getter for a specified
        /// FieldInfo in the setterGetterCache dictionary.
        /// </summary>
        internal class SetterGetterPair
        {
            internal Setter Setter;
            internal Getter Getter;
            internal SetterGetterPair(Setter setter, Getter getter)
            {
                Setter = setter;
                Getter = getter;
            }
        }

        static InspectorReflectionHelper()
        {
            setterGetterCache = new Dictionary<String, Dictionary<MemberInfo, SetterGetterPair>>();
        }

        /// <summary>
        /// Method that creates a Diccionary of methods to set/get the value of
        /// each member of an object of the specified type. But this object only
        /// reachable by the path specified. For example, to get the Position (Vector3)
        /// from a player, the parameters must be: path="Position.", t=Vector3.
        /// If the object being ispected is the class World which contains a
        /// player then parameters must be: path="Player.Position.", t=Vector3.
        /// 
        /// This method is ~10X faster than the first one which created C# code.
        /// </summary>
        internal static Dictionary<MemberInfo, SetterGetterPair> GetSetterGetterDict3(InspectorNode node)
        {
            Type nodeType = node.Type;
            Type targetType = node.Target.GetType();
            String expandingObjectTypeName = CreateCSharpTypeString(nodeType);
            String baseObjectTypeName = CreateCSharpTypeString(node.Target.GetType());
            String dictionaryKey = node.Root.Name + node.GetPath();

            // Check if we haven't already generated methods for this node.
            if (setterGetterCache.ContainsKey(dictionaryKey))
            {
                return setterGetterCache[dictionaryKey];
            }
            else
            {
                // Get all instance fields and properties.
                List<MemberInfo> members = (List<MemberInfo>)nodeType.GetFieldsAndProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // Code and return types for the methods.
                List<String> codes = new List<string>();
                List<Type> types = new List<Type>();

                // We haven't created code to set the fields/properties for this kind of node
                // we have to generated code now and compile it generating methods.
                var setterGetterDict = new Dictionary<MemberInfo, SetterGetterPair>();

                // Defines wether a field or property can be set/get because of a parent being
                // read only.
                bool canWriteParent = false;

                InspectorNode iter = node;
                // Create the path to this node, path will always contain at least
                // two elements because the root is always there.
                List<InspectorNode> path = new List<InspectorNode>(5);
                while (iter != null)
                {
                    path.Add(iter);
                    iter = iter.Parent;
                }

                // Add the reference to the Type of the node, and the target type assemblies.
                ReflectionHelper.AddReferencedAssembly(nodeType.Assembly.Location);
                ReflectionHelper.AddReferencedAssembly(node.Target.GetType().Assembly.Location);

                AddReferencedAssemblies(nodeType);
                AddReferencedAssemblies(node.Target.GetType());

                // Find out which is the last parent that we need to asign.
                // values must be reassigned on the path as long as the parent
                // is a read-only struct.
                // If there's no object or writable valuetype in the chain,
                // we can write to the field/property (if the field/property
                // is writable also.
                int breakIndex = 0;
                if (path.Count > 1)
                {
                    // Assume we can write until we prove we can't
                    canWriteParent = true;
                    for (breakIndex = 0; breakIndex < path.Count; ++breakIndex)
                    {
                        // if parent is a read-only value type, continue.
                        if (path[breakIndex].Type.IsValueType)
                            if (!path[breakIndex].CanWrite)
                            {
                                // Sorry, we can't write.
                                canWriteParent = false;
                                break;
                            }
                            else
                                continue;
                        else
                            break;
                    }
                    //breakIndex--;
                }
                else
                {
                    canWriteParent = true;
                    breakIndex = 0;
                }

                // Create an assembly.
                // TODO: Reuse assembly, possible?
                //AssemblyName myAssemblyName = new AssemblyName();
                //myAssemblyName.Name = "Inspector";

                //AssemblyBuilder myAssembly = Thread.GetDomain().DefineDynamicAssembly(myAssemblyName,  AssemblyBuilderAccess.Run, "c:\\");

                //// Create a module. For a single-file assembly the module
                //// name is usually the same as the assembly name.
                //ModuleBuilder myModule = myAssembly.DefineDynamicModule(myAssemblyName.Name, true);

                //// Define a public class 'Example'.
                //TypeBuilder myTypeBuilder = myModule.DefineType("Example", TypeAttributes.Public);

                // Create GET methods for every field
                foreach (MemberInfo memberInfo in members)
                {
                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                    FieldInfo fieldInfo = memberInfo as FieldInfo;
                    Type memberType = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;

                    setterGetterDict.Add(memberInfo, new SetterGetterPair(null, null));

                    // Create the 'Function1' public method, which takes an integer
                    // and returns a string.

                    DynamicMethod myMethod = new DynamicMethod("XdtkGet_" + memberInfo.Name, typeof(Object), new Type[] { typeof(Object[]) }, typeof(Inspector), true);
                    //MethodBuilder myMethod = myTypeBuilder.DefineMethod("XdtkGet_" + memberInfo.Name,
                    //   MethodAttributes.Public | MethodAttributes.Static,
                    //   typeof(Object), new Type[] { typeof(Object[]) });

                    ILGenerator ilGenerator = myMethod.GetILGenerator();

                    // Load the target object
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Ldelem_Ref);
                    ilGenerator.Emit(OpCodes.Castclass, targetType);

                    for (int i = path.Count - 2; i >= 0; --i)
                    {
                        InspectorNode parent = path[i];
                        // Local variable (aka v#) to store this section of the path.
                        if (parent.IsProperty)
                        {
                            ilGenerator.Emit(OpCodes.Callvirt, path[i + 1].Type.GetProperty(parent.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true));
                            if (parent.Type.IsValueType)
                            {
                                LocalBuilder tempLocal = ilGenerator.DeclareLocal(parent.Type);
                                ilGenerator.Emit(OpCodes.Stloc_S, tempLocal);
                                ilGenerator.Emit(OpCodes.Ldloca_S, tempLocal);
                            }
                        }
                        else
                        {
                            if (parent.Type.IsValueType)
                                ilGenerator.Emit(OpCodes.Ldflda, path[i + 1].Type.GetField(parent.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                            else
                                ilGenerator.Emit(OpCodes.Ldfld, path[i + 1].Type.GetField(parent.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                        }
                    }

                    // Get the value now!
                    if (propertyInfo != null)
                    {
                        MethodInfo getMethod = propertyInfo.GetGetMethod(true);
                        if (getMethod == null)
                            continue;
                        ilGenerator.Emit(OpCodes.Callvirt, getMethod);
                    }
                    else
                        ilGenerator.Emit(OpCodes.Ldfld, fieldInfo);

                    if (memberType.IsValueType)
                        ilGenerator.Emit(OpCodes.Box, memberType);

                    // Return
                    ilGenerator.Emit(OpCodes.Ret);

                    //if (myMethod.InitLocals == false)
                    //    System.Diagnostics.Debugger.Break();

                    setterGetterDict[memberInfo].Getter = (Getter)myMethod.CreateDelegate(typeof(Getter));
                }


                // Create SET methods for every field
                foreach (MemberInfo memberInfo in members)
                {
                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                    FieldInfo fieldInfo = memberInfo as FieldInfo;

                    // If it's a read-only property, continue.
                    if (propertyInfo != null && (propertyInfo.GetSetMethod(true) == null || !propertyInfo.CanWrite))
                        continue;

                    // Create the 'Function1' public method, which takes an integer
                    // and returns a string.
                    DynamicMethod myMethod = new DynamicMethod("XdtkSet_" + memberInfo.Name, null, new Type[] { typeof(Object[]) }, typeof(Inspector), true);
                    myMethod.InitLocals = true;
                    //MethodBuilder myMethod = myTypeBuilder.DefineMethod("XdtkSet_" + memberInfo.Name,
                    //   MethodAttributes.Public | MethodAttributes.Static,
                    //   typeof(void), new Type[] { typeof(Object[]) });

                    ILGenerator ilGenerator = myMethod.GetILGenerator();

                    // This boolean used to be how we controlled the licensing of Gearset, but now
                    // is hard-wired to true.
                    bool willGenerateSetter = true;
                    if (node.Type.Assembly == typeof(GearConsole).Assembly ||
                        (node.Parent != null && node.Parent.Type.Assembly == typeof(GearConsole).Assembly))
                        willGenerateSetter = true;
                    if (!willGenerateSetter)
                    {
                        ilGenerator.Emit(OpCodes.Ldc_R4, 4f);
                        ilGenerator.Emit(OpCodes.Stsfld, typeof(GearConsole).GetField("LiteVersionNoticeAlpha", BindingFlags.Public | BindingFlags.Static));
                    }
                    else
                    {
                        Stack<LocalBuilder> locals = new Stack<LocalBuilder>();

                        // Load the target object into local 0.
                        LocalBuilder targetLocal = ilGenerator.DeclareLocal(targetType);
                        ilGenerator.Emit(OpCodes.Ldarg_0);
                        ilGenerator.Emit(OpCodes.Ldc_I4_0);
                        ilGenerator.Emit(OpCodes.Ldelem_Ref);
                        ilGenerator.Emit(OpCodes.Castclass, targetType);
                        ilGenerator.Emit(OpCodes.Stloc_S, targetLocal);
                        locals.Push(targetLocal);

                        for (int i = path.Count - 2; i >= 0; --i)
                        {
                            InspectorNode parent = path[i];

                            // Load a reference to the previous variable.
                            if (path[i + 1].Type.IsValueType)
                                ilGenerator.Emit(OpCodes.Ldloca, locals.Peek());
                            else
                                ilGenerator.Emit(OpCodes.Ldloc, locals.Peek());

                            // Read the new variable.
                            if (parent.IsProperty)
                                ilGenerator.Emit(OpCodes.Callvirt, path[i + 1].Type.GetProperty(parent.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true));
                            else
                                ilGenerator.Emit(OpCodes.Ldfld, path[i + 1].Type.GetField(parent.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

                            // Store it in a new local.
                            LocalBuilder l = ilGenerator.DeclareLocal(parent.Type);
                            locals.Push(l);
                            ilGenerator.Emit(OpCodes.Stloc_S, l);
                        }

                        Type memberType = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;

                        // Load the object or struct to set the property to.
                        if (path[0].Type.IsValueType)
                            ilGenerator.Emit(OpCodes.Ldloca, locals.Peek());
                        else
                            ilGenerator.Emit(OpCodes.Ldloc, locals.Peek());

                        // Load the value to be set (as object) and cast/unbox it.
                        ilGenerator.Emit(OpCodes.Ldarg_0);
                        ilGenerator.Emit(OpCodes.Ldc_I4_1);
                        ilGenerator.Emit(OpCodes.Ldelem_Ref);
                        if (memberType.IsValueType)
                            ilGenerator.Emit(OpCodes.Unbox_Any, memberType);
                        else
                            ilGenerator.Emit(OpCodes.Castclass, memberType);

                        // Set the value now!
                        if (propertyInfo != null)
                            ilGenerator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod(true));
                        else
                            ilGenerator.Emit(OpCodes.Stfld, fieldInfo);

                        // Now set all fields/properties in reverse order
                        for (int i = 0; i < path.Count - 1; i++)
                        {
                            InspectorNode parent = path[i];

                            // Push to object to set the value to.
                            LocalBuilder valueToSet = locals.Pop();
                            if (locals.Peek().LocalType.IsValueType)
                                ilGenerator.Emit(OpCodes.Ldloca_S, locals.Peek());
                            else
                                ilGenerator.Emit(OpCodes.Ldloc_S, locals.Peek());

                            // Push the value to set
                            ilGenerator.Emit(OpCodes.Ldloc, valueToSet);

                            //ilGenerator.Emit(OpCodes.Ldloc, local);
                            // Local variable (aka v#) to store this section of the path.
                            if (parent.IsProperty)
                            {
                                MethodInfo methodInfo = path[i + 1].Type.GetProperty(parent.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetSetMethod(true);
                                if (methodInfo == null)
                                {
                                    ilGenerator.Emit(OpCodes.Pop);
                                    ilGenerator.Emit(OpCodes.Pop);
                                    break;
                                }
                                else
                                    ilGenerator.Emit(OpCodes.Callvirt, methodInfo);
                            }
                            else
                            {
                                ilGenerator.Emit(OpCodes.Stfld, path[i + 1].Type.GetField(parent.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                            }
                        }
                    }
                    // Return
                    ilGenerator.Emit(OpCodes.Ret);

                    //if (myMethod.InitLocals == false)
                    //    System.Diagnostics.Debugger.Break();

                    setterGetterDict[memberInfo].Setter = (Setter)myMethod.CreateDelegate(typeof(Setter));

                }


                //Type finalType = myTypeBuilder.CreateType();
                //myAssembly.Save("SetterGetterMethods.dll");


                // Build the dictionary
                //foreach (MemberInfo memberInfo in members)
                //{
                //    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                //    if (propertyInfo != null && propertyInfo.GetIndexParameters().Length > 0)
                //        continue;
                //    MethodInfo[] finalMethods = finalType.GetMethods();

                //    // HACK: If there are two methods with the same name (i.e. new'd in a derived class)
                //    MethodInfo setterInfo = finalMethods.FirstOrDefault((m) => m.Name == "XdtkSet_" + memberInfo.Name);
                //    MethodInfo getterInfo = finalMethods.FirstOrDefault((m) => m.Name == "XdtkGet_" + memberInfo.Name);

                //    if (setterInfo != null)
                //        setterGetterDict[memberInfo].Setter = (Setter)Delegate.CreateDelegate(typeof(Setter), setterInfo);
                //    if (getterInfo != null)
                //        setterGetterDict[memberInfo].Getter = (Getter)Delegate.CreateDelegate(typeof(Getter), getterInfo); ;
                //}
                return setterGetterDict;
            }
        }

        /// <summary>
        /// Method that creates a Diccionary of methods to set/get the value of
        /// each member of an object of the specified type. But this object only
        /// reachable by the path specified. For example, to get the Position (Vector3)
        /// from a player, the parameters must be: path="Position.", t=Vector3.
        /// If the object being ispected is the class World which contains a
        /// player then parameters must be: path="Player.Position.", t=Vector3.
        /// 
        /// This method is ~10X faster than the previous one that creaated C# code.
        /// </summary>
        internal static Dictionary<MemberInfo, SetterGetterPair> GetSetterGetterDict2(InspectorNode node)
        {
            Type nodeType = node.Type;
            Type targetType = node.Target.GetType();
            String expandingObjectTypeName = CreateCSharpTypeString(nodeType);
            String baseObjectTypeName = CreateCSharpTypeString(node.Target.GetType());
            String dictionaryKey = node.Root.Name + node.GetPath();

            // Check if we haven't already generated methods for this node.
            if (setterGetterCache.ContainsKey(dictionaryKey))
            {
                return setterGetterCache[dictionaryKey];
            }
            else
            {
                // Get all instance fields and properties.
                List<MemberInfo> members = (List<MemberInfo>)nodeType.GetFieldsAndProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                // Code and return types for the methods.
                List<String> codes = new List<string>();
                List<Type> types = new List<Type>();

                // We haven't created code to set the fields/properties for this kind of node
                // we have to generated code now and compile it generating methods.
                var setterGetterDict = new Dictionary<MemberInfo, SetterGetterPair>();

                // Defines wether a field or property can be set/get because of a parent being
                // read only.
                bool canWriteParent = false;

                InspectorNode iter = node;
                // Create the path to this node, path will always contain at least
                // two elements because the root is always there.
                List<InspectorNode> path = new List<InspectorNode>(5);
                while (iter != null)
                {
                    path.Add(iter);
                    iter = iter.Parent;
                }

                // Add the reference to the Type of the node, and the target type assemblies.
                ReflectionHelper.AddReferencedAssembly(nodeType.Assembly.Location);
                ReflectionHelper.AddReferencedAssembly(node.Target.GetType().Assembly.Location);

                AddReferencedAssemblies(nodeType);
                AddReferencedAssemblies(node.Target.GetType());

                // Find out which is the last parent that we need to asign.
                // values must be reassigned on the path as long as the parent
                // is a read-only struct.
                // If there's no object or writable valuetype in the chain,
                // we can write to the field/property (if the field/property
                // is writable also.
                int breakIndex = 0;
                if (path.Count > 1)
                {
                    // Assume we can write until we prove we can't
                    canWriteParent = true;
                    for (breakIndex = 0; breakIndex < path.Count; ++breakIndex)
                    {
                        // if parent is a read-only value type, continue.
                        if (path[breakIndex].Type.IsValueType)
                            if (!path[breakIndex].CanWrite)
                            {
                                // Sorry, we can't write.
                                canWriteParent = false;
                                break;
                            }
                            else
                                continue;
                        else
                            break;
                    }
                    //breakIndex--;
                }
                else
                {
                    canWriteParent = true;
                    breakIndex = 0;
                }

                // Create an assembly.
                // TODO: Reuse assembly, possible?
                AssemblyName myAssemblyName = new AssemblyName();
                myAssemblyName.Name = "Inspector";

                AssemblyBuilder myAssembly = Thread.GetDomain().DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.Run, "c:\\");

                // Create a module. For a single-file assembly the module
                // name is usually the same as the assembly name.
                ModuleBuilder myModule = myAssembly.DefineDynamicModule(myAssemblyName.Name, true);

                // Define a public class 'Example'.
                TypeBuilder myTypeBuilder = myModule.DefineType("Example", TypeAttributes.Public);

                // Create GET methods for every field
                foreach (MemberInfo memberInfo in members)
                {
                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                    FieldInfo fieldInfo = memberInfo as FieldInfo;
                    Type memberType = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;

                    setterGetterDict.Add(memberInfo, new SetterGetterPair(null, null));

                    // Create the 'Function1' public method, which takes an integer
                    // and returns a string.
                    MethodBuilder myMethod = myTypeBuilder.DefineMethod("XdtkGet_" + memberInfo.Name,
                       MethodAttributes.Public | MethodAttributes.Static,
                       typeof(Object), new Type[] { typeof(Object[]) });

                    ILGenerator ilGenerator = myMethod.GetILGenerator();

                    // Load the target object
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    ilGenerator.Emit(OpCodes.Ldelem_Ref);
                    ilGenerator.Emit(OpCodes.Castclass, targetType);

                    for (int i = path.Count - 2; i >= 0; --i)
                    {
                        InspectorNode parent = path[i];
                        // Local variable (aka v#) to store this section of the path.
                        if (parent.IsProperty)
                        {
                            ilGenerator.Emit(OpCodes.Callvirt, path[i + 1].Type.GetProperty(parent.Name).GetGetMethod(true));
                            if (parent.Type.IsValueType)
                            {
                                LocalBuilder tempLocal = ilGenerator.DeclareLocal(parent.Type);
                                ilGenerator.Emit(OpCodes.Stloc_S, tempLocal);
                                ilGenerator.Emit(OpCodes.Ldloca_S, tempLocal);
                            }
                        }
                        else
                        {
                            if (parent.Type.IsValueType)
                                ilGenerator.Emit(OpCodes.Ldflda, path[i + 1].Type.GetField(parent.Name));
                            else
                                ilGenerator.Emit(OpCodes.Ldfld, path[i + 1].Type.GetField(parent.Name));
                        }
                    }

                    // Get the value now!
                    if (propertyInfo != null)
                    {
                        MethodInfo getMethod = propertyInfo.GetGetMethod();
                        if (getMethod == null)
                            continue;
                        ilGenerator.Emit(OpCodes.Callvirt, getMethod);
                    }
                    else
                        ilGenerator.Emit(OpCodes.Ldfld, fieldInfo);

                    if (memberType.IsValueType)
                        ilGenerator.Emit(OpCodes.Box, memberType);

                    // Return
                    ilGenerator.Emit(OpCodes.Ret);
                }


                // Create SET methods for every field
                foreach (MemberInfo memberInfo in members)
                {
                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                    FieldInfo fieldInfo = memberInfo as FieldInfo;

                    // If it's a read-only property, continue.
                    if (propertyInfo != null && (propertyInfo.GetSetMethod() == null || !propertyInfo.CanWrite))
                        continue;

                    // Create the 'Function1' public method, which takes an integer
                    // and returns a string.
                    MethodBuilder myMethod = myTypeBuilder.DefineMethod("XdtkSet_" + memberInfo.Name,
                       MethodAttributes.Public | MethodAttributes.Static,
                       typeof(void), new Type[] { typeof(Object[]) });

                    ILGenerator ilGenerator = myMethod.GetILGenerator();

                    // This boolean used to be how we controlled the licensing of Gearset, but now
                    // is hard-wired to true.
                    bool willGenerateSetter = true;
                    if (node.Type.Assembly == typeof(GearConsole).Assembly ||
                        (node.Parent != null && node.Parent.Type.Assembly == typeof(GearConsole).Assembly))
                        willGenerateSetter = true;
                    if (!willGenerateSetter)
                    {
                        ilGenerator.Emit(OpCodes.Ldc_R4, 4f);
                        ilGenerator.Emit(OpCodes.Stsfld, typeof(GearConsole).GetField("LiteVersionNoticeAlpha", BindingFlags.Public | BindingFlags.Static));
                    }
                    else
                    {
                        Stack<LocalBuilder> locals = new Stack<LocalBuilder>();

                        // Load the target object into local 0.
                        LocalBuilder targetLocal = ilGenerator.DeclareLocal(targetType);
                        ilGenerator.Emit(OpCodes.Ldarg_0);
                        ilGenerator.Emit(OpCodes.Ldc_I4_0);
                        ilGenerator.Emit(OpCodes.Ldelem_Ref);
                        ilGenerator.Emit(OpCodes.Castclass, targetType);
                        ilGenerator.Emit(OpCodes.Stloc_S, targetLocal);
                        locals.Push(targetLocal);

                        for (int i = path.Count - 2; i >= 0; --i)
                        {
                            InspectorNode parent = path[i];

                            // Load a reference to the previous variable.
                            if (path[i + 1].Type.IsValueType)
                                ilGenerator.Emit(OpCodes.Ldloca, locals.Peek());
                            else
                                ilGenerator.Emit(OpCodes.Ldloc, locals.Peek());

                            // Read the new variable.
                            if (parent.IsProperty)
                                ilGenerator.Emit(OpCodes.Callvirt, path[i + 1].Type.GetProperty(parent.Name).GetGetMethod(true));
                            else
                                ilGenerator.Emit(OpCodes.Ldfld, path[i + 1].Type.GetField(parent.Name));

                            // Store it in a new local.
                            LocalBuilder l = ilGenerator.DeclareLocal(parent.Type);
                            locals.Push(l);
                            ilGenerator.Emit(OpCodes.Stloc_S, l);
                        }

                        Type memberType = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;

                        // Load the object or struct to set the property to.
                        if (path[0].Type.IsValueType)
                            ilGenerator.Emit(OpCodes.Ldloca, locals.Peek());
                        else
                            ilGenerator.Emit(OpCodes.Ldloc, locals.Peek());

                        // Load the value to be set (as object) and cast/unbox it.
                        ilGenerator.Emit(OpCodes.Ldarg_0);
                        ilGenerator.Emit(OpCodes.Ldc_I4_1);
                        ilGenerator.Emit(OpCodes.Ldelem_Ref);
                        if (memberType.IsValueType)
                            ilGenerator.Emit(OpCodes.Unbox_Any, memberType);
                        else
                            ilGenerator.Emit(OpCodes.Castclass, memberType);

                        // Set the value now!
                        if (propertyInfo != null)
                            ilGenerator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                        else
                            ilGenerator.Emit(OpCodes.Stfld, fieldInfo);

                        // Now set all fields/properties in reverse order
                        for (int i = 0; i < path.Count - 1; i++)
                        {
                            InspectorNode parent = path[i];

                            // Push to object to set the value to.
                            LocalBuilder valueToSet = locals.Pop();
                            if (locals.Peek().LocalType.IsValueType)
                                ilGenerator.Emit(OpCodes.Ldloca_S, locals.Peek());
                            else
                                ilGenerator.Emit(OpCodes.Ldloc_S, locals.Peek());

                            // Push the value to set
                            ilGenerator.Emit(OpCodes.Ldloc, valueToSet);

                            //ilGenerator.Emit(OpCodes.Ldloc, local);
                            // Local variable (aka v#) to store this section of the path.
                            if (parent.IsProperty)
                            {
                                MethodInfo methodInfo = path[i + 1].Type.GetProperty(parent.Name).GetSetMethod(false);
                                if (methodInfo == null)
                                {
                                    ilGenerator.Emit(OpCodes.Pop);
                                    ilGenerator.Emit(OpCodes.Pop);
                                    break;
                                }
                                else
                                    ilGenerator.Emit(OpCodes.Callvirt, methodInfo);
                            }
                            else
                            {
                                ilGenerator.Emit(OpCodes.Stfld, path[i + 1].Type.GetField(parent.Name));
                            }
                        }
                    }
                    // Return
                    ilGenerator.Emit(OpCodes.Ret);

                }


                Type finalType = myTypeBuilder.CreateType();
                //myAssembly.Save("SetterGetterMethods.dll");


                // Build the dictionary
                foreach (MemberInfo memberInfo in members)
                {
                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                    if (propertyInfo != null && propertyInfo.GetIndexParameters().Length > 0)
                        continue;
                    MethodInfo[] finalMethods = finalType.GetMethods();

                    // HACK: If there are two methods with the same name (i.e. new'd in a derived class)
                    MethodInfo setterInfo = finalMethods.FirstOrDefault((m) => m.Name == "XdtkSet_" + memberInfo.Name);
                    MethodInfo getterInfo = finalMethods.FirstOrDefault((m) => m.Name == "XdtkGet_" + memberInfo.Name);

                    if (setterInfo != null)
                        setterGetterDict[memberInfo].Setter = (Setter)Delegate.CreateDelegate(typeof(Setter), setterInfo);
                    if (getterInfo != null)
                        setterGetterDict[memberInfo].Getter = (Getter)Delegate.CreateDelegate(typeof(Getter), getterInfo); ;
                }
                return setterGetterDict;
            }
        }

        /// <summary>
        /// Method that creates a Diccionary of methods to set/get the value of
        /// each member of an object of the specified type. But this object only
        /// reachable by the path specified. For example, to get the Position (Vector3)
        /// from a player, the parameters must be: path="Position.", t=Vector3.
        /// If the object being ispected is the class World which contains a
        /// player then parameters must be: path="Player.Position.", t=Vector3.
        /// </summary>
        /// <param name="path">Path to get to object of type t, including the 
        /// name of the object itself and a point at the end.</param>
        /// <param name="o">Type of the object to get methods for.</param>
        /// <returns></returns>
        internal static Dictionary<MemberInfo, SetterGetterPair> GetSetterGetterDict(InspectorNode node)
        {
            throw new Exception("This method is obsolete, use GetSetterGetterDict2 instead which is a lot faster.");
            Type nodeType = node.Type;
            String expandingObjectTypeName = CreateCSharpTypeString(nodeType);
            String baseObjectTypeName = CreateCSharpTypeString(node.Target.GetType());
            String dictionaryKey = node.Root.Name + node.GetPath();

            // Check if we haven't already generated methods for this node.
            if (setterGetterCache.ContainsKey(dictionaryKey) && false)
            {
                return setterGetterCache[dictionaryKey];
            }
            else
            {
                // Get all instance fields and properties.
                FieldInfo[] fields = nodeType.GetInstanceFields();
                PropertyInfo[] properties = nodeType.GetInstanceProperties();

                // Code and return types for the methods.
                List<String> codes = new List<string>();
                List<Type> types = new List<Type>();

                // List used to get the output of all setters and getters.
                List<MethodInfo> methods = null;

                // We haven't created code to set the fields/properties for this kind of node
                // we have to generated code now and compile it generating methods.
                var setterGetterDict = new Dictionary<MemberInfo, SetterGetterPair>();

                // Defines wether a field or property can be set/get because of a parent being
                // read only.
                bool canWriteParent = false;

                InspectorNode iter = node;
                // Create the path to this node, path will always contain at least
                // two elements because the root is always there.
                List<InspectorNode> path = new List<InspectorNode>(5);
                while (iter != null)
                {
                    path.Add(iter);
                    iter = iter.Parent;
                }

                // Add the reference to the Type of the node, and the target type assemblies.
                ReflectionHelper.AddReferencedAssembly(nodeType.Assembly.Location);
                ReflectionHelper.AddReferencedAssembly(node.Target.GetType().Assembly.Location);

                AddReferencedAssemblies(nodeType);
                AddReferencedAssemblies(node.Target.GetType());

                // Find out which is the last parent that we need to asign.
                // values must be reassigned on the path as long as the parent
                // is a read-only struct.
                // If there's no object or writable valuetype in the chain,
                // we can write to the field/property (if the field/property
                // is writable also.
                int breakIndex = 0;
                if (path.Count > 1)
                {
                    // Assume we can write until we prove we can't
                    canWriteParent = true;
                    for (breakIndex = 0; breakIndex < path.Count; ++breakIndex)
                    {
                        // if parent is a read-only value type, continue.
                        if (path[breakIndex].Type.IsValueType)
                            if (!path[breakIndex].CanWrite)
                            {
                                // Sorry, we can't write.
                                canWriteParent = false;
                                break;
                            }
                            else
                                continue;

                        else
                            break;
                    }

                    //breakIndex--;
                }
                else
                {
                    canWriteParent = true;
                    breakIndex = 0;
                }

                // Now we create a pre/suffix for the SETTER method, it will
                // instanciate everything on the path and assign a value to it,
                // this is because our setters must also handle ValueType properties
                // which members need to be instanciated and reassigned.
                Object target = path[path.Count-1].Target;
                String targetReference = String.Format("(({0})p[0])", CreateCSharpTypeString(target.GetType()));
                StringBuilder prefix = new StringBuilder();
                StringBuilder suffix = new StringBuilder();
                int i;
                for (i = breakIndex; i >= 0; --i)
                {
                    InspectorNode parent = path[i];

                    prefix.Append(String.Format("var v{0} = ", i));
                    prefix.Append(targetReference);
                    prefix.Append(parent.GetPath());
                    prefix.Remove(prefix.Length - 1, 1);    // Remove the trailing dot.
                    prefix.Append(";\n");

                    // TODO: To allow changing the value on objects we have to cut the 
                    // assingment chain in the prefix/suffix
                    //if (parent.CanWrite)//| !parent.Type.IsValueType)
                    //{
                    //    canWrite = true;
                    //}
                    //else
                    //{
                    //    canWrite = false;
                    //}

                    if (i > 0)                              // The last item is particular so we'll generate it later.
                    {
                        InspectorNode child = path[i - 1];
                        suffix.Insert(0, String.Format("v{0}.{1} = v{2};\n", i, child.Name, i - 1));
                    }
                }

                // Create methods for every field
                foreach (FieldInfo info in fields)
                {
                    if (info.IsSpecialName)
                        continue;

                    Type fieldType = info.FieldType;
                    String fieldTypeName = CreateCSharpTypeString(fieldType);
                    //ReflectionHelper.AddUsingNamespace(fieldType.Namespace);

                    // Add references to all types needed, including generic parameters.
                    // We go up the hierarchy, baseType could be null if the type is 
                    // a Interface.
                    AddReferencedAssemblies(fieldType);

                    if (!info.IsInitOnly && canWriteParent)
                        codes.Add(prefix + String.Format("v0.{0} = ({1})p[1];\n", info.Name, fieldTypeName) + suffix);
                    else
                        codes.Add(null);
                        
                    types.Add(null);
                    codes.Add(String.Format("return {0}{1}{2};\n", targetReference, node.GetPath(), info.Name));
                    types.Add(fieldType);
                }

                if (codes.Count != 0)
                {
                    methods = ReflectionHelper.CompileCSharpMethodBatch(codes, types);

                    // Add the fields to the dictionary
                    var methodsEnum = methods.GetEnumerator();
                    var fieldsEnum = fields.GetEnumerator();
                    for (int j = 0; j < methods.Count; j += 2)
                    {
                        methodsEnum.MoveNext();
                        fieldsEnum.MoveNext();
                        if (((FieldInfo)(fieldsEnum.Current)).IsInitOnly) continue;

                        Setter setter = null;
                        Getter getter = null;
                        if (methods[j] != null)
                            setter = (Setter)Delegate.CreateDelegate(typeof(Setter), methods[j]);
                        if (methods[j + 1] != null)
                            getter = (Getter)Delegate.CreateDelegate(typeof(Getter), methods[j + 1]);

                        setterGetterDict.Add((FieldInfo)fieldsEnum.Current, new SetterGetterPair(setter, getter));
                    }
                }

                // Create methods for every property
                codes.Clear();
                types.Clear();
                
                // Create methods for every field
                foreach (PropertyInfo info in properties)
                {
                    if (info.GetIndexParameters().Length > 0)
                        continue;
                    if (info.IsSpecialName) 
                        continue;

                    Type propertyType = info.PropertyType;
                    String propertyTypeName = CreateCSharpTypeString(propertyType);
                    //ReflectionHelper.AddUsingNamespace(propertyType.Namespace);

                    AddReferencedAssemblies(propertyType);

                    // Get parameters for indexed Properties
                    ParameterInfo[] paramInfo = info.GetIndexParameters();
                    MethodInfo setMethodInfo = info.GetSetMethod();
                    if (info.CanWrite && canWriteParent && setMethodInfo != null && setMethodInfo.IsPublic == true)
                    {
                        // We can only handle indexed properties with
                        // a single index parameter.
                        if (paramInfo.Length == 1)
                        {
                            codes.Add(prefix + String.Format("v0[p[1]] = ({1})p[2];\n", info.Name, propertyTypeName) + suffix);
                        }
                        else
                        {
                            codes.Add(prefix + String.Format("v0.{0} = ({1})p[1];\n", info.Name, propertyTypeName) + suffix);
                        }
                    }
                    else
                        codes.Add(null);
                    types.Add(null);

                    if (info.CanRead)
                        // We can only handle indexed properties with
                        // a single index parameter.
                        if (paramInfo.Length == 1)
                        {
                            String pathToVariable = targetReference + node.GetPath();
                            pathToVariable = pathToVariable.Remove(pathToVariable.Length - 1, 1);
                            codes.Add(String.Format("return {0}[p[0]];\n", pathToVariable));
                        }
                        else
                        {
                            codes.Add(String.Format("return {0}{1}{2};\n", targetReference, node.GetPath(), info.Name));
                        }
                    else
                        codes.Add(null);
                    types.Add(propertyType);
                }

                if (codes.Count != 0)
                {
                    methods = ReflectionHelper.CompileCSharpMethodBatch(codes, types);

                    // Add the properties to the dictionary
                    var methodsEnum = methods.GetEnumerator();
                    var propertiesEnum = properties.GetEnumerator();
                    for (int j = 0; j < methods.Count; j += 2)
                    {
                        methodsEnum.MoveNext();
                        propertiesEnum.MoveNext();

                        Setter setter = null;
                        Getter getter = null;
                        if (methods[j] != null)
                            setter = (Setter)Delegate.CreateDelegate(typeof(Setter), methods[j]);
                        if (methods[j + 1] != null)
                            getter = (Getter)Delegate.CreateDelegate(typeof(Getter), methods[j + 1]);

                        setterGetterDict.Add((PropertyInfo)propertiesEnum.Current, new SetterGetterPair(setter, getter));
                    }
                    //setterGetterCache.Add(dictionaryKey, setterGetterDict);
                    
                }
                
                // Clear the ReflectionHelper.
                //ReflectionHelper.ClearUsings();
                //ReflectionHelper.ClearReferencedAsseblies();

                return setterGetterDict;
            }
        }

        /// <summary>
        /// Adds all the needed assemblies to deal with a specified type.
        /// Add references to all types needed, including generic parameters and
        /// implemented interfaces.
        /// We go up the hierarchy, baseType could be null if the type is 
        /// a Interface.
        /// </summary>
        private static void AddReferencedAssemblies(Type t)
        {
            Type baseType = t;
            while (baseType != typeof(Object) && baseType != null)
            {
                ReflectionHelper.AddReferencedAssembly(baseType.Assembly.Location);
                baseType = baseType.BaseType;
            }
            foreach (var genericType in t.GetGenericArguments())
            {
                baseType = genericType;
                while (baseType != typeof(Object) && baseType != null)
                {
                    ReflectionHelper.AddReferencedAssembly(baseType.Assembly.Location);
                    baseType = baseType.BaseType;
                }
            }
            foreach (var interfaceType in t.GetInterfaces())
            {
                baseType = interfaceType;
                while (baseType != null)
                {
                    ReflectionHelper.AddReferencedAssembly(baseType.Assembly.Location);
                    baseType = baseType.BaseType;
                }
            }
            // If this type is nested, the parent types should be added to the
            // reference assembly list.
            if (t.IsNested)
            {
                String parentTypeName = t.AssemblyQualifiedName.Replace("+" + t.Name + ",", ",");
                AddReferencedAssemblies(Type.GetType(parentTypeName));
            }
        }

        private static String CreateCSharpTypeString(Type t)
        {
            StringBuilder result = new StringBuilder(t.FullName);
            if (t.IsGenericType)
            {
                // Remove generic parameters, becuase the notation returned
                // by FullName does not work in C#.
                int usableLength = t.FullName.IndexOf('`');
                result = result.Remove(usableLength, result.Length - usableLength);
                result.Append("<");
                foreach (Type genericParam in t.GetGenericArguments())
                {
                    result.Append(CreateCSharpTypeString(genericParam));
                    result.Append(",");
                }
                result.Remove(result.Length - 1, 1);    // Remove the last comma
                result.Append(">");
            }
            if (t.IsNested)
                result.Replace('+', '.');
            return result.ToString();
        }

    }
}
