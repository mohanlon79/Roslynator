﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MarkOperatorAsPublicAndStaticCodeFixProvider))]
    [Shared]
    public class MarkOperatorAsPublicAndStaticCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.UserDefinedOperatorMustBeDeclaredStaticAndPublic); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MarkOperatorAsPublicAndStatic))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.UserDefinedOperatorMustBeDeclaredStaticAndPublic:
                        {
                            ModifierListInfo info = SyntaxInfo.ModifierListInfo(memberDeclaration);

                            string title = "Add ";

                            if (info.ExplicitAccessibility == Accessibility.Public)
                            {
                                title += "modifier 'static'";
                            }
                            else if (info.IsStatic)
                            {
                                title += "modifier 'public'";
                            }
                            else
                            {
                                title += "modifiers 'public static'";
                            }

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                cancellationToken =>
                                {
                                    SyntaxTokenList modifiers = info.Modifiers;

                                    SyntaxTokenList newModifiers = modifiers;

                                    if (!modifiers.Contains(SyntaxKind.PublicKeyword))
                                        newModifiers = Modifier.Insert(newModifiers, SyntaxKind.PublicKeyword);

                                    if (!modifiers.Contains(SyntaxKind.StaticKeyword))
                                        newModifiers = Modifier.Insert(newModifiers, SyntaxKind.StaticKeyword);

                                    return context.Document.ReplaceModifiersAsync(info, newModifiers, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
