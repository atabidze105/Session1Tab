using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using static Session1Tab.Stats;

namespace Session1Tab;

public partial class RedWindow : Window
{
    private string _SelectedProductImageSource = _SelectedProduct != null ? _SelectedProduct.pImageSource : null; //�������� ����������� � ��� ���� ������� ����������� �� ������ (����� ������������ �� ���� �� ��������� ������ ��� ��������������)
    private string? _SelectedImage = null; //��������� �����������
    
    public RedWindow()
    {
        InitializeComponent();
        tblock_panelHeader.Text = _SelectedProduct == null ? "������� �����" : "�������������� ������"; //���������� ��������� � ������ ���������� ��������� �������� � ����������� �� ���������� ���� ��� ���������� ��������
        btn_confirm.Content = _SelectedProduct == null ? "��������" : "���������";
        cbox_cathegories.ItemsSource = _Cathegories; //����������� ������ �������� �������� (����������� ������ ���������)
        cbox_cathegories.SelectedIndex = 0; //��������� ������� ������ ��������� - "������"
        cbox_measurements.ItemsSource = _Measurements; //����������� ������ �������� �������� (����������� ������ ������ ���������)
        cbox_measurements.SelectedIndex = 0; //��������� ������� ������ ������ ��������� - "��."
        ProductCheck(); //�������� �� ������� ���������� ������
    }

    private void ProductCheck() //�������� ������� ���������� ������
    {
        if (_SelectedProduct != null) //���� ����� �������������, �� ���� � �������������� ������� ����������� �������������
        {
            tblock_id.Text = $"ID: {_SelectedProduct.pId}";
            tbox_pName.Text = _SelectedProduct.pName;
            tbox_pSupplier.Text = _SelectedProduct.pSupplier;
            tbox_pPrice.Text = Convert.ToString(_SelectedProduct.pPrice);
            tbox_pQuantity.Text = Convert.ToString(_SelectedProduct.pQuantity);
            tbox_pDescription.Text = _SelectedProduct.pDescription;
            cbox_cathegories.SelectedIndex = _SelectedProduct.pCathegory;
            cbox_measurements.SelectedIndex = _SelectedProduct.pMeasurement;
            if (_SelectedProduct.pImageSource != null) //���� � ������ ���� �����������, �� ������������ ��� ������
            {
                tblock_preview.IsVisible = img_preview.IsVisible = true; //����������� � �������� �������� ���������� ��������
                tblock_preview.Text = _SelectedImage = _SelectedProduct.pImageSource; //������ ������ � ���� ��� ���������� ����������� ���������� �������� ����� �����������
                img_preview.Source = new Bitmap($"Assets/{_SelectedProduct.pImageSource}"); //�������� ������ ���������� ������ ����������� � ��������� ����� ����������� �� ������� � �������� ���������
            }
        }
    }

    private void newListWindow() //������� � ���� �� ������� �������
    {
        _SelectedProduct = null; //������� ����� �������������� ������ � ���������� �����������
        ListWindow listWindow = new ListWindow();
        listWindow.Show();
        this.Close();
    }

    private void RedActivity(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try //���� � ���� ��� int/double �������� ������� ���� �������, �� ���������� ����������. � ����� ������ ���� ��������/�������������� ���������, ��������� ���� ������ ������� � ��������� �� ����������
        {
            var button = (sender as Button)!;
            switch (button.Name)
            {
                case "btn_cancel": //������. ����������� ������� � ���� � �������� //���� �� ����� �������������� ������, ������� ����� �����������, �������� ��������������, �� �������� ���������, �� ����� �������� ����� ������� �� �������, ��� ���� ���������� ����������� ����������� ������
                    if (_SelectedImage != null && _SelectedImage != _SelectedProductImageSource) //���� ���� ��� ���������� ����������� �� ����� null � ��������� ����������� �� ��������� �� ��������� �� ����, �������� ������������ ����������� ������
                        File.Delete($"Assets/{_SelectedImage}"); //���� ����������� ��������� �� ����� �������
                    newListWindow();
                    break;
                case "btn_confirm": //�������������
                    if (_SelectedProduct != null) //���� ���� �������������� ������ �� ������ (�.�. ��������� ����� �������������)
                    {
                        for (int i = 0; i < _LboxItems.Count; i++)//����� �������������� ������ � ������ �������
                        {
                            if (_LboxItems[i].pId == _SelectedProduct.pId) //���� ������:
                            {
                                if (_SelectedImage != _SelectedProductImageSource && _SelectedProductImageSource != null) //���� ���� ����������� ����� �����������, 
                                    File.Delete($"Assets/{_SelectedProductImageSource}");

                                _LboxItems[i] = new Product(tbox_pName.Text, _SelectedProduct.pId, cbox_cathegories.SelectedIndex, tbox_pDescription.Text, tbox_pSupplier.Text, _SelectedImage, Convert.ToDouble(tbox_pPrice.Text), Convert.ToInt32(tbox_pQuantity.Text), cbox_measurements.SelectedIndex); //������ ������ � ������ ������� �� ����� � ������������ �������
                                newListWindow();
                                break;
                            }
                        }
                    }
                    else //���� ���� �������������� ������ ������ (�.�. ��������� ����� �����)
                    {
                        _LboxItems.Add(new Product(tbox_pName.Text, _LboxItems.Count, cbox_cathegories.SelectedIndex, tbox_pDescription.Text, tbox_pSupplier.Text, _SelectedImage, Convert.ToDouble(tbox_pPrice.Text), Convert.ToInt32(tbox_pQuantity.Text), cbox_measurements.SelectedIndex)); //����������� ����� ����� � ����� ������ �������
                        newListWindow();
                    }
                    break;
            }
        }
        catch
        {
            if (_SelectedImage != null && _SelectedImage != _SelectedProductImageSource) //���� �� ���������� ���� ������� ������ �����������, ��� �������� �� �������. ���� � ����� ��� ���� �����������, ��� ��������� �� �����
                File.Delete($"Assets/{_SelectedImage}");
            newListWindow();
        }
    }

