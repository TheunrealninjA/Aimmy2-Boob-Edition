using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Aimmy2.Other
{
    internal class KeybindNameManager
    {
        public static readonly Dictionary<string, string> KeybindNames = new()
        {
            { "D1", "1" },
            { "D2", "2" },
            { "D3", "3" },
            { "D4", "4" },
            { "D5", "5" },
            { "D6", "6" },
            { "D7", "7" },
            { "D8", "8" },
            { "D9", "9" },
            { "D0", "0" },
            { "OemMinus", "-" },
            { "OemPlus", "+" },
            { "Back", "Backspace" },
            { "Oem5", "Backslash (\\)" },
            { "LMenu", "Left Alt" },
            { "RMenu", "Right Alt" },
            { "RShiftKey", "Right Shift" },
            { "LShiftKey", "Left Shift" },
            { "Oem4", "Left Bracket" },
            { "OemOpenBrackets", "Left Bracket" },
            { "Oem6", "Right Bracket" },
            { "Oem1", ";" },
            { "Oem3", "`" },
            { "OemQuotes", "'" },
            { "OemQuestion", "/" },
            { "OemPeriod", "." },
            { "OemComma", "," },
            { "Return", "Enter" },
        };

        public static string ConvertToRegularKey(string keyName)
        {
            try
            {
                KeyConverter kc = new();
                // Convert the keyName to Key enum
                if (Enum.TryParse(keyName, true, out Key key))
                {
                    // Get the display name from the dictionary
                    return KeybindNames.TryGetValue(key.ToString(), out var displayName) ? displayName : key.ToString();
                }
                else
                {
                    // Key name is invalid
                    FileManager.LogError($"Invalid key name: {keyName}");
                    return keyName;
                }
            }
            catch (Exception ex)
            {
                FileManager.LogError("Failed to grab keybind (most likely a missing keybind from dictionary) " + ex);
                return keyName;
            }
        }
    }
}
