﻿// <copyright file="JsonHelper.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

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
        public List<string> Errors { get; private set; }
        public enum ConfigFileType
        {
            Error,
            GameConfig,
            ListConfig,
            DeviceConfig
        }

        public JsonHelper()
        {
            Errors = new List<string>();
        }


        /// <summary>
        /// Determine the supposed content type of the json file
        /// </summary>
        /// <param name="jsonFileName">The name of the json file to check.</param>
        /// <returns>Error or the type of json content in the file.</returns>
        public ConfigFileType GetFileType(string jsonFileName)
        {
            ConfigFileType returnValue = ConfigFileType.Error;
            JsonDocumentOptions documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };
            JsonNodeOptions nodeOptions = new JsonNodeOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string jsonList = File.ReadAllText(jsonFileName);
            try
            {
                JsonNode jsonListNode = JsonNode.Parse(jsonList, nodeOptions, documentOptions);
                if (jsonListNode is JsonObject jsonObject)
                {
                    if (!jsonObject.ContainsKey("FileId"))
                    {
                        Errors.Add("Required field \"FileId\" missing.  Unable to determine that the input file is of a known type");
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
                                if (Enum.TryParse(fileId, true, out returnValue))
                                {
                                    switch (returnValue)
                                    {
                                        case ConfigFileType.GameConfig:
                                        case ConfigFileType.ListConfig:
                                        case ConfigFileType.DeviceConfig:
                                            return returnValue;
                                        case ConfigFileType.Error:
                                        default:
                                            Errors.Add(string.Format("\"FileId\" indicates that this is not \"DeviceConfig\", \"GameConfig\", or \"ListConfig\" but {0}", fileId));
                                            break;
                                    }
                                }
                                else
                                    Errors.Add(string.Format("\"FileId\" indicates that this is not \"DeviceConfig\", \"GameConfig\", or \"ListConfig\" but {0}", fileId));
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
                }
            }
            catch (JsonException ex)
            {
                Errors.Add(string.Format("File {0} is not well formed JSON.  Error {1} captured.", jsonFileName, ex.Message));
            }
            return returnValue;
        }

        /// <summary>
        /// Validates that the main config file is structured correctly, and has no data type issues.
        /// Ignores extra data in the file that isn't used by the game engine.
        /// </summary>
        /// <param name="jsonGameFileName">The full path to the Json file to be checked.</param>
        /// <returns>True when there are no errors.</returns>
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
                PropertyNameCaseInsensitive = true
            };

            string jsonList = File.ReadAllText(jsonGameFileName);
            try
            {
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
                                if (Enum.TryParse(fileId, true, out ConfigFileType configFileType))
                                {
                                    if (configFileType != ConfigFileType.GameConfig)
                                        Errors.Add(string.Format("\"FileId\" indicates that this is not \"GameConfig\" but {0}", fileId));
                                }
                                else
                                    Errors.Add(string.Format("\"FileId\" indicates that this is not \"GameConfig\" but {0}", fileId));
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

                    if (!jsonObject.ContainsKey("FindStrings"))
                    {
                        Errors.Add("Required root field \"FindStrings\" missing.");
                    }
                    else
                    {
                        JsonNode rootNode = jsonObject["FindStrings"];
                        if (!(rootNode is JsonObject findStringsObject))
                        {
                            Errors.Add("Required root field \"FindStrings\" is of the wrong type.  Expecting an Object with one or more named objects of FindStrings data");
                        }
                        else
                        {
                            foreach (KeyValuePair<string, JsonNode?> findStringsItem in findStringsObject)
                            {
                                if (!(findStringsItem.Value is JsonObject findStringJsonObject))
                                {
                                    Errors.Add(string.Format("FindStrings item {0} at path {1} is of the wrong type.  Was expecting Object with zero or more named findString data", findStringsItem.Key, findStringsItem.Value.GetPath()));
                                }
                                else
                                {
                                    // Check for required fields.
                                    ValidateJsonValue("FindStrings", findStringsItem.Key, "SearchString", findStringJsonObject, JsonValueKind.String);
                                    if (ValidateJsonValue("FindStrings", findStringsItem.Key, "searchArea", findStringJsonObject, "JsonObject", "containing a search area"))
                                    {
                                        JsonObject searchAreaObject = (JsonObject)findStringJsonObject["searchArea"];
                                        ValidateJsonValue("FindStrings", findStringsItem.Key, "X", searchAreaObject, JsonValueKind.Number);
                                        ValidateJsonValue("FindStrings", findStringsItem.Key, "Y", searchAreaObject, JsonValueKind.Number);
                                        ValidateJsonValue("FindStrings", findStringsItem.Key, "width", searchAreaObject, JsonValueKind.Number);
                                        ValidateJsonValue("FindStrings", findStringsItem.Key, "height", searchAreaObject, JsonValueKind.Number);
                                    }
                                    ValidateJsonValue("FindStrings", findStringsItem.Key, "textTolerance", findStringJsonObject, JsonValueKind.Number);
                                    ValidateJsonValue("FindStrings", findStringsItem.Key, "backgroundTolerance", findStringJsonObject, JsonValueKind.Number);
                                }
                            }
                        }
                    }

                    if (!jsonObject.ContainsKey("SystemActions"))
                    {
                        Errors.Add("Required root field \"SystemActions\" missing.");
                    }
                    else
                    {
                        JsonNode rootNode = jsonObject["SystemActions"];
                        if (!(rootNode is JsonObject systemActionsObject))
                        {
                            Errors.Add("Required root field \"SystemActions\" is of the wrong type.  Expecting an Object with one or more named objects of Action data");
                        }
                        else
                        {
                            foreach (KeyValuePair<string, JsonNode?> systemActionsItem in systemActionsObject)
                            {
                                if (!(systemActionsItem.Value is JsonObject systemActionJsonObject))
                                {
                                    Errors.Add(string.Format("SystemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Object with zero or more named objects contained within.", systemActionsItem.Key, systemActionsItem.Value.GetPath()));
                                }
                                else
                                {
                                    // Check for required fields.
                                    if (!systemActionJsonObject.ContainsKey("ActionType"))
                                    {
                                        Errors.Add(string.Format("SystemActions list item \"{0}\" at path {1} is missing required field \"ActionType\"", systemActionsItem.Key, systemActionsItem.Value.GetPath()));
                                    }
                                    else
                                    {
                                        if (!(systemActionJsonObject["ActionType"] is JsonValue))
                                        {
                                            Errors.Add(string.Format("SystemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", systemActionsItem.Key, systemActionJsonObject["ActionType"].GetPath(), systemActionJsonObject["ActionType"].GetType()));
                                        }
                                        else
                                        {
                                            JsonElement itemNode = systemActionJsonObject["ActionType"].GetValue<JsonElement>();
                                            if (itemNode.ValueKind != JsonValueKind.String)
                                                Errors.Add(string.Format("SystemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", systemActionsItem.Key, systemActionJsonObject["ActionType"].GetPath(), itemNode.ValueKind));
                                            else
                                            {
                                                switch (itemNode.GetString().ToLower())
                                                {
                                                    case "system":
                                                        break;
                                                    default:
                                                        Errors.Add(string.Format("SystemActions list item \"{0}\" at path {1} with value \"{2}\" is not valid.  Was expecting \"System\"", systemActionsItem.Key, systemActionJsonObject["ActionType"].GetPath(), itemNode.GetString()));
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    if (!systemActionJsonObject.ContainsKey("Commands"))
                                    {
                                        Errors.Add(string.Format("SystemActions list item \"{0}\" at path {1} is missing required field \"Commands\"", systemActionsItem.Key, systemActionsItem.Value.GetPath()));
                                    }
                                    else
                                    {
                                        if (!(systemActionJsonObject["Commands"] is JsonArray commandsJsonArray))
                                        {
                                            Errors.Add(string.Format("SystemActions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Array but found {2}", systemActionsItem.Key, systemActionJsonObject["Commands"].GetPath(), systemActionJsonObject["Commands"].GetType()));
                                        }
                                        else
                                        {
                                            ValidateCommands("SystemActions", systemActionsItem.Key, commandsJsonArray);
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
                                if (!(actionsItem.Value is JsonObject actionJsonObject))
                                {
                                    JsonElement jsonElement = actionsItem.Value.GetValue<JsonElement>();
                                    Errors.Add(string.Format("FindStrings item {0} at path {1} is of the wrong type.  Was expecting Object, but found {2}", actionsItem.Key, actionsItem.Value.GetPath(), jsonElement.ValueKind));
                                }
                                else
                                {
                                    // Check for required fields.
                                    if (!actionJsonObject.ContainsKey("ActionType"))
                                    {
                                        Errors.Add(string.Format("actions list item \"{0}\" at path {1} is missing required field \"ActionType\"", actionsItem.Key, actionsItem.Value.GetPath()));
                                    }
                                    else
                                    {
                                        if (!(actionJsonObject["ActionType"] is JsonValue))
                                        {
                                            Errors.Add(string.Format("actions list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", actionsItem.Key, actionJsonObject["ActionType"].GetPath(), actionJsonObject["ActionType"].GetType()));
                                        }
                                        else
                                        {
                                            JsonElement itemNode = actionJsonObject["ActionType"].GetValue<JsonElement>();
                                            if (itemNode.ValueKind != JsonValueKind.String)
                                                Errors.Add(string.Format("actions list item \"{0}\" at path {1} is of the wrong type.  Was expecting String but found {2}", actionsItem.Key, actionJsonObject["ActionType"].GetPath(), itemNode.ValueKind));
                                            else
                                            {
                                                if (!Enum.TryParse(itemNode.GetString(), true, out ValidActionType validActionType))
                                                    Errors.Add(string.Format("actions list item \"{0}\" at path {1} with value \"{2}\" is not valid.  Was expecting one of the following \"System\", \"Scheduled\", \"Daily\", \"Adhoc\", \"Always\"", actionsItem.Key, actionJsonObject["ActionType"].GetPath(), itemNode.GetString()));
                                            }
                                        }
                                    }
                                    if (!actionJsonObject.ContainsKey("Commands"))
                                    {
                                        Errors.Add(string.Format("actions list item \"{0}\" at path {1} is missing required field \"Commands\"", actionsItem.Key, actionsItem.Value.GetPath()));
                                    }
                                    else
                                    {
                                        if (!(actionJsonObject["Commands"] is JsonArray commandsJsonArray))
                                        {
                                            Errors.Add(string.Format("actions list item \"{0}\" at path {1} is of the wrong type.  Was expecting Array but found {2}", actionsItem.Key, actionJsonObject["Commands"].GetPath(), actionJsonObject["Commands"].GetType()));
                                        }
                                        else
                                        {
                                            ValidateCommands("actions", actionsItem.Key, commandsJsonArray);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Errors.Add(string.Format("Root of {0} was not a json Object, was expecting {\"FileId\": \"ListConfig\",", jsonGameFileName));
                }
            }
            catch (JsonException ex)
            {
                Errors.Add(string.Format("File {0} is not well formed JSON.  Error {1} captured.", jsonGameFileName, ex.Message));
            }


            if (Errors.Count == startErrorCount)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Validates that the commands are of a valid name, and required parameters are provided and of the correct type
        /// </summary>
        /// <param name="location">This is the type of Action (systemAction, scheduledAction)</param>
        /// <param name="listItemName">The name of the Action that this command is within</param>
        /// <param name="commandsJsonArray">The JsonArray that holds all the commands to parse</param>
        private void ValidateCommands(string location, string listItemName, JsonArray commandsJsonArray)
        {
            string Key;
            BotEngine.ValidCommandIds validCommandIds = BotEngine.ValidCommandIds.Exit;

            foreach (JsonNode commandsItem in commandsJsonArray)
            {
                if (!(commandsItem is JsonObject commandsObject))
                {
                    if (!(commandsItem is JsonValue))
                        Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting Array but found {3}", location, listItemName, commandsItem.GetPath(), commandsItem.GetType()));
                    else
                        Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting Array but found {3}", location, listItemName, commandsItem.GetPath(), commandsItem.GetValue<JsonElement>().ValueKind));
                }
                else
                {
                    ValidateJsonValue(location, listItemName, "CommandNumber", commandsObject, JsonValueKind.Number);
                    string CommandId = string.Empty;
                    Key = "CommandId";
                    if (!commandsObject.ContainsKey(Key))
                    {
                        Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is missing required field \"{3}\"", location, listItemName, commandsItem.GetPath(), Key));
                    }
                    else
                    {
                        if (!(commandsObject[Key] is JsonValue))
                        {
                            Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting String but found {3}", location, listItemName, commandsObject[Key].GetPath(), commandsObject[Key].GetType()));
                        }
                        else
                        {
                            JsonElement itemNode = commandsObject[Key].GetValue<JsonElement>();
                            if (itemNode.ValueKind != JsonValueKind.String)
                                Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting String but found {3}", location, listItemName, commandsObject[Key].GetPath(), itemNode.ValueKind));
                            else
                            {
                                if (Enum.TryParse(itemNode.GetString(), true, out validCommandIds))
                                {
                                    CommandId = itemNode.GetString();
                                }
                                else
                                {
                                    CommandId = string.Empty;
                                    Errors.Add(string.Format("{0} list item \"{1}\" at path {2} with value \"{3}\" is not valid.  Was expecting a Command like one of the following \"WaitFor\", \"Click\", \"IfExists\", \"FindClickAndWait\"", location, listItemName, commandsObject[Key].GetPath(), itemNode.GetString()));
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(CommandId))
                    {
                        switch (validCommandIds)
                        {
                            case BotEngine.ValidCommandIds.Click:
                                Key = "Location";
                                if (ValidateJsonValue(location, listItemName, Key, commandsObject, "JsonObject", "with X/Y"))
                                {
                                    if (ValidateJsonValue(location, listItemName, commandsObject[Key], "JsonObject", "with X/Y"))
                                    {
                                        ValidateJsonValue(location, listItemName, "X", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                        ValidateJsonValue(location, listItemName, "Y", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                    }
                                }
                                break;
                            case BotEngine.ValidCommandIds.ClickWhenNotFoundInArea:
                                ValidateJsonValue(location, listItemName, "ImageName", commandsObject, JsonValueKind.String);
                                Key = "Areas";
                                if (ValidateJsonValue(location, listItemName, Key, commandsObject, "JsonArray", "with X/Y/width/height objects"))
                                {
                                    foreach (JsonNode areasItem in commandsObject[Key].AsArray())
                                    {
                                        if (ValidateJsonValue(location, listItemName, areasItem, "JsonObject", "with X/Y/width/height objects"))
                                        {
                                            JsonObject areasObject = (JsonObject)areasItem;
                                            ValidateJsonValue(location, listItemName, "X", areasObject, JsonValueKind.Number);
                                            ValidateJsonValue(location, listItemName, "Y", areasObject, JsonValueKind.Number);
                                            ValidateJsonValue(location, listItemName, "width", areasObject, JsonValueKind.Number);
                                            ValidateJsonValue(location, listItemName, "height", areasObject, JsonValueKind.Number);
                                        }
                                    }
                                }
                                break;
                            case BotEngine.ValidCommandIds.Drag:
                                ValidateJsonValue(location, listItemName, "Delay", commandsObject, JsonValueKind.Number);
                                Key = "Swipe";
                                if (ValidateJsonValue(location, listItemName, Key, commandsObject, "JsonObject", "with X1/Y1/X2/Y2 objects"))
                                {
                                    ValidateJsonValue(location, listItemName, "X1", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                    ValidateJsonValue(location, listItemName, "Y1", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                    ValidateJsonValue(location, listItemName, "X2", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                    ValidateJsonValue(location, listItemName, "Y2", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                }
                                break;
                            case BotEngine.ValidCommandIds.Exit:
                                break;
                            case BotEngine.ValidCommandIds.EnterLoopCoordinate:
                            case BotEngine.ValidCommandIds.EnterText:
                                ValidateJsonValue(location, listItemName, "Value", commandsObject, JsonValueKind.String);
                                break;
                            case BotEngine.ValidCommandIds.FindClick:
                                ValidateJsonValue(location, listItemName, "ImageName", commandsObject, JsonValueKind.String);
                                if (commandsObject.ContainsKey("IgnoreMissing"))
                                    ValidateJsonValue(location, listItemName, "IgnoreMissing", commandsObject, JsonValueKind.True);
                                break;
                            case BotEngine.ValidCommandIds.FindClickAndWait:
                                if (commandsObject.ContainsKey("ImageNames"))
                                {
                                    if (ValidateJsonValue(location, listItemName, "ImageNames", commandsObject, "JsonArray", "with one or more Strings"))
                                    {
                                        foreach (JsonNode imageItem in commandsObject["ImageNames"].AsArray())
                                        {
                                            ValidateJsonValue(location, listItemName, imageItem, "JsonValueKind.String", string.Empty);
                                        }
                                    }
                                }
                                else
                                    ValidateJsonValue(location, listItemName, "ImageName", commandsObject, JsonValueKind.String);
                                ValidateJsonValue(location, listItemName, "TimeOut", commandsObject, JsonValueKind.Number);
                                if (commandsObject.ContainsKey("IgnoreMissing"))
                                    ValidateJsonValue(location, listItemName, "IgnoreMissing", commandsObject, JsonValueKind.True);
                                break;
                            case BotEngine.ValidCommandIds.IfExists:
                            case BotEngine.ValidCommandIds.IfNotExists:
                                ValidateJsonValue(location, listItemName, "ImageName", commandsObject, JsonValueKind.String);
                                Key = "Commands";
                                if (ValidateJsonValue(location, listItemName, Key, commandsObject, "JsonArray", "with one or more Command objects"))
                                {
                                    ValidateCommands(location, listItemName, commandsObject[Key].AsArray());
                                }
                                break;
                            case BotEngine.ValidCommandIds.LoopCoordinates:
                                ValidateJsonValue(location, listItemName, "Coordinates", commandsObject, JsonValueKind.String);
                                Key = "Commands";
                                if (ValidateJsonValue(location, listItemName, Key, commandsObject, "JsonArray", "with one or more Command objects"))
                                {
                                    ValidateCommands(location, listItemName, commandsObject[Key].AsArray());
                                }
                                break;
                            case BotEngine.ValidCommandIds.LoopUntilFound:
                            case BotEngine.ValidCommandIds.LoopUntilNotFound:
                                if (commandsObject.ContainsKey("ImageNames"))
                                {
                                    if (ValidateJsonValue(location, listItemName, "ImageNames", commandsObject, "JsonArray", "with one or more Strings"))
                                    {
                                        foreach (JsonNode imageItem in commandsObject["ImageNames"].AsArray())
                                        {
                                            ValidateJsonValue(location, listItemName, imageItem, "JsonValueKind.String", string.Empty);
                                        }
                                    }
                                }
                                else
                                    ValidateJsonValue(location, listItemName, "ImageName", commandsObject, JsonValueKind.String);
                                ValidateJsonValue(location, listItemName, "TimeOut", commandsObject, JsonValueKind.Number);
                                Key = "Commands";
                                if (ValidateJsonValue(location, listItemName, Key, commandsObject, "JsonArray", "with one or more Command objects"))
                                {
                                    ValidateCommands(location, listItemName, commandsObject[Key].AsArray());
                                }
                                if (commandsObject.ContainsKey("IgnoreMissing"))
                                    ValidateJsonValue(location, listItemName, "IgnoreMissing", commandsObject, JsonValueKind.True);
                                break;
                            case BotEngine.ValidCommandIds.Restart:
                                break;
                            case BotEngine.ValidCommandIds.RunAction:
                                ValidateJsonValue(location, listItemName, "ActionName", commandsObject, JsonValueKind.String);
                                break;
                            case BotEngine.ValidCommandIds.Sleep:
                                ValidateJsonValue(location, listItemName, "Delay", commandsObject, JsonValueKind.Number);
                                break;
                            case BotEngine.ValidCommandIds.StartGame:
                            case BotEngine.ValidCommandIds.StopGame:
                                ValidateJsonValue(location, listItemName, "TimeOut", commandsObject, JsonValueKind.Number);
                                ValidateJsonValue(location, listItemName, "Value", commandsObject, JsonValueKind.String);
                                break;
                            case BotEngine.ValidCommandIds.WaitFor:
                                if (commandsObject.ContainsKey("ImageNames"))
                                {
                                    if (ValidateJsonValue(location, listItemName, "ImageNames", commandsObject, "JsonArray", "with one or more Strings"))
                                    {
                                        foreach (JsonNode imageItem in commandsObject["ImageNames"].AsArray())
                                        {
                                            ValidateJsonValue(location, listItemName, imageItem, "JsonValueKind.String", string.Empty);
                                        }
                                    }
                                }
                                else
                                    ValidateJsonValue(location, listItemName, "ImageName", commandsObject, JsonValueKind.String);
                                ValidateJsonValue(location, listItemName, "TimeOut", commandsObject, JsonValueKind.Number);
                                if (commandsObject.ContainsKey("IgnoreMissing"))
                                    ValidateJsonValue(location, listItemName, "IgnoreMissing", commandsObject, JsonValueKind.True);
                                break;
                            case BotEngine.ValidCommandIds.WaitForThenClick:
                                ValidateJsonValue(location, listItemName, "ImageName", commandsObject, JsonValueKind.String);
                                ValidateJsonValue(location, listItemName, "TimeOut", commandsObject, JsonValueKind.Number);
                                break;
                            case BotEngine.ValidCommandIds.WaitForChange:
                            case BotEngine.ValidCommandIds.WaitForNoChange:
                                ValidateJsonValue(location, listItemName, "TimeOut", commandsObject, JsonValueKind.Number);
                                ValidateJsonValue(location, listItemName, "ChangeDetectDifference", commandsObject, JsonValueKind.Number);
                                Key = "ChangeDetectArea";
                                if (ValidateJsonValue(location, listItemName, Key, commandsObject, "JsonObject", "with X/Y/width/height objects"))
                                {
                                    ValidateJsonValue(location, listItemName, "X", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                    ValidateJsonValue(location, listItemName, "Y", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                    ValidateJsonValue(location, listItemName, "width", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                    ValidateJsonValue(location, listItemName, "height", commandsObject[Key].AsObject(), JsonValueKind.Number);
                                }
                                break;
                            case BotEngine.ValidCommandIds.LoopCounter:
                                ValidateJsonValue(location, listItemName, "Value", commandsObject, JsonValueKind.String);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Validates a json Key entry exists in a Json Object, and is of the correct type.
        /// </summary>
        /// <param name="location">This is the type of Action (systemAction, scheduledAction)</param>
        /// <param name="listItemName">The name of the Action that this JsonObject is within</param>
        /// <param name="Key">The Key that identifies the entry within the JsonObject to check</param>
        /// <param name="jsonObject">The JsonObject that may contain the Key</param>
        /// <param name="jsonType">The type that is expected</param>
        private bool ValidateJsonValue(string location, string listItemName, string Key, JsonObject jsonObject, JsonValueKind jsonType)
        {
            if (!jsonObject.ContainsKey(Key))
            {
                Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is missing required field \"{3}\"", location, listItemName, jsonObject.GetPath(), Key));
                return false;
            }
            else
            {
                if (jsonObject[Key] != null)
                    if (!(jsonObject[Key] is JsonValue))
                    {
                        Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting {3} but found {4}", location, listItemName, jsonObject[Key].GetPath(), jsonType, jsonObject[Key].GetType()));
                        return false;
                    }
                    else
                    {
                        JsonElement itemNode = jsonObject[Key].GetValue<JsonElement>();
                        if (itemNode.ValueKind != jsonType)
                        {
                            if (!(itemNode.ValueKind == JsonValueKind.False && jsonType == JsonValueKind.True))
                            {
                                Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting {3} but found {4}", location, listItemName, jsonObject[Key].GetPath(), jsonType, itemNode.ValueKind));
                                return false;
                            }
                        }
                    }
            }
            return true;
        }

        /// <summary>
        /// Validates a json Key entry exists in a Json Object, and is of the correct type.
        /// </summary>
        /// <param name="location">This is the type of Action (systemAction, scheduledAction)</param>
        /// <param name="listItemName">The name of the Action that this JsonObject is within</param>
        /// <param name="Key">The Key that identifies the entry within the JsonObject to check</param>
        /// <param name="jsonObject">The JsonObject that may contain the Key</param>
        /// <param name="jsonType">The string at the end of the type that is expected</param>
        /// <param name="jsonTypeExtraInfo">Extra text to include in the error message</param>
        /// <returns></returns>
        private bool ValidateJsonValue(string location, string listItemName, string Key, JsonObject jsonObject, string jsonType, string jsonTypeExtraInfo)
        {
            if (!jsonObject.ContainsKey(Key))
            {
                Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is missing required field \"{3}\"", location, listItemName, jsonObject.GetPath(), Key));
                return false;
            }
            else
            {
                if (!jsonObject[Key].GetType().ToString().EndsWith(jsonType))
                {
                    if (!(jsonObject[Key] is JsonValue))
                    {
                        Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting {3} {4} but found {5}", location, listItemName, jsonObject[Key].GetPath(), jsonType, jsonTypeExtraInfo, jsonObject[Key].GetType()));
                        return false;
                    }
                    else
                    {
                        JsonElement itemNode = jsonObject[Key].GetValue<JsonElement>();
                        Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting {3} {4} but found {5}", location, listItemName, jsonObject[Key].GetPath(), jsonType, jsonTypeExtraInfo, itemNode.ValueKind));
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Validates that a JsonNode is of the correct type.
        /// </summary>
        /// <param name="location">This is the type of Action (systemAction, scheduledAction)</param>
        /// <param name="listItemName">The name of the Action that this JsonObject is within</param>
        /// <param name="jsonNode">The JsonNode that is being validated</param>
        /// <param name="jsonType">The string at the end of the type that is expected</param>
        /// <param name="jsonTypeExtraInfo">Extra text to include in the error message</param>
        /// <returns></returns>
        private bool ValidateJsonValue(string location, string listItemName, JsonNode jsonNode, string jsonType, string jsonTypeExtraInfo)
        {
            if (jsonNode is JsonValue && jsonType.StartsWith("JsonValueKind"))
            {
                JsonElement itemNode = jsonNode.GetValue<JsonElement>();
                if (!jsonType.ToLower().EndsWith(itemNode.ValueKind.ToString().ToLower()))
                {
                    Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting {3} {4} but found {5}", location, listItemName, jsonNode.GetPath(), jsonType, jsonTypeExtraInfo, itemNode.ValueKind));
                    return false;
                }
            }
            else
            if (!jsonNode.GetType().ToString().EndsWith(jsonType))
            {
                if (!(jsonNode is JsonValue))
                {
                    Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting {3} {4} but found {5}", location, listItemName, jsonNode.GetPath(), jsonType, jsonTypeExtraInfo, jsonNode.GetType()));
                    return false;
                }
                else
                {
                    JsonElement itemNode = jsonNode.GetValue<JsonElement>();
                    Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting {3} {4} but found {5}", location, listItemName, jsonNode.GetPath(), jsonType, jsonTypeExtraInfo, itemNode.ValueKind));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validates that a json List File has the correct data structure.
        /// </summary>
        /// <param name="jsonListFileName"></param>
        /// <returns></returns>
        public bool ValidateListConfigStructure(string jsonListFileName)
        {
            int startErrorCount = Errors.Count;
            JsonDocumentOptions documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };
            JsonNodeOptions nodeOptions = new JsonNodeOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string jsonList = File.ReadAllText(jsonListFileName);
            try
            {
                JsonNode jsonListNode = JsonNode.Parse(jsonList, nodeOptions, documentOptions);
                if (jsonListNode is JsonObject jsonObject)
                {
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
                                if (Enum.TryParse(fileId, true, out ConfigFileType configFileType))
                                {
                                    if (configFileType != ConfigFileType.ListConfig)
                                        Errors.Add(string.Format("\"FileId\" indicates that this is not \"ListConfig\" but {0}", fileId));
                                }
                                else
                                    Errors.Add(string.Format("\"FileId\" indicates that this is not \"ListConfig\" but {0}", fileId));
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
                        if (rootNode is JsonObject coordinatesObject)
                        {
                            foreach (KeyValuePair<string, JsonNode?> coordItem in coordinatesObject)
                            {
                                if (coordItem.Value is JsonArray)
                                {
                                    JsonArray coordArray = coordItem.Value.AsArray();
                                    foreach (JsonNode arrayItem in coordArray)
                                    {
                                        if (arrayItem is JsonObject arrayObject)
                                        {
                                            ValidateJsonValue("Coordinates", coordItem.Key, "X", arrayObject, JsonValueKind.Number);
                                            ValidateJsonValue("Coordinates", coordItem.Key, "Y", arrayObject, JsonValueKind.Number);
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
            }
            catch (JsonException ex)
            {
                Errors.Add(string.Format("File {0} is not well formed JSON.  Error {1} captured.", jsonListFileName, ex.Message));
            }

            if (Errors.Count == startErrorCount)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Validates that a device config file is valid structurally.  Also checks that Dates are valid.
        /// </summary>
        /// <param name="jsonDeviceFileName"></param>
        /// <returns></returns>
        public bool ValidateDeviceConfigStructure(string jsonDeviceFileName)
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

            string jsonList = File.ReadAllText(jsonDeviceFileName);
            try
            {
                JsonNode jsonListNode = JsonNode.Parse(jsonList, nodeOptions, documentOptions);
                if (jsonListNode is JsonObject jsonObject)
                {
                    if (!jsonObject.ContainsKey("FileId"))
                    {
                        Errors.Add("Required field \"FileId\" missing.  Unable to confirm that the input file is a DeviceConfigFile");
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
                                if (Enum.TryParse(fileId, true, out ConfigFileType configFileType))
                                {
                                    if (configFileType != ConfigFileType.DeviceConfig)
                                        Errors.Add(string.Format("\"FileId\" indicates that this is not \"DeviceConfig\" but {0}", fileId));
                                }
                                else
                                    Errors.Add(string.Format("\"FileId\" indicates that this is not \"DeviceConfig\" but {0}", fileId));
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

                    if (!jsonObject.ContainsKey("LastActionTaken"))
                    {
                        Errors.Add("Required field \"LastActionTaken\" missing.");
                    }
                    else
                    {
                        JsonNode rootNode = jsonObject["LastActionTaken"];
                        if (rootNode is JsonObject lastActionTakenObject)
                        {
                            foreach (KeyValuePair<string, JsonNode?> lastActionItem in lastActionTakenObject)
                            {
                                if (ValidateJsonValue("LastActionTaken", lastActionItem.Key, lastActionItem.Value, "JsonObject", "with at least LastRun, ActionEnabled"))
                                {
                                    if (ValidateJsonValue("LastActionTaken", lastActionItem.Key, "LastRun", lastActionItem.Value.AsObject(), JsonValueKind.String))
                                    {
                                        if (!lastActionItem.Value.AsObject()["LastRun"].AsValue().TryGetValue<DateTime>(out DateTime temp))
                                        {
                                            Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting DateTime but found {3}", "LastActionTaken", lastActionItem.Key, lastActionItem.Value.AsObject()["LastRun"].GetPath(), lastActionItem.Value.AsObject()["LastRun"].AsValue()));
                                        }
                                    }
                                    ValidateJsonValue("LastActionTaken", lastActionItem.Key, "ActionEnabled", lastActionItem.Value.AsObject(), JsonValueKind.True);
                                    if (lastActionItem.Value.AsObject().ContainsKey("Frequency"))
                                        ValidateJsonValue("LastActionTaken", lastActionItem.Key, "Frequency", lastActionItem.Value.AsObject(), JsonValueKind.Number);
                                    if (lastActionItem.Value.AsObject().ContainsKey("DailyScheduledTime"))
                                    {
                                        if (ValidateJsonValue("LastActionTaken", lastActionItem.Key, "DailyScheduledTime", lastActionItem.Value.AsObject(), JsonValueKind.String))
                                        {
                                            if (lastActionItem.Value.AsObject()["DailyScheduledTime"] != null)
                                            {
                                                if (!lastActionItem.Value.AsObject()["DailyScheduledTime"].AsValue().TryGetValue<DateTime>(out DateTime temp))
                                                {
                                                    Errors.Add(string.Format("{0} list item \"{1}\" at path {2} is of the wrong type.  Was expecting DateTime but found {3}", "LastActionTaken", lastActionItem.Key, lastActionItem.Value.AsObject()["DailyScheduledTime"].GetPath(), lastActionItem.Value.AsObject()["DailyScheduledTime"].AsValue()));
                                                }
                                            }
                                        }
                                    }
                                    if (lastActionItem.Value.AsObject().ContainsKey("CommandLoopStatus"))
                                    {
                                        JsonNode commandStatusNode = lastActionItem.Value.AsObject()["CommandLoopStatus"];
                                        if (commandStatusNode is JsonObject commandStatusObject)
                                        {
                                            foreach (KeyValuePair<string, JsonNode?> commandStatusItem in commandStatusObject)
                                            {
                                                ValidateJsonValue("LastActionTaken", lastActionItem.Key, commandStatusItem.Key, commandStatusObject, JsonValueKind.String);
                                            }
                                        }
                                        else
                                        {
                                            if (commandStatusNode != null)
                                                Errors.Add("Optional field \"CommandStatus\" is of the wrong type.  Expecting an Object with one or more named objects");
                                        }
                                    }
                                    if (lastActionItem.Value.AsObject().ContainsKey("CommandValueOverride"))
                                    {
                                        JsonNode commandValueOverrideNode = lastActionItem.Value.AsObject()["CommandValueOverride"];
                                        if (commandValueOverrideNode is JsonObject commandStatusObject)
                                        {
                                            foreach (KeyValuePair<string, JsonNode?> commandValueOverrideItem in commandStatusObject)
                                            {
                                                ValidateJsonValue("LastActionTaken", lastActionItem.Key, commandValueOverrideItem.Key, commandStatusObject, JsonValueKind.String);
                                            }
                                        }
                                        else
                                        {
                                            if (commandValueOverrideNode != null)
                                                Errors.Add("Optional field \"CommandStatus\" is of the wrong type.  Expecting an Object with one or more named objects");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Errors.Add("Required field \"LastActionTaken\" is of the wrong type.  Expecting an Object with one or more named objects of at least LastRun, ActionEnabled");
                        }
                    }
                }
                else
                {
                    Errors.Add(string.Format("Root of {0} was not a json Object, was expecting {\"FileId\": \"DeviceConfig\",", jsonDeviceFileName));
                }
            }
            catch (JsonException ex)
            {
                Errors.Add(string.Format("File {0} is not well formed JSON.  Error {1} captured.", jsonDeviceFileName, ex.Message));
            }
            if (Errors.Count == startErrorCount)
                return true;
            else
                return false;
        }
    }
}
