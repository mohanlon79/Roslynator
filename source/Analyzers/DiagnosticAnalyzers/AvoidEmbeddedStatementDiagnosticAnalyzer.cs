﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidEmbeddedStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidEmbeddedStatement); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeNode(f),
                SyntaxKind.IfStatement,
                SyntaxKind.ElseClause,
                SyntaxKind.ForEachStatement,
                SyntaxKind.ForStatement,
                SyntaxKind.UsingStatement,
                SyntaxKind.WhileStatement,
                SyntaxKind.DoStatement,
                SyntaxKind.LockStatement,
                SyntaxKind.FixedStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            StatementSyntax statement = EmbeddedStatementHelper.GetEmbeddedStatement(context.Node, ifInsideElse: false, usingInsideUsing: false);

            if (statement != null)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AvoidEmbeddedStatement,
                    statement,
                    context.Node.GetTitle());
            }
        }
    }
}
