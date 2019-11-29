using System;
using System.Diagnostics.CodeAnalysis;
using CompilingMethods.Enums;

namespace CompilingMethods.Classes.Compiler
{
    public class OperatorType
    {
        public static OpTypes ReturnOperatorKind(ExprBinKind type)
        {
            switch (type) 
            {
                case ExprBinKind t when t == ExprBinKind.Add || t == ExprBinKind.Div || t == ExprBinKind.Mul || t == ExprBinKind.Sub:
                    return OpTypes.Arithmetic;
                case ExprBinKind t when t == ExprBinKind.Equal || t == ExprBinKind.NotEqual:
                    return OpTypes.Equality;
                case ExprBinKind t when t == ExprBinKind.Less || t == ExprBinKind.More || t == ExprBinKind.LessEqual || t == ExprBinKind.MoreEqual:
                    return OpTypes.Comparison;
                case ExprBinKind t when t == ExprBinKind.And || t == ExprBinKind.Or:
                    return OpTypes.Logic;
                default:
                    return OpTypes.Bad;
            }
        }
    }
}