using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Animations
{
    /// <summary>
    /// Animation Manager for Sprite objects.
    /// </summary>
    public class AnimationManager
    {

        /*
         * The Sprite parent that this manager belongs to.
         */
        public Sprite Parent { get; }

        /*
         * A reference to the running state.
         */
        public GameState State { get; }

        /*
         * Currently displayed Frame.
         */
        public Frame CurrentFrame { get; set; }

        /*
         * Currently playing animation.
         */
        public Animation CurrentAnimation { get; set; }

        /*
         * Dictionary holding all the Animation objects.
         */
        public Dictionary<string, Animation> Animations { get; }

        public AnimationManager(Sprite parent, GameState state)
        {

            Parent = parent;
            State = state;

            Animations = new Dictionary<string, Animation>();

            CurrentFrame = null;
            CurrentAnimation = null;

        }

        public void SetCurrentFrame(Frame frame)
        {

            if(CurrentAnimation != null)
            {
                CurrentAnimation.Stop();
            }

            CurrentFrame = frame;

        }

        public Animation Add(string name, int[] frameIndexes, int frameRate, bool loop, bool killoncomplete = false)
        {
            Animation anim = new Animation(Parent, name, frameIndexes, frameRate, loop, killoncomplete);

            Animations.Add(name, anim);

            return anim;

        }
        public Animation Add(string name, Frame[] frames, int frameRate, bool loop, bool killoncomplete = false)
        {
            Animation anim = new Animation(Parent, name, frames, frameRate, loop, killoncomplete);

            Animations.Add(name, anim);

            return anim;

        }


        public Animation Play(string key)
        {
            Animation anim;

            if (Animations.TryGetValue(key, out anim))
            {
                //Console.WriteLine("playing anim " + key);

                CurrentAnimation = anim;
                CurrentAnimation.Reset();

                return anim;
            }

            return null;

        }

        public void Update(GameTime gameTime)
        {

            if(CurrentAnimation != null)
            {
                //Console.WriteLine("updating anim" + CurrentAnimation.Name);

                CurrentAnimation.Update(gameTime);

                CurrentFrame = CurrentAnimation.CurrentFrame;
                
            }

        }

    }
}
