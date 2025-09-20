using ExpressionEngine.Domain;

namespace ExpressionEngine.Infrastructure;

public interface IFunctionRegistry
{
    Func<object?[], IReadOnlyDictionary<string, string>, object?> Resolve(string name);
}

public sealed class DefaultFunctionRegistry : IFunctionRegistry
{
    private readonly Dictionary<string, Func<object?[], IReadOnlyDictionary<string, string>, object?>> _funcs;

    public DefaultFunctionRegistry()
    {
        _funcs = new(StringComparer.Ordinal)
        {
            ["ADD"] = (args, _) => args.Aggregate<object?, double>(0, (acc, v) => acc + Convert.ToDouble(v, System.Globalization.CultureInfo.InvariantCulture)),
            ["SUB"] = (args, _) => args.Length >= 2 ? Convert.ToDouble(args[0]) - Convert.ToDouble(args[1]) : throw new Exception("SUBTRACT requires 2 args"),
            ["MUL"] = (args, _) => args.Aggregate<object?, double>(1, (acc, v) => acc * Convert.ToDouble(v, System.Globalization.CultureInfo.InvariantCulture)),
            ["DIV"] = (args, _) =>
            {
                if (args.Length < 2) throw new Exception("DIVIDE requires 2 args");
                var a = Convert.ToDouble(args[0], System.Globalization.CultureInfo.InvariantCulture);
                var b = Convert.ToDouble(args[1], System.Globalization.CultureInfo.InvariantCulture);
                if (Math.Abs(b) < double.Epsilon) throw new Exception("Division by zero");
                return a / b;
            },
            ["CONCAT"] = (args, _) => string.Concat(args.Select(a => a?.ToString() ?? string.Empty)),
            ["EQUALS"] = (args, _) => Equals(args.ElementAtOrDefault(0), args.ElementAtOrDefault(1)),
            ["NOTEQUALS"] = (args, _) => !Equals(args.ElementAtOrDefault(0), args.ElementAtOrDefault(1)),
            ["GREATER"] = (args, _) => Convert.ToDouble(args[0]) > Convert.ToDouble(args[1]),
            ["LESS"] = (args, _) => Convert.ToDouble(args[0]) < Convert.ToDouble(args[1]),
            ["GREATEROREQUALS"] = (args, _) => Convert.ToDouble(args[0]) >= Convert.ToDouble(args[1]),
            ["LESSOREQUALS"] = (args, _) => Convert.ToDouble(args[0]) <= Convert.ToDouble(args[1]),
            ["IF"] = (args, _) => Convert.ToBoolean(args[0]) ? args[1] : args[2],
            ["AND"] = (args, _) => args.All(a => Convert.ToBoolean(a)),
            ["OR"] = (args, _) => args.Any(a => Convert.ToBoolean(a)),
            ["NOT"] = (args, _) => !Convert.ToBoolean(args[0]),
            ["TOUPPER"] = (args, _) => (args[0]?.ToString() ?? string.Empty).ToUpperInvariant(),
            ["TOLOWER"] = (args, _) => (args[0]?.ToString() ?? string.Empty).ToLowerInvariant(),
            ["TRIM"] = (args, _) => (args[0]?.ToString() ?? string.Empty).Trim(),
            ["LENGTH"] = (args, _) => (args[0]?.ToString() ?? string.Empty).Length,
            ["SUBSTRING"] = (args, _) => (args[0]?.ToString() ?? string.Empty).Substring(Convert.ToInt32(args[1]), Convert.ToInt32(args[2])),
            ["CONTAINS"] = (args, _) => (args[0]?.ToString() ?? string.Empty).Contains(args[1]?.ToString() ?? string.Empty, StringComparison.Ordinal),
            ["STARTSWITH"] = (args, _) => (args[0]?.ToString() ?? string.Empty).StartsWith(args[1]?.ToString() ?? string.Empty, StringComparison.Ordinal),
            ["ENDSWITH"] = (args, _) => (args[0]?.ToString() ?? string.Empty).EndsWith(args[1]?.ToString() ?? string.Empty, StringComparison.Ordinal)
            ,
            ["GETJSONPROPERTY"] = (args, _) =>
            {
                if (args.Length < 2) throw new Exception("GETJSONPROPERTY requires obj and key");
                object? source = args[0];
                // Unwrap single-element enumerable/array
                if (source is IEnumerable<object?> seq && source is not string)
                {
                    using var e = seq.GetEnumerator();
                    if (e.MoveNext()) source = e.Current;
                }
                else if (source is Array arr && arr.Length > 0)
                {
                    source = arr.GetValue(0);
                }

                IDictionary<string, object?> dict;
                if (source is IDictionary<string, object?> d)
                {
                    dict = d;
                }
                else if (source is System.Text.Json.JsonElement je)
                {
                    if (je.ValueKind != System.Text.Json.JsonValueKind.Object)
                        throw new Exception("GETJSONPROPERTY: JSON root must be an object");
                    dict = JsonElementToObject(je) as IDictionary<string, object?>
                        ?? throw new Exception("GETJSONPROPERTY: failed to parse JSON object");
                }
                else if (source is string s)
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(s);
                    if (doc.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
                        throw new Exception("GETJSONPROPERTY: JSON root must be an object");
                    dict = JsonElementToObject(doc.RootElement) as IDictionary<string, object?>
                        ?? throw new Exception("GETJSONPROPERTY: failed to parse JSON object");
                }
                else
                {
                    throw new Exception("GETJSONPROPERTY: obj must be a dictionary or JSON string");
                }
                var key = args[1]?.ToString() ?? string.Empty;
                return dict.TryGetValue(key, out var val) ? val : null;
            }
            ,
            ["FIRST"] = (args, _) =>
            {
                if (args.Length < 1) throw new Exception("FIRST requires array");
                if (args[0] is IEnumerable<object?> seq)
                {
                    using var e = seq.GetEnumerator();
                    if (!e.MoveNext()) return null;
                    var first = e.Current;
                    if (first is string s1 && s1.Length > 1 && s1[0] == '[' && s1[^1] == ']')
                    {
                        try
                        {
                            using var doc = System.Text.Json.JsonDocument.Parse(s1);
                            if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                            {
                                var en = doc.RootElement.EnumerateArray();
                                if (en.MoveNext()) return en.Current.GetRawText();
                                return null;
                            }
                        }
                        catch { }
                    }
                    // If complex type, return JSON string
                    if (first is IDictionary<string, object?> || (first is IEnumerable<object?> && first is not string))
                    {
                        return System.Text.Json.JsonSerializer.Serialize(first);
                    }
                    return first;
                }
                if (args[0] is Array arr) return arr.Length > 0 ? arr.GetValue(0) : null;
                // Try to parse JSON array string
                var s = args[0]?.ToString() ?? string.Empty;
                if (s.StartsWith("[") && s.EndsWith("]"))
                {
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(s);
                        if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            var enumerator = doc.RootElement.EnumerateArray();
                            if (enumerator.MoveNext()) return enumerator.Current.GetRawText();
                            return null;
                        }
                    }
                    catch { /* fallthrough */ }
                }
                throw new Exception("FIRST: argument must be an enumerable");
            }
            ,
            // Math & numeric
            ["MOD"] = (args, _) => ((Convert.ToDouble(args[0]) % Convert.ToDouble(args[1])) + Convert.ToDouble(args[1])) % Convert.ToDouble(args[1])
            ,
            ["POW"] = (args, _) => Math.Pow(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]))
            ,
            ["ROUND"] = (args, _) =>
            {
                var val = Convert.ToDouble(args[0]);
                var digits = args.Length > 1 ? Convert.ToInt32(args[1]) : 0;
                return Math.Round(val, digits, MidpointRounding.AwayFromZero);
            }
            ,
            ["MAX"] = (args, _) => args.Select(Convert.ToDouble).Max()
            ,
            ["MIN"] = (args, _) => args.Select(Convert.ToDouble).Min()
            ,
            // Strings & collections
            ["SPLIT"] = (args, _) =>
            {
                var sep = args.ElementAtOrDefault(1)?.ToString() ?? string.Empty;
                if (args[0] is IEnumerable<object?> seq && args[0] is not string)
                {
                    var parts = new List<string>();
                    foreach (var item in seq)
                    {
                        var s = item?.ToString() ?? string.Empty;
                        if (sep.Length == 0) { foreach (var ch in s) parts.Add(ch.ToString()); }
                        else parts.AddRange(s.Split(sep));
                    }
                    return parts.ToArray();
                }
                var input = args[0]?.ToString() ?? string.Empty;
                return input.Split(sep);
            }
            ,
            ["REPLACE"] = (args, _) => (args[0]?.ToString() ?? string.Empty).Replace(args[1]?.ToString() ?? string.Empty, args[2]?.ToString() ?? string.Empty)
            ,
            ["INDEXOF"] = (args, _) => (args[0]?.ToString() ?? string.Empty).IndexOf(args[1]?.ToString() ?? string.Empty, StringComparison.Ordinal)
            ,
            ["JOIN"] = (args, _) => (args[0] as IEnumerable<object?>) != null ? string.Join(args.ElementAtOrDefault(1)?.ToString() ?? string.Empty, (IEnumerable<object?>)args[0]!) : (args[0]?.ToString() ?? string.Empty)
            ,
            ["LAST"] = (args, _) =>
            {
                if (args[0] is IList<object?> list) return list.Count > 0 ? list[^1] : null;
                if (args[0] is IEnumerable<object?> seq) return seq.LastOrDefault();
                return null;
            }
            ,
            ["COUNT"] = (args, _) =>
            {
                if (args[0] is IEnumerable<object?> seq) return seq.Count();
                return (args[0]?.ToString() ?? string.Empty).Length;
            }
            ,
            ["SKIP"] = (args, _) => (args[0] as IEnumerable<object?>)?.Skip(Convert.ToInt32(args[1])).ToList() ?? args[0]
            ,
            ["TAKE"] = (args, _) => (args[0] as IEnumerable<object?>)?.Take(Convert.ToInt32(args[1])).ToList() ?? args[0]
            ,
            ["APPEND"] = (args, _) =>
            {
                if (args[0] is IEnumerable<object?> seq) return seq.Concat(new[]{ args[1] }).ToList();
                return new List<object?> { args[0], args[1] };
            }
            ,
            ["INCLUDES"] = (args, _) =>
            {
                if (args[0] is IEnumerable<object?> seq) return seq.Any(x => Equals(x, args[1]));
                return (args[0]?.ToString() ?? string.Empty).Contains(args[1]?.ToString() ?? string.Empty, StringComparison.Ordinal);
            }
            ,
            ["INTERSECTION"] = (args, _) =>
            {
                if (args[0] is IEnumerable<object?> a && args[1] is IEnumerable<object?> b) return a.Intersect(b).ToList();
                return new List<object?>();
            }
            ,
            ["UNION"] = (args, _) =>
            {
                if (args[0] is IEnumerable<object?> a && args[1] is IEnumerable<object?> b) return a.Union(b).ToList();
                return new List<object?>();
            }
            ,
            ["SUMBYPROPERTY"] = (args, _) =>
            {
                if (args[0] is IEnumerable<object?> list)
                {
                    var prop = args[1]?.ToString() ?? string.Empty;
                    double sum = 0;
                    foreach (var it in list)
                    {
                        if (it is IDictionary<string, object?> dict2 && dict2.TryGetValue(prop, out var val)) sum += Convert.ToDouble(val);
                    }
                    return sum;
                }
                return 0d;
            }
            ,
            ["CONCATBYPROPERTY"] = (args, _) =>
            {
                if (args[0] is IEnumerable<object?> list)
                {
                    var prop = args[1]?.ToString() ?? string.Empty;
                    var sep = args.ElementAtOrDefault(2)?.ToString() ?? string.Empty;
                    return string.Join(sep, list.Select(it => it is IDictionary<string, object?> d && d.TryGetValue(prop, out var v) ? v?.ToString() ?? string.Empty : string.Empty));
                }
                return string.Empty;
            }
            ,
            ["EMPTY"] = (args, _) =>
            {
                var v = args[0];
                if (v is null) return true;
                if (v is string s) return s.Length == 0;
                if (v is IEnumerable<object?> seq) return !seq.Any();
                if (v is IDictionary<string, object?> dict) return dict.Count == 0;
                return false;
            }
            ,
            ["BETWEEN"] = (args, _) =>
            {
                var val = Convert.ToDouble(args[0]);
                var min = Convert.ToDouble(args[1]);
                var max = Convert.ToDouble(args[2]);
                var inclusive = args.Length > 3 && Convert.ToBoolean(args[3]);
                return inclusive ? val >= min && val <= max : val > min && val < max;
            }
            ,
            // Date & time
            ["ADDDAYS"] = (args, _) => DateTime.Parse(args[0]?.ToString()!).AddDays(Convert.ToDouble(args[1])).ToUniversalTime().ToString("o")
            ,
            ["ADDHOURS"] = (args, _) => DateTime.Parse(args[0]?.ToString()!).AddHours(Convert.ToDouble(args[1])).ToUniversalTime().ToString("o")
            ,
            ["ADDMINUTES"] = (args, _) => DateTime.Parse(args[0]?.ToString()!).AddMinutes(Convert.ToDouble(args[1])).ToUniversalTime().ToString("o")
            ,
            ["ADDSECONDS"] = (args, _) => DateTime.Parse(args[0]?.ToString()!).AddSeconds(Convert.ToDouble(args[1])).ToUniversalTime().ToString("o")
            ,
            ["FORMATDATETIME"] = (args, _) => DateTime.Parse(args[0]?.ToString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal).ToString(args[1]?.ToString())
            ,
            ["UTCNOW"] = (args, _) => DateTime.UtcNow.ToString("o")
            ,
            ["TODAY"] = (args, _) => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0,0,0, DateTimeKind.Utc).ToString("o")
            ,
            ["DAYOFWEEK"] = (args, _) => (int)DateTime.Parse(args[0]?.ToString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal).DayOfWeek
            ,
            ["DAYOFMONTH"] = (args, _) => DateTime.Parse(args[0]?.ToString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal).Day
            ,
            ["DAYOFYEAR"] = (args, _) => DateTime.Parse(args[0]?.ToString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal).DayOfYear
            ,
            ["DATEDIFF"] = (args, _) =>
            {
                var a = DateTime.Parse(args[0]?.ToString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal);
                var b = DateTime.Parse(args[1]?.ToString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal);
                var unit = args.ElementAtOrDefault(2)?.ToString() ?? "milliseconds";
                var diff = b - a;
                return unit switch
                {
                    "seconds" => (int)diff.TotalSeconds,
                    "minutes" => (int)diff.TotalMinutes,
                    "hours" => (int)diff.TotalHours,
                    "days" => (int)diff.TotalDays,
                    _ => (long)diff.TotalMilliseconds
                };
            }
            ,
            ["TICKS"] = (args, _) => DateTime.Parse(args[0]?.ToString()!, null, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal).Ticks
            ,
            // Encoding & XML/JSON mutate
            ["BASE64"] = (args, _) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(args[0]?.ToString() ?? string.Empty))
            ,
            ["BASE64TOSTRING"] = (args, _) => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(args[0]?.ToString() ?? string.Empty))
            ,
            ["SETJSONPROPERTY"] = (args, _) =>
            {
                IDictionary<string, object?> dict;
                if (args[0] is IDictionary<string, object?> d)
                {
                    dict = new Dictionary<string, object?>(d, StringComparer.Ordinal);
                }
                else if (args[0] is string s)
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(s);
                    if (doc.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
                        throw new Exception("SETJSONPROPERTY: JSON root must be an object");
                    dict = (JsonElementToObject(doc.RootElement) as IDictionary<string, object?>)!
                        ?? throw new Exception("SETJSONPROPERTY: failed to parse JSON object");
                }
                else
                {
                    throw new Exception("SETJSONPROPERTY: obj must be a dictionary or JSON string");
                }
                dict[args[1]?.ToString() ?? string.Empty] = args[2];
                return dict;
            }
            ,
            ["GETXMLPROPERTY"] = (args, _) =>
            {
                var xmlInput = args[0];
                var xmlString = xmlInput is string sxml ? sxml : xmlInput?.ToString() ?? "<root/>";
                var doc = System.Xml.Linq.XDocument.Parse(xmlString);
                var tag = args[1]?.ToString() ?? string.Empty;
                var el = doc.Descendants(tag).FirstOrDefault();
                return el != null ? el.Value : null;
            }
            ,
            ["SETXMLPROPERTY"] = (args, _) =>
            {
                var xml = args[0]?.ToString() ?? "<root/>";
                var doc = System.Xml.Linq.XDocument.Parse(xml);
                var tag = args[1]?.ToString() ?? string.Empty;
                var value = args[2]?.ToString() ?? string.Empty;
                var el = doc.Descendants(tag).FirstOrDefault();
                if (el == null)
                {
                    if (doc.Root == null) doc.Add(new System.Xml.Linq.XElement("root"));
                    el = new System.Xml.Linq.XElement(tag);
                    doc.Root!.Add(el);
                }
                el.Value = value;
                return doc.ToString(System.Xml.Linq.SaveOptions.DisableFormatting);
            }
        };
    }

    private static object? JsonElementToObject(System.Text.Json.JsonElement element)
    {
        switch (element.ValueKind)
        {
            case System.Text.Json.JsonValueKind.Object:
                var dict = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var prop in element.EnumerateObject())
                {
                    dict[prop.Name] = JsonElementToObject(prop.Value);
                }
                return dict;
            case System.Text.Json.JsonValueKind.Array:
                var list = new List<object?>();
                foreach (var el in element.EnumerateArray()) list.Add(JsonElementToObject(el));
                return list;
            case System.Text.Json.JsonValueKind.String:
                return element.GetString();
            case System.Text.Json.JsonValueKind.Number:
                if (element.TryGetInt64(out var l)) return l;
                if (element.TryGetDouble(out var d)) return d;
                return element.GetRawText();
            case System.Text.Json.JsonValueKind.True:
                return true;
            case System.Text.Json.JsonValueKind.False:
                return false;
            case System.Text.Json.JsonValueKind.Null:
            case System.Text.Json.JsonValueKind.Undefined:
                return null;
            default:
                return element.GetRawText();
        }
    }

    public Func<object?[], IReadOnlyDictionary<string, string>, object?> Resolve(string name)
    {
        if (!_funcs.TryGetValue(name, out var func)) throw new Exception($"Unknown function: {name}");
        return func;
    }
}


