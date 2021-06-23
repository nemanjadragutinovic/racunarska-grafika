// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 50.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);
            m_scene.LoadScene();
            m_scene.Initialize();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);


            gl.PushMatrix();
            gl.Translate(0.0f, -0.01f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            
            

            m_scene.Draw();
            
            

            gl.PushMatrix();
            gl.Color(0.9f, 0.95f, 1.0f);
            gl.Scale(1f, 1f, 1f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Begin(OpenGL.GL_QUADS);

            gl.Vertex(-17.0f, 0f, -10.0f);

            gl.Vertex(-17.0f, 0f, 13.0f);

            gl.Vertex(17.0f, 0f, 13.0f);

            gl.Vertex(17.0f, 0f, -10.0f);
            gl.End();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.1f, 0.53f, 0.9f);
            gl.Scale(1f, 1f, 1f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Begin(OpenGL.GL_QUADS);

            gl.Vertex(5.0f, 0.01f, -3.0f);

            gl.Vertex(5.0f, 0.01f, 4.0f);

            gl.Vertex(12.0f, 0.01f, 4.0f);

            gl.Vertex(12.0f, 0.01f, -3.0f);
            gl.End();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Rotate(-90, 0, 0);

            gl.Translate(-12f, 0, 0);
            gl.Color(0.8f, 0.89f, 1.0f);



            Cylinder cil = new Cylinder();
            cil.TopRadius =  4;
            
            cil.Height = 7;
            cil.QuadricDrawStyle = DrawStyle.Fill;
            

            cil.BaseRadius = 4;
            cil.CreateInContext(gl);
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);


            gl.PopMatrix();

            gl.PushMatrix();
            
            
            gl.Translate(-12f, 7, 0);

            Sphere sp = new Sphere();
            sp.Radius = 4f;
            sp.CreateInContext(gl);
            sp.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();


            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            DrawText(gl);
            gl.Flush();
        }

        public void DrawText(OpenGL gl)
        {

            gl.PushMatrix();
            String[] ispis = {  "Predmet: Racunarska grafika",
                                "Sk.god: 2020/21",
                                "Ime: Nemanja",
                                "Prezime: Dragutinovic",
                                "Sifra zad: 14.1"
                             };
            for (int i = 0; i < ispis.Length; i++)
            {
                gl.PushMatrix();
                gl.Ortho2D(0, 10, 0, 10);
                gl.Translate(15f, 26f - i, 10);
                gl.Color(0.3f, 0.3f, 0.3f);



                gl.DrawText3D("Arial Bold", 10f, 1f, 0.05f, ispis[i]);

                

                gl.PopMatrix();
            }
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(50f, (double)m_width / m_height, 0.5f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.FrontFace(OpenGL.GL_CCW);

            gl.PopMatrix();


        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(50f, (double)width / height, 0.5f, 20000f);
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
