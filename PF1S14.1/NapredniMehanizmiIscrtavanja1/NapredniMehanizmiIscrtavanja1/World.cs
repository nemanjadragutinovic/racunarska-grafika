using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using System.Diagnostics;

namespace NapredniMehanizmiIscrtavanja1
{
    /// <summary>
    ///  Nabrojani tip OpenGL podrzanih mehanizama iscrtavanja
    /// </summary>
    public enum RenderingMode
    {
        Immediate,
        DisplayList,
        VertexArray,
        IndexedVertexArray,
        VertexBufferObject,
        IndexedVertexBufferObject
    };

    ///<summary> Klasa koja enkapsulira OpenGL programski kod </summary>
    class World
    {
        #region Atributi
        /// <summary>
        ///  Izabrana OpenGL mehanizam za iscrtavanje.
        /// </summary>
        private RenderingMode m_selectedMode = RenderingMode.Immediate;

        /// <summary>
        ///  Ugao rotacije sveta oko X ose.
        /// </summary>
        float m_xRotation = 0.0f;

        /// <summary>
        ///  Ugao rotacije sveta oko Y ose.
        /// </summary>
        float m_yRotation = 0.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        /// <summary>
        ///	 Identifikator mesh modela, koristi se za iscrtavanje preko liste.
        /// </summary>
        private uint m_modelDL;

        /// <summary>
        ///	 Broj komponenti po temenu.
        /// </summary>
        private const int VERTEX_COMPONENT_COUNT = 3;

        /// <summary>
        ///	 Vertex buffer object identifikator.
        /// </summary>
        private uint[] m_modelsVBO = new uint[1];

        /// <summary>
        ///  Indexed buffer object identifikatori.
        ///  Neophodna dva identifikatora, zato sto jedan bafer sadrzi podatke o jedinstvenim koordinatama temena, 
        ///  a drugi podatke o indeksima temena.
        /// </summary>
        private uint[] m_indexedVBOs = new uint[2];

        /// <summary>
        ///	 Referenca na OpenGL instancu unutar WPF kontrole.
        /// </summary>
        private OpenGL gl;

        #endregion

        #region Properties

        /// <summary>
        ///	 Property za rendering mode.
        /// </summary>
        public RenderingMode SelectedMode
        {
            get { return m_selectedMode; }
            set { m_selectedMode = value; }
        }

