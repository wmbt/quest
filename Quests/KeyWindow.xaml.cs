using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Key = Common.Key;

namespace QuestClient
{
    /// <summary>
    /// Логика взаимодействия для KeyWindow.xaml
    /// </summary>
    public partial class KeyWindow : Window
    {
        private readonly Timer _closeTimer;
        public KeyWindow(Key key)
        {
            InitializeComponent();

            Description.Content = key.Description;

            if (key.Image != null && key.Image.Length > 0)
            {
                KeyImage.Source = GetBitmapImage(key.Image);
            }
            var interval = int.Parse(ConfigurationManager.AppSettings["KeyWindowInterval"]);

            _closeTimer = new Timer(1000*interval);
            _closeTimer.Elapsed += CloseTimerOnElapsed;
            Closing += OnClosing;
            _closeTimer.Start();
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _closeTimer.Dispose();
        }

        private void CloseTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _closeTimer.Stop();
            Dispatcher.Invoke(Close);
        }

        public BitmapImage GetBitmapImage(byte[] imageBytes)
        {
            var stream = new MemoryStream(imageBytes);

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }

        private void CloseOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = Canvas;
            e.Mode = ManipulationModes.Scale | ManipulationModes.Translate;
        }

        void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            //this just gets the source. 
            // I cast it to FE because I wanted to use ActualWidth for Center. You could try RenderSize as alternate
            var element = e.Source as FrameworkElement;
            if (element != null)
            {
                //e.DeltaManipulation has the changes 
                // Scale is a delta multiplier; 1.0 is last size,  (so 1.1 == scale 10%, 0.8 = shrink 20%) 
                // Rotate = Rotation, in degrees
                // Pan = Translation, == Translate offset, in Device Independent Pixels 

                var deltaManipulation = e.DeltaManipulation;
                var matrix = ((MatrixTransform)element.RenderTransform).Matrix;
                var scaleMatrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                
                // find the old center; arguaby this could be cached 
                Point center = new Point(element.ActualWidth / 2, element.ActualHeight / 2);
                // transform it to take into account transforms from previous manipulations 
                center = matrix.Transform(center);
                //this will be a Zoom. 

                /*var scaleX = deltaManipulation.Scale.X;
                var scaleY = deltaManipulation.Scale.Y;*/

                scaleMatrix.ScaleAt(deltaManipulation.Scale.X, deltaManipulation.Scale.Y, center.X, center.Y);
                if (scaleMatrix.M11 >= 1 && scaleMatrix.M11 <= 10)
                    matrix.ScaleAt(deltaManipulation.Scale.X, deltaManipulation.Scale.Y, center.X, center.Y);
                
               

                // Rotation 
                //matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);
                //Translation (pan) 

                //var translateMatrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                
                //var offsetX  = bounds.Left > 0 ?
                matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);

                element.RenderTransform = new MatrixTransform(matrix);
                var bounds = element.TransformToAncestor(Canvas)
                      .TransformBounds(new Rect(element.RenderSize));
                
                
                //element.RenderTransform = new MatrixTransform(translateMatrix);
                
                /*var offsetX = bounds.TopLeft.X;
                if (offsetX > 0)
                    matrix.Translate(-offsetX, 0);
                var offsetY = bounds.TopLeft.Y;
                if (offsetY > 0)
                    matrix.Translate(0, -offsetY);*/
                //element.RenderTransform = new MatrixTransform(matrix);
                
                /*var correctedBounds = element.TransformToAncestor(Canvas)
                      .TransformBounds(new Rect(element.RenderSize));*/

                
                    
                
                //matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                
                
                //var a = new MatrixTransform();
                
                
                

//                ((MatrixTransform)element.RenderTransform).Matrix = matrix;

                e.Handled = true;
            }
        }
    }
}
