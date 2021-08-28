using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VRChatConfigEditor
{
    [JsonObject]
    class VRCConfig
    {
        /**
         * Basic
         */
        [JsonProperty("disableRichPresence")]
        public bool? DisableRichPresence { get; set; }

        [JsonProperty("cache_directory")]
        public string CacheDirectory { get; set; }

        [JsonProperty("cache_size")]
        public int? CacheSize { get; set; }

        [JsonProperty("cache_expiry_delay")]
        public int? CacheExpiryDelay { get; set; }

        [JsonProperty("camera_res_height")]
        public int? CameraResHeight { get; set; }

        [JsonProperty("camera_res_width")]
        public int? CameraResWidth { get; set; }

        [JsonProperty("screenshot_res_height")]
        public int? ScreenshotResHeight { get; set; }

        [JsonProperty("screenshot_res_width")]
        public int? ScreenshotResWidth { get; set; }

        /**
         * Dynamic Bone
         */
        [JsonProperty("dynamic_bone_max_affected_transform_count")]
        public int? DynamicBoneMaxAffectedTransformCount { get; set; }

        [JsonProperty("dynamic_bone_max_collider_check_count")]
        public int? DynamicBoneMaxColliderCheckCount { get; set; }

        /**
         * Betas
         */
        [JsonProperty("betas")]
        public IList<string> Betas { get; set; }

        [JsonProperty("ps_max_particles")]
        public int? PsMaxParticles { get; set; }

        [JsonProperty("ps_max_emission")]
        public int? PsMaxEmission { get; set; }

        [JsonProperty("ps_max_total_emission")]
        public int? PsMaxTotalEmission { get; set; }

        [JsonProperty("ps_mesh_particle_divider")]
        public int? PsMeshParticleDivider { get; set; }

        [JsonProperty("ps_mesh_particle_poly_limit")]
        public int? PsMeshParticlePolyLimit { get; set; }

        [JsonProperty("ps_collision_penalty_high")]
        public int? PsCollisionPenaltyHigh { get; set; }

        [JsonProperty("ps_collision_penalty_med")]
        public int? PsCollisionPenaltyMed { get; set; }

        [JsonProperty("ps_collision_penalty_low")]
        public int? PsCollisionPenaltyLow { get; set; }

        [JsonProperty("ps_trails_penalty")]
        public int? PsTrailsPenalty { get; set; }

    }
}
