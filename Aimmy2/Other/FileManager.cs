using Aimmy2.AILogic;
using Aimmy2.Class;
using Aimmy2.Visuality;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Visuality;

namespace Aimmy2.Other
{
    internal class FileManager
    {
        public FileSystemWatcher? ModelFileWatcher;
        public FileSystemWatcher? ConfigFileWatcher;

        private ListBox ModelListBox;
        private Label SelectedModelNotifier;
        private ObservableCollection<ModelItem> _modelItems;

        private ListBox ConfigListBox;
        private Label SelectedConfigNotifier;

        public bool InQuittingState = false;

        //private DetectedPlayerWindow DetectedPlayerOverlay;
        //private FOV FOVWindow;

        public static AIManager? AIManager;
        #region general things
        public FileManager(ListBox modelListBox, Label selectedModelNotifier, ListBox configListBox, Label selectedConfigNotifier)
        {
            ModelListBox = modelListBox;
            SelectedModelNotifier = selectedModelNotifier;

            ConfigListBox = configListBox;
            SelectedConfigNotifier = selectedConfigNotifier;

            ModelListBox.SelectionChanged += ModelListBox_SelectionChanged;
            ConfigListBox.SelectionChanged += ConfigListBox_SelectionChanged;

            ModelListBox.DragOver += ModelListBox_DragOver;
            ModelListBox.Drop += ModelListBox_DragDrop;

            ConfigListBox.AllowDrop = true;
            ConfigListBox.DragOver += ConfigListBox_DragDrop;
            ConfigListBox.Drop += ConfigListBox_DragDrop;

            _modelItems = new ObservableCollection<ModelItem>();
            ModelListBox.ItemsSource = _modelItems;

            CheckForRequiredFolders();
            InitializeFileWatchers();

            LoadModelsIntoListBox(null, null);
            LoadConfigsIntoListBox(null, null);
        }

        public void InitializeFileWatchers()
        {
            ModelFileWatcher = new FileSystemWatcher();
            ConfigFileWatcher = new FileSystemWatcher();

            InitializeWatcher(ref ModelFileWatcher, "bin/models", "*.onnx");
            InitializeWatcher(ref ConfigFileWatcher, "bin/configs", "*.cfg");
        }

        private void InitializeWatcher(ref FileSystemWatcher watcher, string path, string filter)
        {
            watcher.Path = path;
            watcher.Filter = filter;
            watcher.EnableRaisingEvents = true;

            if (filter == "*.onnx")
            {
                watcher.Changed += LoadModelsIntoListBox;
                watcher.Created += LoadModelsIntoListBox;
                watcher.Deleted += LoadModelsIntoListBox;
                watcher.Renamed += LoadModelsIntoListBox;
            }
            else if (filter == "*.cfg")
            {
                watcher.Changed += LoadConfigsIntoListBox;
                watcher.Created += LoadConfigsIntoListBox;
                watcher.Deleted += LoadConfigsIntoListBox;
                watcher.Renamed += LoadConfigsIntoListBox;
            }
        }
        private void CheckForRequiredFolders()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] dirs = ["bin\\models", "bin\\images", "bin\\labels", "bin\\configs", "bin\\anti_recoil_configs"];

            try
            {
                foreach (string dir in dirs)
                {
                    string fullPath = Path.Combine(baseDir, dir);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("Error creating a required directory: " + ex);
                MessageBox.Show($"Error creating a required directory: {ex}");
                Application.Current.Shutdown();
            }
        }
        #endregion  
        #region drag drop
        private void ModelListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void ModelListBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string targetFolder = "bin/models";

                foreach (var file in files)
                {
                    if (Path.GetExtension(file) == ".onnx")
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(targetFolder, fileName);
                        File.Move(file, destFile, true);
                    }
                }
            }
        }
        private void ConfigListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void ConfigListBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string targetFolder = "bin/models";

                foreach (var file in files)
                {
                    if (Path.GetExtension(file) == ".cfg")
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = Path.Combine(targetFolder, fileName);
                        File.Move(file, destFile, true);
                    }
                }
            }
        }
