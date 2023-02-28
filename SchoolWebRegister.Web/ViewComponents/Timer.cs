using Microsoft.AspNetCore.Mvc;

namespace SchoolWebRegister.Web.ViewComponents
{
    public class Timer : ViewComponent
    {
        public string Invoke()
        {
            return $"Текущее время по серверу: {DateTime.Now.ToString("hh:mm:ss")}";
        }
    }
}
