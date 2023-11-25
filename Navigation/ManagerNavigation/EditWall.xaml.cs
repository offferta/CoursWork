using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Window = System.Windows.Window;

namespace Coursework;

public partial class EditWall : Window, INotifyPropertyChanged
{
    private readonly MyDbContext _context = new();
    private double _countSiding;
    public object _dataC0ontext;
    private int _intCalculationId;

    private int _intSiding;
    private double _newCount;
    private double _newLenght;
    private double _newWight;
    private string _newTitle;
    private double _sumWall; //сумма стенк
    private int _wallId;

    public EditWall(object dataContext)
    {
        InitializeComponent();

        _dataC0ontext = dataContext;

        DataContext = _dataC0ontext;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
    {
        ReplaceDotWithComma();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        NewCountSiding();
    }

    private void NewCountSiding()
    {
        _intSiding = Convert.ToInt32(txtCalculationId.Text);
        _wallId = Convert.ToInt32(txtWallId.Text);
        _newLenght = Convert.ToDouble(txtLength.Text);
        _newWight = Convert.ToDouble(txtWidth.Text);
        _newCount = Convert.ToDouble(txtCount.Text);
        _intCalculationId = Convert.ToInt32(txtCalculationId.Text);
        _newTitle = Convert.ToString(txtName.Text);

        try
        {
            var result = (
                from mc in _context.MaterialsCalculations
                join fm in _context.FeaturesMaterials on mc.SidingId equals fm.SidingId
                join s in _context.Sidings on mc.SidingId equals s.SidingId
                where mc.CalculationId == _intSiding && (fm.FeaturesId == 1 || fm.FeaturesId == 2)
                select new
                {
                    sidingId = mc.SidingId,
                    calculationId = mc.CalculationId,
                    count = mc.Count,
                    currentPrice = mc.CurrentPrice,
                    featuresId = fm.FeaturesId,
                    value = fm.Value,
                    sidingTitle = s.Title,
                    sidingDescription = s.Description,
                    sidingPrice = s.Price
                }
            ).ToList();

            var serializedResult = JsonConvert.SerializeObject(result);

            var featuresMaterialsValue = JsonConvert.DeserializeObject<List<dynamic>>(serializedResult)
                .Select(item => item.value)
                .ToList();

            decimal _widthSiding = Math.Round(Convert.ToDecimal(featuresMaterialsValue[0]), 3);
            decimal _lengthSiding = Math.Round(Convert.ToDecimal(featuresMaterialsValue[1]), 3);

            if (_lengthSiding != 0 && _widthSiding != 0)
            {
                var _areaSidind = _lengthSiding * _widthSiding;
                var _totalAreaWall = CalculateTotal();

                if (_areaSidind != 0)
                {
                    _countSiding = Convert.ToDouble(_totalAreaWall) / Convert.ToDouble(_areaSidind);
                    MessageBox.Show(_countSiding.ToString());
                }

                var updateTitleCalSiding = _context.Set<Calculation>()
                    .AsNoTrackingWithIdentityResolution()
                    .FirstOrDefault(e => e.CalculationId == _intCalculationId);

                updateTitleCalSiding.Title = _newTitle;
                _context.Set<Calculation>().Update(updateTitleCalSiding);
                _context.SaveChanges();
                
                var updateCountSiding = _context.Set<MaterialsCalculation>()
                    .AsNoTrackingWithIdentityResolution()
                    .FirstOrDefault(e => e.CalculationId == _intCalculationId);
                
                updateCountSiding.Count = (decimal)_countSiding;
                _context.Set<MaterialsCalculation>().Update(updateCountSiding);
                _context.SaveChanges();
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Test" + e);
        }
    }

    private double CalculateTotal()
    {
        var calculationId = Convert.ToInt32(txtCalculationId.Text);

        decimal requiredSidingArea = 0;

        var wallToUpdate = _context.Walls.FirstOrDefault(w => w.WallId == _wallId);
        if (wallToUpdate != null)
        {
            wallToUpdate.Length = (decimal)_newLenght;
            wallToUpdate.Width = (decimal)_newWight;
            wallToUpdate.Count = (byte)_newCount;
            _context.SaveChanges();
        }

        var updatedWallArea = _context.Walls
            .Where(wall => wall.WallId == _wallId)
            .Select(wall => wall.Length * wall.Width)
            .FirstOrDefault();

        var totalWallArea = _context.Walls
            .Where(wall => wall.CalculationId == calculationId)
            .Select(wall => wall.Length * wall.Width)
            .Sum();

        var totalWindowArea = _context.Windows
            .Where(wn => wn.CalculationId == calculationId)
            .Sum(wn => wn.Length * wn.Width * wn.Count);

        var result = new
        {
            UpdatedWallArea = updatedWallArea,
            TotalWallArea = totalWallArea,
            TotalWindowArea = totalWindowArea,
            RequiredSidingArea = totalWallArea - totalWindowArea
        };
        if (result != null) requiredSidingArea = result.TotalWallArea - result.TotalWindowArea;
        return (double)requiredSidingArea;
    }

    private void ReplaceDotWithComma()
    {
        if (txtLength != null) txtLength.Text = txtLength.Text.Replace('.', ',');
        if (txtWidth != null) txtWidth.Text = txtWidth.Text.Replace('.', ',');
    }
}