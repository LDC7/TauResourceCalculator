using System;

namespace TauResourceCalculator.BlazorServer.Utils;

public static class DateUtils
{
  /// <summary>
  /// Найти следующее по определённому счёту воскресенье.
  /// </summary>
  /// <param name="date">Дата с которой начинаем поиск.</param>
  /// <param name="sundaysCount">Какое по счёту воскресенье требуется.</param>
  /// <returns>Дата будущего воскресенья.</returns>
  public static DateOnly FindNextSunday(DateOnly date, int sundaysCount)
  {
    return date.AddDays(7 - (int)date.DayOfWeek).AddDays((sundaysCount - 1) * 7);
  }
}
