using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL.SceneGraph;
using SharpGL;

namespace NapredniMehanizmiIscrtavanja1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///  Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World();
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta", "GRESKA", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw();
        }            

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;
            m_world.Initialize(gl);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize((int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            renderingTypeComboBox.ItemsSource = Enum.GetValues(typeof(RenderingMode));
        }

        private void renderingTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_world.SelectedMode = (RenderingMode)renderingTypeComboBox.SelectedItem;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.F10: this.Close(); break;
                case System.Windows.Input.Key.W: m_world.RotationX -= 5.0f; break;
                case System.Windows.Input.Key.S: m_world.RotationX += 5.0f; break;
                case System.Windows.Input.Key.A: m_world.RotationY -= 5.0f; break;
                case System.Windows.Input.Key.D: m_world.RotationY += 5.0f; break;
            }
        }

    }
}
