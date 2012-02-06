using System;
using Microsoft.Xna.Framework;

namespace Gearset
{
    /// <summary>
    /// This is a dummy class, it does not have any Gearset functionality. It's a dummy
    /// version of Gearset for Windows Phone and Xbox
    /// </summary>
    public class GearConsole
    {
        public Matrix WorldMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        public bool VisibleOverlays { get; set; }
        public bool Initialized { get; private set; }

        public GearConsole(Game game) { }
        public void Initialize() { Initialized = true; }
        public void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection){ }
        public void Show(String key){ }
        public void Show(String key, object value){ }
        public void AddQuickAction(String name, Action action){ }
        public void Plot(String plotName, float value){ }
        public void Plot(String plotName, float value, int historyLength){ }
        public void Log(String streamName, String content){ }
        public void Log(String content){ }
        public void Log(String streamName, String format, Object arg0) { }
        public void Log(String streamName, String format, Object arg0, Object arg1) { }
        public void Log(String streamName, String format, Object arg0, Object arg1, Object arg2){ }
        public void Log(String streamName, String format, params Object[] args){ }
        public void SaveLogToFile(){ }
        public void SaveLogToFile(string filename){ }
        public void ShowMark(String key, Vector3 position, Color color){ }
        public void ShowMark(String key, Vector3 position){ }
        public void ShowMark(String key, Vector2 position, Color color){ }
        public void ShowMark(String key, Vector2 position){ }
        public void Alert(String message){ }
        public void ShowLine(String key, Vector3 v1, Vector3 v2){ }
        public void ShowLine(String key, Vector3 v1, Vector3 v2, Color color){ }
        public void ShowLineOnce(Vector3 v1, Vector3 v2){ }
        public void ShowLineOnce(Vector3 v1, Vector3 v2, Color color){ }
        public void ShowLine(String key, Vector2 v1, Vector2 v2){ }
        public void ShowLine(String key, Vector2 v1, Vector2 v2, Color color){ }
        public void ShowLineOnce(Vector2 v1, Vector2 v2){ }
        public void ShowLineOnce(Vector2 v1, Vector2 v2, Color color){ }
        public void ShowBox(String key, BoundingBox box){ }
        public void ShowBox(String key, Vector3 min, Vector3 max){ }
        public void ShowBox(String key, BoundingBox box, Color color){ }
        public void ShowBox(String key, Vector3 min, Vector3 max, Color color){ }
        public void ShowBoxOnce(BoundingBox box){ }
        public void ShowBoxOnce(Vector3 min, Vector3 max){ }
        public void ShowBoxOnce(BoundingBox box, Color color){ }
        public void ShowBoxOnce(Vector3 min, Vector3 max, Color color){ }
        public void ShowSphere(String key, BoundingSphere sphere){ }
        public void ShowSphere(String key, Vector3 center, float radius){ }
        public void ShowSphere(String key, BoundingSphere sphere, Color color){ }
        public void ShowSphere(String key, Vector3 center, float radius, Color color){ }
        public void ShowSphereOnce(BoundingSphere sphere){ }
        public void ShowSphereOnce(Vector3 center, float radius){ }
        public void ShowSphereOnce(BoundingSphere sphere, Color color){ }
        public void ShowSphereOnce(Vector3 center, float radius, Color color){ }
        public void ShowLabel(String name, Vector2 position){ }
        public void ShowLabel(String name, Vector2 position, String text){ }
        public void ShowLabel(String name, Vector2 position, String text, Color color){ }
        public void ShowLabel(String name, Vector3 position){ }
        public void ShowLabel(String name, Vector3 position, String text){ }
        public void ShowLabel(String name, Vector3 position, String text, Color color){ }
        public void Inspect(String name, Object o){ }
        public void RemoveInspect(Object o){ }
        public void ClearInspector(){ }
        public void SetFinderSearchFunction(SearchFunction searchFunction){ }
        public void ShowTransform(String name, Matrix transform, float axisScale){ }
        public void ShowTransform(String name, Matrix transform){ }
        public void ShowTransformOnce(Matrix transform){ }
        public void ShowTransformOnce(Matrix transform, float axisScale){ }
        public void ShowVector3(String name, Vector3 location, Vector3 vector, Color color){ }
        public void ShowVector3(String name, Vector3 location, Vector3 vector){ }
        public void ShowVector3Once(Vector3 location, Vector3 vector){ }
        public void ShowVector3Once(Vector3 location, Vector3 vector, Color color){ }
        public void ShowVector2(String name, Vector2 location, Vector2 vector, Color color){ }
        public void ShowVector2(String name, Vector2 location, Vector2 vector){ }
        public void ShowVector2Once(Vector2 location, Vector2 vector){ }
        public void ShowVector2Once(Vector2 location, Vector2 vector, Color color){ }
        public void ClearAll(){ }
        public void Update(GameTime gameTime){ }
        public void Draw(GameTime gameTime) { }
    }
}

