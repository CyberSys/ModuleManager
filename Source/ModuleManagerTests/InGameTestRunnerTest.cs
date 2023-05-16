﻿/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT
		© 2013-2018 Sarbian; Blowfish
		© 2013 ialdabaoth

	Module Manager /L is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Module Manager /L is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0
	along with Module Manager /L. If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Linq;
using Xunit;
using NSubstitute;
using UnityEngine;
using TestUtils;
using ModuleManager;
using ModuleManager.Logging;

namespace ModuleManagerTests
{
    public class InGameTestRunnerTest
    {
        private readonly IBasicLogger logger;
        private readonly UrlDir databaseRoot;
        private readonly InGameTestRunner testRunner;

        public InGameTestRunnerTest()
        {
            logger = Substitute.For<IBasicLogger>();
            databaseRoot = UrlBuilder.CreateRoot();
            testRunner = new InGameTestRunner(logger);
        }

        [Fact]
        public void TestConstructor__LoggerNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new InGameTestRunner(null);
            });

            Assert.Equal("logger", ex.ParamName);
        }

        [Fact]
        public void TestRunTestCases__DatabaseRootNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                testRunner.RunTestCases(null);
            });

            Assert.Equal("gameDatabaseRoot", ex.ParamName);
        }

        [Fact]
        public void TestRunTestCases__WrongNumberOfNodes()
        {
            UrlDir.UrlFile file1 = UrlBuilder.CreateFile("abc/blah1.cfg", databaseRoot);

            // Call CreateCopy otherwise XUnit sees that it's an IEnumerable and attempts to compare by enumeration
            ConfigNode testNode1 = new TestConfigNode("NODE1")
            {
                { "key1", "value1" },
            }.CreateCopy();

            ConfigNode testNode2 = new ConfigNode("NODE2");

            ConfigNode expectNode = new TestConfigNode("MMTEST_EXPECT")
            {
                new TestConfigNode("NODE1")
                {
                    { "key1", "value1" },
                },
            }.CreateCopy();

            UrlBuilder.CreateConfig(testNode1, file1);
            UrlBuilder.CreateConfig(testNode2, file1);
            UrlBuilder.CreateConfig(expectNode, file1);

            testRunner.RunTestCases(databaseRoot);

            Received.InOrder(delegate
            {
                logger.AssertInfo("Running tests...");
                logger.AssertError($"Test blah1 failed as expected number of nodes differs expected: 1 found: 2");
                logger.AssertInfo(testNode1.ToString());
                logger.AssertInfo(testNode2.ToString());
                logger.AssertInfo(expectNode.ToString());
                logger.AssertInfo("tests complete.");
            });

            Assert.Equal(3, file1.configs.Count);
            Assert.Equal(testNode1, file1.configs[0].config);
            Assert.Equal(testNode2, file1.configs[1].config);
            Assert.Equal(expectNode, file1.configs[2].config);
        }

        [Fact]
        public void TestRunTestCases__AllPassing()
        {
            UrlDir.UrlFile file1 = UrlBuilder.CreateFile("abc/blah1.cfg", databaseRoot);
            UrlDir.UrlFile file2 = UrlBuilder.CreateFile("abc/blah2.cfg", databaseRoot);

            ConfigNode testNode1 = new TestConfigNode("NODE1")
            {
                { "key1", "value1" },
                { "key2", "value2" },
                new TestConfigNode("NODE2")
                {
                    { "key3", "value3" },
                },
            };

            ConfigNode testNode2 = new TestConfigNode("NODE3")
            {
                { "key4", "value4" },
            };

            ConfigNode testNode3 = new TestConfigNode("NODE4")
            {
                { "key5", "value5" },
            };

            UrlBuilder.CreateConfig(testNode1, file1);
            UrlBuilder.CreateConfig(testNode2, file1);
            UrlBuilder.CreateConfig(new TestConfigNode("MMTEST_EXPECT")
            {
                testNode1.CreateCopy(),
                testNode2.CreateCopy(),
            }, file1);

            UrlBuilder.CreateConfig(testNode3, file2);
            UrlBuilder.CreateConfig(new TestConfigNode("MMTEST_EXPECT")
            {
                testNode3.CreateCopy(),
            }, file2);

            testRunner.RunTestCases(databaseRoot);

            Received.InOrder(delegate
            {
                logger.AssertInfo("Running tests...");
                logger.AssertInfo("tests complete.");
            });

            logger.AssertNoError();

            Assert.Empty(file1.configs);
            Assert.Empty(file2.configs);
        }

        [Fact]
        public void TestRunTestCases__Failure()
        {
            UrlDir.UrlFile file1 = UrlBuilder.CreateFile("abc/blah1.cfg", databaseRoot);

            ConfigNode testNode1 = new TestConfigNode("NODE1")
            {
                { "key1", "value1" },
                { "key2", "value2" },
                new TestConfigNode("NODE2")
                {
                    { "key3", "value3" },
                },
            };

            ConfigNode expectNode1 = new TestConfigNode("NODE1")
            {
                { "key1", "value1" },
                { "key2", "value2" },
                new TestConfigNode("NODE2")
                {
                    { "key4", "value3" },
                },
            };

            UrlBuilder.CreateConfig(testNode1, file1);
            UrlBuilder.CreateConfig(new TestConfigNode("MMTEST_EXPECT")
            {
                expectNode1,
            }, file1);

            testRunner.RunTestCases(databaseRoot);

            Received.InOrder(delegate
            {
                logger.AssertInfo("Running tests...");
                logger.AssertError($"Test blah1[0] failed as expected output and actual output differ.\nexpected:\n{expectNode1}\nActually got:\n{testNode1}");
                logger.AssertInfo("tests complete.");
            });


            Assert.Empty(file1.configs);
        }
    }
}
