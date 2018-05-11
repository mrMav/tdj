using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Animations
{
    /// <summary>
    /// Represents an Animation.
    /// </summary>
    public class Animation
    {
        /*
         * The Sprite parent that this animation belongs to.
         */ 
        public Sprite Parent { get; }

        /*
         * The defined name of this animation.
         */ 
        public string Name { get; set; }

        /*
         * The Frame array of this animation.
         */ 
        public Frame[] Frames { get; }

        /*
         * The delay fo the playback between frames.
         */
        public double Delay { get; set; }

        /*
         * How many times the animation has looped.
         */
        public int LoopCount { get; }

        /*
         * Flags if the animation should loop.
         */
        public bool Loop { get; set; }

        /*
         * Flags if the parent Sprite should be killed on animation complete.
         */
        public bool KillOnComplete { get; set; }

        /*
         * Flags if the animation is currently playng.
         */
        public bool IsPlaying { get; }

        /*
         * The current frame index.
         */
        public int FrameIndex { get; }

        /*
         * Current Frame
         */
        public Frame CurrentFrame { get; }

        public Animation(Sprite parent, string name, Frame[] frames, double frameRate = 60, bool loop = true)
        {

            Parent = parent;
            Name = name;

            Frames = frames;
            Delay = 1000 / frameRate;

            Loop = loop;

        }
    }
}
