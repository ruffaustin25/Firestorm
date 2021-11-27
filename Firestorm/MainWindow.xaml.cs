using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Firestorm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WriteableBitmap Bitmap;

        public MainWindow()
        {
            InitializeComponent();

            RoutedCommand saveCommand = new RoutedCommand();
            saveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control, "Save"));
            CommandBindings.Add(new CommandBinding(saveCommand, SaveCommandExecuted));

            Bitmap = new WriteableBitmap(1280, 720, 72, 72, PixelFormats.Bgra32, null);
            MainImage.Source = Bitmap;
        }

        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void LoadCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void DrawPixel(MouseEventArgs e)
        {
            int column = (int)(e.GetPosition(MainImage).X);
            int row = (int)(e.GetPosition(MainImage).Y);

            try
            {
                // Reserve the back buffer for updates.
                Bitmap.Lock();

                for (int x = 0; x < 32; ++x)
                {
                    for (int y = 0; y < 32; ++y)
                    {
                        unsafe
                        {
                            // Get a pointer to the back buffer.
                            IntPtr pBackBuffer = Bitmap.BackBuffer;

                            // Find the address of the pixel to draw.
                            pBackBuffer += (row + y) * Bitmap.BackBufferStride;
                            pBackBuffer += (column + x) * 4;

                            // Compute the pixel's color.
                            int color_data = 255 << 24; // A
                            color_data |= 0 << 16;   // R
                            color_data |= 0 << 8;   // G
                            color_data |= 0 << 0;   // B

                            // Assign the color data to the pixel.
                            *((int*)pBackBuffer) = color_data;
                        }
                    }
                }

                // Specify the area of the bitmap that changed.
                Bitmap.AddDirtyRect(new Int32Rect(column, row, 32, 32));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                Bitmap.Unlock();
            }
        }

        void ErasePixel(MouseEventArgs e)
        {
            byte[] ColorData = { 0, 0, 0, 0 }; // B G R

            Int32Rect rect = new Int32Rect(
                    (int)(e.GetPosition(MainImage).X),
                    (int)(e.GetPosition(MainImage).Y),
                    1,
                    1);

            Bitmap.WritePixels(rect, ColorData, 4, 0);
        }

        private void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DrawPixel(e);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                ErasePixel(e);
            }
        }

        private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DrawPixel(e);
        }

        private void MainImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ErasePixel(e);
        }
    }
}
