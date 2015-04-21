using Gearset.Components.Profiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset;
using System.ComponentModel;
using Gearset.Components;
using System.IO;
#if WINDOWS
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
#endif

namespace Gearset
{
    /// <summary>
    /// This class holds the settings of Gearset.
    /// </summary>
    [Serializable]
    public class GearsetSettings : INotifyPropertyChanged
    {
        [Inspector(FriendlyName = "Master Switch")]
        public bool Enabled { get { return enabled; } set { enabled = value; OnPropertyChanged("Enabled"); } }
        private bool enabled;

        [Inspector(FriendlyName = "Show Overlays (Ctrl + Space)", HideCantWriteIcon=true)]
        public bool ShowOverlays { get { return showOverlays; } set { showOverlays = value; OnPropertyChanged("ShowOverlays"); } }
        private bool showOverlays;

        [Inspector(FriendlyName = "Enable Depth Buffer for 3D overlays")]
        public bool DepthBufferEnabled { get; set; }

        [Inspector(FriendlyName = "Save Frequency (secs)")]
        public float SaveFrequency
        {
            get { return saveFrequency; }
            set { saveFrequency = Math.Max(value, 2); }
        }
        private float saveFrequency;

#if WINDOWS
        [Inspector(FriendlyName = "Inspector", HideCantWriteIcon = true)]
        public InspectorConfig InspectorConfig { get; internal set; }

        [Inspector(FriendlyName = "Logger", HideCantWriteIcon = true)]
        public LoggerConfig LoggerConfig { get; internal set; }

        [Inspector(FriendlyName = "Finder", HideCantWriteIcon = true)]
        public FinderConfig FinderConfig { get; set; }

        [Inspector(FriendlyName = "Bender", HideCantWriteIcon = true)]
        public BenderConfig BenderConfig { get; set; }
#endif

        [Inspector(FriendlyName = "Profiler", HideCantWriteIcon = true)]
        public ProfilerConfig ProfilerConfig { get; internal set; }

        [Inspector(FriendlyName = "Overlaid Plots", HideCantWriteIcon = true)]
        public PlotterConfig PlotterConfig { get; internal set; }

        [Inspector(FriendlyName = "Overlaid Tree View", HideCantWriteIcon = true)]
        public TreeViewConfig TreeViewConfig { get; internal set; }

        [Inspector(FriendlyName = "Overlaid Labels", HideCantWriteIcon = true)]
        public LabelerConfig LabelerConfig { get; internal set; }

        [Inspector(FriendlyName = "Overlaid Geometry", HideCantWriteIcon = true)]
        public LineDrawerConfig LineDrawerConfig { get; internal set; }

        [Inspector(FriendlyName = "Overlaid Alerts", HideCantWriteIcon = true)]
        public AlerterConfig AlerterConfig { get; private set; }

        [InspectorIgnore]
        public DataSamplerConfig DataSamplerConfig { get; internal set; }

        /// <summary>
        /// Actual settings.
        /// </summary>
        public static GearsetSettings Instance { get; private set; }

        public GearsetSettings()
        {
            Enabled = true;
            ShowOverlays = true;
#if WINDOWS
            InspectorConfig = new Components.InspectorConfig();
            LoggerConfig = new Components.LoggerConfig();
#endif
            ProfilerConfig = new ProfilerConfig();
            DataSamplerConfig = new Components.DataSamplerConfig();
            PlotterConfig = new Components.PlotterConfig();
            TreeViewConfig = new Components.TreeViewConfig();
            LabelerConfig= new Components.LabelerConfig();
            LineDrawerConfig = new Components.LineDrawerConfig();
            AlerterConfig = new Components.AlerterConfig();
#if WINDOWS
            FinderConfig = new Components.FinderConfig();
#endif

            // IMPORTANT:
            // NEW CONFIG INSTANCES SHOULD BE ADDED IN THE LOAD METHOD BELOW.
            DepthBufferEnabled = true;
            saveFrequency = 5;
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        /// <summary>
        /// Saves the current state of the configuration.
        /// </summary>
        internal static void Save()
        {
#if WINDOWS
            try
            {
                using (MemoryStream memFile = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memFile, Instance);

                    // If the file serialized correctly to the memfile, dump it to an actual file.
                    using (FileStream file = new FileStream("gearset.conf", FileMode.Create))
                    {
                        file.Write(memFile.GetBuffer(), 0, (int)memFile.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Gearset settings could not be saved: " + e.Message);
            }
#endif
        }

        /// <summary>
        /// Loads a saved configuration
        /// </summary>
        internal static void Load()
        {
#if WINDOWS
            try
            {
                if (File.Exists("gearset.conf"))
                {
                    using (FileStream file = new FileStream("gearset.conf", FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        Instance = formatter.Deserialize(file) as GearsetSettings;
                    }
                }
                else
                {
                    Console.WriteLine("No gearset.conf file found, using a fresh config.");
                    Instance = new GearsetSettings();
                }
            }
            catch
            {
                Console.WriteLine("Problem while reading gearset.conf, using a fresh config.");
                Instance = new GearsetSettings();
            }
#else
            Instance = new GearsetSettings();
#endif
            InitializeSettingsIntroducedAfterV1();
        }

        private static void InitializeSettingsIntroducedAfterV1()
        {
#if WINDOWS
            // Here we should check Configs added after 1st release to permit backward
            // compatibility with old gearset.conf files.
            if (Instance.FinderConfig == null)
                Instance.FinderConfig = new FinderConfig();

            if (Instance.LoggerConfig.HiddenStreams == null)
                Instance.LoggerConfig.HiddenStreams = new List<string>();

            if (Instance.BenderConfig == null)
                Instance.BenderConfig = new BenderConfig();
#endif

            if (Instance.PlotterConfig.HiddenPlots == null)
                Instance.PlotterConfig.HiddenPlots = new List<string>();

            if (Instance.ProfilerConfig == null)
                Instance.ProfilerConfig = new ProfilerConfig();

            if (Instance.ProfilerConfig.TimeRulerConfig == null)
                Instance.ProfilerConfig.TimeRulerConfig = new ProfilerConfig.TimeRulerUIViewConfig();

            if (Instance.ProfilerConfig.PerformanceGraphConfig == null)
                Instance.ProfilerConfig.PerformanceGraphConfig = new ProfilerConfig.PerformanceGraphUIViewConfig();
        }
    }
}
