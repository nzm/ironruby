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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Dynamic;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using AstUtils = Microsoft.Scripting.Ast.Utils;

namespace Microsoft.Scripting.Hosting {
    /// <summary>
    /// A ScriptScope is a unit of execution for code.  It consists of a global Scope which
    /// all code executes in.  A ScriptScope can have an arbitrary initializer and arbitrary
    /// reloader. 
    /// 
    /// ScriptScope is not thread safe. Host should either lock when multiple threads could 
    /// access the same module or should make a copy for each thread.
    ///
    /// Hosting API counterpart for <see cref="Scope"/>.
    /// </summary>
#if SILVERLIGHT
    public sealed class ScriptScope : IDynamicObject {
#else
    [DebuggerTypeProxy(typeof(ScriptScope.DebugView))]
    public sealed class ScriptScope : MarshalByRefObject, IDynamicObject {
#endif
        private readonly Scope _scope;
        private readonly ScriptEngine _engine;

        internal ScriptScope(ScriptEngine engine, Scope scope) {
            Assert.NotNull(engine);
            Assert.NotNull(scope);

            _scope = scope;
            _engine = engine;
        }

        internal Scope Scope {
            get { return _scope; }
        }

        /// <summary>
        /// Gets an engine for the language associated with this scope.
        /// Returns invariant engine if the scope is language agnostic.
        /// </summary>
        public ScriptEngine Engine {
            get {
                // InvariantContext should not have an engine
                // TODO: If _engine itself could be set to null, we wouldn't
                // need this check
                if (_engine.LanguageContext is InvariantContext) {
                    return null;
                }
                return _engine;
            }
        }
        
        /// <summary>
        /// Gets a value stored in the scope under the given name.
        /// </summary>
        /// <exception cref="MissingMemberException">The specified name is not defined in the scope.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is a <c>null</c> reference.</exception>
        public object GetVariable(string name) {
            return _scope.LookupName(_engine.LanguageContext, SymbolTable.StringToId(name));
        }

        /// <summary>
        /// Gets a value stored in the scope under the given name.
        /// Converts the result to the specified type using the conversion that the language associated with the scope defines.
        /// If no language is associated with the scope, the default CLR conversion is attempted.
        /// </summary>
        /// <exception cref="MissingMemberException">The specified name is not defined in the scope.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is a <c>null</c> reference.</exception>
        public T GetVariable<T>(string name) {
            return _engine.Operations.ConvertTo<T>(_engine.GetVariable(this, name));
        }

        /// <summary>
        /// Tries to get a value stored in the scope under the given name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is a <c>null</c> reference.</exception>
        public bool TryGetVariable(string name, out object value) {
            return _scope.TryGetName(_engine.LanguageContext, SymbolTable.StringToId(name), out value);
        }

        /// <summary>
        /// Tries to get a value stored in the scope under the given name.
        /// Converts the result to the specified type using the conversion that the language associated with the scope defines.
        /// If no language is associated with the scope, the default CLR conversion is attempted.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is a <c>null</c> reference.</exception>
        public bool TryGetVariable<T>(string name, out T value) {
            object result;
            if (_scope.TryGetName(_engine.LanguageContext, SymbolTable.StringToId(name), out result)) {
                value = _engine.Operations.ConvertTo<T>(result);
                return true;
            }
            value = default(T);
            return false;
        }

        /// <summary>
        /// Sets the name to the specified value.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is a <c>null</c> reference.</exception>
        public void SetVariable(string name, object value) {
            _scope.SetName(SymbolTable.StringToId(name), value);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets a handle for a value stored in the scope under the given name.
        /// </summary>
        /// <exception cref="MissingMemberException">The specified name is not defined in the scope.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is a <c>null</c> reference.</exception>
        public ObjectHandle GetVariableHandle(string name) {
            return new ObjectHandle(GetVariable(name));
        }

        /// <summary>
        /// Tries to get a handle for a value stored in the scope under the given name.
        /// Returns <c>true</c> if there is such name, <c>false</c> otherwise. 
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is a <c>null</c> reference.</exception>
        public bool TryGetVariableHandle(string name, out ObjectHandle handle) {
            object value;
            if (TryGetVariable(name, out value)) {
                handle = new ObjectHandle(value);
                return true;
            } else {
                handle = null;
                return false;
            }
        }

        /// <summary>
        /// Sets the name to the specified value.
        /// </summary>
        /// <exception cref="SerializationException">
        /// The value held by the handle isn't from the scope's app-domain and isn't serializable or MarshalByRefObject.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="handle"/> is a <c>null</c> reference.</exception>
        public void SetVariable(string name, ObjectHandle handle) {
            ContractUtils.RequiresNotNull(handle, "handle");
            SetVariable(name, handle.Unwrap());
        }
#endif

        /// <summary>
        /// Determines if this context or any outer scope contains the defined name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is a <c>null</c> reference.</exception>
        public bool ContainsVariable(string name) {
            return _scope.ContainsName(_engine.LanguageContext, SymbolTable.StringToId(name));
        }

        /// <summary>
        /// Removes the variable of the given name from this scope.
        /// </summary> 
        /// <returns><c>true</c> if the value existed in the scope before it has been removed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is a <c>null</c> reference.</exception>
        public bool RemoveVariable(string name) {
            return _scope.TryRemoveName(_engine.LanguageContext, SymbolTable.StringToId(name));
        }

        /// <summary>
        /// Gets a list of variable names stored in the scope.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<string> GetVariableNames() {
            // Remoting: we eagerly enumerate all variables to avoid cross domain calls for each item.

            var result = new List<string>();
            foreach (var entry in _scope.Items) {
                result.Add(SymbolTable.IdToString(entry.Key));
            }
            result.TrimExcess();
            return result;
        }

