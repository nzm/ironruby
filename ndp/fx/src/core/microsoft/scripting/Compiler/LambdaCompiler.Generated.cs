﻿/* ****************************************************************************
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

using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions.Compiler {
    partial class LambdaCompiler {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void EmitExpression(Expression node, bool emitStart) {
            Debug.Assert(node != null);

            ExpressionStart startEmitted = emitStart ? EmitExpressionStart(node) : ExpressionStart.None;

            switch (node.NodeType) {
                #region Generated Expression Compiler

                // *** BEGIN GENERATED CODE ***
                // generated by function: gen_compiler from: generate_tree.py

                case ExpressionType.Add:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.AddChecked:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.And:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.AndAlso:
                    EmitAndAlsoBinaryExpression(node);
                    break;
                case ExpressionType.ArrayLength:
                    EmitUnaryExpression(node);
                    break;
                case ExpressionType.ArrayIndex:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.Call:
                    EmitMethodCallExpression(node);
                    break;
                case ExpressionType.Coalesce:
                    EmitCoalesceBinaryExpression(node);
                    break;
                case ExpressionType.Conditional:
                    EmitConditionalExpression(node);
                    break;
                case ExpressionType.Constant:
                    EmitConstantExpression(node);
                    break;
                case ExpressionType.Convert:
                    EmitConvertUnaryExpression(node);
                    break;
                case ExpressionType.ConvertChecked:
                    EmitConvertUnaryExpression(node);
                    break;
                case ExpressionType.Divide:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.Equal:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.ExclusiveOr:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.GreaterThan:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.Invoke:
                    EmitInvocationExpression(node);
                    break;
                case ExpressionType.Lambda:
                    EmitLambdaExpression(node);
                    break;
                case ExpressionType.LeftShift:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.LessThan:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.LessThanOrEqual:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.ListInit:
                    EmitListInitExpression(node);
                    break;
                case ExpressionType.MemberAccess:
                    EmitMemberExpression(node);
                    break;
                case ExpressionType.MemberInit:
                    EmitMemberInitExpression(node);
                    break;
                case ExpressionType.Modulo:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.Multiply:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.MultiplyChecked:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.Negate:
                    EmitUnaryExpression(node);
                    break;
                case ExpressionType.UnaryPlus:
                    EmitUnaryExpression(node);
                    break;
                case ExpressionType.NegateChecked:
                    EmitUnaryExpression(node);
                    break;
                case ExpressionType.New:
                    EmitNewExpression(node);
                    break;
                case ExpressionType.NewArrayInit:
                    EmitNewArrayExpression(node);
                    break;
                case ExpressionType.NewArrayBounds:
                    EmitNewArrayExpression(node);
                    break;
                case ExpressionType.Not:
                    EmitUnaryExpression(node);
                    break;
                case ExpressionType.NotEqual:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.Or:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.OrElse:
                    EmitOrElseBinaryExpression(node);
                    break;
                case ExpressionType.Parameter:
                    EmitParameterExpression(node);
                    break;
                case ExpressionType.Power:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.Quote:
                    EmitQuoteUnaryExpression(node);
                    break;
                case ExpressionType.RightShift:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.Subtract:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.SubtractChecked:
                    EmitBinaryExpression(node);
                    break;
                case ExpressionType.TypeAs:
                    EmitUnaryExpression(node);
                    break;
                case ExpressionType.TypeIs:
                    EmitTypeBinaryExpression(node);
                    break;
                case ExpressionType.Assign:
                    EmitAssignBinaryExpression(node);
                    break;
                case ExpressionType.Block:
                    EmitBlockExpression(node);
                    break;
                case ExpressionType.DebugInfo:
                    EmitDebugInfoExpression(node);
                    break;
                case ExpressionType.Decrement:
                    EmitUnaryExpression(node);
                    break;
                case ExpressionType.Dynamic:
                    EmitDynamicExpression(node);
                    break;
                case ExpressionType.Default:
                    EmitDefaultExpression(node);
                    break;
                case ExpressionType.Extension:
                    EmitExtensionExpression(node);
                    break;
                case ExpressionType.Goto:
                    EmitGotoExpression(node);
                    break;
                case ExpressionType.Increment:
                    EmitUnaryExpression(node);
                    break;
                case ExpressionType.Index:
                    EmitIndexExpression(node);
                    break;
                case ExpressionType.Label:
                    EmitLabelExpression(node);
                    break;
                case ExpressionType.RuntimeVariables:
                    EmitRuntimeVariablesExpression(node);
                    break;
                case ExpressionType.Loop:
                    EmitLoopExpression(node);
                    break;
                case ExpressionType.Switch:
                    EmitSwitchExpression(node);
                    break;
                case ExpressionType.Throw:
                    EmitThrowUnaryExpression(node);
                    break;
                case ExpressionType.Try:
                    EmitTryExpression(node);
                    break;
                case ExpressionType.Unbox:
                    EmitUnboxUnaryExpression(node);
                    break;

                // *** END GENERATED CODE ***

                #endregion

                default:
                    throw Assert.Unreachable;
            }

            if (emitStart) {
                EmitExpressionEnd(startEmitted);
            }
        }

        private static bool IsChecked(ExpressionType op) {
            switch (op) {
                #region Generated Checked Operations

                // *** BEGIN GENERATED CODE ***
                // generated by function: gen_checked_ops from: generate_tree.py

                case ExpressionType.AddChecked:
                case ExpressionType.ConvertChecked:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NegateChecked:
                case ExpressionType.SubtractChecked:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:

                // *** END GENERATED CODE ***

                #endregion
                    return true;
            }
            return false;
        }

    }
}