        /// <summary>
        ///	 Property za rendering mode.
        /// </summary>
        public OpenGL GL
        {
            get { return gl; }
            set { gl = value; }
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
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        #endregion

        #region Metode

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            this.gl = gl;
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.ClearColor(0f, 0f, 0f, 1f);
            gl.Color(1f, 0f, 0f);
            // Podrazumevano
            gl.FrontFace(OpenGL.GL_CCW);
            // Kreiraj identifikator liste
            m_modelDL = gl.GenLists(1);

            // Kreiraj listu
            gl.NewList(m_modelDL, OpenGL.GL_COMPILE);
            DrawMeshModelPorsche(gl);
            gl.EndList();

            // Podesavanja za vertex buffer objects
            // Generisi i podesi bafer za crtanje
            gl.GenBuffers(1, m_modelsVBO);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, m_modelsVBO[0]);
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, Porsche.vertices, OpenGL.GL_STATIC_DRAW);
            // Moramo unbind-ovati ovaj bafer jer u suprotnom mogu ga "ostetiti" drugi pozivi crtanja
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);

            // Podesavanja za vertex buffer objekte uparene sa nizovima indeksa temena.
            //Neophodno je kreirati dva posebna bafer objekta (prvi sadrzi nizove koordinata temena, a drugi indekse temena)
            gl.GenBuffers(2, m_indexedVBOs);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, m_indexedVBOs[0]);
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, Porsche.uniqueVertices, OpenGL.GL_STATIC_DRAW);
            gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, m_indexedVBOs[1]);
            gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, Porsche.indicesShort, OpenGL.GL_STATIC_DRAW);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
            gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, 0);
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(int width, int height)
        {
            float nRange = 100.0f;
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            if (m_width <= m_height)
                gl.Ortho(-nRange, nRange, -nRange * m_height / m_width, nRange * m_height / m_width, -nRange, nRange);
            else
                gl.Ortho(-nRange * m_width / m_height, nRange * m_width / m_height, -nRange, nRange, -nRange, nRange);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw()
        {
            // Ocisti sadrzaj kolor bafera i bafera dubine
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.PushMatrix();
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            for (int i = -4; i <= 4; ++i)
            {
                for (int j = -4; j <= 4; ++j)
                {
                    for (int k = -4; k <= 4; ++k)
                    {
                        gl.PushMatrix();
                        gl.Translate(i * 20.0f, k * 20.0f, j * 20.0f);
                        gl.Scale(10.0f, 10.0f, 10.0f);
                        switch (m_selectedMode)
                        {
                            case RenderingMode.Immediate:
                                DrawMeshModelPorsche(gl);
                                break;

                            case RenderingMode.DisplayList:
                                gl.CallList(m_modelDL);
                                break;

                            case RenderingMode.VertexArray:
                                // Ukljuci rad sa vertex array mehanizmom i podesi nizove vertex-a
                                gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
                                gl.VertexPointer(VERTEX_COMPONENT_COUNT, 0, Porsche.vertices);
                                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, Porsche.vertices.Length / VERTEX_COMPONENT_COUNT);
                                // Iskljuci rad sa vertex array mehanizmom
                                gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
                                break;

                            case RenderingMode.IndexedVertexArray:

                                // Ukljuci rad sa vertex array mehanizmom i podesi nizove vertex-a
                                gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
                                gl.VertexPointer(VERTEX_COMPONENT_COUNT, 0, Porsche.uniqueVertices);
                                gl.DrawElements(OpenGL.GL_TRIANGLES, Porsche.vertices.Length / VERTEX_COMPONENT_COUNT, Porsche.indices);
                                // Iskljuci rad sa vertex array mehanizmom
                                gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
                                break;

                            case RenderingMode.VertexBufferObject:

                                // Ukljuci rad sa vertex array mehanizmom
                                gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
                                // Povezi vec kreirani bafer
                                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, m_modelsVBO[0]);
                                // Neophodno zbog vertex array-a - iskljucuje postojeci vertexpointer
                                gl.VertexPointer(VERTEX_COMPONENT_COUNT, OpenGL.GL_FLOAT, 0, IntPtr.Zero);
                                // Iscrtaj sadrzaj bafera
                                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, Porsche.vertices.Length / VERTEX_COMPONENT_COUNT);
                                // Unbind bafer
                                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
                                // Iskljuci rad sa vertex array mehanizmom
                                gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
                                break;

                            case RenderingMode.IndexedVertexBufferObject:

                                // Ukljuci rad sa vertex array mehanizmom
                                gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
                                // Povezi kreirane bafere
                                //Bafer sa nizom jedinstvenih temena
                                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, m_indexedVBOs[0]);
                                //Bafer sa nizom indeksa temena
                                gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, m_indexedVBOs[1]);
                                // Neophodno zbog vertex array-a - iskljucuje postojeci vertexpointer
                                gl.VertexPointer(VERTEX_COMPONENT_COUNT, OpenGL.GL_FLOAT, 0, IntPtr.Zero);
                                // Iskljucuje se postojeci pokazivac na niz sa indeksima temena.
                                gl.IndexPointer(OpenGL.GL_UNSIGNED_SHORT, 0, null);
                                // Preuzimaju se podaci iz povezanog bafer objekta, koji sadrzi niz indeksa temena.
                                // Iscrtaj sadrzaj bafera
                                gl.DrawElements(OpenGL.GL_TRIANGLES, Porsche.vertices.Length / VERTEX_COMPONENT_COUNT, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero);
                                // Unbind bafer
                                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
                                gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, 0);
                                // Iskljuci rad sa vertex array mehanizmom
                                gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
                                break;
                        }
                        gl.PopMatrix();
                    }
                }
            }
            gl.PopMatrix();
            gl.Flush();
        }

        /// <summary>
        ///  Iscrtavanje mesh modela automobila preko GL_TRIANGLES primitive.
        /// </summary>
        private void DrawMeshModelPorsche(OpenGL gl)
        {
            gl.Begin(OpenGL.GL_TRIANGLES);
            for (int i = 0; i < Porsche.vertices.Length - 3; i += 3)
            {
                gl.Vertex(Porsche.vertices[i], Porsche.vertices[i + 1], Porsche.vertices[i + 2]);
            }
            gl.End();
        }

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Destruktor.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion

        #region IDisposable Metode

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.WriteLine("Brise listu Dispose");
                gl.DeleteLists(m_modelDL, 1);
                gl.DeleteBuffers(1, m_modelsVBO);
                gl.DeleteBuffers(2, m_indexedVBOs);
            }
        }

        #endregion
    }
}