        /// <summary>
        /// Gets an array of variable names and their values stored in the scope.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<KeyValuePair<string, object>> GetItems() {
            // Remoting: we eagerly enumerate all variables to avoid cross domain calls for each item.

            var result = new List<KeyValuePair<string, object>>();
            foreach (var entry in _scope.Items) {
                result.Add(new KeyValuePair<string, object>(SymbolTable.IdToString(entry.Key), entry.Value));
            }
            result.TrimExcess();
            return result;
        }

        #region DebugView
#if !SILVERLIGHT
        internal sealed class DebugView {
            private readonly ScriptScope _scope;

            public DebugView(ScriptScope scope) {
                Assert.NotNull(scope);
                _scope = scope;
            }

            public ScriptEngine Language {
                get { return _scope._engine; }
            }

            public System.Collections.Hashtable Variables {
                get {
                    System.Collections.Hashtable result = new System.Collections.Hashtable();
                    foreach (KeyValuePair<string, object> variable in _scope.GetItems()) {
                        result[variable.Key] = variable.Value;
                    }
                    return result;
                }
            }
        }
#endif
        #endregion

        #region IDynamicObject implementation

        MetaObject IDynamicObject.GetMetaObject(Expression parameter) {
            return new Meta(parameter, this);
        }

        private sealed class Meta : MetaObject {
            internal Meta(Expression parameter, ScriptScope scope)
                : base(parameter, Restrictions.Empty, scope) {
            }

            // TODO: support for IgnoreCase in underlying ScriptScope APIs
            public override MetaObject BindGetMember(GetMemberBinder action) {
                var result = Expression.Variable(typeof(object), "result");
                var fallback = action.FallbackGetMember(this);

                return new MetaObject(
                    Expression.Block(
                        new ParameterExpression[] { result },
                        Expression.Condition(
                            Expression.Call(
                                AstUtils.Convert(Expression, typeof(ScriptScope)),
                                typeof(ScriptScope).GetMethod("TryGetVariable", new[] { typeof(string), typeof(object).MakeByRefType() }),
                                Expression.Constant(action.Name),
                                result
                            ),
                            result,
                            AstUtils.Convert(fallback.Expression, typeof(object))
                        )
                    ),
                    Restrictions.GetTypeRestriction(Expression, typeof(ScriptScope)).Merge(fallback.Restrictions)
                );
            }

            // TODO: support for IgnoreCase in underlying ScriptScope APIs
            public override MetaObject BindSetMember(SetMemberBinder action, MetaObject value) {
                return new MetaObject(
                    Expression.Call(
                        AstUtils.Convert(Expression, typeof(ScriptScope)),
                        typeof(ScriptScope).GetMethod("SetVariable", new[] { typeof(string), typeof(object) }),
                        Expression.Constant(action.Name),
                        AstUtils.Convert(value.Expression, typeof(object))
                    ),
                    Restrictions.Merge(value.Restrictions).Merge(Restrictions.GetTypeRestriction(Expression, typeof(ScriptScope)))
                );
            }

            // TODO: support for IgnoreCase in underlying ScriptScope APIs
            public override MetaObject BindDeleteMember(DeleteMemberBinder action) {
                var fallback = action.FallbackDeleteMember(this);
                return new MetaObject(
                    Expression.Condition(
                        Expression.Call(
                            AstUtils.Convert(Expression, typeof(ScriptScope)),
                            typeof(ScriptScope).GetMethod("RemoveVariable"),
                            Expression.Constant(action.Name)
                        ),
                        Expression.Empty(),
                        Expression.Convert(fallback.Expression, typeof(void))
                    ),
                    Restrictions.Merge(Restrictions.GetTypeRestriction(Expression, typeof(ScriptScope))).Merge(fallback.Restrictions)
                );
            }

            // TODO: support for IgnoreCase in underlying ScriptScope APIs
            public override MetaObject BindInvokeMember(InvokeMemberBinder action, MetaObject[] args) {
                var fallback = action.FallbackInvokeMember(this, args);
                var result = Expression.Variable(typeof(object), "result");

                var fallbackInvoke = action.FallbackInvoke(new MetaObject(result, Restrictions.Empty), args, null);

                return new MetaObject(
                    Expression.Block(
                        new ParameterExpression[] { result },
                        Expression.Condition(
                            Expression.Call(
                                AstUtils.Convert(Expression, typeof(ScriptScope)),
                                typeof(ScriptScope).GetMethod("TryGetVariable", new[] { typeof(string), typeof(object).MakeByRefType() }),
                                Expression.Constant(action.Name),
                                result
                            ),
                            AstUtils.Convert(fallbackInvoke.Expression, typeof(object)),
                            AstUtils.Convert(fallback.Expression, typeof(object))
                        )
                    ),
                    Restrictions.Combine(args).Merge(Restrictions.GetTypeRestriction(Expression, typeof(ScriptScope))).Merge(fallback.Restrictions)
                );
            }
        }

        #endregion
    }
}
