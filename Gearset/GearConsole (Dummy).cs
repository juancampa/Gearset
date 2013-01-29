using System;
using Microsoft.Xna.Framework;

namespace Gearset
{
    /// <summary>
    /// This is a dummy class, it does not have any Gearset functionality. It's a dummy
    /// version of Gearset for Windows Phone and Xbox. Use the FunctionExtractor to autogenerate it.
    /// </summary>
    public class GearConsole
    {
        public Matrix WorldMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        public bool VisibleOverlays { get; set; }
        public Matrix Transform2D { get; set; }
        public float BenderNeedlePosition { get { return 0; } }
        public bool Initialized { get; private set; }

        public GearConsole(Game game) { }
        public void Initialize() { Initialized = true; }
        public void Update(GameTime gameTime) { }
        public void Draw(GameTime gameTime) { }
        public void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection) { }

        #region Wrappers for Gearset methods
        void Show(String key) { }
        void Show(String key, object value) { }
        void AddQuickAction(String name, Action action) { }
        void Plot(String plotName, float value) { }
        void Plot(String plotName, float value, int historyLength) { }
        void Log(String streamName, String content) { }
        void Log(String content) { }
        void Log(String streamName, String format, Object arg0)  { }
        void Log(String streamName, String format, Object arg0, Object arg1)  { }
        void Log(String streamName, String format, Object arg0, Object arg1, Object arg2) { }
        void Log(String streamName, String format, params Object[] args) { }
        void SaveLogToFile() { }
        void SaveLogToFile(string filename) { }
        void ShowMark(String key, Vector3 position, Color color) { }
        void ShowMark(String key, Vector3 position) { }
        void ShowMark(String key, Vector2 position, Color color) { }
        void ShowMark(String key, Vector2 position) { }
        void Alert(String message) { }
        void ShowLine(String key, Vector3 v1, Vector3 v2) { }
        void ShowLine(String key, Vector3 v1, Vector3 v2, Color color) { }
        void ShowLineOnce(Vector3 v1, Vector3 v2) { }
        void ShowLineOnce(Vector3 v1, Vector3 v2, Color color) { }
        void ShowLine(String key, Vector2 v1, Vector2 v2) { }
        void ShowLine(String key, Vector2 v1, Vector2 v2, Color color) { }
        void ShowLineOnce(Vector2 v1, Vector2 v2) { }
        void ShowLineOnce(Vector2 v1, Vector2 v2, Color color) { }
        void ShowBox(String key, BoundingBox box) { }
        void ShowBox(String key, Vector3 min, Vector3 max) { }
        void ShowBox(String key, BoundingBox box, Color color) { }
        void ShowBox(String key, Vector3 min, Vector3 max, Color color) { }
        void ShowBoxOnce(BoundingBox box) { }
        void ShowBoxOnce(Vector3 min, Vector3 max) { }
        void ShowBoxOnce(BoundingBox box, Color color) { }
        void ShowBoxOnce(Vector3 min, Vector3 max, Color color) { }
        void ShowSphere(String key, BoundingSphere sphere) { }
        void ShowSphere(String key, Vector3 center, float radius) { }
        void ShowSphere(String key, BoundingSphere sphere, Color color) { }
        void ShowSphere(String key, Vector3 center, float radius, Color color) { }
        void ShowSphereOnce(BoundingSphere sphere) { }
        void ShowSphereOnce(Vector3 center, float radius) { }
        void ShowSphereOnce(BoundingSphere sphere, Color color) { }
        void ShowSphereOnce(Vector3 center, float radius, Color color) { }
        void ShowLabel(String name, Vector2 position) { }
        void ShowLabel(String name, Vector2 position, String text) { }
        void ShowLabel(String name, Vector2 position, String text, Color color) { }
        void ShowLabel(String name, Vector3 position) { }
        void ShowLabel(String name, Vector3 position, String text) { }
        void ShowLabel(String name, Vector3 position, String text, Color color) { }
        void Inspect(String name, Object o) { }
        void Inspect(String name, Object o, bool autoExpand) { }
        void RemoveInspect(Object o) { }
        void ClearInspector() { }
        void SetFinderSearchFunction(SearchFunction searchFunction) { }
        void ShowTransform(String name, Matrix transform, float axisScale) { }
        void ShowTransform(String name, Matrix transform) { }
        void ShowTransformOnce(Matrix transform) { }
        void ShowTransformOnce(Matrix transform, float axisScale) { }
        void ShowVector3(String name, Vector3 location, Vector3 vector, Color color) { }
        void ShowVector3(String name, Vector3 location, Vector3 vector) { }
        void ShowVector3Once(Vector3 location, Vector3 vector) { }
        void ShowVector3Once(Vector3 location, Vector3 vector, Color color) { }
        void ShowVector2(String name, Vector2 location, Vector2 vector, Color color) { }
        void ShowVector2(String name, Vector2 location, Vector2 vector) { }
        void ShowVector2Once(Vector2 location, Vector2 vector) { }
        void ShowVector2Once(Vector2 location, Vector2 vector, Color color) { }
        void AddCurve(String name, Curve curve) { }
        void RemoveCurve(Curve curve) { }
        void RemoveCurveOrGroup(String name) { }
        void ClearAll() { }

        #endregion
    }
}

