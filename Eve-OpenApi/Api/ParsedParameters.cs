using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi.Api
{
	internal struct ParsedParameters
	{
		public int MaxLength { get; }

		public List<KeyValuePair<string, List<string>>> Queries { get; }

		public List<KeyValuePair<string, List<string>>> Headers { get; }

		public List<KeyValuePair<string, List<string>>> PathParameters { get; }

		public List<string> Users { get; }

		public ParsedParameters(int maxLength, List<KeyValuePair<string, List<string>>> queries, List<KeyValuePair<string, List<string>>> headers, List<KeyValuePair<string, List<string>>> pathParameters, List<string> users)
		{
			MaxLength = maxLength;
			Queries = queries;
			Headers = headers;
			PathParameters = pathParameters;
			Users = users;
		}

		public override int GetHashCode()
		{
			return GetHashCode(0);
		}

		public int GetHashCode(int index)
		{
			int hash = 17;
			hash *= 23 + GetListHash(Queries, index);
			hash *= 23 + GetListHash(PathParameters, index);

			return hash;
		}

		int GetListHash(List<KeyValuePair<string, List<string>>> list, int index)
		{
			int hash = 31;
			for (int i = 0; i < list.Count; i++)
			{
				KeyValuePair<string, List<string>> kvp = list[i];

				if (kvp.Key == "token" || kvp.Key == "X-Token")
					continue;

				hash *= 7 + kvp.Key.GetHashCode();
				if (kvp.Value.Count == 1)
					hash *= 7 + kvp.Value[0].GetHashCode();
				else
					hash *= 7 + kvp.Value[index].GetHashCode();
			}

			return hash;
		}
	}
}
