﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Refactorings.UseConditionalAccessAnalysis;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseConditionalAccessDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseConditionalAccess); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol expressionType = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Linq_Expressions_Expression_1);

                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeLogicalAndExpression(nodeContext, expressionType), SyntaxKind.LogicalAndExpression);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeIfStatement(nodeContext, expressionType), SyntaxKind.IfStatement);
            });
        }
    }
}
