// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Boku.Common.ParticleSystem
{
    // Emits "Explosion" as a series of expanding, rotating 
    // sprites which fade and grow as they age.  The sprites
    // are rendered using additive blending.
    public class InspectExplosionEmitter : BaseSpriteEmitter
    {
        private static Texture2D texture = null;

        #region accessors
        protected override Texture2D Texture
        {
            get { return texture; }
        }
        public override ParticleSystemManager.EffectTech2d TechniqueName
        {
            get { return ParticleSystemManager.EffectTech2d.TexturedColorPassOneOneBlend; }
        }
        /// <summary>
        /// Keep a dark lightrig from turning explosions black.
        /// </summary>
        public override bool IsEmissive
        {
            get { return true; }
        }

        #endregion

        // c'tor
        public InspectExplosionEmitter(ParticleSystemManager manager)
            : base(manager)
        {
            Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            StartRadius = 0.0f;
            EndRadius = 1.0f;
            StartAlpha = 1.0f;
            EndAlpha = 0.0f;
            MinLifetime = 0.2f;     // Particle lifetime.
            MaxLifetime = 1.0f;
            EmissionRate = 100.0f;  // Particles per second.

            MaxRotationRate = 2.0f;
        }   // end of c'tor

        new public static void LoadContent(bool immediate)
        {
            // Load the texture.
            if (InspectExplosionEmitter.texture == null)
            {
                InspectExplosionEmitter.texture = BokuGame.Load<Texture2D>(BokuGame.Settings.MediaPath + @"Textures/InspectFire");
            }
        }   // end of ExplosionEmitter LoadContent()

        new public static void InitDeviceResources(GraphicsDevice device)
        {
        }

        new public static void UnloadContent()
        {
            InspectExplosionEmitter.texture = null;
        }   // end of ExplosionEmitter UnloadContent()

        new public static void DeviceReset(GraphicsDevice device)
        {
        }

    }   // end of class InspectExplosionEmitter

    // Emits "Explosion" as a series of expanding, rotating 
    // sprites which fade and grow as they age.  The sprites
    // are rendered using additive blending.
    public class InspectSmokeEmitter : BaseSpriteEmitter
    {
        private static Texture2D texture = null;

        #region accessors
        protected override Texture2D Texture
        {
            get { return texture; }
        }
        #endregion

        // c'tor
        public InspectSmokeEmitter(ParticleSystemManager manager)
            : base(manager)
        {
            Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            StartRadius = 0.0f;
            EndRadius = 1.0f;
            StartAlpha = 1.0f;
            EndAlpha = 0.0f;
            MinLifetime = 0.2f;     // Particle lifetime.
            MaxLifetime = 1.0f;
            EmissionRate = 100.0f;  // Particles per second.

            MaxRotationRate = 2.0f;
        }   // end of c'tor

        new public static void LoadContent(bool immediate)
        {
            // Load the texture.
            if (InspectSmokeEmitter.texture == null)
            {
                InspectSmokeEmitter.texture = BokuGame.Load<Texture2D>(BokuGame.Settings.MediaPath + @"Textures/InspectSmoke");
            }
        }   // end of ExplosionEmitter LoadContent()

        new public static void InitDeviceResources(GraphicsDevice device)
        {
        }

        new public static void UnloadContent()
        {
            InspectSmokeEmitter.texture = null;
        }   // end of ExplosionEmitter UnloadContent()

        new public static void DeviceReset(GraphicsDevice device)
        {
        }

    }   // end of class InspectSmokeEmitter

}   // end of namespace Boku.Common

