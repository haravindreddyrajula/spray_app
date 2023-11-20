using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;
using Newtonsoft.Json;
using Path = System.IO.Path;
using System.ComponentModel;

namespace sprayapp1
{

    #region model classes
    public class ImageDataModel
    {
        public Uri ImageUri { get; set; }
        public List<SprayPaintModel> SprayPaintData { get; set; }
    }

    public class SprayPaintModel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Color Color { get; set; }
        public double Opacity { get; set; }
    }
    #endregion

    // view model region
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Uri _imageUri;
        public Uri ImageUri
        {
            get { return _imageUri; }
            set
            {
                if (_imageUri != value)
                {
                    _imageUri = value;
                    OnPropertyChanged(nameof(ImageUri));
                }
            }
        }

        private List<SprayPaintModel> _sprayPaintDataList;
        public List<SprayPaintModel> SprayPaintDataList
        {
            get { return _sprayPaintDataList; }
            set
            {
                if (_sprayPaintDataList != value)
                {
                    _sprayPaintDataList = value;
                    OnPropertyChanged(nameof(SprayPaintDataList));
                }
            }
        }

        private bool _isSpraying;
        public bool IsSpraying
        {
            get { return _isSpraying; }
            set
            {
                if (_isSpraying != value)
                {
                    _isSpraying = value;
                    OnPropertyChanged(nameof(IsSpraying));
                }
            }
        }

        private bool _isErasing;
        public bool IsErasing
        {
            get { return _isErasing; }
            set
            {
                if (_isErasing != value)
                {
                    _isErasing = value;
                    OnPropertyChanged(nameof(IsErasing));
                }
            }
        }

        private Random _random = new Random();

        public Random Random
        {
            get { return _random; }
            set
            {
                if (_random != value)
                {
                    _random = value;
                    OnPropertyChanged(nameof(Random));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public partial class MainWindow : Window
    {

        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //CLearing the previous work
                Canvas?.Children?.Clear();

                // Open a dialog box to select an image file
                OpenFileDialog openFileDialog = new OpenFileDialog();

                //The Filter property of the OpenFileDialog is set to allow the user to select only image files with extensions .png, .jpeg and .jpg.
                openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

                //The InitialDirectory property is set to the "My Pictures" folder of the user's 
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                List<SprayPaintModel> sprayPaintDataList = new List<SprayPaintModel>();

                // If the user selects a file, the ShowDialog method returns true.
                if (openFileDialog.ShowDialog() == true)
                {

                    // Load the image URI from the JSON file (if it exists)
                    string jsonFilePath = Path.ChangeExtension(openFileDialog.FileName, "json");

                    if (File.Exists(jsonFilePath))
                    {
                        // reading the file
                        string jsonData = File.ReadAllText(jsonFilePath);
                        try
                        {

                            // deserilizing
                            ImageDataModel data = JsonConvert.DeserializeObject<ImageDataModel>(jsonData);

                            _viewModel.ImageUri =  (Uri) data.ImageUri;
                            sprayPaintDataList = (List<SprayPaintModel>) data.SprayPaintData;


                        }
                        catch (JsonException ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                        catch (ArgumentNullException ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        _viewModel.ImageUri = new Uri(openFileDialog.FileName);
                    }

                    string filePath = openFileDialog.FileName;

                    // Extracting only the file name from the full path
                    string fileName = Path.GetFileName(filePath);

                    FilePathText.Text = "Selected File: " + fileName;

                    // Load the selected image file into a BitmapImage object
                    BitmapImage bitmapImage = new BitmapImage(_viewModel.ImageUri);

                    // Create a new Image object and set its Source property to the BitmapImage object
                    System.Windows.Controls.Image image = new Image();
                    image.Source = bitmapImage;

                    // Add the Image object as a child of the Canvas object
                    Canvas.Children.Add(image);

                    // Calculate the aspect ratio of the selected image and the ScrollViewer object
                    double widthRatio = ScrollViewer.ActualWidth / bitmapImage.PixelWidth;
                    double heightRatio = ScrollViewer.ActualHeight / bitmapImage.PixelHeight;
                    double ratio = Math.Min(widthRatio, heightRatio);

                    // Resize the Canvas object to fit the selected image while maintaining its aspect ratio
                    Canvas.Width = bitmapImage.PixelWidth * ratio;
                    Canvas.Height = bitmapImage.PixelHeight * ratio;

                    // Set the Width and Height properties of the Image object to match the Canvas object
                    image.Width = Canvas.Width;
                    image.Height = Canvas.Height;

                    // Apply spray paint data to the Canvas
                    foreach (SprayPaintModel sprayPaintData in sprayPaintDataList)
                    {
                        Ellipse ellipse = new Ellipse
                        {
                            Width = sprayPaintData.Width,
                            Height = sprayPaintData.Height,
                            Fill = new SolidColorBrush(sprayPaintData.Color) { Opacity = sprayPaintData.Opacity }
                        };

                        // Set position on the Canvas
                        Canvas.SetLeft(ellipse, sprayPaintData.X);
                        Canvas.SetTop(ellipse, sprayPaintData.Y);

                        // Add the ellipse to the Canvas
                        Canvas.Children.Add(ellipse);
                    }

                }
            }
            catch (ArgumentException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void SprayPaintCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.IsSpraying = true;
        }

        private void SprayPaintCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.IsSpraying = false;
        }

        private void SprayPaintCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if ((_viewModel.IsSpraying || _viewModel.IsErasing) && e.LeftButton == MouseButtonState.Pressed)
                {
                    double thickness = ThicknessSlider.Value;
                    double opacity = _viewModel.IsErasing ? 0 : 1; // Setting opacity to 0 when erasing
                    Color selectedColor = _viewModel.IsErasing ? Colors.Transparent : GetSelectedColor();

                    double offsetX = _viewModel.Random.NextDouble() * 20 - 10;
                    double offsetY = _viewModel.Random.NextDouble() * 20 - 10;

                    Point relativePosition = e.GetPosition(Canvas);
                    Point absolutePosition = Canvas.PointToScreen(relativePosition);
                    Point positionInImage = Canvas.PointFromScreen(absolutePosition);

                    Ellipse ellipse = new Ellipse
                    {
                        Width = thickness,
                        Height = thickness,
                        Fill = new SolidColorBrush(selectedColor) { Opacity = opacity }
                    };

                    // Setting position relative to the image
                    Canvas.SetLeft(ellipse, positionInImage.X + offsetX);
                    Canvas.SetTop(ellipse, positionInImage.Y + offsetY);

                    Canvas.Children.Add(ellipse);

                    // If in erase mode, removing existing strokes
                    if (_viewModel.IsErasing)
                    {
                        HitTestResult hit = VisualTreeHelper.HitTest(Canvas, positionInImage);
                        if (hit != null)
                        {
                            UIElement hitElement = hit.VisualHit as UIElement;
                            if (hitElement != null && hit.VisualHit is Ellipse)
                            {
                                Canvas.Children.Remove(hitElement);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }


        }

        private void SaveSprayPaintData(SaveFileDialog saveFileDialog)
        {
            try
            {
                //  a list to store spray paint data
                List<SprayPaintModel> sprayPaintDataList = new List<SprayPaintModel>();

                // Iterating through Canvas children and saving relevant information
                foreach (UIElement element in Canvas.Children)
                {
                    if (element is Ellipse ellipse)
                    {
                        SprayPaintModel sprayPaintData = new SprayPaintModel
                        {
                            X = Canvas.GetLeft(ellipse),
                            Y = Canvas.GetTop(ellipse),
                            Width = ellipse.Width,
                            Height = ellipse.Height,
                            Color = (ellipse.Fill as SolidColorBrush)?.Color ?? Colors.Black,
                            Opacity = ellipse.Opacity
                        };

                        sprayPaintDataList.Add(sprayPaintData);
                    }
                }

                var json = new { _viewModel.ImageUri, SprayPaintData = sprayPaintDataList };

                // Serializing and save spray paint data to a JSON file
                string jsonData = JsonConvert.SerializeObject(json);

                File.WriteAllText(Path.ChangeExtension(saveFileDialog.FileName, "json"), jsonData);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a RenderTargetBitmap to render the Canvas
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                    (int)Canvas.ActualWidth, (int)Canvas.ActualHeight, 96, 96, PixelFormats.Default);
                renderTargetBitmap.Render(Canvas);

                // Create a BitmapEncoder (e.g., PngBitmapEncoder) to save the RenderTargetBitmap
                BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                // Open a SaveFileDialog to get the save path
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Save the image to the selected path
                    using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        bitmapEncoder.Save(fileStream);
                    }
                }

                // Save spray paint data separately
                SaveSprayPaintData(saveFileDialog);
            }
            catch (ArgumentException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((Button)sender).Visibility = Visibility.Collapsed;

                if (_viewModel.IsErasing == true)
                {
                    EraseButton.Visibility = Visibility.Visible;

                }
                else
                {
                    PaintButton.Visibility = Visibility.Visible;
                }
                _viewModel.IsErasing = !_viewModel.IsErasing; // Toggle erase mode
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 //  file name and extension
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "README.md");

                if (File.Exists(filePath)) // check if file exists
                {
                    string fileContent = File.ReadAllText(filePath); // read the entire content of the file

                    // create a new dialog box window and set its properties
                    Window dialogBox = new Window();
                    dialogBox.Title = "File Content";
                    // dialogBox.SizeToContent = SizeToContent.WidthAndHeight;
                    dialogBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    // create a scroll viewer to display the file content
                    ScrollViewer scrollViewer = new ScrollViewer();
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    scrollViewer.Margin = new Thickness(10);

                    // create a text block to display the file content
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = fileContent;
                    textBlock.Margin = new Thickness(10);

                    // add the text block to the scroll viewer's content
                    scrollViewer.Content = textBlock;

                    // add the scroll viewer to the dialog box's content
                    dialogBox.Content = scrollViewer;

                    // display the dialog box
                    dialogBox.ShowDialog();
                }
                else
                {
                    MessageBox.Show("File not found!", "Error"); // if file does not exist, display an error message in a dialog box
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private Color GetSelectedColor()
        {
            string colorName = (ColorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Black";

            switch (colorName)
            {
                case "Black":
                    return Colors.Black;
                case "Red":
                    return Colors.Red;
                case "Blue":
                    return Colors.Blue;
                case "Green":
                    return Colors.Green;
                default:
                    return Colors.Black;
            }

        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Update the selected color when the ComboBox selection changes
                Color selectedColor = GetSelectedColor();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
           

        }

    }

}
