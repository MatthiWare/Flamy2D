using OpenTK.Input;

namespace Flamy2D.Input
{
    /// <summary>
    /// Contains the state of a <see cref="OpenTK.Input.Key"/>.
    /// </summary>
    public class KeyState
    {
        /// <summary>
        /// Represents the <see cref="OpenTK.Input.Key"/> itself. 
        /// </summary>
        public Key Key { get; private set; }

        /// <summary>
        /// Represents if the key is currently down. 
        /// </summary>
        public bool IsDown { get; set; }

        /// <summary>
        /// .ctor for <see cref="KeyState"/>
        /// </summary>
        /// <param name="k">The <see cref="OpenTK.Input.Key"/></param>
        public KeyState(Key k)
        {
            Key = k;
            IsDown = false;
        }

    }
}
