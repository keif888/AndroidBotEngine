using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BotEngineClient
{
    public class JsonHelper
    {
        public List<string> Errors;

        public JsonHelper()
        {
            Errors = new List<string>();
        }

        public bool ValidateGameConfigStructure(string jsonGameFileName)
        {
            return true;
        }

        public bool ValidateListConfig(string jsonListFileName)
        {
            JsonDocumentOptions documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };
            JsonNodeOptions nodeOptions = new JsonNodeOptions
            {
                PropertyNameCaseInsensitive = false
            };

            string jsonList = File.ReadAllText(jsonListFileName);
            JsonNode jsonListNode = JsonNode.Parse(jsonList, nodeOptions, documentOptions);
            if (jsonListNode is JsonObject)
            {
                JsonObject jsonObject = (JsonObject)jsonListNode;
                if (!jsonObject.ContainsKey("FileId"))
                {
                    Errors.Add("Required field \"FileId\" missing.  Unable to confirm that the input file is a ListConfigFile");
                }
                else
                {
                    JsonNode fileIdValue = jsonObject["FileId"];
                    if (fileIdValue is JsonValue)
                    {
                        JsonElement value = fileIdValue.GetValue<JsonElement>();
                        if (value.ValueKind == JsonValueKind.String)
                        {
                            string fileId = value.GetString();
                            if (fileId.ToLower() != "listconfig")
                            {
                                Errors.Add(string.Format("\"FileId\" indicates that this is not \"ListConfig\" but {0}", fileId));
                            }
                        }
                        else
                        {
                            Errors.Add(string.Format("\"FileId\" value is of incorrect type.  Expecting a String but got {0}", value.ValueKind));
                        }
                    }
                    else
                    {
                        Errors.Add("Required field \"FileId\" is of the wrong type.  Expecting a String Value");
                    }
                }

                if (!jsonObject.ContainsKey("Coordinates"))
                {
                    Errors.Add("Required field \"Coordinates\" missing.");
                }
                else
                {
                    JsonNode coordinatesNode = jsonObject["Coordinates"];
                    if (coordinatesNode is JsonObject)
                    {
                        JsonObject coordinatesObject = (JsonObject)coordinatesNode;
                        foreach (KeyValuePair<string,JsonNode?>  coordItem in coordinatesObject)
                        {
                            if (coordItem.Value is JsonArray)
                            {
                                JsonArray coordArray = coordItem.Value.AsArray();
                                foreach (JsonNode arrayItem in coordArray)
                                {
                                    if (arrayItem is JsonObject)
                                    {
                                        JsonObject arrayObject = (JsonObject)arrayItem;
                                        if (arrayObject.ContainsKey("X"))
                                        {
                                            JsonElement value = arrayObject["X"].GetValue<JsonElement>();
                                            if (value.ValueKind != JsonValueKind.Number)
                                                Errors.Add(string.Format("Coordinates list item \"X\" at path {0} is is of the wrong type.  Was expecting Number but found String", arrayObject.GetPath(), value.ValueKind));
                                        }
                                        else
                                        {
                                            Errors.Add(string.Format("Coordinates list item at path {0} is missing required element \"X\"", arrayObject.GetPath()));
                                        }
                                        if (arrayObject.ContainsKey("Y"))
                                        {
                                            JsonElement value = arrayObject["Y"].GetValue<JsonElement>();
                                            if (value.ValueKind != JsonValueKind.Number)
                                                Errors.Add(string.Format("Coordinates list item \"Y\" at path {0} is is of the wrong type.  Was expecting Number but found String", arrayObject.GetPath(), value.ValueKind));
                                        }
                                        else
                                        {
                                            Errors.Add(string.Format("Coordinates list item at path {0} is missing required element \"Y\"", arrayObject.GetPath()));
                                        }
                                    }
                                    else
                                    {
                                        JsonElement jsonElement = arrayItem.GetValue<JsonElement>();
                                        Errors.Add(string.Format("Coordinates list item {0} at path {1} is of the wrong type.  Was expecting Object, but found {2}", arrayItem.ToJsonString(), arrayItem.GetPath(), jsonElement.ValueKind));
                                    }
                                }
                            }
                            else
                            {
                                JsonElement jsonElement = coordItem.Value.GetValue<JsonElement>();
                                Errors.Add(string.Format("Coordinates item {0} at path {1} is of the wrong type.  Was expecting Array, but found {2}", coordItem.Key, coordItem.Value.GetPath(), jsonElement.ValueKind));
                            }
                        }
                    }
                    else
                    {
                        Errors.Add("Required field \"Coordinates\" is of the wrong type.  Expecting an Object with one or more named arrays of X/Y value pairs");
                    }
                }
            }
            else
            {
                Errors.Add(string.Format("Root of {0} was not a json Object, was expecting {\"FileId\": \"ListConfig\",", jsonListFileName));
            }
            JsonNode root = jsonListNode.Root;
            if (Errors.Count == 0)
                return true;
            else
                return false;
        }

        public bool ValidateDeviceConfigStructure(string jsonDeviceFileName)
        {
            return true;
        }
    }
}
