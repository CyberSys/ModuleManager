﻿/*
	This file is part of Module Manager /L
		© 2018-2024 LisiasT
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
using ModuleManager.Logging;
using ModuleManager.Patches.PassSpecifiers;
using ModuleManager.Progress;

namespace ModuleManager.Patches
{
    public interface IPatch
    {
        UrlDir.UrlConfig UrlConfig { get; }
        IPassSpecifier PassSpecifier { get; }
        bool CountsAsPatch { get; }
        void Apply(LinkedList<IProtoUrlConfig> configs, IPatchProgress progress, IBasicLogger logger);
    }
}
