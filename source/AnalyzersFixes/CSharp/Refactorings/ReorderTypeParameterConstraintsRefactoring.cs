﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analyzers;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReorderTypeParameterConstraintsRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            GenericInfo genericInfo = SyntaxInfo.GenericInfo(node);

            SyntaxList<TypeParameterConstraintClauseSyntax> newConstraintClauses = SortConstraints(genericInfo.TypeParameters, genericInfo.ConstraintClauses);

            GenericInfo newInfo = genericInfo.WithConstraintClauses(newConstraintClauses);

            return document.ReplaceNodeAsync(genericInfo.Declaration, newInfo.Declaration, cancellationToken);
        }

        private static SyntaxList<TypeParameterConstraintClauseSyntax> SortConstraints(
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            int lastIndex = -1;

            for (int i = 0; i < typeParameters.Count; i++)
            {
                string name = typeParameters[i].Identifier.ValueText;

                int index = ReorderTypeParameterConstraintsAnalyzer.IndexOf(constraintClauses, name);

                if (index != -1)
                {
                    if (index != lastIndex + 1)
                        constraintClauses = Swap(constraintClauses, index, lastIndex + 1);

                    lastIndex++;
                }
            }

            return constraintClauses;
        }

        private static SyntaxList<TNode> Swap<TNode>(
            SyntaxList<TNode> list,
            int index1,
            int index2) where TNode : SyntaxNode
        {
            TNode first = list[index1];
            TNode second = list[index2];

            SyntaxTriviaList firstLeading = first.GetLeadingTrivia();
            SyntaxTriviaList secondLeading = second.GetLeadingTrivia();

            if (firstLeading.IsEmptyOrWhitespace()
                && secondLeading.IsEmptyOrWhitespace())
            {
                first = first.WithLeadingTrivia(secondLeading);
                second = second.WithLeadingTrivia(firstLeading);
            }

            SyntaxTriviaList firstTrailing = first.GetTrailingTrivia();
            SyntaxTriviaList secondTrailing = second.GetTrailingTrivia();

            if (firstTrailing.IsEmptyOrWhitespace()
                && secondTrailing.IsEmptyOrWhitespace())
            {
                first = first.WithTrailingTrivia(secondTrailing);
                second = second.WithTrailingTrivia(firstTrailing);
            }

            return list
                .ReplaceAt(index1, second)
                .ReplaceAt(index2, first);
        }
    }
}
