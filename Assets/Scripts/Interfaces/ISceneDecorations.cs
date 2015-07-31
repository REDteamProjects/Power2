using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface ISceneDecorations
    {
        /// <summary>
        /// Accent color of all game objects
        /// </summary>
        Color AccentColor { get; set; }

        /// <summary>
        /// Accent color of all game objects
        /// </summary>
        Color ActionColor { get; set; }

        /// <summary>
        /// Update decorations of all game objects
        /// </summary>
        void UpdateDecorations();
    }
}
