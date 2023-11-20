using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Coursework;

public partial class CalculatePage : Page, INotifyPropertyChanged
{
    private readonly List<Tuple<TextBox, TextBox>> _groupTextBoxes = new();
    private readonly List<Tuple<TextBox, TextBox, TextBox>> _groupTextBoxes2 = new();
    private int _wallButtonClickCount;
    private int _windowButtonClickCount;
    private Siding _selectedSiding;
    private Worker _worker;
    private readonly double _totalResult = 0;
    private int _textBoxCount = 0;
    private double _totalLengthSum;
    
    private double _sumFilm; //сумма плёнки
    private int _countFilm; //количество плёнки
    private double _priceFilm = 5200; //цена пленки5200 
    
    private double _sumSlats; //сумма планокСтартовых/Финишных
    private double _countSlats; //кол-во планок Стартовых/Финишных

    private double _sumSiding; //сумма сайдинга итоговая
    private double _countSiding; //кол-во сайдинга 

    private double _sumWindow; //сумма окон 
    private double _sumWall; //сумма стенк

    private decimal _lengthSiding; //длиннна выбранного сайдинга
    private decimal _widthSiding; //ширина выбранного сайдинга
    private decimal _areaSidind; //площадь сайдинга

    private double _totalAreaWall; //общаяя площадь стен
    private double _totalLengthWall; //общаяя длинна стен

    private Visibility _isVisibleSelected = Visibility.Collapsed;

    public Siding SelectedSiding
    {
        get => _selectedSiding;
        set
        {
            _selectedSiding = value;
            IsVisibleSelected = Visibility.Visible;
            OnPropertyChanged();
        }
    }

    public Visibility IsVisibleSelected
    {
        get => _isVisibleSelected;
        set
        {
            _isVisibleSelected = value;
            OnPropertyChanged();
        }
    }

    public CalculatePage(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        UpdateList();
        DataContext = this;
    }

    private void CalculatePage_OnLoaded(object sender, RoutedEventArgs e)
    {
        //  
    }

    private void UpdateList()
    {
        using var context = new MyDbContext();
        var b = new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\picture.png"));
        var project = context.Sidings.Include(p => p.FeaturesMaterials).ToList();

        SelectedSiding = project.First();
        lvDataBinding.ItemsSource = project
            .Select(p => new Siding
            {
                SidingId = p.SidingId,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                Image = p.Image ?? b
            }).ToList();
    }

