using Npgsql;

namespace AccountService.Utils;

public static class Utils 
{
	public static bool TryGetPg(Exception ex, out PostgresException pg)
	{
		pg = null!;
		for (var e = ex; e != null; e = e.InnerException!)
			if (e is PostgresException p) { pg = p; return true; }
		return false;
	}	
}