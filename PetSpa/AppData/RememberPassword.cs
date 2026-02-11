namespace PetSpa.AppData
{
    public static class RememberPassword
    {
        // Сохранить логин и пароль
        public static void SaveLogin(string userId, string password)
        {
            Properties.Settings.Default.SavedUserId = userId;
            Properties.Settings.Default.SavedPassword = password;
            Properties.Settings.Default.RememberMe = true;
            Properties.Settings.Default.Save();
        }

        // Получить сохраненный логин
        public static string GetSavedLogin()
        {
            return Properties.Settings.Default.SavedUserId;
        }

        // Получить сохраненный пароль
        public static string GetSavedPassword()
        {
            return Properties.Settings.Default.SavedPassword;
        }

        // Проверить, включено ли запоминание
        public static bool IsRememberEnabled()
        {
            return Properties.Settings.Default.RememberMe;
        }

        // Забыть данные
        public static void Forget()
        {
            Properties.Settings.Default.SavedUserId = "";
            Properties.Settings.Default.SavedPassword = "";
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();
        }
    }
}