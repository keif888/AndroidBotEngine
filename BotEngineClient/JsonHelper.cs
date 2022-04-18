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
                                Errors.Add(string.Format("findStrings item {0} at path {1} is of the wrong type.  Was expecting Object with zero or more named findString data", findStringsItem.Key, findStringsItem.Value.GetPath()));
                            }
                            else
                            {
                                // Check for required fields.
                                if (!findStringJsonObject.ContainsKey("findString"))
                                {
                                    Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is missing required field \"findString\"", findStringsItem.Key, findStringsItem.Value.GetPath()));
                                }
                                else
                                {
                                    if (!(findStringJsonObject["findString"] is JsonValue))
                                    {
                                        Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", findStringsItem.Key, findStringJsonObject["findString"].GetPath(), findStringJsonObject["findString"].GetType()));
                                    }
                                    else
                                    {
                                        JsonElement itemNode = findStringJsonObject["findString"].GetValue<JsonElement>();
                                        if (itemNode.ValueKind != JsonValueKind.String)
                                            Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", findStringsItem.Key, findStringJsonObject["findString"].GetPath(), itemNode.ValueKind));
                                    }
                                }
                                if (!findStringJsonObject.ContainsKey("searchArea"))
                                {
                                    Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is missing required field \"searchArea\"", findStringsItem.Key, findStringsItem.Value.GetPath()));
                                }
                                else
                                {
                                    JsonNode itemNode = findStringJsonObject["searchArea"];
                                    if (!(itemNode is JsonObject))
                                        Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Object containing a search area", findStringsItem.Key, findStringJsonObject["searchArea"].GetPath()));
                                    else
                                    {
                                        JsonObject searchAreaObject = (JsonObject)itemNode;
                                        if (!searchAreaObject.ContainsKey("X"))
                                        {
                                            Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is missing required field \"X\"", findStringsItem.Key, searchAreaObject.GetPath()));
                                        }
                                        else
                                        {
                                            if (!(searchAreaObject["X"] is JsonValue))
                                            {
                                                Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, searchAreaObject["X"].GetPath(), searchAreaObject["X"].GetType()));
                                            }
                                            else
                                            {
                                                JsonElement searchAreaItemNode = searchAreaObject["X"].GetValue<JsonElement>();
                                                if (searchAreaItemNode.ValueKind != JsonValueKind.Number)
                                                    Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, searchAreaObject["X"].GetPath(), searchAreaItemNode.ValueKind));
                                            }
                                        }
                                        if (!searchAreaObject.ContainsKey("Y"))
                                        {
                                            Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is missing required field \"Y\"", findStringsItem.Key, searchAreaObject.GetPath()));
                                        }
                                        else
                                        {
                                            if (!(searchAreaObject["Y"] is JsonValue))
                                            {
                                                Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, searchAreaObject["Y"].GetPath(), searchAreaObject["Y"].GetType()));
                                            }
                                            else
                                            {
                                                JsonElement searchAreaItemNode = searchAreaObject["Y"].GetValue<JsonElement>();
                                                if (searchAreaItemNode.ValueKind != JsonValueKind.Number)
                                                    Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, searchAreaObject["Y"].GetPath(), searchAreaItemNode.ValueKind));
                                            }
                                        }
                                        if (!searchAreaObject.ContainsKey("width"))
                                        {
                                            Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is missing required field \"width\"", findStringsItem.Key, searchAreaObject.GetPath()));
                                        }
                                        else
                                        {
                                            if (!(searchAreaObject["width"] is JsonValue))
                                            {
                                                Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, searchAreaObject["width"].GetPath(), searchAreaObject["width"].GetType()));
                                            }
                                            else
                                            {
                                                JsonElement searchAreaItemNode = searchAreaObject["width"].GetValue<JsonElement>();
                                                if (searchAreaItemNode.ValueKind != JsonValueKind.Number)
                                                    Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, searchAreaObject["width"].GetPath(), searchAreaItemNode.ValueKind));
                                            }
                                        }
                                        if (!searchAreaObject.ContainsKey("height"))
                                        {
                                            Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is missing required field \"height\"", findStringsItem.Key, searchAreaObject.GetPath()));
                                        }
                                        else
                                        {
                                            if (!(searchAreaObject["height"] is JsonValue))
                                            {
                                                Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, searchAreaObject["height"].GetPath(), searchAreaObject["height"].GetType()));
                                            }
                                            else
                                            {
                                                JsonElement searchAreaItemNode = searchAreaObject["height"].GetValue<JsonElement>();
                                                if (searchAreaItemNode.ValueKind != JsonValueKind.Number)
                                                    Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, searchAreaObject["height"].GetPath(), searchAreaItemNode.ValueKind));
                                            }
                                        }
                                    }
                                }
                                if (!findStringJsonObject.ContainsKey("textTolerance"))
                                {
                                    Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is missing required field \"textTolerance\"", findStringsItem.Key, findStringsItem.Value.GetPath()));
                                }
                                else
                                {
                                    if (!(findStringJsonObject["textTolerance"] is JsonValue))
                                    {
                                        Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, findStringJsonObject["textTolerance"].GetPath(), findStringJsonObject["textTolerance"].GetType()));
                                    }
                                    else
                                    {
                                        JsonElement itemNode = findStringJsonObject["textTolerance"].GetValue<JsonElement>();
                                        if (itemNode.ValueKind != JsonValueKind.Number)
                                            Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, findStringJsonObject["textTolerance"].GetPath(), itemNode.ValueKind));
                                    }
                                }
                                if (!findStringJsonObject.ContainsKey("backgroundTolerance"))
                                {
                                    Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is missing required field \"backgroundTolerance\"", findStringsItem.Key, findStringsItem.Value.GetPath()));
                                }
                                else
                                {
                                    if (!(findStringJsonObject["backgroundTolerance"] is JsonValue))
                                    {
                                        Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, findStringJsonObject["backgroundTolerance"].GetPath(), findStringJsonObject["backgroundTolerance"].GetType()));
                                    }
                                    else
                                    {
                                        JsonElement itemNode = findStringJsonObject["backgroundTolerance"].GetValue<JsonElement>();
                                        if (itemNode.ValueKind != JsonValueKind.Number)
                                            Errors.Add(string.Format("findStrings list item \"{0}\" at path {1} is of the wrong type.  Was expecting Number but found {2}", findStringsItem.Key, findStringJsonObject["backgroundTolerance"].GetPath(), itemNode.ValueKind));
                                    }
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
                                Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Object with zero or more named objects contained within.", systemActionsItem.Key, systemActionsItem.Value.GetPath()));
                            }
                            else
                            {
                                // Check for required fields.
                                if (!systemActionJsonObject.ContainsKey("ActionType"))
                                {
                                    Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is missing required field \"ActionType\"", systemActionsItem.Key, systemActionsItem.Value.GetPath()));
                                }
                                else
                                {
                                    if (!(systemActionJsonObject["ActionType"] is JsonValue))
                                    {
                                        Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", systemActionsItem.Key, systemActionJsonObject["ActionType"].GetPath(), systemActionJsonObject["ActionType"].GetType()));
                                    }
                                    else
                                    {
                                        JsonElement itemNode = systemActionJsonObject["ActionType"].GetValue<JsonElement>();
                                        if (itemNode.ValueKind != JsonValueKind.String)
                                            Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", systemActionsItem.Key, systemActionJsonObject["ActionType"].GetPath(), itemNode.ValueKind));
                                        else
                                        {
                                            switch (itemNode.GetString().ToLower())
                                            {
                                                case "system":
                                                case "scheduled":
                                                case "daily":
                                                case "always":
                                                    break;
                                                default:
                                                    Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} with value \"{2}\" is not valid.  Was expecting one of the following \"System\", \"Scheduled\", \"Daily\", \"Always\"", systemActionsItem.Key, systemActionJsonObject["ActionType"].GetPath(), itemNode.GetString()));
                                                    break;
                                            }
                                        }
                                    }
                                }
                                if (!systemActionJsonObject.ContainsKey("Commands"))
                                {
                                    Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is missing required field \"Commands\"", systemActionsItem.Key, systemActionsItem.Value.GetPath()));
                                }
                                else
                                {
                                    if (!(systemActionJsonObject["Commands"] is JsonArray commandsJsonArray))
                                    {
                                        Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Array but found {2}", systemActionsItem.Key, systemActionJsonObject["Commands"].GetPath(), systemActionJsonObject["Commands"].GetType()));
                                    }
                                    else
                                    {
                                        ValidateCommands(systemActionsItem.Key, commandsJsonArray);
                                    }
                                }
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

        private void ValidateCommands(string listItemName, JsonArray commandsJsonArray)
        {
            string Key;
            foreach (JsonNode commandsItem in commandsJsonArray)
            {
                if (!(commandsItem is JsonObject commandsObject))
                {
                    if (!(commandsItem is JsonValue))
                        Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Array but found {2}", listItemName, commandsItem.GetPath(), commandsItem.GetType()));
                    else
                        Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Array but found {2}", listItemName, commandsItem.GetPath(), commandsItem.GetValue<JsonElement>().ValueKind));
                }
                else
                {
                    // ToDo: Parse the rest of a command, and ensure it's valid.
                    // Need to handle each of the varying Commands, and ensure they are valid as well.
                    string CommandId = string.Empty;
                    Key = "CommandId";
                    if (!commandsObject.ContainsKey(Key))
                    {
                        Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is missing required field \"{2}\"", listItemName, commandsItem.GetPath(), Key));
                    }
                    else
                    {
                        if (!(commandsObject[Key] is JsonValue))
                        {
                            Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", listItemName, commandsObject[Key].GetPath(), commandsObject[Key].GetType()));
                        }
                        else
                        {
                            JsonElement itemNode = commandsObject[Key].GetValue<JsonElement>();
                            if (itemNode.ValueKind != JsonValueKind.String)
                                Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", listItemName, commandsObject[Key].GetPath(), itemNode.ValueKind));
                            else
                            {
                                switch (itemNode.GetString().ToLower())
                                {
                                    case "click":
                                    case "clickwhennotfoundinarea":
                                    case "drag":
                                    case "exit":
                                    case "enterloopcoordinate":
                                    case "findclick":
                                    case "findclickandwait":
                                    case "ifexists":
                                    case "ifnotexists":
                                    case "loopcoordinates":
                                    case "loopuntilfound":
                                    case "loopuntilnotfound":
                                    case "restart":
                                    case "runaction":
                                    case "sleep":
                                    case "startgame":
                                    case "stopgame":
                                    case "waitfor":
                                    case "waitforthenclick":
                                    case "waitforchange":
                                    case "waitfornochange":
                                        CommandId = itemNode.GetString().ToLower();
                                        break;
                                    default:
                                        Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} with value \"{2}\" is not valid.  Was expecting a Command like one of the following \"WaitFor\", \"Click\", \"IfExists\", \"FindClickAndWait\"", listItemName, commandsObject[Key].GetPath(), itemNode.GetString()));
                                        break;
                                }
                            }
                        }
                    }

                    ValidateJsonValue("systemActions", listItemName, "CommandNumber", commandsItem, commandsObject, JsonValueKind.Number);

                    if (!string.IsNullOrEmpty(CommandId))
                    {
                        switch (CommandId)
                        {
                            case "click":
                                Key = "Location";
                                if (!commandsObject.ContainsKey(Key))
                                {
                                    Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is missing required field \"{2}\"", listItemName, commandsItem.GetPath(), Key));
                                }
                                else
                                {
                                    if (!(commandsObject[Key] is JsonObject))
                                    {
                                        Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Object with X/Y but found {2}", listItemName, commandsObject[Key].GetPath(), commandsObject[Key].GetType()));
                                    }
                                    else
                                    {
                                        ValidateJsonValue("systemActions", listItemName, "X", commandsItem, commandsObject[Key].AsObject(), JsonValueKind.Number);
                                        ValidateJsonValue("systemActions", listItemName, "Y", commandsItem, commandsObject[Key].AsObject(), JsonValueKind.Number);
                                    }
                                }
                                break;
                            case "clickwhennotfoundinarea":
                                ValidateJsonValue("systemActions", listItemName, "ImageName", commandsItem, commandsObject, JsonValueKind.String);
                                Key = "Areas";
                                if (!commandsObject.ContainsKey(Key))
                                {
                                    Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is missing required field \"{2}\"", listItemName, commandsItem.GetPath(), Key));
                                }
                                else
                                {
                                    if (!(commandsObject[Key] is JsonArray))
                                    {
                                        Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Array with X/Y/width/height objects but found {2}", listItemName, commandsObject[Key].GetPath(), commandsObject[Key].GetType()));
                                    }
                                    else
                                    {
                                        foreach (JsonNode areasItem in commandsObject[Key].AsArray())
                                        {
                                            if (!(areasItem is JsonObject))
                                            {
                                                Errors.Add(string.Format("systemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Object with X/Y but found {2}", listItemName, areasItem.GetPath(), areasItem.GetType()));
                                            }
                                            else
                                            {
                                                JsonObject areasObject = (JsonObject)areasItem;
                                                ValidateJsonValue("systemActions", listItemName, "X", areasItem, areasObject, JsonValueKind.Number);
                                                ValidateJsonValue("systemActions", listItemName, "Y", areasItem, areasObject, JsonValueKind.Number);
                                                ValidateJsonValue("systemActions", listItemName, "width", areasItem, areasObject, JsonValueKind.Number);
                                                ValidateJsonValue("systemActions", listItemName, "height", areasItem, areasObject, JsonValueKind.Number);
                                            }
                                        }
                                    }
                                }
                                break;
                            case "drag":
                            case "exit":
                            case "enterloopcoordinate":
                            case "findclick":
                                break;
                            case "findclickandwait":
                                ValidateJsonValue("systemActions", listItemName, "ImageName", commandsItem, commandsObject, JsonValueKind.String);
                                ValidateJsonValue("systemActions", listItemName, "TimeOut", commandsItem, commandsObject, JsonValueKind.Number);
                                break;
                            case "ifexists":
                            case "ifnotexists":
                            case "loopcoordinates":
                            case "loopuntilfound":
                            case "loopuntilnotfound":
                            case "restart":
                            case "runaction":
                            case "sleep":
                            case "startgame":
                            case "stopgame":
                            case "waitfor":
                            case "waitforthenclick":
                            case "waitforchange":
                            case "waitfornochange":
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void ValidateJsonValue(string location, string listItemName, string Key, JsonNode jsonNode, JsonObject jsonObject, JsonValueKind jsonType)
        {
            if (!jsonObject.ContainsKey(Key))
            {
                Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is missing required field \"{3}\"", location, listItemName, jsonNode.GetPath(), Key));
            }
            else
            {
                if (!(jsonObject[Key] is JsonValue))
                {
                    Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting {3} but found {4}", location, listItemName, jsonObject[Key].GetPath(), jsonType, jsonObject[Key].GetType()));
                }
                else
                {
                    JsonElement itemNode = jsonObject[Key].GetValue<JsonElement>();
                    if (itemNode.ValueKind != jsonType)
                        Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting {3} but found {4}", location, listItemName, jsonObject[Key].GetPath(), jsonType, itemNode.ValueKind));
                }
            }
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
                                        ValidateJsonValue("Coordinates", coordItem.Key, "X", arrayItem, arrayObject, JsonValueKind.Number);
                                        ValidateJsonValue("Coordinates", coordItem.Key, "Y", arrayItem, arrayObject, JsonValueKind.Number);
                                    }
                                    else
                                    {
                                        if (!(arrayItem is JsonValue))
                                        {
                                            Errors.Add(string.Format("Coordinates list item {0} at path {1} is of the wrong type.  Was expecting Object, but found {2}", arrayItem.ToJsonString(), arrayItem.GetPath(), arrayItem.GetType()));
                                        }
                                        else
                                        {
                                            JsonElement jsonElement = arrayItem.GetValue<JsonElement>();
                                            Errors.Add(string.Format("Coordinates list item {0} at path {1} is of the wrong type.  Was expecting Object, but found {2}", arrayItem.ToJsonString(), arrayItem.GetPath(), jsonElement.ValueKind));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!(coordItem.Value is JsonValue))
                                {
                                    Errors.Add(string.Format("Coordinates list item {0} at path {1} is of the wrong type.  Was expecting Object, but found {2}", coordItem.Value.ToJsonString(), coordItem.Value.GetPath(), coordItem.Value.GetType()));
                                }
                                else
                                {
                                    JsonElement jsonElement = coordItem.Value.GetValue<JsonElement>();
                                    Errors.Add(string.Format("Coordinates item {0} at path {1} is of the wrong type.  Was expecting Array, but found {2}", coordItem.Key, coordItem.Value.GetPath(), jsonElement.ValueKind));
                                }
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
