//
// report.cs: report errors and warnings.
//
// Author: Miguel de Icaza (miguel@ximian.com)
//
// (C) 2001 Ximian, Inc. (http://www.ximian.com)
//

//
// FIXME: currently our class library does not support custom number format strings
//
using System;
using System.Collections;
using System.Diagnostics;

namespace Mono.CSharp {

	/// <summary>
	///   This class is used to report errors and warnings t te user.
	/// </summary>
	public class Report {
		/// <summary>  
		///   Errors encountered so far
		/// </summary>
		static public int Errors;

		/// <summary>  
		///   Warnings encountered so far
		/// </summary>
		static public int Warnings;

		/// <summary>  
		///   Whether errors should be throw an exception
		/// </summary>
		static public bool Fatal;
		
		/// <summary>  
		///   Whether warnings should be considered errors
		/// </summary>
		static public bool WarningsAreErrors;

		/// <summary>  
		///   Whether to dump a stack trace on errors. 
		/// </summary>
		static public bool Stacktrace;
		
		//
		// If the error code is reported on the given line,
		// then the process exits with a unique error code.
		//
		// Used for the test suite to excercise the error codes
		//
		static int probe_error = 0;

		//
		// Keeps track of the warnings that we are ignoring
		//
		static Hashtable warning_ignore_table;
		
		static void Check (int code)
		{
			if (code ==  probe_error){
				Environment.Exit (123);
			}
		}
		
		static public void RealError (string msg)
		{
			Errors++;
			Console.WriteLine (msg);

			if (Stacktrace)
				Console.WriteLine (new StackTrace ().ToString ());
			if (Fatal)
				throw new Exception (msg);
		}

		static public void Error (int code, Location l, string text)
		{
			string msg = String.Format (
//				"{0}({1}) error CS{2:0000}: {3}", l.Name, l.Row, code, text);
				"{0}({1}) error CS{2}: {3}", l.Name, l.Row, code, text);
			
			RealError (msg);
			Check (code);
		}

		static public void Warning (int code, Location l, string text)
		{
			if (warning_ignore_table != null){
				if (warning_ignore_table.Contains (code))
					return;
			}
			
			if (WarningsAreErrors)
				Error (code, l, text);
			else {
				string row;
				
				if (Location.IsNull (l))
					row = "";
				else
					row = l.Row.ToString ();
				
				Console.WriteLine (String.Format (
//					"{0}({1}) warning CS{2:0000}: {3}",
					"{0}({1}) warning CS{2}: {3}",
					l.Name,  row, code, text));
				Warnings++;
				Check (code);
			}
		}
		
		static public void Warning (int code, string text)
		{
			Warning (code, Location.Null, text);
		}

		static public void Error (int code, string text)
		{
//			string msg = String.Format ("error CS{0:0000}: {1}", code, text);
			string msg = String.Format ("error CS{0}: {1}", code, text);
			
			RealError (msg);
			Check (code);
		}

		static public void Message (Message m)
		{
			if (m is ErrorMessage)
				Error (m.code, m.text);
			else
				Warning (m.code, m.text);
		}

		static public void SetIgnoreWarning (int code)
		{
			if (warning_ignore_table == null)
				warning_ignore_table = new Hashtable ();

			warning_ignore_table [code] = true;
		}
		
		static public void SetProbe (int code)
		{
			probe_error = code;
		}

		static public int ProbeCode {
			get {
				return probe_error;
			}
		}
	}

	public class Message {
		public int code;
		public string text;
		
		public Message (int code, string text)
		{
			this.code = code;
			this.text = text;
		}
	}

	public class WarningMessage : Message {
		public WarningMessage (int code, string text) : base (code, text)
		{
		}
	}

	public class ErrorMessage : Message {
		public ErrorMessage (int code, string text) : base (code, text)
		{
		}

		//
		// For compatibility reasons with old code.
		//
		public static void report_error (string error)
		{
			Console.Write ("ERROR: ");
			Console.WriteLine (error);
		}
	}
}


