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
            int startErrorCount = Errors.Count;
            JsonDocumentOptions documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };
            JsonNodeOptions nodeOptions = new JsonNodeOptions
            {
                PropertyNameCaseInsensitive = false
            };

            string jsonList = File.ReadAllText(jsonGameFileName);
            JsonNode jsonListNode = JsonNode.Parse(jsonList, nodeOptions, documentOptions);
            if (jsonListNode is JsonObject jsonObject)
            {
                if (!jsonObject.ContainsKey("FileId"))
                {
                    Errors.Add("Required field \"FileId\" missing.  Unable to confirm that the input file is a GameConfigFile");
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
                            if (fileId.ToLower() != "gameconfig")
                            {
                                Errors.Add(string.Format("\"FileId\" indicates that this is not \"GameConfig\" but {0}", fileId));
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

                if (!jsonObject.ContainsKey("findStrings"))
                {
                    Errors.Add("Required root field \"findStrings\" missing.");
                }
                else
                {
                    JsonNode rootNode = jsonObject["findStrings"];
                    if (!(rootNode is JsonObject findStringsObject))
                    {
                        Errors.Add("Required root field \"findStrings\" is of the wrong type.  Expecting an Object with one or more named objects of FindStrings data");
                    }
                    else
                    {
                        foreach (KeyValuePair<string, JsonNode?> findStringsItem in findStringsObject)
                        {
                            if (!(findStringsItem.Value is JsonObject findStringJsonObject))
                            {
                                JsonElement jsonElement = findStringsItem.Value.GetValue<JsonElement>();
                                Errors.Add(string.Format("findStrings item {0} at path {1} is of the wrong type.  Was expecting Object, but found {2}", findStringsItem.Key, findStringsItem.Value.GetPath(), jsonElement.ValueKind));
                            }
                            else
                            {
                                // Check for required fields.
                                if (!findStringJsonObject.ContainsKey("findString"))
                                {
                                    Errors.Add(string.Format("findStrings item {0} at path {1} is missing required field \"findString\"", findStringsItem.Key, findStringsItem.Value.GetPath()));
                                }
                                else
                                {
                                    if (!(findStringJsonObject["findString"] is JsonValue))
                                    {
                                        Errors.Add(string.Format("findStrings list item \"findString\" at path {0} is of the wrong type.  Was expecting String but found {1}", findStringJsonObject["findString"].GetPath(), findStringJsonObject["findString"].GetType()));
                                    }
                                    else
                                    {
                                        JsonElement itemNode = findStringJsonObject["findString"].GetValue<JsonElement>();
                                        if (itemNode.ValueKind != JsonValueKind.String)
                                            Errors.Add(string.Format("findStrings list item \"findString\" at path {0} is of the wrong type.  Was expecting String but found {1}", findStringJsonObject["findString"].GetPath(), itemNode.ValueKind));
                                    }
                                }
                                if (!findStringJsonObject.ContainsKey("searchArea"))
                                {
                                    Errors.Add(string.Format("findStrings item {0} at path {1} is missing required field \"searchArea\"", findStringsItem.Key, findStringsItem.Value.GetPath()));
                                }
                                else
                                {
                                    JsonNode itemNode = findStringJsonObject["searchArea"];
                                    if (!(itemNode is JsonObject))
                                        Errors.Add(string.Format("findStrings list item \"searchArea\" at path {0} is of the wrong type.  Was expecting Object containing a search area", findStringJsonObject["searchArea"].GetPath()));
                                    else
                                    {
                                        JsonObject searchAreaObject = (JsonObject)itemNode;
                                        if (!searchAreaObject.ContainsKey("X"))
                                        {
                                            Errors.Add(string.Format("findStrings item {0} at path {1} is missing required field \"X\"", "searchArea", searchAreaObject.GetPath()));
                                        }
                                        // ToDo: Handle data type check
                                        if (!searchAreaObject.ContainsKey("Y"))
                                        {
                                            Errors.Add(string.Format("findStrings item {0} at path {1} is missing required field \"Y\"", "searchArea", searchAreaObject.GetPath()));
                                        }
                                        // ToDo: Handle data type check
                                        if (!searchAreaObject.ContainsKey("width"))
                                        {
                                            Errors.Add(string.Format("findStrings item {0} at path {1} is missing required field \"width\"", "searchArea", searchAreaObject.GetPath()));
                                        }
                                        // ToDo: Handle data type check
                                        if (!searchAreaObject.ContainsKey("height"))
                                        {
                                            Errors.Add(string.Format("findStrings item {0} at path {1} is missing required field \"height\"", "searchArea", searchAreaObject.GetPath()));
                                        }
                                        // ToDo: Handle data type check
                                    }
                                }
                                if (!findStringJsonObject.ContainsKey("textTolerance"))
                                {
                                    Errors.Add(string.Format("findStrings item {0} at path {1} is missing required field \"textTolerance\"", findStringsItem.Key, findStringsItem.Value.GetPath()));
                                }
                                else
                                {
                                    // ToDo: Correct this so that it handles arrays/objects etc.
                                    JsonElement itemNode = findStringJsonObject["textTolerance"].GetValue<JsonElement>();
                                    if (itemNode.ValueKind != JsonValueKind.Number)
                                        Errors.Add(string.Format("findStrings list item \"textTolerance\" at path {0} is of the wrong type.  Was expecting Number but found {1}", findStringJsonObject["textTolerance"].GetPath(), itemNode.ValueKind));
                                }
                                if (!findStringJsonObject.ContainsKey("backgroundTolerance"))
                                {
                                    Errors.Add(string.Format("findStrings item {0} at path {1} is missing required field \"backgroundTolerance\"", findStringsItem.Key, findStringsItem.Value.GetPath()));
                                }
                                else
                                {
                                    // ToDo: Correct this so that it handles arrays/objects etc.
                                    JsonElement itemNode = findStringJsonObject["backgroundTolerance"].GetValue<JsonElement>();
                                    if (itemNode.ValueKind != JsonValueKind.Number)
                                        Errors.Add(string.Format("findStrings list item \"backgroundTolerance\" at path {0} is of the wrong type.  Was expecting Number but found {1}", findStringJsonObject["backgroundTolerance"].GetPath(), itemNode.ValueKind));
                                }
                            }
                        }
                    }
                }

                if (!jsonObject.ContainsKey("systemActions"))
                {
                    Errors.Add("Required root field \"systemActions\" missing.");
                }
                else
                {
                    JsonNode rootNode = jsonObject["systemActions"];
                    if (!(rootNode is JsonObject systemActionsObject))
                    {
                        Errors.Add("Required root field \"systemActions\" is of the wrong type.  Expecting an Object with one or more named objects of Action data");
                    }
                    else
                    {
                        foreach (KeyValuePair<string, JsonNode?> systemActionsItem in systemActionsObject)
                        {
                            if (!(systemActionsItem.Value is JsonObject systemActionJsonObject))
                            {
                                JsonElement jsonElement = systemActionsItem.Value.GetValue<JsonElement>();
                                Errors.Add(string.Format("findStrings item {0} at path {1} is of the wrong type.  Was expecting Object, but found {2}", systemActionsItem.Key, systemActionsItem.Value.GetPath(), jsonElement.ValueKind));
                            }
                            else
                            {
                                // Check for required fields.
                                if (!systemActionJsonObject.ContainsKey("ActionType"))
                                {
                                    Errors.Add(string.Format("systemActions item {0} at path {1} is missing required field \"ActionType\"", systemActionsItem.Key, systemActionsItem.Value.GetPath()));
                                }
                                if (!systemActionJsonObject.ContainsKey("Commands"))
                                {
                                    Errors.Add(string.Format("systemActions item {0} at path {1} is missing required field \"Commands\"", systemActionsItem.Key, systemActionsItem.Value.GetPath()));
                                }
                                // ToDo: Validate the required fields types.
                            }
                        }
                    }

                }

                if (!jsonObject.ContainsKey("actions"))
                {
                    Errors.Add("Required root field \"actions\" missing.");
                }
                else
                {
                    JsonNode rootNode = jsonObject["actions"];
                    if (!(rootNode is JsonObject actionsObject))
                    {
                        Errors.Add("Required root field \"actions\" is of the wrong type.  Expecting an Object with one or more named objects of Action data");
                    }
                    else
                    {
                        foreach (KeyValuePair<string, JsonNode?> actionsItem in actionsObject)
                        {
                            if (!(actionsItem.Value is JsonObject systemActionJsonObject))
                            {
                                JsonElement jsonElement = actionsItem.Value.GetValue<JsonElement>();
                                Errors.Add(string.Format("findStrings item {0} at path {1} is of the wrong type.  Was expecting Object, but found {2}", actionsItem.Key, actionsItem.Value.GetPath(), jsonElement.ValueKind));
                            }
                            else
                            {
                                // Check for required fields.
                                if (!systemActionJsonObject.ContainsKey("ActionType"))
                                {
                                    Errors.Add(string.Format("actions item {0} at path {1} is missing required field \"ActionType\"", actionsItem.Key, actionsItem.Value.GetPath()));
                                }
                                if (!systemActionJsonObject.ContainsKey("Commands"))
                                {
                                    Errors.Add(string.Format("actions item {0} at path {1} is missing required field \"Commands\"", actionsItem.Key, actionsItem.Value.GetPath()));
                                }
                                // ToDo: Validate the required fields types.
                            }
                        }
                    }
                }


            }
            else
            {
                Errors.Add(string.Format("Root of {0} was not a json Object, was expecting {\"FileId\": \"ListConfig\",", jsonGameFileName));
            }



            if (Errors.Count == startErrorCount)
                return true;
            else
                return false;
        }

        public bool ValidateListConfig(string jsonListFileName)
        {
            int startErrorCount = Errors.Count;
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
                    JsonNode rootNode = jsonObject["Coordinates"];
                    if (rootNode is JsonObject)
                    {
                        JsonObject coordinatesObject = (JsonObject)rootNode;
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
                                            // ToDo: Correct this so that it handles arrays/objects etc.
                                            JsonElement value = arrayObject["X"].GetValue<JsonElement>();
                                            if (value.ValueKind != JsonValueKind.Number)
                                                Errors.Add(string.Format("Coordinates list item \"X\" at path {0} is of the wrong type.  Was expecting Number but found {1}", arrayObject.GetPath(), value.ValueKind));
                                        }
                                        else
                                        {
                                            Errors.Add(string.Format("Coordinates list item at path {0} is missing required element \"X\"", arrayObject.GetPath()));
                                        }
                                        if (arrayObject.ContainsKey("Y"))
                                        {
                                            // ToDo: Correct this so that it handles arrays/objects etc.
                                            JsonElement value = arrayObject["Y"].GetValue<JsonElement>();
                                            if (value.ValueKind != JsonValueKind.Number)
                                                Errors.Add(string.Format("Coordinates list item \"Y\" at path {0} is of the wrong type.  Was expecting Number but found {1}", arrayObject.GetPath(), value.ValueKind));
                                        }
                                        else
                                        {
                                            Errors.Add(string.Format("Coordinates list item at path {0} is missing required element \"Y\"", arrayObject.GetPath()));
                                        }
                                    }
                                    else
                                    {
                                        // ToDo: Correct this so that it handles arrays/objects etc.
                                        JsonElement jsonElement = arrayItem.GetValue<JsonElement>();
                                        Errors.Add(string.Format("Coordinates list item {0} at path {1} is of the wrong type.  Was expecting Object, but found {2}", arrayItem.ToJsonString(), arrayItem.GetPath(), jsonElement.ValueKind));
                                    }
                                }
                            }
                            else
                            {
                                // ToDo: Correct this so that it handles arrays/objects etc.
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

            if (Errors.Count == startErrorCount)
                return true;
            else
                return false;
        }

        public bool ValidateDeviceConfigStructure(string jsonDeviceFileName)
        {
            int startErrorCount = Errors.Count;

            //ToDo: Validate Device Config.

            if (Errors.Count == startErrorCount)
                return true;
            else
                return false;
        }
    }
}
