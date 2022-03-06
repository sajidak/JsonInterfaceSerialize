using System;
using System.Text;

namespace JsonInterfaceSerialize.Utilities.Helpers
{
    /// <summary>
    /// Utility functions to parse and format Exception details to string
    /// Needed to augment limited error reporting capabilities of Azure Functions v3 default logger.
    /// </summary>
    public class ExceptionHelpers
    {
        #region Generate exception with deep stack trace
        public static void GenerateErrorDeep() { GenerateError_l1(); }
        private static void GenerateError_l1() { GenerateError_l2(); }
        private static void GenerateError_l2() { GenerateError_l3(); }
        private static void GenerateError_l3() { GenerateError_l4(); }
        private static void GenerateError_l4()
        {
            string lsVar1 = null; _ = lsVar1.Trim();
        }
        #endregion Generate exception with deep stack trace

        /// <summary>
        /// Extracts relevant information from an exception to a string, for logging and analysis.
        /// each entry is delimited by ' | ', for formating convenience.
        /// </summary>
        /// <param name="se">The exception to parse/serialize</param>
        /// <param name="note">Informational note, to add any context, for help in analysis</param>
        /// <returns>String, with lines delimited by ' | '</returns>
        public static string SerializeExceptionTxt(System.Exception se, string note = "")
        {
            StringBuilder lsbErr = new StringBuilder();
            try
            {
                if (string.IsNullOrWhiteSpace(note)) note = "No note given by caller.";
                lsbErr.Append(string.Format("Note:    {0} | ", note));
                lsbErr.Append(string.Format("Message: {0} | ", se.Message));
                lsbErr.Append(string.Format("HResult: {0} | ", se.HResult));
                lsbErr.Append(string.Format("Source:  {0} | ", se.Source));

                if (!string.IsNullOrWhiteSpace(se.StackTrace))
                {
                    string[] lsStackEntries = se.StackTrace.Replace("\r", "").Split("\n");
                    int liPosTag = 0, liPosCut = 0;

                    lsbErr.Append("StackTrace: | ");
                    foreach (string ve in lsStackEntries)
                    {
                        liPosTag = ve.IndexOf(" in ");
                        if (liPosTag >= 0) liPosCut = ve.IndexOf(se.Source, liPosTag);
                        if (liPosTag >= liPosCut) liPosCut = liPosTag + 4; // if exception is from external library
                        if (liPosTag >= 0)
                            lsbErr.AppendFormat("  {0} | ", ve[liPosCut..].Replace(":line", " at line"));
                    }
                }
                // if (!string.IsNullOrWhiteSpace(se.StackTrace)) lsbErr.Append(SerializeStackTrace(se.StackTrace, se.Source));

                if (se.InnerException != null)
                {
                    lsbErr.Append("InnerException: BEGIN | ");
                    lsbErr.Append(SerializeExceptionTxt(se.InnerException));
                    lsbErr.Append("InnerException: FINIS | ");
                }
            }
            catch (Exception ise) { lsbErr.AppendLine("Error parsing exception: " + ise.Message); }
            return lsbErr.ToString();
        }

        private static string SerializeStackTrace(string strace, string source)
        {
            StringBuilder lsbErr = new StringBuilder();
            try
            {
                string[] lsStackEntries = strace.Replace("\r", "").Split("\n");
                int liPosTag = 0, liPosCut = 0;
                lsbErr.Append("StackTrace: | ");
                foreach (string ve in lsStackEntries)
                {
                    liPosTag = ve.IndexOf(" in ");
                    if (liPosTag >= 0) liPosCut = ve.IndexOf(source, liPosTag);
                    if (liPosTag >= liPosCut) liPosCut = liPosTag + 4; // if exception is from external library
                    if (liPosTag >= 0)
                        lsbErr.AppendFormat("  {0} | ", ve[liPosCut..].Replace(":line", " at line"));
                }
            }
            catch (Exception ise) { lsbErr.AppendLine("Error parsing stack trace: " + ise.Message); }
            return lsbErr.ToString();
        }

    }
}
