using System;
using System.Reflection;
using Gearset.Components;
using Gearset.Components.Finder;
using Gearset.Components.InspectorWPF;
using Gearset.Components.Profiler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Gearset.UserInterface
{
    public interface IUserInterface
    {
        event EventHandler<StreamChangedEventArgs> StreamChanged;

        event EventHandler<LevelItemChangedEventArgs> LevelItemChanged;
        
        void Initialise(ContentManager content, int width, int height); 
        void CreateWidget();
        void CreateInspector(InspectorManager inspectorManager);
        void CreateLogger(LoggerConfig config);
        void CreateProfiler(ProfilerConfig config, int enabledTimeRulerLevels, int enabledPerformanceGraphLevels, int enabledProfilerSummaryLevels);
        void CreateFinder(Finder finder);
        void CreateBender(BenderConfig config);
        
        void Update(double deltaTime);
        void Draw(double deltaTime);

        void GotFocus();
        void Resize();
        void MoveTo(int left, int top);

        bool WidgetVisible { set; }
        bool InspectorVisible { set; }
        bool LoggerVisible { set; }
        bool ProfilerVisible { set; }
        bool FinderVisible { set; }
        bool BenderVisible { set; }

        //Widget
        void AddAction(string name, Action action);

        //Inspector
        object InspectorSelectedItem { get; }
        void AddNotice(string message, string url, string linkText);
        void Inspect(String name, Object o, bool autoExpand, Type t);
        void RemoveInspect(Object o);
        void Watch(InspectorNode node);
        void CraftMethodCall(MethodInfo info);
        void ClearInspectedObjects();
        void ClearMethods();
        bool FilterPredicate(object item);

        //Logger
        void Log(string message, int updateNumber);
        void Log(string streamName, string message, int updateNumber);
        void SaveLogToFile();
        void SaveLogToFile(string filename);

        //Finder
        void FinderSearch(FinderResult results);

        //Bender
        void BenderShow();
        void BenderFocus();
        void AddCurve(string name, Curve curve);
        void RemoveCurve(Curve curve);
        void RemoveCurveOrGroup(string name);
        float BenderHorizontalRulerNeedlePosition { get; }
    }
}
