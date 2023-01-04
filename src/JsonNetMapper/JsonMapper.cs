using Newtonsoft.Json.Linq;

namespace JsonNetMapper;

public class JsonMapper
{
    public string Response { get; private set; }

    private static bool _formatValues;
    private readonly JObject _sourceJson;
    private readonly JObject _configJson;
    private readonly JObject _responseJson;
    private const string SELECTOR = "selector";
    private const string VALUE_TYPE = "type";
    private const string VALUE_FORMAT = "format";

    public JsonMapper(string configJson, string sourceJson, bool formatValues = true)
    {
        Response = string.Empty;

        _sourceJson = JObject.Parse(sourceJson);
        _configJson = JObject.Parse(configJson);
        _responseJson = new JObject();
        _formatValues = formatValues;
    }

    public void BuildNewJson()
    {
        foreach (var jsonConfig in _configJson)
        {
            if (jsonConfig.Value != null)
            {
                var selector = jsonConfig.Value.Value<string>(SELECTOR);
                var valueType = jsonConfig.Value.Value<string>(VALUE_TYPE) ?? string.Empty;
                var valueFormat = jsonConfig.Value.Value<string>(VALUE_FORMAT) ?? string.Empty;

                if (selector is not null)
                {
                    _responseJson[jsonConfig.Key] = GetValueFromSourceJson(selector, valueType, valueFormat);
                }
                else
                {
                    var childrens = jsonConfig.Value.Children().ToList();

                    if (childrens.Count <= 0)
                    {
                        continue;
                    }

                    var currentIndex = 0;
                    var hasChildren = true;

                    while (hasChildren)
                    {
                        if (currentIndex < childrens.Count)
                        {
                            var jToken = childrens[currentIndex];
                            var jProperty = jToken.Value<JProperty>();

                            if (jProperty is not null)
                            {
                                selector = jProperty.Value.Value<string>(SELECTOR);
                                valueType = jProperty.Value.Value<string>(VALUE_TYPE) ?? string.Empty;
                                valueFormat = jProperty.Value.Value<string>(VALUE_FORMAT) ?? string.Empty;

                                if (selector is null)
                                {
                                    var child = jToken.Children().Children();
                                    childrens.AddRange(child);
                                }
                                else
                                {                                    
                                    var sourceValue = GetValueFromSourceJson(selector, valueType, valueFormat);
                                    var path = jProperty.Path.Split(".");
                                    var firstPath = path[0];

                                    var objectValue = new JObject
                                    {
                                        [path[^1]] = sourceValue
                                    };

                                    for (int i = 1; i < path.Length - 1; i += 1)
                                    {
                                        var objectAux = new JObject
                                        {
                                            [path[i]] = objectValue
                                        };

                                        objectValue = objectAux;
                                    }

                                    AddJObjectInResponseJson(firstPath, objectValue);
                                }
                            }

                            currentIndex += 1;
                        }
                        else
                        {
                            hasChildren = false;
                        }
                    }
                }
            }
        }

        Response = _responseJson.ToString();
    }

    private void AddJObjectInResponseJson(string objKey, JObject objValue)
    {
        if (_responseJson.TryGetValue(objKey, StringComparison.InvariantCultureIgnoreCase, out JToken? value))
        {
            var auxObj = value.Union(objValue).ToArray();
            var newObj = new JObject
            {
                auxObj[0],
                auxObj[1]
            };

            _responseJson[objKey] = newObj;
        }
        else
        {
            _responseJson[objKey] = objValue;
        }
    }

    private JToken? GetValueFromSourceJson(string selector, string valueType, string valueFormat)
    {
        JToken? sourceValue;

        if (selector.Contains('.', StringComparison.InvariantCultureIgnoreCase))
        {
            sourceValue = _sourceJson.SelectToken(selector);
        }
        else
        {
            sourceValue = _sourceJson.GetValue(selector);            
        }

        FormatValues(ref sourceValue, valueType, valueFormat);

        return sourceValue;
    }

    private static void FormatValues(ref JToken? value, string valueType, string valueFormat)
    {
        switch (valueType)
        {
            case "bool":
                value ??= false;
                break;
            case "string":
                value ??= string.Empty;
                break;
            case "integer":
                value ??= 0;
                break;
            case "decimal":
                value ??= 0.0;
                if (_formatValues) value = string.Format(valueFormat, value.ToString());
                break;
            case "date":
                value ??= string.Empty;
                if (_formatValues) value = value.ToObject<DateTime>().ToString(valueFormat);
                break;
            case "list":
                value ??= new JArray();
                break;
            case "object":
                value ??= new JObject();
                break;
        };
    }
}
