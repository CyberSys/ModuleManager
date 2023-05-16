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
using System.Text;
using Xunit;
using TestUtils;
using ModuleManager.Extensions;

namespace ModuleManagerTests.Extensions
{
    public class ConfigNodeExtensionsTest
    {
        [Fact]
        public void TestShallowCopyFrom()
        {
            ConfigNode fromNode = new TestConfigNode("SOME_NODE")
            {
                { "abc", "def" },
                { "ghi", "jkl" },
                new TestConfigNode("INNER_NODE_1")
                {
                    { "mno", "pqr" },
                    new TestConfigNode("INNER_INNER_NODE_1"),
                },
                new TestConfigNode("INNER_NODE_2")
                {
                    { "stu", "vwx" },
                    new TestConfigNode("INNER_INNER_NODE_2"),
                },
            };

            ConfigNode.Value value1 = fromNode.values[0];
            ConfigNode.Value value2 = fromNode.values[1];

            ConfigNode innerNode1 = fromNode.nodes[0];
            ConfigNode innerNode2 = fromNode.nodes[1];

            ConfigNode toNode = new TestConfigNode("SOME_OTHER_NODE")
            {
                { "value", "will be removed" },
                new TestConfigNode("NODE_WILL_BE_REMOVED"),
            };

            toNode.ShallowCopyFrom(fromNode);

            Assert.Equal("SOME_NODE", fromNode.name);
            Assert.Equal("SOME_OTHER_NODE", toNode.name);

            Assert.Equal(2, fromNode.values.Count);
            Assert.Equal(2, toNode.values.Count);

            Assert.Same(value1, fromNode.values[0]);
            Assert.Same(value1, toNode.values[0]);
            AssertValue("abc", "def", value1);

            Assert.Same(value2, fromNode.values[1]);
            Assert.Same(value2, toNode.values[1]);
            AssertValue("ghi", "jkl", value2);

            Assert.Equal(2, fromNode.nodes.Count);
            Assert.Equal(2, toNode.nodes.Count);

            Assert.Same(innerNode1, fromNode.nodes[0]);
            Assert.Same(innerNode1, toNode.nodes[0]);
            Assert.Equal("INNER_NODE_1", innerNode1.name);
            Assert.Equal(1, innerNode1.values.Count);
            AssertValue("mno", "pqr", innerNode1.values[0]);
            Assert.Equal(1, innerNode1.nodes.Count);
            Assert.Equal("INNER_INNER_NODE_1", innerNode1.nodes[0].name);
            Assert.Empty(innerNode1.nodes[0].values);
            Assert.Empty(innerNode1.nodes[0].nodes);

            Assert.Same(innerNode2, fromNode.nodes[1]);
            Assert.Same(innerNode2, toNode.nodes[1]);
            Assert.Equal("INNER_NODE_2", innerNode2.name);
            Assert.Equal(1, innerNode2.values.Count);
            AssertValue("stu", "vwx", innerNode2.values[0]);
            Assert.Equal(1, innerNode2.nodes.Count);
            Assert.Equal("INNER_INNER_NODE_2", innerNode2.nodes[0].name);
            Assert.Empty(innerNode2.nodes[0].values);
            Assert.Empty(innerNode2.nodes[0].nodes);
        }

        [Fact]
        public void TestDeepCopy()
        {
            ConfigNode fromNode = new TestConfigNode("SOME_NODE")
            {
                { "abc", "def" },
                { "ghi", "jkl" },
                new TestConfigNode("INNER_NODE_1")
                {
                    { "mno", "pqr" },
                    { "weird_values", "some\r\n\tstuff" },
                    new TestConfigNode("INNER_INNER_NODE_1"),
                },
                new TestConfigNode("INNER_NODE_2")
                {
                    { "stu", "vwx" },
                    new TestConfigNode("INNER_INNER_NODE_2"),
                },
            };
            
            ConfigNode toNode = fromNode.DeepCopy();
            
            Assert.Equal("SOME_NODE", toNode.name);
            
            Assert.Equal(2, toNode.values.Count);
            
            Assert.NotSame(fromNode.values[0], toNode.values[0]);
            AssertValue("abc", "def", toNode.values[0]);

            Assert.NotSame(fromNode.values[1], toNode.values[1]);
            AssertValue("ghi", "jkl", toNode.values[1]);

            Assert.Equal(2, toNode.nodes.Count);

            ConfigNode innerNode1 = toNode.nodes[0];
            Assert.NotSame(fromNode.nodes[0], innerNode1);
            Assert.Equal("INNER_NODE_1", innerNode1.name);
            Assert.Equal(2, innerNode1.values.Count);
            Assert.NotSame(fromNode.nodes[0].values[0], innerNode1.values[0]);
            AssertValue("mno", "pqr", innerNode1.values[0]);
            Assert.NotSame(fromNode.nodes[0].values[1], innerNode1.values[1]);
            AssertValue("weird_values", "some\r\n\tstuff", innerNode1.values[1]);
            Assert.Equal(1, toNode.nodes[0].nodes.Count);
            Assert.NotSame(fromNode.nodes[0].nodes[0], innerNode1.nodes[0]);
            Assert.Equal("INNER_INNER_NODE_1", innerNode1.nodes[0].name);
            Assert.Empty(innerNode1.nodes[0].values);
            Assert.Empty(innerNode1.nodes[0].nodes);

            ConfigNode innerNode2 = toNode.nodes[1];
            Assert.NotSame(fromNode.nodes[1], innerNode2);
            Assert.Equal("INNER_NODE_2", innerNode2.name);
            Assert.Equal(1, innerNode2.values.Count);
            Assert.NotSame(fromNode.nodes[1].values[0], innerNode2.values[0]);
            AssertValue("stu", "vwx", innerNode2.values[0]);
            Assert.Equal(1, innerNode2.nodes.Count);
            Assert.NotSame(fromNode.nodes[1].nodes[0], innerNode2.nodes[0]);
            Assert.Equal("INNER_INNER_NODE_2", innerNode2.nodes[0].name);
            Assert.Empty(innerNode2.nodes[0].values);
            Assert.Empty(innerNode2.nodes[0].nodes);
        }

