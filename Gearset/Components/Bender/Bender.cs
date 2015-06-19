using Gearset.UserInterface;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Bender
{
    class Bender : Gear
    {
        readonly IUserInterface _userInterface;

        public float SeekNeedlePosition { get { return _userInterface.BenderHorizontalRulerNeedlePosition; } }
        public BenderConfig Config { get { return GearsetSettings.Instance.BenderConfig; } }

        public Bender(IUserInterface userInterface)
            : base (GearsetSettings.Instance.BenderConfig)
        {
            _userInterface = userInterface;
            _userInterface.CreateBender(Config);
        }

        protected override void OnVisibleChanged()
        {
            _userInterface.BenderVisible = Visible;
        }

        public void Show()
        {
            _userInterface.BenderShow();
        }

        public void Focus()
        {
            _userInterface.BenderFocus();
        }

        /// <summary>
        /// Adds the provided curve to Bender
        /// </summary>
        /// <param name="name">The name of the curve, a dot-separated path can be used to group curves</param>
        /// <param name="curve">The curve to add to Bender</param>
        public void AddCurve(string name, Curve curve)
        {
            _userInterface.AddCurve(name, curve);
        }

        /// <summary>
        /// Removes the provided Curve from Bender.
        /// </summary>
        public void RemoveCurve(Curve curve)
        {
            _userInterface.RemoveCurve(curve);
        }

        /// <summary>
        /// Removes a Curve or a Group by name. The complete dot-separated
        /// path to the curve or group must be given.
        /// </summary>
        public void RemoveCurveOrGroup(string name)
        {
            _userInterface.RemoveCurveOrGroup(name);
        }
    }
}
