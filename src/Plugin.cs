using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

#if BEPINEX

using BepInEx;

namespace NoSprintBirdseeds {
    [BepInPlugin("com.github.Kaden5480.poy-no-sprint-birdseeds", "NoSprintBirdseeds", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        public void Awake() {
            Harmony.CreateAndPatchAll(typeof(Plugin.DisableSprintBirdseeds));
            Harmony.CreateAndPatchAll(typeof(Plugin.EnableSprintChalk));
        }

#elif MELONLOADER

using MelonLoader;

[assembly: MelonInfo(typeof(NoSprintBirdseeds.Plugin), "NoSprintBirdseeds", "0.1.0", "Kaden5480")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace NoSprintBirdseeds {
    public class Plugin: MelonMod {

#endif

        /**
         * Disables the keybind for using birdseeds when sprinting.
         */
        [HarmonyPatch(typeof(ChalkBag), "UseBirdSeeds")]
        static class DisableSprintBirdseeds {
            static IEnumerable<CodeInstruction> Transpiler(
                IEnumerable<CodeInstruction> insts
            ) {
                FieldInfo player = AccessTools.Field(
                    typeof(ChalkBag), "player"
                );

                MethodInfo getButton = AccessTools.Method(
                    typeof(Rewired.Player), nameof(Rewired.Player.GetButton),
                    new[] { typeof(string) }
                );

                IEnumerable<CodeInstruction> replaced = Helper.BranchAlways(insts,
                    new[] {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, player),
                        new CodeInstruction(OpCodes.Ldstr, "Run"),
                        new CodeInstruction(OpCodes.Callvirt, getButton),
                        new CodeInstruction(OpCodes.Brfalse, null),
                    }
                );

                // Return patched instructions
                foreach (CodeInstruction replace in replaced) {
                    yield return replace;
                }
            }
        }

        /**
         * Allows using chalk while sprinting.
         */
        [HarmonyPatch(typeof(ChalkBag), "UseChalkFromHotkeyMethod")]
        [HarmonyPatch(MethodType.Enumerator)]
        static class EnableSprintChalk {
            static IEnumerable<CodeInstruction> Transpiler(
                IEnumerable<CodeInstruction> insts
            ) {
                FieldInfo player = AccessTools.Field(
                    typeof(ChalkBag), "player"
                );

                MethodInfo getButton = AccessTools.Method(
                    typeof(Rewired.Player), nameof(Rewired.Player.GetButton),
                    new[] { typeof(string) }
                );

                IEnumerable<CodeInstruction> replaced = Helper.BranchAlways(insts,
                    new[] {
                        new CodeInstruction(OpCodes.Ldloc_1),
                        new CodeInstruction(OpCodes.Ldfld, player),
                        new CodeInstruction(OpCodes.Ldstr, "Run"),
                        new CodeInstruction(OpCodes.Callvirt, getButton),
                        new CodeInstruction(OpCodes.Brfalse, null),
                    }
                );

                // Return patched instructions
                foreach (CodeInstruction replace in replaced) {
                    yield return replace;
                }
            }
        }
    }
}
