// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


using System;
using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using Boku.Common;
using Boku.Fx;

namespace Boku.UI2D
{
    /// <summary>
    /// Same as UIGrid2DTextElement2 excpept uses SpriteFont instead of BitmapFont.
    /// </summary>
    public class UIGrid2DTextElement2 : UIGridElement
    {
        #region Members

        private static Effect effect = null;

        private RenderTarget2D diffuse = null;
        private Texture2D normalMap = null;
        private string normalMapName = null;

        // Properties for the underlying 9-grid geometry.
        private float width;
        private float height;
        private float edgeSize;

        private Base9Grid geometry = null;

        private Vector4 baseColor;          // Color that shows through where the texture is transparent.
        private Vector4 selectedColor;
        private Vector4 unselectedColor;
        private bool selected = false;

        private Vector4 specularColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        private float specularPower = 8.0f;

        private string label = null;
        private Color textColor;
        private Color dropShadowColor;
        private bool useDropShadow = false;
        private bool invertDropShadow = false;  // Puts the drop shadow above the regular letter instead of below.
        private Justification justify = Justification.Center;

        #endregion

        #region Accessors

        public override bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    if (value)
                    {
                        // Create a twitch to change to selected color
                        TwitchManager.Set<Vector4> set = delegate(Vector4 val, Object param) { baseColor = val; };
                        TwitchManager.CreateTwitch<Vector4>(baseColor, selectedColor, set, 0.15, TwitchCurve.Shape.EaseInOut);
                    }
                    else
                    {
                        // Create a twitch to change to unselected color.
                        TwitchManager.Set<Vector4> set = delegate(Vector4 val, Object param) { baseColor = val; };
                        TwitchManager.CreateTwitch<Vector4>(baseColor, unselectedColor, set, 0.15, TwitchCurve.Shape.EaseInOut);
                    }
                    selected = value;
                }
            }
        }
        public override string Label
        {
            get { return label; }
            set { label = TextHelper.FilterInvalidCharacters(value); }
        }
        public Color SpecularColor
        {
            set { specularColor = value.ToVector4(); }
        }
        public float SpecularPower
        {
            set { specularPower = value; }
        }
        /*
        public Vector4 BaseColor
        {
            set { baseColor = value; }
            get { return baseColor; }
        }
        */
        public Color TextColor
        {
            get { return textColor; }
        }
        public Color DropShadowColor
        {
            get { return dropShadowColor; }
        }
        public bool UseDropShadow
        {
            get { return useDropShadow; }
        }
        public bool InvertDropShadow
        {
            get { return invertDropShadow; }
        }
        public float Width
        {
            get { return width; }
        }
        public float Height
        {
            get { return height; }
        }

        /// <summary>
        /// Exposed to allow custom rendering of the texture.
        /// </summary>
        public RenderTarget2D Diffuse
        {
            get { return diffuse; }
        }

        public override Vector2 Size
        {
            get { return new Vector2(width, height); }
            set { /* do nothing, should be removed from base class */ }
        }
        
        #endregion

        #region Public

        // c'tor
        /// <summary>
        /// Simple c'tor using a blob to hold the common data.
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="label"></param>
        public UIGrid2DTextElement2(ParamBlob blob, string label)
        {
            this.label = TextHelper.FilterInvalidCharacters(label);

            // blob
            this.width = blob.width;
            this.height = blob.height;
            this.edgeSize = blob.edgeSize;
            this.selectedColor = blob.selectedColor.ToVector4();
            this.unselectedColor = blob.unselectedColor.ToVector4();
            this.baseColor = unselectedColor;

            this.Font = blob.Font;
            this.textColor = blob.textColor;
            this.useDropShadow = blob.useDropShadow;
            this.invertDropShadow = blob.invertDropShadow;
            this.dropShadowColor = blob.dropShadowColor;
            this.justify = blob.justify;

            this.normalMapName = blob.normalMapName;

        }

        /// <summary>
        /// Long form c'tor for use with no drop shadow.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="edgeSize"></param>
        /// <param name="normalMapName"></param>
        /// <param name="baseColor"></param>
        /// <param name="label"></param>
        /// <param name="justify"></param>
        /// <param name="textColor"></param>
        public UIGrid2DTextElement2(float width, float height, float edgeSize, string normalMapName, Color baseColor, string label, Shared.GetFont font, Justification justify, Color textColor)
        {
            this.width = width;
            this.height = height;
            this.edgeSize = edgeSize;
            this.baseColor = baseColor.ToVector4();

            this.normalMapName = normalMapName;

            this.Font = font;
            this.label = label;
            this.justify = justify;
            this.textColor = textColor;
            useDropShadow = false;

        }

        /// <summary>
        /// Long for c'tor for use with a drop shadow.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="edgeSize"></param>
        /// <param name="normalMapName"></param>
        /// <param name="baseColor"></param>
        /// <param name="label"></param>
        /// <param name="justify"></param>
        /// <param name="textColor"></param>
        /// <param name="dropShadowColor"></param>
        /// <param name="invertDropShadow"></param>
        public UIGrid2DTextElement2(float width, float height, float edgeSize, string normalMapName, Color baseColor, string label, Shared.GetFont font, Justification justify, Color textColor, Color dropShadowColor, bool invertDropShadow)
        {
            this.width = width;
            this.height = height;
            this.edgeSize = edgeSize;
            this.baseColor = baseColor.ToVector4();

            this.normalMapName = normalMapName;

            this.Font = font;
            this.label = label;
            this.justify = justify;
            this.textColor = textColor;
            this.dropShadowColor = dropShadowColor;
            useDropShadow = true;
            this.invertDropShadow = invertDropShadow;

        }

        public void Update()
        {
            Matrix parentMatrix = Matrix.Identity;

            base.Update(ref parentMatrix);
        }   // end of UIGrid2DTextElement2 Update()

        public override void HandleMouseInput(Vector2 hitUV)
        {
        }   // end of HandleMouseInput()



        public override void HandleTouchInput(TouchContact touch, Vector2 hitUV)
        {
        }   // end of HandleTouchInput()


        public override void Render(Camera camera)
        {
            if (diffuse == null)
            {
                effect.CurrentTechnique = effect.Techniques["NormalMappedNoTexture"];
            }
            else
            {
                effect.CurrentTechnique = effect.Techniques["NormalMapped"];
                effect.Parameters["DiffuseTexture"].SetValue(diffuse);
            }

            effect.Parameters["WorldMatrix"].SetValue(worldMatrix);
            effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix * camera.ViewProjectionMatrix);

            effect.Parameters["Alpha"].SetValue(baseColor.W);
            effect.Parameters["DiffuseColor"].SetValue(baseColor);
            effect.Parameters["SpecularColor"].SetValue(specularColor);
            effect.Parameters["SpecularPower"].SetValue(specularPower);
            effect.Parameters["NormalMap"].SetValue(normalMap);

            geometry.Render(effect);

        }   // end of UIGrid2DTextElement2 Render()

        #endregion

        #region Internal

        public override void LoadContent(bool immediate)
        {
            // Init the effect.
            if (effect == null)
            {
                effect = BokuGame.Load<Effect>(BokuGame.Settings.MediaPath + @"Shaders\UI2D");
                ShaderGlobals.RegisterEffect("UI2D", effect);
            }

            // Load the normal map texture.
            if (normalMapName != null)
            {
                normalMap = BokuGame.Load<Texture2D>(BokuGame.Settings.MediaPath + @"Textures\UI2D\" + normalMapName);
            }
        }   // end of UIGrid2DTextElement2 LoadContent()

        public override void InitDeviceResources(GraphicsDevice device)
        {
            const int dpi = 96;
            int w = (int)(dpi * width);
            int h = (int)(dpi * height);

            // Create the geometry.
            if (BokuGame.RequiresPowerOf2)
            {
                float u = (float)w / (float)MyMath.GetNextPowerOfTwo(w);
                float v = (float)h / (float)MyMath.GetNextPowerOfTwo(h);
                geometry = new Base9Grid(width, height, edgeSize, u, v);
            }
            else
            {
                geometry = new Base9Grid(width, height, edgeSize);
            }

            if (diffuse == null)
            {
                CreateRenderTargets(device);
            }

            BokuGame.Load(geometry);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            BokuGame.Release(ref effect);
            BokuGame.Release(ref normalMap);

            BokuGame.Unload(geometry);
            geometry = null;
        }   // end of UIGrid2DTextElement2 UnloadContent()

        /// <summary>
        /// Recreate render targets
        /// </summary>
        /// <param name="graphics"></param>
        public override void DeviceReset(GraphicsDevice device)
        {
            ReleaseRenderTargets();
            CreateRenderTargets(device);
        }

        private void CreateRenderTargets(GraphicsDevice device)
        {
            const int dpi = 96;
            int w = (int)(dpi * width);
            int h = (int)(dpi * height);

            int originalWidth = w;
            int originalHeight = h;
            if (BokuGame.RequiresPowerOf2)
            {
                w = MyMath.GetNextPowerOfTwo(w);
                h = MyMath.GetNextPowerOfTwo(h);
            }

            // Create the diffuse texture.  Leave it null if we have no text to render.
            diffuse = new RenderTarget2D(device, w, h, false, SurfaceFormat.Color, DepthFormat.None);
            InGame.GetRT("UIGrid2DTextElement2", diffuse);

            InGame.SetRenderTarget(diffuse);
            InGame.Clear(Color.Transparent);

            if (label != null && label.Length > 0)
            {
                // Render the label text into the texture.
                int margin = 24;
                int x = 0;
                int y = (int)((originalHeight - Font().LineSpacing) / 2.0f) - 2;
                int textWidth = (int)Font().MeasureString(label).X;

                x = TextHelper.CalcJustificationOffset(margin, w, textWidth, justify);

                SpriteBatch batch = UI2D.Shared.SpriteBatch;
                batch.Begin();
                TextHelper.DrawStringWithShadow(Font, batch, x, y, label, textColor, dropShadowColor, invertDropShadow);
                batch.End();
            }

            // Restore backbuffer.
            InGame.RestoreRenderTarget();
        }

        private void ReleaseRenderTargets()
        {
            InGame.RelRT("UIGrid2DTextElement2", diffuse);
            BokuGame.Release(ref diffuse);
        }

        #endregion

    }   // end of class UIGrid2DTextElement2

}   // end of namespace Boku.UI2D






