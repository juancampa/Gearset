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
    public abstract class UserInterface : IUserInterface
    {
        public event EventHandler<StreamChangedEventArgs> StreamChanged;

        public event EventHandler<LevelItemChangedEventArgs> LevelItemChanged;

        protected readonly GraphicsDevice GraphicsDevice;
        protected readonly int Width;
        protected readonly int Height;

        protected UserInterface(GraphicsDevice graphicsDevice, int width, int height)
        {
            GraphicsDevice = graphicsDevice;
            Width = width;
            Height = height;
        }

        public abstract void Initialise(ContentManager content, int width, int height);
        public abstract void CreateWidget();
        public abstract void CreateInspector(InspectorManager inspectorManager);
        public abstract void CreateLogger(LoggerConfig config);
        public abstract void CreateProfiler(ProfilerConfig config, int enabledTimeRulerLevels, int enabledPerformanceGraphLevels, int enabledProfilerSummaryLevels);
        public abstract void CreateFinder(Finder finder);
        public abstract void CreateBender(BenderConfig config);

        public abstract void Update(double deltaTime);
        public virtual void Draw(double deltaTime) { }
        
        public abstract void GotFocus();
        public abstract void Resize();
        public abstract void MoveTo(int left, int top);

        public abstract bool WidgetVisible { set; }
        public abstract bool InspectorVisible { set; }
        public abstract bool LoggerVisible { set; }
        public abstract bool ProfilerVisible { set; }
        public abstract bool FinderVisible { set; }
        public abstract bool BenderVisible { set; }

        //Widget
        public abstract void AddAction(string name, Action action);

        //Inspector
        public virtual object InspectorSelectedItem { get { return null; } }
        public virtual void AddNotice(string message, string url, string linkText) { }
        public virtual void Inspect(String name, Object o, bool autoExpand, Type t) { }
        public virtual void RemoveInspect(Object o) { }
        public virtual void Watch(InspectorNode node) { }
        public virtual void CraftMethodCall(MethodInfo info) { }
        public virtual void ClearInspectedObjects() { }
        public virtual void ClearMethods() { }
        public virtual bool FilterPredicate(object item) { return false; }

        //Logger
        public abstract void Log(string message, int updateNumber);
        public abstract void Log(string streamName, string message, int updateNumber);
        public abstract void SaveLogToFile();
        public abstract void SaveLogToFile(string filename);

        protected virtual void OnStreamChanged(StreamChangedEventArgs e)
        {
            var handler = StreamChanged;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnLevelItemChanged(LevelItemChangedEventArgs e)
        {
            var handler = LevelItemChanged;
            if (handler != null)
                handler(this, e);
        }

        //Finder
        public abstract void FinderSearch(FinderResult results);

        //Bender
        public virtual void BenderShow() { }
        public virtual void BenderFocus() { }
        public virtual void AddCurve(string name, Curve curve) { }
        public virtual void RemoveCurve(Curve curve) { }
        public virtual void RemoveCurveOrGroup(string name) { }
        public virtual float BenderHorizontalRulerNeedlePosition { get { return 0.0f; } }
    }
}
