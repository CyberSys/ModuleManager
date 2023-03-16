﻿/*
	This file is part of Module Manager /L
		© 2018-2023 LisiasT
		© 2013-2018 Sarbian using System; Blowfish
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
using Xunit;
using ModuleManager.Tags;

namespace ModuleManagerTests.Tags
{
    public class TagTest
    {
        [Fact]
        public void Test__OnlyKey()
        {
            Tag tag = new Tag("key", null, null);

            Assert.Equal("key", tag.key);
            Assert.Null(tag.value);
            Assert.Null(tag.trailer);
        }

        [Fact]
        public void Test__KeyAndValue()
        {
            Tag tag = new Tag("key", "value", null);

            Assert.Equal("key", tag.key);
            Assert.Equal("value", tag.value);
            Assert.Null(tag.trailer);
        }

        [Fact]
        public void Test__KeyAndEmptyValue()
        {
            Tag tag = new Tag("key", "", null);

            Assert.Equal("key", tag.key);
            Assert.Equal("", tag.value);
            Assert.Null(tag.trailer);
        }

        [Fact]
        public void Test__KeyValueAndTrailer()
        {
            Tag tag = new Tag("key", "value", "trailer");

            Assert.Equal("key", tag.key);
            Assert.Equal("value", tag.value);
            Assert.Equal("trailer", tag.trailer);
        }

        [Fact]
        public void Test__KeyEmptyValueAndTrailer()
        {
            Tag tag = new Tag("key", "", "trailer");

            Assert.Equal("key", tag.key);
            Assert.Equal("", tag.value);
            Assert.Equal("trailer", tag.trailer);
        }

        [Fact]
        public void TestConstructor__KeyNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new Tag(null, "value", "trailer");
            });

            Assert.Equal("key", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__KeyEmpty()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate
            {
                new Tag("", "value", "trailer");
            });

            Assert.Equal("key", ex.ParamName);
            Assert.Contains("can't be empty", ex.Message);
        }

        [Fact]
        public void TestConstructor__ValueNullButTrailerNotNull()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate
            {
                new Tag("key", null, "trailer");
            });
            
            Assert.Contains("trailer must be null if value is null", ex.Message);
        }

        [Fact]
        public void TestConstructor__TrailerEmpty()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(delegate
            {
                new Tag("key", "value", "");
            });

            Assert.Equal("trailer", ex.ParamName);
            Assert.Contains("can't be empty (null allowed)", ex.Message);
        }

        [Fact]
        public void TestToString__Key()
        {
            Tag tag = new Tag("key", null, null);

            Assert.Equal("< 'key' >", tag.ToString());
        }

        [Fact]
        public void TestToString__KeyAndValue()
        {
            Tag tag = new Tag("key", "value", null);

            Assert.Equal("< 'key' [ 'value' ] >", tag.ToString());
        }

        [Fact]
        public void TestToString__KeyAndEmptyValue()
        {
            Tag tag = new Tag("key", "", null);

            Assert.Equal("< 'key' [ '' ] >", tag.ToString());
        }

        [Fact]
        public void TestToString__KeyValueAndTrailer()
        {
            Tag tag = new Tag("key", "value", "trailer");

            Assert.Equal("< 'key' [ 'value' ] 'trailer' >", tag.ToString());
        }

        [Fact]
        public void TestToString__KeyEmptyValueAndTrailer()
        {
            Tag tag = new Tag("key", "", "trailer");

            Assert.Equal("< 'key' [ '' ] 'trailer' >", tag.ToString());
        }
    }
}