        [Fact]
        public void TestPrettyPrint()
        {
            ConfigNode node = new TestConfigNode("SOME_NODE")
            {
                { "abc", "def" },
                { "ghi", "jkl" },
                new TestConfigNode("INNER_NODE_1")
                {
                    { "mno", "pqr" },
                    new TestConfigNode("INNER_INNER_NODE_1"),
                },
                new TestConfigNode("INNER_NODE_2")
                {
                    { "stu", "vwx" },
                    new TestConfigNode("INNER_INNER_NODE_2"),
                },
            };

            string expected = @"
XXSOME_NODE
XX{
XX  abc = def
XX  ghi = jkl
XX  INNER_NODE_1
XX  {
XX    mno = pqr
XX    INNER_INNER_NODE_1
XX    {
XX    }
XX  }
XX  INNER_NODE_2
XX  {
XX    stu = vwx
XX    INNER_INNER_NODE_2
XX    {
XX    }
XX  }
XX}
".TrimStart().Replace("\r", null);

            StringBuilder sb = new StringBuilder();
            node.PrettyPrint(ref sb, "XX");

            Assert.Equal(expected, sb.ToString());
        }

        [Fact]
        public void TestPrettyPrint__NullNode()
        {
            ConfigNode node = null;
            StringBuilder sb = new StringBuilder();
            node.PrettyPrint(ref sb, "XX");
            Assert.Equal("XX<null node>\n", sb.ToString());
        }

        [Fact]
        public void TestPrettyPrint__NullStringBuilder()
        {
            ConfigNode node = new ConfigNode("NODE");
            StringBuilder sb = null;
            Assert.Throws<ArgumentNullException>(delegate
            {
                node.PrettyPrint(ref sb, "XX");
            });
        }

        [Fact]
        public void TestPrettyPrint__NullIndent()
        {
            ConfigNode node = new TestConfigNode("SOME_NODE")
            {
                { "abc", "def" },
                { "ghi", "jkl" },
                new TestConfigNode("INNER_NODE")
                {
                    { "mno", "pqr" },
                },
            };

            string expected = @"
SOME_NODE
{
  abc = def
  ghi = jkl
  INNER_NODE
  {
    mno = pqr
  }
}
".TrimStart().Replace("\r", null);

            StringBuilder sb = new StringBuilder();
            node.PrettyPrint(ref sb, null);
            Assert.Equal(expected, sb.ToString());
        }

        [Fact]
        public void TestPrettyPrint__NullName()
        {
            ConfigNode node = new TestConfigNode()
            {
                { "abc", "def" },
                { "ghi", "jkl" },
                new TestConfigNode("INNER_NODE")
                {
                    { "mno", "pqr" },
                },
            };

            node.name = null;

            string expected = @"
XX<null>
XX{
XX  abc = def
XX  ghi = jkl
XX  INNER_NODE
XX  {
XX    mno = pqr
XX  }
XX}
".TrimStart().Replace("\r", null);

            StringBuilder sb = new StringBuilder();
            node.PrettyPrint(ref sb, "XX");
            Assert.Equal(expected, sb.ToString());
        }

        [Fact]
        public void TestAddValueSafe()
        {
            ConfigNode node = new TestConfigNode
            {
                { "key1", "value1" },
            };

            node.AddValueSafe("weird_values", "some\r\n\tstuff");

            Assert.Equal(2, node.values.Count);
            AssertValue("key1", "value1", node.values[0]);
            AssertValue("weird_values", "some\r\n\tstuff", node.values[1]);
        }

        [Fact]
        public void TestEscapeValuesRecursive()
        {
            ConfigNode node = new TestConfigNode
            {
                { "key1", "value1" },
                { "key2", "value\nwith\rescped\tchars" },
                new TestConfigNode("SUBNODE")
                {
                    { "key3", "value\nwith\rescped\tchars2" },
                },
            };

            node.EscapeValuesRecursive();

            Assert.Equal(2, node.values.Count);
            AssertValue("key1", "value1", node.values[0]);
            AssertValue("key2", "value\\nwith\\rescped\\tchars", node.values[1]);
            Assert.Equal(1, node.nodes.Count);
            Assert.Equal(1, node.nodes[0].values.Count);
            AssertValue("key3", "value\\nwith\\rescped\\tchars2", node.nodes[0].values[0]);
        }

        [Fact]
        public void TestUnescapeValuesRecursive()
        {
            ConfigNode node = new TestConfigNode
            {
                { "key1", "value1" },
                { "key2", "value\\nwith\\rescped\\tchars" },
                new TestConfigNode("SUBNODE")
                {
                    { "key3", "value\\nwith\\rescped\\tchars2" },
                },
            };

            node.UnescapeValuesRecursive();

            Assert.Equal(2, node.values.Count);
            AssertValue("key1", "value1", node.values[0]);
            AssertValue("key2", "value\nwith\rescped\tchars", node.values[1]);
            Assert.Equal(1, node.nodes.Count);
            Assert.Equal(1, node.nodes[0].values.Count);
            AssertValue("key3", "value\nwith\rescped\tchars2", node.nodes[0].values[0]);
        }

        private void AssertValue(string name, string value, ConfigNode.Value nodeValue)
        {
            Assert.Equal(name, nodeValue.name);
            Assert.Equal(value, nodeValue.value);
        }
    }
}
