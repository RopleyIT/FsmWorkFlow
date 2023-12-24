using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bunit;
namespace FsmBlazorTests;

public abstract class BunitTestContext : TestContextWrapper
{
    [TestInitialize]
    public void Setup() => TestContext = new Bunit.TestContext();

    [TestCleanup]
    public void TearDown() => TestContext?.Dispose();
}
