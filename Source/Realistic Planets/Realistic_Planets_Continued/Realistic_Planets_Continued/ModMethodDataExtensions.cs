using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using System.Reflection;
using System.Diagnostics;

namespace Planets_Code
{
	public static class ModMethodDataExtensions
	{
		public static string FullMethodName(this ModMethodData modMethodData)
		{
			return String.Join("::", modMethodData.TypeName, modMethodData.MethodName);
		}

		public static bool ModIsLoaded(this ModMethodData modMethodData)
		{
			return LoadedModManager.RunningMods.Any(x => x.PackageIdPlayerFacing == modMethodData.PackageId);
		}

		public static MethodInfo GetMethod(this ModMethodData modMethodData)
		{
            if (modMethodData == null)
            {
                Log.Message("Mod method data itself is the thing that's null, throwing exception");
                throw new ArgumentNullException(nameof(modMethodData));
            }

			var mod = LoadedModManager.RunningMods.FirstOrDefault(x => x.PackageIdPlayerFacing == modMethodData.PackageId);

            if (mod == null)
			{
				throw new ArgumentException($"Tried to get method in mod that is not loaded. Target packageId={modMethodData.PackageId}.");
			}

			Debug.Assert(mod.assemblies.loadedAssemblies.Count > 0);

			foreach (var assembly in mod.assemblies.loadedAssemblies)
			{
				var dialog = assembly.GetType(modMethodData.TypeName);
                if (dialog != null)
				{
                    //modified to include non-public methods in search to ensure compatibility with harmony postfix methods
                    return dialog.GetMethod(modMethodData.MethodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
				}
            }

			Log.Warning($"Realistic Planets - Fan Update was unable to find {FullMethodName(modMethodData)} in mod with packageId={modMethodData.PackageId}. Please ensure that both mods have been updated to their latest versions.");

			return null;
		}

		public static MethodInfo GetMethodIfLoaded(this ModMethodData modMethodData)
		{
			if (ModIsLoaded(modMethodData))
			{
				return GetMethod(modMethodData);
			}
			return null;
		}
	}
}
