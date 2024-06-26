using Avalonia.Controls;
using static Session1Tab.Stats;

namespace Session1Tab
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoggingIn(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var button = (sender as Button)!;
            switch (button.Name)
            {
                case "btn_login": //Кнопка входа
                    {
                        tblock_login.IsVisible = false;
                        tblock_password.IsVisible = false; //Устанавливается невидимость текстблоков с ошибкой входа
                        if (tbox_login.Text != null || tbox_login.Text != "" || tbox_password.Text != null || tbox_password.Text != "") //если в поля логина и пароля что-то введено
                        {
                            foreach (User user in _Users)
                            {
                                if (user.uName == tbox_login.Text) //если логин совпадает с именем одного из пользователей
                                {
                                    tblock_login.IsVisible = false; //скрытие текстблока с сообщением о неверном логине
                                    if (user.uPassword == tbox_password.Text) //если введенный пароль совпадает с паролем найденного пользователя
                                    {
                                        _UserAutorized = user; //Авторизированный пользователь становится найденым пользователем
                                        ListWindow listWindow = new(); //переход к окну списка товаров
                                        listWindow.Show();
                                        this.Close();
                                    }
                                    else
                                    {
                                        tblock_password.IsVisible = true; //появление уведомления о неверном пароле
                                        break; //остановка цикла
                                    }
                                }
                                else
                                {
                                    tblock_login.IsVisible = true; //появление уведомлений о неверных логине и пароле
                                    tblock_password.IsVisible = true;
                                }
                            }
                        }
                    }
                    break;
                case "btn_guest": //гостевой вход
                    {
                        _UserAutorized = new("guest", "", false, true); //авторизированным пользователем становится новый объект класса User с параметрами гостя
                        ListWindow listWindow = new(); //переход к окну списка товаров
                        listWindow.Show();
                        this.Close();
                    }
                    break;
            }
        }
    }
}