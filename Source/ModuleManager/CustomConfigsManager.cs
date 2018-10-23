﻿using System;
using System.IO;
using UnityEngine;

namespace ModuleManager
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class CustomConfigsManager : MonoBehaviour
    {
		static bool start_techtree_loaded = false;
		static bool start_physics_loaded = false;
        internal void Start()
        {
#if false
			Log("Blah");
            Log(HighLogic.CurrentGame.Parameters.Career.TechTreeUrl);
			Log(MMPatchLoader.TECHTREE_CONFIG.Path);
			Log(MMPatchLoader.TECHTREE_CONFIG.IsLoadable.ToString());
#endif            
			if (!start_techtree_loaded && MMPatchLoader.TECHTREE_CONFIG.IsLoadable)
            {
                Log("Setting modded tech tree as the active one");
                HighLogic.CurrentGame.Parameters.Career.TechTreeUrl = MMPatchLoader.TECHTREE_CONFIG.KspPath;
				start_techtree_loaded = true;
            }

			if (!start_physics_loaded && MMPatchLoader.PHYSICS_CONFIG.IsLoadable)
            {
                Log("Setting modded physics as the active one");
                PhysicsGlobals.PhysicsDatabaseFilename = MMPatchLoader.PHYSICS_CONFIG.Path;
                if (!PhysicsGlobals.Instance.LoadDatabase())
                    Log("Something went wrong while setting the active physics config.");
                start_physics_loaded = true;
            }
        }

		private static readonly KSPe.Util.Log.Logger log = KSPe.Util.Log.Logger.CreateForType<CustomConfigsManager>();
        private static void Log(String s)
        {
            log.info(s);
        }

    }
}