    private void AddWallButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_wallButtonClickCount < 6)
        {
            var group = CreateElementGroup();
            elementStackPanel.Children.Add(group);
            _wallButtonClickCount++;
        }
        else
        {
            MessageBox.Show("Ограничение на стены");
        }
    }

    private UIElement CreateElementGroup()
    {
        var grid = new Grid();

        grid.Margin = new Thickness(30);
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = new GridLength(150) }); // Fixed width for the second column
        grid.ColumnDefinitions.Add(new ColumnDefinition()); // New column for the first text box

        for (var i = 0; i < 7; i++) grid.RowDefinitions.Add(new RowDefinition());

        // Label 1
        var label1 = new Label { Content = "Длина (м):", VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(label1, 0);
        Grid.SetColumn(label1, 1);
        grid.Children.Add(label1);

        // TextBox 1
        var textBox1 = new TextBox();
        Grid.SetRow(textBox1, 1);
        Grid.SetColumn(textBox1, 1);
        grid.Children.Add(textBox1);

        // Label 2
        var label2 = new Label { Content = "Ширина (м):", VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(label2, 3);
        Grid.SetColumn(label2, 1);
        grid.Children.Add(label2);

        // TextBox 2
        var textBox2 = new TextBox();
        Grid.SetRow(textBox2, 4);
        Grid.SetColumn(textBox2, 1);
        grid.Children.Add(textBox2);

        // Result 2
        var resultLabel2 = new Label { Content = "Результат:", VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(resultLabel2, 5);
        Grid.SetColumn(resultLabel2, 1);
        grid.Children.Add(resultLabel2);

        // Event handlers for automatic calculation
        textBox1.TextChanged += (sender, e) =>
        {
            TextBox_TextChanged(textBox1);
            CalculateResult(textBox1, textBox2, resultLabel2);
        };
        textBox2.TextChanged += (sender, e) =>
        {
            TextBox_TextChanged(textBox2);
            CalculateResult(textBox1, textBox2, resultLabel2);
        };


        // Button
        var deleteButton = new Button { Content = "Удалить стену" };
        deleteButton.Click += DeleteGroup_Click;
        Grid.SetRow(deleteButton, 6);
        Grid.SetColumn(deleteButton, 1); // Moved to the second column
        grid.Children.Add(deleteButton);

        return grid;
    }

    private void TextBox_TextChanged(TextBox textBox)
    {
        if (textBox.Text.StartsWith("."))
        {
            textBox.Text = "0" + textBox.Text;
            textBox.CaretIndex = textBox.Text.Length;
        }
    }

    private double CalculateTotal()
    {
        _sumWall = _groupTextBoxes.Sum(tuple =>
        {
            double value1, value2;
            return double.TryParse(tuple.Item1.Text, out value1) && double.TryParse(tuple.Item2.Text, out value2)
                ? value1 * value2
                : 0;
        });

        txtResult.Text = $"Общая сумма: {_sumWall:F2} м²";
        return _sumWall;
    }
    private double CalculateTotal2()
    {
        _sumWindow = _groupTextBoxes2.Sum(tuple =>
        {
            double value1, value2, value3;
            return double.TryParse(tuple.Item1.Text, out value1) && double.TryParse(tuple.Item2.Text, out value2) && double.TryParse(tuple.Item3.Text, out value3)
                ? value1 * value2 * value3
                : 0;
        });

        txtResult2.Text = $"Общая сумма: {_sumWindow:F2} м²";
        return _sumWindow;
    }

    private double CalculateTotalLength()
    {
        var totalLengthWall = 0.0;

        foreach (var tuple in _groupTextBoxes)
        {
            double length;
            if (double.TryParse(tuple.Item1.Text, out length)) totalLengthWall += length;
        }

        return totalLengthWall;
    }

    private void CalculateResult(TextBox textBox1, TextBox textBox2, Label resultLabel)
    {
        double value1, value2;

        if (!string.IsNullOrWhiteSpace(textBox1.Text) && double.TryParse(textBox1.Text, out value1) &&
            !string.IsNullOrWhiteSpace(textBox2.Text) && double.TryParse(textBox2.Text, out value2))
        {
            var result = value1 * value2;
            resultLabel.Content = $"Результат: {result:F2} м²";
            UpdateResult();

            var tupleToUpdate =
                _groupTextBoxes.FirstOrDefault(tuple => tuple.Item1 == textBox1 && tuple.Item2 == textBox2);
            if (tupleToUpdate != null)
            {
                tupleToUpdate.Item1.Text = textBox1.Text;
                tupleToUpdate.Item2.Text = textBox2.Text;
            }
            else
            {
                _groupTextBoxes.Add(new Tuple<TextBox, TextBox>(textBox1, textBox2));
            }

            CalculateTotal();
        }
        else
        {
            resultLabel.Content = "Неверный ввод. Введите числовые значения.";
        }
    }

    private void CalculateResult(TextBox textBox1, TextBox textBox2, Label resultLabel, TextBox textBox3)
    {
        double value1, value2, value3;

        if (!string.IsNullOrWhiteSpace(textBox1.Text) && double.TryParse(textBox1.Text, out value1) &&
            !string.IsNullOrWhiteSpace(textBox2.Text) && double.TryParse(textBox2.Text, out value2) &&
        !string.IsNullOrWhiteSpace(textBox3.Text) && double.TryParse(textBox3.Text, out value3))
        {
            var result = value1 * value2 * value3;
            resultLabel.Content = $"Результат: {result:F2} м²";
            UpdateResult();

            var tupleToUpdate =
                _groupTextBoxes2.FirstOrDefault(tuple => tuple.Item1 == textBox1 && tuple.Item2 == textBox2 && tuple.Item3 == textBox3);
            if (tupleToUpdate != null)
            {
                tupleToUpdate.Item1.Text = textBox1.Text;
                tupleToUpdate.Item2.Text = textBox2.Text;
                tupleToUpdate.Item3.Text = textBox3.Text;
            }
            else
            {
                _groupTextBoxes2.Add(new Tuple<TextBox, TextBox, TextBox>(textBox1, textBox2, textBox3));
            }

            CalculateTotal2();
        }
        else
        {
            resultLabel.Content = "Неверный ввод. Введите числовые значения.";
        }
    }
    private void DeleteGroup_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            var group = element.Parent as UIElement;

            if (elementStackPanel.Children.Contains(group))
            {
                elementStackPanel.Children.Remove(group);
                _wallButtonClickCount--;

                var tupleToRemove = _groupTextBoxes.FirstOrDefault(tuple => tuple.Item1.Parent == group);
                if (tupleToRemove != null) _groupTextBoxes.Remove(tupleToRemove);

                CalculateTotal();
            }
        }
    }

    private void UpdateResult()
    {
        txtResult.Text = $"{_totalResult:F2} s";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void MakeCalculation_OnClick(object sender, RoutedEventArgs e)
    {
        GetMakeCaculation();
        
        DisplaySidingInfo(getExectResult);
        GetSlats(getExectResult);
        GetSlatsFinish(getExectResult);
        GetFilm(getExectResult);
        ShowCalculationResults();
    }

    private async Task GetMakeCaculation()
    {
        try
        {
            _totalLengthSum = CalculateTotalLength();
            var context = new MyDbContext();
            var result = context.FeaturesMaterials
                .Where(fm => fm.SidingId == _selectedSiding.SidingId)
                .Include(fm => fm.Features)
                .Select(fm => new
                {
                    FeaturesMaterials = fm.Value,
                    Features = fm.Features.Title
                }).ToList();
            var serializedResult = JsonConvert.SerializeObject(result);

            var featuresMaterialsValue = JsonConvert.DeserializeObject<List<dynamic>>(serializedResult)
                .Select(item => item.FeaturesMaterials)
                .ToList();

            _lengthSiding = Math.Round(Convert.ToDecimal(featuresMaterialsValue[0]), 3);
            _widthSiding = Math.Round(Convert.ToDecimal(featuresMaterialsValue[1]), 3);

            if (_lengthSiding != 0 && _widthSiding != 0)
            {
                _areaSidind = _lengthSiding * _widthSiding;
                _totalAreaWall = CalculateTotal();

                if (_areaSidind != 0)
                {
                    _countSiding = Convert.ToDouble(_totalAreaWall) / Convert.ToDouble(_areaSidind);
                    MessageBox.Show(_countSiding.ToString());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    private void DisplaySidingInfo(StackPanel dockPanel)
    {
        dockPanel.HorizontalAlignment = HorizontalAlignment.Center;
        if (SelectedSiding != null)
        {
            var sidingImage = new Image
            {
                Source = SelectedSiding.Image,
                Width = 300,
                Height = 300
            };
            var sidingNameTextBlock = new TextBlock
            {
                Text = SelectedSiding.Title,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Left,
                FontSize = 26,
                FontFamily = new FontFamily("Arial"),
                Margin = new Thickness(5)
            };
            var sidingPriceTextBlock = new TextBlock
            {
                Text = $"Цена за штуку: {SelectedSiding.Price:C}",
                TextAlignment = TextAlignment.Left,
                FontSize = 20,
                FontFamily = new FontFamily("Arial"),
                Margin = new Thickness(5)
            };

            _countSiding = Math.Ceiling(_countSiding);
            _sumSiding = Convert.ToDouble(SelectedSiding.Price) * _countSiding;

            var countSlats = new TextBlock()
            {
                Text = $"Необходимо: {Math.Ceiling(_countSiding)} шт",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var totalSidingPriceTextBlock = new TextBlock
            {
                Text = $"Итоговая цена: {_sumSiding:C}",
                TextAlignment = TextAlignment.Left,
                FontSize = 20,
                FontFamily = new FontFamily("Arial"),
                Margin = new Thickness(5)
            };

            var imageStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            imageStackPanel.Children.Add(sidingImage);

            var infoStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(40)
            };
            infoStackPanel.Children.Add(sidingNameTextBlock);
            infoStackPanel.Children.Add(sidingPriceTextBlock);
            infoStackPanel.Children.Add(countSlats);
            infoStackPanel.Children.Add(totalSidingPriceTextBlock);

            var allStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            allStackPanel.Children.Add(imageStackPanel);
            allStackPanel.Children.Add(infoStackPanel);

            dockPanel.Children.Add(allStackPanel);
        }
    }
    
    private void GetSlats(StackPanel dockPanel)
    {
        if (SelectedSiding != null)
        {
            var imageSlats = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/planka.png")),
                Width = 300,
                Height = 300
            };
            var sidingSlatsTextBlock = new TextBlock
            {
                Text = "Планка стартовая универсальная",
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold,
                FontSize = 26,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5)
            };
            var sidingSlatsPriceBlock = new TextBlock
            {
                Text = $"Цена за штуку: {420:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left, Margin = new Thickness(5)
            };
            
            _countSlats = _totalLengthSum;
            _sumSlats = _countSlats * 420;

            var countSlatsTextBlocl = new TextBlock()
            {
                Text = $"Необходимо: {_countSlats} шт",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var totalSlatsPriceTextBlock = new TextBlock
            {
                Text = $"Итого: {_sumSlats:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var imageSlatsStackPanel = new StackPanel();
            imageSlatsStackPanel.Children.Add(imageSlats);

            var infoStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(40)
            };

            infoStackPanel.Children.Add(sidingSlatsTextBlock);
            infoStackPanel.Children.Add(sidingSlatsPriceBlock);
            infoStackPanel.Children.Add(countSlatsTextBlocl);
            infoStackPanel.Children.Add(totalSlatsPriceTextBlock);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            stackPanel.Children.Add(imageSlatsStackPanel);
            stackPanel.Children.Add(infoStackPanel);

            dockPanel.Children.Add(stackPanel);
        }
    }

    //финишная планка
    private void GetSlatsFinish(StackPanel dockPanel)
    {
        if (SelectedSiding != null)
        {
            var imageSlats = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/finish.png")),
                Width = 300,
                Height = 300
            };
            var sidingSlatsTextBlock = new TextBlock
            {
                Text = "Планка финишная универсальная",
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold,
                FontSize = 26,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5)
            };
            var sidingSlatsPriceBlock = new TextBlock
            {
                Text = $"Цена за штуку: {420:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left, Margin = new Thickness(5)
            };

            _countSlats = _totalLengthSum;
            _sumSlats = _countSlats * 420;

            var countSlataTextBlock = new TextBlock()
            {
                Text = $"необходимо: {_countSlats} шт",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };


            var totalSlatsPriceTextBlock = new TextBlock
            {
                Text = $"Итого: {_sumSlats:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var imageSlatsStackPanel = new StackPanel();
            imageSlatsStackPanel.Children.Add(imageSlats);

            var infoStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(40)
            };

            infoStackPanel.Children.Add(sidingSlatsTextBlock);
            infoStackPanel.Children.Add(sidingSlatsPriceBlock);
            infoStackPanel.Children.Add(countSlataTextBlock);
            infoStackPanel.Children.Add(totalSlatsPriceTextBlock);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            stackPanel.Children.Add(imageSlatsStackPanel);
            stackPanel.Children.Add(infoStackPanel);

            dockPanel.Children.Add(stackPanel);
        }
    }

    //ветрозащитная плёнка
    private void GetFilm(StackPanel dockPanel)
    {
        if (SelectedSiding != null)
        {
            var imageSlats = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/film.png")),
                Width = 300,
                Height = 300
            };
            var sidingSlatsTextBlock = new TextBlock
            {
                Text = "Пленка ветро-влагозащитная Grand Line Facade (75м2)",
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold,
                FontSize = 26,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5)
            };
            var sidingSlatsPriceBlock = new TextBlock
            {
                Text = $"Цена за штуку: {_priceFilm:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left, Margin = new Thickness(5)
            };

            _countSlats = _totalLengthSum;
            //slatsPrice = countSlats * 420;
            _countFilm = (int)Math.Ceiling(_sumWall / 75.0);

            var totalSlatsPriceTextBlock = new TextBlock
            {
                Text = $"Необходимо: {_countFilm} шт",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            _sumFilm = _countFilm * 5200;
            var totalSumPriceTextBlock = new TextBlock
            {
                Text = $"Итого: {_sumFilm:C} ",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var imageSlatsStackPanel = new StackPanel();
            imageSlatsStackPanel.Children.Add(imageSlats);

            var infoStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(40)
            };

            infoStackPanel.Children.Add(sidingSlatsTextBlock);
            infoStackPanel.Children.Add(sidingSlatsPriceBlock);
            infoStackPanel.Children.Add(totalSlatsPriceTextBlock);
            infoStackPanel.Children.Add(totalSumPriceTextBlock);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            stackPanel.Children.Add(imageSlatsStackPanel);
            stackPanel.Children.Add(infoStackPanel);

            dockPanel.Children.Add(stackPanel);
        }
    }
   
    private void ShowCalculationResults()
    {
        exectResult.Children.Clear();

        var sectionLabel = new Label
        {
            Content = "Счёт/заказ",
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 16 // Размер шрифта
        };
        exectResult.Children.Add(sectionLabel);

        foreach (var tuple in _groupTextBoxes)
        {
            double value1, value2;
            if (double.TryParse(tuple.Item1.Text, out value1) && double.TryParse(tuple.Item2.Text, out value2))
            {
                var result = value1 * value2;

                var wallAreaLabel = new Label
                {
                    Content = $"Площадь стены ({value1} м x {value2} м): {result:F2} м²",
                    Margin = new Thickness(0, 5, 5, 0),
                    FontSize = 14 // Размер шрифта
                };
                exectResult.Children.Add(wallAreaLabel);
            }
        }

        var additionalInfoLabel = new Label
        {
            Content = "Количетсво материалов",
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 16 // Размер шрифта
        };
        exectResult.Children.Add(additionalInfoLabel);

        var totalWallLength = _groupTextBoxes.Sum(tuple =>
        {
            double value;
            return double.TryParse(tuple.Item1.Text, out value) ? value : 0;
        });
        
        _countSiding = _totalAreaWall / Convert.ToDouble(_areaSidind);

        var nProfilesCount = _countSiding;

        var sidingCountLabel = new Label
        {
            Content = $"Количество сайдинга: {Math.Ceiling(_countSiding)} шт.",
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 14 // Размер шрифта
        };
        exectResult.Children.Add(sidingCountLabel);

        var starterPlanksLabel = new Label
        {
            Content = $"Количество стартовых планок: {_countSlats} шт.",
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 14 // Размер шрифта
        };
        exectResult.Children.Add(starterPlanksLabel);

        var finishPlanksLabel = new Label
        {
            Content = $"Количество финишных планок: {_countSlats} шт.",
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 14 // Размер шрифта
        };
        exectResult.Children.Add(finishPlanksLabel);
    }

    private void WindowDoorCheckBox_OnChecked(object sender, RoutedEventArgs e)
    {
        var additionalButton = new Button { Content = "Добавить проём" };
        additionalButton.Click += AdditionalButton_Click;
        additionalButton.Width = 200;
        additionalButton.Content = "Добавить проём";
        
        StackPanel.Children.Add(additionalButton);
        
        if (_windowButtonClickCount < 6)
        {
            var group = GetWindow();
            WindowStackPanel.Children.Add(group);
        }
        else
        {
            MessageBox.Show("Ограничение на стены");
        }
    }

    private UIElement GetWindow()
{
    var grid = new Grid();

    grid.Margin = new Thickness(30);
    grid.ColumnDefinitions.Add(new ColumnDefinition()); // New column for the button
    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) }); // Fixed width for the second column
    grid.ColumnDefinitions.Add(new ColumnDefinition()); // New column for the first text box

    for (var i = 0; i < 9; i++) grid.RowDefinitions.Add(new RowDefinition());

    

    // Label for number of windows
    var numWindowsLabel = new Label { Content = "Количество окон", VerticalAlignment = VerticalAlignment.Center };
    Grid.SetRow(numWindowsLabel, 1);
    Grid.SetColumn(numWindowsLabel, 1);
    grid.Children.Add(numWindowsLabel);

    // TextBox for number of windows
    var numWindowsTextBox = new TextBox();
    Grid.SetRow(numWindowsTextBox, 2);
    Grid.SetColumn(numWindowsTextBox, 1);
    grid.Children.Add(numWindowsTextBox);

    // Label 1
    var label1 = new Label { Content = "Длина (м):", VerticalAlignment = VerticalAlignment.Center };
    Grid.SetRow(label1, 3);
    Grid.SetColumn(label1, 1);
    grid.Children.Add(label1);

    // TextBox 1
    var textBox1 = new TextBox();
    Grid.SetRow(textBox1, 4);
    Grid.SetColumn(textBox1, 1);
    grid.Children.Add(textBox1);

    // Label 2
    var label2 = new Label { Content = "Ширина (м):", VerticalAlignment = VerticalAlignment.Center };
    Grid.SetRow(label2, 5);
    Grid.SetColumn(label2, 1);
    grid.Children.Add(label2);

    // TextBox 2
    var textBox2 = new TextBox();
    Grid.SetRow(textBox2, 6);
    Grid.SetColumn(textBox2, 1);
    grid.Children.Add(textBox2);

    // Result label
    var resultLabel = new Label { Content = "Результат:", VerticalAlignment = VerticalAlignment.Center };
    Grid.SetRow(resultLabel, 7);
    Grid.SetColumn(resultLabel, 1);
    grid.Children.Add(resultLabel);

    // Event handlers for automatic calculation
    numWindowsTextBox.TextChanged += (sender, args) =>
    {
        TextBox_TextChanged(numWindowsTextBox);
        CalculateResult(textBox1, textBox2, resultLabel, numWindowsTextBox);
    };
    textBox1.TextChanged += (sender, e) =>
    {
        TextBox_TextChanged(textBox1);
        CalculateResult(textBox1, textBox2, resultLabel, numWindowsTextBox);
    };
    textBox2.TextChanged += (sender, e) =>
    {
        TextBox_TextChanged(textBox2);
        CalculateResult(textBox1, textBox2, resultLabel, numWindowsTextBox);
    };

    // Buttons
    var deleteButton = new Button { Content = "Удалить стену" };
    deleteButton.Click += DeleteWindowGroup_Click;
    Grid.SetRow(deleteButton, 8);
    Grid.SetColumn(deleteButton, 1); // Moved to the second column
    grid.Children.Add(deleteButton);

    return grid;
}
    private void AdditionalButton_Click(object sender, RoutedEventArgs e)
    {
        if (_windowButtonClickCount < 5)
        {
            var group = GetWindow();
            WindowStackPanel.Children.Add(group);
            _windowButtonClickCount++;
        }
        else
        {
            MessageBox.Show("Ограничение на стены");
        }
    }
    private void DeleteWindowGroup_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            var group = element.Parent as UIElement;

            if (WindowStackPanel.Children.Contains(group))
            {
                WindowStackPanel.Children.Remove(group);
                _windowButtonClickCount--;

                var tupleToRemove = _groupTextBoxes.FirstOrDefault(tuple => tuple.Item1.Parent == group);
                if (tupleToRemove != null) _groupTextBoxes.Remove(tupleToRemove);

                CalculateTotal2();
            }
        }
    }
    private void WindowDoorCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
    {
        WindowStackPanel.Children.Clear();
        StackPanel.Children.Clear();
    }
}