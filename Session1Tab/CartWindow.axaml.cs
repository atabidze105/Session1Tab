using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using static Session1Tab.Stats;

namespace Session1Tab;

public partial class CartWindow : Window
{
    private double _wholePrice = 0; //Общая стоимость всех выбраннных товаров
    public CartWindow()
    {
        InitializeComponent();
        LBox_cart.ItemsSource = _UserAutorized.UserCart.ToArray(); //у листбокса источник - корзина
        SetPrice(); //метод, высчитывающий и отображающий общую стоимость
    }

    private void SetPrice() //Метот вычисления и отображения обшщей стоимости
    {
        for (int i = 0; i < _UserAutorized.UserCart.Count; i++) //Перебор всех элемонтов корзины
        {
            _wholePrice += _UserAutorized.UserCart[i].cPrice; //общая цена составляется из цен всех товаров в корзине
        }
        _wholePrice = _UserAutorized.UserCart.Count > 0 ? _wholePrice : 0; //Если корзиина пуста, то стоимость равна 0
        tblock_price.Text = Convert.ToString(_wholePrice); //Отображение общей стоимости
    }

    private void CartActivity(object? sender, Avalonia.Interactivity.RoutedEventArgs e) 
    {
        var button = (sender as Button)!;
        switch (button.Name)
        {
            case "btn_cartReturn": //возвращение к списку товаров
                {
                    ListWindow listWindow = new();
                    listWindow.Show();
                    this.Close();
                }
                break;
            case "btn_cartClear": //Очистка всей корзины
                {
                    for (int i = 0; i < _LboxItems.Count; i++)//перебор всех элементов списка всех товаров
                    {
                        for (int j = 0; j < _UserAutorized.UserCart.Count; j++) //перебор корзины
                        {
                            if (_LboxItems[i].pId == _UserAutorized.UserCart[j].prId) //если идентификаторы совпали,
                            {
                                _LboxItems[i].pQuantity += _UserAutorized.UserCart[j].cQuantity; //то к товарам в общем списке возвращаеся количесво из корзины
                            }
                        }
                    }
                    _UserAutorized.UserCart.Clear(); //Очистка списка корзины
                    SetPrice(); //Обновление цены
                    LBox_cart.ItemsSource = _UserAutorized.UserCart.ToArray(); //Обновление листбокса
                }
                break;
            case "btn_cartItemDelete": //удалнение одного товара 
                {
                    for (int i = 0; i < _LboxItems.Count; i++) //перебор всех товаров 
                    {
                        if (_LboxItems[i].pId == _UserAutorized.UserCart[(int)button!.Tag!].prId) //Если идентификаторы совпали (товара из общего списка и товара, на котором ббыла нажата кнопка)
                        {
                            _LboxItems[i].pQuantity += _UserAutorized.UserCart[(int)button!.Tag!].cQuantity;
                        }
                    }
                    _UserAutorized.UserCart.RemoveAt((int)button!.Tag!);
                    for (int j = 0; j < _UserAutorized.UserCart.Count; j++)
                    {
                        _UserAutorized.UserCart[j].cId = j;
                    }
                    LBox_cart.ItemsSource = _UserAutorized.UserCart.ToArray();
                    SetPrice();
                }
                break;
        }
    }
}