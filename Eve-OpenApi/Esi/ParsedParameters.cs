using System;
using System.Collections.Generic;
using System.Text;

namespace EveOpenApi
{
	internal struct ParsedParameters
	{
		public int MaxLength { get; }

		public List<KeyValuePair<string, List<string>>> Queries { get; }

		public List<KeyValuePair<string, List<string>>> Headers { get; }

		public List<KeyValuePair<string, List<string>>> PathParameters { get; }

		public ParsedParameters(int maxLength, List<KeyValuePair<string, List<string>>> queries, List<KeyValuePair<string, List<string>>> headers, List<KeyValuePair<string, List<string>>> pathParameters)
		{
			MaxLength = maxLength;
			Queries = queries;
			Headers = headers;
			PathParameters = pathParameters;
		}
	}
}