#endregion
        #region models
        public static bool CurrentlyLoadingModel = false;

        public void LoadModelsIntoListBox(object? sender, FileSystemEventArgs? e)
        {
            if (!InQuittingState)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _modelItems.Clear();
                    string[] onnxFiles = Directory.GetFiles("bin/models", "*.onnx");
                    //ModelListBox.Items.Clear();

                    foreach (string filePath in onnxFiles)
                    {
                        LogInfo(filePath + " " + Path.GetFileName(filePath) + " omg");
                        _modelItems.Add(new ModelItem { Name = Path.GetFileName(filePath), IsLoading = false });
                    }

                    if (ModelListBox.Items.Count > 0)
                    {
                        string? lastLoadedModel = Dictionary.lastLoadedModel;
                        if (lastLoadedModel != "N/A" && !ModelListBox.Items.Contains(lastLoadedModel)) { ModelListBox.SelectedItem = lastLoadedModel; }
                        SelectedModelNotifier.Content = $"Loaded Model: {lastLoadedModel}";
                    }
                });
            }
        }
        private async void ModelListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelListBox.SelectedItem as ModelItem == null) return;

            var selectedModel = ModelListBox.SelectedItem as ModelItem;

            string modelPath = Path.Combine("bin", "models", selectedModel!.Name);

            if (!modelPath.EndsWith(".onnx", StringComparison.OrdinalIgnoreCase))
            {
                modelPath += ".onnx";
            }

            if (!File.Exists(modelPath))
            {
                LogError($"Model path doesn't exist: {modelPath}", true);
                return;
            }
            
            // Check if the model is already selected or currently loading
            if (Dictionary.lastLoadedModel == selectedModel.Name || CurrentlyLoadingModel) return;

            CurrentlyLoadingModel = true;
            ModelListBox.IsEnabled = false;

            var selectedModelName = selectedModel.Name;

            var model_ = _modelItems.FirstOrDefault(m => m.Name == selectedModelName);
            if (model_ != null)
            {
                
                model_.IsLoading = true;
            }

            Dictionary.lastLoadedModel = selectedModelName;

            // Store original values and disable them temporarily
            var toggleKeys = new[] { "Aim Assist", "Constant AI Tracking", "Auto Trigger", "Show Detected Player", "Show AI Confidence", "Show Tracers" };
            var originalToggleStates = toggleKeys.ToDictionary(key => key, key => Dictionary.toggleState[key]);

            foreach (var key in toggleKeys)
            {
                Dictionary.toggleState[key] = false;
            }

            // Let the AI finish up
            await Task.Delay(150);

            // Reload AIManager with new model
            AIManager?.Dispose();
            AIManager = new AIManager(modelPath);

            // Restore original values
            foreach (var keyValuePair in originalToggleStates)
            {
                Dictionary.toggleState[keyValuePair.Key] = keyValuePair.Value;
            }

            bool TensorRT = Dictionary.dropdownState["Execution Provider Type"] == "TensorRT";

            foreach (var model in _modelItems)
            {
                if (TensorRT)
                {
                    await Task.Run(async () =>
                    {
                        while (CurrentlyLoadingModel)
                        {
                            await Task.Delay(50); // Check every 50 milliseconds if tensorrt model has loaded, this isnt really needed for cuda as it will not take that long for it to load.
                        }
                    });
                }

                model.IsLoading = false;
            }

            ModelListBox.IsEnabled = true;
            string content = "Loaded Model: " + selectedModelName;
            SelectedModelNotifier.Content = content;
            new NoticeBar(content, 2000).Show();
        }
        #endregion
        #region configs
        private void ConfigListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConfigListBox.SelectedItem == null) return;
            string selectedConfig = ConfigListBox.SelectedItem.ToString()!;

            string configPath = Path.Combine("bin/configs", selectedConfig);

            ConfigListBox.IsEnabled = false;

            SaveDictionary.LoadJSON(Dictionary.sliderSettings, configPath);
            PropertyChanger.PostNewConfig(configPath, true);

            ConfigListBox.IsEnabled = true;
            SelectedConfigNotifier.Content = "Loaded Config: " + selectedConfig;
        }



        public void LoadConfigsIntoListBox(object? sender, FileSystemEventArgs? e)
        {
            if (!InQuittingState)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string[] configFiles = Directory.GetFiles("bin/configs", "*.cfg");
                    ConfigListBox.Items.Clear();

                    foreach (string filePath in configFiles)
                    {
                        ConfigListBox.Items.Add(Path.GetFileName(filePath));
                    }

                    if (ConfigListBox.Items.Count > 0)
                    {
                        string? lastLoadedConfig = Dictionary.lastLoadedConfig;
                        if (lastLoadedConfig != "N/A" && !ConfigListBox.Items.Contains(lastLoadedConfig)) { ConfigListBox.SelectedItem = lastLoadedConfig; }

                        SelectedConfigNotifier.Content = "Loaded Config: " + lastLoadedConfig;
                    }
                });
            }
        }
        #endregion
        #region files 

        public static async Task<HashSet<string>> RetrieveAndAddFiles(string repoLink, string localPath, HashSet<string> allFiles)
        {
            try
            {
                GithubManager githubManager = new();

                var files = await githubManager.FetchGithubFilesAsync(repoLink);

                foreach (var file in files)
                {
                    if (file == null) continue;

                    if (!allFiles.Contains(file) && !File.Exists(Path.Combine(localPath, file)))
                    {
                        allFiles.Add(file);
                    }
                }

                githubManager.Dispose();

                return allFiles;
            }
            catch (Exception ex)
            {
                LogError(ex.ToString(), true);
                throw new Exception(ex.ToString());
            }
        }
        public static void LoadConfig(UI uiManager, string path = "bin\\configs\\Default.cfg", bool loading_from_configlist = false)
        {
            SaveDictionary.LoadJSON(Dictionary.sliderSettings, path);
            try
            {
                if (loading_from_configlist)
                {
                    if (Dictionary.sliderSettings["Suggested Model"] != "N/A" || Dictionary.sliderSettings["Suggested Model"] != "")
                    {
                        MessageBox.Show(
                            "The creator of this model suggests you use this model:\n" +
                            Dictionary.sliderSettings["Suggested Model"], "Suggested Model - Aimmy"
                        );
                    }

                    uiManager.S_FireRate!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Fire Rate", 1);

                    uiManager.S_FOVSize!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "FOV Size", 640);
                    uiManager.S_DynamicFOVSize!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Dynamic FOV Size", 200);

                    uiManager.S_MouseSensitivity!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Mouse Sensitivity (+/-)", 0.8);
                    uiManager.S_MouseJitter!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Mouse Jitter", 0);

                    uiManager.S_YOffset!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Y Offset (Up/Down)", 0);
                    uiManager.S_XOffset!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "X Offset (Left/Right)", 0);

                    uiManager.S_YOffsetPercent!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Y Offset (%)", 0);
                    uiManager.S_XOffsetPercent!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "X Offset (%)", 0);

                    uiManager.S_AutoTriggerDelay!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Auto Trigger Delay", .25);
                    uiManager.S_AIMinimumConfidence!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "AI Minimum Confidence", 50);

                    uiManager.S_EMASmoothing!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "EMA Smoothening", 0.5);

                    uiManager.S_AutoTriggerDelay!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Auto Trigger Delay", 0.1);

                    uiManager.S_DPOpacity!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Opacity", 1);
                    uiManager.S_DPCornerRadius!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Corner Radius", 0);
                    uiManager.S_DPBorderThickness!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "Border Thickness", 1);
                    uiManager.S_DPFontSize!.Slider.Value = Dictionary.GetValueOrDefault(Dictionary.sliderSettings, "AI Confidence Font Size", 20);

                }
            }

            catch (Exception e)
            {
                LogError("Error loading config " + e, true);
            }
        }
        public static void LoadAntiRecoilConfig(UI uiManager, string path = "bin\\anti_recoil_configs\\Default.cfg", bool loading_outside_startup = false)
        {
            if (File.Exists(path))
            {
                SaveDictionary.LoadJSON(Dictionary.AntiRecoilSettings, path);
                try
                {
                    if (loading_outside_startup)
                    {
                        uiManager.S_HoldTime!.Slider.Value = Dictionary.AntiRecoilSettings["Hold Time"];

                        uiManager.S_FireRate!.Slider.Value = Dictionary.AntiRecoilSettings["Fire Rate"];

                        uiManager.S_YAntiRecoilAdjustment!.Slider.Value = Dictionary.AntiRecoilSettings["Y Recoil (Up/Down)"];
                        uiManager.S_XAntiRecoilAdjustment!.Slider.Value = Dictionary.AntiRecoilSettings["X Recoil (Left/Right)"];
                        LogInfo($"[Anti Recoil] Loaded \"{path}\"", true, 2500);
                    }
                }
                catch (Exception e)
                {
                    LogError("Error loading anti-recoil config: " + e, true, 3000);
                }
            }
            else
            {
                LogWarning("[Anti Recoil] Config not found.", true, 5000);
            }
        }

        #endregion
        #region logging
        public static void LogError(string message, bool notifyUser = false, int waitingTime = 5000)
        {
            if (notifyUser)
            {
                Application.Current.Dispatcher.Invoke(() => new NoticeBar(message, waitingTime).Show());
            }

            if (Dictionary.toggleState["Debug Mode"])
            {
#if DEBUG
                Debug.WriteLine(message);
#endif
                string logFilePath = "debug.txt";
                using StreamWriter writer = new StreamWriter(logFilePath, true);
                writer.WriteLine($"[{DateTime.Now}] ERROR: {message}");
            }
        }

        public static void LogInfo(string message, bool notifyUser = false, int waitingTime = 5000)
        {
            if (notifyUser)
            {
                Application.Current.Dispatcher.Invoke(() => new NoticeBar(message, waitingTime).Show());
            }

            if (Dictionary.toggleState["Debug Mode"])
            {
#if DEBUG
                Debug.WriteLine(message);
#endif
                string logFilePath = "debug.txt";
                using StreamWriter writer = new StreamWriter(logFilePath, true);
                writer.WriteLine($"[{DateTime.Now}] INFO: {message}");
            }
        }

        public static void LogWarning(string message, bool notifyUser = false, int waitingTime = 5000)
        {
            if (notifyUser)
            {
                Application.Current.Dispatcher.Invoke(() => new NoticeBar(message, waitingTime).Show());
            }

            if (Dictionary.toggleState["Debug Mode"])
            {
#if DEBUG
                Debug.WriteLine(message);
#endif
                string logFilePath = "debug.txt";
                using StreamWriter writer = new StreamWriter(logFilePath, true);
                writer.WriteLine($"[{DateTime.Now}] WARNING: {message}");
            }
        }
        #endregion logging
    }
}