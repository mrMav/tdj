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
        public Frame CurrentFrame { get; }

        /*
         * Currently playing animation.
         */
        public Animation CurrentAnimation { get; }

        /*
         * Dictionary holding all the Animation objects.
         */
        public Dictionary<string, Animation> Animations { get; }

        public AnimationManager(Sprite parent, GameState state)
        {

            Parent = parent;
            State = state;

            Animations = new Dictionary<string, Animation>();

        }

        public Animation Add(string name, int[] frameIndexes, int frameRate, bool loop)
        {
            Animation anim = new Animation(Parent, name, frameIndexes, frameRate, loop);

            Animations.Add(name, anim);

            return anim;

        }
    }
}
