﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RRTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddBraces;

        //[Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddBraces)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", @"
", RefactoringId);
        }

        //[Theory, Trait(Traits.Refactoring, RefactoringIdentifiers.AddBraces)]
        //[InlineData("", "")]
        public async Task Test2(string fromData, string toData)
        {
            await VerifyRefactoringAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", fromData, toData, RefactoringId);
        }

        //[Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddBraces)]
        public async Task TestNoRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", RefactoringId);
        }
    }
}
