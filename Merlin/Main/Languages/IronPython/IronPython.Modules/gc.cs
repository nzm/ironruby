/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;

using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;

using SpecialName = System.Runtime.CompilerServices.SpecialNameAttribute;

[assembly: PythonModule("gc", typeof(IronPython.Modules.PythonGC))]
namespace IronPython.Modules {
    public static class PythonGC {
        public static PythonType gc = DynamicHelpers.GetPythonTypeFromType(typeof(PythonGC));
        public const int DEBUG_STATS = 1;
        public const int DEBUG_COLLECTABLE = 2;
        public const int DEBUG_UNCOLLECTABLE = 4;
        public const int DEBUG_INSTANCES = 8;
        public const int DEBUG_OBJECTS = 16;
        public const int DEBUG_SAVEALL = 32;
        public const int DEBUG_LEAK = (DEBUG_COLLECTABLE | DEBUG_UNCOLLECTABLE | DEBUG_INSTANCES | DEBUG_OBJECTS | DEBUG_SAVEALL);

        private static readonly object _threadholdKey = new object();

        [SpecialName]
        public static void PerformModuleReload(PythonContext/*!*/ context, IAttributesCollection/*!*/ dict) {            
            context.SetModuleState(_threadholdKey, PythonTuple.MakeTuple(64 * 1024, 256 * 1024, 1024 * 1024));
        }

        public static void enable() {
        }

        public static void disable(CodeContext/*!*/ context) {
            PythonOps.Warn(context, PythonExceptions.RuntimeWarning, "IronPython has no support for disabling the GC");
        }

        public static object isenabled() {
            return ScriptingRuntimeHelpers.True;
        }

        public static int collect(int generation) {
            if (generation > GC.MaxGeneration || generation < 0) throw PythonOps.ValueError("invalid generation {0}", generation);

            long start = GC.GetTotalMemory(false);
#if !SILVERLIGHT // GC.Collect
            GC.Collect(generation);
#else
            GC.Collect();
#endif
            GC.WaitForPendingFinalizers();
            
            return (int)Math.Max(start - GC.GetTotalMemory(false), 0);
        }

        public static int collect() {
            long start = GC.GetTotalMemory(false);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return (int)Math.Max(start - GC.GetTotalMemory(false), 0);
        }

        public static void set_debug(object o) {
            throw PythonOps.NotImplementedError("gc.set_debug isn't implemented");
        }

        public static object get_debug() {
            return null;
        }

        public static object[] get_objects() {
            throw PythonOps.NotImplementedError("gc.get_objects isn't implemented");
        }

        public static void set_threshold(CodeContext/*!*/ context, params object[] args) {
            SetThresholds(context, PythonTuple.MakeTuple(args));
        }

        public static PythonTuple get_threshold(CodeContext/*!*/ context) {
            return GetThresholds(context);
        }

        public static object[] get_referrers(params object[] objs) {
            throw PythonOps.NotImplementedError("gc.get_referrers isn't implemented");
        }

        public static object[] get_referents(params object[] objs) {
            throw PythonOps.NotImplementedError("gc.get_referents isn't implemented");
        }


        public static List garbage {
            get {
                return new List();
            }
        }

        private static PythonTuple GetThresholds(CodeContext/*!*/ context) {
            return (PythonTuple)PythonContext.GetContext(context).GetModuleState(_threadholdKey);
        }

        private static void SetThresholds(CodeContext/*!*/ context, PythonTuple thresholds) {
            PythonContext.GetContext(context).SetModuleState(_threadholdKey, thresholds);
        }
    }
}
