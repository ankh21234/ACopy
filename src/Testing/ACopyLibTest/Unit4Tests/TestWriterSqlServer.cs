﻿using ADatabase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACopyLibTest.Unit4Tests
{
    [TestClass]
    public class TestWriterSqlServer: TestWriter
    {
        [TestInitialize]
        public override void Setup()
        {
            DbContext = DbContextFactory.CreateSqlServerContext(ConnectionStrings.GetSqlServer());
            DbSchema = DbContext.PowerPlant.CreateDbSchema();
            Commands = DbContext.PowerPlant.CreateCommands();

            DbSchema.DropTable(TestTable);
            DeleteFiles();

            ConversionFileForRead = "Resources/Unit4MssReaderConversions.xml";
            ConversionFileForWrite = "Resources/Unit4MssWriterConversions.xml";

            base.Setup();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSWriter_When_SimpleTable_Then_SchemaFileCreated()
        {
            TestWriter_When_SimpleTable_Then_SchemaFileCreated();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSWriter_When_SimpleTable_Then_DataFileCreated()
        {
            TestWriter_When_SimpleTable_Then_DataFileCreated();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSWriter_When_BlobTable()
        {
            TestWriter_When_BlobTable("convert(varbinary, 'A virtually very long line')");
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSWriter_When_AllTypes()
        {
            TestWriter_When_AllTypes();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSWriter_When_NullValue()
        {
            TestWriter_When_NullValue();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSWriter_When_StringContainsQuote()
        {
            TestWriter_When_StringContainsQuote();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSWriter_When_StringContainsNewLine()
        {
            TestWriter_When_StringContainsNewLine();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSWriter_When_UseCompression()
        {
            TestWriter_When_UseCompression();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSWriter_When_BlobTableAndCompressedFlag()
        {
            TestWriter_When_BlobTableAndCompressedFlag("convert(varbinary, 'A virtually very long line')");
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TesMStWriter_When_CharColAndTrailingSpaces_Then_NoTrailingSpacesInDataFile()
        {
            TestWriter_When_CharColAndTrailingSpaces_Then_NoTrailingSpacesInDataFile();
        }
    }
}
