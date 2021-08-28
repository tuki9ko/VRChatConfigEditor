using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace VRChatConfigEditor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string CONFIG_PATH = @"\AppData\LocalLow\VRChat\vrchat\config.json";
        private const string CACHE_PATH = @"\AppData\LocalLow\VRChat\vrchat";

        public MainWindow()
        {
            InitializeComponent();
        }

        private string jsonSerialize()
        {
            var jsonObject = new JObject();

            jsonObject.Add("disableRichPresence" ,disableRichPresenceCheckbox.IsChecked);
            if (!cacheDirectoryTextbox.Text.Equals("")) jsonObject.Add("cache_directory", cacheDirectoryTextbox.Text);
            if (!cacheSizeTextbox.Text.Equals("")) jsonObject.Add("cache_size", int.Parse(cacheSizeTextbox.Text));
            if (!cacheExpiryDelayTextbox.Text.Equals("")) jsonObject.Add("cache_expiry_delay", int.Parse(cacheExpiryDelayTextbox.Text));
            if (!cameraResHeightTextbox.Text.Equals("")) jsonObject.Add("camera_res_height", int.Parse(cameraResHeightTextbox.Text));
            if (!cameraResWidthTextbox.Text.Equals("")) jsonObject.Add("camera_res_width", int.Parse(cameraResWidthTextbox.Text));
            if (!screenshotResHeightTextbox.Text.Equals("")) jsonObject.Add("screenshot_res_height", int.Parse(screenshotResHeightTextbox.Text));
            if (!screenshotResWidthTextbox.Text.Equals("")) jsonObject.Add("screenshot_res_width", int.Parse(screenshotResWidthTextbox.Text));
            
            if (!dynamicBoneMaxAffectedTransformCountTextbox.Text.Equals("")) jsonObject.Add("dynamic_bone_max_affected_transform_count", int.Parse(dynamicBoneMaxAffectedTransformCountTextbox.Text));
            if (!dynamicBoneMaxColliderCheckCountTextbox.Text.Equals("")) jsonObject.Add("dynamic_bone_max_collider_check_count", int.Parse(dynamicBoneMaxColliderCheckCountTextbox.Text));

            if(particleLimiterCheckbox.IsChecked ?? false)
            {
                jsonObject.Add("betas", new JArray { "particle_system_limiter" });
            }
            if (!psMaxParticlesTextbox.Text.Equals("")) jsonObject.Add("ps_max_particles", int.Parse(psMaxParticlesTextbox.Text));
            if (!psMaxEmissionTextbox.Text.Equals("")) jsonObject.Add("ps_max_emission", int.Parse(psMaxEmissionTextbox.Text));
            if (!psMaxTotalEmissionTextbox.Text.Equals("")) jsonObject.Add("ps_max_total_emission", int.Parse(psMaxTotalEmissionTextbox.Text));
            if (!psMeshParticleDividerTextbox.Text.Equals("")) jsonObject.Add("ps_mesh_particle_divider", int.Parse(psMeshParticleDividerTextbox.Text));
            if (!psMeshParticlePolyLimitTextbox.Text.Equals("")) jsonObject.Add("ps_mesh_particle_poly_limit", int.Parse(psMeshParticlePolyLimitTextbox.Text));
            if (!psCollisionPenaltyHighTextbox.Text.Equals("")) jsonObject.Add("ps_collision_penalty_high", int.Parse(psCollisionPenaltyHighTextbox.Text));
            if (!psCollisionPenaltyMedTextbox.Text.Equals("")) jsonObject.Add("ps_collision_penalty_med", int.Parse(psCollisionPenaltyMedTextbox.Text));
            if (!psCollisionPenaltyLowTextbox.Text.Equals("")) jsonObject.Add("ps_collision_penalty_low", int.Parse(psCollisionPenaltyLowTextbox.Text));
            if (!psTrailsPenaltyTextbox.Text.Equals("")) jsonObject.Add("ps_trails_penalty", int.Parse(psTrailsPenaltyTextbox.Text));

            return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        }

        private void jsonDeserialize(string jsonText)
        {
            var jsonObject = JsonConvert.DeserializeObject<VRCConfig>(jsonText);

            if(jsonObject == null)
            {
                jsonObject = new VRCConfig();
            }

            disableRichPresenceCheckbox.IsChecked = jsonObject.DisableRichPresence ?? false;
            cacheDirectoryTextbox.Text = jsonObject.CacheDirectory ?? "";
            cacheSizeTextbox.Text = jsonObject.CacheSize.ToString() ?? "";
            cacheExpiryDelayTextbox.Text = jsonObject.CacheExpiryDelay.ToString() ?? "";
            cameraResHeightTextbox.Text = jsonObject.CameraResHeight.ToString() ?? "";
            cameraResWidthTextbox.Text = jsonObject.CameraResWidth.ToString() ?? "";
            screenshotResHeightTextbox.Text = jsonObject.ScreenshotResHeight.ToString() ?? "";
            screenshotResWidthTextbox.Text = jsonObject.ScreenshotResWidth.ToString() ?? "";

            dynamicBoneMaxAffectedTransformCountTextbox.Text = jsonObject.DynamicBoneMaxAffectedTransformCount.ToString() ?? "";
            dynamicBoneMaxColliderCheckCountTextbox.Text = jsonObject.DynamicBoneMaxColliderCheckCount.ToString() ?? "";

            particleLimiterCheckbox.IsChecked = jsonObject.Betas?.Contains("particle_system_limiter") ?? false;

            particleLimiter.IsEnabled = particleLimiterCheckbox.IsChecked ?? false;

            psMaxParticlesTextbox.Text = jsonObject.PsMaxParticles.ToString() ?? "";
            psMaxEmissionTextbox.Text = jsonObject.PsMaxEmission.ToString() ?? "";
            psMaxTotalEmissionTextbox.Text = jsonObject.PsMaxTotalEmission.ToString() ?? "";
            psMeshParticleDividerTextbox.Text = jsonObject.PsMeshParticleDivider.ToString() ?? "";
            psMeshParticlePolyLimitTextbox.Text = jsonObject.PsMeshParticlePolyLimit.ToString() ?? "";
            psCollisionPenaltyHighTextbox.Text = jsonObject.PsCollisionPenaltyHigh.ToString() ?? "";
            psCollisionPenaltyMedTextbox.Text = jsonObject.PsCollisionPenaltyMed.ToString() ?? "";
            psCollisionPenaltyLowTextbox.Text = jsonObject.PsCollisionPenaltyLow.ToString() ?? "";
            psTrailsPenaltyTextbox.Text = jsonObject.PsTrailsPenalty.ToString() ?? "";
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            string jsonText;

            var path = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + CONFIG_PATH;

            configFilePathTextBox.Text = path;

            using(var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            using(var sr = new StreamReader(fs))
            {
                jsonText = sr.ReadToEnd();

                jsonDeserialize(jsonText);
            }
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                FileName = "config.json",
                DefaultExt = ".json",
                Filter = "Json documents (.json)|*.json"
            };

            bool? result = dialog.ShowDialog();
            if(result == true)
            {
                configFilePathTextBox.Text = dialog.FileName;
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "設定を保存しますか？",
                "設定の保存",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                var jsonText = jsonSerialize();

                using (var fs = new FileStream(configFilePathTextBox.Text, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(jsonText);
                }

                MessageBox.Show("保存完了しました。");
            }
        }

        private void numberTextbox_TextInput(object sender, TextCompositionEventArgs e)
        {
            var source = e.Source as TextBox;
            var regex = new Regex("[^0-9.-]+");
            if (source.Text.Length > 0 && e.Text == "-")
            {
                e.Handled = true;
                return;
            }

            var text = source.Text + e.Text;
            e.Handled = regex.IsMatch(text);
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
               "デフォルト値をロードしますか？(ファイルへの保存は行われません)",
               "デフォルト値のロード",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                disableRichPresenceCheckbox.IsChecked = false;
                cacheDirectoryTextbox.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + CACHE_PATH;
                cacheSizeTextbox.Text = "20";
                cacheExpiryDelayTextbox.Text = "30";
                cameraResHeightTextbox.Text = "1080";
                cameraResWidthTextbox.Text = "1920";
                screenshotResHeightTextbox.Text = "1080";
                screenshotResWidthTextbox.Text = "1920";

                dynamicBoneMaxAffectedTransformCountTextbox.Text = "32";
                dynamicBoneMaxColliderCheckCountTextbox.Text = "8";

                particleLimiterCheckbox.IsChecked = false;

                particleLimiter.IsEnabled = particleLimiterCheckbox.IsChecked ?? false;

                psMaxParticlesTextbox.Text = "50000";
                psMaxEmissionTextbox.Text = "5000";
                psMaxTotalEmissionTextbox.Text = "40000";
                psMeshParticleDividerTextbox.Text = "60";
                psMeshParticlePolyLimitTextbox.Text = "50000";
                psCollisionPenaltyHighTextbox.Text = "50";
                psCollisionPenaltyMedTextbox.Text = "30";
                psCollisionPenaltyLowTextbox.Text = "10";
                psTrailsPenaltyTextbox.Text = "10";
            }
        }
    }
}
