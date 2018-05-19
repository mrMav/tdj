using Microsoft.Xna.Framework;
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
        public int LoopCount { get; set; }

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
        public bool IsPlaying { get; set; }

        /*
         * Flags if the animation is finished
         */
        public bool IsFinished { get; set; }

        /*
         * The current frame index.
         */
        public int FrameIndex { get; set; }

        /*
         * Current Frame
         */
        public Frame CurrentFrame { get; set; }

        /*
         * The time when the next frame plays
         */ 
        public double TimeNextFrame;


        public Animation(Sprite parent, string name, int[] indexes, double frameRate = 60, bool loop = true, bool killOnComplete = false)
        {

            Parent = parent;
            Name = name;

            Frames = MakeFrames(indexes);
            Delay = 1000 / frameRate;

            KillOnComplete = killOnComplete;

            Loop = loop;
            LoopCount = 0;
            IsPlaying = false;
            FrameIndex = 0;
            CurrentFrame = null;
            TimeNextFrame = 0;

        }

        public Animation(Sprite parent, string name, Frame[] frames, double frameRate = 60, bool loop = true, bool killOnComplete = false)
        {

            Parent = parent;
            Name = name;

            Frames = frames;
            Delay = 1000 / frameRate;

            KillOnComplete = killOnComplete;

            Loop = loop;
            LoopCount = 0;
            IsPlaying = false;
            FrameIndex = 0;
            CurrentFrame = null;
            TimeNextFrame = 0;

        }
        public void Update(GameTime gameTime)
        {

            if(gameTime.TotalGameTime.TotalMilliseconds >= TimeNextFrame && !IsFinished)
            {
                if(FrameIndex + 1 > Frames.Length - 1)
                {
                    FrameIndex = 0;
                    LoopCount++;

                } else
                {

                    FrameIndex++;
                }

                CurrentFrame = Frames[FrameIndex];

                TimeNextFrame = gameTime.TotalGameTime.TotalMilliseconds + Delay;
            }

            if(!Loop && LoopCount == 1)
            {                
                Stop();

                if(KillOnComplete)
                {
                    Parent.Alive = false;
                    Parent.Visible = false;
                }

            }

        }

        public Frame[] MakeFrames(int[] indexes)
        {
            Frame[] frames = new Frame[indexes.Length];

            for(int i = 0; i < indexes.Length; i++)
            {
                
                int x = indexes[i] % (Parent.Texture.Width / (int)Parent.Body.Bounds.Width) - 1;
                int y = indexes[i] / (Parent.Texture.Width / (int)Parent.Body.Bounds.Width);

                x *= (int)Parent.Body.Bounds.Width;
                y *= (int)Parent.Body.Bounds.Width;

                int w = (int)Parent.Body.Bounds.Width;
                int h = (int)Parent.Body.Bounds.Height;

                frames[i] = new Frame(x, y, w, h, i);

            }

            return frames;
        }

        public void Reset()
        {
            LoopCount = -1;
            IsPlaying = false;
            IsFinished = false;
            FrameIndex = Frames.Length - 1;
            CurrentFrame = null;
            TimeNextFrame = 0;
        }

        public void Stop()
        {
            IsFinished = true;
        }

        public string GetDebugInfo()
        {
            return $"Animation: {Name}, FPS: {1000 / Delay}, Frames: {Frames.Length}, CurrentFrame {FrameIndex}\nLoop: {Loop}, Loops Count: {LoopCount}\nTimeNextFrame: {TimeNextFrame}";
        }
    }
}
