using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine.PostProcessing;

namespace BloomSettings
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        private Harmony harmony;
        private ConfigEntry<float> bloomIntensity;
        private ConfigEntry<float> bloomThreshold;
        private ConfigEntry<float> softKnee;

        private static PostProcessingBehaviour postProcessingBehaviour;

        private void Awake()
        {
            Instance = this;
            harmony = new Harmony("main");
            harmony.PatchAll(typeof(Plugin));

            bloomIntensity = Config.Bind(
                "General",
                "Bloom intensity (-1 to skip this setting)",
                -1f
            );
            bloomThreshold = Config.Bind(
                "General",
                "Bloom threshold  (-1 to skip this setting)",
                1.4f
            );
            softKnee = Config.Bind(
                "General",
                "Soft Knee (-1 to skip this setting)",
                0.3f
            );
        }


        [HarmonyPatch(typeof(CameraEffects), "SetBloom")]
        [HarmonyPostfix]
        private static void SetBloomPostfix(ref CameraEffects __instance, bool enabled)
        {
            if (postProcessingBehaviour == null)
                postProcessingBehaviour = __instance.GetComponent<PostProcessingBehaviour>();
            if (postProcessingBehaviour == null || !enabled)
                return;

            var settings = postProcessingBehaviour.profile.bloom.settings;
            if (Instance.bloomIntensity.Value >= 0f)
                settings.bloom.intensity = Instance.bloomIntensity.Value;
            if (Instance.bloomThreshold.Value >= 0f)
                settings.bloom.threshold = Instance.bloomThreshold.Value;
            if (Instance.softKnee.Value >= 0f)
                settings.bloom.softKnee = Instance.softKnee.Value;

            postProcessingBehaviour.profile.bloom.settings = settings;
        }
    }
}