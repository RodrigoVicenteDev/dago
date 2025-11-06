using Nager.Date;

namespace dago.Services.Utils
{
    public interface IBusinessDayService
    {
        DateTime AddBusinessDays(DateTime start, int businessDays);
        int BusinessDaysBetween(DateTime from, DateTime to);
        bool IsHoliday(DateTime date);
        bool IsBusinessDay(DateTime date);
    }

    public class BusinessDayService : IBusinessDayService
    {
        public DateTime AddBusinessDays(DateTime start, int businessDays)
        {
            if (businessDays == 0) return start;

            int step = businessDays > 0 ? 1 : -1;
            int daysToAdd = Math.Abs(businessDays);
            var date = start;

            while (daysToAdd > 0)
            {
                date = date.AddDays(step);
                if (IsBusinessDay(date)) daysToAdd--;
            }
            return date;
        }

        public int BusinessDaysBetween(DateTime from, DateTime to)
        {
            if (from > to) (from, to) = (to, from);

            int days = 0;
            var d = from.Date;
            var end = to.Date;

            while (d < end)
            {
                d = d.AddDays(1);
                if (IsBusinessDay(d)) days++;
            }
            return days;
        }

        public bool IsHoliday(DateTime date)
        {
            // Feriados nacionais do Brasil
            var holidays = HolidaySystem.GetHolidays(date.Year, CountryCode.BR);
            return holidays.Any(h => h.Date.Date == date.Date);
        }

        public bool IsBusinessDay(DateTime date)
        {
            // Sábado/Domingo
            if (WeekendSystem.IsWeekend(date, CountryCode.BR)) return false;
            // Feriado
            if (IsHoliday(date)) return false;
            return true;
        }
    }
}
