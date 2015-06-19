using System;
using System.Reflection;
using Gearset.Components;
using Gearset.Components.Finder;
using Gearset.Components.InspectorWPF;
using Gearset.Components.Profiler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gearset.UserInterface
{
    public class NoUserInterface : UserInterface
    {
		public NoUserInterface(GraphicsDevice graphicsDevice, int width, int height) 
			: base (graphicsDevice, width, height) { }

		public override void Initialise(ContentManager content, int width, int height) { }
		public override void CreateWidget() { }
		public override void CreateInspector(InspectorManager inspectorManager) { }
		public override void CreateLogger(LoggerConfig config) { }
		public override void CreateProfiler(ProfilerConfig config, int enabledTimeRulerLevels, int enabledPerformanceGraphLevels, int enabledProfilerSummaryLevels)  { }
		public override void CreateFinder(Finder finder) { }
		public override void CreateBender(BenderConfig config) { }

		public override void Update(double deltaTime) { }
		public override void Draw(double deltaTime) { }
        
		public override void GotFocus() { }
		public override void Resize() { }
		public override void MoveTo(int left, int top) { }

		public override bool WidgetVisible { set { } }
		public override bool InspectorVisible { set { } }
		public override bool LoggerVisible { set { } }
		public override bool ProfilerVisible { set { } }
		public override bool FinderVisible { set { } }
		public override bool BenderVisible { set { } }

        //Widget
		public override void AddAction(string name, Action action) { }

        //Inspector
		public override object InspectorSelectedItem { get { return null; } }
		public override void AddNotice(string message, string url, string linkText) { }
		public override void Inspect(String name, Object o, bool autoExpand, Type t) { }
		public override void RemoveInspect(Object o) { }
		public override void Watch(InspectorNode node) { }
		public override void CraftMethodCall(MethodInfo info) { }
		public override void ClearInspectedObjects() { }
		public override void ClearMethods() { }
		public override bool FilterPredicate(object item) { return false; }

        //Logger
		public override void Log(string message, int updateNumber) { }
		public override void Log(string streamName, string message, int updateNumber) { }
		public override void SaveLogToFile() { }
		public override void SaveLogToFile(string filename) { }

        //Finder
		public override void FinderSearch(FinderResult results) { }

        //Bender
		public override void BenderShow() { }
		public override void BenderFocus() { }
		public override void AddCurve(string name, Curve curve) { }
		public override void RemoveCurve(Curve curve) { }
		public override void RemoveCurveOrGroup(string name) { }
		public override float BenderHorizontalRulerNeedlePosition { get { return 0.0f; } }
    }
}
