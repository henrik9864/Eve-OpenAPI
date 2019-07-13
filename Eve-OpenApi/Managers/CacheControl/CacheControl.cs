using EveOpenApi.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Managers.CacheControl
{
	public class CacheControl
	{
		internal Cacheability Cacheability { get; } = Cacheability.Public;

		internal uint TimeUntillStale { get; } = 60;

		internal Validation Validation { get; } = Validation.MustRevalidate;

		public CacheControl(string cacheControl)
		{
			string[] items = cacheControl.Split(',');

			for (int i = 0; i < items.Length; i++)
			{
				string item = items[i];

				if (TryGetCacheability(item, out Cacheability cacheabilityResult))
				{
					Cacheability = cacheabilityResult;
					continue;
				}
				else if (TryGetExpiration(item, out uint freshnessResult))
				{
					TimeUntillStale = freshnessResult;
					continue;
				}
				if (TryGetValidation(item, out Validation validationResult))
				{
					Validation = validationResult;
					continue;
				}
			}
		}

		bool TryGetCacheability(string item, out Cacheability result)
		{
			item.Replace("-", "");
			return Enum.TryParse(item, out result);
		}

		bool TryGetValidation(string item, out Validation result)
		{
			item.Replace("-", "");
			return Enum.TryParse(item, out result);
		}

		bool TryGetExpiration(string item, out uint result)
		{
			string[] parts = item.Split('=');
			result = default;

			if (parts.Length != 2)
				return false;

			switch (parts[0])
			{
				case "max-age":
				case "s-max-age":
					return uint.TryParse(parts[1], out result);
				default:
					return false;
			}
		}
	}
}
