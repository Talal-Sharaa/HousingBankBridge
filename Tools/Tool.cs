using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ZagAPIServer.Tools
{
    internal static class Tool
    {
        //*************************************************************
        /// <summary>
        /// Converts a string to a DateTime object with the specified format.
        /// </summary>
        /// <param name="vValue">The object that calls the function.</param>
        /// <param name="vFormat">The desired format of the DateTime object.</param>
        /// <param name="vReValue">The value to return if the conversion fails.</param>
        /// <returns>A formatted DateTime object representation of the string. </returns>
        public static DateTime Nvl(this object vValue, DateTime vReValue, string vFormat = "yyyyMMdd")
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToDateTime(Convert.ToString(vValue));
                }
                catch (FormatException)
                {
                    DateTime xMyDate;
                    if (!DateTime.TryParseExact(Convert.ToString(vValue), vFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out xMyDate))
                    {
                        // handle parse failure
                        return vReValue;
                    }
                    return xMyDate;
                }
        }

        //*************************************************************
        /// <summary>
        /// Converts a string to a nullable DateTime object with the specified format.
        /// </summary>
        /// <param name="vValue">The object that calls the function.</param>
        /// <param name="vFormat">The desired format of the DateTime object.</param>
        /// <param name="vReValue">The value to return if the conversion fails.</param>
        /// <returns>A formatted nullable DateTime object representation of the string. </returns>
        public static DateTime? Nvl(this object vValue, DateTime? vReValue, string vFormat = "yyyyMMdd")
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToDateTime(Convert.ToString(vValue));
                }
                catch (FormatException)
                {
                    DateTime xMyDate;
                    if (!DateTime.TryParseExact(Convert.ToString(vValue), vFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out xMyDate))
                    {
                        // handle parse failure
                        return vReValue;
                    }
                    return xMyDate;
                }
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>Int8</returns>
        public static byte Nvl(this object vValue, byte vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToByte(vValue);

                }
                catch (FormatException)
                {
                    return Convert.ToByte(Convert.ToDouble(vValue));

                }
        }
        //*************************************************************
        /// <summary>Convert current object to bool</summary>
        /// <param name="vValue">this object</param>
        /// <param name="vReValue">bool</param>
        /// <returns>bool</returns>
        public static bool Nvl(this object vValue, bool vReValue)
        {
            if (vValue == null)
                return Convert.ToBoolean(vReValue);//Convert.ToInt32(vReValue);
            else if (vValue is DBNull)
                return Convert.ToBoolean(vReValue);//Convert.ToInt32(vReValue);

            else
                return Convert.ToBoolean(vValue);
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>Int8</returns>
        public static sbyte Nvl(this object vValue, sbyte vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToSByte(vValue);

                }
                catch (FormatException)
                {
                    return Convert.ToSByte(Convert.ToDouble(vValue));

                }
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>Int16</returns>
        public static Int16 Nvl(this object vValue, Int16 vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToInt16(vValue);

                }
                catch (FormatException)
                {
                    return Convert.ToInt16(Convert.ToDouble(vValue));

                }
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>int</returns>
        public static Int32 Nvl(this object vValue, int vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToInt32(vValue);

                }
                catch (FormatException)
                {
                    return Convert.ToInt32(Convert.ToDouble(vValue));

                }
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>Int64</returns>
        public static Int64 Nvl(this object vValue, Int64 vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToInt64(vValue);

                }
                catch (FormatException)
                {
                    return Convert.ToInt64(Convert.ToDouble(vValue));

                }
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>UInt8</returns>
        public static byte UNvl(this object vValue, byte vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToByte(vValue);

                }
                catch (FormatException)
                {
                    return Convert.ToByte(Convert.ToDouble(vValue));

                }
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>UInt16</returns>
        public static UInt16 UNvl(this object vValue, UInt16 vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToUInt16(vValue);

                }
                catch (FormatException)
                {
                    return Convert.ToUInt16(Convert.ToDouble(vValue));

                }
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>UInt32</returns>
        public static UInt32 UNvl(this object vValue, UInt32 vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToUInt32(vValue);

                }
                catch (FormatException)
                {
                    return Convert.ToUInt32(Convert.ToDouble(vValue));

                }
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>UInt64</returns>
        public static UInt64 UNvl(this object vValue, UInt64 vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                try
                {
                    return Convert.ToUInt64(vValue);

                }
                catch (FormatException)
                {
                    return Convert.ToUInt64(Convert.ToDouble(vValue));

                }
        }
        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>double</returns>
        public static double Nvl(this object vValue, double vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else if (vValue.ToString() == "double.NegativeInfinity")
                return double.NegativeInfinity;
            else if (vValue.ToString() == "double.PositiveInfinity")
                return double.PositiveInfinity;
            else
                return Convert.ToDouble(vValue);
        }
        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>decimal</returns>
        public static decimal Nvl(this object vValue, decimal vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else if (vValue.ToString() == "∞")
                return decimal.MaxValue;
            else if (vValue.ToString() == "-∞")
                return decimal.MinValue;
            else
                return Convert.ToDecimal(vValue);
        }
        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2022-05-22
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>float</returns>
        public static float Nvl(this object vValue, float vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                return (float)Convert.ToDouble(vValue);
        }

        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2016-01-24
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>string</returns>
        public static string Nvl(this object vValue, string vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                return Convert.ToString(vValue);
        }
        //*************************************************************
        /// <summary>
        /// Powered by Shaheen Al-Habash 2022-03-22
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vReValue"></param>
        /// <returns>char</returns>
        public static char Nvl(this object vValue, char vReValue)
        {
            if (vValue == null)
                return vReValue;
            else if (vValue is System.DBNull)
                return vReValue;
            else if (vValue.ToString() == "")
                return vReValue;
            else
                return Convert.ToChar(Convert.ToByte(vValue));
        }
        //*************************************************************
        public static string MaskCustIdAndChildren(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
            {
                return rawInput;
            }

            // The start of the actual XML content. We'll look for this tag.
            const string xmlStartTag = "<FIXML";
            string prefix = string.Empty;
            string xmlString = rawInput;

            // Step 1: Find the beginning of the XML content to handle dynamic prefixes.
            int xmlStartIndex = rawInput.IndexOf(xmlStartTag, StringComparison.Ordinal);

            if (xmlStartIndex > 0)
            {
                // If the XML tag is found and it's not at the very beginning,
                // we have a dynamic prefix to preserve.
                prefix = rawInput.Substring(0, xmlStartIndex);
                xmlString = rawInput.Substring(xmlStartIndex);
            }
            else if (xmlStartIndex == -1)
            {
                // If the XML start tag is not found, we cannot process the string.
                // Return the original input as-is.
                Console.WriteLine("Warning: XML start tag '<FIXML' not found. Returning original string.");
                return rawInput;
            }

            try
            {
                // Step 2: Parse the isolated XML string into an XDocument.
                XDocument doc = XDocument.Parse(xmlString);

                // Step 3: Define the XML namespace. This is crucial for finding elements correctly.
                XNamespace ns = "http://www.finacle.com/fixml";

                // Step 4: Find the specific <CustId> element that acts as a container.
                // Based on the requirement, we identify this as a <CustId> element that contains another <CustId> element.
                var custIdContainer = doc.Descendants(ns + "CustId")
                                         .FirstOrDefault(e => e.Elements(ns + "CustId").Any());

                if (custIdContainer != null)
                {
                    // Step 5: Mask the target container element and all elements within it (descendants).
                    MaskElementAndDescendants(custIdContainer);
                }

                // NEW: Mask <AcctName> and <AcctShortName> inside any <AcctGenInfo> blocks
                foreach (var acctGenInfo in doc.Descendants(ns + "AcctGenInfo"))
                {
                    var acctNameEl = acctGenInfo.Element(ns + "AcctName");
                    if (acctNameEl != null && !acctNameEl.HasElements && !string.IsNullOrWhiteSpace(acctNameEl.Value))
                    {
                        acctNameEl.Value = MaskValue(acctNameEl.Value);
                    }

                    var acctShortNameEl = acctGenInfo.Element(ns + "AcctShortName");
                    if (acctShortNameEl != null && !acctShortNameEl.HasElements && !string.IsNullOrWhiteSpace(acctShortNameEl.Value))
                    {
                        acctShortNameEl.Value = MaskValue(acctShortNameEl.Value);
                    }
                }

                // Step 6: Return the modified XML as a string, prepending the original dynamic prefix.
                return $"{prefix}{doc.ToString(SaveOptions.DisableFormatting)}";
            }
            catch (Exception ex)
            {
                // In case of a parsing error, return an error message.
                return $"Error processing XML: {ex.Message}";
            }
        }
        //******************************************************************
        /// <summary>
        /// A helper method to recursively mask an element and all its children.
        /// </summary>
        /// <param name="element">The starting XElement to mask.</param>
        private static void MaskElementAndDescendants(XElement element)
        {
            // First, get all descendant elements from the starting element
            var elementsToMask = element.Descendants().ToList();
            // And also add the starting element itself to the list
            elementsToMask.Add(element);

            foreach (var el in elementsToMask)
            {
                // We only want to mask elements that contain a direct text value
                // and not just other elements.
                if (!string.IsNullOrEmpty(el.Value) && !el.HasElements)
                {
                    el.Value = MaskValue(el.Value);
                }
            }
        }
        //******************************************************************
        /// <summary>
        /// Applies the masking rule to a single string value.
        /// "123456789" becomes "12****89".
        /// </summary>
        /// <param name="value">The string to mask.</param>
        /// <returns>The masked string.</returns>
        private static string MaskValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return "********";
        }
        //***********************************************************
    }
}
