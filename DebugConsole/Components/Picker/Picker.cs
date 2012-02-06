using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    /// <summary>
    /// Class that handles picking objects from the screen
    /// </summary>
    internal class Picker : Gear
    {

        private List<IPickable> Pickables;
        private IPickable HoveringObject;
        private IPickable SelectedObject;

        public IPickable Picked { get; private set; }

        public Picker()
            : base(new GearConfig())
        {
            Pickables = new List<IPickable>();
        }

        /// <summary>
        /// Adds a new Pickable so it can be picked with
        /// the mouse.
        /// </summary>
        /// <param name="pickable"></param>
        public void AddPickable(IPickable pickable)
        {
            Pickables.Add(pickable);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            //  Unproject the screen space mouse coordinate into model space 
            //  coordinates. Because the world space matrix is identity, this 
            //  gives the coordinates in world space.
            Viewport vp = GearsetResources.Game.GraphicsDevice.Viewport;
            //  Note the order of the parameters! Projection first.
            Vector3 pos1 = vp.Unproject(new Vector3(GearsetResources.Mouse.Position, 0), GearsetResources.Projection, GearsetResources.View, Matrix.Identity);
            Vector3 pos2 = vp.Unproject(new Vector3(GearsetResources.Mouse.Position, 1), GearsetResources.Projection, GearsetResources.View, Matrix.Identity);
            Vector3 dir = Vector3.Normalize(pos2 - pos1);

            // Cast a ray and check if we've hit anything
            IPickable closestPickable = null;
            float? distanceToClosest = null;
            Ray ray = new Ray(pos1, dir);
            foreach (IPickable pickable in Pickables)
            {
                float? dist = new float?();
                if (pickable is IPickable<BoundingBox>)
                {
                    IPickable<BoundingBox> p = pickable as IPickable<BoundingBox>;
                    BoundingBox v = p.PickableVolume; 
                    ray.Intersects(ref v, out dist);
                }
                else if (pickable is IPickable<BoundingSphere>)
                {
                    IPickable<BoundingSphere> p = pickable as IPickable<BoundingSphere>;
                    BoundingSphere v = p.PickableVolume;
                    ray.Intersects(ref v, out dist);
                }
                else if (pickable is IPickable<Plane>)
                {
                    IPickable<Plane> p = pickable as IPickable<Plane>;
                    Plane v = p.PickableVolume;
                    ray.Intersects(ref v, out dist);
                }
                else if (pickable is IPickable<Rectangle>)
                {
                    IPickable<Rectangle> p = pickable as IPickable<Rectangle>;
                    if (p.PickableVolume.Contains(new Point((int)GearsetResources.Mouse.Position.X, (int)GearsetResources.Mouse.Position.Y)))
                        dist = 0;
                }

                if (dist.HasValue && (!distanceToClosest.HasValue ||dist.Value < distanceToClosest) && SelectedObject != pickable)
                {
                    distanceToClosest = dist.Value;
                    closestPickable = pickable;
                }
            }

            // Check if we actually hit something
            if (distanceToClosest.HasValue)
            {
                HoveringObject = closestPickable;

                if (GearsetResources.Mouse.IsLeftJustDown())
                {
                    Picked = closestPickable;

                    // Let the object know that it has been picked.
                    Picked.Picked();

                    SelectedObject = Picked;
                }
            }
            else
            {
                HoveringObject = null;
            }

        }

        public override void Draw(GameTime gameTime)
        {
            // Only draw if we're doing a BasicEffectPass pass
            if (GearsetResources.CurrentRenderPass != RenderPass.BasicEffectPass) return;

            if (HoveringObject is IPickable<BoundingBox>)
                BoundingBoxHelper.DrawBoundingBox(((IPickable<BoundingBox>)HoveringObject).PickableVolume, Color.Gray);
            if (SelectedObject is IPickable<BoundingBox>)
                BoundingBoxHelper.DrawBoundingBox(((IPickable<BoundingBox>)SelectedObject).PickableVolume, Color.White);
        }
    }
}