    private readonly FileDialogFilter fileFilter = new() //������ ��� ����������
    {
        Extensions = new List<string>() { "png", "jpg", "jpeg" }, //��������� ����������, ������������ � ����������
        Name = "����� �����������" //���������
    };

    private string SameName(string filename) //�������� ������������ ����� �����
    {
        string[] withExtentions = Directory.GetFiles("Assets"); //��������� �������� ���� ����������� �� ������� � ������������
        List<string> withoutExtentions = []; //������������� ������ ������ ��� �������� ������ ��� ����������

        foreach (string file in withExtentions)
            withoutExtentions.Add(Path.GetFileNameWithoutExtension(file)); //� ����� ������ ���������� �������� ������ ��� ����������

        foreach (string file1 in withoutExtentions) //������� ������� �������� ����� �� ������ ��������
            if (file1 == filename) //���� �������� ������ �� ������ ��������� �������� ����� ��������� � ������
            {
                return filename; //���������� �������� �����
            }
        return null; //���� ����� ���� �� ��� ������, ���������� null
    }

    private async void ImageSelection(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var button = (sender as Button)!;
        switch (button.Name){
            case "btn_imgAdd": //���������� ��������
                OpenFileDialog dialog = new(); //�������� ����������
                dialog.Filters.Add(fileFilter); //���������� �������
                string[] result = await dialog.ShowAsync(this); //����� �����
                if (result == null || result.Length == 0)
                    return;//���� ������� ��������� �� �������� �� ����� �������

                string imageName = Path.GetFileName(result[0]); //��������� ����� �����
                string[] extention = imageName.Split('.'); //�������� ����� ������� �� �������� � ����������
                string temp = extention[0]; //� ���������� ���������� �������� �������� �����. ��� ����� �������� � ��������
                int i = 1; //�������
                while(SameName(temp) != null) //���� ����� ��� �������� ������������ ����� ���������� �������� �����
                {
                    temp = extention[0] + $"{i}"; //����� ��� �����
                    i++;
                }
                imageName = temp + '.' + extention[1]; //����� ��� ����� � �����������

                File.Copy(result[0], $"Assets/{imageName}", true); //����������� ����� � ����� �������

                if (_SelectedImage != null && _SelectedImage != _SelectedProductImageSource) //���� �� ��������� ����� �������� ���� ������� ������, � ��� ���� ��������� �������� �� �������� �� ����, �������� ������������ ����������� ������
                    File.Delete($"Assets/{_SelectedImage}"); //�������� ����������� ����������� �� �������

                tblock_preview.IsVisible = img_preview.IsVisible = true; //������������ ��������� ������ (�������� � ���������� � �� ���������)
                tblock_preview.Text = _SelectedImage = imageName; //���������� � ���� ��������� �������� �������� ��� �����
                img_preview.Source = new Bitmap($"Assets/{imageName}"); //��������� ��������� ������-�������� 

                break;
            case "btn_imgDel": //�������� ��������
                tblock_preview.IsVisible = img_preview.IsVisible = false; //������ ���������� ���������
                _SelectedImage = null;//������� ���� � ��������� ������������

                if (tblock_preview.Text != _SelectedProductImageSource) //�������� ���������� ������ ���� ��������� ����������� �� �������� ��������� �� ����, �������� ������������ ����������� ������
                    File.Delete($"Assets/{tblock_preview.Text}"); //�������� ����� �� �������� �� ������-����������

                break;
        }
    }
}