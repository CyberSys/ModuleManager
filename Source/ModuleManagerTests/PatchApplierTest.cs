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
using System.Collections.Generic;
using Xunit;
using NSubstitute;
using UnityEngine;
using ModuleManager;
using ModuleManager.Collections;
using ModuleManager.Logging;
using ModuleManager.Patches;
using ModuleManager.Progress;

namespace ModuleManagerTests
{
    public class PatchApplierTest
    {
        [Fact]
        public void TestConstructor__ProgressNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new PatchApplier(null, Substitute.For<IBasicLogger>());
            });

            Assert.Equal("progress", ex.ParamName);
        }

        [Fact]
        public void TestConstructor__LoggerNull()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                new PatchApplier(Substitute.For<IPatchProgress>(), null);
            });

            Assert.Equal("logger", ex.ParamName);
        }

        [Fact]
        public void TestApplyPatches__PatchesNull()
        {
            PatchApplier applier = new PatchApplier(Substitute.For<IPatchProgress>(), Substitute.For<IBasicLogger>());
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(delegate
            {
                applier.ApplyPatches(null);
            });

            Assert.Equal("patches", ex.ParamName);
        }

        [Fact]
        public void TestApplyPatches()
        {
            IBasicLogger logger = Substitute.For<IBasicLogger>();
            IPatchProgress progress = Substitute.For<IPatchProgress>();
            PatchApplier patchApplier = new PatchApplier(progress, logger);
            IPass pass1 = Substitute.For<IPass>();
            IPass pass2 = Substitute.For<IPass>();
            IPass pass3 = Substitute.For<IPass>();
            pass1.Name.Returns(":PASS1");
            pass2.Name.Returns(":PASS2");
            pass3.Name.Returns(":PASS3");

            UrlDir.UrlConfig[] patchUrlConfigs = new UrlDir.UrlConfig[9];
            IPatch[] patches = new IPatch[9];
            for (int i = 0; i < patches.Length; i++)
            {
                patches[i] = Substitute.For<IPatch>();
            }

            patches[0].CountsAsPatch.Returns(false);
            patches[1].CountsAsPatch.Returns(false);
            patches[2].CountsAsPatch.Returns(false);
            patches[3].CountsAsPatch.Returns(true);
            patches[4].CountsAsPatch.Returns(true);
            patches[5].CountsAsPatch.Returns(true);
            patches[6].CountsAsPatch.Returns(true);
            patches[7].CountsAsPatch.Returns(true);
            patches[8].CountsAsPatch.Returns(true);

            pass1.GetEnumerator().Returns(new ArrayEnumerator<IPatch>(patches[0], patches[1], patches[2]));
            pass2.GetEnumerator().Returns(new ArrayEnumerator<IPatch>(patches[3], patches[4], patches[5]));
            pass3.GetEnumerator().Returns(new ArrayEnumerator<IPatch>(patches[6], patches[7], patches[8]));

            IPass[] patchList = new IPass[] { pass1, pass2, pass3 };

            LinkedList<IProtoUrlConfig> databaseConfigs = Assert.IsType<LinkedList<IProtoUrlConfig>>(patchApplier.ApplyPatches(new[] { pass1, pass2, pass3 }));

            progress.DidNotReceiveWithAnyArgs().Error(null, null);
            progress.DidNotReceiveWithAnyArgs().Exception(null, null);
            progress.DidNotReceiveWithAnyArgs().Exception(null, null, null);

            logger.AssertNoWarning();
            logger.AssertNoError();
            logger.AssertNoException();

            Received.InOrder(delegate
            {
                progress.PassStarted(pass1);
                patches[0].Apply(databaseConfigs, progress, logger);
                patches[1].Apply(databaseConfigs, progress, logger);
                patches[2].Apply(databaseConfigs, progress, logger);
                progress.PassStarted(pass2);
                patches[3].Apply(databaseConfigs, progress, logger);
                progress.PatchApplied();
                patches[4].Apply(databaseConfigs, progress, logger);
                progress.PatchApplied();
                patches[5].Apply(databaseConfigs, progress, logger);
                progress.PatchApplied();
                progress.PassStarted(pass3);
                patches[6].Apply(databaseConfigs, progress, logger);
                progress.PatchApplied();
                patches[7].Apply(databaseConfigs, progress, logger);
                progress.PatchApplied();
                patches[8].Apply(databaseConfigs, progress, logger);
                progress.PatchApplied();
            });
        }
    }
}
