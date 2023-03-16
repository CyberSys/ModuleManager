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
using System.Collections.Generic;
using System.Linq;
using Xunit;
using NSubstitute;
using TestUtils;
using ModuleManager;
using ModuleManager.Patches;
using ModuleManager.Patches.PassSpecifiers;
using ModuleManager.Progress;

namespace ModuleManagerTests
{
    public class PatchListTest
    {
        [Fact]
        public void TestConstructor__ModListNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new PatchList(null, new IPatch[0], Substitute.For<IPatchProgress>());
            });

            Assert.Equal("modList", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__PatchesNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new PatchList(new string[0], null, Substitute.For<IPatchProgress>());
            });

            Assert.Equal("patches", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__ProgressNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new PatchList(new string[0], new IPatch[0], null);
            });

            Assert.Equal("progress", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__UnknownMod()
        {
            IPatch patch = Substitute.For<IPatch>();
            UrlDir.UrlConfig urlConfig = UrlBuilder.CreateConfig("abc/def", new ConfigNode("NODE"));
            patch.PassSpecifier.Returns(new BeforePassSpecifier("mod3", urlConfig));
            IPatchProgress progress = Substitute.For<IPatchProgress>();

            KeyNotFoundException ex = Assert.Throws<KeyNotFoundException>(delegate
            {
                new PatchList(new[] { "mod1", "mod2" }, new[] { patch }, progress);
            });

            Assert.Equal("Mod 'mod3' not found", ex.Message);

            progress.DidNotReceive().PatchAdded();
        }

        [Fact]
        public void TestConstructor__UnknownPassSpecifier()
        {
            IPatch patch = Substitute.For<IPatch>();
            UrlDir.UrlConfig urlConfig = UrlBuilder.CreateConfig("abc/def", new ConfigNode("NODE"));
            IPassSpecifier passSpecifier = Substitute.For<IPassSpecifier>();
            passSpecifier.Descriptor.Returns(":SOMEPASS");
            patch.PassSpecifier.Returns(passSpecifier);
            IPatchProgress progress = Substitute.For<IPatchProgress>();

            NotImplementedException ex = Assert.Throws<NotImplementedException>(delegate
            {
                new PatchList(new string[0], new[] { patch }, progress);
            });

            Assert.Equal("Don't know what to do with pass specifier: :SOMEPASS", ex.Message);

            progress.DidNotReceive().PatchAdded();
        }

        [Fact]
        public void Test__Lifecycle()
        {
            IPatch[] patches = new IPatch[]
            {
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
                Substitute.For<IPatch>(),
            };

            UrlDir.UrlConfig urlConfig = UrlBuilder.CreateConfig("abc/def", new ConfigNode("NODE"));

            patches[00].PassSpecifier.Returns(new InsertPassSpecifier());
            patches[01].PassSpecifier.Returns(new InsertPassSpecifier());
            patches[02].PassSpecifier.Returns(new FirstPassSpecifier());
            patches[03].PassSpecifier.Returns(new FirstPassSpecifier());
            patches[04].PassSpecifier.Returns(new LegacyPassSpecifier());
            patches[05].PassSpecifier.Returns(new LegacyPassSpecifier());
            patches[06].PassSpecifier.Returns(new BeforePassSpecifier("mod1", urlConfig));
            patches[07].PassSpecifier.Returns(new BeforePassSpecifier("MOD1", urlConfig));
            patches[08].PassSpecifier.Returns(new ForPassSpecifier("mod1", urlConfig));
            patches[09].PassSpecifier.Returns(new ForPassSpecifier("MOD1", urlConfig));
            patches[10].PassSpecifier.Returns(new AfterPassSpecifier("mod1", urlConfig));
            patches[11].PassSpecifier.Returns(new AfterPassSpecifier("MOD1", urlConfig));
            patches[12].PassSpecifier.Returns(new LastPassSpecifier("mod1"));
            patches[13].PassSpecifier.Returns(new LastPassSpecifier("MOD1"));
            patches[14].PassSpecifier.Returns(new BeforePassSpecifier("mod2", urlConfig));
            patches[15].PassSpecifier.Returns(new BeforePassSpecifier("MOD2", urlConfig));
            patches[16].PassSpecifier.Returns(new ForPassSpecifier("mod2", urlConfig));
            patches[17].PassSpecifier.Returns(new ForPassSpecifier("MOD2", urlConfig));
            patches[18].PassSpecifier.Returns(new AfterPassSpecifier("mod2", urlConfig));
            patches[19].PassSpecifier.Returns(new AfterPassSpecifier("MOD2", urlConfig));
            patches[20].PassSpecifier.Returns(new LastPassSpecifier("mod2"));
            patches[21].PassSpecifier.Returns(new LastPassSpecifier("MOD2"));
            patches[22].PassSpecifier.Returns(new LastPassSpecifier("mod3"));
            patches[23].PassSpecifier.Returns(new FinalPassSpecifier());
            patches[24].PassSpecifier.Returns(new FinalPassSpecifier());

            patches[00].CountsAsPatch.Returns(false);
            patches[01].CountsAsPatch.Returns(false);
            patches[02].CountsAsPatch.Returns(true);
            patches[03].CountsAsPatch.Returns(true);
            patches[04].CountsAsPatch.Returns(true);
            patches[05].CountsAsPatch.Returns(true);
            patches[06].CountsAsPatch.Returns(true);
            patches[07].CountsAsPatch.Returns(true);
            patches[08].CountsAsPatch.Returns(true);
            patches[09].CountsAsPatch.Returns(true);
            patches[10].CountsAsPatch.Returns(true);
            patches[11].CountsAsPatch.Returns(true);
            patches[12].CountsAsPatch.Returns(true);
            patches[13].CountsAsPatch.Returns(true);
            patches[14].CountsAsPatch.Returns(true);
            patches[15].CountsAsPatch.Returns(true);
            patches[16].CountsAsPatch.Returns(true);
            patches[17].CountsAsPatch.Returns(true);
            patches[18].CountsAsPatch.Returns(true);
            patches[19].CountsAsPatch.Returns(true);
            patches[20].CountsAsPatch.Returns(true);
            patches[21].CountsAsPatch.Returns(true);
            patches[22].CountsAsPatch.Returns(true);
            patches[23].CountsAsPatch.Returns(true);
            patches[24].CountsAsPatch.Returns(true);

            IPatchProgress progress = Substitute.For<IPatchProgress>();

            PatchList patchList = new PatchList(new[] { "mod1", "mod2" }, patches, progress);

            IPass[] passes = patchList.ToArray();

            Assert.Equal(13, passes.Length);

            Assert.Equal(":INSERT (initial)", passes[0].Name);
            Assert.Equal(new[] { patches[0], patches[1] }, passes[0]);

            Assert.Equal(":FIRST", passes[1].Name);
            Assert.Equal(new[] { patches[2], patches[3] }, passes[1]);

            Assert.Equal(":LEGACY (default)", passes[2].Name);
            Assert.Equal(new[] { patches[4], patches[5] }, passes[2]);

            Assert.Equal(":BEFORE[MOD1]", passes[3].Name);
            Assert.Equal(new[] { patches[6], patches[7] }, passes[3]);

            Assert.Equal(":FOR[MOD1]", passes[4].Name);
            Assert.Equal(new[] { patches[8], patches[9] }, passes[4]);

            Assert.Equal(":AFTER[MOD1]", passes[5].Name);
            Assert.Equal(new[] { patches[10], patches[11] }, passes[5]);

            Assert.Equal(":BEFORE[MOD2]", passes[6].Name);
            Assert.Equal(new[] { patches[14], patches[15] }, passes[6]);

            Assert.Equal(":FOR[MOD2]", passes[7].Name);
            Assert.Equal(new[] { patches[16], patches[17] }, passes[7]);

            Assert.Equal(":AFTER[MOD2]", passes[8].Name);
            Assert.Equal(new[] { patches[18], patches[19] }, passes[8]);

            Assert.Equal(":LAST[MOD1]", passes[9].Name);
            Assert.Equal(new[] { patches[12], patches[13] }, passes[9]);

            Assert.Equal(":LAST[MOD2]", passes[10].Name);
            Assert.Equal(new[] { patches[20], patches[21] }, passes[10]);

            Assert.Equal(":LAST[MOD3]", passes[11].Name);
            Assert.Equal(new[] { patches[22] }, passes[11]);

            Assert.Equal(":FINAL", passes[12].Name);
            Assert.Equal(new[] { patches[23], patches[24] }, passes[12]);

            progress.Received(23).PatchAdded();
        }
    }
}
