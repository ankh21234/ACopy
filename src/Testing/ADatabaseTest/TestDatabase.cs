﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACopyTestHelper;
using ADatabase;
using ADatabase.Interfaces;

namespace ADatabaseTest
{
    [TestClass]
    public class TestDatabase
    {
        private IDbContext _msContext;
        private IDbContext _oraContext;
        private ICommands _commands;
        private IDbSchema _dbSchema;
        private string _testTable;
        private readonly ConnectionStrings _connectionStrings = new ConnectionStrings();

        [TestInitialize]
        public void Setup()
        {
            _msContext = DbContextFactory.CreateSqlServerContext(_connectionStrings.GetSqlServer());
            _oraContext = DbContextFactory.CreateOracleContext(_connectionStrings.GetOracle());
            _testTable = "htestdatabase";
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbSchema.DropTable(_testTable);
        }

        private void Initialize(IDbContext dbContext)
        {
            _dbSchema = dbContext.PowerPlant.CreateDbSchema();
            _commands = dbContext.PowerPlant.CreateCommands();

            _dbSchema.DropTable(_testTable);
        }

        private void TestExecuteNonQuery(IDbContext dbContext)
        {
            Initialize(dbContext);
            TestTableCreator.CreateTableSomeColumnsAndOneRow(dbContext, _testTable);
            _commands.ExecuteNonQuery($"update {_testTable} set val = 'AH'").Should().Be(1);
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMS_ExecuteNonQuery()
        {
            TestExecuteNonQuery(_msContext);
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOra_ExecuteNonQuery()
        {
            TestExecuteNonQuery(_oraContext);
        }

        private void TestIsTable_When_TableExists(IDbContext dbContext)
        {
            Initialize(dbContext);
            TestTableCreator.CreateTableSomeColumnsAndOneRow(dbContext, _testTable);
            _dbSchema.IsTable(_testTable).Should().BeTrue($"because {_testTable} should exist");
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMS_IsTable_When_TableExists()
        {
            TestIsTable_When_TableExists(_msContext);
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOra_IsTable_When_TableDoesNotExist()
        {
            TestIsTable_When_TableExists(_oraContext);
        }

        private void TestCreateTableWithVarcharColumn(IDbContext dbContext)
        {
            Initialize(dbContext);
            TableDefinition tableDefinition = TestTableCreator.CreateTableSomeColumnsAndOneRow(dbContext, _testTable);

            _dbSchema.IsTable(tableDefinition.Name).Should().BeTrue();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMSCreateTableWithVarchar()
        {
            TestCreateTableWithVarcharColumn(_msContext);
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOra_CreateTableWithVarchar()
        {
            TestCreateTableWithVarcharColumn(_oraContext);
        }

        private void TestGetTableNames_When_SingleTable(IDbContext dbContext)
        {
            Initialize(dbContext);
            TestTableCreator.CreateTableSomeColumnsAndOneRow(dbContext, _testTable);
            _dbSchema.GetTableNames(_testTable)[0].Name.Should().BeEquivalentTo(_testTable);
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMS_GetTableNames_When_SingleTable()
        {
            TestGetTableNames_When_SingleTable(_msContext);
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOra_GetTableNames_When_SingleTable()
        {
            TestGetTableNames_When_SingleTable(_oraContext);
        }

        private static void CompareToAcrparordAcrrepord(List<ITableShortInfo> actual, string shouldFind1, string shouldFind2)
        {
            (from t in actual
             where string.Compare(t.Name, shouldFind1, StringComparison.OrdinalIgnoreCase) == 0
             select t).Count().Should().Be(1, $"because {shouldFind1} exists");
            (from t in actual
             where string.Compare(t.Name, shouldFind2, StringComparison.OrdinalIgnoreCase) == 0
             select t).Count().Should().Be(1, $"because {shouldFind2} exists");
        }

        private void TestGetTableNames_When_Wildcard(IDbContext dbContext, string searchString, string shouldFind1, string shouldFind2)
        {
            Initialize(dbContext);
            const string table1 = "htest0table";
            const string table2 = "htestsometable";
            try
            {
                TestTableCreator.CreateTableSomeColumnsAndOneRow(dbContext, table1);
                TestTableCreator.CreateTableSomeColumnsAndOneRow(dbContext, table2);
                TestTableCreator.CreateTableSomeColumnsAndOneRow(dbContext, shouldFind1);
                TestTableCreator.CreateTableSomeColumnsAndOneRow(dbContext, shouldFind2);
                List<ITableShortInfo> actual = _dbSchema.GetTableNames(searchString);
                CompareToAcrparordAcrrepord(actual, shouldFind1, shouldFind2);

            }
            finally
            {
                _dbSchema.DropTable(table1);
                _dbSchema.DropTable(table2);
                _dbSchema.DropTable(shouldFind1);
                _dbSchema.DropTable(shouldFind2);
            }
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestMS_GetTableNames_When_WildcardMany()
        {
            TestGetTableNames_When_Wildcard(_msContext, "htest%table", "htest1table", "htestarvetable");
        }

        [TestMethod, TestCategory("Oracle")]
        public void TestOra_GetTableNames_When_WildcardMany()
        {
            TestGetTableNames_When_Wildcard(_oraContext, "htest%table", "htest1table", "htestarvetable");
        }

        [TestMethod, TestCategory("SqlServer")]
        public void Test_MS_GetTableNames_When_WildcardOne()
        {
            TestGetTableNames_When_Wildcard(_msContext, "htest__table", "htest11table", "htest98table");
        }

        [TestMethod, TestCategory("Oracle")]
        public void Test_Ora_GetTableNames_When_WildcardOne()
        {
            TestGetTableNames_When_Wildcard(_oraContext, "htest__table", "htest11table", "htest98table");
        }

        private void TestIsIndex()
        {
            _commands.ExecuteNonQuery(string.Format("create table {0} (id int)", _testTable));
            string indexName = "i_" + _testTable;
            _dbSchema.IsIndex(indexName, _testTable).Should().BeFalse("Since index not created yet");

            _commands.ExecuteNonQuery(string.Format("create unique index {0} on {1}(id)", indexName, _testTable));

            _dbSchema.IsIndex(indexName, _testTable).Should().BeTrue("since index should now be created");
        }

        [TestMethod, TestCategory("Oracle")]
        public void Test_Ora_IsIndex()
        {
            Initialize(_oraContext);
            TestIsIndex();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void Test_MS_IsIndex()
        {
            Initialize(_msContext);
            TestIsIndex();
        }

        [TestMethod, TestCategory("Oracle")]
        public void Test_Ora_GetIndexDefinitions()
        {
            Initialize(_oraContext);
            Test_GetIndexDefinitions();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void Test_MS_GetIndexDefinitions()
        {
            Initialize(_msContext);
            Test_GetIndexDefinitions();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void Test_MS_GetClusteredIndexDefinition()
        {
            Initialize(_msContext);
            _commands.ExecuteNonQuery(string.Format("create table {0} (id int, id2 int, id3 int)", _testTable));
            _commands.ExecuteNonQuery(string.Format("create unique clustered index {0} on {1}(id)", "i1_" + _testTable, _testTable));
            CheckIndexIsClustered();
        }

        private void CheckIndexIsClustered()
        {
            List<IIndexDefinition> indexes = _dbSchema.GetIndexDefinitions(_testTable);
            indexes.Should().HaveCount(1, "because clustered index were created");
            indexes[0].IndexName.Should().BeEquivalentTo("i1_" + _testTable);
            indexes[0].IsClustered.Should().BeTrue("because table has clustered index");
        }

        [TestMethod, TestCategory("SqlServer")]
        public void Test_MS_CreateClusteredIndex()
        {
            Initialize(_msContext);
            _commands.ExecuteNonQuery(string.Format("create table {0} (id int, id2 int, id3 int)", _testTable));
            IIndexDefinition index = _msContext.PowerPlant.CreateIndexDefinition("i1_" + _testTable, _testTable, "", true, 0, true);
            index.Columns = new List<IIndexColumn> {IndexColumnFactory.CreateInstance("id")};
            _dbSchema.CreateIndex(index);
            CheckIndexIsClustered();
        }

        private void Test_GetIndexDefinitions()
        {
            _commands.ExecuteNonQuery(string.Format("create table {0} (id int, id2 int, id3 int)", _testTable));
            _commands.ExecuteNonQuery(string.Format("create unique index {0} on {1}(id)", "i1_" + _testTable, _testTable));
            _commands.ExecuteNonQuery(string.Format("create index {0} on {1}(id2, id3)", "i2_" + _testTable, _testTable));

            CheckIndexes();
        }

        private void CheckIndexes()
        {
            List<IIndexDefinition> indexes = _dbSchema.GetIndexDefinitions(_testTable);
            indexes.Should().HaveCount(2, "because 2 indexes were created");
            indexes[0].IndexName.Should().BeEquivalentTo("i1_" + _testTable);
            indexes[0].IsUnique.Should().BeTrue("because the first index is unique");
            indexes[1].Columns.Should().HaveCount(2, "because the second index has two columns");
            indexes[1].IsUnique.Should().BeFalse("because the second index is not unique");
        }

        private void Test_CreateIndex()
        {
            _commands.ExecuteNonQuery(string.Format("create table {0} (id int, id2 int, id3 int)", _testTable));

            List<IIndexDefinition> indexes = new List<IIndexDefinition>
            {
                _msContext.PowerPlant.CreateIndexDefinition("i1_" + _testTable, _testTable, "", true)
            };
            indexes[0].Columns = new List<IIndexColumn> {IndexColumnFactory.CreateInstance("id")};
            indexes.Add(_msContext.PowerPlant.CreateIndexDefinition("i2_" + _testTable, _testTable, "", false));
            indexes[1].Columns = new List<IIndexColumn>
            {
                IndexColumnFactory.CreateInstance("id2"),
                IndexColumnFactory.CreateInstance("id3")
            };
            _dbSchema.CreateIndexes(indexes);

            CheckIndexes();
        }

        [TestMethod, TestCategory("Oracle")]
        public void Test_Ora_CreateIndex()
        {
            Initialize(_oraContext);
            Test_CreateIndex();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void Test_MS_CreateIndex()
        {
            Initialize(_msContext);
            Test_CreateIndex();
        }

        private void Test_IsView()
        {
            const string testView = "vitestview";
            _dbSchema.DropView(testView);
            _commands.ExecuteNonQuery(string.Format("create view {0} as select 'a' as col1 from {1}", testView, _testTable));
            bool viewExists = _dbSchema.IsView(testView);
            _dbSchema.DropView(testView);
            viewExists.Should().BeTrue("because view was created");
        }

        [TestMethod, TestCategory("Oracle")]
        public void Test_Ora_IsView()
        {
            Initialize(_oraContext);
            TestTableCreator.CreateTableSomeColumnsAndOneRow(_oraContext, _testTable);
            Test_IsView();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void Test_MS_IsView()
        {
            Initialize(_msContext);
            TestTableCreator.CreateTableSomeColumnsAndOneRow(_msContext, _testTable);
            Test_IsView();
        }

        [TestMethod, TestCategory("SqlServer")]
        public void TestExecuteReader_When_ErrorInStatement()
        {
            Initialize(_msContext);
            IDataCursor cursor = _msContext.PowerPlant.CreateDataCursor();
            string errorMsg = "";
            try
            {
	            cursor.ExecuteReader("select * from some_table_that_doesnt_exist");
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            errorMsg.Should().Contain("some_table_that_doesnt_exist", "because it's an illegal table");
        }

    }
}
