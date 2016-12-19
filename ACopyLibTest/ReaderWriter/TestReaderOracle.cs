﻿using System;
using ADatabase;
using ADatabase.Oracle.Columns;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACopyLibTest
{
    [TestClass]
    public class TestReaderOracle: TestReader
    {
        [TestInitialize]
        public override void Setup()
        {
            DbContext = DbContextFactory.CreateOracleContext(ConnectionHolderForTesting.GetOracleConnection());
            DbSchema = DbContext.PowerPlant.CreateDbSchema();
            Commands = DbContext.PowerPlant.CreateCommands();

            SchemaFile = TestTable + ".aschema";
            DataFile = TestTable + ".adata";
            DbSchema.DropTable(TestTable);
            DeleteFiles();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOraReader_When_SimpleTable()
        {
            TestReader_When_SimpleTable();
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOraReader_When_CompressedTable()
        {
            TestReader_When_CompressedTable();
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOraReader_When_RawColumn()
        {
            TestReader_When_RawColumn();
            var val = Commands.ExecuteScalar(string.Format("select utl_raw.cast_to_varchar2(dbms_lob.substr(raw_col)) as tmp from {0}", TestTable));
            val.Should().Be("A long blob");
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOraReader_When_RawColumn_And_Compressed()
        {
            TestReader_When_RawColumn_And_Compressed();
            var val = Commands.ExecuteScalar(string.Format("select utl_raw.cast_to_varchar2(dbms_lob.substr(raw_col)) as tmp from {0}", TestTable));
            val.Should().Be("A long blob");
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOraReader_When_Guid()
        {
            TestReader_When_Guid();
            var val = Commands.ExecuteScalar(string.Format("select test_col from {0}", TestTable));
            Guid guid = OracleGuidColumn.ConvertToGuid((byte[])val);
            guid.ToString().Should().Be(TestGuid);
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOraReader_When_AllTypes()
        {
            TestReader_When_AllTypes();
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOraReader_When_AllTypes_And_Compressed()
        {
            TestReader_When_AllTypes_And_Compressed();
        }
    }
}