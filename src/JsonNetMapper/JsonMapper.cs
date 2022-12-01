using Newtonsoft.Json.Linq;

namespace JsonNetMapper;

public class JsonMapper
{
    private readonly JObject _sourceJson;
    private readonly JObject _configJson;
    private readonly JObject _responseJson;
    private const string SELECTOR = "selector";

    public JsonMapper(string configJson, string sourceJson)
    {
        _sourceJson = JObject.Parse(sourceJson);
        _configJson = JObject.Parse(configJson);
        _responseJson = new JObject();
    }

    public string BuildNewJson()
    {
        foreach (var jsonConfig in _configJson)
        {
            if (jsonConfig.Value != null)
            {
                var selector = jsonConfig.Value.Value<string>(SELECTOR);

                if (selector is not null)
                {
                    _responseJson[jsonConfig.Key] = GetValueFromSourceJson(selector);
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

                                if (selector is null)
                                {
                                    var child = jToken.Children().Children();
                                    childrens.AddRange(child);
                                }
                                else
                                {

                                    var sourceValue = GetValueFromSourceJson(selector);
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

        return _responseJson.ToString();
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

    private JToken? GetValueFromSourceJson(string selector)
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

        return sourceValue;
    }
}
