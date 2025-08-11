using Npgsql;

namespace AccountService.Utils;

public static class Utils 
{
	public static bool TryGetPg(Exception ex, out PostgresException pg)
	{
		pg = null!;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract Нужно для остановки цикла
        for (var e = ex; e != null; e = e.InnerException!)
			if (e is PostgresException p) { pg = p; return true; }
		return false;
	}
}