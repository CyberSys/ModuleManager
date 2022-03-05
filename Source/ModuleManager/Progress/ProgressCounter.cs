﻿using System;
using System.Collections.Generic;
using ModuleManager.Utils;

namespace ModuleManager.Progress
{
    public class ProgressCounter
    {
        public readonly Counter totalPatches = new Counter();
        public readonly Counter appliedPatches = new Counter();
        public readonly SetableCounter patchedNodes = new SetableCounter();
        public readonly Counter warnings = new Counter();
        public readonly Counter errors = new Counter();
        public readonly Counter exceptions = new Counter();
        public readonly Counter needsUnsatisfied = new Counter();

        public readonly Dictionary<String, int> warningFiles = new Dictionary<string, int>();
        public readonly Dictionary<String, int> errorFiles = new Dictionary<string, int>();
    }
}
